#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Temporary visual QA helper. Preview states never call a save or purchase method.
public static class TerminalPreview
{
    private static EconomySaveData original;

    [MenuItem("Tools/TL4 Terminal Preview/Open Shop %&m")]
    private static void OpenShop()
    {
        if (!EditorApplication.isPlaying)
            EditorSceneManager.OpenScene("Assets/src/TL4_Getty/Scenes/MetaShop.unity");
    }

    [MenuItem("Tools/TL4 Terminal Preview/No Points %&1")]
    private static void NoPoints() => Preview(false);

    [MenuItem("Tools/TL4 Terminal Preview/Max Levels %&2")]
    private static void MaxLevels() => Preview(true);

    private static void Preview(bool maxed)
    {
        EconomyManager manager = EconomyManager.Instance;
        if (!EditorApplication.isPlaying || manager == null) return;
        FieldInfo field = typeof(EconomyManager).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
        if (original == null) original = (EconomySaveData)field.GetValue(manager);
        field.SetValue(manager, maxed
            ? new EconomySaveData { points = original.points, healthLevel = 10, defenseLevel = 10, attackLevel = 10 }
            : new EconomySaveData());
    }

    [MenuItem("Tools/TL4 Terminal Preview/Restore Save View %&3")]
    private static void Restore()
    {
        if (EconomyManager.Instance == null || original == null) return;
        typeof(EconomyManager).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(EconomyManager.Instance, original);
        original = null;
    }
}
#endif
