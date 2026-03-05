# Ticket Management API

REST API для системы обработки пользовательских обращений (ticket system).

Проект реализован как учебный backend-api с луковой архитектурой (onion architecture) и role-based доступом.

---

## Технологии

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT Authentication
- Swagger

---

## Архитектура

Проект разделён на слои:

```

Api            — HTTP слой (Controllers, Middleware)
Application    — бизнес-логика, DTO, сервисы
Domain         — доменные сущности
Infrastructure — доступ к данным и security

````

---

## Роли

### User

- создание тикетов
- редактирование тикета (с ограничениями)
- комментарии к своим тикетам
- закрытие тикета

### Agent

- просмотр всех тикетов
- комментарии к любому тикету
- изменение статуса тикета

---

## Клонирование репозитория

```bash
git clone https://github.com/9w-Mikita-H/ticket-management-api.git
cd ticket-management-api
````

---

## Установка EF Tools (при необходимости)

```bash
dotnet tool install --global dotnet-ef
```

---

## Восстановление зависимостей

```bash
dotnet restore
```

---

## Создание базы данных

```bash
dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/Api
```

После выполнения команды будет создан файл базы данных:

```
tickets.db
```

---

## Запуск проекта

```bash
dotnet run --project src/Api
```

---

## Swagger

После запуска API Swagger доступен по адресу:

```
http://localhost:<PORT>/swagger
```

Актуальный порт выводится в консоли после запуска приложения.

---

## Авторизация

API использует JWT.

Порядок действий:

1. Выполнить запрос `POST /api/auth/register`
2. Выполнить запрос `POST /api/auth/login`
3. Использовать полученный `accessToken` в заголовке запроса

Пример:

```
Authorization: Bearer <token>
```

---

## Пример запроса

```bash
curl -X POST "http://localhost:5293/api/tickets" \
-H "Authorization: Bearer <token>" \
-H "Content-Type: application/json" \
-d "{\"title\":\"test\",\"description\":\"test\"}"
```

---

## Статус проекта

Учебный backend-проект для демонстрации:

* REST API
* role-based authorization
* clean architecture
* работа с EF Core
