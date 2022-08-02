using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Tradility.Data.Models;
using Tradility.Data.Repositories;
using Tradility.Data.Repositories.Abstractions;

namespace Tradility.Forms
{
    public abstract class BaseRepositoryViewModel<T> : IRepositoryViewModel, INotifyPropertyChanged where T : BaseModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<T>? Items { get; protected set; }

        public IColumns Columns { get; set; }

        private readonly BaseRepository<T> repository;
        public IRepository Repository => repository;

        public BaseRepositoryViewModel(BaseRepository<T> repository, IColumns columns)
        {
            Columns = columns;// ?? throw new ArgumentNullException(nameof(columns));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

            if (this.repository.Items != null)
                Items = new ObservableCollection<T>(this.repository.Items);

            this.repository.PropertyChanged += Repository_PropertyChanged;
        }

        private void Repository_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == repository && repository.Items != null && e.PropertyName == nameof(repository.Items))
            {
                Items = new ObservableCollection<T>(repository.Items);
            }
        }

        public bool WriteToCSV(string filename, CultureInfo cultureInfo)
        {
            if (Items == null)
                return false;
            using var writer = new StreamWriter(filename, false, Encoding.UTF8);
            writer.WriteAsync('\uFEFF');//https://stackoverflow.com/a/60115275

            var configuration = new CsvConfiguration(cultureInfo)
            {
                Delimiter = ","
            };
            using var csv = new CsvWriter(writer, configuration);

            var columns = Columns.Columns
                .Where(x => x.IsChecked)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToList();

            foreach (var item in columns)
            {
                csv.WriteField(item.Name);
            }
            csv.NextRecord();

            foreach (var item in Items)
            {
                foreach (var column in columns)
                {
                    var value = column.Get(item);
                    if(value is DateTimeOffset dto)
                    {
                        value = dto.DateTime;
                    }
                    csv.WriteField(value);
                }
                csv.NextRecord();
            }

            return true;
        }
    }
}
