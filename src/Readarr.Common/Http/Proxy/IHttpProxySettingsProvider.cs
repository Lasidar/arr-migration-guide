namespace Readarr.Common.Http.Proxy
{
    public interface IHttpProxySettingsProvider
    {
        HttpProxySettings GetProxySettings(HttpUri uri);
        HttpProxySettings GetProxySettings();
    }
}
