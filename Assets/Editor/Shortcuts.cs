#if UNITY_EDITOR
using UnityEditor;

public class Shortcuts {
    [MenuItem("Edit/Clear Progress Bar")]
    public static void ClearProgressBar(){
        EditorUtility.ClearProgressBar();
    }
}
#endif