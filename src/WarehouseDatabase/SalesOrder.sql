CREATE TABLE [dbo].[SalesOrder]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[UpdateBy] [nvarchar](50) NOT NULL
)
