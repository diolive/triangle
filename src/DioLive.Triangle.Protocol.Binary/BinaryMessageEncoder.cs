using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DioLive.Triangle.Protocol.Binary
{
    internal abstract class BinaryMessageEncoder<T> : IMessageEncoder<T>
    {
        public async Task<T> DecodeAsync(HttpContent content)
        {
            return Read(await content.ReadAsStreamAsync());
        }

        public async Task<HttpContent> EncodeAsync(T request)
        {
            var stream = new MemoryStream();
            Write(stream, request);
            return await Task.Run(() => new ByteArrayContent(stream.ToArray()));
        }

        public abstract T Read(Stream stream);

        public abstract void Write(Stream stream, T request);
    }
}