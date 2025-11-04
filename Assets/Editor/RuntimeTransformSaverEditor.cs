#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuntimeTransformSaver))]
public class RuntimeTransformSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RuntimeTransformSaver saver = (RuntimeTransformSaver)target;

        // Safe GUIStyle setup inside OnInspectorGUI
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        };

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Transform Saver", headerStyle);
        EditorGUILayout.Space();

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("📥 Save Transforms to File"))
            {
                saver.SaveCurrentTransforms();
            }
        }
        else
        {
            if (GUILayout.Button("📤 Load Transforms from File"))
            {
                saver.LoadSavedTransforms();
            }
        }
    }
}
#endif
