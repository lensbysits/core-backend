using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Lens.Core.Lib
{
	public static class HttpClientExtensions
	{
		public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string url, Action<HttpRequestHeaders> beforeRequest)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, url);

			beforeRequest(request.Headers);

			return httpClient.SendAsync(request);
		}

		public static async Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string url, HttpContent content, Action<HttpRequestHeaders> beforeRequest)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, url);
			beforeRequest(request.Headers);
			request.Content = content;
			return await httpClient.SendAsync(request);
		}
	}
}
