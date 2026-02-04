# URL Shortener API

A URL shortener built with **ASP.NET Core 8**, **PostgreSQL**, and **Redis**, featuring
Redis-backed caching and rate limiting, fully containerized with Docker Compose.

---

## 🚀 Features

- Shorten long URLs
- Fast redirects with Redis cache
- Redis-based rate limiting middleware
- PostgreSQL persistence
- Minimal API architecture
- Configurable via environment variables
- Docker Compose setup for local development

---

## 🏗 Tech Stack

- **ASP.NET Core 8 (Minimal API)**
- **PostgreSQL 16**
- **Redis 7**
- **StackExchange.Redis**
- **Entity Framework Core**
- **Docker & Docker Compose**

---

## 📦 Prerequisites

- Docker
- Docker Compose

---

## ▶️ Running the project

```bash
docker compose up --build
