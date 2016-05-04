using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DioLive.Triangle.Protocol
{
    public interface IMessageEncoder<T>
    {
        Task<T> DecodeAsync(HttpContent content);

        Task<HttpContent> EncodeAsync(T request);

        T Read(Stream stream);

        void Write(Stream stream, T request);
    }
}