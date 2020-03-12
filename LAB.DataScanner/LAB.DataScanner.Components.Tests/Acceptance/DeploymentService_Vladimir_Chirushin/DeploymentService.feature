Feature: DeploymentService
	In order to run my custom services in service fabric
	I want to rearange files of microservice, create package and deploy it into service fabric.

@ignore
Scenario: Listen for deployment event and reponds to logger about these events
	Given I have no any applications in local service fabric
	And I have no any application types in local service fabric
	And I have empty database for application configuration
	And I have in config database application instance 'CsvToJsonParserType' with this id '1' with build version '20191224.10' with these properties
	| Key                       | Value                                                              |
	| Application_Columns       | 1 2 3 4 5                                                          |
	| Application_Rows          | 3                                                                  |
	| Binding_SenderExchange    | CsvToJsonParser                                                    |
	| Binding_SenderRoutingKeys | CsvToJsonParser_SimpleTextParser,CsvToJsonParser_SimpleImageParser |
	| Binding_ReceiverQueue     | SimpleHttpGetScraper_CsvToJsonParserType                           |
	And I have in config database application instance 'SimpleHttpGetScraperType' with this id '2' with build version '20191224.10' with these properties
	| Key                       | Value                                                                       |
	| Application_TargetUrls    | http://ya.ru/ http://google.com/ http://wikipedia.org                       |
	| Binding_SenderExchange    | SimpleHttpGetScraper                                                        |
	| Binding_SenderRoutingKeys | SimpleHttpGetScraper_CsvToJsonParser,SimpleHttpGetScraper_SimpleImageParser |
	| Binding_ReceiverQueue     | null                                                                        |
	And I have in config database application instance 'SimpleImageParserType' with this id '3' with build version '20191224.10' with these properties
	| Key                       | Value                                                                         |
	| Application_TargetUrls    | https://habr.com/ru/ https://www.spbstu.ru/                                   |
	| Binding_SenderExchange    | SimpleImageParserType                                                         |
	| Binding_SenderRoutingKeys | SimpleImageParserType_CsvToJsonParser,SimpleImageParserType_SimpleImageParser |
	| Binding_ReceiverQueue     | SimpleHttpGetScraperType                                                      |
	And I have in config database application instance 'SimpleTextParserType' with this id '4' with build version '20191224.10' with these properties
	| Key                       | Value                                                                       |
	| Application_Criteria      | Betwen                                                                      |
	| Application_TargetWord    | TestTargetWord                                                              |
	| Application_StartWord     | TestStartWord                                                               |
	| Application_StopWord      | TestStopWord                                                                |
	| Binding_SenderExchange    | SimpleTextParserType                                                        |
	| Binding_SenderRoutingKeys | SimpleTextParserType_CsvToJsonParser,SimpleTextParserType_SimpleImageParser |
	| Binding_ReceiverQueue     | SimpleHttpGetScraperType                                                    |
	And I have empty folder for sfpkgs here 'C:\temp\PackagePreparePlace_VC\'
	And I have DeploymentService with this parameters
	| Key								 | Value					 |
	| Config:Binding:ReceiverQueue       | Deployment			 	 |
	| Config:Application:sfClusterUri    | http://localhost:19080    |
	And I have new deployment message into this Deployment exchange with this data
	| ComponentId | DeploymentType |
	| 1           | Deploy         |
	| 2           | Deploy         |
	| 3           | Deploy         |
	| 4           | Deploy         |
	Then I wait for 180 seconds to check data
	Then I check if there is instance with this name 'CsvToJsonParserType' in service fabric
	Then I check if there is instance with this name 'SimpleHttpGetScraperType' in service fabric
	Then I check if there is instance with this name 'SimpleImageParserType' in service fabric
	Then I check if there is instance with this name 'SimpleTextParserType' in service fabric
	Then I wait for 60 seconds to check data