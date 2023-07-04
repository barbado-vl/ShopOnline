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

В _**/Data**_ все стандартно. Класс наследующий от DbContext – передача схем в ORM (DbSet<T>), конструктор с передачей опций и переопределение метода OnModelCreating(ModelBuilder modelBuilder).


### Контроллеры <a name="backend-4"></a>
Папка /Controllers.
Все контроллеры настроены под поведение Web API, что включает:
- наследование от ControllerBase
- атрибут класса [ApiController]
- атрибут класса указания маршрута [Route(“api/[controller]”)]
Настраиваем методы:
- атрибуты методов для типов запроса [HttpGet], [HttpGet(“{id:int}”)], [HttpPost] и др.
- через атрибут [FromBody] привязываем параметр методы к данным запросу.

Установлены свойства для объектов обращения к данным (Repositories), которые инициализируются в конструкторе. Все методы асинхронные. Внутри идет обертка try catch. Возвращают методы действие, конкретно один из методов ControllerBase класса, возвращающих Status Code ответа, и, получающих в качестве параметра модель данных для ответа.

В качестве модели данных для ответа используются Dto объекты из Class Library. Что перевести объекты данных, описанные в /Entities в Dto объекты, используются методы расширения для этих объектов определенные в папке /Extensions в файле DtoConversions.cs.

- AuthenticateJwtController.cs
  - [HttpPost] async Task<IActionResult> Logit([FromBody] UserDto login);
  - [HttpPost] async Task<IActionResult> Register([FromBody] UserDto login);
  - [HttpPost] async Task<IActionResult> Identify([FromBody] UserDto login);
- ProductController.cs
  - [HttpGet] async Task<IActionResult<IEnumerable<ProductDto>>> GetItems();
  - [HttpGet(“{id:int}”)] async Task<IActionResult<ProductDto>> GetItem(int id);
  - [HttpGet] async Task<IActionResult<IEnumerable<ProductCategoryDto>>> GetIProductCategories();
  - [HttpGet(“{id:int}”)] async Task<IActionResult<IEnumerable<ProductDto>>> GetItemsByCategory(int categoryId);
- ShoppingCartController.cs
  - [HttpGet] async Task<IActionResult<IEnumerable<CartItemDto>>> GetItems();
  - [HttpGet(“{id:int}”)] async Task<IActionResult< CartItemDto >> GetItem(int id);
  - [HttpPost] async Task<ActionResult<CartItemDto>> PostItem([FromBody] CartItemToAddDto cartItemToAddDto)
  - [HttpDelete(“{id:int}”)] async Task<ActionResult< CartItemDto >>DeleteItem(int id);
  - [HttpPatch(“{id:int}”)] async Task<ActionResult<CartItemDto>> UpdateQty(int id, CartItemToAddDto cartItemToAddDto)


### Точка входа <a name="backend-5"></a>
- Program.cs
- Startup.cs

Был использован расширенный вариант настройки и запуска приложения через IHostBuilder.

В файле Program.cs точка входа в приложение, метод Main(string[] args).Внутри него вызываем экземпляр IHostBuilder, который через статический класс Host принимает настройки в виде класса Startup, а затем вызывает методы Build() и Run().

Класс Startup (файл Startup.cs). В нем определяются интерфейсы принимающие свойств и настраивающие поведение приложение (composition root).

При DI CORS политик безопасности указываем на запросы с каких адресов можно отвечать нашей web-службе – указываем url с номером порта приложение Blazor WebAssembly (папка и проект ShopOnline.Web).
Большинство настроек связаны с внедрением Аутентификации и Авторизации, о чем подробнее расписано в соответствующем параграфе.

Свойсвто IConfiguraiotn.
- IServiceCollection – настройка DI:
- AddControllers();
- AddEndpointsApiExplorer();
- AddSwaggerGen();
- AddSqlite<ShopOnlineDbContext>(“Data Source = ShopOnline.db”);
- AddIdentity<User, IdentityRole>(). AddEntityFrameworkStores<ShopOnlineDbContext>(). AddDefaultTokenProviders();
- AddSingleton(tokenParametrs);
- AddAuthentication(option => {…}).AdJwtBearer(option => {…});
- Configure<IdentityOptions>(option => {…});
- AddCors(option => {…});
- AddAuthorization(option => {…});
- AddScoped<IProductRepository, ProductRepository>();
- AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
- AddScoped<UserRepository>();

