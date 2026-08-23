using LocalizationModule;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI
{
    public class LoadingPanel : MonoBehaviour
    {
        private const int MAX_RAND_TIP_NUM = 10;

        [SerializeField]
        private Slider progressBar, subProgressBar;
        [SerializeField]
        private Text progressLable, subProgressLabel, randomTips;
        [SerializeField]
        private Tweener_Alpha tween;

        private float switchTipInterval = 1;
        private int previousRandTipNum = -1;
        private int curFinishTasks = 0;

        public void Show(bool showSumUpProgress = false, int totalTasks = 1, bool showTip = false, float switchTipInterval = 1)
        {
            ResetAll();
            this.gameObject.SetActive(true);
            tween.SetTween(0, 1);
            tween.SetOnCompleteCB(() => {
                if (showSumUpProgress)
                {
                    this.progressBar.gameObject.SetActive(true);
                    this.progressLable.gameObject.SetActive(true);
                    this.progressBar.maxValue = totalTasks;
                    this.progressBar.minValue = 0;
                    progressLable.text = $"{Mathf.RoundToInt(progressBar.value)}%";
                    if (showTip)
                    {
                        StartCoroutine(RandomTips());
                        this.switchTipInterval = switchTipInterval;
                    }
                }
            });
            tween.Play();
        }

        public void SetUpSubProgressBar(float maxValue, float minValue)
        {
            subProgressBar.maxValue = maxValue;
            subProgressBar.minValue = minValue;
        }

        public void OnSubProgressChange(float value, string label)
        {
            progressBar.value = curFinishTasks + (value/subProgressBar.maxValue);
            progressLable.text = $"{Mathf.RoundToInt((progressBar.value / progressBar.maxValue) * 100)}%";
            subProgressBar.value = value;
            subProgressLabel.text = label;
            if (value == subProgressBar.maxValue)
            {
                OnTaskFinish();
            }
        }

        public void OnTaskFinish()
        {
            curFinishTasks++;
            progressBar.value = curFinishTasks;
            progressLable.text = $"{Mathf.RoundToInt((progressBar.value / progressBar.maxValue) * 100)}%";
        }

        public void ResetSubProgressBar()
        {
            subProgressBar.maxValue = 1;
            subProgressBar.minValue = 0;
            subProgressBar.value = 0;
            subProgressLabel.text = "";
        }

        public void ResetProgressBar()
        {
            progressBar.maxValue = 1;
            progressBar.minValue = 0;
            progressBar.value = 0;
            progressLable.text = "";
        }

        public void ResetAll()
        {
            ResetProgressBar();
            ResetSubProgressBar();
            progressBar.value = 0;
            progressLable.text = "";
            subProgressBar.value = 0;
            subProgressLabel.text = "";
            progressBar.gameObject.SetActive(false);
            progressLable.gameObject.SetActive(false);
            randomTips.gameObject.SetActive(false);
            switchTipInterval = 1;
            previousRandTipNum = -1;
        }

        private void OnDisable()
        {
            ResetAll();
        }

        private IEnumerator RandomTips()
        {
            while (this.gameObject.activeInHierarchy)
            {
                int randNum = UnityEngine.Random.Range(0, MAX_RAND_TIP_NUM);
                while(randNum == previousRandTipNum && previousRandTipNum > 0)
                    randNum = UnityEngine.Random.Range(0, MAX_RAND_TIP_NUM);
                previousRandTipNum = randNum;
                randomTips.text = LocalizationManager.singleton.GetLocalization($"SYS_Tips{randNum}");
                yield return new WaitForSeconds(switchTipInterval);
            }
        }

        public void Hide()
        {
            tween.SetTween(1, 0);
            tween.SetOnCompleteCB(() => {
                this.gameObject.SetActive(false);
                this.tween.SetCanvasGroupAlpha(1);
            });
            tween.Play();
        }
    }
}