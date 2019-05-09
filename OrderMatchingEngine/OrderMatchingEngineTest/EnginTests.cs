using System;
using System.Threading;
using FluentAssertions;
using OrderMatchingEngine;
using Xunit;

namespace OrderMatchingEngineTest
{
    public class EnginTests
    {
        Engine btcusdEngine = new Engine();
        
        public EnginTests()
        {
            if (btcusdEngine.Init(1,100,1000))
            {
                Thread btcusdEngineThread = new Thread(() => btcusdEngine.Start());
                btcusdEngineThread.Start();
            }
        }

        [Fact]
        public void AddTowBuyOrdersWithSamePriceToOrderBook_Should_IncrementTotalQuantity()
        {
            UInt64 quantity = 100;
            UInt64 buyPrice = 4;
            UInt64 sellPrice = 90;
            
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice, quantity));
            Thread.Sleep(1000);
            btcusdEngine.OrderEntries[buyPrice].TotalQuantity.Should().Be(quantity);
            btcusdEngine.OrderEntries[sellPrice].TotalQuantity.Should().Be(quantity);
            btcusdEngine.BidMax.Should().Be(buyPrice);
            btcusdEngine.AskMin.Should().Be(sellPrice);

        }
        
        [Fact]
        public void Add2BuyAnd2SellOrdersToOrderBook_Should_IncrementTotalQuantity()
        {
            UInt64 quantity1 = 100;
            UInt64 quantity2 = 200;
            UInt64 buyPrice = 4;
            UInt64 sellPrice = 90;
            
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice, quantity1));
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice, quantity2));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice, quantity1));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice, quantity2));
            Thread.Sleep(1000);
            btcusdEngine.OrderEntries[buyPrice].TotalQuantity.Should().Be(quantity1 + quantity2);
            btcusdEngine.OrderEntries[sellPrice].TotalQuantity.Should().Be(quantity1 + quantity2);
            btcusdEngine.BidMax.Should().Be(buyPrice);
            btcusdEngine.AskMin.Should().Be(sellPrice);
        }

    }
}