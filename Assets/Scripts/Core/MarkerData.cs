using System;
using UnityEngine;

namespace UnityVFXEditor.Core
{
    public enum PresetType
    {
        GlassBreak,
        ThrowObject
    }

    public enum BreakOrigin
    {
        Center,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom
    }

    [Serializable]
    public class MarkerData
    {
        public string id;
        public double timeSec;

        public PresetType preset = PresetType.GlassBreak;

        // 共通（最小セット）
        public float strength = 10f;
        public float depthZ = 0f;

        // ThrowObject 用（最小セット）
        public Vector2 throwDir = new Vector2(1, 0);

        // GlassBreak 用（最小セット）
        public BreakOrigin breakOrigin = BreakOrigin.Center;
    }
}
