using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class SkillBuffModel : Model<SkillBuffModel>
    {
        public string buff { get; set; }
        public string skill { get; set; }
        
        public static List<string> GetBuffModelKeyListBySkillKey(string skillKey) {
            return map.Values.ToList().FindAll(sb => sb.skill.Equals(skillKey)).Select(sb => sb.buff).ToList();
        }

        public static List<string> GetSkillModelKeyListByBuffKey(string buffKey) {
            return map.Values.ToList().FindAll(sb => sb.buff.Equals(buffKey)).Select(sb => sb.skill).ToList();
        }
    }
}