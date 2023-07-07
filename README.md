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

[_**/Entities**_](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Api/Entities). Здесь собраны схемы данных User, Product, ProductCategory, Cart, CartItem (набор продуктов корзины).

В Product через атрибут [Foreiginey(“CategoryId”)] указываем связь свойства типа ProductCategory и свойства CategoryId. Теперь первое будет определять автоматом второе устанавливая в него свое Id.
Мы разделили саму Корзину(Cart) и Товары в Корзине (CartItem). Логика тут в том, что обращений к корзине может быть больше 1 раза, т.е. будет приходить несколько CartItem адресованных одной Cart, завершение работы с которой (оплата) может быть отодвинута на неопределенный срок.

User наследуем от IdentityUser из библиотеки Microsoft.AspNetCore.Identity, что даст все необходимые для идентификации пользователя свойства. От себя добавляем UserRole и CustomerId, с которым отдельная история. Он понадобился из-за требований CORS безопасности –не пускает тип string и тип Guid в качестве параметра маршрута для Контроллера (правда потом все равно я стал доставать данные из Jwt ключа, но продолжил опять же использовать не Guid, а созданное свойство CustomerId).

В [_**/Repositories**_](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Api/Repositories) интерфейс с определением методов и их реализация в отдельных классах. Объектов всего 3:
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

В [_**/Data**_](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Api/Data) все стандартно. Класс наследующий от DbContext – передача схем в ORM (DbSet<T>), конструктор с передачей опций и переопределение метода OnModelCreating(ModelBuilder modelBuilder).


### Контроллеры <a name="backend-4"></a>
Папка [/Controllers](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Api/Controllers).
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
- [Program.cs](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Program.cs)
- [Startup.cs](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Startup.cs)

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

Методы конвертации реализованы как методы расширения для типов схем модели. Файл с методами находится в Web API приложении: [ShopOnline.Api/Extensions/DtoConversions.cs.](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Extensions/DtoConversions.cs)


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

[**_Program.cs_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/Program.cs) – точка входа.**

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

[**_App.razor._**](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/App.razor)

Класс компонента App из файла App.razor в корне проекта представляет основной компонент приложения, в рамках которого будут запускаться все другие компоненты и функциональность приложения Blazor WebAssembly.

С помощью встроенного компонента Router добавляет возможность маршрутизации по вложенным компонентам. Его атрибут AppAssembly указывает на сборку, в которой следует искать запрошенные вложенные компоненты.

При запросе компонентов может быть две ситуации: запрошенный ресурс (компонент) найден и ресурс не найден. Соответственно для каждой из этих ситуаций определены соответственно два элемента: Found и NotFound.

Компонент Found содержит другой компонент - RouteView. Через атрибут RouteData он получает контекст маршрутизации, который будут использоваться при обработке запроса. А другой атрибут - DefaultLayout устанавливает компонент, который будет определять компоновку (layout) содержимого - в данном случае это компонент MainLayout.

Комопонент NotFound определяет, как будет рендерится ответ, если компонент для обработки запроса не найден. С помощью вложенного компонента LayoutView определяется компонент, который будет задавать компоновку. В данном случае это опять же компонент MainLayout.

**[_MainLayout.razor_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/Shared/MainLayout.razor) и папка [_/Shared_](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Web/Shared).**

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

[**_/Pages_.**](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Web/Pages)

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

**[_/wwwroot_](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Web/wwwroot) – статический контент.**

Компоненты Razor может использовать стили css, скрипты javascript, файлы изображений. Для обращений к файлам из этой папки применяется относительный путь – «/».
- /css – хранит определения стилей css (файл стилей фреймворка bootstrap)
- /images – картинки для продуктов
- /js – для файлов Javascript функций.
- index.html – представляет главную страницу, на которую и будет загружаться приложение Blazor
- favicon.png, icon-192.png

[**_/Services_.**](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Web/Services)

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
Для Аутентификации и Авторизации я использую JWT ключ. Настраиваю Web API и клиентскую часть, Blazor WebAssembly.

[Инструкция из онлайн книжки Blazor in Action](https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/).

