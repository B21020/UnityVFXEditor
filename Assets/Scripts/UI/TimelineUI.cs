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
        bool _suppress;

        void Awake()
        {
            tc = GetComponent<TimeController>();
            if(playButton!=null) playButton.onClick.AddListener(()=> tc.Play());
            if(pauseButton!=null) pauseButton.onClick.AddListener(()=> tc.Pause());
            if(slider!=null) slider.onValueChanged.AddListener(OnSlider);
        }

        void OnDestroy()
        {
            if (slider != null)
                slider.onValueChanged.RemoveListener(OnSlider);
        }

        void Update()
        {
            if (tc == null || slider == null) return;

            var dur = tc.Duration;
            if (dur <= 0.0) return;

            // 現在時刻に応じてスライダーを更新（再生中/停止中どちらも）
            _suppress = true;
            slider.value = (float)(tc.CurrentTime / dur);
            _suppress = false;

            // ラベル: 現在 / 総時間
            if (timeLabel != null)
                timeLabel.text = string.Format("{0} / {1}", FormatTime(tc.CurrentTime), FormatTime(dur));
        }

        void OnSlider(float value01)
        {
            if (_suppress) return;
            if (tc == null) return;

            var dur = tc.Duration;
            if (dur <= 0.0) return;

            // 0..1 を秒へ変換してシーク
            tc.Seek((float)(value01 * dur));
        }

        string FormatTime(double s)
        {
            if (s < 0) s = 0;
            int totalSeconds = (int)s;
            int m = totalSeconds / 60;
            int sec = totalSeconds % 60;
            return string.Format("{0:00}:{1:00}", m, sec);
        }
    }
}
