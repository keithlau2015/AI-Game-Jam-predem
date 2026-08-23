#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PortalModule.Editor
{
    public static class PortalTest2DSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/PortalTest2D.unity";

        [MenuItem("Tools/Portal/Build 2D Test Scene")]
        public static void BuildScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            CreateGround();
            CreatePortalService();
            CreatePortalPair();
            CreatePlayer();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[PortalTest2D] Scene saved to {ScenePath}. Press Play and use WASD to move.");
        }

        private static void CreateCamera()
        {
            GameObject cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            Camera camera = cameraGo.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
            cameraGo.transform.position = new Vector3(0f, 0f, -10f);
            cameraGo.AddComponent<AudioListener>();
        }

        private static void CreateGround()
        {
            GameObject ground = CreateColoredSprite("Ground", new Color(0.25f, 0.28f, 0.32f), new Vector3(0f, -3.5f, 0f), new Vector2(24f, 1f));
            BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(24f, 1f);
        }

        private static void CreatePortalService()
        {
            GameObject serviceGo = new GameObject("PortalService");
            serviceGo.AddComponent<PortalService>();
        }

        private static void CreatePortalPair()
        {
            CreatePortalSide(
                "Portal_A",
                new Vector3(-7f, -2.5f, 0f),
                "Portal_A",
                "Portal_B",
                new Color(0.35f, 0.75f, 1f));

            CreatePortalSide(
                "Portal_B",
                new Vector3(7f, -2.5f, 0f),
                "Portal_B",
                "Portal_A",
                new Color(1f, 0.55f, 0.35f));
        }

        private static void CreatePortalSide(string name, Vector3 position, string portalId, string destinationId, Color color)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            GameObject visual = CreateColoredSprite($"{name}_Visual", color, Vector3.zero, new Vector2(1.2f, 2f));
            visual.transform.SetParent(root.transform, false);

            GameObject spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.SetParent(root.transform, false);
            spawnPoint.transform.localPosition = new Vector3(portalId == "Portal_A" ? 2.5f : -2.5f, 0f, 0f);

            PortalDestination destination = root.AddComponent<PortalDestination>();
            SerializedObject destinationObject = new SerializedObject(destination);
            destinationObject.FindProperty("portalId").stringValue = portalId;
            destinationObject.FindProperty("spawnPoint").objectReferenceValue = spawnPoint.transform;
            destinationObject.FindProperty("useSpawnRotation").boolValue = false;
            destinationObject.ApplyModifiedPropertiesWithoutUndo();

            GameObject triggerGo = new GameObject($"{name}_Trigger");
            triggerGo.transform.SetParent(root.transform, false);
            triggerGo.transform.localPosition = Vector3.zero;

            BoxCollider2D triggerCollider = triggerGo.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector2(2f, 2.8f);

            Rigidbody2D triggerBody = triggerGo.AddComponent<Rigidbody2D>();
            triggerBody.bodyType = RigidbodyType2D.Kinematic;
            triggerBody.gravityScale = 0f;

            PortalTrigger2D trigger = triggerGo.AddComponent<PortalTrigger2D>();
            SerializedObject triggerObject = new SerializedObject(trigger);
            triggerObject.FindProperty("sourcePortalId").stringValue = portalId;
            SerializedProperty transition = triggerObject.FindProperty("transition");
            transition.FindPropertyRelative("mode").enumValueIndex = (int)PortalTransitionMode.SameSceneDestination;
            transition.FindPropertyRelative("sourcePortalId").stringValue = portalId;
            transition.FindPropertyRelative("destinationPortalId").stringValue = destinationId;
            triggerObject.FindProperty("filterMode").enumValueIndex = (int)PortalTrigger2D.FilterMode.Tag;
            triggerObject.FindProperty("requiredTag").stringValue = "Player";
            triggerObject.FindProperty("cooldownSeconds").floatValue = 0.35f;
            triggerObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreatePlayer()
        {
            GameObject player = CreateColoredSprite("Player", new Color(0.4f, 1f, 0.55f), new Vector3(-3f, -2.5f, 0f), new Vector2(0.8f, 0.8f));
            player.tag = "Player";

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;

            player.AddComponent<PortalTestPlayer2D>();
        }

        private static GameObject CreateColoredSprite(string name, Color color, Vector3 position, Vector2 size)
        {
            GameObject go = new GameObject(name);
            go.transform.position = position;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            renderer.color = color;
            renderer.sortingOrder = 0;
            return go;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].path == scenePath)
                    return;
            }

            EditorBuildSettingsScene[] updated = new EditorBuildSettingsScene[scenes.Length + 1];
            for (int i = 0; i < scenes.Length; i++)
                updated[i] = scenes[i];

            updated[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = updated;
        }
    }
}
#endif
