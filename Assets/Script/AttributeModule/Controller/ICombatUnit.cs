using System.Collections.Generic;
using UnityEngine;

namespace AttributeModule
{
    /// <summary>
    /// Generic combat participant. Prefer this over CombatUnitAgent in Ability/Formula/Projectile.
    /// </summary>
    public interface ICombatUnit : IAttributeHolder
    {
        Team team { get; }
        bool isAlive { get; }
        bool isShieldActive { get; }
        string UnitId { get; }
        string GetName();
        GameObject GetGameObject();
        List<GameObject> equipmentSlots { get; }
        List<Transform> GetProjectileAnchorList(string skillKey);
        int GetEquipmentIndexBySkill(string skillKey);
    }
}
