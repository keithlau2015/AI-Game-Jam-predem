using AttributeModule;
using Cysharp.Threading.Tasks;
using Model;
using System;
using System.Numerics;
using System.Threading;
using UnityEngine;

namespace AbilityModule
{
    /// <summary>
    /// Applies a temporary attribute modifier to an ICombatUnit.
    /// </summary>
    public class Buff
    {
        public BuffModel Model { get; protected set; }
        public ICombatUnit Target { get; protected set; }
        public bool IsActive { get; protected set; }
        public event Action<Buff> onExpired;

        protected BigInteger appliedDelta;
        protected CancellationTokenSource cts;

        public Buff(BuffModel model, ICombatUnit target)
        {
            Model = model;
            Target = target;
        }

        public virtual void Apply()
        {
            if (Model == null || Target == null || IsActive)
                return;

            if (Model.attributeType >= 0
                && Target.attributes != null
                && Target.attributes.TryGetValue(Model.attributeType, out AttributeData attr)
                && BigInteger.TryParse(Model.attributeDelta ?? "0", out BigInteger delta)
                && delta != 0)
            {
                appliedDelta = delta;
                attr.SetValue(delta, AttributeData.EditMode.Add);
            }

            IsActive = true;
            if (Model.duration > 0f)
                RunDuration().Forget();
        }

        public virtual void Remove()
        {
            if (!IsActive)
                return;

            CancelDuration();
            RevertAttribute();
            IsActive = false;
            onExpired?.Invoke(this);
        }

        protected void RevertAttribute()
        {
            if (appliedDelta == 0 || Target?.attributes == null || Model == null)
                return;

            if (Target.attributes.TryGetValue(Model.attributeType, out AttributeData attr))
                attr.SetValue(-appliedDelta, AttributeData.EditMode.Add);

            appliedDelta = 0;
        }

        private async UniTaskVoid RunDuration()
        {
            CancelDuration();
            cts = new CancellationTokenSource();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Model.duration), cancellationToken: cts.Token);
                Remove();
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected void CancelDuration()
        {
            if (cts == null)
                return;
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }
}
