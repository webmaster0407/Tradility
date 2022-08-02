using System.Collections.Generic;
using System.Threading.Tasks;
using Tradility.TWS.API;

namespace Tradility.TWS.Requests
{
    public class HistoricalDataRequest : BaseRequest
    {
        private readonly List<Bar> bars;

        public HistoricalDataRequest(Client client) : base(client)
        {
            bars = new();
        }

        protected override void OnStartRequest()
        {
            Client.Wrapper.HistoricalData += HistoricalDataWrapper_HistoricalData;
            Client.Wrapper.HistoricalDataEnd += HistoricalDataWrapper_HistoricalDataEnd;
        }

        protected override void OnEndRequest()
        {
            Client.Wrapper.HistoricalData -= HistoricalDataWrapper_HistoricalData;
            Client.Wrapper.HistoricalDataEnd -= HistoricalDataWrapper_HistoricalDataEnd;
        }

        private void HistoricalDataWrapper_HistoricalDataEnd(object? sender, HistoricalDataEndEventArgs e)
        {
            if (e.RequestId == ReqId)
                TaskComplete();
        }

        private void HistoricalDataWrapper_HistoricalData(object? sender, HistoricalDataEventArgs e)
        {
            if (e.RequestId == ReqId)
                bars.Add(e.Bar);
        }

        public async Task<List<Bar>> GetAsync(Contract contract, string endDate, string duration, string barSizeSetting, string whatToShow, int useRTH, int formatDate, bool keepUpToDate, List<TagValue> chartOptions)
        {
            StartRequest();
            Client.Socket.reqHistoricalData(ReqId, contract, endDate, duration, barSizeSetting, whatToShow, useRTH, formatDate, keepUpToDate, chartOptions);
            await Task();
            EndRequest();

            return bars;
        }
    }
}
