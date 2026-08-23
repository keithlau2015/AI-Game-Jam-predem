#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PortalModule.Editor
{
    public static class PortalTestTopDownSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/PortalTestTopDown.unity";

        [MenuItem("Tools/Portal/Build 2.5D Test Scene")]
        public static void BuildScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            CreateLight();
            CreateArena();
            CreatePortalService();
            CreateGameRules();
            CreatePlayer();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[PortalTestTopDown] 2.5D scene saved. Press Play, use WASD to move, Space + mouse to place portals.");
        }

        public static void ApplyMazeToOpenScene()
        {
            RemoveLegacyArenaBlocks();

            GameObject existingMaze = GameObject.Find("Maze");
            if (existingMaze != null)
                Object.DestroyImmediate(existingMaze);

            CreateSimpleMaze();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = GetMazeStartPosition();

            GameObject goal = GameObject.Find("LevelGoal");
            if (goal != null)
                goal.transform.position = GetMazeGoalPosition();

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                mainCamera.orthographicSize = 12f;
        }

        private static void RemoveLegacyArenaBlocks()
        {
            string[] legacyNames =
            {
                "Block_A",
                "Block_B",
                "Pillar_L",
                "Pillar_R",
            };

            for (int i = 0; i < legacyNames.Length; i++)
            {
                GameObject block = GameObject.Find(legacyNames[i]);
                if (block != null)
                    Object.DestroyImmediate(block);
            }
        }

        private static void CreateCamera()
        {
            GameObject cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            Camera camera = cameraGo.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 12f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 80f;
            camera.backgroundColor = new Color(0.1f, 0.12f, 0.16f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraGo.transform.position = new Vector3(0f, 22f, -10f);
            cameraGo.transform.rotation = Quaternion.Euler(65f, 0f, 0f);
            cameraGo.AddComponent<AudioListener>();
        }

        private static void CreateLight()
        {
            GameObject lightGo = new GameObject("Directional Light");
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.9f);
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void CreateArena()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(3f, 1f, 3f);
            ApplyColor(floor, new Color(0.22f, 0.26f, 0.3f));

            CreateWall("Wall_North", new Vector3(0f, 1f, 14.5f), new Vector3(30f, 2f, 1f));
            CreateWall("Wall_South", new Vector3(0f, 1f, -14.5f), new Vector3(30f, 2f, 1f));
            CreateWall("Wall_East", new Vector3(14.5f, 1f, 0f), new Vector3(1f, 2f, 28f));
            CreateWall("Wall_West", new Vector3(-14.5f, 1f, 0f), new Vector3(1f, 2f, 28f));

            CreateSimpleMaze();
        }

        private static void CreateSimpleMaze()
        {
            GameObject mazeRoot = new GameObject("Maze");
            const float cellSize = 2f;
            const int width = 15;
            const int height = 11;
            int[,] cells =
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,1,0,0,0,0,0,1,0,0,0,1},
                {1,0,1,1,1,0,1,1,1,0,1,0,1,0,1},
                {1,0,0,0,0,0,1,0,0,0,0,0,1,0,1},
                {1,1,1,0,1,1,1,0,1,1,1,1,1,0,1},
                {1,0,0,0,0,0,0,0,1,0,0,0,0,0,1},
                {1,0,1,1,1,1,1,0,1,0,1,1,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,0,1,1,1,1,1,1,1,0,1,1,1},
                {1,0,0,0,1,0,0,0,0,0,1,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            };

            float originX = -(width - 1) * cellSize * 0.5f;
            float originZ = -(height - 1) * cellSize * 0.5f;
            Color wallColor = new Color(0.32f, 0.36f, 0.42f);

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (cells[z, x] == 0)
                        continue;

                    Vector3 position = new Vector3(originX + x * cellSize, 0.75f, originZ + z * cellSize);
                    CreateBlock($"MazeWall_{x}_{z}", position, new Vector3(cellSize, 1.5f, cellSize), wallColor, mazeRoot.transform);
                }
            }
        }

        public static Vector3 GetMazeStartPosition()
        {
            return GetMazeCellWorldPosition(1, 1) + new Vector3(0f, 1f, 0f);
        }

        public static Vector3 GetMazeGoalPosition()
        {
            return GetMazeCellWorldPosition(13, 9) + new Vector3(0f, 1f, 0f);
        }

        private static Vector3 GetMazeCellWorldPosition(int gridX, int gridZ)
        {
            const float cellSize = 2f;
            const int width = 15;
            const int height = 11;
            float originX = -(width - 1) * cellSize * 0.5f;
            float originZ = -(height - 1) * cellSize * 0.5f;
            return new Vector3(originX + gridX * cellSize, 0f, originZ + gridZ * cellSize);
        }

        private static void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            CreateBlock(name, position, scale, new Color(0.18f, 0.2f, 0.24f));
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale, Color color, Transform parent = null)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.position = position;
            block.transform.localScale = scale;
            if (parent != null)
                block.transform.SetParent(parent, true);
            ApplyColor(block, color);
            block.AddComponent<PortalWallHazard>();
            return block;
        }

        private static void CreateGameRules()
        {
            GameObject rulesGo = new GameObject("GameRules");
            rulesGo.AddComponent<PortalGameRuleController>();

            GameObject goalGo = new GameObject("LevelGoal");
            goalGo.transform.position = GetMazeGoalPosition();

            BoxCollider goalCollider = goalGo.AddComponent<BoxCollider>();
            goalCollider.isTrigger = true;
            goalCollider.size = new Vector3(4f, 2f, 2f);

            Rigidbody goalBody = goalGo.AddComponent<Rigidbody>();
            goalBody.isKinematic = true;
            goalBody.useGravity = false;

            PortalLevelGoalTrigger goalTrigger = goalGo.AddComponent<PortalLevelGoalTrigger>();
            SerializedObject goalObject = new SerializedObject(goalTrigger);
            SerializedProperty advance = goalObject.FindProperty("advanceSettings");
            advance.FindPropertyRelative("mode").enumValueIndex = (int)PortalLevelAdvanceMode.NextBuildSettingsScene;
            goalObject.FindProperty("filterMode").enumValueIndex = (int)PortalLevelGoalTrigger.FilterMode.Tag;
            goalObject.FindProperty("requiredTag").stringValue = "Player";
            goalObject.ApplyModifiedPropertiesWithoutUndo();

            GameObject goalVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goalVisual.name = "GoalVisual";
            goalVisual.transform.SetParent(goalGo.transform, false);
            goalVisual.transform.localPosition = Vector3.zero;
            goalVisual.transform.localScale = new Vector3(4f, 0.15f, 2f);
            Object.DestroyImmediate(goalVisual.GetComponent<Collider>());
            ApplyColor(goalVisual, new Color(0.35f, 0.95f, 0.45f));
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
                new Vector3(-8f, 0f, 0f),
                "Portal_A",
                "Portal_B",
                new Color(0.25f, 0.75f, 1f),
                new Vector3(2.8f, 0f, 0f));

            CreatePortalSide(
                "Portal_B",
                new Vector3(8f, 0f, 0f),
                "Portal_B",
                "Portal_A",
                new Color(1f, 0.5f, 0.25f),
                new Vector3(-2.8f, 0f, 0f));
        }

        private static void CreatePortalSide(
            string name,
            Vector3 position,
            string portalId,
            string destinationId,
            Color color,
            Vector3 spawnLocalOffset)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = $"{name}_Ring";
            ring.transform.SetParent(root.transform, false);
            ring.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            ring.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            ring.transform.localScale = new Vector3(2.2f, 0.18f, 2.2f);
            Object.DestroyImmediate(ring.GetComponent<Collider>());
            ApplyColor(ring, color);

            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pad.name = $"{name}_Pad";
            pad.transform.SetParent(root.transform, false);
            pad.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            pad.transform.localScale = new Vector3(2.4f, 0.05f, 2.4f);
            Object.DestroyImmediate(pad.GetComponent<Collider>());
            ApplyColor(pad, new Color(color.r, color.g, color.b, 1f) * 0.55f + new Color(0.1f, 0.1f, 0.1f, 0f));

            GameObject spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.SetParent(root.transform, false);
            spawnPoint.transform.localPosition = spawnLocalOffset + new Vector3(0f, 1f, 0f);
            spawnPoint.transform.localRotation = Quaternion.LookRotation(-spawnLocalOffset.normalized, Vector3.up);

            PortalDestination destination = root.AddComponent<PortalDestination>();
            SerializedObject destinationObject = new SerializedObject(destination);
            destinationObject.FindProperty("portalId").stringValue = portalId;
            destinationObject.FindProperty("spawnPoint").objectReferenceValue = spawnPoint.transform;
            destinationObject.FindProperty("useSpawnRotation").boolValue = true;
            destinationObject.ApplyModifiedPropertiesWithoutUndo();

            GameObject triggerGo = new GameObject($"{name}_Trigger");
            triggerGo.transform.SetParent(root.transform, false);
            triggerGo.transform.localPosition = new Vector3(0f, 1.1f, 0f);

            SphereCollider triggerCollider = triggerGo.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = 1.15f;

            Rigidbody triggerBody = triggerGo.AddComponent<Rigidbody>();
            triggerBody.isKinematic = true;
            triggerBody.useGravity = false;

            PortalTrigger trigger = triggerGo.AddComponent<PortalTrigger>();
            SerializedObject triggerObject = new SerializedObject(trigger);
            triggerObject.FindProperty("sourcePortalId").stringValue = portalId;
            SerializedProperty transition = triggerObject.FindProperty("transition");
            transition.FindPropertyRelative("mode").enumValueIndex = (int)PortalTransitionMode.SameSceneDestination;
            transition.FindPropertyRelative("sourcePortalId").stringValue = portalId;
            transition.FindPropertyRelative("destinationPortalId").stringValue = destinationId;
            triggerObject.FindProperty("filterMode").enumValueIndex = (int)PortalTrigger.FilterMode.Tag;
            triggerObject.FindProperty("requiredTag").stringValue = "Player";
            triggerObject.FindProperty("cooldownSeconds").floatValue = 0.5f;
            triggerObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = GetMazeStartPosition();
            ApplyColor(player, new Color(0.35f, 0.95f, 0.55f));

            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CapsuleCollider collider = player.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.5f;
            collider.center = Vector3.zero;

            Rigidbody body = player.AddComponent<Rigidbody>();
            body.mass = 1f;
            body.drag = 0f;
            body.angularDrag = 0.05f;
            body.useGravity = true;
            body.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
            body.interpolation = RigidbodyInterpolation.Interpolate;

            player.AddComponent<PortalTestPlayerTopDown>();
            player.AddComponent<PortalPlayerGameRuleSensor>();

            PortalTestPlayerTopDown movement = player.GetComponent<PortalTestPlayerTopDown>();
            SerializedObject movementObject = new SerializedObject(movement);
            movementObject.FindProperty("moveMode").enumValueIndex = (int)PortalPlayerMoveMode.ForwardOnly;
            movementObject.FindProperty("moveDirection").vector3Value = Vector3.forward;
            movementObject.FindProperty("directionIsLocal").boolValue = false;
            movementObject.FindProperty("autoMove").boolValue = true;
            movementObject.ApplyModifiedPropertiesWithoutUndo();

            PortalPlacementController placement = player.AddComponent<PortalPlacementController>();
            SerializedObject placementObject = new SerializedObject(placement);
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                placementObject.FindProperty("placementCamera").objectReferenceValue = mainCamera;
            placementObject.FindProperty("showControlsHint").boolValue = true;
            placementObject.ApplyModifiedPropertiesWithoutUndo();

            GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nose.name = "FacingMarker";
            nose.transform.SetParent(player.transform, false);
            nose.transform.localPosition = new Vector3(0f, 0.35f, 0.55f);
            nose.transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
            Object.DestroyImmediate(nose.GetComponent<Collider>());
            ApplyColor(nose, new Color(0.9f, 0.95f, 0.4f));
        }

        private static void ApplyColor(GameObject go, Color color)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer == null)
                return;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");
            Material material = new Material(shader);
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            else
                material.color = color;
            renderer.sharedMaterial = material;
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
