using System.Collections.Concurrent;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.ServerCore
{
    public class RequestPool : BlockingCollection<UpdateRequest>
    {
    }
}