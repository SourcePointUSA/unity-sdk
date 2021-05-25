using ConsentManagementProviderLib;
using System;
using UnityEngine;

public class ConsentEventHandler : MonoBehaviour, IOnConsentUIReadyEventHandler, IOnConsentActionEventHandler, IOnConsentErrorEventHandler, IOnConsentUIFinishedEventHandler, IOnConsentReadyEventHandler
{
    void Awake()
    {
        ConsentMessenger.AddListener<IOnConsentUIReadyEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentActionEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentErrorEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentUIFinishedEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentReadyEventHandler>(this.gameObject);
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

    public void OnConsentReady(string jsonConsents)
    {
        Debug.Log($"The user interaction on consent messages is done. Consent info: {jsonConsents} \n If it was the last from the series of consents, you can continue user's gaming experience!");
    }

    private void OnDestroy()
    {
        ConsentMessenger.RemoveListener<IOnConsentUIReadyEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentActionEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentErrorEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentUIFinishedEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentReadyEventHandler>(this.gameObject);
    }
}
