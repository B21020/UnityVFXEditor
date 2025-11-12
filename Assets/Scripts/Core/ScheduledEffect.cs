using System;

namespace UnityVFXEditor.Core
{
    [Serializable]
    public struct ScheduledEffect
    {
        public string id;
        public double timeSec;
        public string type;

        public ScheduledEffect(string id, double timeSec, string type)
        {
            this.id = id;
            this.timeSec = timeSec;
            this.type = type;
        }
    }
}
