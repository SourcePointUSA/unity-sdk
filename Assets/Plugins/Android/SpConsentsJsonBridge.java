package com.sourcepoint.unity;

import com.sourcepoint.cmplibrary.data.network.model.optimized.ConsentStatus;
import com.sourcepoint.cmplibrary.data.network.model.optimized.GCMStatus;
import com.sourcepoint.cmplibrary.data.network.model.optimized.GoogleConsentMode;
import com.sourcepoint.cmplibrary.data.network.model.optimized.GranularState;
import com.sourcepoint.cmplibrary.data.network.model.optimized.USNatConsentData;
import com.sourcepoint.cmplibrary.SpConsentLib;
import com.sourcepoint.cmplibrary.consent.CustomConsentClient;
import com.sourcepoint.cmplibrary.model.JsonToMapExtKt;
import com.sourcepoint.cmplibrary.model.exposed.CCPAConsent;
import com.sourcepoint.cmplibrary.model.exposed.Consentable;
import com.sourcepoint.cmplibrary.model.exposed.GDPRConsent;
import com.sourcepoint.cmplibrary.model.exposed.GDPRPurposeGrants;
import com.sourcepoint.cmplibrary.model.exposed.SPConsents;
import com.sourcepoint.cmplibrary.model.exposed.UsNatConsent;
import com.sourcepoint.cmplibrary.model.exposed.UsNatStatuses;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.Arrays;
import java.util.List;
import java.util.Map;

import kotlin.Unit;
import kotlin.jvm.functions.Function1;

/** Converts the object callbacks used by CMP 7.12+ into the legacy Unity JSON contract. */
public final class SpConsentsJsonBridge {
    private SpConsentsJsonBridge() {
    }

    public static void customConsentGDPR(
            SpConsentLib consentLib,
            String[] vendors,
            String[] categories,
            String[] legIntCategories,
            CustomConsentClient successCallback) {
        consentLib.customConsentGDPR(
                Arrays.asList(vendors),
                Arrays.asList(categories),
                Arrays.asList(legIntCategories),
                new Function1<SPConsents, Unit>() {
                    @Override
                    public Unit invoke(SPConsents consents) {
                        transferCustomConsent(successCallback, consents);
                        return Unit.INSTANCE;
                    }
                });
    }

    public static void deleteCustomConsentTo(
            SpConsentLib consentLib,
            String[] vendors,
            String[] categories,
            String[] legIntCategories,
            CustomConsentClient successCallback) {
        consentLib.deleteCustomConsentTo(
                Arrays.asList(vendors),
                Arrays.asList(categories),
                Arrays.asList(legIntCategories),
                new Function1<SPConsents, Unit>() {
                    @Override
                    public Unit invoke(SPConsents consents) {
                        transferCustomConsent(successCallback, consents);
                        return Unit.INSTANCE;
                    }
                });
    }

    public static String toJson(SPConsents consents) throws JSONException {
        JSONObject json = new JSONObject();
        GDPRConsent gdpr = consents.getGdpr() == null ? null : consents.getGdpr().getConsent();
        CCPAConsent ccpa = consents.getCcpa() == null ? null : consents.getCcpa().getConsent();
        UsNatConsent usnat = consents.getUsNat() == null ? null : consents.getUsNat().getConsent();

        json.put("gdpr", gdpr == null ? JSONObject.NULL : toJson(gdpr));
        json.put("ccpa", ccpa == null ? JSONObject.NULL : toJson(ccpa));
        json.put("usnat", usnat == null ? JSONObject.NULL : toJson(usnat));
        return json.toString();
    }

    private static void transferCustomConsent(
            CustomConsentClient successCallback,
            SPConsents consents) {
        if (consents == null) {
            return;
        }

        try {
            successCallback.transferCustomConsentToUnity(toJson(consents));
        } catch (JSONException error) {
            throw new IllegalStateException("Unable to convert custom consent to Unity JSON.", error);
        }
    }

    private static JSONObject toJson(GDPRConsent consent) throws JSONException {
        JSONObject json = new JSONObject();
        json.put("uuid", nullable(consent.getUuid()));
        json.put("tcData", JsonToMapExtKt.toConsentJSONObj(consent.getTcData()));
        json.put("grants", grantsToJson(consent.getGrants()));
        json.put("euconsent", nullable(consent.getEuconsent()));
        json.put("apply", consent.getApplies());
        json.put("acceptedCategories", nullable(toJsonArray(consent.getAcceptedCategories())));
        json.put("webConsentPayload", webConsentPayload(consent));
        json.put("consentStatus", nullable(toJson(consent.getConsentStatus())));
        json.put("googleConsentMode", nullable(toJson(consent.getGoogleConsentMode())));
        return json;
    }

    private static JSONObject grantsToJson(Map<String, GDPRPurposeGrants> grants) {
        return JsonToMapExtKt.toJSONObjGrant(grants);
    }

    private static JSONObject toJson(CCPAConsent consent) throws JSONException {
        JSONObject json = new JSONObject();
        json.put("uuid", nullable(consent.getUuid()));
        json.put("gppData", JsonToMapExtKt.toConsentJSONObj(consent.getGppData()));
        json.put("status", nullable(consent.getStatus()));
        json.put("uspstring", nullable(consent.getUspstring()));
        json.put("rejectedCategories", nullable(toJsonArray(consent.getRejectedCategories())));
        json.put("childPmId", nullable(consent.getChildPmId()));
        json.put("apply", consent.getApplies());
        json.put("signedLspa", nullable(consent.getSignedLspa()));
        json.put("webConsentPayload", webConsentPayload(consent));
        json.put("rejectedVendors", nullable(toJsonArray(consent.getRejectedVendors())));
        return json;
    }

