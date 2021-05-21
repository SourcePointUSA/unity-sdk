using UnityEngine;

namespace GdprConsentLib
{
    [CreateAssetMenu(fileName = "TargetingParam", menuName = "new TargetingParam", order = 51)]
    public class TargetingParamScriptableObject : ScriptableObject
    {
        [SerializeField]
        public string key;
        [SerializeField]
        public string vale;
    }
    
    public class TargetingParam
    {
        private string key;
        private string val;

        public string Key { get => key; }
        public string Value { get => val; }

        public TargetingParam(string key, string value)
        {
            this.key = key;
            this.val = value;
        }
    }
}