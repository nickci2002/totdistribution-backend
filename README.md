# totdistribution-backend
Backend for TM2020 Track of the Day statistics website written in .NET 10.
This project is being actively worked on as a full-stack solution with plans to deploy to the Azure cloud.

## Tools
- .NET 10 (Framework the backend is written in)
- SQL Server 2025 (Database for interacting with the frontend)
- RedisDB (Temporary storage for in-progress Track of the Day data)

## Features
There are two .NET projects in this solution:
1. NadeoRefinery: A background microservice, using VSA architecture, that communicates with Nadeo's servers to obtain necessary data and processes them for permanent storage in our main database. It runs on a set scheduler using the Quartz.NET scheduler and stores data temporarily in a Redis database.
1. CoreWebAPI: Our main backend, using VSA architecture, that houses our SQL Server database for permanent storage. It will allow queries from our frontend and commands from our NadeoRefinery.
