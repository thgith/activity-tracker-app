# Activity Tracker
Used to track how much time is spent on activities.

# Tech Stack
## Backend
- .NET Core 4
- Entity Framework (code-first)
- Postgres
- MSTest / Moq

## Frontend
- React
- Redux-Toolkit
- TypeScript
- Bootstrap
- SASS

## Other
- VS Code
- PGAdmin
- Postman
- Redux DevTools
- FireFox / Chrome / Edge
  
# How to Set Up
## 1. Set up appsettings.json
You can find this in `/Backend/ActivityTrackerApp` Replace the values in `{{}}` with your values. These values will probably be moved to environment variables later.
- `YOUR_POSTGRES_USER`
- `YOUR_POSTGRES_PASSWORD`
- `YOUR_RANDOM_JWT_SECRET_STRING`
```json
"ConnectionStrings": {
    "Postgres": "User ID={{YOUR_POSTGRES_USER}};Password={{YOUR_POSTGRES_PASSWORD}};Server=localhost;Port=5432;Database=postgres;Integrated Security=true;Pooling=true;"
},
"JwtConfig": {
    "Secret": "{{YOUR_RANDOM_SECRET_STRING}}",
    "Issuer": "",
    "Audience": ""
}
```
## 2. Set up migrations
Create migrations. I don't store the migrations right now since this doesn't have a stable release yet.
In `/Backend/ActivityTrackerApp`, run:
```bash
dotnet ef migrations add "NAME_OF_MIGRATION"
```
## 3. Apply the migrations to your database
```
dotnet ef database update
```

## 4. Start up backend API
In `/Backend/ActivityTrackerApp`
```bash
#Will start on port 7109
dotnet run
```
## 5. Start up frontend
In `/Frontend`
```bash
# Will start on port 3000
npm start 
```

You can now go to `localhost:3000` and test the application out.

# Features
- Login/registration with email and JWT token auth
- Add/edit a user
- Add/edit/delete an activity
- Manually add/edit/delete a session
- Add session with stopwatch
- Color picker for activity
- Activity filter on list page
- Tagging on activities

[Implementation Notes](/docs/ImplementationNotes.md)
[To Dos](/docs/Todos.md)