package com.sourcepoint.unity;

import com.sourcepoint.cmplibrary.SpConsentLib;
import com.sourcepoint.cmplibrary.consent.CustomConsentClient;
import com.sourcepoint.cmplibrary.model.exposed.CCPAConsent;
import com.sourcepoint.cmplibrary.model.exposed.GDPRConsent;
import com.sourcepoint.cmplibrary.model.exposed.SPCCPAConsent;
import com.sourcepoint.cmplibrary.model.exposed.SPGDPRConsent;
import com.sourcepoint.cmplibrary.model.exposed.SPConsents;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Method;
import java.lang.reflect.Proxy;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.atomic.AtomicReference;

import kotlin.jvm.functions.Function1;
import kotlinx.serialization.json.JsonObject;

public final class SpConsentsJsonBridgeRegressionTest {
    private static final String EXPECTED_JSON =
            "{\"gdpr\":null,\"ccpa\":null,\"usnat\":null}";

    private SpConsentsJsonBridgeRegressionTest() {
    }

    public static void main(String[] args) throws Exception {
        routesCustomConsentThroughObjectCallback();
        routesDeleteCustomConsentThroughObjectCallback();
        preservesLegacyFieldsAndNullableCollections();
    }

    private static void preservesLegacyFieldsAndNullableCollections() throws Exception {
        GDPRConsent gdpr = consentProxy(GDPRConsent.class, values(
                "getUuid", "gdpr-uuid", "getTcData", Collections.emptyMap(),
                "getGrants", Collections.emptyMap(), "getEuconsent", "eu-consent",
                "getApplies", true, "getAcceptedCategories", null,
                "getWebConsentPayload", new JsonObject(Collections.emptyMap()),
                "getConsentStatus", null, "getGoogleConsentMode", null));
        CCPAConsent ccpa = consentProxy(CCPAConsent.class, values(
                "getUuid", "ccpa-uuid", "getGppData", Collections.emptyMap(),
                "getStatus", null, "getUspstring", "usp-string", "getChildPmId", "child-pm-id",
                "getApplies", true, "getSignedLspa", true, "getWebConsentPayload", null,
                "getRejectedCategories", null, "getRejectedVendors", null));
        String json = SpConsentsJsonBridge.toJson(new SPConsents(
                new SPGDPRConsent(gdpr), new SPCCPAConsent(ccpa), null));

        assertContains(json, "\"acceptedCategories\":null");
        assertContains(json, "\"webConsentPayload\":\"{}\"");
        assertContains(json, "\"childPmId\":\"child-pm-id\"");
        assertContains(json, "\"signedLspa\":true");
        assertContains(json, "\"signedLspa\":true,\"webConsentPayload\":null");
        assertContains(json, "\"rejectedCategories\":null");
        assertContains(json, "\"rejectedVendors\":null");
    }

    private static Map<String, Object> values(Object... entries) {
        Map<String, Object> values = new HashMap<>();
        for (int index = 0; index < entries.length; index += 2) {
            values.put((String) entries[index], entries[index + 1]);
        }
        return values;
    }

    private static <T> T consentProxy(Class<T> type, Map<String, Object> values) {
        InvocationHandler handler = (proxy, method, arguments) -> values.containsKey(method.getName())
                ? values.get(method.getName()) : defaultValue(method.getReturnType());
        return type.cast(Proxy.newProxyInstance(type.getClassLoader(), new Class<?>[] {type}, handler));
    }

    private static void assertContains(String value, String expectedFragment) {
        if (!value.contains(expectedFragment)) {
            throw new AssertionError("Expected JSON to contain " + expectedFragment + " but got " + value);
        }
    }

    private static void routesCustomConsentThroughObjectCallback() throws Exception {
        assertObjectCallbackRoute("customConsentGDPR", "customConsentGDPR");
    }

    private static void routesDeleteCustomConsentThroughObjectCallback() throws Exception {
        assertObjectCallbackRoute("deleteCustomConsentTo", "deleteCustomConsentTo");
    }

    private static void assertObjectCallbackRoute(String bridgeMethodName, String sdkMethodName)
            throws Exception {
        String[] vendors = {"vendor-a"};
        String[] categories = {"category-a"};
        String[] legIntCategories = {"leg-int-a"};
        AtomicReference<String> callbackJson = new AtomicReference<>();
        AtomicReference<String> calledOverload = new AtomicReference<>();

        InvocationHandler handler = (proxy, method, arguments) -> {
            if (!method.getName().equals(sdkMethodName)) {
                return defaultValue(method.getReturnType());
            }

            if (arguments[0] instanceof String[]) {
                throw new AssertionError("Bridge called the broken CMP 7.12 array overload.");
            }

            assertListEquals(arguments[0], vendors);
            assertListEquals(arguments[1], categories);
            assertListEquals(arguments[2], legIntCategories);
            calledOverload.set("list");

            @SuppressWarnings("unchecked")
            Function1<SPConsents, ?> callback = (Function1<SPConsents, ?>) arguments[3];
            callback.invoke(new SPConsents());
            return null;
        };

        SpConsentLib consentLib = (SpConsentLib) Proxy.newProxyInstance(
                SpConsentLib.class.getClassLoader(),
                new Class<?>[] {SpConsentLib.class},
                handler);
        CustomConsentClient callback = callbackJson::set;

        Method bridgeMethod;
        try {
            bridgeMethod = SpConsentsJsonBridge.class.getMethod(
                    bridgeMethodName,
                    SpConsentLib.class,
                    String[].class,
                    String[].class,
                    String[].class,
                    CustomConsentClient.class);
        } catch (NoSuchMethodException error) {
            throw new AssertionError(
                    "Missing object-callback bridge method SpConsentsJsonBridge."
                            + bridgeMethodName,
                    error);
        }

        bridgeMethod.invoke(
                null,
                consentLib,
                vendors,
                categories,
                legIntCategories,
                callback);

        if (!"list".equals(calledOverload.get())) {
            throw new AssertionError("CMP 7.12 list overload was not called.");
        }
        if (!EXPECTED_JSON.equals(callbackJson.get())) {
            throw new AssertionError(
                    "Expected legacy callback JSON " + EXPECTED_JSON + " but got "
                            + callbackJson.get());
        }
    }

    private static void assertListEquals(Object actual, String[] expected) {
        if (!(actual instanceof List) || !actual.equals(Arrays.asList(expected))) {
            throw new AssertionError(
                    "Expected list " + Arrays.toString(expected) + " but got " + actual);
        }
    }

    private static Object defaultValue(Class<?> returnType) {
        if (!returnType.isPrimitive()) {
            return null;
        }
        if (returnType == boolean.class) {
            return false;
        }
        if (returnType == char.class) {
            return '\0';
        }
        return 0;
    }
}
