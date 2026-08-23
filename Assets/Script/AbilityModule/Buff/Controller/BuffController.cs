using AttributeModule;
using Model;
using System.Collections.Generic;
using UnityEngine;

namespace AbilityModule
{
    /// <summary>
    /// Tracks active buffs on combat units.
    /// </summary>
    public static class BuffController
    {
        private static readonly Dictionary<ICombatUnit, List<Buff>> activeByTarget =
            new Dictionary<ICombatUnit, List<Buff>>();

        public static Buff ApplyBuff(string buffKey, ICombatUnit target)
        {
            if (target == null || string.IsNullOrEmpty(buffKey))
                return null;

            if (!BuffModel.map.TryGetValue(buffKey, out BuffModel model) || model == null)
            {
                Debug.LogError($"[Buff] Unknown buff key '{buffKey}'");
                return null;
            }

            if (!activeByTarget.TryGetValue(target, out List<Buff> list))
            {
                list = new List<Buff>();
                activeByTarget[target] = list;
            }

            Buff existing = list.Find(b => b.Model != null && b.Model.key != null && b.Model.key.Equals(model.key));
            if (existing != null)
            {
                if (existing is StackableBuff stackable)
                {
                    stackable.Apply();
                    return stackable;
                }

                existing.Remove();
                list.Remove(existing);
            }

            Buff buff = model.maxStack > 1
                ? new StackableBuff(model, target)
                : new Buff(model, target);

            buff.onExpired += b =>
            {
                if (activeByTarget.TryGetValue(target, out List<Buff> live))
                    live.Remove(b);
            };

            buff.Apply();
            list.Add(buff);
            return buff;
        }

        public static void Clear(ICombatUnit target)
        {
            if (target == null || !activeByTarget.TryGetValue(target, out List<Buff> list))
                return;

            for (int i = list.Count - 1; i >= 0; i--)
                list[i].Remove();

            activeByTarget.Remove(target);
        }
    }
}
