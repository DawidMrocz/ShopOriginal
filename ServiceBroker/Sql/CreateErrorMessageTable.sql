/****** Object:  Table [dbo].[MroczwareServiceBrokerErrorMessage]    Script Date: 20.07.2023 15:17:55 ******/

IF NOT EXISTS (
  SELECT * 
  FROM   sys.tables 
  WHERE  object_id = OBJECT_ID(N'[dbo].[MroczwareServiceBrokerErrorMessage]') 
         AND name = 'MroczwareServiceBrokerErrorMessage'
)

BEGIN

CREATE TABLE [dbo].[MroczwareServiceBrokerErrorMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_MroczwareServiceBrokerErrorMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[MroczwareServiceBrokerErrorMessage] ADD  CONSTRAINT [DF_MroczwareServiceBrokerErrorMessage_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

ALTER TABLE [dbo].[MroczwareServiceBrokerErrorMessage] ADD  CONSTRAINT [DF_MroczwareServiceBrokerErrorMessage_ModifyDate]  DEFAULT (getdate()) FOR [ModifyDate]
GO

ALTER TABLE [dbo].[MroczwareServiceBrokerErrorMessage] ADD  CONSTRAINT [DF_MroczwareServiceBrokerErrorMessage_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Przechowuje wiadomości Service Brokera które skończyły się błędem' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MroczwareServiceBrokerErrorMessage', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Data utworzenia' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MroczwareServiceBrokerErrorMessage', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Data modyfikacji' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MroczwareServiceBrokerErrorMessage', @level2type=N'COLUMN',@level2name=N'ModifyDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Czy aktywna' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MroczwareServiceBrokerErrorMessage', @level2type=N'COLUMN',@level2name=N'IsActive'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Przechowuje wiadomości Service Brokera które skończyły się błędem' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MroczwareServiceBrokerErrorMessage'
GO

END