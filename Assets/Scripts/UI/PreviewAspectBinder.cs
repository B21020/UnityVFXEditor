using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace UnityVFXEditor.UI
{
    public class PreviewAspectBinder : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private AspectRatioFitter aspectFitter;

        void OnEnable()
        {
            if (videoPlayer != null)
                videoPlayer.prepareCompleted += OnPrepared;
        }

        void OnDisable()
        {
            if (videoPlayer != null)
                videoPlayer.prepareCompleted -= OnPrepared;
        }

        private void OnPrepared(VideoPlayer vp)
        {
            // texture から安全に取得（Prepare後なら基本取れる）
            var tex = vp.texture;
            if (tex == null || aspectFitter == null) return;

            float w = tex.width;
            float h = tex.height;
            if (w <= 0 || h <= 0) return;

            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            aspectFitter.aspectRatio = w / h;
        }
    }
}
