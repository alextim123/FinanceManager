using FinanceModel.domain.entities;
using FinanceModel.repository.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.repository
{
    public class RepoCategoryMemory: IRepoCategory
    {
        Dictionary<string, Category> _categoryData = new();
        public void Save(Category cat)
        {
            _categoryData.Add(cat.ID, cat);
        }
        public void Delete(Category cat)
        { 
            _categoryData.Remove(cat.ID);
        }
        public void Update(Category cat)
        {
               _categoryData[cat.ID] = cat;
        }
        public void DeleteAll() 
        {
            _categoryData.Clear();
        }
        public void UpdateAll(List<Category> cats)
        {
            Dictionary<string, Category> result = new();
            for (int i = 0; i < cats.Count; i++)
            {
                result.Add(cats[i].ID, cats[i]);
            }
            _categoryData = result;
        }
        public List<Category> GetAllCategory()
        {
            return _categoryData.Values.ToList();
        }
    }
}
