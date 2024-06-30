CREATE TABLE [D01].[InsurancePolicyData] (
    [Id]                    VARCHAR (50)  NOT NULL,
    [FirstName]             VARCHAR (50)  NOT NULL,
    [LastName]              VARCHAR (50)  NOT NULL,
    [DOB]                   DATE          NOT NULL,
    [Gender]                VARCHAR (10)  NOT NULL,
    [Email]                 VARCHAR (100) NOT NULL,
    [PhoneNumber]           VARCHAR (20)  NULL,
    [Address]               VARCHAR (100) NULL,
    [City]                  VARCHAR (50)  NULL,
    [State]                 VARCHAR (2)   NULL,
    [ZipCode]               VARCHAR (10)  NULL,
    [PolicyStartDate]       DATE          NOT NULL,
    [PrimaryCarePhysician]  VARCHAR (100) NULL,
    [EmergencyContactName]  VARCHAR (100) NULL,
    [EmergencyContactPhone] VARCHAR (20)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

