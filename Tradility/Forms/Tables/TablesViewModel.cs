using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tradility.Common.Exceptions;
using Tradility.Data;
using Tradility.Data.Repositories;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Data.Services;
using Tradility.Forms.CashReports;
using Tradility.Forms.Positions;
using Tradility.Forms.SideBar;
using Tradility.Forms.Trades;
using Tradility.Messages;
using Tradility.UI;
using Tradility.UI.Services;
using Tradility.UI.Utils;
using Tradility.ViewModels;

namespace Tradility.Forms.Tables
{
    public class TablesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public SideBarViewModel SideBarViewModel { get; set; }
        //public TableViewModel SelectedRepository { get; set; }
        public bool IsICurrencyRepository => SideBarViewModel.SelectedRepository?.Repository is ICurrency;

        public bool IsLoading { get; set; }
        public bool IsExporting { get; set; }
              
        public int ExportType { get; set; } = 0;
        public ICommand ExportCommand { get; set; }
        public ICommand LoadCommand { get; set; }

        public CustomDropHandler DropHandler { get; set; }

        public TablesViewModel(SideBarViewModel sideBarViewModel)
        {
            SideBarViewModel = sideBarViewModel;
            SideBarViewModel.PositionsLoaded += OnPositionsLoaded;

            //SelectedRepository = SideBarViewModel.Repositories[0];
            ExportCommand = new DelegateCommand(ExportAsync);
            LoadCommand = new DelegateCommand(Load);

            DropHandler = new();
        }       

        private void Load()
        {
            //SideBarViewModel.SelectedRepository = SelectedRepository;
            SideBarViewModel.LoadCommand.Execute(this);
        }

        private void OnPositionsLoaded(object? sender, EventArgs e)
        {
            var positionsRepo = SideBarViewModel.Repositories.First(x=>x.Name == Res.Positions);
            if (SideBarViewModel.SelectedRepository != positionsRepo)
                SideBarViewModel.SelectedRepository = positionsRepo;
        }

        private async void ExportAsync()
        {
            if(ExportType == 2)
            {
                MessageBox.Show(R.Error_Export_Excel_Unavailable);
                return;
            }
            IsExporting = true;

            await SafeCaller.TryAsync(() =>
            {
                SaveFileDialog sfd = new()
                {
                    Filter = "CSV|*.csv"
                };

                if (sfd.ShowDialog() == true)
                {
                    SideBarViewModel.SelectedRepository.ViewModel.WriteToCSV(sfd.FileName, new CultureInfo(ExportType == 0 ? "en-US" : "de-DE"));
                }

                return Task.CompletedTask;
            });

            IsExporting = false;
        }

        public class CustomDropHandler : DefaultDropHandler
        {
            public override void Drop(IDropInfo dropInfo)
            {
                base.Drop(dropInfo);
                var i = 0;
                foreach (var item in dropInfo.TargetCollection)
                {
                    if (item is Column column)
                        column.Order = i++;
                }
            }
        }
    }
}
