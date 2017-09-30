## Bonavena
Bonavena is a Lightweight DAO implementation for .NET

Initialy developed to deliver DB communication wrapping ADO.NET to simplify data access, having simple configuration parameters and virtually no requirement of setting user permissions over the DB.
This project derives from an original source, called simply GenericDAO, born inside a project where users couldn´t be trusted with direct DB access to tables or query execution, so all comunication goes back and forth using optimized stored procedures and views.

The aim of getting the code open is to provide a lightweight solution, alternative to bigger and slightly more convoluted frameworks, for simple and clean communication.

We are currently in process of migrating to .NET Core

Currently supported Engines:
 - SQLServer 
Future: 
 - Oracle (estimated to be working by late 2017 with the introduction of ODP.NET)
 - MySQL
 - SQLite
 - PostgreSQL
