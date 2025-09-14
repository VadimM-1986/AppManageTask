Task Management Application

Веб-приложение для управления задачами с функциями создания, отслеживания и управления задачами с установленными сроками выполнения.

🚀 Основные возможности
    ✅ Создание и управление задачами
    📅 Установка сроков выполнения
    🏷️ Статусы задач (активные, завершенные, просроченные)
    🔍 Поиск и фильтрация задач
    📊 Автоматическая проверка просроченных задач
    🐳 Docker-контейнеризация

🛠 Технологический стек
    Backend: .NET 8.0, ASP.NET Core
    Database: PostgreSQL 16
    ORM: Entity Framework Core 8.0
    Containerization: Docker, Docker Compose
    Architecture: Clean Architecture, Repository Pattern

📦 Предварительные требования
    Docker (версия 20.10+)
    Docker Compose (версия 2.0+)
    Git

🐳 Запуск через Docker Compose
1. Клонирование репозитория
bash: git clone <your-repository-url>
cd ManageTasks

3. Создание сети Docker (если не создана)
bash: docker network create shared_net

4. Запуск приложения
bash: docker-compose up --build -d

4. Остановка приложения
bash: docker-compose down

🌐 Доступ к приложению
После успешного запуска приложение будет доступно по адресу:
    Web Application: http://localhost:4050
    API Endpoints: http://localhost:4050/api/[endpoints]

📊 База данных
Приложение использует PostgreSQL с автоматическими миграциями EF Core. База данных инициализируется автоматически при первом запуске.

Параметры подключения:
    Хост: localhost
    Порт: 5432
    База данных: userTaskDb
    Пользователь: postgres
    Пароль: postgres

🔧 Переменные окружения
    Приложение поддерживает следующие переменные окружения:
    Переменная	Значение по умолчанию	Описание
    ASPNETCORE_ENVIRONMENT	Development	Окружение выполнения
    ASPNETCORE_URLS	http://*:8080	URL-адреса приложения
    ConnectionStrings__DefaultConnection	-	Строка подключения к БД
    TZ	Europe/Moscow	Часовой пояс

📋 API Endpoints Tasks Management
    GET /api/tasks - Получить список всех задач
    GET /api/tasks/{id} - Получить задачу по ID
    POST /api/tasks - Создать новую задачу
    PUT /api/tasks/{id} - Обновить задачу
    DELETE /api/tasks/{id} - Удалить задачу
    GET /api/tasks/overdue - Получить просроченные задачи

Модель задачи (Task)
json:
{
  "id": "uuid",
  "title": "string",
  "description": "string",
  "dueDate": "datetime",
  "status": "int (0-2)"
}

🔄 Миграции базы данных
Миграции выполняются автоматически при запуске приложения. Для ручного выполнения:
bash: dotnet ef database update

📁 Структура проекта
text:
AppManageTasks/
├── Controllers/          # API контроллеры
├── Services/            # Бизнес-логика
├── Repository/          # Паттерн Repository
├── Models/              # Модели данных
├── Migrations/          # Миграции БД
└── Dockerfile           # Конфигурация Docker

📝 Лицензия
Этот проект лицензирован под MIT License - см. файл LICENSE для деталей.
🆘 Поддержка

Если у вас возникли проблемы с запуском приложения:
    Проверьте, что Docker запущен
    Убедитесь, что порты 4050 и 5432 не заняты
    Проверьте логи: docker-compose logs
    Создайте issue в репозитории проекта

Примечание: Для production использования рекомендуется изменить настройки безопасности, пароли и переменные окружения.
