using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ConsentManagementProvider;
using NUnit.Framework;
using UnityEngine;

public class AndroidCampaignTypeCompatibilityTests
{
    private const string AndroidJavaConstructTypeName =
        "ConsentManagementProvider.Android.AndroidJavaConstruct";
    private const string SpClientProxyTypeName =
        "ConsentManagementProvider.Android.SpClientProxy";
    private const string BroadcastEventDispatcherTypeName =
        "ConsentManagementProvider.Observer.BroadcastEventDispatcher";
    private const string MinimalSpConsentsJson =
        "{\"gdpr\":null,\"ccpa\":null,\"usnat\":null}";

    [TestCase(CAMPAIGN_TYPE.GDPR, "GDPR")]
    [TestCase(CAMPAIGN_TYPE.CCPA, "CCPA")]
    [TestCase(CAMPAIGN_TYPE.USNAT, "USNAT")]
    public void CampaignTypesUseAndroid712JavaReference(CAMPAIGN_TYPE campaignType, string expectedEnumName)
    {
        var constructorType = typeof(CAMPAIGN_TYPE).Assembly.GetType(AndroidJavaConstructTypeName);
        Assert.That(constructorType, Is.Not.Null,
            "AndroidJavaConstruct must remain available to the Android bridge.");

        var mappingMethod = constructorType.GetMethod(
            "GetCampaignTypeJavaReference",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(mappingMethod, Is.Not.Null,
            "AndroidJavaConstruct must expose its internal JNI enum mapping for compatibility verification.");

        var reference = (KeyValuePair<string, string>)mappingMethod.Invoke(
            null,
            new object[] { campaignType });

        Assert.That(reference.Key,
            Is.EqualTo("com.sourcepoint.cmplibrary.data.network.util.CampaignType"));
        Assert.That(reference.Value, Is.EqualTo(expectedEnumName));
    }

    [TestCase(CAMPAIGN_ENV.STAGE, "STAGE")]
    [TestCase(CAMPAIGN_ENV.PUBLIC, "PUBLIC")]
    public void CampaignEnvironmentsUseAndroid712JavaReference(
        CAMPAIGN_ENV campaignEnvironment,
        string expectedEnumName)
    {
        var constructorType = typeof(CAMPAIGN_ENV).Assembly.GetType(AndroidJavaConstructTypeName);
        Assert.That(constructorType, Is.Not.Null,
            "AndroidJavaConstruct must remain available to the Android bridge.");

        var mappingMethod = constructorType.GetMethod(
            "GetCampaignEnvJavaReference",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(mappingMethod, Is.Not.Null,
            "AndroidJavaConstruct must expose its internal JNI environment mapping for compatibility verification.");

        var reference = (KeyValuePair<string, string>)mappingMethod.Invoke(
            null,
            new object[] { campaignEnvironment });

        Assert.That(reference.Key,
            Is.EqualTo("com.sourcepoint.cmplibrary.model.CampaignsEnv"));
        Assert.That(reference.Value, Is.EqualTo(expectedEnumName));
    }

    [TestCase("onConsentReady")]
    [TestCase("onSpFinished")]
    public void Android712ObjectConsentCallbacksPopulateConsentsLikeJsonCallbacks(string callbackName)
    {
        var runtimeAssembly = typeof(CAMPAIGN_TYPE).Assembly;
        var proxyType = runtimeAssembly.GetType(SpClientProxyTypeName);
        Assert.That(proxyType, Is.Not.Null, "SpClientProxy must remain available to the Android bridge.");

        var callbacks = proxyType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(method => method.Name == callbackName)
            .ToArray();
        var jsonCallback = callbacks.Single(method =>
            method.GetParameters()[0].ParameterType == typeof(string));
        var objectCallback = callbacks.Single(method =>
            method.GetParameters()[0].ParameterType == typeof(AndroidJavaObject));

        var converterField = proxyType.GetField(
            "spConsentsJsonConverter",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(converterField, Is.Not.Null,
            "The Android object callback must convert 7.12 SPConsents before dispatching it.");

        var expectedProxy = FormatterServices.GetUninitializedObject(proxyType);
        var actualProxy = FormatterServices.GetUninitializedObject(proxyType);
        converterField.SetValue(
            actualProxy,
            new Func<AndroidJavaObject, string>(_ => MinimalSpConsentsJson));

        object expectedConsents = InvokeConsentCallback(runtimeAssembly, expectedProxy, jsonCallback,
            MinimalSpConsentsJson);
        object actualConsents = InvokeConsentCallback(runtimeAssembly, actualProxy, objectCallback, null);

        Assert.That(actualConsents, Is.Not.Null,
            "The 7.12 object callback must populate the same cached consent value as the JSON callback.");
        Assert.That(actualConsents.GetType(), Is.EqualTo(expectedConsents.GetType()));
    }

    private static object InvokeConsentCallback(
        Assembly runtimeAssembly,
        object proxy,
        MethodInfo callback,
        object callbackArgument)
    {
        var dispatcherType = runtimeAssembly.GetType(BroadcastEventDispatcherTypeName);
        var actionsField = dispatcherType.GetField(
            "actions",
            BindingFlags.Public | BindingFlags.Static);
        var actions = (ConcurrentQueue<Action>)actionsField.GetValue(null);

        while (actions.TryDequeue(out _))
        {
        }

        callback.Invoke(proxy, new[] { callbackArgument });
        Assert.That(actions.TryDequeue(out Action dispatch), Is.True,
            "Consent callbacks must dispatch their state update to the Unity thread.");
        dispatch();

        var consentsField = proxy.GetType().GetField(
            "_spConsents",
            BindingFlags.NonPublic | BindingFlags.Instance);
        return consentsField.GetValue(proxy);
    }
}
