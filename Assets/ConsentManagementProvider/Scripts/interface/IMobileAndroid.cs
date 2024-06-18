using UnityEngine;

namespace ConsentManagementProviderLib
{
    public interface IMobileAndroid: IMobile
    {
        void CallShowView(AndroidJavaObject view);
        void CallRemoveView(AndroidJavaObject view);
    }
}