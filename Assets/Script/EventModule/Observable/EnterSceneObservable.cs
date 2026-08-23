using Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EvtModule
{
    public class EnterSceneObservable : EvtObserable
    {
        private enum Operator
        {
            Equal = 0,
            NotEqual = 1,
        }

        [SerializeField]
        private string sceneName;

        [SerializeField]
        private Operator operatorValue;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene Loaded: {scene.name}");
            string targetSceneName = sceneName;
            bool isFullfilled = false;
            if (operatorValue == Operator.Equal)
            {                
                isFullfilled = scene.name.Equals(targetSceneName);
            }

            if(isFullfilled)
            {
                Notify();
            }
        }
    }
}