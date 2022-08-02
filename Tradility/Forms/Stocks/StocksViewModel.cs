using PubSub;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Tradility.Common.Extensions;
using Tradility.Common.Localization;
using Tradility.Common.Utils;
using Tradility.Data;
using Tradility.Data.Extensions;
using Tradility.Data.Models;
using Tradility.Data.Repositories;
using Tradility.Data.Services;
using Tradility.Extensions;
using Tradility.Forms.Positions;
using Tradility.Forms.SideBar;
using Tradility.Messages;
using Tradility.UI.Utils;
using Tradility.ViewModels;

namespace Tradility.Forms.Stocks
{
    public class StocksViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<EventArgs<string>>? ScriptExecuted;

        public SideBarViewModel SideBarViewModel { get; set; }

        //public IEnumerable<TableViewModel> Repositories => SideBarViewModel.Repositories.Where(x => x.VisibleInStocks);
        //public TableViewModel SelectedRepository { get; set; }

        public ICommand LoadCommand { get; set; }

        public Uri URI { get; set; }

        private readonly ExchangeService exchangeService;
        private readonly PositionRepository positionRepository;

        public StocksViewModel(SideBarViewModel sideBarViewModel, ExchangeService exchangeService, PositionRepository positionRepository)
        {
            SideBarViewModel = sideBarViewModel;
            SideBarViewModel.IsOnlyStocksRepositories = true;
            SideBarViewModel.PositionsLoaded += SideBarViewModel_PositionsLoaded;
            LoadCommand = new DelegateCommand(Load);
            //SelectedRepository = SideBarViewModel.Repositories[0];
            URI = new Uri("http://localhost:9999");

            this.exchangeService = exchangeService;
            this.positionRepository = positionRepository;

            Hub.Default.Subscribe<TradeSelectedMessage>(TradeSelected);
            Hub.Default.Subscribe<PositionSelectedMessage>(PositionSelected);
            Hub.Default.Subscribe<DividendSelectedMessage>(DividendSelected);
        }

        private void SideBarViewModel_PositionsLoaded(object? sender, EventArgs e)
        {
            var positionsRepo = SideBarViewModel.Repositories.First(x => x.Name == Res.Positions);
            if (SideBarViewModel.SelectedRepository != positionsRepo)
                SideBarViewModel.SelectedRepository = positionsRepo;

            if(positionsRepo.MiniViewModel is PositionsMiniViewModel mvm)
            {
                mvm.SelectedItem = mvm.Items.FirstOrDefault();
            }
        }

        private void Load()
        {
            //SideBarViewModel.SelectedRepository = SelectedRepository;
            SideBarViewModel.LoadCommand.Execute(this);
        }

        private void DividendSelected(DividendSelectedMessage message)
        {
            var dividendModel = message.SelectedDividend;
            if (dividendModel == null)
                return;

            var exchangeModel = new ExchangeService.ExchangeArgs
            {
                Date = dividendModel.Date,
                From = dividendModel.Currency,
                To = Currency.USD,
                Value = dividendModel.Amount
            };
            var symbol = $"{dividendModel.TickerSymbol}";
            //var symbol = $"{tradeModel.Symbol}";
            var usdPrice = exchangeService
                .Exchange(exchangeModel);
            var time = dividendModel
                .Date.ToDateTimeOffset()
                .ToUnixTimeSeconds();

            Clean();
            SetSymbol(symbol);
            if (exchangeService.IsLoaded)
                DrawVerticalLine(usdPrice, time, $"{dividendModel.Date.ToString("dd/MM/yyyy")} {dividendModel.AmountExchanged}", "left", "#000000", true, false);
            //ExecuteScript($"addTrade({usdPrice}, {time}, 'Transaction Price', '#1f7299')");
            Save();
        }

        private void TradeSelected(TradeSelectedMessage message)
        {
            var tradeModel = message.SelectedTrade;
            if (tradeModel == null)
                return;

            var exchangeModel = new ExchangeService.ExchangeArgs
            {
                Date = tradeModel.DateTime.ToDateOnly(),
                From = tradeModel.Currency.ToCurrency(),
                To = Currency.USD,
                Value = tradeModel.TPrice
            };
            var symbol = $"{tradeModel.Symbol}:{tradeModel.Exchange}";
            //var symbol = $"{tradeModel.Symbol}";
            var usdPrice = exchangeService
                .Exchange(exchangeModel);
            var time = tradeModel
                .DateTime
                .ToUnixTimeSeconds();

            Clean();
            SetSymbol(symbol);
            if(exchangeService.IsLoaded)
                DrawHorizontalLine(usdPrice, time, R.TransactionPrice, "#1f7299");
            //ExecuteScript($"addTrade({usdPrice}, {time}, 'Transaction Price', '#1f7299')");
            Save();
        }

