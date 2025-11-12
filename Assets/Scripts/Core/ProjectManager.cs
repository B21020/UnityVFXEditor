using System;
using System.Collections.Generic;
using UnityEngine;
using UnityVFXEditor.Effects;

namespace UnityVFXEditor.Core
{
    public class ProjectManager : MonoBehaviour
    {
        public static ProjectManager Instance { get; private set; }

        public List<ScheduledEffect> scheduled = new List<ScheduledEffect>();

        // simple param store per effect id
        public Dictionary<string, GlassBreakParams> effectParams = new Dictionary<string, GlassBreakParams>();

        public event Action<string> OnSelectionChanged; // id
        public event Action OnScheduleChanged;

        string selectedId;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        public string AddEffect(double timeSec, string type = "GlassBreak")
        {
            var id = Guid.NewGuid().ToString();
            var se = new ScheduledEffect(id, timeSec, type);
            scheduled.Add(se);
            effectParams[id] = new GlassBreakParams();
            OnScheduleChanged?.Invoke();
            return id;
        }

        public void UpdateEffectTime(string id, double timeSec)
        {
            for (int i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].id == id)
                {
                    var s = scheduled[i]; s.timeSec = timeSec; scheduled[i] = s; OnScheduleChanged?.Invoke(); return;
                }
            }
        }

        public void SelectEffect(string id)
        {
            selectedId = id;
            OnSelectionChanged?.Invoke(id);
        }

        public GlassBreakParams GetParams(string id)
        {
            if (id == null) return null;
            if (!effectParams.ContainsKey(id)) effectParams[id] = new GlassBreakParams();
            return effectParams[id];
        }

        public void SetParams(string id, GlassBreakParams p)
        {
            effectParams[id] = p;
        }
    }
}
