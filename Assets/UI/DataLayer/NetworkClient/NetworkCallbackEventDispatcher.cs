using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCallbackEventDispatcher : MonoBehaviour
{
    private readonly Queue<Action> actions = new Queue<Action>();

    public void Enqueue(Action action)
    {
        actions.Enqueue(action);
    }
    
    private void Awake()
    {
        gameObject.name = "CMP_NetworkCallbackEventDispatcher";
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        while (actions.Count > 0)
        {
            actions.Dequeue()?.Invoke();
        }
    }
}