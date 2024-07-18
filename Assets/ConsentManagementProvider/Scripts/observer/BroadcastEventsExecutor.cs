using UnityEngine;

namespace ConsentManagementProvider.Observer
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
            while (BroadcastEventDispatcher.actions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }
}