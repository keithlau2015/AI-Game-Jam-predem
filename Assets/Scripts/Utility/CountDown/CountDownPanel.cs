using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownPanel : MonoBehaviour
{
    [SerializeField]
    private Text label;
    public void SetLabel(string content)
    {
        label.text = content;
    }
}
