using System;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using MatchingEngineLight;
using Xunit;

namespace MatchingEngineLightTest
{
    public class MatchingEngineTests
    {
        [Fact]
        public void Test1()
        {
            MatchingEngine engine = new MatchingEngine();
            UInt32 price = 1000;
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.BUY, price, 10, OrderType.GFD, "order1");
            engine.SendOrder(order1);

            engine.orderDict.Keys.Should().Contain("order1");
            engine.OrderEntries[price].TotalQuantity.Should().Be(10);
            engine.OrderEntries[price].Orders.Count.Should().Be(1);
            order1.Status.Should().Be(OrderStatus.ON_MARKET);
            engine.MaxBid.Should().Be(1000);

        }
        
        [Fact]
        public void Test2()
        {
            MatchingEngine engine = new MatchingEngine();
            UInt32 price = 1000;
            
            engine.InitOrderBook();
            
            engine.SendOrder(new Order(OrderSide.BUY,price,10,OrderType.GFD,"order1"));
            engine.SendOrder(new Order(OrderSide.BUY,price,20,OrderType.GFD,"order2"));

            engine.orderDict.Keys.Should().Contain("order1");
            engine.orderDict.Keys.Should().Contain("order2");
            engine.OrderEntries[price].TotalQuantity.Should().Be(30);
            engine.OrderEntries[price].Orders.Count.Should().Be(2);
            engine.MaxBid.Should().Be(1000);

        }
        
        [Fact]
        public void Test3()
        {
            MatchingEngine engine = new MatchingEngine();
            UInt32 price = 1000;
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.BUY, price, 10, OrderType.IOC, "order1");
            engine.SendOrder(order1);

            engine.orderDict.Keys.Should().Contain("order1");
            engine.OrderEntries[price].TotalQuantity.Should().Be(0);
            engine.OrderEntries[price].Orders.Count.Should().Be(0);
            order1.Status.Should().Be(OrderStatus.CANCELED);
            engine.MaxBid.Should().Be(1);
        }
        
        [Fact]
        public void Test4()
        {
            MatchingEngine engine = new MatchingEngine();
            
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.BUY, 1000, 10, OrderType.GFD, "order1");
            Order order2 = new Order(OrderSide.BUY, 1001, 20, OrderType.GFD, "order2");
            engine.SendOrder(order1);
            engine.SendOrder(order2);

            engine.orderDict.Keys.Should().Contain("order1");
            engine.OrderEntries[1000].TotalQuantity.Should().Be(10);
            engine.OrderEntries[1000].Orders.Count.Should().Be(1);
            engine.OrderEntries[1001].TotalQuantity.Should().Be(20);
            engine.OrderEntries[1001].Orders.Count.Should().Be(1);
            order1.Status.Should().Be(OrderStatus.ON_MARKET);
            order2.Status.Should().Be(OrderStatus.ON_MARKET);
            engine.MaxBid.Should().Be(1001);
        }   

        [Fact]
        public void Test5()
        {
            MatchingEngine engine = new MatchingEngine();
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.BUY, 1000, 10, OrderType.GFD, "order1");
            Order order2 = new Order(OrderSide.SELL, 900, 20, OrderType.GFD, "order2");
            engine.SendOrder(order1);
            engine.SendOrder(order2);

            engine.orderDict.Keys.Should().Contain("order1");
            engine.orderDict.Keys.Should().Contain("order2");
            
            engine.OrderEntries[1000].TotalQuantity.Should().Be(0);
            engine.OrderEntries[1000].Orders.Count.Should().Be(0);
            order1.Status.Should().Be(OrderStatus.FILLED);
            order1.FilledQuantity.Should().Be(10);
            
            order2.Status.Should().Be(OrderStatus.ON_MARKET);
            order2.FilledQuantity.Should().Be(10);
            
            engine.MaxBid.Should().Be(1);
            engine.MinAsk.Should().Be(900);
        }   
        
        [Fact]
        public void AddAndCancelOrder()
        {
            MatchingEngine engine = new MatchingEngine();
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.BUY, 1000, 10, OrderType.GFD, "order1");

            engine.SendOrder(order1);
            engine.CancelOrder("order1");

            engine.orderDict.Keys.Should().Contain("order1");

            
            engine.OrderEntries[1000].TotalQuantity.Should().Be(0);
            engine.OrderEntries[1000].Orders.Count.Should().Be(1);
            engine.MaxBid.Should().Be(1);
            
            order1.Status.Should().Be(OrderStatus.CANCELED);
        }   
        
        [Fact]
        public void SellOrderShouldNotMatchCanceledBuyOrder()
        {
            MatchingEngine engine = new MatchingEngine();
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.BUY, 1000, 10, OrderType.GFD, "order1");

            engine.SendOrder(order1);
            engine.CancelOrder("order1");

            engine.orderDict.Keys.Should().Contain("order1");

            
            engine.OrderEntries[1000].TotalQuantity.Should().Be(0);
            engine.OrderEntries[1000].Orders.Count.Should().Be(1);
            engine.MaxBid.Should().Be(1);
            
            order1.Status.Should().Be(OrderStatus.CANCELED);
        }

        [Fact]
        public void CancelFilledOrderShouldFail()
        {
            MatchingEngine engine = new MatchingEngine();
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.BUY, 1000, 10, OrderType.GFD, "order1");
            Order order2 = new Order(OrderSide.SELL, 1000, 10, OrderType.GFD, "order2");
            engine.SendOrder(order1);
            engine.SendOrder(order2);
            engine.CancelOrder("order2");

            order1.Status.Should().Be(OrderStatus.FILLED);
            order2.Status.Should().Be(OrderStatus.FILLED);
            engine.OrderEntries[1000].TotalQuantity.Should().Be(0);
        }

        [Fact]
        public void CancelPartiallyFilled()
        {
            MatchingEngine engine = new MatchingEngine();
            
            engine.InitOrderBook();
            Order order1 = new Order(OrderSide.SELL, 1000, 10, OrderType.GFD, "order1");
            Order order2 = new Order(OrderSide.BUY, 1100, 20, OrderType.GFD, "order2");
            Order order3 = new Order(OrderSide.BUY, 1100, 20, OrderType.GFD, "order3");
            Order order4 = new Order(OrderSide.SELL, 900, 60, OrderType.GFD, "order4");      
            
            engine.SendOrder(order1);
            engine.SendOrder(order2);
            engine.CancelOrder("order2");
            engine.SendOrder(order3);
            engine.MaxBid.Should().Be(1100);
            
            engine.SendOrder(order4);
            
            order1.Status.Should().Be(OrderStatus.FILLED);
            order2.Status.Should().Be(OrderStatus.CANCELED);
            
            engine.OrderEntries[1100].TotalQuantity.Should().Be(0);
            engine.OrderEntries[1100].Orders.Count.Should().Be(0);
            engine.MaxBid.Should().Be(engine.MinPrice);
            engine.MinAsk.Should().Be(900);

            engine.OrderEntries[900].TotalQuantity.Should().Be(40);
            order2.Status.Should().Be(OrderStatus.CANCELED);
            order2.FilledQuantity.Should().Be(10);
            order3.Status.Should().Be(OrderStatus.FILLED);
            order4.Status.Should().Be(OrderStatus.ON_MARKET);
            order4.FilledQuantity.Should().Be(20);
        }
    }
}