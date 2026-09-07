package com.sourcepoint.unity;

import com.sourcepoint.cmplibrary.SpConsentLib;
import com.sourcepoint.cmplibrary.consent.CustomConsentClient;
import com.sourcepoint.cmplibrary.model.exposed.SPConsents;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Method;
import java.lang.reflect.Proxy;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.atomic.AtomicReference;

import kotlin.jvm.functions.Function1;

public final class SpConsentsJsonBridgeRegressionTest {
    private static final String EXPECTED_JSON =
            "{\"gdpr\":null,\"ccpa\":null,\"usnat\":null}";

    private SpConsentsJsonBridgeRegressionTest() {
    }

    public static void main(String[] args) throws Exception {
        routesCustomConsentThroughObjectCallback();
        routesDeleteCustomConsentThroughObjectCallback();
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
