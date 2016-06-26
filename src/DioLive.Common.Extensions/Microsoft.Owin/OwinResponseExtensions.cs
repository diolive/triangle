using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Microsoft.Owin
{
    public static class OwinResponseExtensions
    {
        public static void WriteJson(this IOwinResponse response, object obj)
        {
            string text = JsonConvert.SerializeObject(obj);
            response.Write(text);
        }

        public static async Task WriteJsonAsync(this IOwinResponse response, object obj)
        {
            string text = JsonConvert.SerializeObject(obj);
            await response.WriteAsync(text);
        }
    }
}