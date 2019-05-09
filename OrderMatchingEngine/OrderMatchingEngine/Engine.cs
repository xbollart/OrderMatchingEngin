using System;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Threading;


namespace OrderMatchingEngine
{
    public class Engine : IEngine
    {
        private ConcurrentQueue<Order> _incomingOrders = new ConcurrentQueue<Order>();
        public ConcurrentQueue<Trade> OutGoingTrades  = new ConcurrentQueue<Trade>();
        public ConcurrentQueue<Order> RejectedOrders  = new ConcurrentQueue<Order>();
        public OrderBookEntry[] OrderEntries { get; private set; }

        private Boolean _isUp = false;
        private Boolean _isInit = false;
        public UInt64 AskMin { get; private set; } = UInt64.MaxValue;
        public UInt64 BidMax { get; private set; } = UInt64.MinValue;
        private UInt64 _maxPrice = UInt64.MaxValue;
        private UInt64 _minPrice = UInt64.MinValue;
        
        public UInt64 _currOrderID = 0; // what is the max value of this???

        private Int64 _maxNbOrders;

        public Boolean Init(UInt64 minPrice, UInt64 maxPrice, Int64 maxNbOrders )
        {
            if (_isInit)
                return _isInit;

            if (minPrice == UInt64.MinValue || maxPrice == UInt64.MaxValue)
                return false;

            OrderEntries = new OrderBookEntry[maxPrice + 1]; // todo optimize size

            for (int i = 0; i < OrderEntries.Length; i++)
            {
                OrderEntries[i] = new OrderBookEntry();
            }
            
            AskMin = maxPrice;
            BidMax = minPrice;
            _maxNbOrders = maxNbOrders;

            _isInit = true;
            return _isInit;
        }

        public String SendToEngine(Order incomingOrder)
        {
            if (_incomingOrders.Count >= _maxNbOrders)
                return "System under heavy load Try again later";
            //todo return order with status rejected + msg reason
            _incomingOrders.Enqueue(incomingOrder);
            return "ack Order received";
        }

        public void Start()
        {
            // study thread safety
            
            if(_isUp)
                return;

            _isUp = true;
                            
            while (_isUp)
            {

                Process();
                
                //todo replace with a loop timer
                Thread.Sleep(100);
            }
        }

        private void Process()
        {
            if (_incomingOrders.Count == 0)
                return;
             
            if(!_incomingOrders.TryDequeue(out Order currOrder))
                return;
            
            if (currOrder.Price > _maxPrice || currOrder.Price < _minPrice)
            {
                RejectedOrders.Enqueue(currOrder);
                Console.WriteLine("Order rejected");
                return;
            }

            currOrder.ExchangeOrderId = _currOrderID;
            _currOrderID++;
            
            if (currOrder.Side == OrderSide.BUY && currOrder.Price < AskMin)
            {                   
                Console.WriteLine($"Add buy to order book Order: {currOrder.ExchangeOrderId}");
                OrderEntries[currOrder.Price].AddOrder(currOrder);
                if (currOrder.Price > BidMax)
                    BidMax = currOrder.Price;


            }
            else if ((currOrder.Side == OrderSide.SELL && currOrder.Price > BidMax))
            {
                Console.WriteLine($"Add sell to order book Order: {currOrder.ExchangeOrderId}");
                OrderEntries[currOrder.Price].AddOrder(currOrder);
                if (currOrder.Price < AskMin)
                    AskMin = currOrder.Price;

            }
            else
            {
                Console.WriteLine($"try match Order: {currOrder.ExchangeOrderId}");
                TryMatch(currOrder);
                
                //if match adjust bidMax / askMin
            }
            
            Console.WriteLine($"Bid/ask: {BidMax} | {AskMin}");
        }

        // FIFO matching
        private void TryMatch(Order order)
        {
         //   throw new NotImplementedException;
        }

        public void Stop()
        {
            _isUp = false;
        }
    }
}