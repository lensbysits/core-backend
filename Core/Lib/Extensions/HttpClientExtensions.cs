using System.Net.Http.Headers;

namespace Lens.Core.Lib;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string url, Action<HttpRequestHeaders> beforeRequest, HttpContent? content = null)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url);

		return SendRequest(httpClient, beforeRequest, content, request);
	}

	public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string url, Action<HttpRequestHeaders> beforeRequest, HttpContent? content = null)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, url);

		return SendRequest(httpClient, beforeRequest, content, request);
	}

	public static Task<HttpResponseMessage> PutAsync(this HttpClient httpClient, string url, Action<HttpRequestHeaders> beforeRequest, HttpContent? content = null)
	{
		var request = new HttpRequestMessage(HttpMethod.Put, url);

		return SendRequest(httpClient, beforeRequest, content, request);
	}

	public static Task<HttpResponseMessage> DeleteAsync(this HttpClient httpClient, string url, Action<HttpRequestHeaders> beforeRequest, HttpContent? content = null)
	{
		var request = new HttpRequestMessage(HttpMethod.Delete, url);

		return SendRequest(httpClient, beforeRequest, content, request);
	}

	private static Task<HttpResponseMessage> SendRequest(HttpClient httpClient, Action<HttpRequestHeaders> beforeRequest, HttpContent? content, HttpRequestMessage request)
	{
		beforeRequest(request.Headers);
      	if (content != null)
		{
			request.Content = content;
		}

		return httpClient.SendAsync(request);
	}
}
