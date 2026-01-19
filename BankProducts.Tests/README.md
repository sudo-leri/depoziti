# BankProducts.Tests

Unit тестове за BankProductsCatalog проекта.

## Общо

Този проект съдържа комплексни unit тестове за основните компоненти на BankProductsCatalog:
- **DepositService** - бизнес логика за работа с депозити и банки
- **DepositsController** - API контролер за депозити
- **BanksController** - API контролер за банки

## Технологии

- **xUnit** - тестов framework
- **FluentAssertions** - fluent API за assertions
- **Moq** - mocking library за изолиране на зависимости
- **EntityFrameworkCore.InMemory** - in-memory база данни за тестване

## Структура на тестовете

```
BankProducts.Tests/
├── Controllers/
│   ├── DepositsControllerTests.cs    # 14 теста за DepositsController
│   └── BanksControllerTests.cs       # 7 теста за BanksController
└── Services/
    └── DepositServiceTests.cs        # 23 теста за DepositService
```

## Статистика

- **Общо тестове**: 44
- **Успешни**: 44 ✅
- **Неуспешни**: 0

## Покритие

### DepositService (23 теста)

**CRUD операции за депозити:**
- ✅ Извличане на всички активни депозити
- ✅ Извличане на депозити сортирани по лихвен процент
- ✅ Извличане с включена банкова информация
- ✅ Извличане на депозит по ID
- ✅ Създаване на нов депозит
- ✅ Актуализиране на съществуващ депозит
- ✅ Изтриване на депозит

**Филтриране на депозити:**
- ✅ Филтър по банка (BankId)
- ✅ Филтър по валута (Currency)
- ✅ Филтър по минимален срок (MinTermMonths)
- ✅ Филтър по максимален срок (MaxTermMonths)
- ✅ Филтър по налична сума (MinAmount)
- ✅ Комбинирани филтри

**Сортиране на депозити:**
- ✅ Сортиране по срок (възходящо/низходящо)
- ✅ Сортиране по минимална сума
- ✅ Сортиране по име на банка
- ✅ Сортиране по лихвен процент (по подразбиране)

**CRUD операции за банки:**
- ✅ Извличане на всички банки сортирани по име
- ✅ Извличане на банка по ID с депозити
- ✅ Създаване на нова банка
- ✅ Актуализиране на съществуваща банка
- ✅ Изтриване на банка

**Edge cases:**
- ✅ Изтриване на несъществуващ депозит (не хвърля грешка)
- ✅ Изтриване на несъществуваща банка (не хвърля грешка)
- ✅ Извличане на несъществуващ депозит (връща null)
- ✅ Извличане на несъществуваща банка (връща null)

### DepositsController (14 теста)

**HTTP endpoints:**
- ✅ GET /api/deposits - връща всички депозити
- ✅ GET /api/deposits/{id} - връща конкретен депозит
- ✅ GET /api/deposits/{id} - 404 при несъществуващ ID

**Query параметри:**
- ✅ Филтър по BankId
- ✅ Филтър по Currency
- ✅ Филтър по MinTerm
- ✅ Филтър по MaxTerm
- ✅ Филтър по MinAmount
- ✅ SortBy параметър
- ✅ Комбинирани параметри

**DTO трансформация:**
- ✅ Правилно мапване на Currency enum към string
- ✅ Правилно включване на банкова информация
- ✅ Правилно обработване на nullable полета (MaxAmount)

### BanksController (7 теста)

**HTTP endpoints:**
- ✅ GET /api/banks - връща всички банки

**Функционалност:**
- ✅ Връща всички банки
- ✅ Правилен формат на BankDto
- ✅ Обработване на празен резултат
- ✅ Правилно мапване на ID
- ✅ Правилно мапване на име
- ✅ Запазване на реда на банките
- ✅ Извикване на сервиза само веднъж

## Стартиране на тестовете

### Всички тестове

```bash
dotnet test
```

### С подробен изход

```bash
dotnet test --verbosity normal
```

### Конкретен тестов клас

```bash
dotnet test --filter "FullyQualifiedName~DepositServiceTests"
dotnet test --filter "FullyQualifiedName~DepositsControllerTests"
dotnet test --filter "FullyQualifiedName~BanksControllerTests"
```

### Конкретен тест

```bash
dotnet test --filter "FullyQualifiedName~GetAllDepositsAsync_ShouldReturnOnlyActiveDeposits"
```

## Забележки

### Използване на InMemory база данни

За тестване на `DepositService` се използва Entity Framework Core InMemory provider. Това позволява бързо изпълнение на тестовете без нужда от реална база данни. Всеки тест получава свой собствен database context с уникално име, което гарантира изолация между тестовете.

### Използване на Moq

За тестване на контролерите се използва Moq библиотеката за създаване на mock обекти на `IDepositService`. Това позволява:
- Изолиране на тестовете от бизнес логиката
- Симулиране на различни сценарии (успешни заявки, грешки, празни резултати)
- Проверка дали контролерите правилно извикват сервизите

### FluentAssertions

FluentAssertions предоставя изразителен и четим синтаксис за assertions:
```csharp
result.Should().NotBeNull();
result.Should().HaveCount(3);
deposit.InterestRate.Should().Be(3.5m);
```

## Добавяне на нови тестове

При добавяне на нова функционалност, добавете и съответните unit тестове:

1. **За нов метод в сервиза** - добавете тест в `DepositServiceTests.cs`
2. **За нов API endpoint** - добавете тест в съответния Controller test файл
3. **За нов модел или валидация** - създайте нов тестов клас

### Пример за нов тест

```csharp
[Fact]
public async Task NewMethod_WithValidInput_ShouldReturnExpectedResult()
{
    // Arrange
    var input = new TestData();

    // Act
    var result = await _service.NewMethod(input);

    // Assert
    result.Should().NotBeNull();
    result.SomeProperty.Should().Be(expectedValue);
}
```

## Continuous Integration

Тестовете могат лесно да бъдат интегрирани в CI/CD pipeline:

```yaml
# GitHub Actions пример
- name: Run tests
  run: dotnet test --verbosity normal --logger "trx" --results-directory "TestResults"
```

## Покритие на кода (Code Coverage)

За генериране на code coverage отчет:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

За по-подробен HTML отчет може да се използва [ReportGenerator](https://github.com/danielpalme/ReportGenerator):

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## Въпроси и проблеми

При проблеми с тестовете:
1. Уверете се, че всички NuGet пакети са актуални
2. Проверете дали .NET 10.0 SDK е инсталиран
3. Изчистете и презаредете проекта: `dotnet clean && dotnet restore`
