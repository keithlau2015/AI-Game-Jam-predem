using System.Collections;
using UnityEngine;
namespace ProjectileModule
{
    public class SelfDestruction : MonoBehaviour
    {
        public float lifeTime = 2;
        private float currentLifeTime = 0;
        public bool isAutoStart = false;
        public bool hardDestroy = false;
        private Coroutine countDownCoroutine;

        private IEnumerator CountDown()
        {
            while (currentLifeTime > 0)
            {
                if (GameStateController.singleton.IsPause)
                    yield return null;

                yield return new WaitForSeconds(0.1f);
                currentLifeTime -= 0.1f;
            }

            if (hardDestroy)
            {
                Destroy(this.gameObject);
                currentLifeTime = lifeTime;
            }
            else
            {
                this.gameObject.SetActive(false);
                currentLifeTime = lifeTime;
            }
        }

        public void StartCountingDown()
        {
            if (lifeTime == -1)
            {
                Destroy(this);
            }
            else
            {
                Debug.Log($"SelfDestruction Start Counting Down: {this.gameObject.name}, life time {lifeTime}");
                countDownCoroutine = StartCoroutine(CountDown());
            }
        }

        public void StopCountingDown()
        {
            if (countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
                countDownCoroutine = null;
            }
        }


        private void OnEnable()
        {
            if (isAutoStart)
            {
                currentLifeTime = lifeTime;
                StartCoroutine(CountDown());
            }
        }

        private void OnDisable()
        {
            Debug.Log($"SelfDestruction OnDisable: {this.gameObject.name}");
            StopCountingDown();
            currentLifeTime = lifeTime;
        }
    }
}