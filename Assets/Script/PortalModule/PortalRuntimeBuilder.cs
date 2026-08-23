using UnityEngine;

namespace PortalModule
{
    public static class PortalRuntimeBuilder
    {
        public struct PortalSideBuild
        {
            public GameObject root;
            public PortalDestination destination;
            public PortalTrigger trigger;
        }

        public static PortalSideBuild CreatePortalSide(
            string name,
            Vector3 position,
            Quaternion rotation,
            Color color,
            string portalId,
            string destinationPortalId,
            float spawnOffsetDistance,
            float cooldownSeconds)
        {
            GameObject root = new GameObject(name);
            root.transform.SetPositionAndRotation(position, rotation);

            CreateRingVisual(root.transform, color);
            CreatePadVisual(root.transform, color);

            Vector3 localSpawnOffset = Vector3.forward * spawnOffsetDistance;
            GameObject spawnPointGo = new GameObject("SpawnPoint");
            spawnPointGo.transform.SetParent(root.transform, false);
            spawnPointGo.transform.localPosition = localSpawnOffset + new Vector3(0f, 1f, 0f);
            spawnPointGo.transform.localRotation = Quaternion.LookRotation(-localSpawnOffset.normalized, Vector3.up);

            PortalDestination destination = root.AddComponent<PortalDestination>();
            destination.Configure(portalId, spawnPointGo.transform, true);

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
            var transition = new PortalTransitionSettings
            {
                mode = PortalTransitionMode.SameSceneDestination,
                sourcePortalId = portalId,
                destinationPortalId = destinationPortalId
            };
            trigger.Configure(portalId, transition, PortalTrigger.FilterMode.Tag, "Player", cooldownSeconds);

            return new PortalSideBuild
            {
                root = root,
                destination = destination,
                trigger = trigger
            };
        }

        public static GameObject CreatePreviewVisual(Color color, float alpha)
        {
            GameObject root = new GameObject("PortalPreview");

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.SetParent(root.transform, false);
            ring.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            ring.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            ring.transform.localScale = new Vector3(2.2f, 0.18f, 2.2f);
            Object.Destroy(ring.GetComponent<Collider>());
            ApplyColor(ring, color, alpha);

            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pad.name = "Pad";
            pad.transform.SetParent(root.transform, false);
            pad.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            pad.transform.localScale = new Vector3(2.4f, 0.05f, 2.4f);
            Object.Destroy(pad.GetComponent<Collider>());
            Color padColor = new Color(color.r, color.g, color.b, 1f) * 0.55f + new Color(0.1f, 0.1f, 0.1f, 0f);
            ApplyColor(pad, padColor, alpha);

            GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arrow.name = "DirectionArrow";
            arrow.transform.SetParent(root.transform, false);
            arrow.transform.localPosition = new Vector3(0f, 1.1f, 1.35f);
            arrow.transform.localScale = new Vector3(0.35f, 0.25f, 1.1f);
            Object.Destroy(arrow.GetComponent<Collider>());
            ApplyColor(arrow, new Color(color.r, color.g, color.b, 1f), Mathf.Min(1f, alpha + 0.25f));

            GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tip.name = "DirectionTip";
            tip.transform.SetParent(root.transform, false);
            tip.transform.localPosition = new Vector3(0f, 1.1f, 2.05f);
            tip.transform.localScale = new Vector3(0.55f, 0.25f, 0.35f);
            Object.Destroy(tip.GetComponent<Collider>());
            ApplyColor(tip, Color.white, Mathf.Min(1f, alpha + 0.35f));

            SetDirectionVisible(root, false);
            return root;
        }

        public static void UpdatePreviewVisual(
            GameObject preview,
            Vector3 position,
            Quaternion rotation,
            Color color,
            float alpha,
            bool showDirection)
        {
            if (preview == null)
                return;

            preview.transform.SetPositionAndRotation(position, rotation);
            SetDirectionVisible(preview, showDirection);

            Renderer[] renderers = preview.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                string objectName = renderers[i].gameObject.name;
                if (objectName == "DirectionArrow" || objectName == "DirectionTip")
                    continue;

                ApplyColor(renderers[i].gameObject, color, alpha);
            }

            if (showDirection)
            {
                Transform arrow = preview.transform.Find("DirectionArrow");
                if (arrow != null)
                    ApplyColor(arrow.gameObject, new Color(color.r, color.g, color.b, 1f), Mathf.Min(1f, alpha + 0.25f));

                Transform tip = preview.transform.Find("DirectionTip");
                if (tip != null)
                    ApplyColor(tip.gameObject, Color.white, Mathf.Min(1f, alpha + 0.35f));
            }
        }

        private static void SetDirectionVisible(GameObject preview, bool visible)
        {
            Transform arrow = preview.transform.Find("DirectionArrow");
            if (arrow != null)
                arrow.gameObject.SetActive(visible);

            Transform tip = preview.transform.Find("DirectionTip");
            if (tip != null)
                tip.gameObject.SetActive(visible);
        }

        public static GameObject CreatePreviewVisual(Transform parent, Color color, float alpha)
        {
            GameObject preview = CreatePreviewVisual(color, alpha);
            preview.transform.SetParent(parent, false);
            return preview;
        }

        private static void CreateRingVisual(Transform parent, Color color)
        {
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.SetParent(parent, false);
            ring.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            ring.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            ring.transform.localScale = new Vector3(2.2f, 0.18f, 2.2f);
            Object.Destroy(ring.GetComponent<Collider>());
            ApplyColor(ring, color, 1f);
        }

        private static void CreatePadVisual(Transform parent, Color color)
        {
            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pad.name = "Pad";
            pad.transform.SetParent(parent, false);
            pad.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            pad.transform.localScale = new Vector3(2.4f, 0.05f, 2.4f);
            Object.Destroy(pad.GetComponent<Collider>());
            Color padColor = new Color(color.r, color.g, color.b, 1f) * 0.55f + new Color(0.1f, 0.1f, 0.1f, 0f);
            ApplyColor(pad, padColor, 1f);
        }

        private static void ApplyColor(GameObject go, Color color, float alpha)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer == null)
                return;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            Material material = new Material(shader);
            Color finalColor = new Color(color.r, color.g, color.b, alpha);
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", finalColor);
                if (material.HasProperty("_Surface"))
                    material.SetFloat("_Surface", 1f);
            }
            else
            {
                material.color = finalColor;
                if (alpha < 1f)
                {
                    material.SetFloat("_Mode", 3f);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                }
            }

            renderer.sharedMaterial = material;
        }
    }
}
