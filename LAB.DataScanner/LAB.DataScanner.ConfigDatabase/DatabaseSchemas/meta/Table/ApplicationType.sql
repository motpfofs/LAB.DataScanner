CREATE TABLE [meta].[ApplicationType] (
	[TypeID] INT IDENTITY (1,1) NOT NULL,
	TypeName NVARCHAR(50) NOT NULL,
	TypeVersion NVARCHAR(12) NOT NUll,
	ConfigTemplateJson NVARCHAR(MAX) NOT NULL, 
	CONSTRAINT PK_ApplicationType_TypeID PRIMARY KEY CLUSTERED ([TypeID])
);