    private static JSONArray toJsonArray(List<?> values) {
        return values == null ? null : new JSONArray(values);
    }

    private static Object webConsentPayload(Object consent) {
        try {
            Object payload = consent.getClass().getMethod("getWebConsentPayload").invoke(consent);
            return payload == null ? JSONObject.NULL : payload.toString();
        } catch (ReflectiveOperationException error) {
            throw new IllegalStateException("Unable to read web consent payload.", error);
        }
    }

    private static JSONObject toJson(UsNatConsent consent) throws JSONException {
        JSONObject json = new JSONObject();
        json.put("applies", consent.getApplies());
        json.put("gppData", JsonToMapExtKt.toConsentJSONObj(consent.getGppData()));
        json.put("statuses", toJson(consent.getStatuses()));
        json.put("consentStrings", consentStringsToJson(consent.getConsentStrings()).toString());
        json.put("dateCreated", consent.getDateCreated());
        json.put("vendors", consentablesToJson(consent.getVendors()).toString());
        json.put("categories", consentablesToJson(consent.getCategories()).toString());
        json.put("uuid", consent.getUuid());
        return json;
    }

    private static JSONObject toJson(UsNatStatuses statuses) throws JSONException {
        if (statuses == null) {
            return null;
        }

        JSONObject json = new JSONObject();
        json.put("hasConsentData", nullable(statuses.getHasConsentData()));
        json.put("rejectedAny", nullable(statuses.getRejectedAny()));
        json.put("consentedToAll", nullable(statuses.getConsentedToAll()));
        json.put("consentedToAny", nullable(statuses.getConsentedToAny()));
        json.put("sellStatus", nullable(statuses.getSellStatus()));
        json.put("shareStatus", nullable(statuses.getShareStatus()));
        json.put("sensitiveDataStatus", nullable(statuses.getSensitiveDataStatus()));
        json.put("gpcStatus", nullable(statuses.getGpcStatus()));
        return json;
    }

    private static JSONArray consentStringsToJson(List<USNatConsentData.ConsentString> consentStrings)
            throws JSONException {
        JSONArray json = new JSONArray();
        for (USNatConsentData.ConsentString consentString : consentStrings) {
            JSONObject item = new JSONObject();
            item.put("sectionId", consentString.getSectionId());
            item.put("sectionName", consentString.getSectionName());
            item.put("consentString", consentString.getConsentString());
            json.put(item);
        }
        return json;
    }

    private static JSONArray consentablesToJson(List<? extends Consentable> consentables)
            throws JSONException {
        JSONArray json = new JSONArray();
        for (Consentable consentable : consentables) {
            JSONObject item = new JSONObject();
            item.put("id", consentable.getId());
            item.put("consented", consentable.getConsented());
            json.put(item);
        }
        return json;
    }

    private static JSONObject toJson(ConsentStatus status) throws JSONException {
        if (status == null) {
            return null;
        }

        JSONObject json = new JSONObject();
        json.put("consentedAll", nullable(status.getConsentedAll()));
        json.put("consentedToAny", nullable(status.getConsentedToAny()));
        json.put("hasConsentData", nullable(status.getHasConsentData()));
        json.put("rejectedAny", nullable(status.getRejectedAny()));
        json.put("rejectedLI", nullable(status.getRejectedLI()));
        json.put("legalBasisChanges", nullable(status.getLegalBasisChanges()));
        json.put("vendorListAdditions", nullable(status.getVendorListAdditions()));
        json.put("granularStatus", nullable(toJson(status.getGranularStatus())));
        return json;
    }

    private static JSONObject toJson(ConsentStatus.GranularStatus status) throws JSONException {
        if (status == null) {
            return null;
        }

        JSONObject json = new JSONObject();
        json.put("defaultConsent", nullable(status.getDefaultConsent()));
        json.put("previousOptInAll", nullable(status.getPreviousOptInAll()));
        json.put("purposeConsent", nullable(name(status.getPurposeConsent())));
        json.put("purposeLegInt", nullable(name(status.getPurposeLegInt())));
        json.put("vendorConsent", nullable(name(status.getVendorConsent())));
        json.put("vendorLegInt", nullable(name(status.getVendorLegInt())));
        return json;
    }

    private static JSONObject toJson(GoogleConsentMode mode) throws JSONException {
        if (mode == null) {
            return null;
        }

        JSONObject json = new JSONObject();
        json.put("ad_user_data", nullable(status(mode.getAdUserData())));
        json.put("ad_personalization", nullable(status(mode.getAdPersonalization())));
        json.put("analytics_storage", nullable(status(mode.getAnalyticsStorage())));
        json.put("ad_storage", nullable(status(mode.getAdStorage())));
        return json;
    }

    private static String name(GranularState state) {
        return state == null ? null : state.name();
    }

    private static String status(GCMStatus status) {
        return status == null ? null : status.getStatus();
    }

    private static Object nullable(Object value) {
        return value == null ? JSONObject.NULL : value;
    }
}
