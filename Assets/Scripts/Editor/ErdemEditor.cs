#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Reflection;

public class MeshCombiner : EditorWindow
{

    [MenuItem("Erdem/Mesh Combiner")]
    static void OpenWindow()
    {
        MeshCombiner window = (MeshCombiner) EditorWindow.GetWindow(typeof(MeshCombiner));
        window.Show();
    }
    public List<Object> meshes = new List<Object>();
    public int meshCount = 0;
    public Object newMeshObject;
    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        meshCount = EditorGUILayout.IntField("Mesh Count", meshCount);
        if (EditorGUI.EndChangeCheck())
        {
            meshes.Clear();
            for (int i = 0; i < meshCount; i++)
            {
                meshes.Add(new Object());
            }

        }
        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i] = EditorGUILayout.ObjectField(meshes[i], typeof(MeshFilter), true);
        }
        newMeshObject = EditorGUILayout.ObjectField(newMeshObject, typeof(GameObject), true);
        if (GUILayout.Button("Combine It!"))
        {
            CombineMeshes();
        }
        if (GUILayout.Button("Clear"))
        {
            meshes.Clear();
        }
    }

    private void CombineMeshes()
    {
        Vector3 pos = (newMeshObject as GameObject).transform.position;
        CombineInstance[] combine = new CombineInstance[meshCount];
        for (int i = 0; i < meshCount; i++)
        {
            MeshFilter meshFilter = meshes[i] as MeshFilter;
            if (meshFilter == null) continue;

            combine[i].mesh = meshFilter.sharedMesh;
            combine[i].transform = meshFilter.transform.localToWorldMatrix;
            meshFilter.gameObject.SetActive(false);
        }
        GameObject obj = newMeshObject as GameObject;

        if (obj.transform.GetComponent<MeshFilter>() == null)
        {
            obj.AddComponent<MeshFilter>();
        }
        if (obj.transform.GetComponent<MeshRenderer>() == null)
        {
            obj.AddComponent<MeshRenderer>();
        }
        obj.transform.position = Vector3.zero;
        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);
        obj.transform.position = pos;

        obj.transform.GetComponent<MeshRenderer>().material = (meshes[0] as MeshFilter).transform.GetComponent<MeshRenderer>().sharedMaterial;



    }
}

public class StateCreator : EditorWindow
{
    [MenuItem("Erdem/State Creator")]
    static void OpenWindow()
    {
        StateCreator window = (StateCreator) EditorWindow.GetWindow(typeof(StateCreator));
        window.Show();
    }
    public string stateName;

    private void OnGUI()
    {
        stateName = EditorGUILayout.TextField("State Name: ", stateName);

        if (GUILayout.Button("Create State"))
        {
            CreateState();
        }
    }