        private void PositionSelected(PositionSelectedMessage message)
        {
            var selectedPosition = message?.SelectedPosition;
            if (selectedPosition == null)
                return;

            var positions = positionRepository.Items
                .Where(x => x.Contract.Symbol == selectedPosition.Contract.Symbol)
                .ToList();

            var positionsOPTs = positions
                .Where(x => x.Contract.SecType == "OPT")
                .ToList();          

            Clean();
            SetSymbol(selectedPosition.Contract.Symbol);
            if(positionsOPTs.Count > 0)
                DrawIronCondor(positionsOPTs);

            var positionSTK = positions.FirstOrDefault(x => x.Contract.SecType == "STK");
            if(positionSTK != null)
            {
                var positionSTKCurrency = positionSTK.Contract.Currency.ToCurrency();
                decimal avgPrice = -1;
                if(exchangeService.IsLoaded && positionSTKCurrency != Currency.USD)
                {
                    avgPrice = exchangeService.Exchange(new ExchangeService.ExchangeArgs
                    {
                        Date = Now.DateOnly,
                        From = positionSTK.Contract.Currency.ToCurrency(),
                        To = Currency.USD,
                        Value = Convert.ToDecimal(positionSTK.AvgCost)
                    });
                }
                else
                {
                    avgPrice = Convert.ToDecimal(positionSTK.AvgCost);
                }
                

                if(avgPrice > -1)
                    DrawHorizontalLine(avgPrice, 0, R.AveragePrice, "#1f7299");
            }          

            Save();
        }

        private void DrawIronCondor(List<PositionModel> positions)
        {
            foreach (var item in positions)
            {
                var price = item.Contract.Strike;
                var pos = item.Pos;
                var dateNormal = item.Contract.LastTradeDateOrContractMonth.TWSDateTimeToDateTimeOffset();
                var date = dateNormal.ToUTCUnixSeconds();
                var right = item.Contract.Right == "P" ? R.PUT : R.CALL;
                var text = $"{right} {price}, POS {pos}";
                var color = pos < 0 ? "#EF5350" : "#26A69A";
                var script = $"addOPT({price}, {date}, '{text}', '{color}')";

                ExecuteScript(script);
            }

            var expDate = positions
                .First()
                .Contract.LastTradeDateOrContractMonth
                .TWSDateTimeToDateTimeOffset();

            var expTime = expDate
                .AddDays(1)
                .ToUnixTimeSeconds()
                .ToJS();

            var expText = expDate
                .ToString("M", TLocalization.Instance.Language.ToCultureInfo())
                .ToJS();

            var expAvgPrice = positions.Average(x => x.Contract.Strike).ToJS();

            ExecuteScript($"addOPTExp({expAvgPrice}, {expTime}, {expText})");
        }

        private void DrawHorizontalLine(decimal price, long time, string text, string color)
        {
            var priceJS = price.ToJS();
            var textJS = text.ToJS();
            var colorJS = color.ToJS();
            var timeJS = time.ToJS();

            ExecuteScript($"addHLine({priceJS}, {timeJS}, {textJS}, {colorJS})");
        }

        private void DrawVerticalLine(decimal price, long time, string text, string textAlign, string color, bool extLeft, bool extRight)
        {
            var priceJS = price.ToJS();
            var textJS = text.ToJS();
            var textAlignJS = textAlign.ToJS();
            var timeJS = time.ToJS();
            var colorJS = color.ToJS();
            var extLeftJS = extLeft.ToString().ToLower();
            var extRightJS = extRight.ToString().ToLower();

            ExecuteScript($"addVLine({priceJS}, {timeJS}, {textJS}, {textAlignJS}, {colorJS}, {extLeftJS}, {extRightJS})");
        }

        private void SetSymbol(string symbol)
        {
            var symbolJS = symbol.ToJS();
            ExecuteScript($"setSymbol({symbolJS})");
        }

        private void Clean()
        {
            ExecuteScript("cleanLines()");
        }

        private void Save()
        {
            ExecuteScript("save()");
        }

        private void ExecuteScript(string script)
        {
            ScriptExecuted?.Invoke(this, new EventArgs<string>(script));
        }
    }
}
