# Library Management System

A complete library system: a REST API for the catalog, borrowing and members, plus an **admin dashboard** (ASP.NET Core MVC) for the librarians.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-EF%20Core-CC2927)
![Auth](https://img.shields.io/badge/Auth-Identity%20%2B%20JWT-2f5d8a)
![Architecture](https://img.shields.io/badge/Architecture-Clean-b8893b)




## Features

- **Catalog**: books, authors and categories with search, sorting and pagination.
- **Accounts**: registration, login (JWT), and two roles: `Admin` and `Member`.
- **Borrowing**: borrow, return and extend loans, with due dates.
- **Fines**: calculated automatically on late return, paid at the library desk and marked as paid by an admin.
- **Reviews**: 1-5 star rating with a comment, only for books the member has borrowed.
- **Wishlist**: members save books for later.
- **Admin dashboard**: statistics, book/author/category management, overdue loans, unpaid fines, users and reviews moderation.

## Solution structure

```
LibraryDomain          Entities and contracts (repositories, unit of work, specifications)
LibraryApplication     Services, DTOs, AutoMapper profiles, specifications, business rules
LibraryInfrastructure  EF Core (DbContexts, repositories, migrations), Identity, JWT, data seeders
LibraryApi             REST API: controllers, middleware and configuration
Admin.Dashboard        Admin web dashboard (ASP.NET Core MVC, cookie authentication)
```

Both `LibraryApi` and `Admin.Dashboard` use the same Application and Infrastructure layers, so the business rules are written once.

### Two databases

| Database | DbContext | Contents |
|---|---|---|
| `Library` | `StoreDbContext` | Books, authors, categories, borrowings, reviews, wishlist |
| `LibraryIdentity` | `StoreIdentityDbContext` | Users, roles |

Identity lives in its own database, so the `UserId` columns in borrowings, reviews and the wishlist are plain strings with **no foreign key constraint**. User names are resolved through the identity service when needed.

## Getting started

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express or a full instance)
- `dotnet-ef` tool, only if you create new migrations: `dotnet tool install --global dotnet-ef`

### 1. Configure secrets (development)

Run from the `LibraryApi` folder. Secrets are stored outside the project and never committed.

```bash
dotnet user-secrets init
dotnet user-secrets set "JWT:SeceretKey" "<random string, at least 32 characters>"
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:Password" "<strong password>"
dotnet user-secrets set "SeedAdmin:DisplayName" "Library Admin"
```

Connection strings are in `appsettings.json` and point at a local SQL Server by default:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=Library;Trusted_Connection=True;TrustServerCertificate=True;",
  "IdentityConnection": "Server=.;Database=LibraryIdentity;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

`Admin.Dashboard` reads the same connection strings. Make sure they match the API's, so both apps use the same databases.

### 2. Run

```bash
# REST API
dotnet run --project LibraryApi

# Admin dashboard
dotnet run --project Admin.Dashboard
```

- Swagger UI: `https://localhost:7050/swagger` (development only)
- Dashboard: open the address printed in the console and sign in with the seeded admin account.
- On startup the app **applies pending migrations and seeds data automatically**. There is no need to run `Update-Database`.

### What gets seeded

| Data | Source | When |
|---|---|---|
| Roles `Admin` and `Member` | Code | If missing |
| Default admin account | `SeedAdmin:*` settings | If the email does not exist. If the settings are empty, no admin is created and a warning is logged |
| Authors, categories, books | `DataSeeder/*.json` | Only if the table is empty |

The JSON files are copied to the output folder at build time. In `Books.json`, `AuthorId` and `CategoryId` refer to the **position** of the author or category in the corresponding file (1 = first item).

## Admin dashboard

Only users with the `Admin` role can sign in.

| Section | What you can do |
|---|---|
| Dashboard | Totals for books, users and active loans, overdue loans, fines, and a category breakdown |
| Books | Search, filter by author/category, add, edit, delete, view details and moderate reviews |
| Authors / Categories | Add, edit and delete (blocked while they still have books) |
| Loans | All loans with filters, overdue loans, unpaid fines, return a book, mark a fine as paid |
| Users | View members and manage their roles |

## Configuration reference

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | Catalog database |
| `ConnectionStrings:IdentityConnection` | Identity database |
| `JWT:SeceretKey` | Signing key, at least 32 characters. The API refuses to start without it |
| `JWT:Issuer` / `JWT:Audience` | Token issuer and audience |
| `JWT:ExpirationMinutes` | Token lifetime (default 30) |
| `SeedAdmin:Email` / `Password` / `DisplayName` | Initial administrator |
| `Cors:AllowedOrigins` | Array of frontend origins allowed to call the API |

> The key names above are written exactly as they appear in the code.

## Deployment

1. Set `ASPNETCORE_ENVIRONMENT=Production`.
2. Provide settings as environment variables, using `__` instead of `:`:
   ```
   ConnectionStrings__DefaultConnection
   ConnectionStrings__IdentityConnection
   JWT__SeceretKey
   JWT__Issuer
   JWT__Audience
   SeedAdmin__Email
   SeedAdmin__Password
   Cors__AllowedOrigins__0      (add __1, __2 ... for more origins)
   ```
3. Use a strong `SeedAdmin__Password`. **There is no change-password endpoint yet** (see Roadmap), so choose the final password before the first start.
4. Serve over HTTPS. If the API runs behind a reverse proxy, configure forwarded headers so rate limiting sees the real client IP.
5. Make sure the SQL login can create databases (needed for the first migration), or create both databases beforehand.

## Migrations

Migrations are applied on startup. To create a new one:

```bash
# Catalog database
dotnet ef migrations add  --context StoreDbContext --project LibraryInfrastructure --startup-project LibraryApi

# Identity database
dotnet ef migrations add  --context StoreIdentityDbContext --project LibraryInfrastructure --startup-project LibraryApi --output-dir Migrations/Identity
```

Always open the generated file and check it before running the app.

## Authentication and roles

1. `POST /api/Authentication/Register` creates a **Member** account.
2. `POST /api/Authentication/Login` returns a JWT in the `token` field.
3. Send it on every protected request: `Authorization: Bearer <token>`.

| Capability | Guest | Member | Admin |
|---|:-:|:-:|:-:|
| Browse books, authors, categories, reviews | yes | yes | yes |
| Borrow, return, extend, view own loans | | yes | yes |
| Review books, manage own reviews | | yes | yes |
| Wishlist | | yes | yes |
| Create, edit, delete books, authors, categories | | | yes |
| View all loans, overdue loans, unpaid fines; mark fines paid | | | yes |
| Delete any review; assign roles; use the dashboard | | | yes |

Login and registration are rate limited per IP, and an account is locked for 15 minutes after 5 failed password attempts.

## API overview

Full details and request/response models are in Swagger. Dates are always **UTC**.

### Authentication (`/api/Authentication`)

| Method | Route | Access |
|---|---|---|
| POST | `/Register` | Public |
| POST | `/Login` | Public |
| GET | `/EmailExists?email=` | Public |
| GET | `/CurrentUser` | Authenticated |
| POST | `/assign-role` | Admin |

### Catalog (`/api/Books`, `/api/Authors`, `/api/Categories`)

| Method | Route | Access |
|---|---|---|
| GET | `/` | Public. Query: `pageIndex`, `pageSize` (max 50), `searchValue`, `sorting`. Books also accept `authorId`, `categoryId` |
| GET | `/{id}` | Public. A book also returns `averageRating` and `reviewsCount` |
| POST | `/` | Admin |
| PUT | `/{id}` | Admin |
| DELETE | `/{id}` | Admin |

Book search matches title, ISBN and publisher.

### Borrowings (`/api/Borrowings`)

| Method | Route | Access |
|---|---|---|
| POST | `/` body `{ "bookId": 1 }` | Authenticated |
| POST | `/{id}/return` | Owner or Admin |
| POST | `/{id}/extend` | Owner |
| GET | `/my-loans` | Authenticated |
| GET | `/admin/all` | Admin. Paginated; filters: `status` (`All`, `Active`, `Overdue`, `Returned`), `bookId`, `userId`, `searchValue`, `sorting` |
| GET | `/admin/overdue` | Admin |
| GET | `/admin/unpaid-fines` | Admin |
| PUT | `/{id}/pay-fine` | Admin |

### Reviews

| Method | Route | Access |
|---|---|---|
| GET | `/api/books/{bookId}/reviews` | Public |
| GET | `/api/books/{bookId}/reviews/mine` | Authenticated |
| GET | `/api/books/{bookId}/rating-summary` | Public |
| POST | `/api/books/{bookId}/reviews` | Authenticated |
| PUT | `/api/reviews/{reviewId}` | Review owner |
| DELETE | `/api/reviews/{reviewId}` | Review owner or Admin |

### Wishlist (`/api/Wishlist`, authenticated)

| Method | Route | Description |
|---|---|---|
| GET | `/` | My wishlist, with availability |
| GET | `/ids` | Only the book ids (for marking hearts in a list) |
| GET | `/{bookId}/exists` | Is this book in my wishlist? |
| POST | `/{bookId}` | Add |
| DELETE | `/{bookId}` | Remove |

## Business rules

**Borrowing**

- Loan period is 14 days. A member can hold at most 3 active loans.
- A book can be borrowed only if a copy is available, and not twice by the same member at the same time.
- A member with an overdue loan, or with an unpaid fine, cannot borrow new books.
- Loans can be extended a limited number of times, only before they are overdue and only with no unpaid fines.
- Extension limits and the daily fine rate are defined in `BorrowingConstants`.

**Fines**

- Calculated when a late book is returned: late days (any part of a day counts as a full day) multiplied by the daily rate.
- Paid in cash at the library desk. An admin then marks it as paid, and the payment time is recorded.

**Reviews and wishlist**

- Only members who borrowed a book can review it. One review per member per book; it can be edited or deleted. Rating is 1 to 5.
- A book can appear once in a member's wishlist.

**Data protection**

- A category or author that still has books cannot be deleted.
- A book that has borrowing history cannot be deleted.
- Changing a book's total copies keeps the number of borrowed copies consistent and is rejected if it would drop below the copies currently on loan.

## Error format

Errors are returned as `ProblemDetails`. The `title` field is a machine-readable code, `detail` is a readable message.

```json
{
  "status": 409,
  "title": "Borrowing.UnpaidFines",
  "detail": "You have unpaid fines. Please pay them at the library desk before borrowing new books."
}
```

| Status | Meaning |
|---|---|
| 400 | Validation error (invalid input, or a business limit such as the loan limit) |
| 401 | Missing, invalid or expired token |
| 403 | Authenticated but not allowed |
| 404 | Resource not found |
| 409 | Conflict with the current state (duplicates, business rules) |
| 500 | Unexpected server error (details are logged, not returned) |

Model validation errors (`400`) use the standard ASP.NET Core format with an `errors` object keyed by field.

Codes the frontend should handle explicitly:

| Code | Status | Meaning |
|---|---|---|
| `Borrowing.UnpaidFines` | 409 | Member must pay fines first |
| `Borrowing.OverdueLoansExist` | 409 | Member must return overdue books first |
| `Borrowing.LimitExceeded` | 400 | Already holding the maximum number of loans |
| `Borrowing.NoCopyAvailable` | 409 | No copies left |
| `Borrowing.AlreadyBorrowed` | 409 | Member already has this book |
| `Review.NotAllowed` | 403 | Member has not borrowed this book |
| `Review.AlreadyExists` | 409 | Member already reviewed this book |
| `Wishlist.AlreadyExists` | 409 | Book already in the wishlist |
| `Book.HasBorrowings`, `Category.HasBooks`, `Author.HasBooks` | 409 | Cannot delete |

Paginated endpoints return `{ pageSize, pageIndex, count, data }`, where `count` is the total number of matching items.

## Notes for frontend developers

- All dates are UTC. Convert to the user's local time for display.
- Send the token on every request except the public ones listed above.
- CORS only allows the origins listed in `Cors:AllowedOrigins`. Add the frontend's address there.
- Book cover images are served from `/Folder/...` on the API host.

## Roadmap

Not included in this version:

- Change password, forgot/reset password (requires an email service) and refresh tokens
- Online payment of fines (e.g. Stripe). Fines are currently settled at the desk
- Fine waivers and partial payments
- Book reservations (hold a copy for a member when it is returned)
- Searching loans by member name
- Pagination for the overdue, unpaid-fines and my-loans lists
- Caching of rarely changing data, if load requires it
