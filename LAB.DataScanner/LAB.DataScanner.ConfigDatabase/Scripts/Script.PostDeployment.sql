/*
Add sample data to Type Table
*/

DECLARE @meta_type_sample_data TABLE
(
	TypeID INT NOT NULL PRIMARY KEY,
	TypeName NVARCHAR(50) NOT NULL,
	TypeVersion NVARCHAR(12) NOT NULL,
	ConfigTemplateJson NVARCHAR(MAX) NOT NULL
)

INSERT INTO @meta_type_sample_data (TypeID, TypeName, TypeVersion,  ConfigTemplateJson)
VALUES 
	(1, 'SimpleHttpGetScraper ', '20191215.2 ','HttpScraper config template json'),
	(2, 'WikiPageScraper','1.0', 'Wiki page config template json'),
	(3, 'RealEstateMarketScraper', '1.0', 'Real estate market config template json'),
	(4, 'ExchangeRatesScraper', '1.0','Exchange rates config template json'),
	(5, 'HabrArticleScraper', '1.0','Habr article config template json'),
	(6, 'NewsScraper', '1.0','News config template json'),
	(7, 'StockMarketScraper','1.0', 'Stock market config template json'),
	(8, 'OnlineLibraryScraper', '1.0','Online library config template json'),	
	(9, 'TemperatureParser', '1.0','Temperature config template json'),
	(10, 'ArticleDateParser','1.0', 'Article date config template json'),
	(11, 'RubleExchangeRateParser','1.0', 'Ruble exchange rate config template json'),
	(12, 'MobilePhonePriceParser', '1.0','Mobile phone price config template json'),
	(13, 'WindParser', '1.0','Wind config template json'),
	(14, 'ArticleAuthorParser', '1.0','Article author config template json'),
	(15, 'NewsTopicParser', '1.0','News topic config template json'),
	(16, 'BookAvailabilityParser','1.0', 'Book availability config template json'),
	(17, 'ComputerPricesPersister','1.0', 'Computer prices config template json'),
	(18, 'SportResultsPersister', '1.0','Sport results date config template json'),
	(19, 'ArticlePersister','1.0', 'Article config template json'),
	(20, 'UpdatesHistoryPersister','1.0', 'Updates history config template json'),
	(21, 'WeatherPersister', '1.0','Weather config template json');

SET IDENTITY_INSERT [meta].[ApplicationType] ON;  


MERGE [meta].[ApplicationType] AS BaseTable
	USING @meta_type_sample_data AS SourceTable
	ON (BaseTable.TypeID = SourceTable.TypeID)
	WHEN MATCHED AND (BaseTable.TypeName != SourceTable.TypeName OR
		BaseTable.TypeVersion != SourceTable.TypeVersion OR
		BaseTable.ConfigTemplateJson != SourceTable.ConfigTemplateJson) THEN
                 UPDATE SET TypeName = SourceTable.TypeName, TypeVersion=SourceTable.TypeVersion, ConfigTemplateJson = SourceTable.ConfigTemplateJson
	WHEN NOT MATCHED THEN
                 INSERT (TypeID, TypeName, TypeVersion,  ConfigTemplateJson)
                 VALUES (SourceTable.TypeID, SourceTable.TypeName, SourceTable.TypeVersion,  SourceTable.ConfigTemplateJson)
	
	OUTPUT $action AS [Operation], Inserted.TypeID as TypeID,
				Inserted.TypeName as TypeName,
				Inserted.TypeVersion as TypeVersion,
				Inserted.ConfigTemplateJson as ConfigTemplateJson;

SET IDENTITY_INSERT [meta].[ApplicationType] OFF;

/*
Add sample data to Components Table
*/


DECLARE @component_component_sample_data TABLE
(
	[InstanceID] INT NOT NULL PRIMARY KEY, 
    [TypeID] INT NOT NULL,
	[InstanceName] NVARCHAR(50) NOT NULL,
	[ConfigJson] NVARCHAR(MAX) NOT NULL
);

