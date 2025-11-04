
namespace FinanceModel.Export
{
    using FinanceModel.domain.entities;
    using FinanceModel.Export.Intefaces;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public sealed class CsvExportVisitor : IExportVisitor
    {
        private readonly StringBuilder _sb = new();
        private bool _header;

        private void EnsureHeader()
        {
            if (_header) return;
            _sb.AppendLine("entity,id,name,balance,type,bank_account_id,category_id,amount,date,description");
            _header = true;
        }

        public void Visit(List<BankAccount> accounts)
        {
            if (accounts is null || accounts.Count == 0) return;
            EnsureHeader();
            foreach (var acc in accounts)
            {
                _sb.Append("account,")
                   .Append(E(acc.ID)).Append(',')
                   .Append(E(acc.Name)).Append(',')
                   .Append(acc.Balance.ToString(CultureInfo.InvariantCulture))
                   .Append(",,,,,,\n");
            }
        }

        public void Visit(List<Category> categories)
        {
            if (categories is null || categories.Count == 0) return;
            EnsureHeader();
            foreach (var cat in categories)
            {
                var type = cat.Type == EntryType.Income ? "Доход" : "Расход";
                _sb.Append("category,")
                   .Append(E(cat.ID)).Append(',')
                   .Append(E(cat.Name)).Append(',')
                   .Append(',')                
                   .Append(E(type))            
                   .Append(",,,,,\n");
            }
        }

        public void Visit(List<Operation> operations)
        {
            if (operations is null || operations.Count == 0) return;
            EnsureHeader();
            foreach (var op in operations)
            {
                _sb.Append("operation,")
                   .Append(E(op.ID)).Append(',')                   
                   .Append(',')                                    
                   .Append(',')                                     
                   .Append(',')                                      
                   .Append(E(op.Account?.ID ?? "")).Append(',')     
                   .Append(E(op.Category?.ID ?? "")).Append(',')     
                   .Append(op.Amount.ToString(CultureInfo.InvariantCulture)).Append(',')
                   .Append(E(op.Date.ToString("dd yyyy MM", CultureInfo.InvariantCulture))).Append(',') 
                   .Append(E(op.Description ?? ""))                
                   .Append('\n');
            }
        }


        public string GetText() => _sb.ToString();

        public void Save(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, GetText(), new UTF8Encoding(false));
        }

        private static string E(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var need = s.Contains(',') || s.Contains('"') || s.Contains('\n');
            return need ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
        }
    }
}
