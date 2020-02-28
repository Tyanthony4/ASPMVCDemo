using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWebApplication.ViewModels
{
    public class PatternViewModel
    {
        public PatternViewModel()
        {
            OpenTimes = new List<double>();
            Opens = new List<string>();
            Highs = new List<string>();
            Lows = new List<string>();
            Closes = new List<string>();
            Volumes = new List<string>();
            CloseTimes = new List<double>();
            QuoteAssetVolumes = new List<string>();
            NumberOfTrades = new List<int>();
            TakerBuyBaseAssetVolume = new List<string>();
            TakerBuyQuoteAssetVolume = new List<string>();

        }
        public string AskPrice { get; set; }
        public string PatternType { get; set; }
        public string Price { get; set; }
        public string Interval { get; set; }
        public string Symbol { get; set; }
        public List<double> OpenTimes { get; set; }
        public List<string> Opens { get; set; }
        public List<string> Highs { get; set; }
        public List<string> Lows { get; set; }
        public List<string> Closes { get; set; }
        public List<string> Volumes { get; set; }
        public List<double> CloseTimes { get; set; }
        public List<string> QuoteAssetVolumes { get; set; }
        public List<int> NumberOfTrades { get; set; }
        public List<string> TakerBuyBaseAssetVolume { get; set; }
        public List<string> TakerBuyQuoteAssetVolume { get; set; }
    }
}
