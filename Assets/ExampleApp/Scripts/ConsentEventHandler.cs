using ConsentManagementProviderLib;
using System;
using UnityEngine;

public class ConsentEventHandler : MonoBehaviour, IOnConsentUIReady, IOnConsentAction, IOnConsentError, IOnConsentUIFinished, IOnConsentReady
{
    void Awake()
    {
        ConsentMessenger.AddListener<IOnConsentUIReady>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentAction>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentError>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentUIFinished>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentReady>(this.gameObject);
    }

    public void OnConsentUIReady()
    {
        Debug.LogWarning("User will be shown the web view with series of consent messages!");
    }

    public void OnConsentAction(CONSENT_ACTION_TYPE action)
    {
        Debug.LogWarning($"User made {action} action with consent view!");
    }

    public void OnConsentError(Exception exception)
    {
        Debug.LogError("Oh no, an error! " + exception.Message);
    }

    public void OnConsentUIFinished()
    {
        Debug.LogWarning("User has interacted with the web view consent message and it is disappeared!");
    }

    public void OnConsentReady(SpConsents spConsents)
    {
        Debug.Log($"The user interaction on consent messages is done. You can use the spConsent info; \n If it was the last from the series of consents, you can continue user's gaming experience!");
    }

    private void OnDestroy()
    {
        ConsentMessenger.RemoveListener<IOnConsentUIReady>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentAction>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentError>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentUIFinished>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentReady>(this.gameObject);
    }
}