INSERT INTO @component_component_sample_data (InstanceID, TypeID, InstanceName, ConfigJson)
VALUES 
	(1, 1, 'YandexWeatherScrapper',  'Yandex scraper config json'),
	(2, 1, 'LAWeatherScrapper','Weather config json'),
	(3, 2, 'WikiPage1Scrapper','Wiki page config json'),
	(4, 4, 'RUB_EURO_ExchangeRatesScrapper','Exchange rates config json'),
	(5, 4, 'USD_EURO_ExchangeRatesScrapper','Exchange rates config json'),
	(6, 4, 'USD-CAD_ExchangeRatesScrapper', 'Exchange rates config json'),
	(7, 6, 'RBKNewsScrapper','News config json'),
	(8, 6, 'YandexNewsScrapper','News config config json'),
	(9, 9, 'CSVTemperatureParser', 'Temperature config json'),
	(10, 10, 'HTMLArticleDataParser','Article date config json'),
	(11, 10, 'TextExtractorDataParser','Article date config json'),
	(12, 10, 'ImageExtractorDataParser', 'Article date config json'),
	(13, 11, 'RUBExchangeRateParser','Ruble exchange rate config json'),
	(14, 15, 'SomeNewsParser','News topic config json'),
	(15, 17, 'ComputerPricePersisterA', 'Computer prices config json'),
	(16, 17, 'ComputerPricePersisterB','Computer prices config json'),
	(17, 18, 'SportResultsPersisterA','Sport results config json');

SET IDENTITY_INSERT [component].[ApplicationInstance] ON;


MERGE [component].[ApplicationInstance] AS BaseTable
	USING @component_component_sample_data AS SourceTable
	ON (BaseTable.InstanceID = SourceTable.InstanceID)
	WHEN MATCHED AND (BaseTable.TypeID != SourceTable.TypeID OR
		BaseTable.ConfigJson != SourceTable.ConfigJson OR BaseTable.InstanceName != SourceTable.InstanceName) THEN
                 UPDATE SET TypeID = SourceTable.TypeID, ConfigJson = SourceTable.ConfigJson, InstanceName=SourceTable.InstanceName
	WHEN NOT MATCHED THEN
                 INSERT (InstanceID, TypeID, InstanceName,  ConfigJson)
                 VALUES (SourceTable.InstanceID, SourceTable.TypeID, SourceTable.InstanceName, SourceTable.ConfigJson)
	
	OUTPUT $action AS [Operation], Inserted.InstanceID as InstanceID,
				Inserted.TypeID as TypeID,
				Inserted.InstanceName as InstanceName,
				Inserted.ConfigJson as ConfigJson;
SET IDENTITY_INSERT [component].[ApplicationInstance] OFF;

/*
Add sample data to Bindings Table
*/


DECLARE @binding_binding_sample_data TABLE
(
	[PublisherInstanceID] INT NOT NULL, 
	[ConsumerInstanceID] INT NOT NULL,  
	PRIMARY KEY([PublisherInstanceID], [ConsumerInstanceID])
)

INSERT INTO @binding_binding_sample_data (PublisherInstanceID, ConsumerInstanceID )
VALUES 
	(1, 9),
	(2, 9),
	(3, 10),
	(4, 13),
	(7, 14),
	(8, 14),
	(14, 15),
	(14, 16);

	


MERGE [binding].[Binding] AS BaseTable
	USING  @binding_binding_sample_data AS SourceTable
	ON (BaseTable.PublisherInstanceID = SourceTable.PublisherInstanceID AND BaseTable.ConsumerInstanceID=SourceTable.ConsumerInstanceID)
	
	WHEN NOT MATCHED THEN
                 INSERT (PublisherInstanceID, ConsumerInstanceID)
                 VALUES (SourceTable.PublisherInstanceID, SourceTable.ConsumerInstanceID)
	OUTPUT $action AS [Operation], Inserted.PublisherInstanceID as PublisherInstanceID,
				Inserted.ConsumerInstanceID as ConsumerInstanceID;

