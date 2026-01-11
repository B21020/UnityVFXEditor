using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class TimelineUIInitializer : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private TimeController timeController;

        [Header("UI")]
        [SerializeField] private Slider timelineSlider;   // 0..1 の正規化で使うのが楽
        [SerializeField] private TMP_Text currentTimeText;
        [SerializeField] private TMP_Text durationText;

        private bool _suppress;

        void Awake()
        {
            if (timelineSlider != null)
            {
                timelineSlider.minValue = 0f;
                timelineSlider.maxValue = 1f;
                timelineSlider.onValueChanged.AddListener(OnSliderChanged);
            }
        }

        void OnDestroy()
        {
            if (timelineSlider != null)
                timelineSlider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        // Video準備完了後に呼ぶ
        public void InitAfterVideoPrepared()
        {
            if (timeController == null) return;
            double dur = timeController.Duration;

            // まだ length が 0 の場合は安全に抜ける（Prepare直後に稀に起きる）
            if (dur <= 0.0) return;

            // 表示更新
            if (durationText) durationText.text = Format(dur);

            // time=0へ
            _suppress = true;
            if (timelineSlider) timelineSlider.value = 0f;
            _suppress = false;

            timeController.Seek(0.0);
            UpdateCurrentTimeUI(0.0);
        }

        // 再生中にUIを追従させたい場合：呼び出し側で定期的に呼ぶ（任意）
        public void RefreshWhilePlaying()
        {
            if (timeController == null || timelineSlider == null) return;
            double dur = timeController.Duration;
            if (dur <= 0.0) return;

            double t = timeController.CurrentTime;
            float v = Mathf.Clamp01((float)(t / dur));

            _suppress = true;
            timelineSlider.value = v;
            _suppress = false;

            UpdateCurrentTimeUI(t);
        }

        private void OnSliderChanged(float v01)
        {
            if (_suppress || timeController == null) return;

            double dur = timeController.Duration;
            if (dur <= 0.0) return;

            double t = v01 * dur;
            timeController.Seek(t);
            UpdateCurrentTimeUI(t);
        }

        private void UpdateCurrentTimeUI(double t)
        {
            if (currentTimeText) currentTimeText.text = Format(t);
        }

        private static string Format(double sec)
        {
            if (sec < 0) sec = 0;
            int s = (int)sec;
            int m = s / 60;
            s = s % 60;
            return $"{m:00}:{s:00}";
        }
    }
}
