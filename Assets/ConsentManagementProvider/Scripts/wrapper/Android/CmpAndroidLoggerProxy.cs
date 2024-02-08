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
        * @param e instance of [ConsentLibExceptionK]
        */
        void error(AndroidJavaObject RuntimeException)
        {
            CmpDebugUtil.LogError("Got logger: error");
            CmpDebugUtil.LogError(RuntimeException.ToString());
        }

        /**
        * Send an {@link #ERROR} log message.
        * @param tag Used to identify the source of a log message.  It usually identifies
        *        the class or activity where the log call occurs.
        * @param msg The message you would like logged.
        */
        void e(string tag, string msg)
        {
            CmpDebugUtil.Log("Got logger: e");
            CmpDebugUtil.Log("TAG: "+tag);
            CmpDebugUtil.Log("MSG: "+msg);
        }

        /**
         * The [i] method receives contains the logic to communicate with the server
         * it is used only in debug
         * @param tag Used to identify the source of a log message.
         *            It usually identifies the class or activity where the log call occurs.
        * @param msg The message you would like logged.
         */
        void i(string tag, string msg)
        {
            CmpDebugUtil.Log("Got logger: i");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
        }

        /**
        * The [d] method receives contains the logic to communicate with the server
        * it is used only in debug
        * @param tag Used to identify the source of a log message.
        *            It usually identifies the class or activity where the log call occurs.
        * @param msg The message you would like logged.
        */
        void d(string tag, string msg)
        {
            CmpDebugUtil.Log("Got logger: d");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
        }

        /**
        * The [v] method receives contains the logic to communicate with the server
        * it is used only in debug
        * @param tag Used to identify the source of a log message.
        *            It usually identifies the class or activity where the log call occurs.
        * @param msg The message you would like logged.
        */
        void v(string tag, string msg)
        {
            CmpDebugUtil.Log("Got logger: v");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
        }

        void req(string tag, string url, string type, string body)
        {
            CmpDebugUtil.Log("Got logger: req");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("URL: " + url);
            CmpDebugUtil.Log("TYPE: " + type);
            CmpDebugUtil.Log("BODY: " + body);
        }

        void res(string tag, string msg, string status, string body)
        {
            CmpDebugUtil.Log("Got logger: res");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
            CmpDebugUtil.Log("STATUS: " + status);
            CmpDebugUtil.Log("BODY: " + body);
        }

        void webAppAction(string tag, string msg, AndroidJavaObject? json)
        {
            CmpDebugUtil.LogWarning("Got logger: webAppAction");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
            if(json != null) CmpDebugUtil.Log("json: " + json.ToString());
        }

        void nativeMessageAction(string tag, string msg, AndroidJavaObject? json)
        {
            CmpDebugUtil.LogWarning("Got logger: nativeMessageAction");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
            if (json != null) CmpDebugUtil.Log("json: " + json.ToString());
        }

        void computation(string tag, string msg)
        {
            CmpDebugUtil.Log("Got logger: computation");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
        }

        void computation(string tag, string msg, AndroidJavaObject? json)
        {
            CmpDebugUtil.Log("Got logger: computation2");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("MSG: " + msg);
            if (json != null) CmpDebugUtil.Log("json: " + json.ToString());
        }

        void clientEvent(string _event, string msg, string content)
        {
            CmpDebugUtil.LogWarning("Got logger: clientEvent");
            CmpDebugUtil.Log("TAG: " + _event);
            CmpDebugUtil.Log("MSG: " + msg);
            CmpDebugUtil.Log("CONTENT: " + content);
        }

        void pm(string tag, string url, string type, string? _params)
        {
            CmpDebugUtil.LogWarning("Got logger: pm");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("URL: " + url);
            CmpDebugUtil.Log("TYPE: " + type);
            if (_params != null) CmpDebugUtil.Log("PARAMS: " + _params);
        }

        void flm(string tag, string url, string type, AndroidJavaObject json)
        {
            CmpDebugUtil.LogWarning("Got logger: flm");
            CmpDebugUtil.Log("TAG: " + tag);
            CmpDebugUtil.Log("URL: " + url);
            CmpDebugUtil.Log("TYPE: " + type);
            CmpDebugUtil.Log("json: " + json);
        }
    }
}