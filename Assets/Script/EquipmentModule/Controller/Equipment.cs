using AbilityModule;
using System.Collections.Generic;
using UnityEngine;
namespace EquipmentModule
{
    public class Equipment : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> projectileAnchor = new List<Transform>();

        private List<Skill> _skills = new List<Skill>();
        public List<Skill> Skills
        {
            get { return _skills; }
        }

        public List<Transform> ProjectileAnchor
        {
            get { return projectileAnchor; }
        }


        public void SetUp(List<Skill> skillList)
        {
            _skills.Clear();
            if (skillList == null || skillList.Count == 0) return;
            _skills.AddRange(skillList);
        }
    }
}