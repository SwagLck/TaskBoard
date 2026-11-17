# TaskBoard

A simple app for managing projects and tasks, a bit like a small version of Jira.

You can create an account, make projects, add tasks to them, change a task's status (To Do, In Progress, Done), assign tasks to people and write comments.

I built this project to practice full stack development and to learn how to structure a backend properly.

## What I used

- **Backend:** ASP.NET Core (C#)
- **Database:** SQL Server (running in Docker)
- **Frontend:** React with TypeScript

## How it works

The backend is split into four projects, each with its own job:

- **Domain:** the basic data classes (User, Project, Task, Comment)
- **Application:** the main logic and rules of the app
- **Infrastructure:** talks to the database and handles passwords and login tokens
- **Api:** receives requests from the frontend and sends back responses

I did it this way so the logic is not tied to the database or the web framework, which makes the code easier to change later.

## What the app can do

- Register and log in
- Create, edit and delete projects (you only see your own)
- Add tasks, change their status, assign them and delete them
- Filter tasks by status and browse them page by page
- Comment on tasks

## How to run it

You need: .NET SDK, Node.js, and Docker Desktop.

**1. Start the database**

```powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<your-password>" -p 1433:1433 --name taskboard-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

Next time you only need `docker start taskboard-sql`.

**2. Set up the secrets**

Passwords and keys are not stored in the code. They are saved on your own computer with .NET User Secrets:

```powershell
dotnet user-secrets init --project src/TaskBoard.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" 'Server=localhost,1433;Database=TaskBoardDb;User Id=sa;Password=<your-password>;TrustServerCertificate=True;' --project src/TaskBoard.Api
dotnet user-secrets set "Jwt:Secret" "<any-long-random-text-at-least-32-characters>" --project src/TaskBoard.Api
```

**3. Create the tables and start the API**

```powershell
dotnet ef database update --project src/TaskBoard.Infrastructure --startup-project src/TaskBoard.Api
dotnet run --project src/TaskBoard.Api
```

The console will show the address the API runs on

**4. Start the frontend**

```powershell
cd client
npm install
copy .env.example .env
```

Open `client/.env` and put the API address there:

```
VITE_API_URL=http://localhost:5206/api
```

Then run:

```powershell
npm run dev
```

Open the given localhost address in your browser.

## Things I did on purpose

- Passwords are never saved as plain text. They are hashed first.
- Secrets are not in Git. Settings that change between computers live in configuration files, not in the code.
- The frontend never talks to the database* All rules and permission checks happen in the backend.
- Errors are handled in one place, so the API always answers in the same format.

## What is missing

- Automated tests
- Sharing a project with other people (right now only the owner can see it)
- Drag and drop for tasks on the board
