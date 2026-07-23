#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Can only be used in UNITY_EDITOR.
/// </summary>
[System.Serializable]
public class FolderReference
{
    public string GUID;
    public string AssetPath => AssetDatabase.GUIDToAssetPath(GUID);

    public List<AssetType> GetAssetsOfType<AssetType>(bool recursive)
        where AssetType : Object
    {
        List<AssetType> assets = new();

        // Build search filter - this searches by type
        string filter = $"t:{typeof(AssetType).Name}";

        // Specify search folders
        string[] searchInFolders = new[] { AssetPath };

        // Find all GUIDs matching the type filter (always searches recursively)
        string[] guids = AssetDatabase.FindAssets(filter, searchInFolders);

        // Normalize the base path for comparison
        string normalizedBasePath = AssetPath.Replace('\\', '/');

        // Load assets by GUID (much faster than path-based loading)
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // If not recursive, skip assets in subdirectories
            if (!recursive)
            {
                string assetDirectory = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');

                // Skip this asset if it's not directly in the target folder
                if (assetDirectory != normalizedBasePath)
                {
                    continue;
                }
            }

            var asset = AssetDatabase.LoadAssetAtPath<AssetType>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }

        return assets;
    }
}

/// <summary>
/// Can only be used in UNITY_EDITOR
/// </summary>
[CustomPropertyDrawer(typeof(FolderReference))]
public class FolderReferencePropertyDrawer : PropertyDrawer
{
    private bool initialized;
    private SerializedProperty guid;
    private Object obj;

    private void Init(SerializedProperty property)
    {
        initialized = true;
        guid = property.FindPropertyRelative("GUID");
        obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid.stringValue));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!initialized) Init(property);

        GUIContent guiContent = EditorGUIUtility.ObjectContent(obj, typeof(DefaultAsset));

        Rect r = EditorGUI.PrefixLabel(position, label);

        Rect textFieldRect = r;
        textFieldRect.width -= 19f;

        GUIStyle textFieldStyle = new("TextField")
        {
            imagePosition = obj ? ImagePosition.ImageLeft : ImagePosition.TextOnly
        };

        if (GUI.Button(textFieldRect, guiContent, textFieldStyle) && obj)
            EditorGUIUtility.PingObject(obj);

        if (textFieldRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                Object reference = DragAndDrop.objectReferences[0];
                string path = AssetDatabase.GetAssetPath(reference);
                DragAndDrop.visualMode = Directory.Exists(path) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                Object reference = DragAndDrop.objectReferences[0];
                string path = AssetDatabase.GetAssetPath(reference);
                if (Directory.Exists(path))
                {
                    obj = reference;
                    guid.stringValue = AssetDatabase.AssetPathToGUID(path);
                }
                Event.current.Use();
            }
        }

        Rect objectFieldRect = r;
        objectFieldRect.x = textFieldRect.xMax + 1f;
        objectFieldRect.width = 19f;

        if (GUI.Button(objectFieldRect, "", GUI.skin.GetStyle("IN ObjectField")))
        {
            string path = EditorUtility.OpenFolderPanel("Select a folder", "Assets", "");
            if (path.Contains(Application.dataPath))
            {
                path = "Assets" + path[Application.dataPath.Length..];
                obj = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset));
                guid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            }
            else Debug.LogError("The path must be in the Assets folder");
            GUIUtility.ExitGUI();
        }
    }
}
#endif