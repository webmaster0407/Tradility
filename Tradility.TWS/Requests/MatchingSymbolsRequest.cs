using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.TWS.API;

namespace Tradility.TWS.Requests
{
    public class MatchingSymbolsRequest : BaseRequest
    {
        private readonly List<ContractDescription> contractDescriptions;

        public MatchingSymbolsRequest(Client client) : base(client)
        {
            contractDescriptions = new();
        }

        protected override void OnStartRequest()
        {
            Client.Wrapper.SymbolSamples += Wrapper_SymbolSamples;
        }

        protected override void OnEndRequest()
        {
            Client.Wrapper.SymbolSamples -= Wrapper_SymbolSamples;
        }

        private void Wrapper_SymbolSamples(object? sender, SymbolSamplesEventArgs e)
        {
            if (e.ReqId == ReqId)
                contractDescriptions.AddRange(e.ContractDescriptions);

            TaskComplete();
        }

        public async Task<List<ContractDescription>> GetAsync(string pattern)
        {
            StartRequest();
            Client.Socket.reqMatchingSymbols(ReqId, pattern);
            await Task();
            EndRequest();

            return contractDescriptions;
        }
    }
}
