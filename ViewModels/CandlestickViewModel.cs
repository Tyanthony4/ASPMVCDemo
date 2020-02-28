using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWebApplication.ViewModels
{
    public class CandlestickViewModel
    {
        public CandlestickViewModel(List<string> candlestick)
        {
            this.OpenTime = candlestick[0];
            this.Open = candlestick[1];
            this.High = candlestick[2];
            this.Low = candlestick[3];
            this.Close = candlestick[4];
            this.Volume = candlestick[5];
            this.CloseTime = candlestick[6];
            this.QuoteAssetVolume = candlestick[7];
            this.NumberOfTrades = candlestick[8];
            this.TakerBuyBaseAssetVolume = candlestick[9];
            this.TakerBuyQuoteAssetVolume = candlestick[10];

        }
        public string OpenTime { get; set; }
        public string Open { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Close { get; set; }
        public string Volume { get; set; }
        public string CloseTime { get; set; }
        public string QuoteAssetVolume { get; set; }
        public string NumberOfTrades { get; set; }
        public string TakerBuyBaseAssetVolume { get; set; }
        public string TakerBuyQuoteAssetVolume { get; set; }
    }
}
