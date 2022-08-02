using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.TWS.API;

namespace Tradility.TWS.Requests
{
    public class ContractDetailsRequest : BaseRequest
    {
        private readonly List<ContractDetails> contractDetails;

        public ContractDetailsRequest(Client client) : base(client)
        {
            contractDetails = new();
        }

        protected override void OnStartRequest()
        {
            Client.Wrapper.ContractDetails += Wrapper_ContractDetails;
            Client.Wrapper.ContractDetailsEnd += Wrapper_ContractDetailsEnd;
        }

        protected override void OnEndRequest()
        {
            Client.Wrapper.ContractDetails -= Wrapper_ContractDetails;
            Client.Wrapper.ContractDetailsEnd -= Wrapper_ContractDetailsEnd;
        }

        private void Wrapper_ContractDetailsEnd(object? sender, ContractDetailsEndEventArgs e)
        {
            if (e.ReqId == ReqId)
                TaskComplete();
        }

        private void Wrapper_ContractDetails(object? sender, ContractDetailsEventArgs e)
        {
            if (e.ReqId == ReqId)
                contractDetails.Add(e.ContractDetails);
        }

        public async Task<List<ContractDetails>> GetAsync(Contract contract)
        {
            StartRequest();
            Client.Socket.reqContractDetails(ReqId, contract);
            await Task();
            EndRequest();

            return contractDetails;
        }
    }
}
