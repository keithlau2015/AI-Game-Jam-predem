using System;
using System.Collections.Generic;
using System.IO;
using Model;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace PortalModule
{
    public class PortalService : Singleton<PortalService>
    {
        public static event Action<PortalTeleportContext> OnBeforeTeleport;
        public static event Action<PortalTeleportContext> OnAfterTeleport;
        public static event Action<string> OnCrossSceneArrivalWithoutTraveler;

        private readonly Dictionary<string, PortalDestination> destinationsById = new Dictionary<string, PortalDestination>();
        private readonly Dictionary<int, float> globalCooldownUntilByTraveler = new Dictionary<int, float>();
        private PendingCrossSceneTeleport pendingCrossScene;

        private struct PendingCrossSceneTeleport
        {
            public bool active;
            public string destinationPortalId;
            public string sourcePortalId;
            public string travelerTag;
            public string expectedSceneName;
        }

        protected override void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            RefreshDestinations();
        }

        protected override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (instance == this)
                instance = null;
        }

        public void RefreshDestinations()
        {
            PortalDestination[] destinations = FindObjectsOfType<PortalDestination>(true);
            for (int i = 0; i < destinations.Length; i++)
                RegisterDestination(destinations[i]);
        }

        public static PortalService Resolve()
        {
            if (instance != null)
                return instance;

            PortalService sceneService = FindObjectOfType<PortalService>();
            if (sceneService != null)
            {
                instance = sceneService;
                return sceneService;
            }

            return singleton;
        }

        public void RegisterDestination(PortalDestination destination)
        {
            if (destination == null || string.IsNullOrEmpty(destination.PortalId))
                return;

            if (destinationsById.TryGetValue(destination.PortalId, out PortalDestination existing)
                && existing != null
                && existing != destination)
            {
                Debug.LogWarning($"[PortalService] Duplicate portal id '{destination.PortalId}'. Overwriting registration.", destination);
            }

            destinationsById[destination.PortalId] = destination;
        }

        public void UnregisterDestination(PortalDestination destination)
        {
            if (destination == null || string.IsNullOrEmpty(destination.PortalId))
                return;

            if (destinationsById.TryGetValue(destination.PortalId, out PortalDestination existing) && existing == destination)
                destinationsById.Remove(destination.PortalId);
        }

        public bool TryGetDestination(string portalId, out PortalDestination destination)
        {
            destination = null;
            if (string.IsNullOrEmpty(portalId))
                return false;

            return destinationsById.TryGetValue(portalId, out destination) && destination != null;
        }

        public bool CanUsePortal(GameObject traveler)
        {
            if (traveler == null)
                return false;

            int travelerId = traveler.GetInstanceID();
            return !globalCooldownUntilByTraveler.TryGetValue(travelerId, out float cooldownUntil)
                || Time.time >= cooldownUntil;
        }

        public void RegisterCooldown(GameObject traveler, float cooldownSeconds)
        {
            if (traveler == null || cooldownSeconds <= 0f)
                return;

            globalCooldownUntilByTraveler[traveler.GetInstanceID()] = Time.time + cooldownSeconds;
        }

        public bool ExecuteTransition(PortalTransitionSettings settings, GameObject traveler, PortalTrigger sourceTrigger = null)
        {
            if (settings == null || traveler == null)
                return false;

            switch (settings.mode)
            {
                case PortalTransitionMode.SameSceneDestination:
                    return Teleport(traveler, settings.destinationPortalId, sourceTrigger, settings.sourcePortalId);

                case PortalTransitionMode.LoadSceneByName:
                    return TeleportToScene(traveler, settings.targetSceneName, settings.destinationPortalId, settings.sourcePortalId, sourceTrigger);

                case PortalTransitionMode.LoadLevelByKey:
                    return TeleportViaLevelKey(traveler, settings.targetLevelKey, settings.destinationPortalId, settings.sourcePortalId, sourceTrigger);

                default:
                    return false;
            }
        }

        public bool Teleport(GameObject traveler, string destinationPortalId, PortalTrigger sourceTrigger = null, string sourcePortalId = null)
        {
            if (traveler == null || string.IsNullOrEmpty(destinationPortalId))
                return false;

            if (!TryGetDestination(destinationPortalId, out PortalDestination destination))
            {
                Debug.LogWarning($"[PortalService] Destination '{destinationPortalId}' not found.", sourceTrigger);
                return false;
            }

            return ApplyTeleport(traveler, destination, sourceTrigger, false, sourcePortalId);
        }

        public bool TeleportToScene(GameObject traveler, string sceneName, string destinationPortalId, string sourcePortalId = null, PortalTrigger sourceTrigger = null)
        {
            if (traveler == null || string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(destinationPortalId))
                return false;

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == sceneName)
                return Teleport(traveler, destinationPortalId, sourceTrigger, sourcePortalId);

            QueueCrossSceneTeleport(sceneName, destinationPortalId, sourcePortalId, traveler.tag);

            var context = new PortalTeleportContext
            {
                traveler = traveler,
                sourceTrigger = sourceTrigger,
                destinationPortalId = destinationPortalId,
                sourcePortalId = sourcePortalId ?? (sourceTrigger != null ? sourceTrigger.SourcePortalId : null),
                isCrossScene = true
            };
            OnBeforeTeleport?.Invoke(context);
            SceneManager.LoadScene(sceneName);
            return true;
        }

        private bool TeleportViaLevelKey(GameObject traveler, string levelKey, string destinationPortalId, string sourcePortalId, PortalTrigger sourceTrigger)
        {
            if (traveler == null || string.IsNullOrEmpty(levelKey) || string.IsNullOrEmpty(destinationPortalId))
                return false;

            if (!LevelModel.map.TryGetValue(levelKey, out LevelModel levelModel))
            {
                Debug.LogError($"[PortalService] Unknown level key '{levelKey}'.", sourceTrigger);
                return false;
            }

            string expectedSceneName = string.Empty;
            if (levelModel.sceneIndex >= 0 && levelModel.sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(levelModel.sceneIndex);
                expectedSceneName = Path.GetFileNameWithoutExtension(scenePath);
            }

            QueueCrossSceneTeleport(expectedSceneName, destinationPortalId, sourcePortalId, traveler.tag, allowAnyScene: string.IsNullOrEmpty(expectedSceneName));

            var context = new PortalTeleportContext
            {
                traveler = traveler,
                sourceTrigger = sourceTrigger,
                destinationPortalId = destinationPortalId,
                sourcePortalId = sourcePortalId ?? (sourceTrigger != null ? sourceTrigger.SourcePortalId : null),
                isCrossScene = true
            };
            OnBeforeTeleport?.Invoke(context);

            if (GameStateController.singleton != null && GameStateController.singleton.stateMachine != null)
                return GameStateController.singleton.stateMachine.LoadLevel(levelKey);

            if (levelModel.sceneIndex < 0 || levelModel.sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"[PortalService] Invalid sceneIndex {levelModel.sceneIndex} for level '{levelKey}'.", sourceTrigger);
                pendingCrossScene = default;
                return false;
            }

            SceneManager.LoadScene(levelModel.sceneIndex);
            return true;
        }

        private void QueueCrossSceneTeleport(string expectedSceneName, string destinationPortalId, string sourcePortalId, string travelerTag, bool allowAnyScene = false)
        {
            pendingCrossScene = new PendingCrossSceneTeleport
            {
                active = true,
                destinationPortalId = destinationPortalId,
                sourcePortalId = sourcePortalId,
                travelerTag = travelerTag,
                expectedSceneName = allowAnyScene ? string.Empty : expectedSceneName
            };
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!pendingCrossScene.active)
                return;

            if (!string.IsNullOrEmpty(pendingCrossScene.expectedSceneName)
                && scene.name != pendingCrossScene.expectedSceneName)
                return;

            PendingCrossSceneTeleport pending = pendingCrossScene;
            pendingCrossScene = default;

            if (!TryGetDestination(pending.destinationPortalId, out PortalDestination destination))
            {
                Debug.LogWarning($"[PortalService] Cross-scene destination '{pending.destinationPortalId}' not found in '{scene.name}'.");
                OnCrossSceneArrivalWithoutTraveler?.Invoke(pending.destinationPortalId);
                return;
            }

            GameObject traveler = FindTraveler(pending.travelerTag);
            if (traveler == null)
            {
                Debug.LogWarning($"[PortalService] Traveler tag '{pending.travelerTag}' not found in '{scene.name}'.");
                OnCrossSceneArrivalWithoutTraveler?.Invoke(pending.destinationPortalId);
                return;
            }

            ApplyTeleport(traveler, destination, null, true, pending.sourcePortalId);
        }

        private GameObject FindTraveler(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return null;

            try
            {
                return GameObject.FindGameObjectWithTag(tag);
            }
            catch (UnityException)
            {
                return null;
            }
        }

        private bool ApplyTeleport(GameObject traveler, PortalDestination destination, PortalTrigger sourceTrigger, bool isCrossScene, string sourcePortalId = null)
        {
            var context = new PortalTeleportContext
            {
                traveler = traveler,
                sourceTrigger = sourceTrigger,
                destination = destination,
                destinationPortalId = destination.PortalId,
                sourcePortalId = sourcePortalId ?? (sourceTrigger != null ? sourceTrigger.SourcePortalId : null),
                isCrossScene = isCrossScene
            };

            OnBeforeTeleport?.Invoke(context);

            IPortalTeleportable[] customHandlers = traveler.GetComponentsInParent<IPortalTeleportable>(true);
            for (int i = 0; i < customHandlers.Length; i++)
            {
                if (!customHandlers[i].OnBeforePortalTeleport(context))
                    return false;
            }

            Vector3 position = destination.GetSpawnPosition();
            Quaternion rotation = destination.GetSpawnRotation();
            NavMeshAgent agent = traveler.GetComponentInParent<NavMeshAgent>();
            Rigidbody2D body2D = traveler.GetComponentInParent<Rigidbody2D>();

            if (agent != null && agent.enabled)
            {
                agent.Warp(position);
                if (useSpawnRotationForAgent(agent))
                    agent.transform.rotation = rotation;
            }
            else if (body2D != null)
            {
                body2D.velocity = Vector2.zero;
                body2D.angularVelocity = 0f;
                body2D.MovePosition(position);
                body2D.rotation = rotation.eulerAngles.z;
            }
            else
            {
                traveler.transform.SetPositionAndRotation(position, rotation);
            }

            for (int i = 0; i < customHandlers.Length; i++)
                customHandlers[i].OnAfterPortalTeleport(context);

            OnAfterTeleport?.Invoke(context);
            return true;
        }

        private static bool useSpawnRotationForAgent(NavMeshAgent agent)
        {
            return agent.updateRotation;
        }
    }
}
