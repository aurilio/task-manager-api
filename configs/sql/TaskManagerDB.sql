-- Verifica se o banco de dados existe, se não existir, cria-o
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TaskManagerDB')
BEGIN
    EXEC sp_executesql N'CREATE DATABASE TaskManagerDB';
END
GO

-- Seleciona o banco de dados para uso
USE TaskManagerDB;
GO

-- Cria a tabela Task refletindo a classe TaskEntity
IF OBJECT_ID('dbo.Task', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Task;
END
GO

CREATE TABLE dbo.Task (
    Id UNIQUEIDENTIFIER NOT NULL,
    Title VARCHAR(100) NOT NULL,
    Description VARCHAR(200) NOT NULL,
    IsCompleted BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT PK_Task PRIMARY KEY (Id)
);
GO
