using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib.Observer
{
    internal class BroadcastEventsExecutor : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.name = "CMP_BroadcastEventsExecutor";
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            while (BroadcastEventDispatcher.actions.Count > 0)
            {
                BroadcastEventDispatcher.actions.Dequeue()?.Invoke();
            }
            BroadcastContext.BroadcastOnConsentUIReadyIfNeeded();
            BroadcastContext.BroadcastIOnConsentUIFinishedIfNeeded();
            BroadcastContext.BroadcastIOnConsentErrorIfNeeded();
            BroadcastContext.BroadcastIOnConsentActionIfNeeded();
            BroadcastContext.BroadcastIOnConsentReadyIfNeeded();
        }
        
        public void InstantiateHomeView(GameObject CmpHomePrefab, Canvas canvas)
        {
            StartCoroutine(WaitPrivacyManagerInitialized(CmpHomePrefab, canvas));
        }

        private IEnumerator WaitPrivacyManagerInitialized(GameObject CmpHomePrefab, Canvas canvas)
        {
            while (!CmpLocalizationMapper.IsInitialized)
            {
                yield return new WaitForEndOfFrame();
            }
            if (!CmpLocalizationMapper.IsConsented || CmpLocalizationMapper.IsPmReadyForResurface)
            {
                Instantiate(CmpHomePrefab, canvas.transform);
            }
        }
    }
}