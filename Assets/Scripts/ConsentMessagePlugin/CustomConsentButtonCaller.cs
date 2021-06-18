using ConsentManagementProviderLib;
using UnityEngine;

public class CustomConsentButtonCaller : MonoBehaviour
{
    [SerializeField]
    string[] vendors = new string[] { "5fbe6f050d88c7d28d765d47" };
    [SerializeField]
    string[] categories = new string[] { "60657acc9c97c400122f21f3" };
    [SerializeField]
    string[] legIntCategories = new string[] { };

    public void OnCustomConsentButtonClick()
    {
        CMP.CustomConsentGDPR(vendors: this.vendors,
                              categories: this.categories,
                              legIntCategories: this.legIntCategories,
                              onSuccessDelegate: SuccessDelegate);
    }

    private void SuccessDelegate(GdprConsent customConsent)
    {
        Debug.Log($"I am your success callback!");
    }
}
