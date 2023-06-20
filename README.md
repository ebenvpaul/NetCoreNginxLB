# NetCoreNginxLB

This repository contains the source code and configuration files for My Application. It is a web application built with .NET Core, Docker, Redis, and NGINX.

## Prerequisites

Before running the application, make sure you have the following dependencies installed:

- [.NET Core 7 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- Docker Compose

## Installation

1. Clone this repository to your local machine:

```bash
git clone https://github.com/ebenvpaul/NetCoreNginxLB.git
````
2.  Change to the project directory:

 ```bash
 cd NetCoreNginxLB
 ````
3. Build and run the application using Docker Compose:

 ```bash
  docker-compose up --scale api=4 --build
 ````
4. Once the containers are up and running, you can access the application at http://localhost:8090 in your web browser.
 ```bash
  http://localhost:8090/
 ````
5. Run the endpoint to hit the controller 
 ```bash
 http://localhost:8090/weatherforecast
 ````

## License

This project is licensed under the [MIT License](LICENSE).


## Acknowledgements

Thank you all !!!
   
