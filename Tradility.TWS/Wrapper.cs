using System;
using Tradility.TWS.API;

namespace Tradility.TWS
{
    public class Wrapper : DefaultEWrapper
    {
        public event EventHandler<NextValidIdEventArgs>? NextValidId;
        public event EventHandler<ManagedAccountsEventArgs>? ManagedAccounts;

        public override void nextValidId(int orderId) => NextValidId?.Invoke(this, new(orderId));
        public override void managedAccounts(string accountsList) => ManagedAccounts?.Invoke(this, new(accountsList));

        // Connection
        public event EventHandler? ConnectAck;
        public event EventHandler? ConnectionClosed;

        public override void connectAck() => ConnectAck?.Invoke(this, EventArgs.Empty);
        public override void connectionClosed() => ConnectionClosed?.Invoke(this, EventArgs.Empty);

        // Error
        public event EventHandler<ErrorEventArgs>? Error;

        public override void error(string str) => Error?.Invoke(this, new(0, 0, str, null));
        public override void error(Exception e) => Error?.Invoke(this, new(0, 0, e.Message, e));
        public override void error(int id, int errorCode, string errorMsg) => Error?.Invoke(this, new(id, errorCode, errorMsg, null));

        // HistoricalData
        public event EventHandler<HistoricalDataEventArgs>? HistoricalData;
        public event EventHandler<HistoricalDataEndEventArgs>? HistoricalDataEnd;

        public override void historicalData(int reqId, Bar bar) => HistoricalData?.Invoke(this, new(reqId, bar));
        public override void historicalDataEnd(int reqId, string start, string end) => HistoricalDataEnd?.Invoke(this, new(reqId, start, end));

        // Positions
        public event EventHandler<PositionsEventArgs>? Position;
        public event EventHandler? PositionEnd;

        public override void position(string account, Contract contract, double pos, double avgCost) => Position?.Invoke(this, new(account, contract, pos, avgCost));
        public override void positionEnd() => PositionEnd?.Invoke(this, EventArgs.Empty);

        // MatchingSymbols
        public event EventHandler<SymbolSamplesEventArgs>? SymbolSamples;
        public override void symbolSamples(int reqId, ContractDescription[] contractDescriptions) => SymbolSamples?.Invoke(this, new(reqId, contractDescriptions));

        // ContractDetails
        public event EventHandler<ContractDetailsEventArgs>? ContractDetails;
        public event EventHandler<ContractDetailsEndEventArgs>? ContractDetailsEnd;

        public override void contractDetails(int reqId, ContractDetails contractDetails) => ContractDetails?.Invoke(this, new(reqId, contractDetails));
        public override void contractDetailsEnd(int reqId) => ContractDetailsEnd?.Invoke(this, new(reqId));
    }

    public record class NextValidIdEventArgs(int NextValidId);
    public record class ManagedAccountsEventArgs(string AccountsList);
    public record class ErrorEventArgs(int Id, int ErrorCode, string ErrorMsg, Exception? Exception);
    public record class HistoricalDataEventArgs(int RequestId, Bar Bar);
    public record class HistoricalDataEndEventArgs(int RequestId, string Start, string End);
    public record class PositionsEventArgs(string Account, Contract Contract, double Pos, double AvgCost);
    public record class SymbolSamplesEventArgs(int ReqId, ContractDescription[] ContractDescriptions);
    public record class ContractDetailsEventArgs(int ReqId, ContractDetails ContractDetails);
    public record class ContractDetailsEndEventArgs(int ReqId);
}
