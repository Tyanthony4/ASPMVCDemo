using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWebApplication.Models
{
    public class Ticker
    {

        public string Symbol { get; set; }
        public double Price { get; set; }
        public double AskPrice { get; set; }
    }
}
