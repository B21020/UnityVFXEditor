using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityVFXEditor.Core;
using UnityVFXEditor.Effects;

namespace UnityVFXEditor.Core
{
    public class PickManager : MonoBehaviour
    {
        public Camera cam;
        public GameObject videoPlane; // assign in scene generator
        public GameObject pickOverlayPrefab; // small X mark

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ev = EventSystem.current;
                if (ev != null && ev.IsPointerOverGameObject()) return; // skip UI
                Ray r = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(r, out var hit))
                {
                    if (hit.collider.gameObject == videoPlane)
                    {
                        var local = videoPlane.transform.InverseTransformPoint(hit.point);
                        var mr = videoPlane.GetComponent<MeshRenderer>();
                        // assuming quad scaled uniformly, convert local x/z to 0..1
                        var uvx = local.x + 0.5f;
                        var uvy = local.y + 0.5f;
                        var id = ProjectManager.Instance?.scheduled.Count>0 ? ProjectManager.Instance.scheduled[ProjectManager.Instance.scheduled.Count-1].id : null;
                        if (id!=null)
                        {
                            var p = ProjectManager.Instance.GetParams(id);
                            if (p!=null) { p.breakOriginUV = new Vector2(uvx, uvy); ProjectManager.Instance.SetParams(id, p); }
                        }
                        // overlay
                        if (pickOverlayPrefab)
                        {
                            var ov = Instantiate(pickOverlayPrefab, hit.point, Quaternion.identity);
                            ov.transform.SetParent(videoPlane.transform, true);
                        }
                    }
                }
            }
        }
    }
}
