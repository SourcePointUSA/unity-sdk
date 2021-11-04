using UnityEngine;

public class InstantiatePrefabController : MonoBehaviour
{
    [SerializeField] private GameObject vendorsDetails;
    [SerializeField] private GameObject managePartners;
    [SerializeField] private GameObject privacyPolicy;
    [SerializeField] private GameObject managePreferences;
    [SerializeField] private GameObject partnersDetails;

    public void InstantiateManagePreferences()
    {
        CmpLocalizationMapper.PrivacyManagerView(managePreferences);
    }
    
    public void InstantiateManagePartners()
    {
        CmpLocalizationMapper.PrivacyManagerView(managePartners);
    }   
    
    public void InstantiatePrivacyPolicy()
    {
        CmpLocalizationMapper.InstantiateOnCanvas(privacyPolicy);
    }   
    
    public void InstantiateVendorsDetails()
    {
        CmpLocalizationMapper.InstantiateOnCanvas(vendorsDetails);
    }
    
    public void InstantiatePartnersDetails()
    {
        CmpLocalizationMapper.InstantiateOnCanvas(partnersDetails);
    }
}
