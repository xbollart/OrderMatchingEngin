using System;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Serilog;


namespace OrderMatchingEngine
{
    public class Engine : IEngine
    {
        private ConcurrentQueue<Order> _incomingOrders = new ConcurrentQueue<Order>();
        public ConcurrentQueue<Trade> OutGoingTrades  = new ConcurrentQueue<Trade>();
        public ConcurrentQueue<Order> RejectedOrders  = new ConcurrentQueue<Order>();
        public OrderBookEntry[] OrderEntries { get; private set; }

        private Boolean _stopRequested = false;
        private Boolean _resetRequested = false;
        private Boolean _isUp = false;
        private Boolean _isInit = false;
        public UInt64 AskMin { get; private set; } = UInt64.MaxValue;
        public UInt64 BidMax { get; private set; } = UInt64.MinValue;
        private UInt64 _maxPrice = UInt64.MaxValue;
        private UInt64 _minPrice = UInt64.MinValue;
        
        public UInt64 _currOrderID = 0; // what is the max value of this???

        private Int64 _maxNbOrders;
        private ILogger _logger;
        

        public Engine(ILogger logger)
        {
            _logger = logger;
        }

        public Boolean Init(UInt64 minPrice, UInt64 maxPrice, Int64 maxNbOrders )
        {
            _logger.Information("Init engine");
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

        public Order SendToEngine(Order incomingOrder)
        {
            if (_incomingOrders.Count >= _maxNbOrders)
            {
                incomingOrder.Status = OrderStatus.REJECTED;
                incomingOrder.ErrorMessage = "System under heavy load Try again later";
                return incomingOrder;
            }
            
            if (incomingOrder.Price > _maxPrice || incomingOrder.Price < _minPrice)
            {
                incomingOrder.Status = OrderStatus.REJECTED;
                incomingOrder.ErrorMessage = $"{ErrorType.PRICE_OUSTIDE_OF_RANGE.ToString()} should feet within min max price";
                RejectedOrders.Enqueue(incomingOrder);
                _logger.Information("order Rejected");
                return incomingOrder;
            }
            
            if (incomingOrder.Quantity == 0 )
            {
                incomingOrder.Status = OrderStatus.REJECTED;
                incomingOrder.ErrorMessage = $"{ErrorType.QUANTITY_OUTSIDE_OF_RANGE.ToString()} should feet within min max quantity";
                RejectedOrders.Enqueue(incomingOrder);
                _logger.Information("order Rejected");
                return incomingOrder;
            }

            incomingOrder.Status = OrderStatus.INIT;
            _incomingOrders.Enqueue(incomingOrder);
            
            return incomingOrder;
        }

        public void Start()
        {
            //todo study thread safety  probably need a lock
            _logger.Information("Start engine");
            if(_isUp)
                return;

            _isUp = true;

            while (!_stopRequested)
            {
                Process();
                //todo replace with a loop timer
                Thread.Sleep(100);
            }
            
            _logger.Information("Stop engine");
            _isUp = false;
        }

        private void Process()
        {
            if (_incomingOrders.Count == 0)
                return;
             
            if(!_incomingOrders.TryDequeue(out Order currOrder))
                return;

            currOrder.ExchangeOrderId = _currOrderID;
            _currOrderID++;
            
            if (currOrder.Side == OrderSide.BUY && currOrder.Price < AskMin)
            {                   
                _logger.Information($"Add buy to order book Order: {currOrder.ExchangeOrderId}");
                OrderEntries[currOrder.Price].AddOrder(currOrder);
                if (currOrder.Price > BidMax)
                    BidMax = currOrder.Price;
            }
            else if ((currOrder.Side == OrderSide.SELL && currOrder.Price > BidMax))
            {
                _logger.Information($"Add sell to order book Order: {currOrder.ExchangeOrderId}");
                OrderEntries[currOrder.Price].AddOrder(currOrder);
                if (currOrder.Price < AskMin)
                    AskMin = currOrder.Price;
            }
            else
            {
                _logger.Information($"try match Order: {currOrder.ExchangeOrderId}");
                TryMatch(currOrder);
            }
            
            _logger.Information($"Bid/ask: {BidMax} | {AskMin}");
        }

        // FIFO matching
        private void TryMatch(Order order)
        {                         
            //if match adjust bidMax / askMin
            
            if (order.Side == OrderSide.BUY && order.Price >= AskMin)
            {
                TryMatchAsk(order);
            }
            else if (order.Side == OrderSide.SELL && order.Price <= BidMax)
            {
                TryMatchBid(order);
            }
        }

        private void TryMatchAsk(Order order)
        {
            UInt64 currentPrice = AskMin;
            while (order.Quantity > 0 && currentPrice <= order.Price)
            {
                var currentEntry = OrderEntries[currentPrice];
                while (order.Quantity > 0 && currentEntry.Orders.Count > 0)
                {
                    
                }


            }
        }

        private void TryMatchBid(Order order)
        {
            
        }

        public void Stop()
        {
            _stopRequested = true;
        }
        
        public void Dispose()
        {
            if (!_isUp)
            {
                //todo clean up
            }
        }
    }
}