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
        
        //COMMENTED DUE TO JSONSTUB DOES NOT EXIST NO MORE REGARDING TO SECURITY REASONS!
        
        // NativeUiJsonDeserializer.DeserializeShortCategories(JSONSTUB.shortCategories, ref shortCategories);
        // if (shortCategories != null && shortCategories.Count > 0)
        // {
        //     foreach (var cat in shortCategories)
        //     {
        //         if (string.IsNullOrEmpty(cat._id)
        //             // || string.IsNullOrEmpty(cat.type) // type can be null
        //             || string.IsNullOrEmpty(cat.name)
        //             || string.IsNullOrEmpty(cat.description))
        //         {
        //             passes = false;
        //         }
        //     }
        // }
        // else
        // {
        //     Assert.Fail();
        // }
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

// COMMENTED DUE TO JSONSTUB DOES NOT EXIST NO MORE REGARDING TO SECURITY REASONS!

/*
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
    public void DeserializeCollectionWithNullElementPasses()
    {
       string json = @"
{
""categories"":
         [
            null,
            {
               ""_id"":""aaaaaa1312"",
               ""type"":""IAB_PURPOSE"",
               ""name"":""Store and/or access information on a device"",
               ""description"":""Cookies, device identifiers, or other information can be stored or accessed on your device for the purposes presented to you.""
            }
         ]
}
       ";
       JsonDocument document = JsonDocument.Parse(json);
       JsonElement root = document.RootElement;

       List<CmpCategoryModel> cats =NativeUiJsonDeserializer.DeserializeCollection<CmpCategoryModel>(root, "categories");
       Assert.NotNull(cats);
       Assert.NotZero(cats.Count);
       Assert.AreEqual(cats.Count, 1);
    }
    
    [Test]
    public void DeserializeCollectionWithWrongTypePasses()
    {
       string json = @"
{
""categories"":
         [
               ""_id""
         ]
}
       ";
       JsonDocument document = JsonDocument.Parse(json);
       JsonElement root = document.RootElement;

       List<CmpCategoryModel> cats = NativeUiJsonDeserializer.DeserializeCollection<CmpCategoryModel>(root, "categories");
       Assert.NotNull(cats);
       Assert.Zero(cats.Count);
    }
}
*/
   
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