IApplicationBuilder – прописываем конвейер вызовов. Порядок вызовов очень важен.
- в зависимости от параметра env.IsDevelopment() (тип IWebHostEnvironment), добавляем запуска midlware спецификации:
- app.UseSwagger();
- app.UseSwaggerUI();
- app.UseHttpsRedirection() 
- app.UseRouting() 
- app.UseAuthentication()
- app.UseAuthorization()
- app.UseCors();
- app.UseEndpoints();

Нюанс – настройка CORS политики безопасности.
```
services.AddAuthorization(options =>
{
  options.AddDefaultPolicy( policy =>
  {
    Policy.WithOrigins(“http адрес клиента”, “https адрес клиента”)
          .AllowAnyHeader()
          .AllowAnyMethod();
  });
});
```

## Shared Model (Dto) <a name="dto"></a>
Dto (Data transfer object) – срез данных переносимых с сервера на клиента. Так же можно использовать название Shared, что подразумевает общие объекты, объекты посредники. По реализации это просто классы-схемы для данных, без каких-либо методов.

- /ShopOnline.Models – проект Class Library. Внутри:
  - /Dtos
    - CartItemDto.cs
    - CartItemQtyUpdateDto.cs
    - CartItemToAddDto.cs
    - ProductCategoryDto.cs
    - ProductDto.cs
    - UserDto.cs

Использование. В Web API в контроллерах при отправке данных в ответ на запрос происходит конвертация данных из схемы модели в схемы Dto. С другой стороны клиент, приложение Blazor WebAssembly комплектует данные в Dto схемы, а на стороне Web API при их получении в контроллерах идет конвертация в схемы модели.

Методы конвертации реализованы как методы расширения для типов схем модели. Файл с методами находится в Web API приложении: ShopOnline.Api/Extensions/DtoConversions.cs.


## Blazor WebAssembly (frontend) <a name="frontend"></a>
Как говорилось во введении web-сайт создается как SPA-приложение, с динамическим UI, что обеспечивается использование Компонентов. Технология предоставляющая такие возможности на языке C# это ASP.NET Core Blazor Web Assembly. Суть – приложение на C# языке будет загружаться в браузер и компилироваться им встроенным движком WebAssembly.

Проект ShopOnline.Web.

### Зависимости <a name="frontend-1"></a>
Пакет Sdk – Microsoft.NET.Sdk.BlazorWebAssembly.

Зависимости устанавливаемые вместе с пакетом:
- Microsoft.AspNetCore.Components.WebAssembly – основная библиотека
- Microsoft.AspNetCore.Components.WebAssembly.DevServer – сервер разработки, используемый при компиляции приложений Blazor. Он добавляет ПО промежуточного слоя (midleware) для отладки приложения в средствах разработчика Chromium.

Для реализации Аутентификации добавил библиотеки:
- Microsoft.AspNetCore.Components.WebAssembly.Authentication
- System.IdentityModel.Jwt

Библиотека для работы с localstorage браузера – Blazored.LocalStorage

Библиотека для работы с Json форматом данных – Newtonsoft.Json.

Добавляем зависимость на созданный /ShopOnline.Model (Shared Model) проект, через Reference Manager.

### Структура <a name="frontend-2"></a>
_Program.cs_ – содержит класс Program, который представляет точку входа в приложение. Так же в данном случае запуска и настройку хоста (IHost), в рамках которого разворачивается приложение с Blazor, оставили в этом же файле.

_App.razor_ – содержит определение корневого компонента приложения, который позволяет установить маршрутизацию между вложенными компонентами с помощью другого встроенного компонента Router.

__Imports.razor_ – содержит подключения пространств имен с помощью директивы using, которые будут подключаться в компоненты Razor.

_PageHistoryState.cs_ – класс в помощь для переадресации маршрута.