    private IEnumerator PingAfterCompiling()
    {
        yield return new WaitUntil(() => !EditorApplication.isCompiling);
        var obj = AssetDatabase.LoadAssetAtPath($"Assets/Scripts/Runtime/States/{stateName}.cs", typeof(Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
    private void CreateState()
    {
        if (System.IO.File.Exists($"{Application.dataPath}/Scripts/Runtime/States/{stateName}.cs"))
        {
            EditorUtility.DisplayDialog("Error!", $"{stateName}.cs Already Exist!", "OK");
            TMP_EditorCoroutine.StartCoroutine(PingAfterCompiling());
            return;
        }
        if (string.IsNullOrEmpty(stateName))
        {
            EditorUtility.DisplayDialog("Error!", $"State name cannot be empty!", "OK");
            return;
        }
        FileUtil.CopyFileOrDirectory($"{Application.dataPath}/Scripts/Runtime/States/EmptyState.cs", $"{Application.dataPath}/Scripts/Runtime/States/{stateName}.cs");

        StreamReader sr = new StreamReader($"{Application.dataPath}/Scripts/Runtime/States/{stateName}.cs");
        List<string> rows = StreamReaderLineByLine(sr);
        sr.Close();

        StreamWriter sw = new StreamWriter($"{Application.dataPath}/Scripts/Runtime/States/{stateName}.cs");
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Contains("EmptyState"))
            {
                rows[i] = rows[i].Replace("EmptyState", stateName);
            }
            sw.WriteLine(rows[i]);
        }
        sw.Close();

        StreamReader imReader = new StreamReader($"{Application.dataPath}/Scripts/Managers/InputManager.cs");
        List<string> imRows = StreamReaderLineByLine(imReader);
        imReader.Close();

        StreamWriter imWriter = new StreamWriter($"{Application.dataPath}/Scripts/Managers/InputManager.cs");
        for (int i = 0; i < imRows.Count; i++)
        {
            if (imRows[i].Contains($"[NEW_STATE]"))
            {
                imRows[i] = $"\t\tallStates.Add(\"{stateName}\", new {stateName}());\n\t\t//[NEW_STATE]";
            }
            imWriter.WriteLine(imRows[i]);
        }
        imWriter.Close();

        AssetDatabase.Refresh();
        TMP_EditorCoroutine.StartCoroutine(PingAfterCompiling());
    }
    private List<string> StreamReaderLineByLine(StreamReader sr)
    {
        var result = new List<string>();
        using (sr)
        {
            while (!sr.EndOfStream)
            {
                result.Add(sr.ReadLine());
            }
        }
        return result;
    }
}

public class SiblingSorterWindow : EditorWindow
{
    [MenuItem("Erdem/Sibling Sorter")]
    static void SiblingSorter()
    {
        SiblingSorterWindow siblingSorterWindow = (SiblingSorterWindow) EditorWindow.GetWindow(typeof(SiblingSorterWindow));
        siblingSorterWindow.Show();
    }
    public List<Object> selectedSiblings = new List<Object>();
    private void OnGUI()
    {
        if (Selection.objects.Length <= 0) return;
        var selectedObject = Selection.objects[Selection.objects.Length - 1];
        if (!selectedSiblings.Contains(selectedObject))
        {
            selectedSiblings.Add(selectedObject);
        }
        for (int i = 0; i < selectedSiblings.Count; i++)
        {
            selectedSiblings[i] = EditorGUILayout.ObjectField(selectedSiblings[i], typeof(GameObject), true);
        }
        if (GUILayout.Button("Sort It!"))
        {
            for (int i = 0; i < selectedSiblings.Count; i++)
            {
                GameObject transform = (GameObject) selectedSiblings[i];
                transform.transform.SetSiblingIndex(i);
            }
        }
        if (GUILayout.Button("Clear"))
        {
            selectedSiblings.Clear();
        }
    }
}

internal static class ReflectionExtensions
{
    internal static object FetchField(this Type type, string field)
    {
        return type.GetFieldRecursive(field, true).GetValue(null);
    }

    internal static object FetchField(this object obj, string field)
    {
        return obj.GetType().GetFieldRecursive(field, false).GetValue(obj);
    }

    internal static object FetchProperty(this Type type, string property)
    {
        return type.GetPropertyRecursive(property, true).GetValue(null, null);
    }

    internal static object FetchProperty(this object obj, string property)
    {
        return obj.GetType().GetPropertyRecursive(property, false).GetValue(obj, null);
    }

    internal static object CallMethod(this Type type, string method, params object[] parameters)
    {
        return type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, parameters);
    }

    internal static object CallMethod(this object obj, string method, params object[] parameters)
    {
        return obj.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, parameters);
    }

    internal static object CreateInstance(this Type type, params object[] parameters)
    {
        Type[] parameterTypes;
        if (parameters == null)
            parameterTypes = null;
        else
        {
            parameterTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                parameterTypes[i] = parameters[i].GetType();
        }

        return CreateInstance(type, parameterTypes, parameters);
    }

    internal static object CreateInstance(this Type type, Type[] parameterTypes, object[] parameters)
    {
        return type.GetConstructor(parameterTypes).Invoke(parameters);
    }

    private static FieldInfo GetFieldRecursive(this Type type, string field, bool isStatic)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | (isStatic ? BindingFlags.Static : BindingFlags.Instance);
        do
        {
            FieldInfo fieldInfo = type.GetField(field, flags);
            if (fieldInfo != null)
                return fieldInfo;

            type = type.BaseType;
        }
        while (type != null);

        return null;
    }

    private static PropertyInfo GetPropertyRecursive(this Type type, string property, bool isStatic)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | (isStatic ? BindingFlags.Static : BindingFlags.Instance);
        do
        {
            PropertyInfo propertyInfo = type.GetProperty(property, flags);
            if (propertyInfo != null)
                return propertyInfo;

            type = type.BaseType;
        }
        while (type != null);

        return null;
    }
}

