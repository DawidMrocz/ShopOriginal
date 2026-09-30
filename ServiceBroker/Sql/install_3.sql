-- =============================================
-- Author:		Siwak Jakub
-- Create date: 2017-09-25
-- Description:	Procedura wstawiająca dane do kolejki service broker'a
-- =============================================
create PROCEDURE [dbo].[ServiceBroker_Insert]
	@Type NVARCHAR(MAX),
	@Text NVARCHAR(MAX)
AS
BEGIN
	DECLARE @LocalTranCount INT
	SET @LocalTranCount = @@TRANCOUNT

		IF(@LocalTranCount = 0)
	BEGIN
		BEGIN TRANSACTION
	END

	BEGIN TRY

		DECLARE @sqlHandle as uniqueIdentifier
		DECLARE @message NVARCHAR(MAX)
 
		Set @message = '<MroczwareMessage><Type>' + ISNULL (@Type, '') + '</Type><Data>' + ISNULL (@Text, '') + '</Data></MroczwareMessage>'
 
		BEGIN 
			DIALOG 
		CONVERSATION 
			@sqlHandle
		FROM 
			SERVICE [MroczwareBrokerService] 
		TO 
			SERVICE 'MroczwareBrokerService', 'CURRENT DATABASE'
		ON 
			CONTRACT [MroczwareBrokerContract]
		WITH 
			ENCRYPTION = OFF;
	
		SEND ON 
			CONVERSATION @sqlHandle 
		MESSAGE TYPE 
			[MroczwareBrokerMessage] (@message)
		IF EXISTS(SELECT TOP 1 1 FROM sys.conversation_endpoints WHERE conversation_handle=@sqlHandle and state='ER')
		BEGIN
			RAISERROR ('Service Broker in error state',18,127)
		END
		ELSE
		BEGIN
			END CONVERSATION 
				@sqlHandle 
			WITH 
				CLEANUP;
		END 
	
	END TRY
	BEGIN CATCH
		IF (@LocalTranCount = 0  AND @@TRANCOUNT > 0) ROLLBACK TRANSACTION
		DECLARE @ErrorMessage NVARCHAR(4000)
		DECLARE @ErrorSeverity INT
		SELECT @ErrorMessage = ERROR_MESSAGE(),@ErrorSeverity = ERROR_SEVERITY()
		RAISERROR(@ErrorMessage,@ErrorSeverity,1)
	END CATCH
	IF(@LocalTranCount = 0  AND @@TRANCOUNT > 0) COMMIT TRANSACTION	
END
