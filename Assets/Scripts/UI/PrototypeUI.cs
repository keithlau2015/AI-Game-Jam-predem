using UnityEngine;
using UnityEngine.UI;
using System;

namespace PortalEscort.UI
{
    /// <summary>
    /// Prototype HUD. Subscribes ONLY to GameManager / PortalPlacementController events.
    /// Never calls gameplay internals. Degrades gracefully if those objects are absent.
    /// </summary>
    public class PrototypeUI : MonoBehaviour
    {
        [Header("Optional UI elements (auto-found by name if left null)")]
        [SerializeField] private Text portalStateText;
        [SerializeField] private Text hintText;
        [SerializeField] private Text cooldownText;
        [SerializeField] private Text rescuedText;
        [SerializeField] private Text bannerText;
        [SerializeField] private Button retryButton;

        [Header("Fallback state (used if PortalPlacementController is absent in-scene)")]
        [SerializeField] private string portalStateFallback = "Idle";

        [Header("Hints per portal state")]
        [SerializeField] private string hintIdle = "Click an entrance cell to start placing a portal.";
        [SerializeField] private string hintSelectingEntrance = "Pick an ENTRANCE cell, then choose its direction.";
        [SerializeField] private string hintSelectingExit = "Pick an EXIT cell within range, then choose its direction.";
        [SerializeField] private string hintActiveLocked = "Portal active. Wait for cooldown to reconfigure.";
        [SerializeField] private string hintReconfigurable = "Portal active. Reconfiguration available.";

        private GameManager gameManager;
        private PortalPlacementController placementController;

        private string currentPortalState;
        private float cooldownRemaining;
        private bool cooldownActive;

        private void Awake()
        {
            FindReferences();
            AutoWireUI();
            WireRetryButton();
        }

        private void Start()
        {
            FindReferences();
            Subscribe();
            UpdateRescuedCounter();
            RefreshPortalState();
            HideBanner();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Update()
        {
            if (cooldownActive)
            {
                cooldownRemaining -= Time.deltaTime;
                if (cooldownRemaining <= 0f)
                {
                    cooldownRemaining = 0f;
                    cooldownActive = false;
                }
                UpdateCooldownDisplay();
            }
        }

        private void FindReferences()
        {
            if (gameManager == null)
                gameManager = FindObjectOfType<GameManager>();
            if (placementController == null)
                placementController = FindObjectOfType<PortalPlacementController>();
        }

        private void AutoWireUI()
        {
            Transform root = transform;
            portalStateText = EnsureText(portalStateText, root, "PortalStateText");
            hintText = EnsureText(hintText, root, "HintText");
            cooldownText = EnsureText(cooldownText, root, "CooldownText");
            rescuedText = EnsureText(rescuedText, root, "RescuedText");
            bannerText = EnsureText(bannerText, root, "BannerText");
            if (retryButton == null)
                retryButton = GetComponentInChildren<Button>(true);
        }

        private static Text EnsureText(Text field, Transform root, string childName)
        {
            if (field != null) return field;
            Transform found = root.Find(childName);
            return found != null ? found.GetComponent<Text>() : null;
        }

        private void WireRetryButton()
        {
            if (retryButton != null && gameManager != null)
                retryButton.onClick.AddListener(OnRetryClicked);
        }

        private void Subscribe()
        {
            FindReferences();
            if (gameManager != null)
            {
                gameManager.OnEscortRescued += HandleEscortRescued;
                gameManager.OnEscortDied += HandleEscortDied;
                gameManager.OnGameClear += HandleGameClear;
                gameManager.OnGameFail += HandleGameFail;
            }
            if (placementController != null)
            {
                placementController.OnPortalStateChanged += HandlePortalStateChanged;
            }
        }

        private void Unsubscribe()
        {
            if (gameManager != null)
            {
                gameManager.OnEscortRescued -= HandleEscortRescued;
                gameManager.OnEscortDied -= HandleEscortDied;
                gameManager.OnGameClear -= HandleGameClear;
                gameManager.OnGameFail -= HandleGameFail;
            }
            if (placementController != null)
            {
                placementController.OnPortalStateChanged -= HandlePortalStateChanged;
            }
        }

        // ---- Event handlers (event signatures per Contracts.md §8) ----

        private void HandleEscortRescued(EscortTarget t)
        {
            UpdateRescuedCounter();
        }

        private void HandleEscortDied(EscortTarget t)
        {
            UpdateRescuedCounter();
        }

        private void HandleGameClear()
        {
            ShowBanner("LEVEL CLEAR!");
        }

        private void HandleGameFail()
        {
            ShowBanner("LEVEL FAILED");
        }

        private void HandlePortalStateChanged(string state)
        {
            SetPortalState(state);
        }

        // ---- UI updates ----

        private void SetPortalState(string state)
        {
            currentPortalState = state;
            RefreshPortalState();
        }

        private void RefreshPortalState()
        {
            string state = currentPortalState ?? portalStateFallback;
            if (portalStateText != null)
                portalStateText.text = "Portal: " + state;

            if (hintText != null)
                hintText.text = HintForState(state);

            // Cooldown trigger: entering a reconfiguring/cooldown state starts the timer.
            bool enteringCooldown = state.IndexOf("econfig", StringComparison.OrdinalIgnoreCase) >= 0
                                     || state.IndexOf("cooldown", StringComparison.OrdinalIgnoreCase) >= 0;
            if (enteringCooldown && !cooldownActive)
            {
                float cd = placementController != null
                    ? placementController.reconfigurationCooldown
                    : 3f;
                cooldownRemaining = cd;
                cooldownActive = true;
                UpdateCooldownDisplay();
            }
        }

        private string HintForState(string state)
        {
            if (state.IndexOf("SelectingEntrance", StringComparison.OrdinalIgnoreCase) >= 0)
                return hintSelectingEntrance;
            if (state.IndexOf("SelectingExit", StringComparison.OrdinalIgnoreCase) >= 0)
                return hintSelectingExit;
            if (state.IndexOf("ActiveLocked", StringComparison.OrdinalIgnoreCase) >= 0)
                return hintActiveLocked;
            if (state.IndexOf("Reconfigurable", StringComparison.OrdinalIgnoreCase) >= 0)
                return hintReconfigurable;
            return hintIdle;
        }

        private void UpdateCooldownDisplay()
        {
            if (cooldownText == null) return;
            cooldownText.text = cooldownActive
                ? string.Format("Reconfigure in: {0:0.0}s", cooldownRemaining)
                : "Reconfigure: ready";
        }

        private void UpdateRescuedCounter()
        {
            if (rescuedText == null) return;
            int rescued = gameManager != null ? gameManager.rescuedCount : 0;
            int total = gameManager != null ? gameManager.totalEscortCount : 0;
            rescuedText.text = string.Format("Rescued: {0} / {1}", rescued, total);
        }

        private void ShowBanner(string message)
        {
            if (bannerText == null) return;
            bannerText.text = message;
            bannerText.enabled = true;
            if (bannerText.transform is RectTransform)
                bannerText.gameObject.SetActive(true);
        }

        private void HideBanner()
        {
            if (bannerText == null) return;
            bannerText.enabled = false;
            bannerText.gameObject.SetActive(false);
        }

        private void OnRetryClicked()
        {
            if (gameManager != null)
                gameManager.RestartLevel();
        }
    }
}
