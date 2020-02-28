using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWebApplication.Models
{
    public class Pattern
    {
        public string Symbol { get; set; }
        public List<Candlestick> Candlesticks { get; set; }
        public Pattern()
        {
            Candlesticks = new List<Candlestick>();
        }
    }
}
