using UnityEngine;
using UnityEngine.UI;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    [RequireComponent(typeof(TimeController))]
    public class TimelineUI : MonoBehaviour
    {
        public Button playButton;
        public Button pauseButton;
        public Slider  slider;
        public Text    timeLabel;

        TimeController tc;

        void Awake()
        {
            tc = GetComponent<TimeController>();
            if(playButton!=null) playButton.onClick.AddListener(()=> tc.Play());
            if(pauseButton!=null) pauseButton.onClick.AddListener(()=> tc.Pause());
            if(slider!=null) slider.onValueChanged.AddListener(OnSlider);
        }

        void Update()
        {
            if(tc==null) return;
            if(tc.Duration>0)
            {
                if(!slider.IsActive() || !slider.interactable) { slider.value = (float)(tc.CurrentTime / tc.Duration); }
                if(timeLabel) timeLabel.text = FormatTime(tc.CurrentTime);
            }
        }

        void OnSlider(float v)
        {
            if(tc==null) return;
            var t = v * (float)tc.Duration;
            tc.Seek(t);
        }

        string FormatTime(double s)
        {
            var ts = System.TimeSpan.FromSeconds(s);
            return string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
        }
    }
}
