using FinanceModel.ErrorData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel.Decorators
{
    sealed class TimedLoggingCommand : CommandDecorator
    {
        private readonly IErrorData _store;
        private readonly string _scenario;

        public TimedLoggingCommand(IErrorData store, string scenarioName = null)
        {
            _store = store;
            _scenario = scenarioName ?? string.Empty;
        }

        public override void Execute()
        {
            var name = string.IsNullOrWhiteSpace(_scenario) ? inner?.GetType().Name ?? "Неизвестно" : _scenario;
            var sw = Stopwatch.StartNew();
            try
            {
                base.Execute();
                sw.Stop();
                _store.Add($"[{DateTime.UtcNow:O}] {name} Execute OK in {sw.Elapsed.TotalMilliseconds:F1} ms");
            }
            catch (Exception ex)
            {
                sw.Stop();
                _store.Add($"[{DateTime.UtcNow:O}] {name} Execute ERROR in {sw.Elapsed.TotalMilliseconds:F1} ms :: {ex.Message}");
                throw;
            }
        }

        public override void Undo()
        {
            var name = string.IsNullOrWhiteSpace(_scenario) ? inner?.GetType().Name ?? "Неизвестно" : _scenario;
            var sw = Stopwatch.StartNew();
            try
            {
                base.Undo();
                sw.Stop();
                _store.Add($"[{DateTime.UtcNow:O}] {name} Undo OK in {sw.Elapsed.TotalMilliseconds:F1} ms");
            }
            catch (Exception ex)
            {
                sw.Stop();
                _store.Add($"[{DateTime.UtcNow:O}] {name} Undo ERROR in {sw.Elapsed.TotalMilliseconds:F1} ms :: {ex.Message}");
                throw;
            }
        }
    }
}
