# SeminarIntegration

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [EF Core CLI Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) (install using `dotnet tool install --global dotnet-ef`)

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/murageh/SeminarApp-v1-NET-Core.git cd SeminarIntegration
```

### 2. Install Dependencies

Restore the .NET project dependencies:

```bash
dotnet restore
```

### 3. Configure the Database

Ensure that your database settings are correctly configured in the `appsettings.json` file.

```json
{
  // ...
  "DbSettings": {
	"ConnectionString": "Server=localhost;Database=SeminarIntegration;User Id=sa;Password=your_password;"
	// "ConnectionString": "Server=localhost;Database=SeminarIntegration;Integrated Security=true;TrustServerCertificate=true;" // For Windows Authentication
  }
  // ...
}
```

Additionally, you need to configure Microsoft BC settings. Make sure to add the following to the `appsettings.json` file:
> Note the underscore at the end of the PORTALCODEUNIT value. This is important as it is used to append the function name to the URL.
```json
{
  // ...
  "AppSettings": {
        "W_USER": "your_username_",
        "W_PWD": "your_password",
        "DOMAIN": "your_domain_",
        "UseWindowsAuth": true, // Set to false if you are not using Windows Authentication. If set to false, you need to provide the above username, password, and domain settings.
        "BCOMPANY": "CRONUS International Ltd.", // Or your company name
        "PORTALCODEUNIT": "http://<deviceName>:<port>/<BC_Instance>/ODataV4/<Exposed_Webservice_Name>_" // e.g. http://desktop-v789:7048/BC240/ODataV4/MyWebservice_
    }
  // ...
}
```

### 4. Create and Apply Migrations

#### Create a Migration

To create a new migration, run the following command:

```bash
./create-migration.sh 
```
or (for Windows)

```bash
.\create-migration.bat
```

#### Apply Migrations

To apply the migrations to the database, run the following command:

```bash
./apply-migrations.sh
```
or (for Windows)

```bash
.\apply-migrations.bat
```


### 5. Configure Authentication

This application uses JWT authentication. You need to configure the JWT settings in the `appsettings.json` file:
```json
{
  // ...
  ""AuthSettings": {
        "SecretKey": "THIS_IS_MY_AUTHENTICATION_SECRET!!",
        "Issuer": "http://localhost",
        "Audience": "http://localhost"
    },
// ...
}
```

You should adjust the `SecretKey`, `Issuer`, and `Audience` values to match your application's requirements.


### 6. Run the Application

Run the application using the following command:

```bash
dotnet run
```

The application should now be running at `https://localhost:5001` (or the port specified in your configuration).

### 6. Access Swagger UI

If you are in a development environment, you can access the Swagger UI to explore the API endpoints:

```
https://localhost:5001/swagger
```


## Additional Information

- For more information on configuring Swagger/OpenAPI, visit [Microsoft's documentation](https://aka.ms/aspnetcore/swashbuckle).
- For more details on EF Core migrations, refer to the [EF Core documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/).

## Troubleshooting

If you encounter any issues, please check the following:

- Ensure that the .NET 8 SDK is installed and properly configured.
- Verify that the database settings in `appsettings.json` are correct.
- Check the logs for any error messages and consult the documentation or seek help from the community.

Feel free to open an issue if you need further assistance.
