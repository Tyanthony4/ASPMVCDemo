using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWebApplication.Models
{
    public interface ICandlestickRepository
    {
        IEnumerable<Candlestick> AllCandlesticks { get; }
        Candlestick GetCandlestickById(int id);
    }
}
