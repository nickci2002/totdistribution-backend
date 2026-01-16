# totdistribution-backend
Backend for TM2020 Track of the Day statistics website written in .NET 10, using Vertical Slice Architecture.
This project is currently un

## Tools
- .NET 10 (Framework the backend is written in)
- SQL Server 2025 (Database for interacting with the frontend)
- RedisDB (Temporary storage for in-progress Track of the Day events)

## Features
There are two parts to this program.
    1. The NadeoCommunicator: Gets necessary data from Nadeo's official servers and processes it for our main database. Temporary values are stored in a Redis database. It only performs create, update, and delete operations via various commands. 
    2. The Web API: Communicates with the frontend to get TOTD information from our SQL Server database. It only performs read operations via various queries.