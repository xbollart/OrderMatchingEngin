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
        INIT,
        ON_MARKET,
        CANCELED,
        REJECTED,
        PARTIAL_FILLED,
        FILLED,
    }

    public enum ErrorType
    {
        PRICE_OUSTIDE_OF_RANGE,
        QUANTITY_OUTSIDE_OF_RANGE
        
    }
}