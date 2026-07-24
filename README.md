# Geekspace Learning System

Geekspace is an ASP.NET Core MVC learning platform focused on practical cybersecurity education. It combines database-backed content management, role-based access control, multimedia resources, member discussions, and browser-based interactive learning activities.

## Main Features

- Responsive public home page and learning library
- Keyword, topic, and resource-type filtering
- Article, video, simulation, self-assessment, and virtual-lab resources
- User registration, login, account management, and role-based authorisation
- Member comments and personal activity management
- Admin/Root CRUD management for resources and categories
- Admin/Root user and community activity management
- SQLite database connectivity through Entity Framework Core
- Client-side form validation and interactive learning feedback

## Interactive Learning

| Resource type | Interaction |
| --- | --- |
| Simulation | Three realistic phishing-email decisions with immediate explanations and a final score |
| SelfAssessment | Five TCP/IP questions with validation, answer review, and percentage result |
| VirtualLab | Six-step isolated-lab checklist with a progress bar and browser-local progress |
| Video | HTML5 video playback |
| Article | Structured reading content and supporting media |

Interactive results are processed in the browser. Assessment and simulation scores are not stored in the database. Virtual-lab checklist progress is stored in `localStorage` on the current browser only.

## Technology

- .NET 10 / ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- ASP.NET Core Identity
- SQLite
- Bootstrap 5
- HTML5, CSS, and JavaScript

## Run the Project

From PowerShell:

```powershell
cd "C:\Users\JD\Desktop\HTML_Assignment\DotNet-Website"
dotnet restore
dotnet build
dotnet run
```

Open the HTTP or HTTPS address displayed in the terminal. Press `Ctrl+C` to stop the application.

## Local Demonstration Accounts

| Role | Email | Password |
| --- | --- | --- |
| Root | `root@fosvcat.com` | `#Root123` |
| Admin | `admin@fosvcat.com` | `#Admin123` |
| Member | `User@fosvcat.com` | `#User123` |

These credentials are intended only for the local assignment demonstration. Replace or remove them before any public deployment.

## Database

The SQLite connection string is configured in `appsettings.json`:

```json
"DefaultConnection": "DataSource=app.db;Cache=Shared"
```

The local `app.db` file contains:

- `Categories`
- `LearningResources`
- `ResourceComments`
- ASP.NET Identity user, role, claim, login, and token tables

Entity Framework migrations are stored under `Data/Migrations`.

To apply migrations:

```powershell
dotnet ef database update
```

To inspect the database visually, stop the application and open `app.db` with DB Browser for SQLite.

## Role Permissions

### Visitor

- Browse published resources
- Search and filter the library
- View multimedia and interactive learning content

### Member

- All visitor features
- Post and delete personal comments
- View and manage personal activity

### Admin

- Manage resources and categories
- Moderate community activity
- Manage ordinary users

### Root

- All administrator capabilities
- Manage administrator accounts
- Protect Root-owned content from lower-level administration

## Assignment Requirement Coverage

| Requirement | Implementation |
| --- | --- |
| Interlinked pages and navigation | Responsive navigation, breadcrumbs, library, topics, account, and admin pages |
| HTML5 and multimedia | Semantic layout, figures, images, forms, and HTML5 video |
| CSS usage | External site stylesheet, responsive design, theme variables, and Bootstrap |
| Database connectivity | EF Core with local SQLite database |
| Insert, display, update, delete | Resource and category CRUD plus comments and user administration |
| Registration and member module | ASP.NET Identity registration, login, account, comments, and activity |
| Administrator module | Role-protected content, user, and activity management |
| Form validation | Data annotations, ModelState validation, unobtrusive client validation, and JavaScript checks |
| Client/server processing | Razor/EF Core server processing plus interactive JavaScript modules |

## Important Submission Notes

- Stop the application before copying or inspecting `app.db`.
- Do not submit temporary `app.db-wal` or `app.db-shm` files.
- Verify that all media under `wwwroot/media` is included.
- Run `dotnet build` and confirm there are no errors before submission.
- The written proposal and final report are separate assignment deliverables.
