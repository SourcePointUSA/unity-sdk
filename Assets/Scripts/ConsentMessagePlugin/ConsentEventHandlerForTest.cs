using GdprConsentLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsentEventHandlerForTest : MonoBehaviour, IOnConsentUIReadyEventHandler, IOnConsentActionEventHandler, IOnConsentErrorEventHandler, IOnConsentUIFinishedEventHandler, IOnConsentReadyEventHandler
{
    [SerializeField] private Text text;
    [SerializeField] private bool isEnabled;
    
    void Awake()
    {
        if (isEnabled)
        {
            ConsentMessenger.AddListener<IOnConsentUIReadyEventHandler>(this.gameObject);
            ConsentMessenger.AddListener<IOnConsentActionEventHandler>(this.gameObject);
            ConsentMessenger.AddListener<IOnConsentErrorEventHandler>(this.gameObject);
            ConsentMessenger.AddListener<IOnConsentUIFinishedEventHandler>(this.gameObject);
            ConsentMessenger.AddListener<IOnConsentReadyEventHandler>(this.gameObject);
        }
    }

    public void OnConsentUIReady()
    {
        text.text = "OnUIReady";
    }

    public void OnConsentAction(CONSENT_ACTION_TYPE action)
    {
        text.text = $"User made {action}";
    }

    public void OnConsentError(Exception exception)
    {
        text.text = $"Oh no, an error! {exception.Message}";
    }

    public void OnConsentUIFinished()
    {
        text.text = "OnConsentUIFinished";
    }

    public void OnConsentReady(string jsonConsents)
    {
        Debug.Log($"The user interaction on consent messages is done. Consent info: {jsonConsents} \n If it was the last from the series of consents, you can continue user's gaming experience!");
        text.text = $"OnConsentReady. JSON: {jsonConsents}";
    }

    private void OnDestroy()
    {
        if (isEnabled)
        {
            ConsentMessenger.RemoveListener<IOnConsentUIReadyEventHandler>(this.gameObject);
            ConsentMessenger.RemoveListener<IOnConsentActionEventHandler>(this.gameObject);
            ConsentMessenger.RemoveListener<IOnConsentErrorEventHandler>(this.gameObject);
            ConsentMessenger.RemoveListener<IOnConsentUIFinishedEventHandler>(this.gameObject);
            ConsentMessenger.RemoveListener<IOnConsentReadyEventHandler>(this.gameObject);
        }
    }
}
