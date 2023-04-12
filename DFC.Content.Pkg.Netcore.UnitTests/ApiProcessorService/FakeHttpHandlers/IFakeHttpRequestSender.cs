using System.Net.Http;

namespace DFC.Content.Pkg.Netcore.ApiProcessorService.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
