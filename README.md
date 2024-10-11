Grubly
======

> **Grubly** is an ASP.NET MVC web application for managing recipes, ingredients, and categories with ease. Built with .NET Core, the app is deployed on Azure and incorporates a CI/CD pipeline using GitHub Actions for streamlined deployment and testing.

Table of Contents
-----------------

-   [Overview](#overview)
-   [Features](#features)
-   [Tech Stack](#tech-stack)
-   [Getting Started](#getting-started)
-   [Configuration](#configuration)
-   [Testing](#testing)
-   [Deployment](#deployment)

* * * * *

Overview
--------

**Grubly** simplifies recipe management by allowing users to add, view, and filter recipes, ingredients, and categories. The application supports user authentication, model validation, and a clean UI. With an Azure-backed infrastructure and automated testing through GitHub Actions, Grubly is built to be scalable and user-friendly.

Features
--------

-   **Recipe Management**: Add, update, delete, and view recipes with details.
-   **Ingredient and Category Filtering**: Sort recipes by ingredients or categories for easy navigation.
-   **User Authentication**: Secure access with role-based controls.
-   **Responsive UI**: Optimized layout for desktop and mobile.
-   **Automated CI/CD Pipeline**: GitHub Actions run tests and deploys automatically to Azure.

Tech Stack
----------

-   **Frontend**: ASP.NET MVC, Razor, TailwindCSS
-   **Backend**: .NET Core, Entity Framework Core
-   **Database**: Azure SQL Database
-   **Deployment**: Azure App Service, GitHub Actions

Getting Started
---------------

### Prerequisites

1.  **.NET 6** - [Install from Microsoft](https://dotnet.microsoft.com/download/dotnet/6.0)
2.  **Node.js** (for frontend dependencies) - [Install from Node.js](https://nodejs.org/)
3.  **Azure CLI** (for Azure deployment) - [Install Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)

### Installation

1.  **Clone the repository:**

    ```bash

    git clone https://github.com/Freeman-md/Grubly.git
    cd Grubly
    ```

2.  **Install dependencies:**

    ```bash

    dotnet restore
    npm install  # if Node.js dependencies are required
    ```

3.  **Setup Database Migrations** (optional):

    ```bash

    dotnet ef database update
    ```

4.  **Run the Application:**

    ```bash

    dotnet run
    ```
    

Configuration
-------------

### AppSettings Configuration

Update the `appsettings.json` for local development and `appsettings.Production.json` for production.

#### Sample Configuration

```json

{
  "ConnectionStrings": {
    "DefaultConnection": "YourAzureSQLDatabaseConnectionString"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables

Set the `ASPNETCORE_ENVIRONMENT` to `Development` for local debugging or `Production` for live.

Testing
-------

### Running Tests Locally

Tests are located in the `Grubly.Tests` project and can be run as follows:

```bash

dotnet test
```

### GitHub Actions

Grubly has a GitHub Actions workflow to automatically build and test code on every push. See the `.github/workflows` directory for the CI/CD pipeline configuration.

Deployment
----------

Deploying Grubly to Azure App Service can be done in two ways: via **Azure CLI** or **GitHub Actions**.

Checkout my [Blog Post](https://freemancodz.hashnode.dev/end-to-end-guide-to-building-deploying-and-securing-a-net-app-with-azure-app-service#heading-method-2-using-the-azure-portal) on deploying .NET applications in Azure App Service for more info.
