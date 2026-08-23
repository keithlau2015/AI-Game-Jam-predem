using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeNumPopUpUI : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private Tweener_Alpha tweener_alpha;
    [SerializeField]
    private Tweener_Position tweener_position;

    public void SetUp(Color color, string value, Vector3 from, Vector3 to)
    {
        text.color = color;
        text.text = value;
        tweener_alpha.SetOnCompleteCB(() => { this.gameObject.SetActive(false); });
        tweener_position.SetTween(from, to);
        gameObject.SetActive(true);
        tweener_alpha.Play();
        tweener_position.Play();
    }

    private void OnDisable()
    {
        text.text = "";
    }
}
