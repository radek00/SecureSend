
<h1 align="center">
  <br>
  <br>
  SecureSend
  <br>
</h1>

<h4 align="center">Simple file sharing solution with e2ee</h4>

<p align="center">
  <a href="#features">Features</a> •
  <a href="#run-with-docker">Run with Docker</a> •
  <a href="#building-from-source">Build from source</a> •
  <a href="#credits">Credits</a> •
  <a href="#license">License</a>
</p>

## About
SecureSend is a simple file sharing solution with end to end encryption and serves as an alternative to discontinued Firefox Send. It's easy to self-host and provides user friendly interface for sharing files. Each upload can have any number of files and generates a link to the list of files where each of them can be downloaded separately.

## Features

* End to end encryption
* Support for large files
* Set expiry date for upload
* Easy to self-host with Docker
* Upload pause/resume
* Password protected uploads
* Postgres and SqlServer support

## Run with Docker

Easiest way to run SecureSend is to use provided Docker Compose file. SecureSend comes with support for Postgres and SqlServer, simply uncomment options for the database of choice.

```yaml
version: '3.4'
volumes:
  secureSend:
networks:
  secureSend:
services:
  securesend:
    image: chupacabra500/secure_send:latest
    build:
      dockerfile: ./Dockerfile
    volumes:
      - secureSend:/app/files
    environment:
    #db provider
      - Database=Postgres
    #postgres options
      - PostgresOptions__Host=db
      - PostgresOptions__Password=example
      - PostgresOptions__UserId=postgres
      - PostgresOptions__Database=SecureSend

    #sql server options
      # - SqlServerOptions__Server=db
      # - SqlServerOptions__Port=1433
      # - SqlServerOptions__Database=SecureSend
      # - SqlServerOptions__TrustedConnection=False
      # - SqlServerOptions__UserId=SA
      # - SqlServerOptions__Password=YourStrong@Passw0rd
      # - SqlServerOptions__TrustServerCertificate=True
    ports:
      - 5000:5000
    networks:
      - secureSend
    depends_on:
      - db
  # db:
  #   image: "mcr.microsoft.com/mssql/server"
  #   environment:
  #     SA_PASSWORD: "YourStrong@Passw0rd"
  #     ACCEPT_EULA: "Y"
  #   ports:
  #     - 1433:1433
  #   networks:
  #     - secureSend
  db:
    image: postgres
    restart: always
    environment:
      POSTGRES_PASSWORD: example
    ports:
      - 5432:5432
    networks:
      - secureSend

```

## Building from source

In order to build SecureSend from source you'll need [Node.js](https://nodejs.org/en/download/), [.Net](https://dotnet.microsoft.com/en-us/) and [Postgres](https://www.postgresql.org/) or [SqlServer](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) instance.

### Build the project from source:
```bash
# Clone this repository
$ git clone https://github.com/radek00/SecureSend.git

# Go into the repository
$ cd SecureSend

# Build and run Asp.net Core project
$ cd SecureSend
$ dotnet watch -lp https

#install npm dependencies
$ cd ClientApp
$ npm install

# Run vue frontend
$ npm run dev
```

### Setup Postgres instance
Postgres can be run with the following Docker compose file:
```yaml
version: '3.4'
volumes:
  pgadmin:
services:
  postgres:
    image: postgres
    restart: always
    environment:
      POSTGRES_PASSWORD: example
    ports:
      - 5432:5432
  pgadmin:
    image: dpage/pgadmin4
    environment:
        PGADMIN_DEFAULT_EMAIL: admin@pgadmin.com
        PGADMIN_DEFAULT_PASSWORD: password
        PGADMIN_LISTEN_PORT: 80
    ports:
        - 15432:80
    volumes:
        - pgadmin:/var/lib/pgadmin
    depends_on:
        - postgres
```

### Adjust appsettings.json file
In order to conect with the Postgres instance appsettings.json options have to be adjusted. Simply change appropriate options accordingly.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Database": "Postgres",
  "SqlServerOptions": {
    "Server": "localhost",
    "Port": "1433",
    "Database": "SecureSend",
    "TrustedConnection": "False",
    "UserId": "SA",
    "Password": "YourStrong@Passw0rd",
    "TrustServerCertificate": "True"
  },
  "PostgresOptions": {
    "Host": "localhost",
    "Password": "example",
    "UserId": "postgres",
    "Database": "SecureSend",
    "Port": "5432"
  },
  "FileStorageOptions": {
    "Path": "D:\\SecureSendStorage"
  }
}
```

## Credits

This software is built with:

- [Node.js](https://nodejs.org/)
- [.Net](https://dotnet.microsoft.com/)
- [Vue](vuejs.org/)
- [Tailwind](tailwindcss.com/)
- [Flowbite](https://flowbite.com/)

## License

MIT

