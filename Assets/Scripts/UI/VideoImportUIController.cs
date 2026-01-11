using SFB;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityVFXEditor.Core;

namespace UnityVFXEditor.UI
{
    public class VideoImportUIController : MonoBehaviour
    {
        public enum SourceMode { Url = 0, File = 1 }

        [Header("Refs")]
        [SerializeField] private VideoLoaderController loader;

        [Header("UI")]
        [SerializeField] private TMP_Dropdown sourceDropdown;
        [SerializeField] private TMP_InputField pathInput;
        [SerializeField] private Button browseButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private TMP_Text statusText;

        void Awake()
        {
            if (sourceDropdown) sourceDropdown.onValueChanged.AddListener(_ => RefreshUI());
            if (loadButton) loadButton.onClick.AddListener(OnClickLoad);
            if (browseButton) browseButton.onClick.AddListener(OnClickBrowse);

            if (loader != null)
                loader.OnStatusChanged += SetStatus;

            RefreshUI();
            SetStatus("Idle");
        }

        void OnDestroy()
        {
            if (loader != null)
                loader.OnStatusChanged -= SetStatus;
        }

        void Update()
        {
            // preparing 中はUIを無効化（最低限のロック）
            bool preparing = loader != null && loader.IsPreparing;

            if (sourceDropdown) sourceDropdown.interactable = !preparing;
            if (pathInput) pathInput.interactable = !preparing;
            if (loadButton) loadButton.interactable = !preparing;
            if (browseButton) browseButton.interactable = !preparing && IsEditorBuild();
        }

        private void RefreshUI()
        {
            // Fileモード時は file:// を付ける想定をUI側で促す（必要ならプレースホルダ変更）
            if (statusText == null) return;
        }

        private void OnClickLoad()
        {
            if (loader == null || pathInput == null) return;

            var mode = (SourceMode)(sourceDropdown ? sourceDropdown.value : 0);
            string text = pathInput.text?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                SetStatus("Path is empty.");
                return;
            }

            // URLモード：そのまま
            if (mode == SourceMode.Url)
            {
                loader.LoadVideoUrl(text);
                return;
            }

            // Fileモード：file:/// を付ける（付いていなければ補正）
            string url = NormalizeFileUrl(text);
            loader.LoadVideoUrl(url);
        }

        private void OnClickBrowse()
        {
            var extensions = new[]
            {
                new ExtensionFilter("Video Files", "mp4", "mov", "webm", "avi"),
                new ExtensionFilter("All Files", "*"),
            };

            string[] paths = StandaloneFileBrowser.OpenFilePanel(
                "Select Video File",
                "",
                extensions,
                false
            );

            if (paths != null && paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                pathInput.text = paths[0];
                SetStatus("Selected: " + System.IO.Path.GetFileName(paths[0]));
            }
        }

        private static string NormalizeFileUrl(string pathOrUrl)
        {
            // すでに file:// が付いている場合はそのまま
            if (pathOrUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                return pathOrUrl;

            // Windowsパス "C:\xxx" → file:///C:/xxx
            string p = pathOrUrl.Replace("\\", "/");
            if (!p.StartsWith("/")) p = "/" + p; // file URLとしての最低限の形
            return "file://" + p;
        }

        private void SetStatus(string msg)
        {
            if (statusText) statusText.text = msg;
        }

        private static bool IsEditorBuild()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }
}
