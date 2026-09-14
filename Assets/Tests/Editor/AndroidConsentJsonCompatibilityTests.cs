using System;
using System.Collections.Generic;
using System.Reflection;
using ConsentManagementProvider;
using NUnit.Framework;

public class AndroidConsentJsonCompatibilityTests
{
    [Test]
    public void CcpaConsentNormalizesMissingRejectedArrays()
    {
        var consent = new CcpaConsent("ccpa-id", null, "usp", (string[])null, (string[])null, null, true, null, null, null, null);
        Assert.That(consent.rejectedCategories, Is.Empty);
        Assert.That(consent.rejectedVendors, Is.Empty);
    }

    [Test]
    public void BaseGdprUnwrapperPreservesAcceptedCategories()
    {
        var assembly = typeof(CcpaConsent).Assembly;
        var wrapperType = assembly.GetType("ConsentManagementProvider.Json.SpGdprConsentWrapperAndroid");
        var helperType = assembly.GetType("ConsentManagementProvider.Json.JsonUnwrapperHelper");
        Assert.That(wrapperType, Is.Not.Null);
        Assert.That(helperType, Is.Not.Null);
        var wrapper = Activator.CreateInstance(wrapperType);
        wrapperType.GetField("acceptedCategories").SetValue(wrapper, new List<string> { "category-a" });
        var unwrapMethod = helperType.GetMethod("UnwrapBaseSpGdprConsent", BindingFlags.NonPublic | BindingFlags.Static);
        var consent = (GdprConsent)unwrapMethod.MakeGenericMethod(wrapperType).Invoke(null, new[] { wrapper });
        Assert.That(consent.acceptedCategories, Is.EqualTo(new[] { "category-a" }));
    }

    [Test]
    public void CcpaWrapperRetainsNullableSignedLspa()
    {
        var wrapperType = typeof(CcpaConsent).Assembly.GetType("ConsentManagementProvider.Json.CcpaConsentWrapper");
        var signedLspa = wrapperType.GetField("signedLspa");
        Assert.That(signedLspa.FieldType, Is.EqualTo(typeof(bool?)));
    }
}
