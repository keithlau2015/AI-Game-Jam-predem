using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PortalEscort.Core
{
    public enum GameState
    {
        Playing,
        Clear,
        Fail
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState gameState = GameState.Playing;

        [Header("Counters")]
        public int totalEscortCount;
        public int spawnedCount;
        public int aliveCount;
        public int rescuedCount;
        public int deadCount;

        [Header("Events (C#)")]
        public event Action<EscortTarget> OnEscortRescued;
        public event Action<EscortTarget> OnEscortDied;
        public event Action OnGameClear;
        public event Action OnGameFail;

        [Header("Scene")]
        public string sceneToLoad;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void RegisterSpawned(EscortTarget t)
        {
            spawnedCount++;
            aliveCount++;
            if (totalEscortCount < spawnedCount)
            {
                totalEscortCount = spawnedCount;
            }
        }

        public void OnEscortRescued(EscortTarget t)
        {
            if (t == null || !t.isAlive) return;
            t.isAlive = false;
            rescuedCount++;
            aliveCount = Mathf.Max(0, aliveCount - 1);

            OnEscortRescued?.Invoke(t);

            if (rescuedCount == totalEscortCount && deadCount == 0)
            {
                gameState = GameState.Clear;
                OnGameClear?.Invoke();
            }
        }

        public void OnEscortDied(EscortTarget t)
        {
            if (t == null || !t.isAlive) return;
            t.isAlive = false;
            deadCount++;
            aliveCount = Mathf.Max(0, aliveCount - 1);

            OnEscortDied?.Invoke(t);

            gameState = GameState.Fail;
            OnGameFail?.Invoke();
        }

        public void RestartLevel()
        {
            string sceneName = string.IsNullOrEmpty(sceneToLoad)
                ? SceneManager.GetActiveScene().name
                : sceneToLoad;
            SceneManager.LoadScene(sceneName);
        }
    }
}
