namespace OrderMatchingEngine
{
    public enum OrderSide
    {
        BUY,
        SELL
    }

    public enum Operation
    {
        BUY, 
        SELL, 
        CANCEL, 
        MODIFY, 
        PRINT
    }

    public enum OrderStatus
    {
        ON_MARKET,
        CANCELED,
        REJECTED,
        PARTIAL_FILLED,
        FILLED,
    }
}