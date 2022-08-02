using PropertyChanged;
using PubSub;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Tradility.Data;
using Tradility.Data.Extensions;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Messages;

namespace Tradility.Forms
{
    public class ColumnsBase : IColumns, ICurrency, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private Currency currency;
        public Currency Currency
        {
            get => currency; 
            set
            {
                if(Columns != null)
                {
                    foreach (var column in Columns)
                    {
                        column.Currency = value;
                    }
                }               

                currency = value;
            }
        }

        private ObservableCollection<Column> _columns;
        public ObservableCollection<Column> Columns { get; protected set; }
        public bool? AllChecked { get => GetAllChecked(); set => SetAllChecked(value); }

        protected void InitOrders()
        {
            var i = 0;
            foreach (var item in Columns)
            {
                item.Order = i;
                //item.IsChecked = false;
            }
        }

        private void SetAllChecked(bool? value)
        {
            foreach (var item in Columns)
                item.IsChecked = value == true;

            PropertyChanged?.Invoke(this, new(nameof(AllChecked)));
        }

        private bool? GetAllChecked()
        {
            var a = Columns.All(x => x.IsChecked);
            var b = Columns.All(x => !x.IsChecked);

            if (a || b)
                if (a)
                    return a;
                else
                    return !b;
            else
                return null;
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(Columns) && Columns != null)
            {
                if (_columns != null)
                    foreach (var item in _columns)
                        item.PropertyChanged -= Item_PropertyChanged;

                foreach (var item in Columns)
                    item.PropertyChanged += Item_PropertyChanged;

                _columns = Columns;
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new(nameof(AllChecked)));
        }
    }

    public interface IColumns
    {
        public ObservableCollection<Column> Columns { get; }
    }

    public abstract class Column : ICurrency, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual string Name { get; } = "";
        [AlsoNotifyFor(nameof(Name))]
        public Currency Currency { get; set; }
        public bool IsChecked { get; set; } = true;
        public int Order { get; set; }

        public abstract object Get(object model);

        public Column()
        {
            Currency = CurrencyExtensions.Default;
            Hub.Default.Subscribe<CurrencyChangedMessage>(CurrencyChanged);
        }

        private void CurrencyChanged(CurrencyChangedMessage message)
        {
            Currency = message.Currency;
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Column<ModelType, ReturnType> : Column
    {
        private readonly Func<ModelType, ReturnType> valueGetter;
        private readonly Func<Column, string> nameGetter;
        private readonly string name;

        public override string Name => nameGetter == null ? name : nameGetter.Invoke(this);

        public Column(string name, Func<ModelType, ReturnType> valueGetter)
        {
            this.name = name;
            this.valueGetter = valueGetter;
        }

        public Column(Func<Column, string> nameGetter, Func<ModelType, ReturnType> valueGetter)
        {
            this.nameGetter = nameGetter;
            this.valueGetter = valueGetter;
        }

        public override object Get(object model)
        {
            if (valueGetter == null)
                return null;

            return valueGetter.Invoke((ModelType)model);
        }
    }
}
