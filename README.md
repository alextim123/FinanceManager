# FinanceManager

Консольное приложение для учёта финансов: счета, категории и операции, импорт/экспорт данных, аналитика (баланс за период, группировка по категориям). Внутри реализация классических паттернов GoF + DI, SOLID и GRASP.
# Функционал приложения для управления финансами
## Как запустить 
 Скачать зип архив и открыть файл FinanceManager.sln в Visual Studio. Затем нажать на зеленую стрелочку
## Основные возможности

### CRUD для доменных сущностей

**Счета (BankAccount):**
- Добавить новый счет
- Удалить счет по ID
- Заменить существующий счет
- Получить список всех счетов

**Категории (Category):**
- Добавить новую категорию
- Удалить категорию по ID
- Заменить существующую категорию
- Получить список всех категорий

**Операции (Operation):**
- Добавить новую операцию
- Удалить операцию по ID
- Заменить существующую операцию
- Получить список всех операций

*Все операции проходят через фабрики (валидация полей) и FinanceManager (проверки уникальности, события).*

### Импорт данных

**Поддерживаемые форматы:**
- CSV
- JSON (одним файлом)

**Особенности импорта:**
- Поддержка русских типов операций: «Доход» / «Расход»
- Поддержка форматов дат:
  - `dd yyyy MM` (например, 05 2025 11)
  - `d yyyy M` 
  - ISO `yyyy-MM-dd`
- Невалидные записи пропускаются
- Ошибки и предупреждения записываются в ErrorData
- Единый сценарий импорта через ImportFacade (Template Method + Strategy)
- Подменю импорта с выбором формата и указанием пути к файлу

### Экспорт данных

**Поддерживаемые форматы:**
- CSV
- JSON (включая вложенные account/category у операций)

**Особенности экспорта:**
- Русские символы не экранируются
- Сохранение в стандартную папку (например, `data-exports`)
- Проверка корректности имени файла
- Единая точка экспорта — ExportFacade
- Подменю экспорта с выбором формата и имени файла

### Аналитика

**Доступные отчеты:**
- Разница доходов и расходов за выбранный период
- Итоги: доход, расход
- Группировка по категориям 

**Архитектура аналитики:**
- Реализовано через Команды (`CalcPeriodBalance`, `GroupByCategory`) + Invoker
- Возможность измерения времени выполнения сценариев (декоратор `TimedCommand`)
- Логирование сценариев

### Автоподдержка баланса / аудит (Observer)

**Система событий:**
- `OperationAdded` / `OperationRemoved` / `OperationReplaced` в FinanceManager

**Наблюдатели:**
- `AuditObserver` — логирует действия (что добавлено/удалено/заменено)
- `BalanceObserver` — проверяет соответствие сохранённого баланса вычисленному
- Опционально: инкрементальная корректировка баланса при добавлении/удалении операции

### Консольные меню

**Главное меню:**
- CRUD операции
- Импорт данных
- Экспорт данных
- Аналитика
- Просмотр логов

**Подменю:**
- **CRUD (ConsoleCreator):** все операции со счетами/категориями/операциями
- **Импорт:** выбор формата, путь к файлу, отчёт об импорте и ошибках
- **Экспорт:** выбор формата, имя файла, подтверждение пути
- **Аналитика:** расчёт баланса за период, группировка по категориям, логи

## Техническая архитектура

### Хранилище и инфраструктура

- **In-memory репозитории** для счетов, категорий, операций
- **DI-контейнер** (`Microsoft.Extensions.DependencyInjection`) для всех зависимостей
- **ErrorData** (`MemoryErrorData`) — накопление и просмотр сообщений/ошибок
- Единые утилиты путей для экспорта
- Проверка корректности имени файла


