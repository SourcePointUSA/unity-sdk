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
        var canvas = GameObject.Find("Canvas").transform;
        Instantiate(managePreferences, canvas);
    }
    
    public void InstantiateManagePartners()
    {
        var canvas = GameObject.Find("Canvas").transform;
        Instantiate(managePartners, canvas);
    }   
    
    public void InstantiatePrivacyPolicy()
    {
        var canvas = GameObject.Find("Canvas").transform;
        Instantiate(privacyPolicy, canvas);
    }   
    
    public void InstantiateVendorsDetails()
    {
        var canvas = GameObject.Find("Canvas").transform;
        Instantiate(vendorsDetails, canvas);
    }
    
    public void InstantiatePartnersDetails()
    {
        var canvas = GameObject.Find("Canvas").transform;
        Instantiate(partnersDetails, canvas);
    }
}
