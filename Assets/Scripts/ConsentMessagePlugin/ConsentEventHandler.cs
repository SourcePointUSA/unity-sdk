using GdprConsentLib;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public void OnConsentReady(SpConsents consents)
    {
        Debug.Log("The user interaction on series of consent messages is done. You can continue user's gaming experience!");
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
