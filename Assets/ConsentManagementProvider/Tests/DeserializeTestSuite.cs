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
                                                                  ""_id"":""60fa8f7c2f489a6308ee7abb"",
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

//TODO: DeserializeNativePm