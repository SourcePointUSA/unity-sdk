using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CmpPartnersCounter : MonoBehaviour
{
    [SerializeField] private RectTransform contentRectTrans;
    [SerializeField] private Text counterText;

    private void Awake()
    {
        counterText.text = $"PARTNERS ({contentRectTrans.childCount})"; 
    }

    public void OnScrollElementAmountChange()
    {
        StartCoroutine(WaitAndChangePartnersAmount());
    }

    private IEnumerator WaitAndChangePartnersAmount()
    {
        yield return new WaitForSeconds(0.100f);
        counterText.text = $"PARTNERS ({contentRectTrans.childCount})";
    }
}
