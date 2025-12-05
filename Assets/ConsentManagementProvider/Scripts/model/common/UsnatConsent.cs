using System;
using System.Collections.Generic;
using System.Text;

namespace ConsentManagementProvider
{
    public class UsnatConsent
    {
        public bool applies;
        public List<ConsentString> consentStrings;
        public List<Consentable> vendors;
        public List<Consentable> categories;
#nullable enable
        public string? uuid;
        public Dictionary<string, object>? GPPData;
		public StatusesUsnat? statuses;
#nullable disable

        public UsnatConsent(
#nullable enable
                        string? uuid,
                        bool applies,
                        List<ConsentString> consentStrings,
                        List<Consentable> vendors,
                        List<Consentable> categories,
                        ConsentStatus consentStatus,
                        Dictionary<string, object>? GPPData
#nullable disable
        ) {
            this.uuid = uuid;
            this.applies = applies;
            this.consentStrings = consentStrings;
            this.vendors = vendors;
            this.categories = categories;
            statuses = StatusesUsnat.collectData(consentStatus);
            this.GPPData = GPPData;
        }

        public UsnatConsent(
#nullable enable
                string? uuid,
                bool applies,
                List<ConsentString> consentStrings,
                List<Consentable> vendors,
                List<Consentable> categories,
                StatusesUsnat statuses,
                Dictionary<string, object>? GPPData
#nullable disable
)
        {
            this.uuid = uuid;
            this.applies = applies;
            this.consentStrings = consentStrings;
            this.vendors = vendors;
            this.categories = categories;
            this.statuses = statuses;
            this.GPPData = GPPData;
        }

        public string ToFullString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"USNAT");

            if(uuid != null)
                sb.AppendLine($"UUID: {uuid}");
            sb.AppendLine($"Applies: {applies}");

            sb.AppendLine($"ConsentStrings:");
            foreach (var _string in consentStrings)
            {
                sb.AppendLine($"    sectionId: {_string.sectionId}, sectionName: {_string.sectionName}, consentString: {_string.consentString}");
            }
            sb.AppendLine("Categories:");
            foreach (var _consentable in categories)
            {
                sb.AppendLine($"    id:{_consentable.id}, consented:{_consentable.consented}");
            }
            sb.AppendLine("Vendors:");
            foreach (var _consentable in vendors)
            {
                sb.AppendLine($"    id:{_consentable.id}, consented:{_consentable.consented}");
            }
            sb = statuses.ToFullString(sb);

            if(GPPData != null)
            {
                sb.AppendLine("GPPData:");
                foreach (var kvp in GPPData)
                    sb.AppendLine($"    {kvp.Key}: {kvp.Value.ToString()}");
            }

            return sb.ToString();
        }
    }

    public class StatusesUsnat
    {        
#nullable enable
        public bool? rejectedAny, consentedToAll, consentedToAny,
            hasConsentData, sellStatus, shareStatus,
            sensitiveDataStatus, gpcStatus, previousOptInAll;
#nullable disable

        internal static StatusesUsnat collectData(ConsentStatus status)
        {
            return new StatusesUsnat
            {
                rejectedAny = status.rejectedAny,
                consentedToAll = status.consentedToAll,
                consentedToAny = status.consentedToAny,
                hasConsentData = status.hasConsentData,
                sellStatus = status.granularStatus?.sellStatus,
                shareStatus = status.granularStatus?.shareStatus,
                sensitiveDataStatus = status.granularStatus?.sensitiveDataStatus,
                gpcStatus = status.granularStatus?.gpcStatus,
                previousOptInAll = status.granularStatus?.previousOptInAll
            };
        }

        public StringBuilder ToFullString(StringBuilder sb)
        {
            sb.AppendLine("ConsentStatus:");
            if(rejectedAny != null)
                sb.AppendLine($"    rejectedAny: {rejectedAny}");
            if(consentedToAll != null)
                sb.AppendLine($"    consentedToAll: {consentedToAll}");
            if(consentedToAny != null)
                sb.AppendLine($"    consentedToAny: {consentedToAny}");
            if(hasConsentData != null)
                sb.AppendLine($"    hasConsentData: {hasConsentData}");
            if(sellStatus != null)
                sb.AppendLine($"    sellStatus: {sellStatus}");
            if(shareStatus != null)
                sb.AppendLine($"    shareStatus: {shareStatus}");
            if(sensitiveDataStatus != null)
                sb.AppendLine($"    sensitiveDataStatus: {sensitiveDataStatus}");
            if(gpcStatus != null)
                sb.AppendLine($"    gpcStatus: {gpcStatus}");
            if(previousOptInAll != null)
                sb.AppendLine($"    previousOptInAll: {previousOptInAll}");

            return sb;
        }
    }

    public class Consentable
    {
        public string id;
        public bool consented;
    }

    public class ConsentString 
    {
        public string consentString;
        public int sectionId;
        public string sectionName;

        public ConsentString(string consentString, int sectionId, string sectionName)
        {
            this.consentString = consentString;
            this.sectionId = sectionId;
            this.sectionName = sectionName;
        }
    }
}