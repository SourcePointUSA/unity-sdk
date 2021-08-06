using UnityEngine;

public abstract class CmpLocalizationUiElement : MonoBehaviour
{
    [SerializeField] private string id;
    public string ID => id;
    protected CmpUiElementModel model;
    
    public abstract void SetLocalization(CmpUiElementModel elementModel);
}