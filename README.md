# SkillSwap

SkillSwap is a .NET 10 peer-to-peer learning platform for college students. Students exchange one-hour learning sessions using SkillCredits rather than money.

## Features

- Browse and search student skill listings.
- Request, accept, reject, cancel, deliver, and confirm one-hour sessions.
- Reserve credits at acceptance and release or transfer them exactly once.
- Reviews are available only after learner-confirmed completion.
- SQL Server persistence through EF Core, seed data, REST API, and Razor MVC frontend.
- Three SQL Server reporting datasets for SSRS / Microsoft Report Builder.

## Architecture

- **SkillSwap.Domain**: entities and enums with no infrastructure dependencies.
- **SkillSwap.Application**: DTOs, contracts, business rules, and asynchronous booking workflows.
- **SkillSwap.Infrastructure**: EF Core SQL Server DbContext, repository, configuration, and seed data.
- **SkillSwap.API**: REST controllers and centralized exception handling.
- **SkillSwap.Web**: MVC/Razor UI using a typed `HttpClient`; it does not access EF Core.
- **SkillSwap.Tests**: xUnit business workflow tests.

## Setup

1. Install the .NET 10 SDK and SQL Server LocalDB (or change the API connection string to SQL Server Express).
2. From the solution directory run:
   ```powershell
   dotnet restore
   dotnet ef migrations add InitialCreate --project SkillSwap.Infrastructure --startup-project SkillSwap.API
   dotnet ef database update --project SkillSwap.Infrastructure --startup-project SkillSwap.API
   dotnet build
   dotnet test
   ```
3. Start the API with `dotnet run --project SkillSwap.API`.
4. Start the MVC site with `dotnet run --project SkillSwap.Web` in a second terminal. Set `SkillSwap.Web:Api:BaseUrl` to the API HTTPS URL if the launch port differs from `https://localhost:7001/`.

The API does not automatically delete, reset, or recreate an existing database. Seed data includes six demo users, six categories, twelve listings, and wallets with the initial three-credit grant. The initial migration must be applied before first use.

## Demo mode and security limitation

This MVP is designed for classroom demonstration. API workflow actions receive a user ID in the query/body, which is **not authentication** and must not be used in production. A production deployment must replace this with ASP.NET Core Identity, claims-based ownership checks, CSRF protections for the API client, and proper account management. Do not expose demo-user switching publicly.

## Business rules

All persisted timestamps are UTC. A new user receives three credits; sessions cost one credit. Acceptance requires available credits and checks both participants for overlapping confirmed bookings. Cancellation before start releases a reservation. Learner confirmation changes `AwaitingConfirmation` to `Completed` and transfers one reserved credit to the teacher. Unique indexes prevent duplicate emails, wallets, and reviews per booking/reviewer.

## Testing

`dotnet test` currently verifies eight tests, including seven meaningful booking workflow scenarios. Tests use a fake repository to isolate application logic from EF Core.

## Reporting

See `Reports/README.md` and the three `.sql` files. They are SQL Server-compatible parameterized datasets. Report Builder layout and `.rdl` preview require manual creation in Microsoft Report Builder; that manual step is intentionally not claimed as automated.

## Demo walkthrough

1. Start LocalDB, apply the migration, then run API and Web.
2. Open the Web home page and show the SkillCredits explanation.
3. Open Explore Skills and search for `Python`.
4. Open a listing and explain the one-hour/one-credit rule.
5. Use the API or future dashboard forms to create a future booking as a different demo learner.
6. Accept it as the listing teacher and show the learner wallet reservation.
7. Mark delivered, confirm completion as the learner, and show the transfer and Completed status.
8. Submit one review and show that a second review is rejected.
9. Open the Reports folder and demonstrate the three datasets and date parameters in Report Builder.

## Known limitations and next work

- Full dashboard, wallet, profile, booking-management forms, and real authentication remain planned UI expansion; the API workflow surface and skill discovery UI are implemented.
- EF migration files must be generated with the EF CLI in an environment where the .NET 10 tool is installed.
- `.rdl` layouts require manual Report Builder design as documented above.

## Team contributions

- Student 1: ____________________
- Student 2: ____________________
- Student 3: ____________________

## AI assistance acknowledgement

AI assistance was used for scaffolding and implementation suggestions. The project team remains responsible for reviewing, testing, securing, and presenting the final code.
