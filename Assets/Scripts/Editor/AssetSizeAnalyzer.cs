using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class AssetSizeAnalyzer : EditorWindow
{
    [MenuItem("Tools/Asset Size Analyzer")]
    public static void ShowWindow()
    {
        GetWindow<AssetSizeAnalyzer>("Asset Size Analyzer");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Analyze Assets"))
        {
            Analyze();
        }
    }

    void Analyze()
    {
        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets" });
        var sizes = new Dictionary<string, long>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            long size = GetFileSize(path);
            if (size > 1024 * 1024) // > 1MB
            {
                sizes[path] = size;
            }
        }

        var sorted = sizes.OrderByDescending(x => x.Value).Take(50);

        Debug.Log("=== TOP 50 MAIORES ASSETS ===");
        foreach (var item in sorted)
        {
            Debug.Log($"{item.Key} - {item.Value / (1024 * 1024)} MB");
        }
    }

    long GetFileSize(string path)
    {
        string fullPath = Application.dataPath + "/../" + path;
        if (File.Exists(fullPath))
        {
            return new FileInfo(fullPath).Length;
        }
        return 0;
    }
}