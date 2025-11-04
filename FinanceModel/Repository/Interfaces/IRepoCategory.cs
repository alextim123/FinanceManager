using FinanceModel.domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.repository.interfaces
{
    public interface IRepoCategory
    {
        public void Save(Category cat);
        public void Delete(Category cat);
        public void Update(Category cat);
        public void DeleteAll();
        public void UpdateAll(List<Category> cats);
        public List<Category> GetAllCategory();
    }
}
