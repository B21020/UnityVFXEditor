using UnityEngine;
using UnityEngine.EventSystems;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class MarkerDragHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform timelineBackground; // TimelineSlider/Background
        [SerializeField] private TimeController timeController;

        [SerializeField] private TimelineMarkerManager markerManager;

        private RectTransform _markerRt;
        private UnityEngine.UI.Image _img;
        private bool _dragging;

        public MarkerData Data { get; private set; }

        void Awake()
        {
            _markerRt = transform as RectTransform;
            _img = GetComponent<UnityEngine.UI.Image>();
        }

        public void Init(RectTransform bg, TimeController tc, TimelineMarkerManager mgr, MarkerData data)
        {
            timelineBackground = bg;
            timeController = tc;
            markerManager = mgr;
            Data = data;
        }

        public void SetSelected(bool selected)
        {
            if (_img == null) return;
            _img.color = selected ? Color.yellow : Color.white; // まず固定でOK
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _dragging = true;
            markerManager?.Select(this);
            eventData.Use(); // Sliderにイベントが流れないようにする
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging || timelineBackground == null || timeController == null || Data == null) return;

            // 画面座標 → Backgroundローカル座標
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    timelineBackground,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 local))
                return;

            float width = timelineBackground.rect.width;
            if (width <= 0f) return;

            // local.x は -width/2..+width/2 なので、そこにクランプ
            float x = Mathf.Clamp(local.x, -width * 0.5f, width * 0.5f);

            // マーカーを移動
            _markerRt.anchoredPosition = new Vector2(x, _markerRt.anchoredPosition.y);

            // x → 0..1 → 秒 に変換してシーク
            double dur = timeController.Duration;
            if (dur <= 0.0) return;

            float t01 = (x / width) + 0.5f;      // -w/2..+w/2 → 0..1
            double tSec = t01 * dur;

            Data.timeSec = tSec;
            timeController.Seek(tSec);

            markerManager?.NotifySelectedTimeChangedIfThis(this);

            eventData.Use();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _dragging = false;
            eventData.Use();
        }
    }
}
