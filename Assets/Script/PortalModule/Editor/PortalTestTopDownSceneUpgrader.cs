#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PortalModule.Editor
{
    public static class PortalTestTopDownSceneUpgrader
    {
        private const string ScenePath = "Assets/Scenes/PortalTestTopDown.unity";

        [MenuItem("Tools/Portal/Upgrade 2.5D Test Scene")]
        public static void UpgradeScene()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            PortalTestTopDownSceneBuilder.ApplyMazeToOpenScene();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[PortalTestTopDown] Player not found in scene.");
                return;
            }

            PortalPlacementController placement = player.GetComponent<PortalPlacementController>();
            if (placement == null)
                placement = player.AddComponent<PortalPlacementController>();

            SerializedObject placementObject = new SerializedObject(placement);
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                placementObject.FindProperty("placementCamera").objectReferenceValue = mainCamera;
            placementObject.FindProperty("showControlsHint").boolValue = true;
            placementObject.ApplyModifiedPropertiesWithoutUndo();

            player.transform.position = PortalTestTopDownSceneBuilder.GetMazeStartPosition();

            PortalTestPlayerTopDown movement = player.GetComponent<PortalTestPlayerTopDown>();
            if (movement != null)
            {
                SerializedObject movementObject = new SerializedObject(movement);
                movementObject.FindProperty("moveMode").enumValueIndex = (int)PortalPlayerMoveMode.ForwardOnly;
                movementObject.FindProperty("moveDirection").vector3Value = Vector3.forward;
                movementObject.FindProperty("directionIsLocal").boolValue = false;
                movementObject.FindProperty("autoMove").boolValue = true;
                movementObject.ApplyModifiedPropertiesWithoutUndo();
            }

            GameObject goal = GameObject.Find("LevelGoal");
            if (goal != null)
                goal.transform.position = PortalTestTopDownSceneBuilder.GetMazeGoalPosition();

            Camera mainCameraComponent = Camera.main;
            if (mainCameraComponent != null)
                mainCameraComponent.orthographicSize = 12f;

            DisableScenePortal("Portal_A");
            DisableScenePortal("Portal_B");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[PortalTestTopDown] 2.5D scene upgraded with maze. Press Play and use Space + mouse to place portals.");
        }

        private static void DisableScenePortal(string portalName)
        {
            GameObject portal = GameObject.Find(portalName);
            if (portal == null)
                return;

            portal.SetActive(false);
        }
    }
}
#endif
