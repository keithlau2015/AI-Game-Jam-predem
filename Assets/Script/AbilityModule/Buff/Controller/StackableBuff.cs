using AttributeModule;
using Model;
using System.Numerics;

namespace AbilityModule
{
    /// <summary>
    /// Buff that can stack attribute deltas up to BuffModel.maxStack.
    /// </summary>
    public class StackableBuff : Buff
    {
        public int StackCount { get; private set; }

        public StackableBuff(BuffModel model, ICombatUnit target) : base(model, target)
        {
        }

        public override void Apply()
        {
            if (Model == null || Target == null)
                return;

            int max = Model.maxStack > 0 ? Model.maxStack : 1;
            if (StackCount >= max)
            {
                // Refresh duration only
                if (Model.duration > 0f && IsActive)
                {
                    CancelDuration();
                    // re-enter duration by toggling
                    IsActive = false;
                    base.Apply();
                    StackCount = max;
                }
                return;
            }

            if (!IsActive)
            {
                base.Apply();
                StackCount = 1;
                return;
            }

            // Additional stack: apply another delta
            if (Model.attributeType >= 0
                && Target.attributes != null
                && Target.attributes.TryGetValue(Model.attributeType, out AttributeData attr)
                && BigInteger.TryParse(Model.attributeDelta ?? "0", out BigInteger delta)
                && delta != 0)
            {
                attr.SetValue(delta, AttributeData.EditMode.Add);
                appliedDelta += delta;
                StackCount++;
            }
        }

        public override void Remove()
        {
            StackCount = 0;
            base.Remove();
        }
    }
}