public class MultiScreenshotCapture : EditorWindow
{
    private enum TargetCamera { GameView = 0, SceneView = 1 };

    private class CustomResolution
    {
        public readonly int width, height;
        private int originalIndex, newIndex;

        private bool m_isActive;
        public bool IsActive
        {
            get { return m_isActive; }
            set
            {
                if (m_isActive != value)
                {
                    m_isActive = value;

                    int resolutionIndex;
                    if (m_isActive)
                    {
                        originalIndex = (int) GameView.FetchProperty("selectedSizeIndex");

                        object customSize = GetFixedResolution(width, height);
                        SizeHolder.CallMethod("AddCustomSize", customSize);
                        newIndex = (int) SizeHolder.CallMethod("IndexOf", customSize) + (int) SizeHolder.CallMethod("GetBuiltinCount");
                        resolutionIndex = newIndex;
                    }
                    else
                    {
                        SizeHolder.CallMethod("RemoveCustomSize", newIndex);
                        resolutionIndex = originalIndex;
                    }

                    GameView.CallMethod("SizeSelectionCallback", resolutionIndex, null);
                    GameView.Repaint();
                }
            }
        }

        public CustomResolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    [Serializable]
    private class SaveData
    {
        public List<Vector2> resolutions;
        public List<bool> resolutionsEnabled;
        public bool currentResolutionEnabled;
    }

    [Serializable]
    private class SessionData
    {
        public List<Vector2> resolutions;
        public List<bool> resolutionsEnabled;
        public bool currentResolutionEnabled;
        public float resolutionMultiplier;
        public TargetCamera targetCamera;
        public bool captureOverlayUI;
        public bool setTimeScaleToZero;
        public bool saveAsPNG;
        public bool allowTransparentBackground;
        public string saveDirectory;
    }

    private const string SESSION_DATA_PATH = "Library/MSC_Session.json";
    private const string TEMPORARY_RESOLUTION_LABEL = "MSC_temp";
    private readonly GUILayoutOption GL_WIDTH_25 = GUILayout.Width(25f);
    private readonly GUILayoutOption GL_EXPAND_WIDTH = GUILayout.ExpandWidth(true);

    private static object SizeHolder { get { return GetType("GameViewSizes").FetchProperty("instance").FetchProperty("currentGroup"); } }
    private static EditorWindow GameView { get { return GetWindow(GetType("GameView")); } }
    //private static EditorWindow GameView { get { return (EditorWindow) GetType( "GameView" ).CallMethod( "GetMainGameView" ); } }

    private List<Vector2> resolutions = new List<Vector2>() { new Vector2(1024, 768) }; // Not readonly to support serialization
    private List<bool> resolutionsEnabled = new List<bool>() { true }; // Same as above
    private bool currentResolutionEnabled = true;
    private float resolutionMultiplier = 1f;

    private TargetCamera targetCamera = TargetCamera.GameView;
    private bool captureOverlayUI = false;
    private bool setTimeScaleToZero = true;
    private float prevTimeScale;
    private bool saveAsPNG = true;
    private bool allowTransparentBackground = false;
    private string saveDirectory;

    private Vector2 scrollPos;

    private readonly List<CustomResolution> queuedScreenshots = new List<CustomResolution>();

    [MenuItem("Erdem/Multi Screenshot Capture")]
    private static void Init()
    {
        MultiScreenshotCapture window = GetWindow<MultiScreenshotCapture>();
        window.titleContent = new GUIContent("Screenshot");
        window.minSize = new Vector2(325f, 150f);
        window.Show();
    }

