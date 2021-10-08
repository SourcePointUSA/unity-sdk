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
            if (!CmpLocalizationMapper.IsConsented)
                Instantiate(CmpHomePrefab, canvas.transform);
        }
    }
}