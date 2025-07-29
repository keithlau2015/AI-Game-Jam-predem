using UnityEngine;
using UnityEngine.UI;

public class CreditItem : MonoBehaviour
{
    [SerializeField]
    private Transform anchor;
    [SerializeField]
    private Text textLablePrefab;
    
    public void SetCreditItem(CreditModel creditModel)
    {
        if(!string.IsNullOrEmpty(creditModel.grpName) && string.IsNullOrEmpty(creditModel.name) && string.IsNullOrEmpty(creditModel.position))
        {
            Text grpNameLabel = Instantiate(textLablePrefab, anchor);
            grpNameLabel.text = creditModel.grpName;
        }
        else
        {
            Text nameLable = Instantiate(textLablePrefab, anchor);
            nameLable.text = creditModel.name;
            Text posLable = Instantiate(textLablePrefab, anchor);
            posLable.text = creditModel.position;
        }
    }
}
