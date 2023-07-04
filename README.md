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
### Зависимости <a name="backend-1"></a>
### Структура <a name="backend-2"></a>
### Работа с данными <a name="backend-3"></a>
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


