using UnityEngine;
using UnityEngine.Video;

namespace UnityVFXEditor.Core
{
    public class TimeController : MonoBehaviour
    {
        [SerializeField] VideoPlayer video;
        public bool IsPlaying { get; private set; }
        public double CurrentTime => video ? video.time : 0.0;
        public double Duration    => video && video.length > 0 ? video.length : 0.0;

        public void Play()  { if(video){ IsPlaying = true; video.Play(); } }
        public void Pause() { if(video){ IsPlaying = false; video.Pause(); } }
        public void Seek(double tSec) {
            if(!video) return;
            tSec = Mathf.Clamp((float)tSec, 0f, (float)Duration);
            video.time = tSec;
            if(!IsPlaying) video.Pause();
        }
    }
}
