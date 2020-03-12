namespace LAB.DataScanner.Components.DataRetriever
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HttpDataRetriever : IDataRetriever, IDisposable
    {
        private HttpClient httpClient = null;
        private bool isInitialized = false;

        ~HttpDataRetriever()
        {
            this.Dispose();
        }

        public async Task<byte[]> RetrieveBytesAsync(string uri)
        {
            if (this.httpClient == null || this.isInitialized == false)
            {
                this.Initialize();
            }

            return await this.httpClient.GetByteArrayAsync(uri);
        }

        public async Task<string> RetrieveStringAsync(string uri)
        {
            if (this.httpClient == null || this.isInitialized == false)
            {
                this.Initialize();
            }

            return await this.httpClient.GetStringAsync(uri);
        }

        public async Task<HttpResponseMessage> RetrieveGetResponse(string uri)
        {
            if (this.httpClient == null || this.isInitialized == false)
            {
                this.Initialize();
            }

            return await this.httpClient.GetAsync(uri);
        }

        public async Task<HttpResponseMessage> RetrievePostResponse(string uri, string body)
        {
            if (this.httpClient == null || this.isInitialized == false)
            {
                this.Initialize();
            }

            var content = new StringContent(body);
            return await this.httpClient.PostAsync(uri, content);
        }

        private void Initialize()
        {
            this.httpClient = new HttpClient();
            this.isInitialized = true;
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