### Теория и постановка задачи <a name="auth-1"></a>
Аутентификация, или проверка подлинности — это процесс установления личности пользователя, когда пользователь вводит учетные данные, которые затем сравниваются с данными, хранящимися в операционной системе, базе данных, приложении или ресурсе. Если они совпадают, пользователи успешно проходят аутентификацию.

Авторизация — это процесс определения, есть ли у пользователя доступ к запрашиваемому им ресурсу, что подразумевает 1) проверку подлинности, 2) указание неких параметров, по которым будет проходить проверка подлинности для запрашиваемого пользователем ресурса. 

Оба понятия имеют юридическое происхождение, оба представляют собой описание процесса. Для «данных», или «параметров», которыми оперируют процессы, так же есть юридический термин Удостоверения. Оно должно быть создано, выдано пользователю, далее должно проверяться издателем при предъявлении.

Другой нюанс заключается в отрасли применения этих процессов. Отрасль связь с моделью общения «клиент-сервер» с растянутой во времени сессией связи с множеством обращений и ответов. Клиентов обращающихся к серверу много, у них разные роли. Для каждого надо провести проверку подлинности, и запускать её при каждом обращении клиента к серверу. Это требует включения в приложения соответствующих обработчиков.

Задачи для программы:
- Задание параметров/схем/политик авторизации и аутентификации
- Хранение и кодирование параметров
- Отметить действия/области, которые будут требовать авторизации, т.е. запускать событие аутентификации
- Службы и обработчики событий аутентификации и авторизации
- UI для прохода аутентификации

Поскольку создаем web приложение разбитое на 3 части, то задачи надо распределить между ними.
- Web API (backend)
  - Задание параметров/схем/политик авторизации и аутентификации
  - Хранение и кодирование параметров
  - Отметить действия/области, которые будут требовать авторизации, т.е. запускать событие аутентификации
  - Службы и обработчики событий аутентификации и авторизации
- Blazor WebAssembly (frontend)
  - Отметить действия/области, которые будут требовать авторизации, т.е. запускать событие аутентификации
  - Службы и обработчики событий аутентификации и авторизации
  - UI для прохода аутентификации

### Контекст безопасности <a name="auth-2"></a>
Идею о Удостоверении разработчики .NET перенесли в понятие «_Контекст безопасности_» -- объект, от имени которого выполняется код, чтобы к нему привязать удостоверение пользователя с ролями.

«Удостоверение» пользователя реализовано в интерфейсе IIdentity и классе Identity.

Реализация объекта «контекста безопасности» описывается в интерфейсе IPrincipal и классе ClaimsPrincipal, который предоставляет коллекцию удостоверений, представляющих собой класс ClaimsIdentity наследуемый от IIdentity.

Коллекция принимает утверждения— это утверждение о субъекте со стороны издателя. Утверждения представляют атрибуты субъекта, которые полезны в контексте операций проверки подлинности и авторизации. Субъекты и издатели — это сущности, которые являются частью сценария идентификации. Утверждения реализованы в классе Claims.

Утверждение и его Удостоверение это абстракция над системой ролей, куда можно включить гораздо более подробные сведения, которые затем использовать для авторизации или проверки подлинности.

Перечень Утверждений может быть расширен за счет сторонних ресурсов, например параметров Cookie или JWT.

### JSON Web Token (JWT) <a name="auth-3"></a>
Общедоступный стандарт для создания ключей доступа, основанный на формате JSON. Применяется при передаче данных в клиент-серверных приложениях.

Коротко об алгоритме работы JWT: 1) берем контекст безопасности, 2) добавляем к нему секретный ключ и срок действия ключа, 3) конструкт из данных переводится в JSON тип, 4) с помощью алгоритма кодирования и секретного ключа перекодируется в 50+ значное строковое значение – это и есть ключ.

При успешном прохождении аутентификации на сервере, к ответу пользователя прикрепляется созданный ключ, который далее будет сопровождать все запросы и ответы в общении пользователя и сервера до истечения срока годности ключа, по истечении которого пользователю надо будет снова пройти аутентификацию.

### В ASP.NET Core <a name="auth-4"></a>
Аутентификация и Авторизация это часто используемые функциональности, в ASP.NET сложился свой паттерн проектирования по её внедрению, завязанный на абстракции и особенности платформы.

