using System;

namespace OrderMatchingEngine
{
    public class Order
    {
        public Order(OrderSide side, String instrument, UInt64 price, UInt64 quantity)
        {
            Side = side;
            Instrument = instrument;
            Price = price;
            Quantity = quantity;
        }

        public UInt64 Price { get; set; }
        public UInt64 Quantity { get; set; }
        public OrderSide Side { get; set; }
        public UInt64 ExchangeOrderId { get; set; }
        public String ClientOrderId { get; set; }
        public String Instrument { get; set; }
    }
}