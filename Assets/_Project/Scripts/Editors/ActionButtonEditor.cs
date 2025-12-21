using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavigationButton), true)]
public class ActionButtonEditor : Editor
{
    SerializedProperty actionMode;
    SerializedProperty screenManager;
    SerializedProperty m_ScreenName;
    private void OnEnable()
    {
        actionMode = serializedObject.FindProperty("actionMode");
        screenManager = serializedObject.FindProperty("m_ScreenManager");
        m_ScreenName = serializedObject.FindProperty("m_ScreenName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(actionMode);
        EditorGUILayout.PropertyField(m_ScreenName);

        var mode = (NavigationButton.ActionButtonMode)actionMode.enumValueIndex;

        if (mode == NavigationButton.ActionButtonMode.SwitchScreen)
        {
            EditorGUILayout.PropertyField(screenManager);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