    private void Awake()
    {
        if (File.Exists(SESSION_DATA_PATH))
        {
            SessionData sessionData = JsonUtility.FromJson<SessionData>(File.ReadAllText(SESSION_DATA_PATH));
            resolutions = sessionData.resolutions;
            resolutionsEnabled = sessionData.resolutionsEnabled;
            currentResolutionEnabled = sessionData.currentResolutionEnabled;
            resolutionMultiplier = sessionData.resolutionMultiplier > 0f ? sessionData.resolutionMultiplier : 1f;
            targetCamera = sessionData.targetCamera;
            captureOverlayUI = sessionData.captureOverlayUI;
            setTimeScaleToZero = sessionData.setTimeScaleToZero;
            saveAsPNG = sessionData.saveAsPNG;
            allowTransparentBackground = sessionData.allowTransparentBackground;
            saveDirectory = sessionData.saveDirectory;
        }
    }

    private void OnDestroy()
    {
        SessionData sessionData = new SessionData()
        {
            resolutions = resolutions,
            resolutionsEnabled = resolutionsEnabled,
            currentResolutionEnabled = currentResolutionEnabled,
            resolutionMultiplier = resolutionMultiplier,
            targetCamera = targetCamera,
            captureOverlayUI = captureOverlayUI,
            setTimeScaleToZero = setTimeScaleToZero,
            saveAsPNG = saveAsPNG,
            allowTransparentBackground = allowTransparentBackground,
            saveDirectory = saveDirectory
        };

        File.WriteAllText(SESSION_DATA_PATH, JsonUtility.ToJson(sessionData));
    }

