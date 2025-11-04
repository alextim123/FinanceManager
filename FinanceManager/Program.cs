using Microsoft.Extensions.DependencyInjection;


using FinanceModel.repository;
using FinanceModel.repository.interfaces;


using FinanceModel.ConsoleCreator;


using FinanceModel.Domain.Fabric.Interfaces;
using FinanceModel.domain.Fabric;


using FinanceModel.Import;
using FinanceModel.ConsoleMenus;
using FinanceModel.Export;


using FinanceModel.Analytics;
using FinanceModel.ErrorData;


using FinanceModel.App;


using FinanceModel.FinanceController.Observe;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IRepoOperation, RepoOperationMemory>();
        services.AddSingleton<IRepoCategory, RepoCategoryMemory>();
        services.AddSingleton<IRepoBankAccount, RepoBankAccountMemory>();

        services.AddSingleton<FinanceManager>();

        services.AddSingleton<IBankAccountFactory, BankAccountFactory>();
        services.AddSingleton<ICategoryFactory, CategoryFactory>();
        services.AddSingleton<IOperationFactory, OperationFactory>();

        services.AddSingleton<FinanceModel.ErrorData.IErrorData, MemoryErrorData>();

        services.AddSingleton<ImportFacade>();
        services.AddSingleton<ExportFacade>();
        services.AddSingleton<AnalyticsFacade>();

        services.AddSingleton<ConsoleImportSubmenu>();
        services.AddSingleton<ExportSubMenu>();
        services.AddSingleton<ConsoleAnalyticsMenu>();
        services.AddSingleton<FinanceModel.ConsoleCreator.ConsoleCreator>();

        services.AddSingleton<ConsoleMainMenu>();

        services.AddSingleton<AuditObserver>();
        services.AddSingleton<BalanceObserver>(sp =>
        {
            var mgr = sp.GetRequiredService<FinanceManager>();
            return new BalanceObserver(mgr);
        });

        var provider = services.BuildServiceProvider();

        var mgr = provider.GetRequiredService<FinanceManager>();
        var audit = provider.GetRequiredService<AuditObserver>();
        var balance = provider.GetRequiredService<BalanceObserver>();

        mgr.OperationAdded += audit.OnAdded;
        mgr.OperationRemoved += audit.OnRemoved;
        mgr.OperationReplaced += audit.OnReplaced;

        mgr.OperationAdded += balance.OnAdded;
        mgr.OperationRemoved += balance.OnRemoved;
        mgr.OperationReplaced += balance.OnReplaced;

        var mainMenu = provider.GetRequiredService<ConsoleMainMenu>();
        mainMenu.Run();
    }
}
