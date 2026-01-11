using TMPro;
using UnityEngine;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class InspectorPanelBinder : MonoBehaviour
    {
        
        [Header("Common")]
        [SerializeField] private TMP_Dropdown presetDropdown;
        [SerializeField] private TMP_InputField strengthInput;
        [SerializeField] private TMP_InputField depthZInput;

        [Header("Groups")]
        [SerializeField] private GameObject glassGroup;
        [SerializeField] private GameObject throwGroup;

        [Header("GlassBreak")]
        [SerializeField] private TMP_Dropdown breakOriginDropdown;

        [Header("ThrowObject")]
        [SerializeField] private TMP_InputField dirXInput;
        [SerializeField] private TMP_InputField dirYInput;

        private MarkerData _bound;
        private bool _suppress;

        public void Bind(MarkerData data)
        {
            _bound = data;
            RefreshUI();
        }

        public void Unbind()
        {
            _bound = null;
            RefreshUI();
        }

        void Awake()
        {
            // Dropdown / Input のイベントを接続
            if (presetDropdown) presetDropdown.onValueChanged.AddListener(OnPresetChanged);
            if (strengthInput) strengthInput.onEndEdit.AddListener(_ => OnStrengthEdited());
            if (depthZInput) depthZInput.onEndEdit.AddListener(_ => OnDepthEdited());

            if (breakOriginDropdown) breakOriginDropdown.onValueChanged.AddListener(OnBreakOriginChanged);

            if (dirXInput) dirXInput.onEndEdit.AddListener(_ => OnDirEdited());
            if (dirYInput) dirYInput.onEndEdit.AddListener(_ => OnDirEdited());

            // 初期状態を反映（起動時に正しい表示にする）
            if (presetDropdown != null)
            {
                ApplyPresetVisibility(GetSelectedPreset());
            }
        }

        void OnDestroy()
        {
            if (presetDropdown) presetDropdown.onValueChanged.RemoveListener(OnPresetChanged);
            if (strengthInput) strengthInput.onEndEdit.RemoveAllListeners();
            if (depthZInput) depthZInput.onEndEdit.RemoveAllListeners();

            if (breakOriginDropdown) breakOriginDropdown.onValueChanged.RemoveListener(OnBreakOriginChanged);

            if (dirXInput) dirXInput.onEndEdit.RemoveAllListeners();
            if (dirYInput) dirYInput.onEndEdit.RemoveAllListeners();
        }

        private void RefreshUI()
        {
            _suppress = true;

            bool has = _bound != null;

            if (!has)
            {
                SetGroups(null);
                SetText(strengthInput, "");
                SetText(depthZInput, "");
                SetText(dirXInput, "");
                SetText(dirYInput, "");
                if (presetDropdown) presetDropdown.value = 0;
                if (breakOriginDropdown) breakOriginDropdown.value = 0;

                _suppress = false;
                return;
            }

            // preset
            if (presetDropdown) presetDropdown.value = (int)_bound.preset;

            // common
            SetText(strengthInput, _bound.strength.ToString("0.###"));
            SetText(depthZInput, _bound.depthZ.ToString("0.###"));

            // groups
            SetGroups(_bound.preset);

            // glass
            if (breakOriginDropdown) breakOriginDropdown.value = (int)_bound.breakOrigin;

            // throw
            SetText(dirXInput, _bound.throwDir.x.ToString("0.###"));
            SetText(dirYInput, _bound.throwDir.y.ToString("0.###"));

            _suppress = false;
        }

        private void SetGroups(PresetType? preset)
        {
            // enum から文字列名を経由して表示切り替えを行う
            var name = preset?.ToString() ?? "";
            ApplyPresetVisibility(name);
        }

        private string GetSelectedPreset()
        {
            if (presetDropdown == null) return "";
            return presetDropdown.options[presetDropdown.value].text;
        }

        private void ApplyPresetVisibility(string preset)
        {
            bool isGlass = preset == "GlassBreak";
            bool isThrow = preset == "ThrowObject" || preset == "Throw";

            if (glassGroup != null) glassGroup.SetActive(isGlass);
            if (throwGroup != null) throwGroup.SetActive(isThrow);

            // どちらにも一致しない場合は安全に両方表示（デバッグしやすい）
            if (!isGlass && !isThrow)
            {
                if (glassGroup != null) glassGroup.SetActive(true);
                if (throwGroup != null) throwGroup.SetActive(true);
            }
        }

        private static void SetText(TMP_InputField f, string s)
        {
            if (!f) return;
            f.SetTextWithoutNotify(s);
        }

        private void OnPresetChanged(int v)
        {
            if (_suppress) return;

            if (_bound != null)
            {
                _bound.preset = (PresetType)v;
                SetGroups(_bound.preset);
            }
            else
            {
                // バインド対象がない場合でも、プリセット選択に応じて表示のみ切り替える
                ApplyPresetVisibility(GetSelectedPreset());
            }
        }

        private void OnStrengthEdited()
        {
            if (_suppress || _bound == null || !strengthInput) return;
            if (float.TryParse(strengthInput.text, out var f)) _bound.strength = f;
            RefreshUI();
        }

        private void OnDepthEdited()
        {
            if (_suppress || _bound == null || !depthZInput) return;
            if (float.TryParse(depthZInput.text, out var f)) _bound.depthZ = f;
            RefreshUI();
        }

        private void OnBreakOriginChanged(int v)
        {
            if (_suppress || _bound == null) return;
            _bound.breakOrigin = (BreakOrigin)v;
        }

        private void OnDirEdited()
        {
            if (_suppress || _bound == null) return;

            float x = _bound.throwDir.x;
            float y = _bound.throwDir.y;

            if (dirXInput && float.TryParse(dirXInput.text, out var fx)) x = fx;
            if (dirYInput && float.TryParse(dirYInput.text, out var fy)) y = fy;

            _bound.throwDir = new Vector2(x, y);
            RefreshUI();
        }
    }
}
