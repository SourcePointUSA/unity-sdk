using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CmpPopupDestroyer
{
    private static Stack<GameObject> shownPopups = new Stack<GameObject>();

    public static void RegisterPopup(GameObject go)
    {
        shownPopups.Push(go);
    }

    private static void DestroyTopmostPopup()
    {
        if (shownPopups.Count > 0)
        {
            var topmost = shownPopups.Pop();
            UnityEngine.Object.Destroy(topmost);
        }
    }

    public static void DestroyAllPopups()
    {
        CmpLocalizationMapper.IsPmReadyForResurface = false;
        while (shownPopups.Count>0)
        {
            DestroyTopmostPopup();
        }
    }

    public static void DestroyAllHelperGO(bool onDestroyInvoked = false)
    {
        if (onDestroyInvoked)
        {
            var dispatcher = GameObject.Find("CMP_NetworkCallbackEventDispatcher");
            var executor = GameObject.Find("CMP_BroadcastEventsExecutor");
            if (dispatcher != null)
                UnityEngine.Object.Destroy(dispatcher);
            if (executor != null)
                UnityEngine.Object.Destroy(executor);
        }
        else
        {
            CmpDestroyHelper go = new GameObject().AddComponent<CmpDestroyHelper>();
        }
    }

    private class CmpDestroyHelper : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.name = "CmpDestroyHelper";
            StartCoroutine(WaitAndDestroy());
        }

        private IEnumerator WaitAndDestroy()
        {
            var dispatcher = GameObject.Find("CMP_NetworkCallbackEventDispatcher");
            var executor = GameObject.Find("CMP_BroadcastEventsExecutor");
            yield return new WaitForSeconds(float.Parse((NetworkClient.msTimeout/1000).ToString())*2f);
            if(dispatcher!=null)
                UnityEngine.Object.Destroy(dispatcher);
            if(executor!=null)
                UnityEngine.Object.Destroy(executor);
            Destroy(this.gameObject);
        }
    }
}