**Модель, схема, данных пользователя, БД, Обращение к данным.**

Можно задать свою схему. У ASP.NET Core есть библиотека Microsoft.AspNetCore.Identity, которая дает:
- базовый класс IdentityUser с готовым списком необходимых свойств, свою модель надо наследовать от него или не создавать вовсе;
- класс EntityFrameworkCore.IdentityDbContext<T>, который сам наследует от DbContext Entity Framework, при этом добавляет, вместо T можно указать IdentityUser или свой класс наследующий от IdentityUser. Это избавляет нас от необходимости самостоятельно добавлять схему пользователя (DbSet<T>), а во вторых дает классу работы с ORM ряд свойств схемы IdentityUser (Users, Roles, RoleClaims, UserLogins, UserClaims, UserRoles, UserTokens).
- классы UserManager<T> и RoleManager<T>, где реализованы методы работы с IdentityUser,

DI объектов Identity, настройка, добавление их обработчиков:
```
services.AddIdentity<класс Пользователя, IdentityRole>(options => {…})
  .AddEntityFrameworkStores<класс БД>()
  .AddDefaultTokensProvider();
```
Настройку опций можно перенести в объект регистрации приложения:
```
Services.Configure<IdentityOptions>(options => {…});
```

**Службы и обработчики событий Аутентификации**

Надо (1) добавить, DI, в приложение обработчик события аутентификации, (2) задать ему «схему аутентификации», (3) добавить вызов обработчика в поток вызовов.

Обработчиком выступает готовый сервис IAuthenticationService, который регистрируется в приложении с помощью метода AddAuthentication().

«Схема аутентификации» это абстрактное понятие введение разработчиками ASP.NET. Оно включает в себя схему параметров, по которым будет проходить проверка подлинности, т.е. сопоставление с данными по пользователю хранящимися в БД. И включает объекты отвечающие за кодирование и декодирование данных. Настройка схем происходит за счет передачи параметров в метод AddAuthentication() и вызове дополнительных методов после него.

Зарегистрированный через DI IAuthenticationService надо вставить в очередь вызова ПО промежуточного слоя, чтобы он был вызван при обработке запросов. Вызываем UseAuthentication, что регистрирует middleware в цепочке обработчика запроса, использующее зарегистрированные ранее схемы проверки подлинности. Вызывать UseAuthentication следует перед вызовом любого middleware, требующего проверки подлинности пользователей (например, перед UseAuthorization()).

**Службы и обработчики событий Авторизации**

Перечень задач схожий. Надо (1) добавить в приложение обработчик события авторизации, (2) задать требования авторизации, (3) добавить вызов обработчика в поток вызовов, (4) расставить по приложению триггеры вызывающие обращение к обработчику, т.е. подписать объекты на событие авторизации.

Встроенный сервис AuthorizationMiddleware, определяющий данные и алгоритмы авторизации добавляется (DI) с помощью метода AddAuthorization().

Метод в качестве параметра может принять делегат устанавливающий «политики» авторизации. Политика это «правило», оно состоит из названия и требования. Название будет использоваться как триггер, требования укажет какие свойств использовать для сопоставления с данными их схемы аутентификации.

Мы можем создать собственный класс для Требований, который должен наследовать от IAutherizationRequirement. В нем в ручную прописать условия сопоставления с данных.

Подписка на событие авторизации имеет несколько видов реализации. В backend части используются атрибуты, главным из которых является [Authorize], а например отменить действие одного из методов можно добавив к нему атрибут [AllowAnonymous]. В frontend части используются специальные теги. Для приложения Blazor Web Assembly требуется отдельные настройки, примером реализации которых является данное web-приложение, подробный разбор будет в параграфе ниже.

### Настройка ShopOnline.Api (backend) <a name="auth-5"></a>
#### Аутентификация
[_**/Entities/User.cs**_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Entities/User.cs)

Создаем схему данных пользователя. Наследуем класс от IdentityUser из библиотеки Microsoft.AspNetCore.Identity. Из родителя получим почти все параметры, от себя только добавим UserRole и CustomerId.

[_**/Data/ShopOnlineDbContext.cs**_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Data/ShopOnlineDbContext.cs)

