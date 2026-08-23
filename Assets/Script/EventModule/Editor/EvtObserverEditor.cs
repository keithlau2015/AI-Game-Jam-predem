using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EvtObserver), true)]
public class EvtObserverEditor : Editor
{
    private SerializedProperty evtNameIdProperty;

    private void OnEnable()
    {
        evtNameIdProperty = serializedObject.FindProperty("evtNameId");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Event Identity", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(evtNameIdProperty, new GUIContent("Evt Name Id"));

        DrawEvtNameIdValidation(evtNameIdProperty.stringValue);

        if (GUILayout.Button("Generate Unique Evt Name Id"))
        {
            evtNameIdProperty.stringValue = EvtIdentityValidation.GenerateUniqueObserverEvtNameId();
        }

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script", "evtNameId");

        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawEvtNameIdValidation(string evtNameId)
    {
        string normalized = EvtIdentityValidation.Normalize(evtNameId);
        if (!EvtIdentityValidation.IsValid(normalized))
        {
            EditorGUILayout.HelpBox("Evt Name Id is required. Enter a unique name or use Generate Unique Evt Name Id.", MessageType.Warning);
            return;
        }

        string conflict = EvtIdentityValidation.DescribeIdentityConflict(normalized);
        if (conflict != null)
        {
            EditorGUILayout.HelpBox(conflict, MessageType.Error);
            return;
        }

        EditorGUILayout.HelpBox($"Evt Name Id '{normalized}' is unique among all EvtObserable and EvtObserver in this scene.", MessageType.Info);
    }
}
