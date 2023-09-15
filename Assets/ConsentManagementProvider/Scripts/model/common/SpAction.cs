namespace ConsentManagementProviderLib
{
    public class SpAction 
    {
        public CONSENT_ACTION_TYPE Type {get; private set;}
        public string CustomActionId {get; private set;}

        public SpAction(CONSENT_ACTION_TYPE type, string customActionId) 
        {
            this.Type = type;
            this.CustomActionId = customActionId;
        }
    }
}