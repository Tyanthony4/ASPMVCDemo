using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCWebApplication.Models;
using MVCWebApplication.ViewModels;
using Newtonsoft.Json;
using System.IO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCWebApplication.Controllers
{
    public class CandlestickController : Controller
    {
        
        string burl = "https://api.binance.com";
        string query = "";

        List<string> Tickers = new List<string>()
        {
            "BTCUSDT",
            "XVGBTC",
            "PPTBTC",
            "DOGEBTC",
            "ONEBTC",
            "LENDBTC",
            "LOOMBTC",
            "RLCBTC",
            "ERDBTC",
            "WABIBTC",
            "KMDBTC",
            "AIONBTC",
            "DGDBTC",
            "XRPBTC",
            "ETHBTC",
            "LTCBTC",
            "KNCBTC",
            "HOTBTC",
            "THETABTC",
            "GASBTC",
            "ZENBTC",
            "ZRXBTC",
            "MATICBTC",
            "FETBTC",
            "CELRBTC",
            "SNMBTC",
            "BATBTC",
            "IOSTBTC",
            "MTHBTC",
            "GOBTC",
            "BRDBTC",
            "REQBTC",
            "QKCBTC",
            "FUNBTC",
            "WAVESBTC",
            "WANBTC",
            "ADABTC",
            "XRPBTC",
            "POABTC",
            "OSTBTC",
            "STRATBTC",
            "BEAMBTC",
            "ATOMBTC",
            "NEOBTC",
            "RVNBTC",
            "TNTBTC",
            "XTZBTC",
            "VIABTC"

        };
        // GET: /<controller>/
        public CandlestickController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        public ViewResult Pattern()
        {
            ViewBag.Message = "Hello";
            return View();
        }
        private List<Candlestick> GetCandlesticks(string ticker, string interval, string limit)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            List<Candlestick> listOfCandlesticks = new List<Candlestick>();
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization");
            string query = "/api/v1/klines?symbol=" + ticker + "&interval=" + interval + "&limit=" + limit;
            var result = client.GetAsync(new Uri(burl + query)).Result;
            if (result.IsSuccessStatusCode)
            {
                string myContent = result.Content.ReadAsStringAsync().Result;
                var candlestickRange = JsonConvert.DeserializeObject<List<List<string>>>(myContent);

                foreach (var candlestick in candlestickRange)
                {
                    Candlestick cvm = new Candlestick(ticker, candlestick);
                    listOfCandlesticks.Add(cvm);
                }
                return listOfCandlesticks;
                //WriteToTextFile(listOfCandlesticks);
                //return View(listOfCandlesticks);
            }
            return null;
        }
        private Ticker GetPrice(string symbol)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            var priceQuery = "/api/v3/ticker/price";
            priceQuery += "?symbol=" + symbol;
            using (HttpClient priceClient = new HttpClient(handler))
            {
                priceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization");
                var result = priceClient.GetAsync(new Uri(burl + priceQuery)).Result;
                if (result.IsSuccessStatusCode)
                {
                    string myContent = result.Content.ReadAsStringAsync().Result;
                    var ticker = JsonConvert.DeserializeObject<Ticker>(myContent);
                    return ticker;

                }
            }

            return null;
        }

        private Ticker CheckIfBullFlagPattern(List<Candlestick> listOfCandlesticks)
        {
            List<Candlestick> orderedSticksByLow = listOfCandlesticks.OrderBy(x => x.Low).ToList<Candlestick>();
            List<Candlestick> orderedSticksByHigh = listOfCandlesticks.OrderBy(x => x.High).ToList<Candlestick>();

            string name = orderedSticksByLow.FirstOrDefault().Symbol;
            double price = Convert.ToDouble(orderedSticksByLow.FirstOrDefault().Price);
            double min = Convert.ToDouble(orderedSticksByLow.FirstOrDefault().Low);
            double max = Convert.ToDouble(orderedSticksByHigh.Last().High);
            double maxClose = Convert.ToDouble(listOfCandlesticks.OrderBy(x => x.Close).Last().Close);
            double percent = (max - min) / max;
            double percentThreshold = percent * .7;
            double minAverage = listOfCandlesticks.Average(x => Convert.ToDouble(x.Low));
            double minAVeragePercent = (max - minAverage) / max;


            if (percent >= .1)
            {
                if (Convert.ToDouble(orderedSticksByHigh.Last().OpenTime) >Convert.ToDouble( orderedSticksByLow.First().OpenTime))
                {
                    if ((int)minAVeragePercent <= (int)percentThreshold * (.98))
                    {
                        Ticker ticker = new Ticker();
                        ticker.Symbol = name;
                        ticker.Price = price;
                        double priceIncrease = (1 + (.667 * percent));
                        ticker.AskPrice = priceIncrease * min;
                        return ticker;

                    }
                }

            }
            return null;
        }
        [HttpPost]
        public ViewResult Pattern(SearchCandlestickRangeInputViewModel svm)
        {
           

            //string timeInMilliseconds = new DateTimeOffset(svm.EndTime).ToUnixTimeMilliseconds().ToString();
            //List<Pattern> Patterns = new List<Pattern>();
            var listOfCandlesticks = new List<Candlestick>();
            int counter = 0;
            List<PatternViewModel> patternViewModels = new List<PatternViewModel>();
            foreach(var ticker in Tickers)
            {
                counter++;
                Ticker tick = GetPrice(ticker);
                listOfCandlesticks.AddRange(GetCandlesticks(ticker, svm.Interval, svm.Limit));
                patternViewModels.Add(CreatePVM(tick, GetCandlesticks(ticker, svm.Interval, svm.Limit)));
                //if (counter > 5)
                //    break;
            }
            List<PatternViewModel> Patterns = new List<PatternViewModel>();
            if (listOfCandlesticks.Count > 0)
            {
                List<Ticker> TickersThatPassedBullFlagTest = new List<Ticker>();
                var GroupByTicker = listOfCandlesticks.GroupBy(x => x.Symbol).ToList();
                foreach (IEnumerable<Candlestick> ticker in GroupByTicker)
                {
                    Ticker validTicker = CheckIfBullFlagPattern(ticker.ToList<Candlestick>());
                    if (validTicker != null)
                    {
                        TickersThatPassedBullFlagTest.Add(validTicker);
                    }
                    
                }
                if (TickersThatPassedBullFlagTest.Count > 0)
                {
                  
                    //SendEmailForTickersThatPassedBullFlagTest(TickersThatPassedBullFlagTest);
                    foreach (var ticker in TickersThatPassedBullFlagTest)
                    {
                        
                        PatternViewModel pvm = CreatePVM(ticker, listOfCandlesticks);
                        if (pvm != null)
                        {
                            Patterns.Add(pvm);
                        }
                        
                        
                    }

                    
                }
            }
                return View(Patterns);

        }

        private PatternViewModel CreatePVM(Ticker ticker, List<Candlestick> listOfCandlesticks)
        {
            PatternViewModel pvm = new PatternViewModel();


            var CandlesticksForValidTickers = listOfCandlesticks.FindAll(x => x.Symbol == ticker.Symbol);
            if (CandlesticksForValidTickers.Count > 0)
            {

                pvm.Symbol = ticker.Symbol;
                pvm.Price = ticker.Price.ToString();
                pvm.AskPrice = ticker.AskPrice.ToString();
                pvm.PatternType = "BULLFLAG";
                for (int i = 0; i < CandlesticksForValidTickers.Count; i++)
                {
                    Candlestick candlestick = CandlesticksForValidTickers[i];
                    pvm.Interval = candlestick.Interval;
                    pvm.OpenTimes.Add(Convert.ToDouble( candlestick.OpenTime));
                    pvm.Opens.Add(candlestick.Open);
                    pvm.Highs.Add(candlestick.High);
                    pvm.Lows.Add(candlestick.Low);
                    pvm.Closes.Add(candlestick.Close);
                    pvm.Volumes.Add(candlestick.Volume);
                    pvm.CloseTimes.Add(Convert.ToDouble(candlestick.CloseTime));
                    pvm.QuoteAssetVolumes.Add(candlestick.QuoteAssetVolume);
                    pvm.NumberOfTrades.Add(Convert.ToInt32(candlestick.NumberOfTrades));
                    pvm.TakerBuyBaseAssetVolume.Add(candlestick.TakerBuyBaseAssetVolume);
                    pvm.TakerBuyQuoteAssetVolume.Add(candlestick.TakerBuyQuoteAssetVolume);


                }
                return pvm;

            }
            return null;
        }

        public ViewResult List()
        {
            ViewBag.Message = "Hello";
            return View();
        }

        [HttpPost]
        public ViewResult List(SearchCandlestickRangeInputViewModel svm)
        {
            string timeInMilliseconds = new DateTimeOffset(svm.EndTime).ToUnixTimeMilliseconds().ToString();
            string query = "/api/v1/klines?symbol="+svm.Symbol +"&interval="+ svm.Interval +"&endTime="+ timeInMilliseconds+"&limit=" + svm.Limit;//"/api/v3/time";
            HttpClientHandler handler = new HttpClientHandler();
            handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization");
            var result = client.GetAsync(new Uri(burl + query)).Result;
            if (result.IsSuccessStatusCode)
            {
                string myContent = result.Content.ReadAsStringAsync().Result;
                var candlestickRange = JsonConvert.DeserializeObject<List<List<string>>>(myContent);
                var listOfCandlesticks = new List<CandlestickViewModel>();
                foreach (var candlestick in candlestickRange)
                {
                    CandlestickViewModel cvm = new CandlestickViewModel(candlestick);
                    listOfCandlesticks.Add(cvm);
                }

                WriteToTextFile(svm.Symbol,svm.EndTime.ToString(),  listOfCandlesticks);
                return View(svm);
            }


            SearchCandlestickRangeInputViewModel errorSvm = new SearchCandlestickRangeInputViewModel();
            errorSvm.Symbol = result.ReasonPhrase;
            //ViewBag.Message = text;
            return View(errorSvm);
        }

        private void WriteToTextFile(string symbol, string endTime, List<CandlestickViewModel> listOfCandlesticks)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = path + "/Tickers.txt";
            // This text is added only once to the file.
            if (!System.IO.File.Exists(filePath))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(filePath))
                {
                    sw.WriteLine("Symbol,EndTime,OpenTime,Open,High,Low,Close,Volume,CloseTime,QuoteAssetVolume,NumberOfTrades,TakerBaseAssetVolume,TakerBuyQuoteAssetVolume");   
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = System.IO.File.AppendText(filePath))
            {
                sw.WriteLine("New Pattern");
                //endTime is the time you selected to search at!
                foreach (var candlestick in listOfCandlesticks)
                {
                 
                    sw.WriteLine(symbol+","+ endTime +"," + candlestick.OpenTime + "," + candlestick.Open + "," + candlestick.High + "," + candlestick.Low
                        + "," + candlestick.Close + "," + candlestick.Volume + "," + candlestick.CloseTime + "," + candlestick.QuoteAssetVolume
                        + "," + candlestick.NumberOfTrades + "," + candlestick.TakerBuyBaseAssetVolume + "," + candlestick.TakerBuyQuoteAssetVolume);
                }
       
            }
        }
    }
}
