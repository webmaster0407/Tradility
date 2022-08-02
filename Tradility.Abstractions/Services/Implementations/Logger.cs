namespace Tradility.Abstractions.Services.Implementations
{
    internal class Logger : ILogger
    {
        public string Category { get; }

        public string? SubCategory { get; private set; }

        public string Message { get; }

        public string[]? Extras { get; private set; }

        public Logger(string message, string category)
        {
            Message = message;
            Category = category;
        }

        public ILogger WithExtras(params string[] extras)
        {
            if (Extras?.Any() == true)
            {
                var prevExtras = Extras.ToList();
                prevExtras.AddRange(extras);
                Extras = prevExtras.ToArray();
            }
            else
            {
                Extras = extras;
            }
            return this;
        }

        public ILogger WithSubCategory(string subCategory)
        {
            if (!string.IsNullOrWhiteSpace(SubCategory))
            {
                SubCategory = Path.Combine(SubCategory, subCategory);
            }
            else
            {
                SubCategory = subCategory;
            }
            return this;
        }

        public void Log()
        {
            var options = IOC.Instance.Resolve<ILoggerOptions>();
            if (!options.IsEnabled)
                return;

            var directory = Path.Combine(options.DirectoryPath, Category);

            if (!string.IsNullOrWhiteSpace(SubCategory))
                directory = Path.Combine(directory, SubCategory);

            Directory.CreateDirectory(directory);

            var date = DateTime.Now;
            

            var lines = new List<string>();
            lines.Add("");
            lines.Add(date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            lines.Add(Message);
            if(Extras?.Any() == true)
            {
                lines.Add("Extras:");
                lines.AddRange(Extras);
            }

            var fileName = $"{date:yyyy_MM_dd}.txt";

            var fullPath = Path.Combine(directory, fileName);

            File.AppendAllLines(fullPath, lines);
        }
    }
}
