INSERT INTO [dbo].[ServiceBrokerErrorMessage]
           ([CreateDate]
           ,[ModifyDate]
           ,[IsActive]
           ,[Message])
     VALUES
           (
            GETDATE()--<CreateDate, datetime,>
           ,GETDATE()--<ModifyDate, datetime,>
           ,1--<IsActive, bit,>
           ,@Message--, nvarchar(max),
		   )