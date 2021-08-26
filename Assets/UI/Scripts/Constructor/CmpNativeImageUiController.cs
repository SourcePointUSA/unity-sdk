using Assets.UI.Scripts.Util;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CmpNativeImageUiController : CmpLocalizationUiElement
{
    [SerializeField] private Image CompanyLogo;
    private string imageLink;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var nativeImage = elementModel as CmpNativeImageModel;
        imageLink = nativeImage.LogoImageLink;
        StartCoroutine(DownloadImage(imageLink));
        model = nativeImage;
    }

    private IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError
            || request.result == UnityWebRequest.Result.DataProcessingError
            || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            yield return CompanyLogo.ChangeColor(new Color(CompanyLogo.color.r, CompanyLogo.color.g, CompanyLogo.color.b, 0f));
            CompanyLogo.sprite = sprite;
            yield return CompanyLogo.ChangeColor(new Color(CompanyLogo.color.r, CompanyLogo.color.g, CompanyLogo.color.b, 1f));
        }
    }
}