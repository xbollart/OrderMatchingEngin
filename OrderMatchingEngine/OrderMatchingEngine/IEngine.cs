using System;

namespace OrderMatchingEngine
{
    public interface IEngine : IDisposable
    {
        Boolean Init(UInt64 minPrice, UInt64 maxPrice, Int64 maxNbOrders);
        void Start();
        void Stop();
    }
}