_/wwwroot_ – содержит статические файлы приложения.

_/Pages_— содержит компоненты Razor, являющиеся отдельными страницами.

_/Shared_ – хранит дополнительные компоненты Razor, основной компонент MainLayout.razor

_/Services_ – находится реализация обработки данных бизнес логики на стороне клиента

Для части Компонентов представления создаются Base классы, куда переносится вся логика компонента представления. Пример:
- _DisplayProduct.razor_
- _DisplayProductBase.cs_

Компонент наследуем от Base класса, а сам он уже наследует от ComponentBase класса платформы ASP.NET.

Архитектура модульного типа с разделением на конструкционные объекты и функционал в виде api используемого в конструкциях. Конструкционные объекты это : /Pages и /Shared компоненты. Функционал это стилизация, папка /wwwroot, и работы с данными и web-службой, папка /Services.

Еще нюанс – не удалены предустановленные файлы демонстрирующие возможности Blazor. Это /Pages/Counter.razor, /wwwroot/sampledata/, /Shared/SurveyPrompt.razor.


### Работа приложения <a name="frontend-3"></a>
Логика основной части приложения описана в данном разделе.

Механизм Аутентификации и Авторизации рассматривается в отдельном разделе.

**_Program.cs_ – точка входа.**

Вызываем экземпляр класса WebAssemblyHostBuildre, настраиваем его, собираем (Build()) и запускаем (RunAsync() в случаи текущего проекта).

С помощью свойства RootComponents и его свойства Add() добавляется класс корневого компонента и его селектор. То есть в данном случае класс компонента называется App, а для его рендеринга на веб-странице используется элемент c css-селектором #app, то есть такой элемент, у которого id=app (/wwwroot/index.html).
>   builder.RootComponents.Add<App>("#app");

Добавляется компонент HeadOutlet - он позволяет вносить изменения в элемент <head> на html-странице (например, обновлять мета-теги или заголовок страницы).

Главное, в приложение внедряется в качестве сервиса HttpClient, который используется в компонентах Blazor для отправки http-запросов:
>   builder.Services.AddScoped(sp => new HttpClient { BaseAdress = new Uri(адрес Web API) });

Далее идет DI всех необходимых объектов в builder.Services (тип IServicesCollection):
- AddScoped<IProductService, ProductService>();
- AddScoped<IShoppingCartService, ShoppingCartService>();
- AddScoped<IManageProductsLocalStorageService, ManageProductsLocalStorageService>
- AddSingleton<PageHistoryState>();
- AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();
- AddScoped<AuthenticateService>();
- AddOptions();
- AddAuthorizationCore();
- AddBlazoredLocalStorage();

**_App.razor._**

Класс компонента App из файла App.razor в корне проекта представляет основной компонент приложения, в рамках которого будут запускаться все другие компоненты и функциональность приложения Blazor WebAssembly.

С помощью встроенного компонента Router добавляет возможность маршрутизации по вложенным компонентам. Его атрибут AppAssembly указывает на сборку, в которой следует искать запрошенные вложенные компоненты.

При запросе компонентов может быть две ситуации: запрошенный ресурс (компонент) найден и ресурс не найден. Соответственно для каждой из этих ситуаций определены соответственно два элемента: Found и NotFound.

Компонент Found содержит другой компонент - RouteView. Через атрибут RouteData он получает контекст маршрутизации, который будут использоваться при обработке запроса. А другой атрибут - DefaultLayout устанавливает компонент, который будет определять компоновку (layout) содержимого - в данном случае это компонент MainLayout.

Комопонент NotFound определяет, как будет рендерится ответ, если компонент для обработки запроса не найден. С помощью вложенного компонента LayoutView определяется компонент, который будет задавать компоновку. В данном случае это опять же компонент MainLayout.

**_MainLayout.razor_ и папка _/Shared_.**

Предустановленные компоненты:
- MainLayout.razor – компонент MainLayout определяет структуру или компоновку приложения blazor.
- NavMenu.razor – компонент NavMenu определяет элементы навигации.

