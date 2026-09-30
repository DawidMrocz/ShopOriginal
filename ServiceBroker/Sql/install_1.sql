USE [TU NAZWA BAZY]

ALTER DATABASE [TU NAZWA BAZY] SET TRUSTWORTHY ON

-- CLEANUP 
IF EXISTS(SELECT * FROM sys.service_message_types WHERE [name] = 'MroczwareBrokerMessage')
BEGIN
	DROP MESSAGE TYPE [MroczwareBrokerMessage]
	CREATE MESSAGE TYPE [MroczwareBrokerMessage] VALIDATION = NONE 
END
ELSE
BEGIN
	CREATE MESSAGE TYPE [MroczwareBrokerMessage] VALIDATION = NONE 
END

IF EXISTS(SELECT * FROM sys.service_contracts WHERE [name] = 'MroczwareBrokerContract')
BEGIN 
	DROP CONTRACT [MroczwareBrokerContract]
	CREATE CONTRACT [MroczwareBrokerContract] ([MroczwareBrokerMessage] SENT BY ANY)  
END
ELSE
BEGIN
	CREATE CONTRACT [MroczwareBrokerContract] ([MroczwareBrokerMessage] SENT BY ANY)  
END

IF EXISTS(SELECT * FROM sys.service_queues WHERE [name] = 'MroczwareBrokerQueue')
BEGIN
	DROP QUEUE [MroczwareBrokerQueue]
	CREATE QUEUE [MroczwareBrokerQueue] WITH STATUS = ON, RETENTION = OFF, Poison_Message_Handling (STATUS = OFF) 
END
BEGIN
	CREATE QUEUE [MroczwareBrokerQueue] WITH STATUS = ON, RETENTION = OFF, Poison_Message_Handling (STATUS = OFF) 
END

IF EXISTS(SELECT * FROM sys.services WHERE [Name] = 'MroczwareBrokerService')
BEGIN
	DROP SERVICE [MroczwareBrokerService]
	CREATE SERVICE [MroczwareBrokerService] ON QUEUE [MroczwareBrokerQueue]
	ALTER SERVICE [MroczwareBrokerService] (ADD CONTRACT [MroczwareBrokerContract])
END
ELSE
BEGIN
	CREATE SERVICE [MroczwareBrokerService] ON QUEUE [MroczwareBrokerQueue]
	ALTER SERVICE [MroczwareBrokerService] (ADD CONTRACT [MroczwareBrokerContract])
END

