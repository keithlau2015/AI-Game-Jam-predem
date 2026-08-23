using UnityEngine;

namespace PortalEscort.Debug
{
    /// <summary>
    /// Example invalid-placement preview. Tints red when the prospective placement is invalid,
    /// green/neutral when valid. Driven externally via SetValid(bool) (e.g. by the placement
    /// validation path owned by Transition).
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class PlacementPreview : MonoBehaviour
    {
        [SerializeField] private bool isValid = true;
        [SerializeField] private Color validColor = Color.green;
        [SerializeField] private Color invalidColor = Color.red;

        private Renderer cachedRenderer;
        private MaterialPropertyBlock propertyBlock;

        private void Awake()
        {
            cachedRenderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            ApplyTint();
        }

        private void OnValidate()
        {
            ApplyTint();
        }

        private void ApplyTint()
        {
            if (cachedRenderer == null)
                cachedRenderer = GetComponent<Renderer>();
            if (cachedRenderer == null) return;

            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();

            propertyBlock.SetColor("_Color", isValid ? validColor : invalidColor);
            cachedRenderer.SetPropertyBlock(propertyBlock);
        }

        public void SetValid(bool valid)
        {
            isValid = valid;
            ApplyTint();
        }
    }
}
