using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWebApplication.ViewModels
{
    public class SearchCandlestickRangeInputViewModel
    {
        public string Symbol { get; set; }

        public string Limit {get;set;}
        public string Interval { get; set; }
        public DateTime EndTime { get; set; }

        
    }
}
