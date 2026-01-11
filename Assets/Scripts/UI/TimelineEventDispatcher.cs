using System.Collections.Generic;
using UnityEngine;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class TimelineEventDispatcher : MonoBehaviour
    {
        [SerializeField] private TimeController timeController;
        [SerializeField] private TimelineMarkerManager markerManager;

        // 1回だけ発火させるための記録
        private readonly HashSet<string> _firedMarkerIds = new HashSet<string>();

        private double _prevTime = 0.0;
        private bool _hasPrev = false;

        [Tooltip("判定誤差対策（秒）")]
        [SerializeField] private double epsilon = 0.0005;

        void Update()
        {
            if (timeController == null || markerManager == null) return;

            // 再生中だけ監視（UIドラッグ中のSeek等で発火させない）
            if (!timeController.IsPlaying) { _hasPrev = false; return; }

            double dur = timeController.Duration;
            if (dur <= 0.0) return;

            double now = timeController.CurrentTime;

            if (!_hasPrev)
            {
                _prevTime = now;
                _hasPrev = true;
                return;
            }

            // 時間が戻った（シーク/ループ等） → 発火済みをリセットして再判定の基準を作る
            if (now + epsilon < _prevTime)
            {
                _firedMarkerIds.Clear();
                _prevTime = now;
                return;
            }

            // 前回→今回で跨いだマーカーを発火
            var list = markerManager.GetAllMarkerData();
            for (int i = 0; i < list.Count; i++)
            {
                var m = list[i];
                if (m == null) continue;

                // 同一IDは1回のみ
                if (_firedMarkerIds.Contains(m.id)) continue;

                //  prev < markerTime <= now で発火
                if (_prevTime + epsilon < m.timeSec && m.timeSec <= now + epsilon)
                {
                    Fire(m);
                    _firedMarkerIds.Add(m.id);
                }
            }

            _prevTime = now;
        }

        private void Fire(MarkerData m)
        {
            // ここを後で MediaPipe担当の「姿勢推定結果」呼び出しに差し替える
            Debug.Log(
                $"[EVENT] id={m.id} t={m.timeSec:0.000}s preset={m.preset} " +
                $"strength={m.strength} depthZ={m.depthZ} " +
                $"throwDir=({m.throwDir.x:0.###},{m.throwDir.y:0.###}) breakOrigin={m.breakOrigin}"
            );
        }
        public void ResetForNewVideo()
        {
            _firedMarkerIds.Clear();
            _hasPrev = false;
        }
    }
}
