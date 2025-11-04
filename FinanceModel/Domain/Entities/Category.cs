using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.domain.entities
{
    public class Category
    {
        public string ID;
        public EntryType Type;
        public string Name;
        public Category(string id, EntryType type, string name)
        {
            ID = id;
            Type = type;
            Name = name;
        }
    }
}
