using System.Net.Http;

namespace dfc_cmsapi_pkg_netcore.ApiProcessorService.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
