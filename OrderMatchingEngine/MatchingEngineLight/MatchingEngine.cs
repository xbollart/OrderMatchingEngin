using System;
using System.Collections.Generic;

namespace MatchingEngineLight
{
    public class MatchingEngine
    {
        public Dictionary<String, Order> OrderDict { get; set; } = new Dictionary<String, Order>() { };

        public OrderBookEntry[] OrderEntries { get; set; }
        public UInt32 MaxPrice { get; } = 100000;
        public UInt32 MinPrice { get; } = 1;
        public UInt32 MinQty { get; } = 1;

        public UInt32 MinAsk { get; private set; }
        public UInt32 MaxBid { get; private set; }

        public void InitOrderBook()
        {
            MinAsk = MaxPrice;
            MaxBid = MinPrice;

            OrderEntries = new OrderBookEntry[MaxPrice + 1];

            for (int i = 0; i < OrderEntries.Length; i++)
            {
                OrderEntries[i] = new OrderBookEntry();
            }
        }

        public void SendOrder(Order order)
        {
            if (!IsValid(order))
                return;

            if (OrderDict.ContainsKey(order.Id))
                return;

            OrderDict.Add(order.Id, order);

            // making buy order
            if (order.Side == OrderSide.BUY && order.Price < MinAsk)
            {
                if (order.Type == OrderType.IOC)
                {
                    order.Status = OrderStatus.CANCELED;
                    return;
                }

                OrderEntries[order.Price].AddOrder(order);
                if (order.Price > MaxBid)
                    MaxBid = order.Price;

                order.Status = OrderStatus.ON_MARKET;
            }
            // making sell order
            else if ((order.Side == OrderSide.SELL && order.Price > MaxBid))
            {
                if (order.Type == OrderType.IOC)
                {
                    order.Status = OrderStatus.CANCELED;
                    return;
                }

                OrderEntries[order.Price].AddOrder(order);
                if (order.Price < MinAsk)
                    MinAsk = order.Price;

                order.Status = OrderStatus.ON_MARKET;
            }
            // taking order
            else
            {
                TryMatch(order);
            }
        }

        public void UpdateOrder(String orderId, OrderSide side, UInt32 price, UInt32 qty)
        {
            if (!OrderDict.ContainsKey(orderId))
                return;

            var orderToUpdate = OrderDict[orderId];

            if (orderToUpdate.Status != OrderStatus.ON_MARKET)
                return;

            if (orderToUpdate.Type == OrderType.IOC)
                return;

            CancelOrder(orderId);
            OrderDict.Remove(orderId);
            var newOrder = new Order(side, price, qty, OrderType.GFD, orderId);
            SendOrder(newOrder);
        }

        public void CancelOrder(String id)
        {
            if (!OrderDict.ContainsKey(id))
                return;

            var orderToCancel = OrderDict[id];

            if (orderToCancel.Status != OrderStatus.ON_MARKET)
            {
                return;
            }

            orderToCancel.Status = OrderStatus.CANCELED;
            ulong openQuantity = orderToCancel.Quantity - orderToCancel.FilledQuantity;
            OrderEntries[orderToCancel.Price].TotalQuantity -= openQuantity;

            if (orderToCancel.Side == OrderSide.BUY)
            {
                while (OrderEntries[MaxBid].TotalQuantity == 0 && MaxBid > MinPrice)
                {
                    MaxBid--;
                }
            }
            else
            {
                while (OrderEntries[MinAsk].TotalQuantity == 0 && MinAsk < MaxPrice)
                {
                    MinAsk++;
                }
            }
        }

        private void TryMatch(Order order)
        {
            //match Buy taker against Sell maker
            if (order.Side == OrderSide.BUY && order.Price >= MinAsk)
            {
                TryMatchAsk(order);
            }
            //match Sell taker against Buy maker
            else if (order.Side == OrderSide.SELL && order.Price <= MaxBid)
            {
                TryMatchBid(order);
            }
        }

        private void TryMatchBid(Order incomingOrder)
        {
            while (incomingOrder.FilledQuantity < incomingOrder.Quantity && MaxBid >= incomingOrder.Price)
            {
                var currentEntry = OrderEntries[MaxBid];

                currentEntry.MatchOrder(incomingOrder);

                while (OrderEntries[MaxBid].TotalQuantity == 0 && MaxBid > MinPrice)
                {
                    MaxBid--;
                }
            }

            if (incomingOrder.Type == OrderType.GFD && incomingOrder.FilledQuantity < incomingOrder.Quantity)
            {
                MinAsk = incomingOrder.Price;
                incomingOrder.Status = OrderStatus.ON_MARKET;
                OrderEntries[MinAsk].AddOrder(incomingOrder);
            }
        }

        private void TryMatchAsk(Order incomingOrder)
        {
            while (incomingOrder.FilledQuantity < incomingOrder.Quantity && MinAsk <= incomingOrder.Price)
            {
                var currentEntry = OrderEntries[MinAsk];

                currentEntry.MatchOrder(incomingOrder);

                while (OrderEntries[MinAsk].TotalQuantity == 0 && MinAsk < MaxPrice)
                {
                    MinAsk++;
                }
            }

            if (incomingOrder.Type == OrderType.GFD && incomingOrder.FilledQuantity < incomingOrder.Quantity)
            {
                MaxBid = incomingOrder.Price;
                incomingOrder.Status = OrderStatus.ON_MARKET;
                OrderEntries[MaxBid].AddOrder(incomingOrder);
            }
        }

        private Boolean IsValid(Order order)
        {
            if (String.IsNullOrEmpty(order.Id))
                return false;
            if (order.Price < MinPrice || order.Price > MaxPrice)
                return false;
            if (order.Quantity < MinQty)
                return false;

            return true;
        }

        public void DisplayOrderBook()
        {
            Console.WriteLine("SELL:");
            for (UInt32 i = MaxPrice; i >= MinAsk; i--)
            {
                if (OrderEntries[i].TotalQuantity > 0)
                {
                    Console.WriteLine($"{i} {OrderEntries[i].TotalQuantity}");
                }
            }

            Console.WriteLine("BUY:");
            for (UInt64 i = MaxBid; i >= MinPrice; i--)
            {
                if (OrderEntries[i].TotalQuantity > 0)
                {
                    Console.WriteLine($"{i} {OrderEntries[i].TotalQuantity}");
                }
            }
        }
    }
}