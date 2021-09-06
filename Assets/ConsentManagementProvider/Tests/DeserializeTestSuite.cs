using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class ShortCategoriesTestSuite
{
    private const string ONE_SHORT_CATEGORY_COLLECTION_JSON = @"{""categories"":[{
                                                                  ""_id"":""sahdfbasdfhasouf"",
                                                                  ""type"":""IAB_PURPOSE"",
                                                                  ""name"":""Store and/or access information on a device"",
                                                                  ""description"":""Cookies, device identifiers, or other information can be stored or accessed on your device for the purposes presented to you.""
                                                                    }]}";
    private const string ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON = @"{""categories"":[{}]}";
    private const string ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_2 = @"{""categories"":[]}";
    private const string ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_3 = @"{""categories"":""LOL""}";
    private const string ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_4 = @"{""here_is_no_categories"":""LOL""}";
    private const string ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_5 = @"malformed_json";

    [Test]
    public void DeserializeShortCategoriesWithNullCollectionPasses()
    {
        List<CmpShortCategoryModel> shortCategories = null;
        NativeUiJsonDeserializer.DeserializeShortCategories(ONE_SHORT_CATEGORY_COLLECTION_JSON, ref shortCategories);
        Assert.NotNull(shortCategories);
    }
    
    [Test]
    public void DeserializeShortCategoriesWithNotNullCollectionPasses()
    {
        List<CmpShortCategoryModel> shortCategories = new List<CmpShortCategoryModel> { new CmpShortCategoryModel()};
        NativeUiJsonDeserializer.DeserializeShortCategories(ONE_SHORT_CATEGORY_COLLECTION_JSON, ref shortCategories);
        Assert.NotNull(shortCategories);
        Assert.AreEqual(shortCategories.Count, 2);
        var sublist = shortCategories.GetRange(1, 1);
        var sublistJson = JsonSerializer.Serialize(sublist);
        var deserialized = JsonSerializer.Deserialize<List<CmpShortCategoryModel>>(sublistJson);
        Assert.AreEqual(deserialized[0]._id, sublist[0]._id);
        Assert.AreEqual(deserialized[0].name, sublist[0].name);
        Assert.AreEqual(deserialized[0].type, sublist[0].type);
        Assert.AreEqual(deserialized[0].description, sublist[0].description);
    }

    [Test]
    public void DeserializeShortCategoriesPasses()
    {
        bool passes = true;
        List<CmpShortCategoryModel> shortCategories = null;
        NativeUiJsonDeserializer.DeserializeShortCategories(JSONSTUB.shortCategories, ref shortCategories);
        if (shortCategories != null && shortCategories.Count > 0)
        {
            foreach (var cat in shortCategories)
            {
                if (string.IsNullOrEmpty(cat._id)
                    // || string.IsNullOrEmpty(cat.type) // type can be null
                    || string.IsNullOrEmpty(cat.name)
                    || string.IsNullOrEmpty(cat.description))
                {
                    passes = false;
                }
            }
        }
        else
        {
            Assert.Fail();
        }
        Assert.IsTrue(passes);
    }

    [Test]
    public void DeserializeShortCategoriesPassesWithNullElements()
    {
        List<CmpShortCategoryModel> shortCategories = null;
        NativeUiJsonDeserializer.DeserializeShortCategories(ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON, ref shortCategories);
        Assert.NotNull(shortCategories);
        Assert.AreEqual(shortCategories.Count, 1);
        Assert.IsNull(shortCategories[0]._id);
        Assert.IsNull(shortCategories[0].type);
        Assert.IsNull(shortCategories[0].name);
        Assert.IsNull(shortCategories[0].description);
    }
    
    [Test]
    public void DeserializeShortCategoriesPassesWithEmptyCollection()
    {
        List<CmpShortCategoryModel> shortCategories = null;
        NativeUiJsonDeserializer.DeserializeShortCategories(ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_2, ref shortCategories);
        Assert.NotNull(shortCategories);
        Assert.AreEqual(shortCategories.Count, 0);
    }
    
    [Test]
    public void DeserializeShortCategoriesFailsWithInvalidOperationException()
    {
        List<CmpShortCategoryModel> shortCategories = null;
        Assert.Throws<InvalidOperationException>(
            delegate
            {
                NativeUiJsonDeserializer.DeserializeShortCategories(ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_3, ref shortCategories);
            });
    }

    [Test]
    public void DeserializeShortCategoriesPassesWithAbsentCategoryKeyword()
    {
        List<CmpShortCategoryModel> shortCategories = null;
        NativeUiJsonDeserializer.DeserializeShortCategories(ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_4, ref shortCategories);
        Assert.NotNull(shortCategories);
        Assert.AreEqual(shortCategories.Count, 0);
    }
    
    //JsonException -> System.Text.Json.JsonReaderException but it is internal
    // [Test]
    // public void DeserializeShortCategoriesFailsWithMalformedJson()
    // {
    //     List<CmpShortCategoryModel> shortCategories = null;
    //     Assert.Throws(   
    //         Is.TypeOf<System.Text.Json.JsonReaderException>(),
    //         delegate
    //         {
    //             NativeUiJsonDeserializer.DeserializeShortCategories(ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON_5, ref shortCategories);
    //         });
    // }
}

