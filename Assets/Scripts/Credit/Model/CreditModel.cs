using System.Collections.Generic;

public class CreditModel : Model<CreditModel>
{
    public string grpName;
    public string name;
    public string position;

    public static SortedDictionary<string, List<CreditModel>> grpNameMap = new SortedDictionary<string, List<CreditModel>>();

    public CreditModel() : base()
    {

    }

    public CreditModel(object key) : base(key)
    {
        List<CreditModel> creditModelList = null;
        if(!grpNameMap.TryGetValue(grpName, out creditModelList))
        {
            creditModelList = new List<CreditModel>();            
        }
        creditModelList.Add(this);
    }
}