    private void OnGUI()
    {
        // In case resolutionsEnabled didn't exist when the latest SessionData was created
        if (resolutionsEnabled == null || resolutionsEnabled.Count != resolutions.Count)
        {
            resolutionsEnabled = new List<bool>(resolutions.Count);
            for (int i = 0; i < resolutions.Count; i++)
                resolutionsEnabled.Add(true);
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.BeginHorizontal();

        GUILayout.Label("Resolutions:", GL_EXPAND_WIDTH);

        if (GUILayout.Button("Save"))
            SaveSettings();

        if (GUILayout.Button("Load"))
            LoadSettings();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUI.enabled = currentResolutionEnabled;
        GUILayout.Label("Current Resolution", GL_EXPAND_WIDTH);
        GUI.enabled = true;

        currentResolutionEnabled = EditorGUILayout.Toggle(GUIContent.none, currentResolutionEnabled, GL_WIDTH_25);

        if (GUILayout.Button("+", GL_WIDTH_25))
        {
            resolutions.Insert(0, new Vector2());
            resolutionsEnabled.Insert(0, true);
        }

        GUI.enabled = false;
        GUILayout.Button("-", GL_WIDTH_25);
        GUI.enabled = true;

        GUILayout.EndHorizontal();

        for (int i = 0; i < resolutions.Count; i++)
        {
            GUILayout.BeginHorizontal();

            GUI.enabled = resolutionsEnabled[i];
            resolutions[i] = EditorGUILayout.Vector2Field(GUIContent.none, resolutions[i]);
            GUI.enabled = true;
            resolutionsEnabled[i] = EditorGUILayout.Toggle(GUIContent.none, resolutionsEnabled[i], GL_WIDTH_25);

            if (GUILayout.Button("+", GL_WIDTH_25))
            {
                resolutions.Insert(i + 1, new Vector2());
                resolutionsEnabled.Insert(i + 1, true);
            }

            if (GUILayout.Button("-", GL_WIDTH_25))
            {
                resolutions.RemoveAt(i);
                resolutionsEnabled.RemoveAt(i);
                i--;
            }

            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        resolutionMultiplier = EditorGUILayout.FloatField("Resolution Multiplier", resolutionMultiplier);
        targetCamera = (TargetCamera) EditorGUILayout.EnumPopup("Target Camera", targetCamera);

        EditorGUILayout.Space();

        if (targetCamera == TargetCamera.GameView)
        {
            captureOverlayUI = EditorGUILayout.ToggleLeft("Capture Overlay UI", captureOverlayUI);
            if (captureOverlayUI && EditorApplication.isPlaying)
            {
                EditorGUI.indentLevel++;
                setTimeScaleToZero = EditorGUILayout.ToggleLeft("Set timeScale to 0 during capture", setTimeScaleToZero);
                EditorGUI.indentLevel--;
            }
        }

        saveAsPNG = EditorGUILayout.ToggleLeft("Save as PNG", saveAsPNG);
        if (saveAsPNG && !captureOverlayUI && targetCamera == TargetCamera.GameView)
        {
            EditorGUI.indentLevel++;
            allowTransparentBackground = EditorGUILayout.ToggleLeft("Allow transparent background", allowTransparentBackground);
            if (allowTransparentBackground)
                EditorGUILayout.HelpBox("For transparent background to work, you may need to disable post-processing on the Main Camera.", MessageType.Info);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        saveDirectory = PathField("Save to:", saveDirectory);

        EditorGUILayout.Space();

        GUI.enabled = queuedScreenshots.Count == 0 && resolutionMultiplier > 0f;
        if (GUILayout.Button("Capture Screenshots"))
        {
            if (string.IsNullOrEmpty(saveDirectory))
                saveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            if (currentResolutionEnabled)
                CaptureScreenshot((targetCamera == TargetCamera.GameView ? Camera.main : SceneView.lastActiveSceneView.camera).pixelRect.size);

            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutionsEnabled[i])
                    CaptureScreenshot(resolutions[i]);
            }

            if (!captureOverlayUI || targetCamera == TargetCamera.SceneView)
                Debug.Log("<b>Saved screenshots:</b> " + saveDirectory);
            else
            {
                if (EditorApplication.isPlaying && setTimeScaleToZero)
                {
                    prevTimeScale = Time.timeScale;
                    Time.timeScale = 0f;
                }

                EditorApplication.update -= CaptureQueuedScreenshots;
                EditorApplication.update += CaptureQueuedScreenshots;
            }
        }
        GUI.enabled = true;

        EditorGUILayout.EndScrollView();
    }

    private void CaptureScreenshot(Vector2 resolution)
    {
        int width = Mathf.RoundToInt(resolution.x * resolutionMultiplier);
        int height = Mathf.RoundToInt(resolution.y * resolutionMultiplier);

        if (width <= 0 || height <= 0)
            Debug.LogWarning("Skipped resolution: " + resolution);
        else if (!captureOverlayUI || targetCamera == TargetCamera.SceneView)
            CaptureScreenshotWithoutUI(width, height);
        else
            queuedScreenshots.Add(new CustomResolution(width, height));
    }

    private void CaptureQueuedScreenshots()
    {
        if (queuedScreenshots.Count == 0)
        {
            EditorApplication.update -= CaptureQueuedScreenshots;
            return;
        }

        CustomResolution resolution = queuedScreenshots[0];
        if (!resolution.IsActive)
        {
            resolution.IsActive = true;

            if (EditorApplication.isPlaying && EditorApplication.isPaused)
                EditorApplication.Step(); // Necessary to refresh overlay UI
        }
        else
        {
            try
            {
                CaptureScreenshotWithUI();
            }
            catch ( Exception e )
            {
                Debug.LogException(e);
            }

            resolution.IsActive = false;

            queuedScreenshots.RemoveAt(0);
            if (queuedScreenshots.Count == 0)
            {
                if (EditorApplication.isPlaying && EditorApplication.isPaused)
                    EditorApplication.Step(); // Necessary to restore overlay UI

                if (EditorApplication.isPlaying && setTimeScaleToZero)
                    Time.timeScale = prevTimeScale;

                Debug.Log("<b>Saved screenshots:</b> " + saveDirectory);
                Repaint();
            }
            else
            {
                // Activate the next resolution immediately
                CaptureQueuedScreenshots();
            }
        }
    }

    private void CaptureScreenshotWithoutUI(int width, int height)
    {
        Camera camera = targetCamera == TargetCamera.GameView ? Camera.main : SceneView.lastActiveSceneView.camera;

        RenderTexture temp = RenderTexture.active;
        RenderTexture temp2 = camera.targetTexture;

        RenderTexture renderTex = RenderTexture.GetTemporary(width, height, 24);
        Texture2D screenshot = null;

        bool allowHDR = camera.allowHDR;
        if (saveAsPNG && allowTransparentBackground)
            camera.allowHDR = false;

        try
        {
            RenderTexture.active = renderTex;

            camera.targetTexture = renderTex;
            camera.Render();

            screenshot = new Texture2D(renderTex.width, renderTex.height, saveAsPNG && allowTransparentBackground ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0, false);
            screenshot.Apply(false, false);

            File.WriteAllBytes(GetUniqueFilePath(renderTex.width, renderTex.height), saveAsPNG ? screenshot.EncodeToPNG() : screenshot.EncodeToJPG(100));
        }
        finally
        {
            camera.targetTexture = temp2;
            if (saveAsPNG && allowTransparentBackground)
                camera.allowHDR = allowHDR;

            RenderTexture.active = temp;
            RenderTexture.ReleaseTemporary(renderTex);

            if (screenshot != null)
                DestroyImmediate(screenshot);
        }
    }

    private void CaptureScreenshotWithUI()
    {
        RenderTexture temp = RenderTexture.active;

        RenderTexture renderTex = (RenderTexture) GameView.FetchField("m_TargetTexture");
        Texture2D screenshot = null;

        int width = renderTex.width;
        int height = renderTex.height;

        try
        {
            RenderTexture.active = renderTex;

            screenshot = new Texture2D(width, height, saveAsPNG && allowTransparentBackground ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

            if (SystemInfo.graphicsUVStartsAtTop)
            {
                Color32[] pixels = screenshot.GetPixels32();
                for (int i = 0; i < height / 2; i++)
                {
                    int startIndex0 = i * width;
                    int startIndex1 = (height - i - 1) * width;
                    for (int x = 0; x < width; x++)
                    {
                        Color32 color = pixels[startIndex0 + x];
                        pixels[startIndex0 + x] = pixels[startIndex1 + x];
                        pixels[startIndex1 + x] = color;
                    }
                }

                screenshot.SetPixels32(pixels);
            }

            screenshot.Apply(false, false);

            File.WriteAllBytes(GetUniqueFilePath(width, height), saveAsPNG ? screenshot.EncodeToPNG() : screenshot.EncodeToJPG(100));
        }
        finally
        {
            RenderTexture.active = temp;

            if (screenshot != null)
                DestroyImmediate(screenshot);
        }
    }

    private string PathField(string label, string path)
    {
        GUILayout.BeginHorizontal();
        path = EditorGUILayout.TextField(label, path);
        if (GUILayout.Button("o", GL_WIDTH_25))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Choose output directory", "", "");
            if (!string.IsNullOrEmpty(selectedPath))
                path = selectedPath;

            GUIUtility.keyboardControl = 0; // Remove focus from active text field
        }
        GUILayout.EndHorizontal();

        return path;
    }

    private void SaveSettings()
    {
        string savePath = EditorUtility.SaveFilePanel("Choose destination", "", "resolutions", "json");
        if (!string.IsNullOrEmpty(savePath))
        {
            SaveData saveData = new SaveData()
            {
                resolutions = resolutions,
                resolutionsEnabled = resolutionsEnabled,
                currentResolutionEnabled = currentResolutionEnabled
            };

            File.WriteAllText(savePath, JsonUtility.ToJson(saveData, false));
        }
    }

    private void LoadSettings()
    {
        string loadPath = EditorUtility.OpenFilePanel("Choose save file", "", "json");
        if (!string.IsNullOrEmpty(loadPath))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(loadPath));
            resolutions = saveData.resolutions ?? new List<Vector2>();
            resolutionsEnabled = saveData.resolutionsEnabled ?? new List<bool>();
            currentResolutionEnabled = saveData.currentResolutionEnabled;
        }
    }

    private string GetUniqueFilePath(int width, int height)
    {
        string filename = string.Concat(width, "x", height, " {0}", saveAsPNG ? ".png" : ".jpeg");
        int fileIndex = 0;
        string path;
        do
        {
            path = Path.Combine(saveDirectory, string.Format(filename, ++fileIndex));
        }
        while (File.Exists(path));

        return path;
    }

    private static object GetFixedResolution(int width, int height)
    {
        object sizeType = Enum.Parse(GetType("GameViewSizeType"), "FixedResolution");
        return GetType("GameViewSize").CreateInstance(sizeType, width, height, TEMPORARY_RESOLUTION_LABEL);
    }

    private static Type GetType(string type)
    {
        return typeof(EditorWindow).Assembly.GetType("UnityEditor." + type);
    }
}

#endif
