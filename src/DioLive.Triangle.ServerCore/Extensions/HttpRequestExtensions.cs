using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNet.Http
{
    public static class HttpRequestExtensions
    {
        public static async Task<string> ReadStringAsync(this HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ReadStringAsync(request, Encoding.ASCII, cancellationToken);
        }

        public static async Task<string> ReadStringAsync(this HttpRequest request, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] buffer = new byte[request.ContentLength ?? 0];
            await request.Body.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            return encoding.GetString(buffer);
        }

        public static async Task<T> ReadJsonAsync<T>(this HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return JsonConvert.DeserializeObject<T>(await ReadStringAsync(request, cancellationToken));
        }

        public static async Task<T> ReadJsonAsync<T>(this HttpRequest request, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            return JsonConvert.DeserializeObject<T>(await ReadStringAsync(request, encoding, cancellationToken));
        }
    }
}