public class DeserializeCollectionTestSuite
{
    private JsonDocument document;
    private JsonElement root;
    
    #region Setup & Teardown
    [SetUp]
    public void Setup()
    {
        document = JsonDocument.Parse(JSONSTUB.extraCall);
        root = document.RootElement;
    }

    [TearDown]
    public void Teardown()
    {
        document = null;
        root = new JsonElement();
    }
    #endregion
    
    [Test]
    public void DeserializeCollectionCmpCategoryModelPasses()
    {
        List<CmpCategoryModel> categoryModels = NativeUiJsonDeserializer.DeserializeCollection<CmpCategoryModel>(root, "categories");
        Assert.NotNull(categoryModels);
        Assert.NotZero(categoryModels.Count);
        foreach (var cat in categoryModels)
        {
            Assert.NotNull(cat._id);
            Assert.NotNull(cat.name);
            Assert.NotNull(cat.type);
            Assert.NotNull(cat.iabId);
            Assert.NotNull(cat.description);
            Assert.NotNull(cat.disclosureOnly);
            Assert.NotNull(cat.friendlyDescription);
            
            Assert.NotNull(cat.legIntVendors);
            Assert.NotNull(cat.disclosureOnlyVendors);
            Assert.NotNull(cat.requiringConsentVendors);

            if (cat.requiringConsentVendors.Count > 0)
            {
                foreach (var req in cat.requiringConsentVendors)
                {
                    Assert.NotNull(req.name);
                    Assert.NotNull(req.vendorId);
                    Assert.NotNull(req.vendorType);
                    // Assert.NotNull(req.policyUrl);   //can be null
                }
            }
            
            if (cat.legIntVendors.Count > 0)
            {
                foreach (var leg in cat.legIntVendors)
                {
                    Assert.NotNull(leg.name);
                    Assert.NotNull(leg.vendorId);
                    Assert.NotNull(leg.vendorType);
                    Assert.NotNull(leg.policyUrl);
                }
            }
            
            if (cat.disclosureOnlyVendors.Count > 0)
            {
                foreach (var dis in cat.disclosureOnlyVendors)
                {
                    Assert.NotNull(dis.name);
                    Assert.NotNull(dis.vendorId);
                    Assert.NotNull(dis.vendorType);
                    Assert.NotNull(dis.policyUrl);
                }
            }
        }
    }

    [Test]
    public void DeserializeCollectionCmpSpecialPurposeModelPasses()
    {
        List<CmpSpecialPurposeModel> specPurp = NativeUiJsonDeserializer.DeserializeCollection<CmpSpecialPurposeModel>(root, "specialPurposes");
        Assert.NotNull(specPurp);
        Assert.NotZero(specPurp.Count);
        foreach (var spec in specPurp)
        {
            Assert.NotNull(spec._id);
            Assert.NotNull(spec.name);
            Assert.NotNull(spec.type);
            Assert.NotNull(spec.iabId);
            Assert.NotNull(spec.description);
            Assert.NotNull(spec.vendors);
            foreach (var vendr in spec.vendors)
            {
                Assert.NotNull(vendr.name);
                Assert.NotNull(vendr.policyUrl);
            }
        }
    }

