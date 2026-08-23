using EvtModule;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Model
{
    public class EvtEvtTriggerModel : Model<EvtEvtTriggerModel>
    {
        public enum OperatorID
        {
            OR = 0,
            AND = 1,
        }

        public string evtID { get; private set; }
        public string triggerID { get; private set; }
        public int operatorID { get; private set; }

        public EvtEvtTriggerModel(string id) : base(id)
        {

        }

        public EvtEvtTriggerModel() : base() { }

        public static List<EvtTriggerModel> GetEvtTriggerModelListByEvtIdOpId(string evtID, OperatorID opId)
        {
            return map.ToList()
                .FindAll(x => x.Value.evtID == evtID && x.Value.operatorID == (int)opId)
                .Select(x => EvtTriggerModel.map.TryGetValue(x.Value.triggerID, out var evtTriggerModel) ? evtTriggerModel : null)
                .Where(x => x != null)
                .ToList();
        }
    }
}
