CREATE TABLE [binding].[Binding]
(
	[PublisherInstanceID] INT NOT NULL	
	CONSTRAINT [FK1_binding.Binding_component.ApplicationInstance] FOREIGN KEY ([PublisherInstanceID]) REFERENCES [component].[ApplicationInstance]([InstanceID]), 
	[ConsumerInstanceID] INT NOT NULL
	CONSTRAINT [FK2_binding.Binding_component.ApplicationInstance] FOREIGN KEY ([ConsumerInstanceID]) REFERENCES [component].[ApplicationInstance]([InstanceID]),  
	PRIMARY KEY([PublisherInstanceID], [ConsumerInstanceID])
)
