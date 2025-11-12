using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class EditorSceneBuilder
{
    [MenuItem("Tools/Generate/Editor Scene UI")] 
    public static void CreateEditorScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        // Camera
        var camGO = GameObject.FindObjectOfType<Camera>();
        if(camGO==null) { camGO = new GameObject("Main Camera"); camGO.AddComponent<Camera>(); }

        // Canvas
        var canvasGO = new GameObject("EditorUI");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Left: Project Panel
        var left = CreatePanel(canvasGO.transform, "ProjectPanel", new Rect(0,0,250,600), new Vector2(0,1), new Vector2(0,1));
        CreateLabel(left.transform, "Project\n(Assets/Video, Presets)");

        // Right: Inspector
        var right = CreatePanel(canvasGO.transform, "Inspector", new Rect(-260,0,250,600), new Vector2(1,1), new Vector2(1,1));
        CreateLabel(right.transform, "Inspector");

        // Top: Preview
        var top = CreatePanel(canvasGO.transform, "Preview", new Rect(260, -50, -520, 400), new Vector2(0.5f,1), new Vector2(0.5f,1));
        CreateLabel(top.transform, "Preview (Video)");

        // Bottom: Timeline
        var bottom = CreatePanel(canvasGO.transform, "Timeline", new Rect(260, -480, -520, 120), new Vector2(0.5f,0), new Vector2(0.5f,0));
        CreateLabel(bottom.transform, "Timeline");

        // Add Play/Pause buttons and slider
        var play = CreateButton(bottom.transform, "Play", new Vector2(-200,20));
        var pause = CreateButton(bottom.transform, "Pause", new Vector2(-120,20));
        var slider = CreateSlider(bottom.transform, new Vector2(0,20), 400);
        var timeLabel = CreateText(bottom.transform, "00:00", new Vector2(220,20));

        // TimeController on root
        var tc = new GameObject("TimeController");
        var controller = tc.AddComponent<UnityVFXEditor.Core.TimeController>();

        // Wire up TimelineUI
        var timelineHolder = tc.AddComponent<UnityVFXEditor.UI.TimelineUI>();
        timelineHolder.playButton = play.GetComponent<UnityEngine.UI.Button>();
        timelineHolder.pauseButton = pause.GetComponent<UnityEngine.UI.Button>();
        timelineHolder.slider = slider.GetComponent<UnityEngine.UI.Slider>();
        timelineHolder.timeLabel = timeLabel.GetComponent<UnityEngine.UI.Text>();

        // Add a Quad for VideoPlane
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "VideoPlane";
        quad.transform.position = new Vector3(0,0,5);
        var vp = quad.AddComponent<UnityEngine.Video.VideoPlayer>();
        vp.playOnAwake = false;
        controller.gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Editor.unity");
        EditorUtility.DisplayDialog("Done", "Editor scene generated at Assets/Scenes/Editor.unity", "OK");
    }

    static GameObject CreatePanel(Transform parent, string name, Rect offsets, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0.15f,0.15f,0.15f,0.9f);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f,0.5f);
        rt.sizeDelta = new Vector2(offsets.width, offsets.height);
        rt.anchoredPosition = new Vector2(offsets.x, offsets.y);
        return go;
    }

    static GameObject CreateLabel(Transform parent, string text)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<UnityEngine.UI.Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.sizeDelta = Vector2.zero;
        return go;
    }

    static GameObject CreateButton(Transform parent, string label, Vector2 pos)
    {
        var go = new GameObject(label+"Button");
        go.transform.SetParent(parent, false);
        var btn = go.AddComponent<UnityEngine.UI.Button>();
        var img = go.AddComponent<UnityEngine.UI.Image>(); img.color = Color.white * 0.8f;
        var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(80,30); rt.anchoredPosition = pos;
        var txtGO = new GameObject("Text"); txtGO.transform.SetParent(go.transform, false);
        var txt = txtGO.AddComponent<UnityEngine.UI.Text>(); txt.text = label; txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.black;
        var rt2 = txtGO.GetComponent<RectTransform>(); rt2.anchorMin = Vector2.zero; rt2.anchorMax = Vector2.one; rt2.sizeDelta = Vector2.zero;
        return go;
    }

    static GameObject CreateSlider(Transform parent, Vector2 pos, float width)
    {
        var go = new GameObject("TimelineSlider"); go.transform.SetParent(parent, false);
        var slider = go.AddComponent<UnityEngine.UI.Slider>();
        var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(width,20); rt.anchoredPosition = pos;
        // Note: not fully fleshed (no background/thumb). For visual placeholder, add Image components
        var bg = new GameObject("Background"); bg.transform.SetParent(go.transform, false); var img = bg.AddComponent<UnityEngine.UI.Image>(); img.color = Color.gray; var rt2 = bg.GetComponent<RectTransform>(); rt2.anchorMin = Vector2.zero; rt2.anchorMax = Vector2.one; rt2.sizeDelta = Vector2.zero;
        var fill = new GameObject("Fill"); fill.transform.SetParent(bg.transform, false); var imgf = fill.AddComponent<UnityEngine.UI.Image>(); imgf.color = Color.green; var rt3 = fill.GetComponent<RectTransform>(); rt3.anchorMin = Vector2.zero; rt3.anchorMax = new Vector2(0.5f,1); rt3.sizeDelta = Vector2.zero;
        var thumb = new GameObject("Thumb"); thumb.transform.SetParent(go.transform, false); var imgt = thumb.AddComponent<UnityEngine.UI.Image>(); imgt.color = Color.white; var rt4 = thumb.GetComponent<RectTransform>(); rt4.sizeDelta = new Vector2(12,20);
        slider.fillRect = imgf.rectTransform;
        slider.targetGraphic = imgt;
        slider.handleRect = rt4;
        return go;
    }

    static GameObject CreateText(Transform parent, string text, Vector2 pos)
    {
        var go = new GameObject("TimeLabel"); go.transform.SetParent(parent, false);
        var t = go.AddComponent<UnityEngine.UI.Text>(); t.text = text; t.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); t.alignment = TextAnchor.MiddleCenter; t.color = Color.white;
        var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(80,30); rt.anchoredPosition = pos;
        return go;
    }
}
