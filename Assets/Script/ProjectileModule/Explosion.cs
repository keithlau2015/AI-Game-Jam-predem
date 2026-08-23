using AttributeModule;
using Model;
using System.Collections;
using UnityEngine;

namespace ProjectileModule
{
    public class Explosion : MonoBehaviour
    {
        private Coroutine scalingCoroutine;

        private IEnumerator ScaleToSize(float duration, Vector3 targetScale)
        {
            Vector3 initialScale = transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);
                yield return null;
            }

            transform.localScale = targetScale;
        }

        public void StartScaling(Vector3 targetScale, float duration = 1f)
        {
            scalingCoroutine = StartCoroutine(ScaleToSize(duration, targetScale));
        }

        public void StopScaling()
        {
            if (scalingCoroutine != null)
                StopCoroutine(scalingCoroutine);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != null)
                Impact(other);
        }

        private void Impact(Collider other)
        {
            ICombatUnit inRangeUnit = other.GetComponentInParent<ICombatUnit>();
            if (inRangeUnit == null)
                return;

            if (!inRangeUnit.attributes.TryGetValue((int)AttributeModel.AttributeType.HP, out AttributeData _)
                || !inRangeUnit.attributes.TryGetValue((int)AttributeModel.AttributeType.DEF, out AttributeData _))
            {
                return;
            }

            // Damage application is left to the owning projectile / skill system.
        }
    }
}
