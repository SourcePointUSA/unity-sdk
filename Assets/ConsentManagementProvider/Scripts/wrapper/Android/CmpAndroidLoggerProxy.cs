using UnityEngine;

namespace ConsentManagementProviderLib.Android
{
    public class CmpAndroidLoggerProxy : AndroidJavaProxy
    {
        static readonly string NativeJavaInterfaceName = "com.sourcepoint.cmplibrary.exception.Logger";

        public CmpAndroidLoggerProxy() : base(new AndroidJavaClass(NativeJavaInterfaceName)) { }

        /**
        * The [error] method receives contains the logic to communicate with the server
        * it is used only in production
        * @param RuntimeException instance of [ConsentLibExceptionK]
        */
        void error(AndroidJavaObject RuntimeException) =>
            CmpDebugUtil.LogError($"ERROR: {RuntimeException.Get<string>("description")}");

        /**
        * Send an {@link #ERROR} log message.
        * @param tag Used to identify the source of a log message.
        * It usually identifies the class or activity where the log call occurs.
        * @param msg The message you would like logged.
        */
        void e(string tag, string msg) => 
            CmpDebugUtil.Log($"E  TAG: {tag}, MSG: {msg}");

        
#region WEB REQ / RES
        // Web Request Log
        void req(string tag, string url, string type, string body) => 
            CmpDebugUtil.Log($"REQ  TAG: {tag}, URL: {url}, TYPE: {type}, BODY: {body}");

        // Web Response Log
        void res(string tag, string msg, string status, string body) => 
            CmpDebugUtil.Log($"RES  TAG: {tag}, MSG: {msg}, STATUS: {status}, BODY: {body}");
#endregion
        
#region DEBUG ONLY
        /**
         * The [i] method receives contains the logic to communicate with the server
         * it is used only in debug
         * @param tag Used to identify the source of a log message.
         * It usually identifies the class or activity where the log call occurs.
        * @param msg The message you would like logged.
         */
        void i(string tag, string msg) => 
            CmpDebugUtil.Log($"I  TAG: {tag}, MSG: {msg}");

        /**
        * The [d] method receives contains the logic to communicate with the server
        * it is used only in DEBUG
        * @param tag Used to identify the source of a log message.
        * It usually identifies the class or activity where the log call occurs.
        * @param msg The message you would like logged.
        */
        void d(string tag, string msg) => 
            CmpDebugUtil.Log($"D  TAG: {tag}, MSG: {msg}");

        /**
        * The [v] method receives contains the logic to communicate with the server
        * it is used only in DEBUG
        * @param tag Used to identify the source of a log message.
        * It usually identifies the class or activity where the log call occurs.
        * @param msg The message you would like logged.
        */
        void v(string tag, string msg) => 
            CmpDebugUtil.Log($"V  TAG: {tag}, MSG: {msg}");
#endregion

#region COMPUTATION // Native library logs about response computation
        void computation(string tag, string msg) => 
            CmpDebugUtil.Log($"Computation  TAG: {tag}, MSG: {msg}");

        void computation(string tag, string msg, AndroidJavaObject? json) => 
            CmpDebugUtil.Log($"Computation2  TAG: {tag}, MSG: {msg}, JSON: {(json != null ? json : "nil")}");
#endregion COMPUTATION

#region WebApp
        // TODO
        void nativeMessageAction(string tag, string msg, AndroidJavaObject? json) => 
            CmpDebugUtil.Log($"NativeMessageAction  TAG: {tag}, MSG: {msg}, JSON: {(json != null ? json : "nil")}");

        // WebApp action on pm tab or first layer tab
        void webAppAction(string tag, string msg, AndroidJavaObject? json) => 
            CmpDebugUtil.Log($"WebAppAction  TAG: {tag}, MSG: {msg}, JSON: {(json != null ? json : "nil")}");

        // Client action on pm tab or first layer tab
        void clientEvent(string _event, string msg, string content) => 
            CmpDebugUtil.Log($"ClientEvent  EVENT: {_event}, MSG: {msg}, CONTENT: {content}");

        // Information about pm tabs(gdpr, ccpa)
        void pm(string tag, string url, string type, string? _params) => 
            CmpDebugUtil.Log($"Pm  TAG: {tag}, URL: {url}, TYPE: {type}, PARAMS: {(_params != null ? _params : "nil")}");

        // Information about preloading tabs
        void flm(string tag, string url, string type, AndroidJavaObject json) => 
            CmpDebugUtil.Log($"Flm  TAG: {tag}, URL: {url}, TYPE: {type}, JSON: {json}");
#endregion WebApp
    }
}