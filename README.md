# ShopOnline

Обучающий проект от youtube канала freeCodeCamp.org: [Blazor WebAssembly & Web API on .NET 6 – Full Course (C#)](https://www.youtube.com/watch?v=sHuuo9L3e5c&t=1812s).

Задача по проекту – создание сайта-онлайн магазина. Бизнес логика включает в себя товары, категории товаров, клиентов, корзину товаров для клиента и систему оплаты.
Задача моя – изучить технологию создания SPA(single page application) web-приложения с помощью технологии ASP.NET Core Blazor Web Assembly, что так же подразумевает создание Web API приложения и приложения «библиотеки классов» в качество data transfer object, соединяющих модель данных Blazor WebAssembly приложения и сервера, Web API приложения.

Платформа .NET 6.

Среда разработки, IDE – Visual Studio 2022.

Клиентская часть (frontend) – ASP.NET Core Blazor Web Assembly.

Backend – ASP.NET Core Web API.

Посредник (DTO, data transfer object) – Class Library.

Создаем три проекта:
- /ShopOnline.Api – Web API
- /ShopOnline.Models – Class Library
- /ShopOnline.Web – Blazor WebAssembly

В настройка запуска первым указываем /ShopOnline.Api.

**Содержание:**
- [Web API (backend)](#backend)
  - [Зависимости](#backend-1)
  - [Структура](#backend-2)
  - [Работа с данными](#backend-3)
  - [Контроллеры](#backend-4)
  - [Точка входа](#backend-5)
- [Shared Model (Dto)](#dto)
- [Blazor WebAssembly (frontend)](#frontend)
  - [Зависимости](#frontend-1)
  - [Структура](#frontend-2)
  - [Работа приложения](#frontend-3)
- [Аутентификация и Авторизация](#auth)
  - [Теория и постановка задачи](#auth-1)
  - [Контекст безопасности](#auth-2)
  - [JSON Web Token (JWT)](#auth-3)
  - [В ASP.NET Core](#auth-4)
  - [Настройка ShopOnline.Api (backend)](#auth-5)
  - [Настройка ShopOnline.Web (frontend)](#auth-6)


## Web API (backend) <a name="backend"></a>
Приложение ASP.NET Core Web API. Это web-сервис, web-служба, которая воспроизводит RESTful Api архитектуру и являться хостом для запросов от клиентов. В случае нашего проекта клиентом будет выступать приложение ASP.NET Core Web Assembly.

Проект ShopOnline.Api.

### Зависимости <a name="backend-1"></a>
Устанавливаем пакеты Entity Framework Core
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Tools

Для реализации Аутентификации и Авторизации пользователей устанавливаем:
- Microsoft.AspNetCore.Authentication.JwtBearer – middleware позволяющее приложению работать с Jwt ключом.
- Microsoft.AspNetCore.Identity.EntityFrameworkCore – реализация идентификации пользователя на основе Entiry Framework Core ORM.

Настройки CORS политики безопасности – Microsoft.AspNetCore.Cors

Встроенный по умолчанию для ASP.NET Core Web API сервис ведения спецификации Swagger – Swashbuckle.AspNetCore.

Добавляем зависимость на созданный DTO проект, через Reference Manager.

### Структура <a name="backend-2"></a>
Папки и файлы настроек конфигурации и точка запуска:
- /Properties
  - launchSettings.json – настройка хоста
- appsetting.json – настройки приложения (настройки logging)
- Barbado-vl_Site.csproj – информация о зависимостях.
- libman.json – 
- Program.cs – точка запуска приложения с методом Main(string[] args), в котором вызываем модуль Startup.cs и командуем приложению Build().Run().
- Startup.cs – настройка и DI.

Модули приложения:
- /Controllers
- /Data – файл настройки ORM ShopOnlineDbContext.cs
- /Entities – схемы данных модели.
- /Extensions – новые методы для типов.
- /Repositories – обращение к данным
- /Security – классы реализующие аутентификацию и авторизацию.

Файлы созданной БД
- ShopOnline.db
- ShopOnline.db-shm
- ShopOnline.db-wal

Папка /Migration, создаваемая EF для файлов миграции.

### Работа с данными <a name="backend-3"></a>
Работа с данными бизнес логики собрана в 3 папки:
- /Entities – схемы данных
- /Repositories – методы обращения к данным
- /Data – класс DbContext

_**/Entities**_. Здесь собраны схемы данных User, Product, ProductCategory, Cart, CartItem (набор продуктов корзины).

В Product через атрибут [Foreiginey(“CategoryId”)] указываем связь свойства типа ProductCategory и свойства CategoryId. Теперь первое будет определять автоматом второе устанавливая в него свое Id.
Мы разделили саму Корзину(Cart) и Товары в Корзине (CartItem). Логика тут в том, что обращений к корзине может быть больше 1 раза, т.е. будет приходить несколько CartItem адресованных одной Cart, завершение работы с которой (оплата) может быть отодвинута на неопределенный срок.

User наследуем от IdentityUser из библиотеки Microsoft.AspNetCore.Identity, что даст все необходимые для идентификации пользователя свойства. От себя добавляем UserRole и CustomerId, с которым отдельная история. Он понадобился из-за требований CORS безопасности –не пускает тип string и тип Guid в качестве параметра маршрута для Контроллера (правда потом все равно я стал доставать данные из Jwt ключа, но продолжил опять же использовать не Guid, а созданное свойство CustomerId).

В _**/Repositories**_ интерфейс с определением методов и их реализация в отдельных классах. Объектов всего 3:
- работа с продуктами (ProductRepository.cs), 
  - Task<IEnumerable<Product>> GetItems();
  - Task<IEnumerable<ProductCategory>> GetCategories();
  - Task<Product> GetItem(int id);
  - Task<Product> GetCategory(int id);
  - Task<IEnumerable<Product>> GetItemsByCategory(int id);
- работа с корзиной (ShoppingCartRepository.cs),
  - Task CreateCustomerCart(int customerid);
  - Task<CartItem> AddItem(CartItemToAddDto cartItemToAddDto);
  - Task<CartItem> UpdateQty(int id, CartItemToQtyUpdatedDto cartItemQtyUpdateDto);
  - Task<CartItem> DeleteItem(int id);
  - Task<CartItem> GetItem(int id);
  - Task<IEnumerable<CartItem>> GetItems(int userId);
- работа с пользователем (UserRepository.cs)
  - Task<string> GenerateJwt(User user);
  - User CreateCustomer(string UserName);

Из особенностей можно отметить. Что все методы сделаны асинхронными. Обращение к данным идет через операторы запросов LINQ (from … in … select … where … select new). Реализация аутентификации и авторизации пользователя в UserRepository.cs, о чем будет рассказано в параграфе далее.

В /Data все стандартно. Класс наследующий от DbContext – передача схем в ORM (DbSet<T>), конструктор с передачей опций и переопределение метода OnModelCreating(ModelBuilder modelBuilder).


### Контроллеры <a name="backend-4"></a>
### Точка входа <a name="backend-5"></a>

## Shared Model (Dto) <a name="dto"></a>

## Blazor WebAssembly (frontend) <a name="frontend"></a>
### Зависимости <a name="frontend-1"></a>
### Структура <a name="frontend-2"></a>
### Работа приложения <a name="frontend-3"></a>

## Аутентификация и Авторизация <a name="auth"></a>
### Теория и постановка задачи <a name="auth-1"></a>
### Контекст безопасности <a name="auth-2"></a>
### JSON Web Token (JWT) <a name="auth-3"></a>
### В ASP.NET Core <a name="auth-4"></a>
### Настройка ShopOnline.Api (backend) <a name="auth-5"></a>
### Настройка ShopOnline.Web (frontend) <a name="auth-6"></a>


