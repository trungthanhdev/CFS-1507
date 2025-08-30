# CFS-1507 (.NET 9 Ecommerce API)

A backend RESTful API developed with **.NET 9**, serving as the foundation for a modern **ecommerce platform**. This project applies **Clean Architecture** and **Domain-Driven Design (DDD)** principles, ensuring **scalability**, **maintainability**, and **testability**.

---

## 👇 Visit Now

- 🌐 [Explore the Website](https://www.vietsnuts.vn/)
- 🌐 [Explore the APIs](https://api.vietsnuts.vn/swagger/index.html)

---

## 📌 Features

- Complete set of features for online purchasing:
  - Product and category management
  - User authentication and role-based access
  - Shopping cart & order processing
  - Email notifications
  - Support payment method: Momo
- Clean and modular DDD structure
- Layered Clean Architecture: `Domain → Application → Infrastructure → API`
- Minimal API design
- PostgreSQL integration via [Neon](https://neon.tech/)
- Docker-ready for deployment
- Optional authentication/authorization support

---

## ⚙️ Tech Stack

| Category          | Technology                      |
| ----------------- | ------------------------------- |
| **.NET SDK**      | .NET 9.0                        |
| **Architecture**  | Clean Architecture + DDD        |
| **Database**      | PostgreSQL (hosted on Neon)     |
| **ORM**           | Entity Framework Core           |
| **Platform**      | WebApp                          |
| **Hosting**       | Render                          |
| **Caching**       |                                 |
| **Media Storage** | Local Storage via Docker volume |
| **Email Service** |                                 |
| **CI/CD**         | GitHub Actions                  |

---

## 🚀 Getting Started

### 📦 Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- PostgreSQL connection string (Neon or local)
- (Optional) Redis, Cloudinary, SendGrid credentials
- Create `.env` in `/CFS-1507.Controller`
- Create `docker-compose.yml` in `/src`

---

### 🛠 Setup with VSCode

```bash
cd /src/CFS-1507.Infrastructure
dotnet restore
cd /src/CFS-1507.Controller
dotnet run
```

### 🛠 Setup with Docker

```bash
cd /src
docker compose up --build
```
