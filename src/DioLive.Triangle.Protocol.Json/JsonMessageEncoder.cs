using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DioLive.Triangle.Protocol.Json
{
    public class JsonMessageEncoder<T> : IMessageEncoder<T>
    {
        public async Task<T> DecodeAsync(HttpContent content)
        {
            string body = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body);
        }

        public async Task<HttpContent> EncodeAsync(T request)
        {
            return await Task.Run(() => new StringContent(JsonConvert.SerializeObject(request)));
        }

        // TODO: fix this
        public T Read(Stream stream)
        {
            return JsonConvert.DeserializeObject<T>(new StreamReader(stream).ReadToEnd());
        }

        public void Write(Stream stream, T request)
        {
            new StreamWriter(stream).Write(JsonConvert.SerializeObject(request));
        }
    }
}