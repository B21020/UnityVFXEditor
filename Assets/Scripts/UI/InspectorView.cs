using UnityEngine;
using UnityEngine.UI;
using UnityVFXEditor.Core;
using UnityVFXEditor.Effects;

namespace UnityVFXEditor.UI
{
    public class InspectorView : MonoBehaviour
    {
        public Text selectedLabel;
        public InputField positionZInput;
        public Text uvLabel;

        string currentId;

        void Start()
        {
            if (ProjectManager.Instance!=null)
            {
                ProjectManager.Instance.OnSelectionChanged += OnSelectionChanged;
            }
            if (positionZInput) positionZInput.onEndEdit.AddListener(OnPositionZEdited);
        }

        void OnDestroy()
        {
            if (ProjectManager.Instance!=null) ProjectManager.Instance.OnSelectionChanged -= OnSelectionChanged;
        }

        void OnSelectionChanged(string id)
        {
            currentId = id;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (currentId==null) { if (selectedLabel) selectedLabel.text = "No selection"; return; }
            var s = ProjectManager.Instance.scheduled.Find(x=>x.id==currentId);
            if (s.id==null) { if (selectedLabel) selectedLabel.text = "No selection"; return; }
            if (selectedLabel) selectedLabel.text = string.Format("選択中: t={0:F2}s type={1}", s.timeSec, s.type);
            var p = ProjectManager.Instance.GetParams(currentId);
            if (p!=null)
            {
                if (positionZInput) positionZInput.text = p.positionZ.ToString("F2");
                if (uvLabel) uvLabel.text = string.Format("UV: {0:F2}, {1:F2}", p.breakOriginUV.x, p.breakOriginUV.y);
                // reflect positionZ to ZGauge if exists
                var gauge = GameObject.Find("ZGauge");
                if (gauge)
                {
                    var rt = gauge.GetComponent<RectTransform>();
                    if (rt && rt.parent!=null)
                    {
                        // map positionZ (-10..10) to x within parent width
                        float norm = Mathf.InverseLerp(-10f, 10f, p.positionZ);
                        var pw = ((RectTransform)rt.parent).rect.width;
                        rt.anchoredPosition = new Vector2(norm * pw - pw*0.5f, rt.anchoredPosition.y);
                    }
                }
            }
        }

        void OnPositionZEdited(string v)
        {
            if (currentId==null) return;
            if (!float.TryParse(v, out var f)) return;
            var p = ProjectManager.Instance.GetParams(currentId);
            if (p==null) return;
            p.positionZ = f;
            ProjectManager.Instance.SetParams(currentId, p);
            UpdateUI();
        }
    }
}
