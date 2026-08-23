using System;
using Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PortalModule
{
    public enum PortalLevelAdvanceMode
    {
        NextBuildSettingsScene = 0,
        ReloadCurrentScene = 1,
        LoadSceneByName = 2,
        LoadSceneByBuildIndex = 3,
        LoadLevelByKey = 4,
    }

    [Serializable]
    public class PortalLevelAdvanceSettings
    {
        public PortalLevelAdvanceMode mode = PortalLevelAdvanceMode.NextBuildSettingsScene;
        public string sceneName;
        public int sceneBuildIndex = -1;
        public string levelKey;

        public bool TryAdvance()
        {
            switch (mode)
            {
                case PortalLevelAdvanceMode.ReloadCurrentScene:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return true;
                case PortalLevelAdvanceMode.LoadSceneByName:
                    if (string.IsNullOrEmpty(sceneName))
                        return false;
                    SceneManager.LoadScene(sceneName);
                    return true;
                case PortalLevelAdvanceMode.LoadSceneByBuildIndex:
                    if (sceneBuildIndex < 0 || sceneBuildIndex >= SceneManager.sceneCountInBuildSettings)
                        return false;
                    SceneManager.LoadScene(sceneBuildIndex);
                    return true;
                case PortalLevelAdvanceMode.LoadLevelByKey:
                    return TryLoadLevelByKey(levelKey);
                case PortalLevelAdvanceMode.NextBuildSettingsScene:
                default:
                    int next = SceneManager.GetActiveScene().buildIndex + 1;
                    if (next >= SceneManager.sceneCountInBuildSettings)
                        return false;
                    SceneManager.LoadScene(next);
                    return true;
            }
        }

        private static bool TryLoadLevelByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (GameStateController.singleton != null && GameStateController.singleton.stateMachine != null)
                return GameStateController.singleton.stateMachine.LoadLevel(key);

            if (LevelModel.map == null || !LevelModel.map.TryGetValue(key, out LevelModel levelModel))
                return false;

            if (levelModel.sceneIndex < 0 || levelModel.sceneIndex >= SceneManager.sceneCountInBuildSettings)
                return false;

            SceneManager.LoadScene(levelModel.sceneIndex);
            return true;
        }
    }
}
