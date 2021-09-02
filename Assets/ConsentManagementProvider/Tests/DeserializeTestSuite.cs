using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class DeserializeTestSuite
{
    private const string ONE_SHORT_CATEGORY_COLLECTION_JSON = @"{""categories"":[{
                                                                  ""_id"":""60fa8f7c2f489a6308ee7abb"",
                                                                  ""type"":""IAB_PURPOSE"",
                                                                  ""name"":""Store and/or access information on a device"",
                                                                  ""description"":""Cookies, device identifiers, or other information can be stored or accessed on your device for the purposes presented to you.""
                                                                    }]}";
    private const string ONE_ODD_SHORT_CATEGORY_COLLECTION_JSON = @"{""categories"":[{}]}";

    [SetUp]
    public void Setup()
    {
    }

    [TearDown]
    public void Teardown()
    {
    }

    #region DeserializeShortCategories
    [Test]
    public void DeserializeShortCategoriesWithNullCollectionPasses()
    {
        List<CmpShortCategoryModel> shortCategories = null;
        NativeUiJsonDeserializer.DeserializeShortCategories(JSONSTUB.shortCategories, ref shortCategories);
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
    public void DeserializeShortCategoriesFails()
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
    #endregion
    
    // using JsonDocument document = JsonDocument.Parse(JSONSTUB.shortCategories);
    // JsonElement root = document.RootElement;
    // NativeUiJsonDeserializer.DeserializeCollection<CmpCategoryModel>(new JsonElement(), "");
    
    /*
        categoryModels.AddRange(DeserializeCollection<CmpCategoryModel>(root, "categories"));
        specialPurposeModels.AddRange(DeserializeCollection<CmpSpecialPurposeModel>(root, "specialPurposes"));
        featureModels.AddRange(DeserializeCollection<CmpFeatureModel>(root, "features"));
        specialFeatureModels.AddRange(DeserializeCollection<CmpSpecialFeatureModel>(root, "specialFeatures"));
        vendorModels.AddRange(DeserializeCollection<CmpVendorModel>(root, "vendors"));
 */
}