Наследуем класс не от DbContext класса, а от IdentityDbContext<T>, где вместо T указываем наш тип пользователя созданный в Entities, User.

[_**/Security**_](https://github.com/barbado-vl/ShopOnline/tree/master/ShopOnline.Api/Security) – службы для JWT

TokenParametrs.cs -- класс параметров JWT. Главный параметр это секретный код для кодирования ключа. Его надо обработать преобразовав в тип SymmetricSecurityKey (из Microsoft.IdentityModel.Tokens). Класс параметров регистрируется в службах приложения в Startup.cs. Там же при настройке схемы аутентификации в AddAuthentication(…).AddJwtBearer(…) параметры в неё передаются. Вторым местом вызова TokenParametrs является класс UserRepository, который описан далее
JwtDecoder.cs – статический класс для чтения параметров из ключа, используется в ShoppingCartController при обращении к данным корзины.
Сам процесс создания ключа ни каких дополнительных служб не требует. Создавать ключ будет UserRepository.cs.

[_**/Repositories/UserRepository.cs**_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Repositories/UserRepository.cs)

Определяем 2 метода:
GenerateJwt(User user) Реализация метода стандартная и представляет собой 3 задачи: 1) создать контекст безопасности (параметр List<Claim>), в который мы заносим интересующие нас данные из пользователя (имя, id, Guid, CustomerId, роли, если есть); 2) добавляем параметры ключа из TokenParametrs; 3) создание ключа (new JwtSecurityToken(…)) и передача его в обработчик события (new JwtSecurityTokenHandler()).
CreateCustomer(string userName) для создания юзера.
Использование методов происходит в контроллере.

[_**/Controllers/AccountController.cs**_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Controllers/AuthenticateJwtController.cs)

В контроллере находится 3 метода помеченных атрибутом [HttpPost], с разными маршрутами [Rout(“…”)] “Login”, “Register”, “Identify” и принимающие параметр UserDto (тип из Shared Model).
UserDto.cs содержит 3 параметра. UserName и UserPassword используются в методе Login для сопоставления с данными БД или для создания нового пользователя в методе Register. В каждом случае происходит создание нового Jwt ключа. Все операции происходят при посредничестве UserRepository.
Третий параметр это UserId используемый в методе Identify для подтверждения аутентификации при операциях авторизации на стороне клиента.

[_**Startup.cs**_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Startup.cs)

Задание схем и внедрение обработчиков в приложение.
```
services.AddIdentity<User, IdentityRole>()
.AddEntityFrameworkStores<ShopOnlineDbContext>()
.AddDefaultTokenProviders();

services.Configurate<IdentityOptions>(options =>
{
  options.Password.RequireLength = 8;
  …
};

services.AddSingleton(new TokenParametrs);

services.AddAuthentication(options =>
{
  option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
.AddJwtBearer(options => 
{
  options… = …,
  …
});
```

#### Авторизация
Косяк – делаю пустую политику CustomerOnly, а сами обращения к ключу, его дешифровку и условия пишу вручную прямо внутри 2-х методов ShoppingCartController.cs.  По хорошему надо класс IAutherizationRequirement, и в него перенести эти условия. Получение CustomerId проводить без всяких проверок (или его тоже инкапсулировать где-то). Handler сделать.

__________________________

[_**/Controllers/ShoppingCartController.cs**_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Controllers/ShoppingCartController.cs)

Корзина привязана к пользователю, доступ к её данным должен иметь только создавший её пользователь. Для данного класса требуется настроить Авторизацию, проверку доступа.

Вставляем атрибут с названием политики вызывающий событие авторизации – [Authorize(Policy = “CustomerOnly”)].

В методах контроллера обращаемся к Header запроса, из свойства Authorization достаем bearerToken, конечно с проверкой на его наличие. Из ключа с помощью созданного статического класса Security/JwtDecoder.cs получаем CustomerId, который идентифицирует пользователя и его корзину.

[_**Startup.cs**_](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Api/Startup.cs)
Задание схем и внедрение обработчиков в приложение.
```
services.AddAuthorization(options =>
{
options.AddPolicy(“CustomerOnly”, policy => police.RequireRole(“Customer”, “Admin”));
});
```

