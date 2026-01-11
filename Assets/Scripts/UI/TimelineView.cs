using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class TimelineView : MonoBehaviour, IPointerClickHandler
    {
        public RectTransform markerLayer;
        public Slider timeSlider;
        public Button playButton;
        public Button pauseButton;
        public Text timeLabel;
        public GameObject markerPrefab; // optional

        public double Duration => timeController ? timeController.Duration : 1.0;

        TimeController timeController;
        Dictionary<string, GameObject> markerMap = new Dictionary<string, GameObject>();

        void Awake()
        {
            timeController = GetComponent<TimeController>() ?? GetComponent<TimeController>();
            if (playButton) playButton.onClick.AddListener(()=> timeController?.Play());
            if (pauseButton) pauseButton.onClick.AddListener(()=> timeController?.Pause());
            if (timeSlider) timeSlider.onValueChanged.AddListener(OnSlider);
            var pm = ProjectManager.Instance;
            if (pm != null)
            {
                pm.OnScheduleChanged += RebuildMarkers;
                pm.OnSelectionChanged += OnSelectionChanged;
            }
        }

        void OnDestroy()
        {
            var pm = ProjectManager.Instance;
            if (pm!=null)
            {
                pm.OnScheduleChanged -= RebuildMarkers;
                pm.OnSelectionChanged -= OnSelectionChanged;
            }
        }

        void Update()
        {
            if (timeController==null) return;
            if (timeLabel) timeLabel.text = FormatTime(timeController.CurrentTime);
            if (timeSlider && Duration>0) timeSlider.value = (float)(timeController.CurrentTime / Duration);
        }

        string FormatTime(double s)
        {
            var ts = System.TimeSpan.FromSeconds(s);
            return string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (markerLayer==null) return;
            Vector2 local;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(markerLayer, eventData.position, eventData.pressEventCamera, out local)) return;
            float norm = (local.x + markerLayer.rect.width * 0.5f) / markerLayer.rect.width;
            norm = Mathf.Clamp01(norm);
            var time = Duration * norm;
            var id = ProjectManager.Instance?.AddEffect(time);
            if (id != null) CreateMarker(id, norm);
        }

        void OnSlider(float v)
        {
            if (timeController==null) return;
            var t = v * (float)Duration;
            timeController.Seek(t);
        }

        void RebuildMarkers()
        {
            // destroy existing
            foreach(var kv in markerMap) { if (kv.Value) Destroy(kv.Value); }
            markerMap.Clear();
            if (ProjectManager.Instance==null) return;
            foreach(var s in ProjectManager.Instance.scheduled)
            {
                float norm = Duration>0 ? (float)(s.timeSec / Duration) : 0f;
                CreateMarker(s.id, norm);
            }
        }

        void CreateMarker(string id, float norm)
        {
            if (markerLayer==null) return;
            GameObject go;
            if (markerPrefab!=null) go = Instantiate(markerPrefab, markerLayer);
            else
            {
                go = new GameObject("Marker", typeof(RectTransform));
                var img = go.AddComponent<Image>(); img.color = Color.red;
                var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(10, 30);
            }
            go.transform.SetParent(markerLayer, false);
            var rt2 = go.GetComponent<RectTransform>();
            float x = norm * markerLayer.rect.width - markerLayer.rect.width*0.5f;
            rt2.anchoredPosition = new Vector2(x, 0);

            var drag = go.AddComponent<MarkerDrag>();
            drag.effectId = id;
            drag.parentLayer = markerLayer;
            drag.timelineView = this;

            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(()=> { ProjectManager.Instance?.SelectEffect(id); });

            markerMap[id] = go;
        }

        void OnSelectionChanged(string id)
        {
            // highlight selected marker
            foreach(var kv in markerMap)
            {
                var img = kv.Value.GetComponent<Image>();
                img.color = kv.Key == id ? Color.yellow : Color.red;
            }
        }
    }
}
