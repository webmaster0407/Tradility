using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TradingViewUdfProvider;
using TradingViewUdfProvider.Models;
using TradingViewUdfProvider.Utils;

namespace Tradility.StocksWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UDFController : Controller
    {
        private readonly UDFProvider provider;

        public UDFController(UDFProvider provider)
        {
            this.provider = provider;
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            var config = await provider.GetConfiguration();
            return Ok(config);
        }

        [HttpGet("symbols")]
        public async Task<IActionResult> GetSymbol([FromQuery] string? symbol = null)
        {
            if(symbol == null)
                return NotFound();

            var tvSymbolInfo = await provider.GetSymbol(symbol);
            return Ok(tvSymbolInfo);
        }

        /// <summary>
        /// Find available symbols by specified values
        /// </summary>
        [Route("search")]
        [HttpGet]
        public IActionResult FindSymbols([FromQuery] string? query = null, [FromQuery] string? type = null, [FromQuery] string? exchange = null, [FromQuery] int? limit = null)
        {
            return NotFound();
            //return _provider.FindSymbols(query, type, exchange, limit);
        }

        [HttpGet("history")]
        public async Task<OkObjectResult> GetHistory([FromQuery] long from, [FromQuery] long to, [FromQuery] string symbol, [FromQuery] string resolution)
        {
            var fromDate = Common.Extensions.DateTimeExtensions.UnixSecondsToDateTimeOffset(from).UtcDateTime;
            var toDate = Common.Extensions.DateTimeExtensions.UnixSecondsToDateTimeOffset(to).UtcDateTime;

            var response = await provider.GetHistory(fromDate, toDate, symbol, resolution);
            var bars = response?.Bars;

            var nextTime = Common.Extensions.DateTimeExtensions.ToUTCUnixSeconds(response?.NextTime);
            var nextTimeLong = (long?)nextTime;

            if (response?.Bars == null)
            {
                if (nextTimeLong != null)
                    return Ok(
                        new
                        {
                            status = "no_data",
                            nextTime = nextTimeLong
                        });
                return Ok(
                    new
                    {
                        status = "error",
                        errmsg = "missing data"
                    });
            }

            var status = GetStatus(response);
            var timestamps = bars.Select(q => Common.Extensions.DateTimeExtensions.ToUTCUnixSeconds(q.Timestamp)).ToArray();
            var closing = bars.Select(q => q.Close).ToArray();
            var opening = bars.Select(q => q.Open).ToArray();
            var high = bars.Select(q => q.High).ToArray();
            var low = bars.Select(q => q.Low).ToArray();
            var volume = bars.Select(q => q.Volume).ToArray();

            return Ok(new
            {
                s = status,
                t = timestamps,
                c = closing,
                o = ArrayOrNull(opening),
                h = ArrayOrNull(high),
                l = ArrayOrNull(low),
                v = ArrayOrNull(volume),
                errmsg = response.ErrorMessage,
                nextTime = nextTimeLong
            });
        }

        [Route("marks")]
        [HttpGet]
        public async Task<IActionResult> GetMarks([FromQuery] long from, [FromQuery] long to, [FromQuery] string symbol, [FromQuery] string resolution)
        {
            return NotFound();

            var fromDate = TvDateTimeConverter.ConvertFromUnixSeconds(from);
            var toDate = TvDateTimeConverter.ConvertFromUnixSeconds(to);

            var response = await provider.GetMarks(fromDate, toDate, symbol, resolution);
            var marks = response ?? new TvMark[0];

            var timestamps = marks.Select(q => q.Timestamp.ToUnixSeconds()).ToArray();
            var ids = marks.Select(q => q.Id).ToArray();
            var labels = marks.Select(q => q.Label).ToArray();
            var labelFonts = marks.Select(q => q.LabelFontColor).ToArray();
            var colors = marks.Select(q => q.Color).ToArray();
            var texts = marks.Select(q => q.Text).ToArray();
            var sizes = marks.Select(q => q.MinSize).ToArray();

            return Ok(new
            {
                id = ids,
                time = timestamps,
                label = labels,
                labelFontColor = labelFonts,
                text = texts,
                color = colors,
                minSize = sizes,
            });
        }

        private static string GetStatus(TvBarResponse response) => response.Status switch
        {
            TvBarStatus.Error => "error",
            TvBarStatus.NoData => "no_data",
            TvBarStatus.Ok => "ok",
            _ => "ok"
        };

        private static double?[]? ArrayOrNull(double?[] values)
        {
            if (values?.Any(x => x > 0) == true)
                return values;

            return null;
        }
    }
}