    [Test]
    public void DeserializeCollectionCmpFeatureModelPasses()
    {
        List<CmpFeatureModel> features = NativeUiJsonDeserializer.DeserializeCollection<CmpFeatureModel>(root, "features");
        Assert.NotNull(features);
        Assert.NotZero(features.Count);
        foreach (var feat in features)
        {
            Assert.NotNull(feat._id);
            Assert.NotNull(feat.name);
            Assert.NotNull(feat.iabId);
            Assert.NotNull(feat.description);
            Assert.NotNull(feat.vendors);
            foreach (var vendr in feat.vendors)
            {
                Assert.NotNull(vendr.name);
                Assert.NotNull(vendr.policyUrl);
            }
        }
    }

    [Test]
    public void DeserializeCollectionCmpSpecialFeatureModelPasses()
    {
        List<CmpSpecialFeatureModel> features = NativeUiJsonDeserializer.DeserializeCollection<CmpSpecialFeatureModel>(root, "specialFeatures");
        Assert.NotNull(features);
        Assert.NotZero(features.Count);
        foreach (var feat in features)
        {
            Assert.NotNull(feat._id);
            Assert.NotNull(feat.name);
            Assert.NotNull(feat.iabId);
            Assert.NotNull(feat.description);
            Assert.NotNull(feat.vendors);
            foreach (var vendr in feat.vendors)
            {
                Assert.NotNull(vendr.name);
                Assert.NotNull(vendr.vendorId);
                Assert.NotNull(vendr.policyUrl);
            }
        }
    }

    [Test]
    public void DeserializeCollectionCmpVendorModelPasses()
    {
        List<CmpVendorModel> vendors = NativeUiJsonDeserializer.DeserializeCollection<CmpVendorModel>(root, "vendors");
        Assert.NotNull(vendors);
        Assert.NotZero(vendors.Count);
        foreach (var vendor in vendors)
        {
            Assert.NotNull(vendor.vendorType);
            Assert.NotNull(vendor.name);
            // Assert.NotNull(vendor.iabId);            //may be null
            // Assert.NotNull(vendor.cookieHeader);     //may be null
            Assert.NotNull(vendor.iabFeatures);
            Assert.NotNull(vendor.iabSpecialPurposes);
            Assert.NotNull(vendor.iabSpecialFeatures);
            Assert.NotNull(vendor.legIntCategories);
            Assert.NotNull(vendor.consentCategories);
            Assert.NotNull(vendor.disclosureOnlyCategories);
            
            if(vendor.iabFeatures.Count>0)
                foreach (var feature in vendor.iabFeatures)
                {
                    Assert.NotNull(feature);
                }
            if(vendor.iabSpecialPurposes.Count>0)
                foreach (var specPurp in vendor.iabSpecialPurposes)
                {
                    Assert.NotNull(specPurp);
                }
            if(vendor.iabSpecialFeatures.Count>0)
                foreach (var feature in vendor.iabSpecialFeatures)
                {
                    Assert.NotNull(feature);
                }
            
            if (vendor.legIntCategories.Count > 0)
            {
                foreach (var cat in vendor.legIntCategories)
                {
                    Assert.NotNull(cat.name);
                    Assert.NotNull(cat.type);
                    // Assert.NotNull(cat.iabId);   //can be null
                }
            }
            
            if (vendor.consentCategories.Count > 0)
            {
                foreach (var cat in vendor.consentCategories)
                {
                    Assert.NotNull(cat.name);
                    Assert.NotNull(cat.type);
                    // Assert.NotNull(cat.iabId);   //can be null
                }
            }
            
            if (vendor.disclosureOnlyCategories.Count > 0)
            {
                foreach (var cat in vendor.disclosureOnlyCategories)
                {
                    Assert.NotNull(cat.name);
                    Assert.NotNull(cat.type);
                    // Assert.NotNull(cat.iabId);   //can be null
                }
            }
        }
    }
}