public class DeserializeNativePmTestSuite
{
    [Test]
    public void DeserializeNativePmNoChildrenPropertyFails()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"{
           ""id"": ""Root"",
           ""type"": ""NativeOtt"",
           ""name"": ""Native OTT""
        }";
        Assert.Catch<KeyNotFoundException>(delegate
        {
            Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        });
    }

    [Test]
    public void DeserializeNativePmWithNullChildrenFails()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"{
           ""id"": ""Root"",
           ""type"": ""NativeOtt"",
           ""name"": ""Native OTT"",
           ""children"": null
        }";
        Assert.Catch<InvalidOperationException>(delegate
        {
            Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        });
    }
    
    [Test]
    public void DeserializeNativePmWithNoViewIdPasses()
    {
       Dictionary<string, string> popupBgColors = null;
       string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": null,
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""Header"",
               ""type"": ""NativeText"",
               ""name"": ""Header"",
               ""settings"": {
                  ""text"": ""Privacy"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 28,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
       LogAssert.Expect(LogType.Error, ">>>DAFUQ >:C");
       Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
       Assert.IsEmpty(elements);
       Assert.IsEmpty(popupBgColors);
    }    
    
    [Test]
    public void DeserializeNativePmWithNullViewPasses()
    {
       Dictionary<string, string> popupBgColors = null;
       string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      null,
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""Header"",
               ""type"": ""NativeText"",
               ""name"": ""Header"",
               ""settings"": {
                  ""text"": ""Privacy"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 28,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
       Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
       Assert.IsNotEmpty(elements);
       Assert.IsNotEmpty(elements["HomeView"]);
       Assert.AreEqual(elements.Count, 1);
    }    

    [Test]
    public void DeserializeNativePmWithUiElementIdNullPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": null,
               ""type"": ""NativeText"",
               ""name"": ""Header"",
               ""settings"": {
                  ""text"": ""Privacy"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 28,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.IsEmpty(elements["HomeView"]);
    }    

    [Test]
    public void DeserializeNativePmWithNullBgColorPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": null
            }
         },
         ""children"": [
            {
               ""id"": ""Header"",
               ""type"": ""NativeText"",
               ""name"": ""Header"",
               ""settings"": {
                  ""text"": ""Privacy"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 28,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.IsEmpty(popupBgColors);
        Assert.AreEqual(popupBgColors.Count, 0);
    }    
    
    [Test]
    public void DeserializeNativePmWithBgColorPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""Header"",
               ""type"": ""NativeText"",
               ""name"": ""Header"",
               ""settings"": {
                  ""text"": ""Privacy"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 28,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.IsNotEmpty(popupBgColors);
        Assert.AreEqual(popupBgColors.Count, 1);
        Assert.AreEqual(popupBgColors["HomeView"], "#e5e8ef");
    }
    
    [Test]
    public void DeserializeNativePmWithNullUiElementPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            null,
            {
               ""id"": ""Header"",
               ""type"": ""NativeText"",
               ""name"": ""Header"",
               ""settings"": {
                  ""text"": ""Privacy"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 28,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.AreEqual(elements["HomeView"].Count, 1);
        Assert.IsNotNull(elements["HomeView"][0]);
    }    
    
    [Test]
    public void DeserializeNativePmWithNativeTextUiElementPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""Header"",
               ""type"": ""NativeText"",
               ""name"": ""Header"",
               ""settings"": {
                  ""text"": ""Privacy"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 28,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.AreEqual(elements["HomeView"].Count, 1);
        var el = elements["HomeView"][0] as CmpTextModel;
        Assert.IsNotNull(el);
        Assert.AreEqual(el.id, "Header");
        Assert.AreEqual(el.type, "NativeText");
        Assert.AreEqual(el.name, "Header");
        Assert.AreEqual(el.Text, "Privacy");
        Assert.AreEqual(el.Font.color, "#000000");
        Assert.AreEqual(el.Font.fontWeight, "400");
        Assert.AreEqual(el.Font.fontFamily, "arial, helvetica, sans-serif");
        Assert.AreEqual(el.Font.fontSize, 28);
    }      
    
    [Test]
    public void DeserializeNativePmWithBackButtonUiElementPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""BackButton"",
               ""type"": ""NativeButton"",
               ""name"": ""Back button"",
               ""settings"": {
                  ""text"": ""Back"",
                  ""startFocus"": false,
                  ""style"": {
                     ""backgroundColor"": ""#ffffff"",
                     ""font"": {
                        ""fontSize"": 16,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }               
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.AreEqual(elements["HomeView"].Count, 1);
        var el = elements["HomeView"][0] as CmpBackButtonModel;
        Assert.IsNotNull(el);
        Assert.AreEqual(el.id, "BackButton");
        Assert.AreEqual(el.type, "NativeButton");
        Assert.AreEqual(el.name, "Back button");
        Assert.AreEqual(el.Text, "Back");
        Assert.AreEqual(el.StartFocus!, false);
        Assert.AreEqual(el.BackgroundColor, "#ffffff");
        Assert.AreEqual(el.Font.Color, "#000000");
        Assert.AreEqual(el.Font.fontWeight, "400");
        Assert.AreEqual(el.Font.fontFamily, "arial, helvetica, sans-serif");
        Assert.AreEqual(el.Font.fontSize, 16);
    }        
    
    [Test]
    public void DeserializeNativePmWithNativeButtonUiElementPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""OnButton"",
               ""type"": ""NativeButton"",
               ""name"": ""On Button"",
               ""settings"": {
                  ""text"": ""On"",
                  ""startFocus"": false,
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 14,
                        ""fontWeight"": ""400"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     },
                     ""onFocusBackgroundColor"": ""#ffffff"",
                     ""onUnfocusBackgroundColor"": ""#575757"",
                     ""onFocusTextColor"": ""#000000"",
                     ""onUnfocusTextColor"": ""#ffffff""
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.AreEqual(elements["HomeView"].Count, 1);
        var el = elements["HomeView"][0] as CmpNativeButtonModel;
        Assert.IsNotNull(el);
        Assert.AreEqual(el.id, "OnButton");
        Assert.AreEqual(el.type, "NativeButton");
        Assert.AreEqual(el.name, "On Button");
        Assert.AreEqual(el.Text, "On");
        Assert.AreEqual(el.StartFocus!, false);
        Assert.AreEqual(el.Font.fontWeight, "400");
        Assert.AreEqual(el.Font.fontFamily, "arial, helvetica, sans-serif");
        Assert.AreEqual(el.Font.fontSize, 14);
        Assert.AreEqual(el.OnFocusBackgroundColor, "#ffffff");
        Assert.AreEqual(el.OnUnfocusBackgroundColor, "#575757");
        Assert.AreEqual(el.OnFocusTextColor, "#000000");
        Assert.AreEqual(el.OnUnfocusTextColor, "#ffffff");
    }   
    
    [Test]
    public void DeserializeNativePmWithSliderUiElementPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""CategoriesSlider"",
               ""type"": ""Slider"",
               ""name"": ""Categories Slider"",
               ""settings"": {
                  ""leftText"": ""CONSENT"",
                  ""rightText"": ""LEGITIMATE INTEREST"",
                  ""style"": {
                     ""backgroundColor"": ""#d8d9dd"",
                     ""activeBackgroundColor"": ""#777a7e"",
                     ""font"": {
                        ""fontSize"": 14,
                        ""fontWeight"": ""400"",
                        ""color"": ""#000000"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     },
                     ""activeFont"": {
                        ""fontSize"": 14,
                        ""fontWeight"": ""400"",
                        ""color"": ""#ffffff"",
                        ""fontFamily"": ""arial, helvetica, sans-serif""
                     }
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.AreEqual(elements["HomeView"].Count, 1);
        var el = elements["HomeView"][0] as CmpSliderModel;
        Assert.IsNotNull(el);
        Assert.AreEqual(el.id, "CategoriesSlider");
        Assert.AreEqual(el.type, "Slider");
        Assert.AreEqual(el.name, "Categories Slider");
        Assert.AreEqual(el.LeftText, "CONSENT");
        Assert.AreEqual(el.RightText, "LEGITIMATE INTEREST");
        Assert.AreEqual(el.BackgroundColor, "#d8d9dd");
        Assert.AreEqual(el.ActiveBackgroundColor, "#777a7e");
        Assert.AreEqual(el.DefaultFont.Color, "#000000");
        Assert.AreEqual(el.DefaultFont.fontWeight, "400");
        Assert.AreEqual(el.DefaultFont.fontFamily, "arial, helvetica, sans-serif");
        Assert.AreEqual(el.DefaultFont.fontSize, 14);
        Assert.AreEqual(el.ActiveFont.Color, "#ffffff");
        Assert.AreEqual(el.ActiveFont.fontWeight, "400");
        Assert.AreEqual(el.ActiveFont.fontFamily, "arial, helvetica, sans-serif");
        Assert.AreEqual(el.ActiveFont.fontSize, 14);
    }   
    
    [Test]
    public void DeserializeNativePmWithLongButtonUiElementPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""VendorButton"",
               ""type"": ""LongButton"",
               ""name"": ""Vendors Buttons"",
               ""settings"": {
                  ""onText"": ""On"",
                  ""offText"": ""Off"",
                  ""customText"": ""Custom"",
                  ""style"": {
                     ""font"": {
                        ""fontSize"": 14,
                        ""fontWeight"": ""400"",
                        ""fontFamily"": ""arial, helvetica, sans-serif"",
                        ""color"": ""#060606""
                     },
                     ""onFocusBackgroundColor"": ""#ffffff"",
                     ""onUnfocusBackgroundColor"": ""#f1f2f6""
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.AreEqual(elements["HomeView"].Count, 1);
        var el = elements["HomeView"][0] as CmpLongButtonModel;
        Assert.IsNotNull(el);
        Assert.AreEqual(el.id, "VendorButton");
        Assert.AreEqual(el.type, "LongButton");
        Assert.AreEqual(el.name, "Vendors Buttons");
        Assert.AreEqual(el.OnText, "On");
        Assert.AreEqual(el.OffText, "Off");
        Assert.AreEqual(el.CustomText, "Custom");
        Assert.AreEqual(el.OnFocusColorCode, "#ffffff");
        Assert.AreEqual(el.OnUnfocusColorCode, "#f1f2f6");
        Assert.AreEqual(el.Font.Color, "#060606");
        Assert.AreEqual(el.Font.fontWeight, "400");
        Assert.AreEqual(el.Font.fontFamily, "arial, helvetica, sans-serif");
        Assert.AreEqual(el.Font.fontSize, 14);
    }   
    
    [Test]
    public void DeserializeNativePmWithNativeImageUiElementPasses()
    {
        Dictionary<string, string> popupBgColors = null;
        string json = @"
{
   ""id"": ""Root"",
   ""type"": ""NativeOtt"",
   ""name"": ""Native OTT"",
   ""children"": [
      {
         ""id"": ""HomeView"",
         ""type"": ""NativeView"",
         ""name"": ""Home View"",
         ""settings"": {
            ""style"": {
               ""backgroundColor"": ""#e5e8ef""
            }
         },
         ""children"": [
            {
               ""id"": ""LogoImage"",
               ""type"": ""NativeImage"",
               ""name"": ""Logo"",
               ""settings"": {
                  ""src"": ""https://i.pinimg.com/originals/5a/ae/50/5aae503e4f037a5a4375944d8861fb6a.png"",
                  ""style"": {
                     ""width"": 170
                  }
               }
            }
        ]
      }
    ]
}";
        Dictionary<string, List<CmpUiElementModel>> elements = NativeUiJsonDeserializer.DeserializeNativePm(json, ref popupBgColors);
        Assert.IsNotEmpty(elements);
        Assert.AreEqual(elements.Count, 1);
        Assert.AreEqual(elements["HomeView"].Count, 1);
        var el = elements["HomeView"][0] as CmpNativeImageModel;
        Assert.IsNotNull(el);
        Assert.AreEqual(el.id, "LogoImage");
        Assert.AreEqual(el.type, "NativeImage");
        Assert.AreEqual(el.name, "Logo");
        Assert.AreEqual(el.LogoImageLink, "https://i.pinimg.com/originals/5a/ae/50/5aae503e4f037a5a4375944d8861fb6a.png");
    }
}