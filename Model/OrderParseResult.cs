using System.Collections.Generic;

namespace DroneFactory.Model
{
    public class OrderParseResult
    {
        public bool HasError;
        public string ErrorMessage;
        public List<OrderLine> Items;
    }
}
