using FinanceModel.domain.entities;
using FinanceModel.import;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace FinanceModel.Import
{
    public sealed class JsonImportStrategy : ImportStrategy
    {
        private string _path = "";
        private readonly Dictionary<string, List<Dictionary<string, string>>> _cache = new(StringComparer.OrdinalIgnoreCase);

        public override void SetFilePath(string path) => _path = (path ?? "").Trim().Trim('"');

        public override List<BankAccount> ParseAccounts()
        {
            var rows = ReadJsonOnce();
            var list = new List<BankAccount>();
            foreach (var r in rows)
            {
                if (!Eq(Get(r, "entity"), "account")) continue;

                var id = Get(r, "id");
                var name = Get(r, "name");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name)) continue;

                if (!TryParseInt(Get(r, "balance"), out var balance)) continue;

                list.Add(new BankAccount(name, id, balance));
            }
            return list;
        }

        public override List<Category> ParseCategories()
        {
            var rows = ReadJsonOnce();
            var list = new List<Category>();
            foreach (var r in rows)
            {
                if (!Eq(Get(r, "entity"), "category")) continue;

                var id = Get(r, "id");
                var name = Get(r, "name");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name)) continue;

               
                if (!TryParseType(Get(r, "type"), out var type)) continue;

                list.Add(new Category(id, type, name));
            }
            return list;
        }

        public override List<Operation> ParseOperations()
        {
            var rows = ReadJsonOnce();
            var list = new List<Operation>();
            foreach (var r in rows)
            {
                if (!Eq(Get(r, "entity"), "operation")) continue;

                var id = Get(r, "id");
                var accId = Get(r, "bank_account_id");
                var catId = Get(r, "category_id");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(accId) || string.IsNullOrWhiteSpace(catId)) continue;

              
                if (!TryParseType(Get(r, "type"), out var type)) continue;
                if (!TryParseInt(Get(r, "amount"), out var amount)) continue;
                if (!TryParseDate(Get(r, "date"), out var date)) continue;

                var desc = TrimOrNull(Get(r, "description"));

           
                var acc = new BankAccount(accId, accId, 0);
                var cat = new Category(catId, type, catId);

                list.Add(new Operation(id, type, acc, cat, amount, date, desc));
            }
            return list;
        }

        
        private List<Dictionary<string, string>> ReadJsonOnce()
        {
            if (_cache.TryGetValue(_path, out var cached)) return cached;

            if (!File.Exists(_path)) { _cache[_path] = new(); return _cache[_path]; }

            using var doc = JsonDocument.Parse(File.ReadAllText(_path));
            if (doc.RootElement.ValueKind != JsonValueKind.Array) { _cache[_path] = new(); return _cache[_path]; }

            var rows = new List<Dictionary<string, string>>();
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                if (el.ValueKind != JsonValueKind.Object) continue;
                var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var p in el.EnumerateObject())
                {
                    d[p.Name] = p.Value.ValueKind switch
                    {
                        JsonValueKind.String => p.Value.GetString() ?? "",
                        JsonValueKind.Number => p.Value.ToString(),
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        JsonValueKind.Null => "",
                        _ => p.Value.ToString()
                    };
                }
                rows.Add(d);
            }

            _cache[_path] = rows;
            return rows;
        }

        private static bool Eq(string s, string t) => string.Equals(s?.Trim(), t, StringComparison.OrdinalIgnoreCase);
        private static string Get(Dictionary<string, string> r, string k) => r.TryGetValue(k, out var v) ? v?.Trim() ?? "" : "";
        private static string? TrimOrNull(string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static bool TryParseInt(string s, out int value) =>
            int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        private static bool TryParseType(string s, out EntryType type)
        {
            s = (s ?? "").Trim().ToLowerInvariant();
            if (s == "доход") { type = EntryType.Income; return true; }
            if (s == "расход") { type = EntryType.Expense; return true; }
            type = default; return false;
        }

        private static bool TryParseDate(string s, out DateTime dt)
        {
            var fmts = new[] { "dd yyyy MM", "d yyyy M" };
            return DateTime.TryParseExact(s, fmts, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        }
    }
}
