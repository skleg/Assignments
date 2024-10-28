# Cloud Sales System Technical Exercise #

This is a coding exercise (not pseudo coding). For each feature you have freedom to implement it in a way you think customer would mostly benefit from it. Commit the final solution to GitHub repository and share the link with us. The submitted code should be production ready.
For tech stack, please go with.Net and SQL.

## Case description – Introduction ##

Crayon wants to implement a solution for cloud sales. Crayon has a business partner, a Cloud Computing Provider (called CCP from now on). The CCP offers an API that Crayon can use to automate the business.
Solution should provide customers to buy and manage software solutions offered by CCP. Each purchased software is tied to a single account. Each customer can have multiple accounts.
Purchased software has a name (e.g. Microsoft Office), quantity (number of licenses, e.g. 5), state (active, etc.) and the valid to date (e.g. software license is valid until 31st of August, 2023.)

Create a Web API that supports following features:

- see the list of its own accounts
- see the list of software services available on CCP (you can mock HTTP calls to CCP and return hardcoded list of services)
- order software license through CCP (you can mock HTTP calls to CCP and return hardcoded list of services) for specific account
- see purchased software licenses for each account
- change quantity of service licenses per subscription
- cancel the specific software under any account
- extend the software license valid to date (e.g. 31st August -> 30th September)

## System configuration ##

The following application settings have to configured:

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
  - Integration test project
