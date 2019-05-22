using System;
using System.Collections.Generic;

namespace MatchingEngineLight
{
    public class OrderBookEntry
    {
        public void AddOrder(Order newOrder)
        {
            Orders.Enqueue(newOrder);
            TotalQuantity += newOrder.Quantity - newOrder.FilledQuantity;
        }

        public void MatchOrder(Order incomingOrder)
        {
            while (incomingOrder.FilledQuantity < incomingOrder.Quantity && Orders.Count > 0)
            {
                if (Orders.Peek().Status == OrderStatus.CANCELED)
                {
                    Orders.Dequeue();
                    continue;
                }

                if (Orders.Peek().Quantity <= incomingOrder.Quantity - incomingOrder.FilledQuantity)
                {
                    var matchedOrder = Orders.Dequeue();
                    UInt32 tradedQty = matchedOrder.Quantity - matchedOrder.FilledQuantity;
                    TotalQuantity -= tradedQty;
                    matchedOrder.FilledQuantity += tradedQty;
                    matchedOrder.Status = OrderStatus.FILLED;
                    incomingOrder.FilledQuantity = tradedQty;
                    if (incomingOrder.FilledQuantity == incomingOrder.Quantity)
                        incomingOrder.Status = OrderStatus.FILLED;
                    Console.WriteLine(
                        $"TRADE {matchedOrder.Id} {matchedOrder.Price} {tradedQty} {incomingOrder.Id} {incomingOrder.Price} {tradedQty}");
                }
                else
                {
                    var matchedOrder = Orders.Peek();
                    UInt32 tradedQty = incomingOrder.Quantity - incomingOrder.FilledQuantity;
                    TotalQuantity -= tradedQty;
                    matchedOrder.FilledQuantity += tradedQty;
                    incomingOrder.FilledQuantity += tradedQty;
                    incomingOrder.Status = OrderStatus.FILLED;
                    Console.WriteLine(
                        $"TRADE {matchedOrder.Id} {matchedOrder.Price} {tradedQty} {incomingOrder.Id} {incomingOrder.Price} {tradedQty}");
                }
            }
        }

        public Queue<Order> Orders { get; private set; } = new Queue<Order>();
        public UInt64 TotalQuantity { get; set; }
    }
}