using AbilityModule;
using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

namespace CombatUnitModule
{
    public class SkillTargetAssignContext
    {
        public SkillTargetAssignContext(CombatUnitAgent agent, Skill skill, Blackboard blackboard)
        {
            Agent = agent;
            Skill = skill;
            Blackboard = blackboard;
            PendingTargets = new List<GameObject>();
            RemoveTargets = new List<GameObject>();
        }

        public CombatUnitAgent Agent { get; }

        public Skill Skill { get; }

        public Blackboard Blackboard { get; }

        public List<GameObject> PendingTargets { get; }

        public List<GameObject> RemoveTargets { get; }

        public bool AddedTargetsThisTick { get; private set; }

        public void MarkTargetsAdded()
        {
            AddedTargetsThisTick = true;
        }
    }
}
