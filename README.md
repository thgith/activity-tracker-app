# Activity Tracker
Used to track how much time is spent on activities.

# Tech Stack
## Backend
- .NET Core
- Entity Framework (code-first)
- Postgres

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
  
# How to Set Up
## Set up appsettings.json
Replace the values in `{{}}` with your values
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
## Set up database
Create migrations. I don't store the migrations right now since this doesn't have a stable release yet.
```
dotnet ef migrations add "NAME_OF_MIGRATION"
```
## Apply the migrations to your database
```
dotnet ef database update
```

## Start up backend API
```bash
cd Backend
#Will start on port 7109
dotnet run
```
Start up frontend
```bash
cd Frontend
# Will start on port 3000
npm start 
```

Backend (on port 7109)

Frontend (on port 3000)
npm start

# Features
- Login/registration with email and JWT token auth
- Add/edit a user
- Add/edit/delete an activity with color picker
- Manually add/edit/delete a session
- Add session with timer
- Color picker for activity
- Activity filter
- Tagging on activities

- [To Dos](/docs/TODO.md)