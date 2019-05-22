namespace MatchingEngineLight
{
    public enum Operation
    {
        SEND,
        CANCEL,
        MODIFY,
        PRINT,
        UNKNOWN
    }
    
    public enum OrderSide
    {
        BUY,
        SELL
    }

    public enum OrderType
    {
        GFD,
        IOC
    }

    public enum OrderStatus
    {
        INIT,
        ON_MARKET,
        FILLED,
        CANCELED,
    }
}