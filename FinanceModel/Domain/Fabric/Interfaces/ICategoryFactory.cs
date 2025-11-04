using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Domain.Fabric.Interfaces
{
    public interface ICategoryFactory
    {
        Category Create(string id, string name, EntryType type);
    }
}
