using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EvtObserable), true)]
public class EvtObserableEditor : Editor
{
    private SerializedProperty idProperty;

    private void OnEnable()
    {
        idProperty = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Event Identity", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(idProperty, new GUIContent("Id"));

        DrawIdValidation(idProperty.stringValue);

        if (GUILayout.Button("Generate Unique Id"))
        {
            idProperty.stringValue = EvtIdentityValidation.GenerateUniqueObservableId();
        }

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script", "id");

        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawIdValidation(string id)
    {
        string normalized = EvtIdentityValidation.Normalize(id);
        if (!EvtIdentityValidation.IsValid(normalized))
        {
            EditorGUILayout.HelpBox("Id is required. Enter a unique name or use Generate Unique Id.", MessageType.Warning);
            return;
        }

        string conflict = EvtIdentityValidation.DescribeIdentityConflict(normalized);
        if (conflict != null)
        {
            EditorGUILayout.HelpBox(conflict, MessageType.Error);
            return;
        }

        EditorGUILayout.HelpBox($"Id '{normalized}' is unique among all EvtObserable and EvtObserver in this scene.", MessageType.Info);
    }
}
