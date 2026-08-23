using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PortalModule
{
    public enum PortalGameOutcome
    {
        Playing,
        Victory,
        Defeat,
    }

    public class PortalGameRuleController : MonoBehaviour
    {
        public static PortalGameRuleController Instance { get; private set; }

        public static event Action<PortalGameOutcome> OnOutcomeChanged;

        [SerializeField]
        private float outcomeDelaySeconds = 0.35f;

        [SerializeField]
        private bool reloadSceneOnDefeat = true;

        private PortalGameOutcome outcome = PortalGameOutcome.Playing;
        private float outcomeTimer = -1f;
        private PortalLevelAdvanceSettings pendingAdvance;

        public PortalGameOutcome Outcome => outcome;
        public bool IsPlaying => outcome == PortalGameOutcome.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            if (outcomeTimer < 0f)
                return;

            outcomeTimer -= Time.unscaledDeltaTime;
            if (outcomeTimer > 0f)
                return;

            outcomeTimer = -1f;
            if (outcome == PortalGameOutcome.Defeat)
                HandleDefeatResolved();
            else if (outcome == PortalGameOutcome.Victory)
                HandleVictoryResolved();
        }

        public void RegisterDefeat(GameObject player)
        {
            if (!IsPlaying)
                return;

            pendingAdvance = null;
            SetOutcome(PortalGameOutcome.Defeat, player);
            outcomeTimer = outcomeDelaySeconds;
        }

        public void RegisterVictory(GameObject player, PortalLevelAdvanceSettings advanceSettings)
        {
            if (!IsPlaying)
                return;

            pendingAdvance = advanceSettings;
            SetOutcome(PortalGameOutcome.Victory, player);
            outcomeTimer = outcomeDelaySeconds;
        }

        private void SetOutcome(PortalGameOutcome newOutcome, GameObject player)
        {
            outcome = newOutcome;
            FreezePlayer(player);
            OnOutcomeChanged?.Invoke(outcome);
        }

        private static void FreezePlayer(GameObject player)
        {
            if (player == null)
                return;

            PortalTestPlayerTopDown topDown = player.GetComponent<PortalTestPlayerTopDown>();
            if (topDown != null)
                topDown.enabled = false;

            PortalTestPlayer2D player2D = player.GetComponent<PortalTestPlayer2D>();
            if (player2D != null)
                player2D.enabled = false;

            PortalPlacementController placement = player.GetComponent<PortalPlacementController>();
            if (placement != null)
                placement.enabled = false;

            Rigidbody body = player.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            Rigidbody2D body2D = player.GetComponent<Rigidbody2D>();
            if (body2D != null)
            {
                body2D.velocity = Vector2.zero;
                body2D.angularVelocity = 0f;
            }
        }

        private void HandleDefeatResolved()
        {
            if (reloadSceneOnDefeat)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleVictoryResolved()
        {
            if (pendingAdvance != null && pendingAdvance.TryAdvance())
                return;

            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextIndex);
        }
    }
}
