using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Tradility.Common.Exceptions;
using Tradility.Common.Localization;
using Tradility.Data.Models;
using Tradility.Data.Repositories.Abstractions;

namespace Tradility.Data.Repositories
{
    public abstract class CSVRepository<T> : BaseRepository<T>, ICSVRepository where T : BaseModel
    {
        protected abstract string Key { get; }
        private static string DataKey => TLocalization.Instance.Language switch
        {
            Language.English => "Data",
            Language.German => "Data",
            _ => throw new NotImplementedException()
        };

        public CSVRepository()
        {
            Items = new();
        }

        public virtual async Task LoadAsync(string filePath)
        {
            var items = await Task.Run(() => Load(filePath));
            if (items.Count == 0)
                throw new TException("Empty or incorrect file");

            Items = items;
        }

        protected virtual ObservableCollection<T> Load(string filePath)
        {
            var R = new ObservableCollection<T>();

            using var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields != null && fields?.ElementAtOrDefault(0) == Key && fields?.ElementAtOrDefault(1) == DataKey)
                {
                    var itemFields = fields.AsSpan(2);
                    if (itemFields != null)
                    {
                        var item = Parse(itemFields);
                        if(item is not null)
                            R.Add(item);
                    }
                }
            }

            return R;
        }

        protected abstract T? Parse(Span<string> items);

        protected static decimal ParseDecimal(string? content)
        {
            if (decimal.TryParse(content?.Replace(",","."), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
                return value;

            return default;
        }

        protected static DateTimeOffset ParseDate(string? content)
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            if (DateTimeOffset.TryParseExact(content, "yyyy-MM-dd, HH:mm:ss", cultureInfo, System.Globalization.DateTimeStyles.AssumeUniversal, out var date))
                return date;

            return default;
        }

        protected static DateOnly ParseDateOnly(string? content)
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            if (DateOnly.TryParseExact(content, "yyyy-MM-dd, HH:mm:ss", cultureInfo, System.Globalization.DateTimeStyles.AssumeUniversal, out var date))
                return date;

            return default;
        }
    }
}
