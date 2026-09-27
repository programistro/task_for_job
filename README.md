```bash
docker compose up -d
```

Поднимутся три контейнера: API, PostgreSQL 16 и pgAdmin. Миграции применяются
автоматически при старте API.

| Сервис    | Адрес                   | Порт |
|-----------|-------------------------|------|
| API       | http://localhost:8090   | 8090 |
| pgAdmin   | http://localhost:8080   | 8080 |
| PostgreSQL | localhost:5433          | 5433 |

## Swagger

<http://localhost:8090/swagger>

## pgAdmin

<http://localhost:8080>

## Пароль для для pgadmin "Abc#1234"
