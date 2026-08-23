using AbilityModule;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class EquipmentSkillModel : Model<EquipmentSkillModel>
    {
        public string equipment { get; private set; }
        public string skill { get; private set; }

        public EquipmentSkillModel(string id) : base(id)
        {

        }

        public static List<EquipmentModel> GetEquipmentModelListBySkillKey(string skillKey)
        {
            List<string> keyList = map.Values.ToList().FindAll(es => es.skill.Equals(skillKey)).Select(es => es.equipment).ToList();
            return EquipmentModel.map.Values.Where(x => keyList.Contains(x.key.ToString())).ToList();
        }

        public static List<SkillModel> GetSkillModelListByEquipmentKey(string equipmentKey)
        {
            List<string> keyList = map.Values.ToList().FindAll(es => es.equipment.Equals(equipmentKey)).Select(es => es.skill).ToList();
            return SkillModel.map.Values.Where(x => keyList.Contains(x.key.ToString())).ToList();
        }
    }
}