using System;
using System.Threading;
using FluentAssertions;
using OrderMatchingEngine;
using Serilog;
using Xunit;

namespace OrderMatchingEngineTest
{
    public class EnginTests
    {
        private Engine btcusdEngine;
        
        public EnginTests()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            btcusdEngine = new Engine(logger);
            
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
        
        [Fact]
        public void PassiveOrderInTheSpread_Should_UpdateBidAsk()
        {
            UInt64 quantity = 100;
            UInt64 buyPrice1 = 4;
            UInt64 buyPrice2 = 20;
            UInt64 sellPrice1 = 90;
            UInt64 sellPrice2 = 60;
            
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice1, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice2, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice1, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice2, quantity));
            Thread.Sleep(1000);
            btcusdEngine.BidMax.Should().Be(buyPrice2);
            btcusdEngine.AskMin.Should().Be(sellPrice2);
        }
        
        [Fact]
        public void PassiveOrderOutsideOffTheSpread_ShouldNot_UpdateBidAsk()
        {
            UInt64 quantity = 100;
            UInt64 buyPrice1 = 20;
            UInt64 buyPrice2 = 10;
            UInt64 sellPrice1 = 60;
            UInt64 sellPrice2 = 90;
            
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice1, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice2, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice1, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice2, quantity));
            Thread.Sleep(1000);
            btcusdEngine.BidMax.Should().Be(buyPrice1);
            btcusdEngine.AskMin.Should().Be(sellPrice1);
        }
        
        [Fact]
        public void MatchOrdersWithTheSameQuantityShouldResetSpread()
        {
            UInt64 quantity = 100;
            UInt64 buyPrice1 = 50;
            UInt64 sellPrice1 = 50;
            
            btcusdEngine.SendToEngine(new Order(OrderSide.BUY, "btcusd", buyPrice1, quantity));
            btcusdEngine.SendToEngine(new Order(OrderSide.SELL, "btcusd", sellPrice1, quantity));
            Thread.Sleep(1000);
            btcusdEngine.BidMax.Should().Be(buyPrice1);
            btcusdEngine.AskMin.Should().Be(sellPrice1);
        }
    }
}