### Настройка ShopOnline.Web (frontend) <a name="auth-6"></a>
Напомню, что клиентская часть приложения представлена отдельным проектом Blazor Web Assembly, которое осуществляет запросы к Web API. На реализации Аутентификации данный нюанс ни как не сказывается, просто идет обращение к Web API. Но с Авторизацией сложнее, требуются объекты вызывающие событие авторизации, требуется обработчики этих вызовов, которые перенаправят запрос на сервер, требуются объекты сохраняющие сведения о состоянии проверки подлинности. 

**Аутентификация – Login и Register**

Компоненты и Base классы для них:
- Login.razor
- LoginBase.cs
- SingUp.razor
- SingUpBase.cs
- Logaout.razor

Из них идет обращение к службе [AuthenticateService.cs](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/Services/Security/AuthenticateService.cs) осуществляющей запросы к Web API и обрабатывающий ответы от него.

В ответе приходит Jwt ключ, который заносим в LocalStorage браузера и специальный класс IdentityAuthenticationStateProvider несущий сведения о состоянии проверки подлинности, о нем будет рассказано далее.

Есть метод Logout, который удаляет ключ и данные о проверки подлинности.

Дополнительно в AuthenticateService реализовали событие RaiseEventOnAuthenticationChanged, которые отслеживает методы Login Register и Logout, и, на которое подписываются компоненты пользовательского интерфейса.

Еще нюанс – в Header добавляем строку bearer и передаем в качестве значения ключ.

**Авторизация – «сведения о состоянии проверки подлинности».**

Задача:
- нужны данные о клиенте, «контекст безопасности» пользователя
- идентификация клиента
- обеспечить доступ к данным о клиенте для компонентов Blazor

Для этих целей в Asp.Net был введен класс AuthenticationStateProvider. У него уже реализована задача по передачи данных о клиенте для компонентов интерфейса, подробнее далее. Но для реализации остальных задач класс требуется доопределять самостоятельно из-за большой вариативности.

Класс наследник [IdentityAuthenticationStateProvide.cs.](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/Services/Security/IdentityAuthenticationStateProvider.cs)

Во первых переопределяем метод Task<AuthenticationState> GetAuthenticationStateAsync(), который будет возвращать данные о клиенте обернутые в указанный в Task тип. Алгоритм метода:
- берем ключ находящийся в LocalStorage браузера;
- если он там был достаем из него контекст безопасности пользователя (List<Claims>), для чего используем специальный созданный статический класс [Services/Security/JwtDecoder.cs ](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/Services/Security/JwtDecoder.cs)
- отправляем запрос на сервер по маршруту на метод Identity с целью проверить, есть ли такой пользователь
- если предыдущие этапы пройдены успешно, возвращаем данные пользователя, в противном случае создаем пустой объект AuthenticationState и возвращаем его.

Во вторых создаем 2 метода MarUserAuthenticated(string token) и MarkLogaout() внутри которых вызываем событие NotifyAuthenticationnStateChanged. В зависимости от того пустой или наполненный данными контекст безопасности передаем зависит состояние Авторизации – прошли проверку подлинности или не прошли.

**Авторизация – настраиваем доступ к AuthenticationState для компонентов.**

Файл [App.razor](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/App.razor). Компонент устанавливающий маршруты <Router>, который до этого является корневым, оборачиваем в компонент <CascadingAuthenticationState>. Внутри компонент RouteView меняем на AuthorizeRouteView.

Добавляем службу в файле [Program.cs](https://github.com/barbado-vl/ShopOnline/blob/master/ShopOnline.Web/Program.cs):
```
builder.Services.AddAutherizationCore();.
```

Данная настройка:
1. создает Cascade Parametr типа AuthenticationState, к которому можно обращаться из всех компонентов приложения.
```
  [CascadingParametr]
  private Task<AuthenticationState> AuthenticationStateTask { get; set; }
```
2. открывает доступ к конструкции из компонентов, которую можно использовать прямо в разметке:
```
  <AuthorizeView>
    <Authorized>< Authorized />
    <NotAuthorized>< NotAuthorized />
  <AuthorizeView/>
```

