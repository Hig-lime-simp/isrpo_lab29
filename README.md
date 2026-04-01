# README.md для лабораторной работы №29

# Лабораторная работа №29: REST API на ASP.NET Core

## Основная информация

**ФИО:** Грошев Никита Андреевич  
**Группа:** ИСП-232  
**Дата:** 01.04.2026

---

## Описание проекта

В ходе лабораторной работы создано REST API для управления задачами (Task Manager). Проект реализован на ASP.NET Core с использованием **Controller-based API**, Entity Framework Core (в памяти), Swagger для документации и CORS для взаимодействия с фронтендом.

### Что было изучено:
- Создание RESTful API с правильными HTTP-методами (GET, POST, PUT, PATCH, DELETE)
- Использование DTO (Data Transfer Objects) для разделения моделей данных
- Настройка Swagger для автоматической документации API
- Реализация фильтрации, поиска, сортировки и статистики
- Подключение CORS для кросс-доменных запросов
- Тестирование API через Swagger UI и REST Client в VS Code

---

## Как запустить проект

### 1. Перейти в папку проекта
```bash
cd TaskApi
```

### 2. Восстановить зависимости
```bash
dotnet restore
```

### 3. Запустить сервер
```bash
dotnet run
```

Сервер запустится на порту, указанном в `launchSettings.json` (например, `http://localhost:5293`).

### 4. Открыть Swagger UI
```
http://localhost:5000/swagger
```

---

## Структура проекта

```
TaskApi/
├── Controllers/
│   └── TasksController.cs          # Контроллер задач
├── Models/
│   ├── TaskItem.cs                 # Модель задачи
│   ├── CreateTaskDto.cs            # DTO для создания
│   └── UpdateTaskDto.cs            # DTO для обновления
├── Program.cs                       # Настройка приложения
├── appsettings.json                 # Конфигурация
├── requests.http                    # Тестовые запросы (REST Client)
└── Properties/
    └── launchSettings.json          # Настройки запуска
```

---

## Таблица маршрутов

| Метод | Маршрут | Описание | Статус успеха | Статус ошибки |
|-------|---------|----------|----------------|----------------|
| GET | `/api/tasks` | Получить все задачи (с фильтром `?completed=true/false`) | 200 OK | — |
| GET | `/api/tasks/{id}` | Получить задачу по ID | 200 OK | 404 Not Found |
| GET | `/api/tasks/search?query=` | Поиск задач по тексту | 200 OK | 400 Bad Request |
| GET | `/api/tasks/priority/{level}` | Задачи по приоритету (Low/Normal/High) | 200 OK | 400 Bad Request |
| GET | `/api/tasks/stats` | Статистика задач | 200 OK | — |
| GET | `/api/tasks/sorted?by=&desc=` | Сортировка задач | 200 OK | — |
| POST | `/api/tasks` | Создать новую задачу | 201 Created | 400 Bad Request |
| PUT | `/api/tasks/{id}` | Обновить задачу целиком | 200 OK | 400, 404 |
| PATCH | `/api/tasks/{id}/complete` | Переключить статус выполнения | 200 OK | 404 Not Found |
| DELETE | `/api/tasks/{id}` | Удалить задачу | 204 No Content | 404 Not Found |

---

## Примеры запросов

### GET все задачи
```http
GET /api/tasks
```

### GET с фильтрацией (только выполненные)
```http
GET /api/tasks?completed=true
```

### GET с поиском
```http
GET /api/tasks/search?query=ASP
```

### GET по приоритету
```http
GET /api/tasks/priority/High
```

### GET статистика
```http
GET /api/tasks/stats
```

### GET сортировка
```http
GET /api/tasks/sorted?by=priority&desc=true
```

### POST создать задачу
```http
POST /api/tasks
Content-Type: application/json

{
    "title": "Новая задача",
    "description": "Описание задачи",
    "priority": "High"
}
```

### PUT обновить задачу
```http
PUT /api/tasks/1
Content-Type: application/json

{
    "title": "Обновлённая задача",
    "description": "Новое описание",
    "isCompleted": true,
    "priority": "Normal"
}
```

### PATCH переключить статус
```http
PATCH /api/tasks/2/complete
```

### DELETE удалить задачу
```http
DELETE /api/tasks/1
```

---

## Использование REST Client в VS Code

1. Установите расширение **REST Client** (от Huachao Mao)
2. Создайте файл `requests.http` в корне проекта
3. Настройте базовый URL:
```http
@baseUrl = http://localhost:5293
```
4. Нажимайте **Send Request** над каждым запросом

Пример файла `requests.http`:
```http
### Получить все задачи
GET {{baseUrl}}/api/tasks

### Создать задачу
POST {{baseUrl}}/api/tasks
Content-Type: application/json

{
    "title": "Тестовая задача",
    "description": "Создана через REST Client",
    "priority": "High"
}
```

---

## Настройка CORS

В `Program.cs` добавлена политика CORS для взаимодействия с фронтендом:

```csharp
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
    )
);

app.UseCors("AllowAll");
```

---

## Главные выводы

1. **REST API** — архитектурный стиль, основанный на ресурсах и HTTP-методах. URL указывает **что**, метод — **как**.

2. **DTO** защищают API от некорректных данных. Клиент передаёт только разрешённые поля (Id и CreatedAt не могут быть изменены).

3. **Swagger** автоматически генерирует документацию и позволяет тестировать API без Postman.

4. **Правильные HTTP-статусы** — часть контракта API. Клиент понимает, что произошло, по коду ответа (200, 201, 400, 404, 204).

5. **LINQ** позволяет писать запросы к коллекциям в стиле SQL прямо в C# коде.

6. **REST Client** в VS Code — удобный инструмент для хранения и повторного использования тестовых запросов.

---

## Сравнение с предыдущими лабораторными

| Концепция | Lab 27 (Minimal API) | Lab 29 (Controller-based API) |
|-----------|----------------------|-------------------------------|
| Организация | Всё в Program.cs | Отдельные классы контроллеров |
| Маршруты | `app.MapGet(...)` | `[HttpGet]` атрибуты |
| Документация | Ручная | Swagger автоматически |
| DTO | Анонимные объекты | Отдельные классы |
| Фильтрация | Встроена в метод | Через `[FromQuery]` |

---

## GitHub репозиторий

[Ссылка на репозиторий](https://github.com/Hig-lime-simp/isrpo_lab29.git)

---

## Автор

**ФИО:** Грошев Никита Андреевич  
**Группа:** ИСП-232  
**Дата:** 01.04.2026