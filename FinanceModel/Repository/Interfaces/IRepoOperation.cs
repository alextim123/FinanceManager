using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace FinanceModel.repository.interfaces
{
    public interface IRepoOperation
    {
        public void Save(Operation oper);
        public void Delete(Operation oper);
        public void Update(Operation oper);
        public void DeleteAll();
        public void UpdateAll(List<Operation> opers);
        public List<Operation> GetAllOperation();
    }
}
