# Magnifier

## Instructions
The Magnifier backend is a C# ASP.NET Core Web API located in `Magnifier`
Spyglass, the frontend, is a C# ASP.NET Core Blazor WASM app located in `Spyglass`

## Config
The backend needs a config file to function. Create `Magnifier/appsettings.json` and put in it this:
```
{
    "JwtAuthSettings": {
        "PrivateKey": "<super secret private key for signing jwt auth tokens>",
        "Issuer": "https://localhost:5000",
        "Audience": "https://localhost:5000",
        "LifetimeDays": 90
    },
    "MongoDBSettings": {
        "ConnectionString": "<super secret mongodb connection string>",
        "DatabaseName": "Magnifier"
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Information"
        }
    },
    "AllowedHosts": "*"
}

```
The `Issuer` and `Audience` fields don't really matter because tokens are validated using their signature but you can, and should, set them to your domain if you want.

## Running
To run the Magnifier backend, just open in Visual Studio, select "Magnifier" from the project select dropdown and from the build type select dropdown. A MongoDB instance and a private key are required.

To run the Spyglass frontend, open `Spyglass` in a terminal window then use `dotnet watch -p Spyglass.csproj run` to run and watch.
