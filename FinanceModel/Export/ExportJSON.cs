namespace FinanceModel.Export
{
    using FinanceModel.domain.entities;
    using FinanceModel.Export.Intefaces;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Unicode;

    public sealed class JsonExportVisitor : IExportVisitor
    {
        private readonly List<Dictionary<string, object?>> _rows = new();

        public void Visit(List<BankAccount> accounts)
        {
            if (accounts is null) return;
            foreach (var acc in accounts)
            {
                _rows.Add(new()
                {
                    ["entity"] = "account",
                    ["id"] = acc.ID,
                    ["name"] = acc.Name,
                    ["balance"] = acc.Balance
                });
            }
        }

        public void Visit(List<Category> categories)
        {
            if (categories is null) return;
            foreach (var cat in categories)
            {
                _rows.Add(new()
                {
                    ["entity"] = "category",
                    ["id"] = cat.ID,
                    ["name"] = cat.Name,
                    ["type"] = cat.Type == EntryType.Income ? "Доход" : "Расход"
                });
            }
        }

        public void Visit(List<Operation> operations)
        {
            if (operations is null) return;

            foreach (var op in operations)
            {
                _rows.Add(new()
                {
                   
                    ["id"] = op.ID,
                    ["type"] = (op.Type == EntryType.Income) ? "доход" : "расход",
                    ["account"] = new Dictionary<string, object?>
                    {
                        ["id"] = op.Account?.ID ?? "",
                        ["name"] = op.Account?.Name ?? "",
                        ["balance"] = op.Account?.Balance ?? 0
                    },
                    ["amount"] = op.Amount,
                    ["date"] = op.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ["description"] = string.IsNullOrWhiteSpace(op.Description) ? "" : op.Description,
                    ["category"] = new Dictionary<string, object?>
                    {
                        ["id"] = op.Category?.ID ?? "",
                        ["type"] = ((op.Category?.Type ?? op.Type) == EntryType.Income) ? "доход" : "расход",
                        ["name"] = op.Category?.Name ?? ""
                    }
                });
            }
        }

        public string GetText()
        {
            var opts = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) 
            };
            return JsonSerializer.Serialize(_rows, opts);
        }

        public void Save(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, GetText(), new UTF8Encoding(false));
        }
    }
}
