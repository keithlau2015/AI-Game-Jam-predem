using UnityEngine;
namespace AbilityModule
{
    public class SkillVisualRange : MonoBehaviour
    {
        public enum SkillVisualType
        {
            Circle,
            Line
        }
        [SerializeField]
        private SkillVisualType skillVisualType;

        private RectTransform rectTransform;

        private void SetUp(int x, int y)
        {
            if (skillVisualType.Equals(SkillVisualType.Line))
            {
                TryGetComponent(out rectTransform);
                rectTransform.sizeDelta = new Vector2(x, y);
            }
            else if (skillVisualType.Equals(SkillVisualType.Circle))
            {
                TryGetComponent(out rectTransform);
                rectTransform.sizeDelta = new Vector2(x, rectTransform.sizeDelta.y);

                Animator animator = null;
                TryGetComponent(out animator);
                if (animator != null)
                {
                    animator.Play("ATKRange_circle", 0, y);
                    animator.Update(0);
                    animator.speed = 0;
                }
            }
        }
    }
}