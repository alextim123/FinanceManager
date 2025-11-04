using FinanceModel.domain.entities;
using FinanceModel.import;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FinanceModel.Import
{
    public sealed class CsvImportStrategy : ImportStrategy
    {
        private string _path = "";
        private readonly Dictionary<string, List<Dictionary<string, string>>> _cache = new(StringComparer.OrdinalIgnoreCase);

        public override void SetFilePath(string path) => _path = (path ?? "").Trim().Trim('"');

        public override List<BankAccount> ParseAccounts()
        {
            var rows = ReadCsvOnce();
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
            var rows = ReadCsvOnce();
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
            var rows = ReadCsvOnce();

           
            var categoryTypeById = new Dictionary<string, EntryType>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in rows)
            {
                if (!Eq(Get(r, "entity"), "category")) continue;
                var cid = Get(r, "id");
                if (string.IsNullOrWhiteSpace(cid)) continue;
                if (TryParseType(Get(r, "type"), out var ct))
                    categoryTypeById[cid] = ct;
            }

            var list = new List<Operation>();
            foreach (var r in rows)
            {
                if (!Eq(Get(r, "entity"), "operation")) continue;

                var id = Get(r, "id");
                var accId = Get(r, "bank_account_id");
                var catId = Get(r, "category_id");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(accId) || string.IsNullOrWhiteSpace(catId))
                    continue;

                
                EntryType type;
                if (!TryParseType(Get(r, "type"), out type))
                {
                    if (!categoryTypeById.TryGetValue(catId, out type))
                        continue; 
                }

                if (!TryParseInt(Get(r, "amount"), out var amount)) continue;
                if (!TryParseDate(Get(r, "date"), out var date)) continue;

                var desc = TrimOrNull(Get(r, "description"));

                var acc = new BankAccount(accId, accId, 0);
                var cat = new Category(catId, type, catId);

                list.Add(new Operation(id, type, acc, cat, amount, date, desc));
            }
            return list;
        }

     
        private List<Dictionary<string, string>> ReadCsvOnce()
        {
            if (_cache.TryGetValue(_path, out var cached)) return cached;

            if (!File.Exists(_path)) { _cache[_path] = new(); return _cache[_path]; }

            var text = File.ReadAllText(_path);
            if (text.Length > 0 && text[0] == '\uFEFF') text = text[1..]; 

            var lines = text.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) { _cache[_path] = new(); return _cache[_path]; }

            char sep = DetectSeparator(lines[0]);

            var header = SplitCsvLine(lines[0], sep);
            for (int i = 0; i < header.Length; i++) header[i] = header[i].Trim();

            var rows = new List<Dictionary<string, string>>();
            for (int i = 1; i < lines.Length; i++)
            {
                var cols = SplitCsvLine(lines[i], sep);
                var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int c = 0; c < header.Length; c++)
                    d[header[c]] = c < cols.Length ? (cols[c]?.Trim() ?? "") : "";
                rows.Add(d);
            }

            _cache[_path] = rows;
            return rows;
        }

        private static char DetectSeparator(string headerLine)
        {
            int commas = CountSep(headerLine, ',');
            int semis = CountSep(headerLine, ';');
            int tabs = CountSep(headerLine, '\t');

            if (semis >= commas && semis >= tabs) return ';';
            if (commas >= semis && commas >= tabs) return ',';
            return '\t';

            static int CountSep(string s, char ch)
            {
                int cnt = 0; bool q = false;
                for (int i = 0; i < s.Length; i++)
                {
                    var c = s[i];
                    if (c == '"')
                    {
                        if (q && i + 1 < s.Length && s[i + 1] == '"') { i++; continue; }
                        q = !q; continue;
                    }
                    if (!q && c == ch) cnt++;
                }
                return cnt;
            }
        }

        private static string[] SplitCsvLine(string line, char sep)
        {
            var res = new List<string>();
            bool q = false;
            var sb = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (ch == '"')
                {
                    if (q && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else { q = !q; }
                    continue;
                }
                if (ch == sep && !q) { res.Add(sb.ToString()); sb.Clear(); }
                else sb.Append(ch);
            }
            res.Add(sb.ToString());
            return res.ToArray();
        }

       
        private static bool Eq(string s, string t) => string.Equals(s?.Trim(), t, StringComparison.OrdinalIgnoreCase);
        private static string Get(Dictionary<string, string> row, string key) => row.TryGetValue(key, out var v) ? v?.Trim() ?? "" : "";
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
            var fmts = new[]
            {
                "dd yyyy MM","d yyyy M",
                "dd-yyyy-MM","d-yyyy-M",
                "dd.yyyy.MM","d.yyyy.M",
                "dd/yyyy/MM","d/yyyy/M"
            };
            return DateTime.TryParseExact(s?.Trim(), fmts, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        }
    }
}
