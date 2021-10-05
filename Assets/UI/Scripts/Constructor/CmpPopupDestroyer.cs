using System.Collections.Generic;
using UnityEngine;

public static class CmpPopupDestroyer
{
    private static Stack<GameObject> shownPopups = new Stack<GameObject>();

    public static void RegisterPopup(GameObject go)
    {
        shownPopups.Push(go);
    }

    public static void DestroyTopmostPopup()
    {
        if (shownPopups.Count > 0)
        {
            var topmost = shownPopups.Pop();
            UnityEngine.Object.Destroy(topmost);
        }
    }

    public static void DestroyAllPopups()
    {
        while (shownPopups.Count>0)
        {
            DestroyTopmostPopup();
        }
    }
}