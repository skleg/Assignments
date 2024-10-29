# System configuration #

The following application settings have to be configured:

- *ConnectionStrings*
  - *SalesDb* - Cloud Sales database connections string
- *Jwt* - bearer token configuration
  - *Key* - signing key
  - *Issuer* - token issuer
  - *Audience* - token audience
  - *Password* - login password for users for token generation (optional)

The settings can be configured via:

- app.settings file
- environment variables
- user secrets

Example for user secrets:
```
{
    "ConnectionStrings": {
      "SalesDb": "Server=localhost,1433;Database=CloudSales;User Id=<user-name>;Password=<password>;Encrypt=True;TrustServerCertificate=True;"
    },
    "Jwt": {
      "Key": "very-secret-key-do-not-use-in-production",
      "Issuer": "https://auth.crayon.se",
      "Audience": "https://crayon.se",
      "Password": "Test1234!"
    }
}
```

## Solution structure ##

- *src*
  - Cloud Sales API
- *auth*
  - Authentication server that can be used to generate access tokens. Authentication uses customers from the Cloud Sales database.
- *test*
  - Unit test project
  - Integration test project (requires Docker)
- *docs*
  - Flow charts and diagrams

## ToDo ##

- Implement Cloud Service
  - **Current implementation is a mock**
  - Create an implementation with an Http client
    - Configure the endpoint
    - Add authentication, resilience and caching

## Improvements ##

- Identity field may be converted into UUID v7 (ULID or GUIDv7 in .Net 9)
- Fluent validation
- Output caching
- Resilience
