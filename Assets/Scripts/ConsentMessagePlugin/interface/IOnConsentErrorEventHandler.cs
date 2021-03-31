using System;
using UnityEngine.EventSystems;

namespace GdprConsentLib
{
    public interface IOnConsentErrorEventHandler : IConsentEventHandler
    {
        void OnConsentError(Exception exception);
    }
}