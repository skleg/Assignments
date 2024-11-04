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
      "Issuer": "https://auth.playcon.se",
      "Audience": "https://playcon.se",
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

## Data migrations ##

- Migrations can be executed manually using the Persistence project

## Running manual tests ##

- Test data for Customers and Accounts can be generated using data seed. Uncomment the code in the ```DatabaseExtensions``` class.
- Use the Swagger in Authentication API to generate a token using and existing customer and configured password.
- Use the token to call the Sales API.

