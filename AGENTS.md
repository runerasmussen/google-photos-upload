# AI Agent Instructions for google-photos-upload

## Project type
- .NET 9 console application with a separate MSTest project.
- Solution: `src/google-photos-upload.sln`
- Main app project: `src/google-photos-upload/google-photos-upload.csproj`
- Test project: `src/google-photos-upload.test/google-photos-upload.test.csproj`

## Build and test commands
- Restore and build: `dotnet restore` then `dotnet build --no-restore` from `src/`.
- Run tests: `dotnet test --no-build --verbosity normal` from `src/`.
- Publish artifacts: `dotnet publish --configuration Release /p:PublishProfile=Publish-win-x64.pubxml` or `Publish-portable.pubxml` from `src/google-photos-upload`
- GitHub Actions workflow: `.github/workflows/dotnet.yml`

## Architecture and key files
- `src/google-photos-upload/Program.cs` configures dependency injection and starts the app.
- `src/google-photos-upload/App.cs` parses CLI arguments, shows help, and routes commands to the upload service.
- `src/google-photos-upload/Services/UploadService.cs` contains the upload and album logic.
- `src/google-photos-upload/Services/AuthenticationService.cs` handles Google API authentication.
- `src/google-photos-upload/Services/IUploadService.cs` and `IAuthenticationService.cs` define service contracts.

## Important conventions and implementation notes
- The CLI uses `Mono.Options` for argument parsing.
- Logging is configured with `Microsoft.Extensions.Logging` and `NLog`.
- `client_secret.json` and `nlog.config` are copied to the output directory and should remain in the main app project.
- This is a hobby utility, so keep changes small and consistent with the existing CLI-focused design.

## Documentation links
- Main repo documentation: `README.md`
- Usage guide: `docs/USER_GUIDE.md`
- Google Photos permissions notes: `docs/google-photos-permissions.md`

## What AI agents should do first
- Inspect `src/google-photos-upload/App.cs` and `src/google-photos-upload/Services/UploadService.cs` before editing upload or command behavior.
- Preserve the app's interactive fallback when `command=0` or no valid command is provided.
- Use the existing `.github/workflows/dotnet.yml` as the source of truth for CI expectations.

## When to ask for clarification
- If a task requires changing authentication scopes or Google API behavior.
- If the request affects publish packaging or the `Publish-*.pubxml` profiles.
- If a change would require new external secrets or local credentials.
