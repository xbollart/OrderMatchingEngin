using System;
using System.Collections.Generic;

namespace OrderMatchingEngine
{
    public class OrderBookEntry //todo think of a struct instead
    {
        public void AddOrder(Order newOrder)
        {
            Orders.Enqueue(newOrder);
            TotalQuantity += newOrder.Quantity;
        }

        public void CancelOrder()
        {
            
        }

        public void UpdateOrder()
        {
            
        }

        public void MatchOrder()
        {
            
     //       TotalQuantity -= newOrder.Quantity;
        }

        public Queue<Order> Orders { get; private set; } = new Queue<Order>();
        public HashSet<String> CanceledOrders { get; private set; } = new HashSet<string>();
        public UInt64 TotalQuantity { get; set; }
    }
}