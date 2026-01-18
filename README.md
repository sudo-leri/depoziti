# Николай Йоцев, F104259, 23/24 пролетен семестър

# CITB613 Практика по бизнес информационни системи

# Подготовка на специалисти по информационни технологии в икономиката на знанието

Многослойно приложение за управление и сравняване на банкови депозитни продукти, разработено с .NET и Vue.js.

## Съдържание

- [Преглед](#преглед)
- [Технологии](#технологии)
- [Архитектура](#архитектура)
- [Инсталация](#инсталация)
- [Конфигурация](#конфигурация)
- [Стартиране](#стартиране)
- [Функционалности](#функционалности)
- [Структура на проекта](#структура-на-проекта)
- [API Документация](#api-документация)

## Преглед

Проектът **BankProductsCatalog** е комплексна система за каталогизиране и сравняване на банкови депозити. Състои се от четири основни компонента:

1. **BankProducts.Core** - Споделена библиотека с модели и бизнес логика
2. **BankProducts.Api** - ASP.NET Core REST API
3. **BankProducts.Desktop** - Avalonia MVVM десктоп приложение
4. **BankProducts.Vue** - Vue.js 3 SPA (Single Page Application)

## Технологии

### Backend
- **.NET 8.0** - Модерна платформа за разработка
- **ASP.NET Core** - REST API framework
- **Entity Framework Core** - ORM за достъп до база данни
- **MySQL** - Релационна база данни
- **C# 12** - Програмен език

### Desktop
- **Avalonia UI** - Cross-platform XAML framework
- **MVVM Toolkit** - Модел-Изглед-ViewModel архитектура
- **ReactiveUI** - Реактивно програмиране

### Frontend
- **Vue.js 3** - Прогресивен JavaScript framework
- **Vue Router** - Маршрутизация
- **Axios** - HTTP клиент
- **Bootstrap 5** - CSS framework

## Архитектура

Проектът следва многослойна архитектура:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  ┌──────────────┐  ┌─────────────────┐  │
│  │  Vue.js SPA  │  │  Avalonia MVVM  │  │
│  └──────────────┘  └─────────────────┘  │
└─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────┐
│           API Layer                     │
│        ASP.NET Core REST API            │
└─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────┐
│         Business Layer                  │
│    BankProducts.Core (Services)         │
└─────────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────┐
│           Data Layer                    │
│   Entity Framework Core + MySQL         │
└─────────────────────────────────────────┘
```

## Инсталация

### Предварителни изисквания

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/)
- [Node.js 18+ и npm](https://nodejs.org/) (за Vue.js приложението)

### Стъпки за инсталация

1. **Клониране на репозиторито**
   ```bash
   git clone <repository-url>
   cd BankProductsCatalog
   ```

2. **Настройка на базата данни**

   Създайте MySQL база данни:
   ```sql
   CREATE DATABASE BankProducts CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```

3. **Инсталиране на зависимости**

   За Vue.js приложението:
   ```bash
   cd BankProducts.Vue
   npm install
   cd ..
   ```

## Конфигурация

### API Configuration

Редактирайте `BankProducts.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankProducts;User=root;Password=yourpassword;Port=3306;"
  }
}
```

### Desktop Configuration

Редактирайте connection string в `BankProducts.Desktop/ViewModels/MainWindowViewModel.cs`:

```csharp
var connectionString = "Server=localhost;Database=BankProducts;User=root;Password=yourpassword;Port=3306;";
```

## Стартиране

### 1. Стартиране на API

```bash
cd BankProducts.Api
dotnet run
```

API ще стартира на `http://localhost:5000` (или `https://localhost:5001`)

При първо стартиране базата данни автоматично ще бъде създадена и напълнена с примерни данни.

### 2. Стартиране на Vue.js приложението

**Режим на разработка:**
```bash
cd BankProducts.Vue
npm run dev
```

Приложението ще стартира на `http://localhost:5173`

**Production build:**
```bash
npm run build
```

Статичните файлове ще бъдат генерирани в `dist/` директория и могат да се сервират от ASP.NET Core API.

### 3. Стартиране на Desktop приложението

```bash
cd BankProducts.Desktop
dotnet run
```

## Функционалности

### Общи функции
- CRUD операции за банки и депозити
- Филтриране по банка, валута, срок и минимална сума
- Сортиране по лихвен процент, срок, минимална сума и банка
- Детайлна информация за всеки депозит

### Vue.js SPA
- **Начална страница** - Топ 6 депозита с най-висок лихвен процент
- **Каталог** - Разширено филтриране и сортиране
- **Детайли** - Пълна информация и финансов калкулатор
  - Месечен разплащателен план
  - Изчисляване на лихви с/без капитализация
  - Данък върху лихвите (10%)
  - ЕГЛ (Ефективен годишен лихвен процент)
- **Сравнение** - Съпоставка до 3 депозита едновременно

### Desktop приложение
- Управление на банки (добавяне, редактиране, изтриване)
- Управление на депозити (пълен CRUD)
- Формуляри с валидация
- Модерен MVVM интерфейс с Avalonia

## Структура на проекта

```
BankProductsCatalog/
├── BankProducts.Core/              # Споделена бизнес логика
│   ├── Models/                     # Домейн модели
│   │   ├── Bank.cs                 # Модел на банка
│   │   ├── Deposit.cs              # Модел на депозит
│   │   └── Currency.cs             # Енумерация на валути
│   ├── Data/                       # Слой за данни
│   │   ├── BankProductsDbContext.cs    # EF Core контекст
│   │   └── DataSeeder.cs               # Инициализация на данни
│   └── Services/                   # Бизнес услуги
│       ├── IDepositService.cs      # Интерфейс на услуга
│       └── DepositService.cs       # Имплементация на услуга
│
├── BankProducts.Api/               # REST API
│   ├── Controllers/Api/
│   │   ├── BanksController.cs      # API за банки
│   │   └── DepositsController.cs   # API за депозити
│   ├── Program.cs                  # Конфигурация на приложението
│   └── appsettings.json            # Настройки
│
├── BankProducts.Desktop/           # Avalonia Desktop App
│   ├── ViewModels/                 # MVVM ViewModel-и
│   │   ├── MainWindowViewModel.cs
│   │   ├── BanksViewModel.cs
│   │   └── DepositsViewModel.cs
│   ├── Views/                      # XAML изгледи
│   │   ├── MainWindow.axaml
│   │   ├── BanksView.axaml
│   │   └── DepositsView.axaml
│   └── Program.cs                  # Входна точка
│
└── BankProducts.Vue/               # Vue.js SPA
    ├── src/
    │   ├── views/                  # Vue компоненти-изгледи
    │   │   ├── Home.vue            # Начална страница
    │   │   ├── Catalog.vue         # Каталог с филтри
    │   │   ├── Details.vue         # Детайли + калкулатор
    │   │   └── Compare.vue         # Сравнение
    │   ├── services/
    │   │   └── api.js              # API клиент (Axios)
    │   ├── router.js               # Vue Router конфигурация
    │   ├── App.vue                 # Главен компонент
    │   └── main.js                 # Входна точка
    ├── package.json
    └── vite.config.js
```

## API Документация

### Endpoints

#### Банки

**GET** `/api/banks`
- Връща всички банки, сортирани по име

**Отговор:**
```json
[
  {
    "id": 1,
    "name": "УниКредит Булбанк"
  }
]
```

#### Депозити

**GET** `/api/deposits`
- Връща всички активни депозити с опционално филтриране

**Query параметри:**
- `bankId` (int, optional) - Филтър по ID на банка
- `currency` (string, optional) - Филтър по валута (BGN, EUR, USD)
- `minTerm` (int, optional) - Минимален срок в месеци
- `maxTerm` (int, optional) - Максимален срок в месеци
- `minAmount` (decimal, optional) - Налична сума (връща депозити с MinAmount ≤ тази стойност)
- `sortBy` (string, optional) - Поле за сортиране: `term`, `minamount`, `bank` или празно за лихва (по подразбиране)

**Примери:**
```
GET /api/deposits?currency=BGN&minTerm=12
GET /api/deposits?bankId=1&sortBy=term
GET /api/deposits?minAmount=5000
```

**Отговор:**
```json
[
  {
    "id": 1,
    "name": "Премиум депозит",
    "description": "Депозит с по-висока лихва за суми над 10000 лв",
    "minAmount": 10000.00,
    "maxAmount": null,
    "interestRate": 3.20,
    "termMonths": 12,
    "currency": "BGN",
    "hasCapitalization": true,
    "allowsAdditionalDeposits": false,
    "allowsPartialWithdrawal": false,
    "autoRenewal": true,
    "bank": {
      "id": 1,
      "name": "УниКредит Булбанк"
    }
  }
]
```

**GET** `/api/deposits/{id}`
- Връща детайли за конкретен депозит

**Отговори:**
- `200 OK` - Депозитът е намерен
- `404 Not Found` - Депозитът не съществува

## Функции на калкулатора

Финансовият калкулатор в детайлите на депозита изчислява:

### С капитализация
Лихвата се добавя към главницата всеки месец:
```
Месец 1: 10000 * (3.2% / 12) = 26.67 лв лихва
Месец 2: 10026.67 * (3.2% / 12) = 26.74 лв лихва
...
```

### Без капитализация
Лихвата се натрупва, но не се добавя към главницата:
```
Всеки месец: 10000 * (3.2% / 12) = 26.67 лв лихва
Главницата остава 10000 лв
```

### Данъци
- 10% данък върху лихвите (актуално за България)
- Нетна лихва = Брутна лихва - Данък

### ЕГЛ (Ефективен Годишен Лихвен процент)
```
ЕГЛ = (Обща печалба / Първоначална сума) * (12 / Месеци) * 100
```

## Валидация и бизнес правила

- Минималната сума на депозит трябва да е > 0
- Лихвеният процент се съхранява с точност 2 знака след десетичната запетая
- Финансовите суми се съхраняват с точност 18,2 (до 16 цифри преди запетаята)
- При изтриване на банка, всички нейни депозити също се изтриват (CASCADE)
- Само активни депозити (`IsActive = true`) се показват в приложенията

## CORS конфигурация

API-то е конфигурирано да приема заявки от:
- `http://localhost:5173` (Vue.js dev server)

За production, актуализирайте CORS политиката в `Program.cs`.

## Примерни данни

При първо стартиране се зареждат 6 български банки и 10 депозитни продукта:

**Банки:**
- УниКредит Булбанк
- Банка ДСК
- Пощенска банка
- Райфайзенбанк
- ОББ
- Първа инвестиционна банка

**Депозити:** Разнообразие от продукти с различни:
- Срокове (6-36 месеца)
- Валути (BGN, EUR, USD)
- Лихвени проценти (1.8% - 4.2%)
- Характеристики (капитализация, довнасяне, теглене)

---

**Забележка:** Това е демонстрационно приложение. Лихвените проценти и условията на депозитите са примерни и не отразяват реални банкови оферти.
