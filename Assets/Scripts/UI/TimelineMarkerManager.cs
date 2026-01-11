using UnityEngine;
using UnityEngine.UI;
using TMPro;   // TimeText を TextMeshPro で扱う
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class TimelineMarkerManager : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private TimeController timeController;
        [SerializeField] private RectTransform timelineBackground; // Slider/Background
        [SerializeField] private RectTransform markersRoot;        // Markers
        [SerializeField] private RectTransform markerPrefab;       // Marker.prefab

        [SerializeField] private InspectorPanelBinder inspectorBinder;

        [SerializeField] private TMP_Text selectedMarkerTimeText;

        [Header("Selection Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private readonly System.Collections.Generic.List<UnityVFXEditor.Core.MarkerData> _markerDataList
            = new System.Collections.Generic.List<UnityVFXEditor.Core.MarkerData>();

        private MarkerDragHandle _selected;

        void Start()
        {
            Debug.Log("[MarkerManager] Start called");
        }

        public void Select(MarkerDragHandle marker)
        {
            if (_selected == marker) return;
            if (_selected != null) _selected.SetSelected(false);
            _selected = marker;
            if (_selected != null) _selected.SetSelected(true);

            UpdateSelectedTimeText();

            if (inspectorBinder != null)
            {
                if (_selected == null || _selected.Data == null) inspectorBinder.Unbind();
                else inspectorBinder.Bind(_selected.Data);
            }
        }

        public void NotifySelectedTimeChangedIfThis(MarkerDragHandle marker)
        {
            if (_selected != marker) return;

            UpdateSelectedTimeText();

            if (inspectorBinder != null && marker != null && marker.Data != null)
                inspectorBinder.Bind(marker.Data); // ざっくり再描画（最小実装）
        }

        private void UpdateSelectedTimeText()
        {
            if (selectedMarkerTimeText == null) return;

            if (_selected == null || _selected.Data == null)
            {
                selectedMarkerTimeText.text = "Time: --:--";
            }
            else
            {
                selectedMarkerTimeText.text = "Time: " + Format(_selected.Data.timeSec);
            }
        }

        private static string Format(double sec)
        {
            if (sec < 0) sec = 0;
            int s = (int)sec;
            int m = s / 60;
            s = s % 60;
            return $"{m:00}:{s:00}";
        }

        public System.Collections.Generic.IReadOnlyList<UnityVFXEditor.Core.MarkerData> GetAllMarkerData()
            => _markerDataList;

        public void ResetForNewVideo()
        {
            _markerDataList.Clear();
            // TODO: 必要であればここで既存の Marker GameObject を Destroy する
        }

        // Add Marker ボタンから呼ぶ
        public void AddMarkerAtCurrentTime()
        {
             Debug.Log("[Marker] AddMarkerAtCurrentTime called");
            if (timeController == null) return;
            if (markerPrefab == null || markersRoot == null) return;

            double duration = timeController.Duration;
            if (duration <= 0.0) return;

            var data = new UnityVFXEditor.Core.MarkerData
            {
                id = System.Guid.NewGuid().ToString("N"),
                timeSec = timeController.CurrentTime,
                preset = UnityVFXEditor.Core.PresetType.GlassBreak
            };

            _markerDataList.Add(data);

            float width = timelineBackground.rect.width;
            float t01 = (float)(data.timeSec / timeController.Duration);
            float x = (t01 - 0.5f) * width; // -width/2 .. +width/2 に変換（Thumbと同じ基準）

            RectTransform marker = Instantiate(markerPrefab, markersRoot);
            marker.anchoredPosition = new Vector2(x, marker.anchoredPosition.y);
            marker.SetAsLastSibling();

            var handle = marker.gameObject.AddComponent<MarkerDragHandle>();
            handle.Init(timelineBackground, timeController, this, data);
        }
    }
}