В папке ExampleFile есть прмиеры json и csv которые парсит приложение 
## Структура проекта
```
FinanceModel/
├─ Analitic/ # Команда + Получатель + Инвокер (аналитика)
│ ├─ AnaliticFacade.cs
│ ├─ AnalyticsReceiver.cs
│ ├─ CalcPeriodBalanceCommand.cs
│ ├─ Command.cs
│ ├─ ConsoleAnalyticsMenu.cs
│ ├─ GroupByCategoryCommand.cs
│ └─ Invoker.cs
│
├─ ClientInterface/
│ └─ ConsoleMenu.cs # Базовое меню/интерфейс
│
├─ ConsoleCreator/ # CRUD-консоль для сущностей
│ └─ ConsoleCreator.cs
│
├─ Decorators/ # Декораторы команд (логирование, тайминг)
│ ├─ CommandDecorator.cs
│ └─ TimedCommand.cs
│
├─ Domain/
│ ├─ Entities/
│ │ ├─ BankAccount.cs
│ │ ├─ Category.cs
│ │ └─ Operation.cs
│ └─ Type.cs # EntryType (Доход/Расход)
│
├─ Fabric/ # Фабрики (валидация + создание)
│ ├─ Interfaces/
│ │ ├─ IBankAccountFactory.cs
│ │ ├─ ICategoryFactory.cs
│ │ └─ IOperationFactory.cs
│ ├─ BankAccountFactory.cs
│ ├─ CategoryFactory.cs
│ └─ OperationFabric.cs
│
├─ ErrorData/ # Хранилище логов/ошибок
│ ├─ ErrorData.cs
│ └─ MemoryErrorData.cs
│
├─ Export/ # Экспорт (Посетитель)
│ ├─ Intefaces/
│ │ └─ IExportVisitor.cs
│ ├─ ExportCSV.cs
│ ├─ ExportFacade.cs
│ ├─ ExportJSON.cs
│ └─ ExportSubMenu.cs
│
├─ FinanceController/
│ ├─ Observe/ # Наблюдатели (Observer)
│ │ ├─ AuditObserver.cs
│ │ └─ BalanceObserver.cs
│ └─ FinanceManager.cs # Медиатор-оркестратор + события
│
├─ import/ # Импорт (Стратегия + Шаблонный метод)
│ ├─ ConsoleImportSubmenu.cs
│ ├─ CsvImportStrategy.cs
│ ├─ ImportFacade.cs
│ ├─ ImportStorage.cs
│ ├─ ImportStrategy.cs
│ └─ JsonImportStrategy.cs
│
└─ Repository/ # In-memory репозитории
  └─ Interfaces/ # IRepoBankAccount / IRepoCategory / IRepoOperation
```
В корне решения — проект `FinanceManager/Program.cs`, где настраивается DI и запускается главное меню.

## Паттерны и их применение

| Паттерн | Файлы | Назначение |
|---------|-------|------------|
| **Factory** | `Fabric/*Factory.cs`, `Fabric/Interfaces/*.cs` | Создание валидных доменных объектов из сырых данных (консоль/импорт). Концентрирует и не дублирует валидацию. |
| **Strategy** | `import/CsvImportStrategy.cs`, `import/JsonImportStrategy.cs`, интерфейс `ImportStrategy` | Вариативный парсинг одного и того же файла разными способами (CSV/JSON), единая точка вызова в фасаде. |
| **Template Method** | `import/ImportStrategy.cs` (TemplateParse) | Фиксирует скелет процесса (Accounts→Categories→Operations), делегируя детали в стратегию. |
| **Facade** | `ImportFacade.cs`, `ExportFacade.cs`, `AnaliticFacade.cs` | Упрощённые "точки входа" для консольных меню. Прячут пайплайн, ошибки, логирование. |
| **Command** | `Analitic/Command.cs`, `CalcPeriodBalanceCommand.cs`, `GroupByCategoryCommand.cs`, `Invoker.cs` | Инкапсулирует сценарии аналитики, их вызов, возможный откат/декорирование. |
| **Decorator** | `Decorators/CommandDecorator.cs`, `TimedCommand.cs` | Ненавязиво добавляет измерение времени/логирование к любой команде без изменения её кода. |
| **Visitor** | `Export/ExportCSV.cs`, `ExportJSON.cs`, `IExportVisitor.cs` | Одинаковый обход разных наборов сущностей (Accounts/Categories/Operations) с разными форматами вывода. |
| **Observer** | `FinanceManager.cs` (события), `Observe/AuditObserver.cs`, `Observe/BalanceObserver.cs` | Реакция на добавление/удаление/замену операций: аудит, пересчёт/проверка баланса — без жёсткой связи с менеджером. |

