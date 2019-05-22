using System;
using System.Collections.Generic;

namespace MatchingEngineLight
{
    class Solution
    {
        static void Main(string[] args)
        {
            List<String> input = new List<String>();

            string line;
            while ((line = Console.ReadLine()) != null && line != "")
            {
                input.Add(line);
            }

            var engine = new MatchingEngine();

            engine.InitOrderBook();

            foreach (var l in input)
            {
                if (String.IsNullOrEmpty(l))
                    return;

                String[] fields = l.Trim().Split(' ');

                if (fields.Length < 1)
                    return;

                Operation ope = ParseOpe(fields[0]);

                if (ope == Operation.UNKNOWN)
                    return;

                if (ope == Operation.PRINT)
                    engine.DisplayOrderBook();
                else if (ope == Operation.SEND)
                {
                    if (fields.Length != 5)
                        return;
                    var order = CreateNewOrder(fields);
                    engine.SendOrder(order);
                }
                else if (ope == Operation.CANCEL)
                {
                    if (fields.Length != 2)
                        return;
                    engine.CancelOrder(fields[1]);
                }
                else if (ope == Operation.MODIFY)
                {
                    if (fields.Length != 5)
                        return;
                    Enum.TryParse(fields[2], out OrderSide side);
                    UInt32.TryParse(fields[3], out UInt32 price);
                    UInt32.TryParse(fields[4], out UInt32 qty);
                    engine.UpdateOrder(fields[1], side, price, qty);
                }
            }
        }

        private static Order CreateNewOrder(String[] fields)
        {
            Enum.TryParse(fields[0], out OrderSide side);
            Enum.TryParse(fields[1], out OrderType type);
            UInt32.TryParse(fields[2], out UInt32 price);
            UInt32.TryParse(fields[3], out UInt32 qty);
            String orderId = fields[4];

            return new Order(side, price, qty, type, orderId);
        }

        private static Operation ParseOpe(String input)
        {
            switch (input)
            {
                case "BUY":
                case "SELL":
                    return Operation.SEND;
                case nameof(Operation.PRINT):
                    return Operation.PRINT;
                case nameof(Operation.CANCEL):
                    return Operation.CANCEL;
                case nameof(Operation.MODIFY):
                    return Operation.MODIFY;
                default:
                    return Operation.UNKNOWN;
            }
        }
    }
}

        
