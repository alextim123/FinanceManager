using FinanceModel.domain.entities;
using FinanceModel.Domain.Fabric.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Linq;

namespace FinanceModel.domain.Fabric
{
    public sealed class OperationFactory : IOperationFactory
    {
        //Разрешаем несколько вариантов: "05 2025 11", "5-2025-11", "05.2025.11", "05/2025/11"
        private static readonly string[] DateFormats = {
        "dd yyyy MM","d yyyy M",
        "dd-yyyy-MM","d-yyyy-M",
        "dd.yyyy.MM","d.yyyy.M",
        "dd/yyyy/MM","d/yyyy/M"
    };

        public Operation Create(string id, EntryType type, BankAccount account, Category category, int amount, string dateDym, string? description)
        {
            //валидация данных
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID операции пуст.");
            if (id.Length > 200)
                throw new ValidationException("ID операции слишком длинный (макс. 200).");

            if (account is null)
                throw new ValidationException("Счёт (account) обязателен.");
            if (category is null)
                throw new ValidationException("Категория (category) обязательна.");
            if (string.IsNullOrWhiteSpace(account.ID))
                throw new ValidationException("У счёта должен быть непустой ID.");
            if (string.IsNullOrWhiteSpace(category.ID))
                throw new ValidationException("У категории должен быть непустой ID.");

            if (amount <= 0)
                throw new ValidationException("Сумма операции должна быть > 0.");

            if (category.Type != type)
                throw new ValidationException("Тип операции должен совпадать с типом категории (Income/Expense).");

            //парсинг даты "день год месяц"
            if (!DateTime.TryParseExact(dateDym?.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                throw new ValidationException("Дата должна быть в формате день год месяц, напр.: 05 2025 11.");
            }
            var date = new DateTime(dt.Year, dt.Month, dt.Day);

            return new Operation(id, type, account, category, amount, date, string.IsNullOrWhiteSpace(description) ? null : description) { };
        }
    }
}
