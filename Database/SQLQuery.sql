CREATE TABLE [dbo].[Users] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [UserName]     NVARCHAR (50)   NOT NULL,
    [UserPasscode] BINARY (128)    NOT NULL,
    [UserImage]  VARBINARY (MAX) NOT NULL,
    [UserID]       BINARY (48)     NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

