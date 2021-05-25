using UnityEngine;

namespace ConsentManagementProviderLib
{
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