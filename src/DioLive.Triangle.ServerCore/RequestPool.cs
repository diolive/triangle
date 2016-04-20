using System.Collections.Concurrent;
using System.Collections.Generic;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.ServerCore
{
    public class RequestPool : BlockingCollection<UpdateRequest>
    {
        public IEnumerable<UpdateRequest> TakeAll()
        {
            while (this.Count > 0)
            {
                yield return this.Take();
            }
        }
    }
}