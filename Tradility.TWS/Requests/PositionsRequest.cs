using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tradility.TWS.API;

namespace Tradility.TWS.Requests
{
    public class PositionsRequest : BaseRequest
    {
        private readonly List<(string Account, double AvgCost, Contract Contract, double Pos)> positions;

        public PositionsRequest(Client client) : base(client)
        {
            positions = new();
        }

        protected override void OnStartRequest()
        {
            Client.Wrapper.Position += Wrapper_Position;
            Client.Wrapper.PositionEnd += Wrapper_PositionEnd;
        }

        protected override void OnEndRequest()
        {
            Client.Wrapper.Position -= Wrapper_Position;
            Client.Wrapper.PositionEnd -= Wrapper_PositionEnd;
        }

        private void Wrapper_PositionEnd(object? sender, EventArgs e)
        {
            TaskComplete();
        }

        private void Wrapper_Position(object? sender, PositionsEventArgs e)
        {
            positions.Add((e.Account, e.AvgCost, e.Contract, e.Pos));
        }

        public async Task<List<(string Account, double AvgCost, Contract Contract, double Pos)>> GetAsync()
        {
            StartRequest();
            Client.Socket.reqPositions();
            await Task();
            EndRequest();
            return positions;
        }
    }
}
