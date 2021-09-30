using System.Collections;
using System.Collections.Generic;
using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;
using QRCoder;
using QRCoder.Unity;

public class CmpVendorDetailsScrollController : CmpScrollController
{
    [SerializeField] Text header;
    [SerializeField] Text description;
    [SerializeField] GameObject cmpDescriptionCellPrefab;
    [SerializeField] Image qrImage;
    CmpVendorModel model;

    internal void SetInfo(CmpVendorModel model)
    {
        header.text = model.name;
        description.text = model.cookieHeader;
        this.model = model;
        FillView();
    }

    private IEnumerator SmoothQrChange(Sprite qrSprite)
    {
        yield return qrImage.ChangeColor(new Color(qrImage.color.r, qrImage.color.g, qrImage.color.b, 0f));
        qrImage.sprite = qrSprite;
        yield return qrImage.ChangeColor(new Color(qrImage.color.r, qrImage.color.g, qrImage.color.b, 1f));

    }
    
    private void GenerateQR(string policyUrl)
    {
        if (!string.IsNullOrEmpty(policyUrl))
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(policyUrl, QRCodeGenerator.ECCLevel.Q);
            UnityQRCode qrCode = new UnityQRCode(qrCodeData);
            Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(20);
            Sprite qrSprite = Sprite.Create(qrCodeAsTexture2D, new Rect(0, 0, qrCodeAsTexture2D.width, qrCodeAsTexture2D.height), 
                                            new Vector2(qrCodeAsTexture2D.width / 2, qrCodeAsTexture2D.height / 2));
            StartCoroutine(SmoothQrChange(qrSprite));
        }
    }

    public CmpVendorModel GetModel()
    {
        return model;
    }

    public override void FillView()
    {
        ClearScrollContent();
        if (model != null)
        {
            GenerateQR(model.policyUrl);
            AddConsentCategories(model.consentCategories); // == Pusposes
            AddLegIntCategories(model.legIntCategories);
            AddSpecialPurposes(model.iabSpecialPurposes);
            AddFeatures(model.iabFeatures);
            AddSpecialFeatures(model.iabSpecialFeatures);
            ScrollAppear();
        }
    }

    private void AddLegIntCategories(List<CmpVendorCategoryModel> legIntCategories)
    {
        if (legIntCategories.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "LegitInterestsText");
            foreach (var model in legIntCategories)
                AddCell(model.name);
        }
    }

    private void AddSpecialFeatures(List<string> iabSpecialFeatures)
    {
        if (iabSpecialFeatures.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "SpecialFeaturesText");
            AddStringList(iabSpecialFeatures);
        }
    }

    private void AddFeatures(List<string> iabFeatures)
    {
        if (iabFeatures.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "FeaturesText");
            AddStringList(iabFeatures);
        }
    }

    private void AddSpecialPurposes(List<string> iabSpecialPurposes)
    {
        if (iabSpecialPurposes.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "SpecialPurposesText");
            AddStringList(iabSpecialPurposes);
        }
    }

    private void AddStringList(List<string> list)
    {
        foreach (string str in list)
            AddCell(str);
    }

    private void AddConsentCategories(List<CmpVendorCategoryModel> consentCategories)
    {
        if (consentCategories.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "PurposesText");
            foreach (CmpVendorCategoryModel model in consentCategories)
                AddCell(model.name);
        }
    }

    private void AddCell(string txt)
    {
        var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
        CmpLongButtonUiController longButtonController = cell.GetComponent<CmpLongButtonUiController>();
        var longElement = postponedElements["CategoryLongButton"];
        longButtonController.SetLocalization(longElement);
        longButtonController.SetMainText(txt);
    }
}