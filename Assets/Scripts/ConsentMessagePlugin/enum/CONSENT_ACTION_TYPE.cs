using System.Collections;
using UnityEngine;

namespace GdprConsentLib
{
    public enum CONSENT_ACTION_TYPE 
    {
        SAVE_AND_EXIT = 1,
        PM_DISMISS = 2,
        ACCEPT_ALL = 11,
        SHOW_OPTIONS = 12,
        REJECT_ALL = 13,
        MSG_CANCEL = 15,
    }
}