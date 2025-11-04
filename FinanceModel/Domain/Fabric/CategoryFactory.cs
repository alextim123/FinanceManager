using FinanceModel.domain.entities;
using FinanceModel.Domain.Fabric.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.domain.Fabric
{
    public sealed class CategoryFactory : ICategoryFactory
    {
        public Category Create(string id, string name, EntryType type)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID категории пуст.");
            if (id.Length > 200)
                throw new ValidationException("ID категории слишком длинный (макс. 200).");

            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Название категории пусто.");
            if (name.Length > 200)
                throw new ValidationException("Название категории слишком длинное (макс. 200).");

           return new Category(id, type, name);
        }
    }
}
