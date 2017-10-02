# Bonavena
Bonavena is a Lightweight DAO implementation for .NET

Initialy developed to deliver DB communication wrapping ADO.NET to simplify data access, having simple configuration parameters and virtually no requirement of setting user permissions over the DB.
This project derives from an original source, called simply GenericDAO, born inside a project where users couldn´t be trusted with direct DB access to tables or query execution, so all comunication goes back and forth using optimized stored procedures and views.

The aim of getting the code open is to provide a lightweight solution, alternative to bigger and slightly more convoluted frameworks, for simple and clean communication.

We are currently in process of migrating to .NET Core

### Support
(NET.Core)
 - SQLServer

Oracle estimated to be working by late 2017 with the introduction of [ODP.NET](http://www.oracle.com/technetwork/topics/dotnet/tech-info/odpnet-dotnet-core-sod-3628981.pdf)

(.Net Framework 4.5)
 - SQLServer 
 - Oracle

### Roadmap
 - Full SQLDb integration with:
   + MySQL
   + SQLite
   + PostgreSQL
 - Migration to NET.Core
