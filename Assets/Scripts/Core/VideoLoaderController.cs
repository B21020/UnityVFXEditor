using UnityEngine;
using UnityEngine.Video;
using UnityVFXEditor.UI;

namespace UnityVFXEditor.Core
{
    public class VideoLoaderController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private TimeController timeController;

        [Header("Optional")]
        [SerializeField] private TimelineMarkerManager markerManager;
        [SerializeField] private TimelineEventDispatcher dispatcher;
        [SerializeField] private UnityVFXEditor.UI.TimelineUIInitializer timelineUI;

        private bool _preparing;

        public bool IsPreparing => _preparing;
        public System.Action<string> OnStatusChanged;

        void Awake()
        {
            videoPlayer.prepareCompleted += OnPrepared;
        }

        void OnDestroy()
        {
            videoPlayer.prepareCompleted -= OnPrepared;
        }

        // --- 公開API：動画を差し替える ---
        public void LoadVideoUrl(string url)
        {
            if (_preparing) return;

            LockUI();
            _preparing = true;

            videoPlayer.Stop();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = url;

            videoPlayer.Prepare();
        }

        public void LoadVideoClip(VideoClip clip)
        {
            if (_preparing) return;

            LockUI();
            _preparing = true;

            videoPlayer.Stop();
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = clip;

            videoPlayer.Prepare();
        }

        // --- Prepare 完了 ---
        private void OnPrepared(VideoPlayer vp)
        {
            _preparing = false;

            // TimeController に再バインド（安全）
            timeController.SetVideoPlayer(vp);

            // Timeline / Marker / Event をリセット
            markerManager?.ResetForNewVideo();
            dispatcher?.ResetForNewVideo();

            UnlockUI();

            timelineUI?.InitAfterVideoPrepared();
        }

        private void LockUI()
        {
            // ここでは最低限ログでOK
            Debug.Log("[VideoLoader] Preparing video...");
            OnStatusChanged?.Invoke("Preparing...");
        }

        private void UnlockUI()
        {
            Debug.Log("[VideoLoader] Video ready.");
            OnStatusChanged?.Invoke("Ready");
        }
    }
}