public class DeserializeExtraCallTestSuite
{
    [Test]
    public void DeserializeExtraCallPasses()
    {
        List<CmpCategoryModel> categoryModels = null;
        List<CmpSpecialPurposeModel> specialPurposeModels = null;
        List<CmpFeatureModel> featureModels = null;
        List<CmpSpecialFeatureModel> specialFeatureModels = null;
        List<CmpVendorModel> vendorModels = null;
        string json = @"
{
   ""categories"":[
      {
         ""_id"":""sahdfbasdfhasouf"",
         ""type"":""IAB_PURPOSE"",
         ""name"":""Store and/or access information on a device"",
         ""iabId"":1,
         ""description"":""Vendors can:\n* Store and access information on the device such as cookies and device identifiers presented to a user."",
         ""friendlyDescription"":""Cookies, device identifiers, or other information can be stored or accessed on your device for the purposes presented to you."",
         ""disclosureOnly"":false,
         ""requiringConsentVendors"":[
            {
               ""vendorId"":""sahdfbasdfhasouf"",
               ""vendorType"":""IAB"",
               ""name"":""LoopMe Limited"",
               ""policyUrl"":""https://loopme.com/privacy-policy/""
            }
         ],
         ""legIntVendors"":[
            {
               ""vendorId"":""sahdfbasdfhasouf"",
               ""vendorType"":""CUSTOM"",
               ""name"":""Google Charts"",
               ""policyUrl"":""https://developers.google.com/chart/interactive/docs/security_privacy"",
               ""cookies"":[ ]
            }
         ],
         ""disclosureOnlyVendors"":[ ],
         ""doNotAllowVendors"":{ }
      }
   ],
   ""specialPurposes"":[
      {
         ""_id"":""sahdfbasdfhasouf"",
         ""iabId"":1,
         ""type"":""IAB"",
         ""name"":""Ensure security, prevent fraud, and debug"",
         ""description"":""To ensure security, prevent fraud and debug vendors can:\n* Ensure data are securely transmitted\n* Detect and prevent malicious, fraudulent, invalid, or illegal activity.\n* Ensure correct and efficient operation of systems and processes, including to monitor and enhance the performance of systems and processes engaged in permitted purposes\nVendors cannot:\n* Conduct any other data processing operation allowed under a different purpose under this purpose.\nNote: Data collected and used to ensure security, prevent fraud, and debug may include automatically-sent device characteristics for identification, precise geolocation data, and data obtained by actively scanning device characteristics for identification without separate disclosure and/or opt-in."",
         ""vendors"":[
            {
               ""name"":""LoopMe Limited"",
               ""policyUrl"":""https://loopme.com/privacy-policy/""
            }
         ]
      }
   ],
   ""features"":[
      {
         ""_id"":""sahdfbasdfhasouf"",
         ""iabId"":1,
         ""name"":""Match and combine offline data sources"",
         ""description"":""Vendors can:\n* Combine data obtained offline with data collected online in support of one or more Purposes or Special Purposes."",
         ""vendors"":[
            {
               ""name"":""LoopMe Limited"",
               ""policyUrl"":""https://loopme.com/privacy-policy/""
            }
         ]
      }
   ],
   ""specialFeatures"":[
      {
         ""_id"":""sahdfbasdfhasouf"",
         ""iabId"":1,
         ""name"":""Use precise geolocation data"",
         ""description"":""Vendors can:\n* Collect and process precise geolocation data in support of one or more purposes.\nN.B. Precise geolocation means that there are no restrictions on the precision of a user’s location; this can be accurate to within several meters."",
         ""vendors"":[
            {
               ""name"":""LoopMe Limited"",
               ""vendorId"":""sahdfbasdfhasouf"",
               ""policyUrl"":""https://loopme.com/privacy-policy/""
            }
         ]
      }
   ],
   ""vendors"":[
      {
         ""vendorId"":""sahdfbasdfhasouf"",
         ""iabId"":109,
         ""vendorType"":""IAB"",
         ""name"":""LoopMe Limited"",
         ""policyUrl"":""https://loopme.com/privacy-policy/"",
         ""legIntCategories"":[
            {
               ""type"":""IAB_PURPOSE"",
               ""iabId"":10,
               ""name"":""Develop and improve products""
            }
         ],
         ""consentCategories"":[
            {
               ""type"":""IAB_PURPOSE"",
               ""iabId"":1,
               ""name"":""Store and/or access information on a device""
            }
         ],
         ""disclosureOnlyCategories"":[ ],
         ""iabSpecialPurposes"":[
            ""Ensure security, prevent fraud, and debug"",
            ""Technically deliver ads or content""
         ],
         ""iabFeatures"":[
            ""Match and combine offline data sources"",
            ""Link different devices""
         ],
         ""iabSpecialFeatures"":[
            ""Use precise geolocation data""
         ],
         ""cookieHeader"":""LoopMe Limited stores cookies with a maximum duration of about 365 Day(s) (31536000 Second(s)). This vendor also uses other methods like \""local storage\"" to store and access information on your device.""
      }
   ]
}
";
        NativeUiJsonDeserializer.DeserializeExtraCall(json,
            ref categoryModels,
            ref specialPurposeModels,
            ref featureModels,
            ref specialFeatureModels,
            ref vendorModels);
        Assert.NotNull(categoryModels);
        Assert.NotNull(specialPurposeModels);
        Assert.NotNull(featureModels);
        Assert.NotNull(specialFeatureModels);
        Assert.NotNull(vendorModels);
        Assert.AreEqual(categoryModels.Count, 1);
        Assert.AreEqual(specialPurposeModels.Count, 1);
        Assert.AreEqual(featureModels.Count, 1);
        Assert.AreEqual(specialFeatureModels.Count, 1);
        Assert.AreEqual(vendorModels.Count, 1);
        var cat = categoryModels[0];
        Assert.AreEqual(cat._id,"sahdfbasdfhasouf");
        Assert.AreEqual(cat.type,"IAB_PURPOSE");
        Assert.AreEqual(cat.name,"Store and/or access information on a device");
        Assert.AreEqual(cat.iabId,1);
        Assert.AreEqual(cat.description,"Vendors can:\n* Store and access information on the device such as cookies and device identifiers presented to a user.");
        Assert.AreEqual(cat.friendlyDescription,"Cookies, device identifiers, or other information can be stored or accessed on your device for the purposes presented to you.");
        Assert.AreEqual(cat.disclosureOnly, false);
        Assert.AreEqual(cat.requiringConsentVendors.Count, 1);
        Assert.AreEqual(cat.requiringConsentVendors[0].vendorId, "sahdfbasdfhasouf");
        Assert.AreEqual(cat.requiringConsentVendors[0].vendorType, "IAB");
        Assert.AreEqual(cat.requiringConsentVendors[0].name, "LoopMe Limited");
        Assert.AreEqual(cat.requiringConsentVendors[0].policyUrl, "https://loopme.com/privacy-policy/");
        Assert.AreEqual(cat.legIntVendors.Count, 1);
        Assert.AreEqual(cat.legIntVendors[0].vendorId, "sahdfbasdfhasouf");
        Assert.AreEqual(cat.legIntVendors[0].vendorType, "CUSTOM");
        Assert.AreEqual(cat.legIntVendors[0].name, "Google Charts");
        Assert.AreEqual(cat.legIntVendors[0].policyUrl, "https://developers.google.com/chart/interactive/docs/security_privacy");
        var specPurp = specialPurposeModels[0];
        Assert.AreEqual(specPurp._id, "sahdfbasdfhasouf");
        Assert.AreEqual(specPurp.iabId, 1);
        Assert.AreEqual(specPurp.type, "IAB");
        Assert.AreEqual(specPurp.name, "Ensure security, prevent fraud, and debug");
        Assert.AreEqual(specPurp.description, "To ensure security, prevent fraud and debug vendors can:\n* Ensure data are securely transmitted\n* Detect and prevent malicious, fraudulent, invalid, or illegal activity.\n* Ensure correct and efficient operation of systems and processes, including to monitor and enhance the performance of systems and processes engaged in permitted purposes\nVendors cannot:\n* Conduct any other data processing operation allowed under a different purpose under this purpose.\nNote: Data collected and used to ensure security, prevent fraud, and debug may include automatically-sent device characteristics for identification, precise geolocation data, and data obtained by actively scanning device characteristics for identification without separate disclosure and/or opt-in.");
        Assert.AreEqual(specPurp.vendors.Count, 1);
        Assert.AreEqual(specPurp.vendors[0].name, "LoopMe Limited");
        Assert.AreEqual(specPurp.vendors[0].policyUrl, "https://loopme.com/privacy-policy/");
        var feat = featureModels[0];
        Assert.AreEqual(feat._id, "sahdfbasdfhasouf");
        Assert.AreEqual(feat.iabId, 1);
        Assert.AreEqual(feat.name, "Match and combine offline data sources");
        Assert.AreEqual(feat.description, "Vendors can:\n* Combine data obtained offline with data collected online in support of one or more Purposes or Special Purposes.");
        Assert.AreEqual(feat.vendors.Count, 1);
        Assert.AreEqual(feat.vendors[0].name, "LoopMe Limited");
        Assert.AreEqual(feat.vendors[0].policyUrl, "https://loopme.com/privacy-policy/");
        var specFeat = specialFeatureModels[0];
        Assert.AreEqual(specFeat._id, "sahdfbasdfhasouf");
        Assert.AreEqual(specFeat.iabId, 1);
        Assert.AreEqual(specFeat.name, "Use precise geolocation data");
        Assert.AreEqual(specFeat.description, "Vendors can:\n* Collect and process precise geolocation data in support of one or more purposes.\nN.B. Precise geolocation means that there are no restrictions on the precision of a user’s location; this can be accurate to within several meters.");
        Assert.AreEqual(specFeat.vendors.Count, 1);
        Assert.AreEqual(specFeat.vendors[0].name, "LoopMe Limited");
        Assert.AreEqual(specFeat.vendors[0].vendorId, "sahdfbasdfhasouf");
        Assert.AreEqual(specFeat.vendors[0].policyUrl, "https://loopme.com/privacy-policy/");
        var vendr = vendorModels[0];
        Assert.AreEqual(vendr.vendorId, "sahdfbasdfhasouf");
        Assert.AreEqual(vendr.iabId, 109);
        Assert.AreEqual(vendr.vendorType, "IAB");
        Assert.AreEqual(vendr.name, "LoopMe Limited");
        Assert.AreEqual(vendr.policyUrl, "https://loopme.com/privacy-policy/");
        Assert.AreEqual(vendr.legIntCategories.Count, 1);
        Assert.AreEqual(vendr.legIntCategories[0].type, "IAB_PURPOSE");
        Assert.AreEqual(vendr.legIntCategories[0].iabId, 10);
        Assert.AreEqual(vendr.legIntCategories[0].name, "Develop and improve products");
        Assert.AreEqual(vendr.consentCategories[0].type, "IAB_PURPOSE");
        Assert.AreEqual(vendr.consentCategories[0].iabId, 1);
        Assert.AreEqual(vendr.consentCategories[0].name, "Store and/or access information on a device");
        Assert.AreEqual(vendr.iabSpecialPurposes[0], "Ensure security, prevent fraud, and debug");
        Assert.AreEqual(vendr.iabSpecialPurposes[1], "Technically deliver ads or content");
        Assert.AreEqual(vendr.iabSpecialPurposes.Count, 2);
        Assert.AreEqual(vendr.iabFeatures[0], "Match and combine offline data sources");
        Assert.AreEqual(vendr.iabFeatures[1], "Link different devices");
        Assert.AreEqual(vendr.iabFeatures.Count, 2);
        Assert.AreEqual(vendr.iabSpecialFeatures.Count, 1);
        Assert.AreEqual(vendr.iabSpecialFeatures[0], "Use precise geolocation data");
        Assert.AreEqual(vendr.cookieHeader, "LoopMe Limited stores cookies with a maximum duration of about 365 Day(s) (31536000 Second(s)). This vendor also uses other methods like \"local storage\" to store and access information on your device.");
    }
}

//TODO: public class DeserializeNativePmTestSuite 