## Dependency Injection

**Расположение**: `Program.cs` с использованием `Microsoft.Extensions.DependencyInjection`

**Регистрируемые сервисы**:
- Репозитории (`Repo*Memory`)
- Фабрики
- Фасады
- Консольные подменю
- Наблюдатели
- `FinanceManager`
- Главное меню

**Процесс**:
1. После `BuildServiceProvider()` выполняется подписка наблюдателей на события `FinanceManager`
2. Запускается `ConsoleMainMenu`

**Преимущества DI**:
- Единый источник правды для зависимостей
- Легкая подмена реализаций (in-memory → БД)

## Принципы SOLID

### S - Single Responsibility
- `*Factory` отвечает только за создание/валидацию объектов
- `*Facade` — только за сценарий (оркестрация шагов импорта/экспорта/аналитики)
- `*Strategy` — только за парсинг формата
- `Console*Menu` — только UI-слой для пользователя

### O - Open/Closed
- Новый формат импорта/экспорта: добавить новую `*Strategy/*Visitor` без изменения существующего кода
- Новая аналитическая команда подключается без изменения `Invoker`

### L - Liskov Substitution
- Любая стратегия импорта (CSV/JSON) взаимозаменяема через `ImportStrategy`
- Любой экспортёр работает через `IExportVisitor`

### I - Interface Segregation
- Разделены интерфейсы фабрик (`IBankAccountFactory`, `ICategoryFactory`, `IOperationFactory`)
- Репозитории разнесены по сущностям (интерфейсы в `Repository/Interfaces`)

### D - Dependency Inversion
- Высокоуровневые сервисы (фасады, меню) зависят от абстракций (интерфейсы фабрик, стратегий, посетителей, репозиториев)
- Конкретика внедряется через DI

## GRASP

### High Cohesion (Высокая связность)
Каждый пакет выполняет одну задачу:
- `Fabric` — создание
- `import` — импорт
- `Export` — экспорт
- `Analitic` — аналитика
- `Decorators` — сквозные аспекты

### Low Coupling (Низкая связность)
- `FinanceManager` общается через события; наблюдатели не знают о внутренней структуре менеджера
- Импорт/экспорт изолированы стратегиями/посетителями — доменные классы не зависят от форматов файлов
- Меню работают с фасадами, не обращаются к репозиториям напрямую

## Ключевые модули

- **Domain/Entities** — простые POCO: `BankAccount`, `Category`, `Operation`, enum `EntryType`
- **Repository** — in-memory реализации `IRepo*`
- **FinanceController/FinanceManager** — операции с доменом + события `OperationAdded/Removed/Replaced`
- **Fabric** — фабрики доменных объектов с валидацией входных данных
- **import** — `ImportStrategy` (Template Method) + конкретные стратегии CSV/JSON (Strategy), `ImportFacade`, подменю
- **Export** — `IExportVisitor` + реализации CSV/JSON (Visitor), `ExportFacade`, подменю, `ExportStorage`
- **Analitic** — команды аналитики (Command), `Invoker`, декораторы (Decorator) для тайминга/логов, фасад и меню
- **Observe** — `AuditObserver` (логирует), `BalanceObserver` (проверяет соответствие баланса), подписка в `Program.cs`
- **ConsoleCreator** — подменю CRUD: добавление/замена/удаление/списки

## Взаимодействие компонентов

1. `Program.cs` через DI создаёт все сервисы и подписывает наблюдателей на события `FinanceManager`
2. Главному меню передаются фасады/подменю
3. Пользователь в консоли:
   - **Импортирует файл** → `ImportFacade` → стратегия (`Csv/JsonImportStrategy`) → фабрики → `FinanceManager` (пишет в репозитории, поднимает события)
   - **Выполняет Ручной ввод** → `ConsoleCreator` → фабрики → `FinanceManager` (поднимает события, баланс инкрементально двигается)
   - **Запускает аналитику** → команды → `Invoker` (+ декоратор тайминга) → `AnalyticsReceiver`
   - **Экспортирует** → `ExportFacade` → `IExportVisitor` (CSV/JSON) → файл