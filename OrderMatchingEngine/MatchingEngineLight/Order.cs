using System;

namespace MatchingEngineLight
{
    public class Order
    {
        public Order(OrderSide side, UInt32 price, UInt32 quantity, OrderType type, String id)
        {
            Side = side;
            Price = price;
            Quantity = quantity;
            Type = type;
            Id = id;
            Status = OrderStatus.INIT;
            FilledQuantity = 0;
        }


        public UInt32 Price { get; private set; }
        public UInt32 Quantity { get; set; }
        public UInt32 FilledQuantity { get; set; }
        public OrderSide Side { get; private set; }
        public OrderType Type { get; private set; }
        public String Id { get; private set; }

        public OrderStatus Status { get; set; }
    }
}