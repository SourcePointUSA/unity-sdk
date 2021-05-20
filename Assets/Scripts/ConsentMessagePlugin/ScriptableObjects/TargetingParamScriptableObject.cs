using UnityEngine;

namespace GdprConsentLib
{
    [CreateAssetMenu(fileName = "TargetingParam", menuName = "new TargetingParam", order = 51)]
    public class TargetingParamScriptableObject : ScriptableObject
    {
        [SerializeField]
        public string key;
        [SerializeField]
        public string value;
    }
}