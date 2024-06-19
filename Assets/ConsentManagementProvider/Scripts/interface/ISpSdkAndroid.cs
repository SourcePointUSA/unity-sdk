using UnityEngine;

namespace ConsentManagementProviderLib
{
    public interface ISpSdkAndroid: ISpSdk
    {
        void CallShowView(AndroidJavaObject view);
        void CallRemoveView(AndroidJavaObject view);
    }
}