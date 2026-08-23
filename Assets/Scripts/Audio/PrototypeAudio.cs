using UnityEngine;

namespace Audio
{
    /// <summary>
    /// PrototypeAudio — event-driven SFX cue player for the Portal Escort prototype.
    /// Subscribes to GameManager events and exposes public play methods for other agents
    /// (Transition, UI/UX, Gameplay) to fire transient cues. Contains no gameplay logic.
    /// </summary>
    public class PrototypeAudio : MonoBehaviour
    {
        [Header("Cues")]
        [Tooltip("Played on escort spawn (call PlaySpawn from spawner/Gameplay).")]
        public AudioClip spawnCue;
        [Tooltip("Played on successful teleport (Transition calls PlayTeleport).")]
        public AudioClip teleportCue;
        [Tooltip("Played when an escort is rescued (GameManager.OnEscortRescued).")]
        public AudioClip rescueCue;
        [Tooltip("Played when an escort dies (GameManager.OnEscortDied).")]
        public AudioClip deathCue;
        [Tooltip("Played when the level fails (GameManager.OnGameFail).")]
        public AudioClip failCue;
        [Tooltip("Played when reconfiguration cooldown ends (Transition calls PlayCooldownEnd).")]
        public AudioClip cooldownEndCue;
        [Tooltip("Played when a turret fires (Gameplay calls PlayTurretFire).")]
        public AudioClip turretFireCue;
        [Tooltip("Played on invalid placement/action (UI/UX calls PlayInvalid).")]
        public AudioClip invalidCue;
        [Tooltip("Played when the level is cleared (GameManager.OnGameClear).")]
        public AudioClip clearCue;

        [Header("Mixer (prototype)")]
        [Range(0f, 1f)]
        public float volume = 1f;

        private AudioSource audioSource;
        private GameManager gameManager;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.volume = volume;
        }

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnEscortRescued.AddListener(HandleEscortRescued);
                gameManager.OnEscortDied.AddListener(HandleEscortDied);
                gameManager.OnGameClear.AddListener(HandleGameClear);
                gameManager.OnGameFail.AddListener(HandleGameFail);
            }
            else
            {
                Debug.LogWarning("[PrototypeAudio] GameManager not found; cues will only fire via public methods.");
            }
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnEscortRescued.RemoveListener(HandleEscortRescued);
                gameManager.OnEscortDied.RemoveListener(HandleEscortDied);
                gameManager.OnGameClear.RemoveListener(HandleGameClear);
                gameManager.OnGameFail.RemoveListener(HandleGameFail);
            }
        }

        // --- GameManager event handlers (defensive; ignore payloads) ---
        private void HandleEscortRescued(EscortTarget escort) { PlayClip(rescueCue); }
        private void HandleEscortDied(EscortTarget escort) { PlayClip(deathCue); }
        private void HandleGameClear() { PlayClip(clearCue); }
        private void HandleGameFail() { PlayClip(failCue); }

        // --- Public cue methods (other agents call these) ---
        public void PlaySpawn() { PlayClip(spawnCue); }
        public void PlayTeleport() { PlayClip(teleportCue); }
        public void PlayCooldownEnd() { PlayClip(cooldownEndCue); }
        public void PlayTurretFire() { PlayClip(turretFireCue); }
        public void PlayInvalid() { PlayClip(invalidCue); }

        // --- Core play helper (null-guarded) ---
        private void PlayClip(AudioClip clip)
        {
            if (clip == null) return;
            if (audioSource == null) return;
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
