using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNet.Http
{
    public static class HttpResponseExtensions
    {
        public static async Task WriteJsonAsync<T>(this HttpResponse response, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            response.ContentType = "application/json";
            await response.WriteAsync(JsonConvert.SerializeObject(value), cancellationToken);
        }

        public static async Task WriteJsonAsync<T>(this HttpResponse response, T value, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            response.ContentType = "application/json";
            await response.WriteAsync(JsonConvert.SerializeObject(value), encoding, cancellationToken);
        }
    }
}