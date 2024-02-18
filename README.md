
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
* Ability to set expiration date for uploads
* Easy to self-host with Docker
* Upload pause/resume
* Password protected uploads
* Configurable upload size and expiration limits
* Postgres and SqlServer support

## Run with Docker

Easiest way to run SecureSend is to use provided Docker Compose file. Below config shows configuration with Postgres, for SqlServer config refer to this [compose](https://github.com/radek00/SecureSend/blob/master/docker-compose-sqlserver.yml) file

```yaml
version: '3.4'
volumes:
  secureSend:
networks:
  secureSend:
services:
  securesend:
    image: chupacabra500/secure_send
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
      #Optional upload limits options. Defaults to no limit if not provided.
      - FileStorageOptions__TotalUploadLimitInGB=10
      - FileStorageOptions__SingleUploadLimitInGB=5
      - FileStorageOptions__MaxExpirationInDays=3
    ports:
      - 8080:80
    networks:
      - secureSend
    depends_on:
      - db
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
**IMPORTANT**: SecureSend uses Web Crypto APIs to provide encryption features, which are disallowed by modern browsers in insecure contexts. In this case it's likely that you will see an error like: `Cannot read property 'importKey'`. In order to prevent that SecureSend must be accesed via localhost or HTTPS. You can set it up using various reverse proxies like Traefik or Caddy. Below Docker Compose file shows an example Traefik configuration with DNS Challenge.

```yaml
version: '3.4'
networks:
  proxy:
  secureSend:
volumes:
  secureSend:
services:
  #reverse proxy
  traefik:
    image: "traefik:v2.4"
    container_name: "traefik"
    restart: unless-stopped
    command:
      - "--log.level=DEBUG"
      - "--api.insecure=true"
      - "--providers.docker=true"
      - "--providers.docker.exposedbydefault=false"
      - "--entrypoints.web.address=:80"
      - "--entrypoints.websecure.address=:443"
      - "--entrypoints.dns.address=:53"
      - "--entrypoints.udpdns.address=:53/udp"
      - "--entrypoints.web.http.redirections.entryPoint.to=websecure"
      - "--entrypoints.web.http.redirections.entryPoint.scheme=https"
      - "--entrypoints.web.http.redirections.entrypoint.permanent=true"
      - "--entrypoints.https.http.tls.certresolver=myresolver"
      - "--entrypoints.https.http.tls.domains[0].main=${BASE_DOMAIN}"
      - "--entrypoints.https.http.tls.domains[0].sans=*.${BASE_DOMAIN}"
      - "--certificatesresolvers.myresolver.acme.dnschallenge=true"
      - "--certificatesresolvers.myresolver.acme.dnschallenge.provider=cloudflare"
      - "--certificatesresolvers.myresolver.acme.email=${API_EMAIL}"
      - "--certificatesresolvers.myresolver.acme.storage=/letsencrypt/acme.json"
      - "--certificatesresolvers.myresolver.acme.dnschallenge.resolvers=1.1.1.1:53,8.8.8.8:53"
      - "--certificatesresolvers.myresolver.acme.dnschallenge.delaybeforecheck=90"
    ports:
      - "80:80"
      - "443:443"
      - "8080:8080"
    environment:
      - "CF_API_EMAIL=${API_EMAIL}"
      - "CF_API_KEY=${API_KEY}"
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.traefik.rule=Host(`traefik.${BASE_DOMAIN}`)"
      - "traefik.http.routers.traefik.entrypoints=websecure"
      - "traefik.http.routers.traefik.tls.certresolver=myresolver"
      - "traefik.http.routers.traefik.service=api@internal"
      - "traefik.http.services.traefik.loadbalancer.server.port=8080"
    networks:
      proxy:
    volumes:
      - "/var/run/docker.sock:/var/run/docker.sock:ro"
      - "./letsencrypt:/letsencrypt"
  securesend:
    image: chupacabra500/secure_send:latest
    volumes:
      - secureSend:/app/files
    environment:
      #db provider
      - Database=Postgres
      #postgres options
      - PostgresOptions__Host=db
      - PostgresOptions__Password=${POSTGRES_PASSWORD}
      - PostgresOptions__UserId=${POSTGRES_USER}
      - PostgresOptions__Database=SecureSend
      #Optional upload limits options. Defaults to no limit if not provided.
      - FileStorageOptions__TotalUploadLimitInGB=10
      - FileStorageOptions__SingleUploadLimitInGB=5
      - FileStorageOptions__MaxExpirationInDays=3
    networks:
      - secureSend
      - proxy
    labels:
      - traefik.enable=true
      - traefik.http.services.securesned.loadbalancer.server.port=80
      - traefik.http.routers.securesend.rule=Host(`securesend.${BASE_DOMAIN}`)"
      - traefik.http.routers.securesend.tls.certresolver=myresolver
      - traefik.http.routers.securesend.entrypoints=websecure
      - traefik.docker.network=proxy
    depends_on:
      - db
  db:
    image: postgres
    restart: always
    environment:
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    ports:
      - 5432:5432
    networks:
      - secureSend
      - proxy
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

