using System;

namespace OrderMatchingEngine
{
    public class Trade
    {
        public Decimal OrderPrice { get; set; }
        public Decimal ExecutedPrice { get; set; }
        public Decimal OrderQuantity { get; set; }
        public Decimal ExecutedQuantity { get; set; }
        public UInt64 ExchangeOrderId { get; set; }
        public String ClientOrderId { get; set; }
        public String TradeId { get; set; }
        public String Instrument { get; set; }
        
    }
}