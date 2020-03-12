namespace LAB.DataScanner.Components.DataRetriever
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IDataRetriever
    {
        Task<byte[]> RetrieveBytesAsync(string uri);
        Task<string> RetrieveStringAsync(string uri);
        Task<HttpResponseMessage> RetrieveGetResponse(string uri);
        Task<HttpResponseMessage> RetrievePostResponse(string uri, string body);
    }
}
