using UnityEngine;

public class LinkButton : MonoBehaviour
{
    public enum Platform
    {
        Undefined,
        Discord,
        Youtube,
        Twitter,
        Steam
    }

    [SerializeField] private Platform selectedPlatform;

    private string GetURL()
    {
        switch (selectedPlatform)
        {
            case Platform.Discord:
                return "n/a";
            case Platform.Youtube:
                return "https://www.youtube.com/@AzureForgeStudio";
            case Platform.Twitter:
                return "https://x.com/AzureForge_Std";
            case Platform.Steam:
                return "https://store.steampowered.com";
            default:
                return string.Empty;
        }
    }

    public void OpenLink()
    {
        string url = GetURL();

        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("URL não definido para a plataforma selecionada.");
        }
    }
}
