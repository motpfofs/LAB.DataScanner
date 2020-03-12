CREATE TABLE [component].[ApplicationInstance]
(
	[InstanceID] INT IDENTITY (1,1) NOT NULL CONSTRAINT [PK_component.ApplicationInstance] PRIMARY KEY CLUSTERED ([InstanceID]), 
    [TypeID] INT NOT NULL 
	CONSTRAINT [FK_component.ApplicationInstance_meta.ApplicationType] FOREIGN KEY ([TypeID]) REFERENCES [meta].[ApplicationType]([TypeID]),
	[InstanceName] NVARCHAR(50) NOT NULL,
	[ConfigJson] NVARCHAR(MAX) NOT NULL, 		
)
