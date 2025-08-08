using UnityEngine;
using UnityEditor;

public class MissingScriptFinder : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts In Scene")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptFinder>("Missing Script Finder");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts In Current Scene"))
        {
            FindMissingScripts();
        }
    }

    private static void FindMissingScripts()
    {
        int missingCount = 0;

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.scene.IsValid())
            {
                Component[] components = go.GetComponents<Component>();

                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        Debug.LogWarning($"Missing script found in GameObject '{GetFullPath(go)}', Component Index: {i}", go);
                        missingCount++;
                    }
                }
            }
        }

        if (missingCount == 0)
            Debug.Log("No missing scripts found in current scene.");
        else
            Debug.Log($"Found {missingCount} missing script(s) in current scene.");
    }

    private static string GetFullPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}