Созданные для задач проекта:
- AccountMenu.razor
- ProductCategoiesNavMenu.razor – компонент
- ProductCategoriesNavMenuBase.cs – базовый класс с логикой для компонента

Компонент MainLayoutнаследуется от класса LayoutComponentBase, который определяет базовую функциональность для компоновки. C помощью свойства Body в определенном месте разметки будет добавляться выбранный компонент. То есть на место вызова @Body будет добавляться контент компонентов из папки /Pages.

С помощью элемента <NavMenu /> добавляется компонент NavMenu, представление боковой панели навигации, из файла Shared/NavMenu.razor, который создает систему навигации. Благодаря этому мы можем переходить к различным компонентам внутри приложения по набору ссылок. При этом при обращении по ссылке никаких запросов на сервер не идет. Все запросы обрабатываются локально.

Так же добавляем перед Body созданный компонент AccountMenu.razor, который дает представление для верхней строки навигации, где будут указаны действия для Идентифицированного и Не Идентифицированного пользователя.

В папке /Shared, кроме перечисленных выше, располагаются другие компоненты общие для всего приложения. В случае текущего проекта это ProductCategoriesNavMenu.razor 

**_/Pages_.**

Компоненты, которые представляют отдельные ресурсы, страницы, и, к которым пользователь может осуществлять запросы, располагаются в папке Pages. Чтобы они могли быть сопоставлены с определенными маршрутами, в начале каждого подобного компонента указывается директива @page с указанием маршрута.
- /Authentication
  - Login.razor
  - LoginBase.cs
  - Logout.razor
  - SingUp.razor
  - SingUpBase.cs
- Checkout.razor
- CheckoutBase.cs
- DisplayError.razor
- DisplayProducts.razor
- DisplayProductsBase.cs
- DisplaySpinner.razor
- ProductDetails.razor
- ProductDetailsBase.cs
- Product.razor
- ProductBase.cs
- ProductCategory.razor
- ProductCategoryBase.cs
- ShoppingCart.razor
- ShoppingCartBase.cs

**_/wwwroot_ – статический контент.**

Компоненты Razor может использовать стили css, скрипты javascript, файлы изображений. Для обращений к файлам из этой папки применяется относительный путь – «/».
- /css – хранит определения стилей css (файл стилей фреймворка bootstrap)
- /images – картинки для продуктов
- /js – для файлов Javascript функций.
- index.html – представляет главную страницу, на которую и будет загружаться приложение Blazor
- favicon.png, icon-192.png

**_/Services_.**

Находится реализация обработки данных бизнес логики на стороне клиента.
- /Contracts – интерфейсы
  - IProductService.cs
  - IShoppingCartService.cs
- /Security – классы для реализации аутентификации и авторизации
  - AuthenticateService.cs
  - IdentityAuthenticationStateProvider.cs
  - JwtDecoder.c
- ProductService.cs – получение данных о товарах через создание запросов к web-службе
- ShoppingCartService.cs – отправка и получение данных от web-службы о Корзине покупателя
- ManageProdactsLocalStorageService.cs – добавление, удаление и получение Коллекции Продуктов из localstorage браузера.

Путь вызовов на примере компонента Products.razor. В модуле логики компонента ProductsBase.cs обращаемся к экземпляру ManageProductsLocalStorageService, который в свою очередь дает вызов в ProductService, где обращаемся к экземпляру HttpClient для создания запросы и получения ответа по адресу “api/Product”.

Создание экземпляров и передача ссылок на них в конструкторах классов осуществляется платформой ASP.NET благодаря настройке DI в файле Program.cs.

Работа модулей из папки /Securit описана в разделе Аутентификация и Авторизация.


## Аутентификация и Авторизация <a name="auth"></a>
### Теория и постановка задачи <a name="auth-1"></a>
### Контекст безопасности <a name="auth-2"></a>
### JSON Web Token (JWT) <a name="auth-3"></a>
### В ASP.NET Core <a name="auth-4"></a>
### Настройка ShopOnline.Api (backend) <a name="auth-5"></a>
### Настройка ShopOnline.Web (frontend) <a name="auth-6"></a>


