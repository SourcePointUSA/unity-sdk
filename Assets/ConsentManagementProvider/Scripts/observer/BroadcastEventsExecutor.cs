using UnityEngine;

namespace ConsentManagementProviderLib.Observer
{
    internal class BroadcastEventsExecutor : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            while (BroadcastEventDispatcher.actions.Count > 0)
            {
                BroadcastEventDispatcher.actions.Dequeue()?.Invoke();
            }
        }
    }
}