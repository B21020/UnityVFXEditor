using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class MarkerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public string effectId;
        public RectTransform parentLayer;
        public RectTransform selfRect;
        public TimelineView timelineView;

        void Awake()
        {
            selfRect = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // select this marker
            ProjectManager.Instance?.SelectEffect(effectId);
        }

        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            if (parentLayer == null || selfRect == null) return;
            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentLayer, eventData.position, eventData.pressEventCamera, out local);
            var half = parentLayer.rect.width * 0.5f;
            // clamp x to parent width
            float x = Mathf.Clamp(local.x, -parentLayer.rect.width * 0.5f, parentLayer.rect.width * 0.5f);
            selfRect.anchoredPosition = new Vector2(x, selfRect.anchoredPosition.y);
            // convert to normalized 0..1
            float norm = (x + parentLayer.rect.width * 0.5f) / parentLayer.rect.width;
            var time = timelineView.Duration * norm;
            ProjectManager.Instance?.UpdateEffectTime(effectId, time);
        }

        public void OnEndDrag(PointerEventData eventData) { }
    }
}
