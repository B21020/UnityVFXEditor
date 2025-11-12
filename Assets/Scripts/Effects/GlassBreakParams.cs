using System;
using UnityEngine;

namespace UnityVFXEditor.Effects
{
    [Serializable]
    public class GlassBreakParams
    {
        public float positionZ = 0f;
        public Vector2 breakOriginUV = new Vector2(0.5f, 0.5f);
        public int shardCount = 12;
        public float explosionForce = 1f;
        public Vector3 mainDirection = new Vector3(0f, 1f, 0f);
    }
}
