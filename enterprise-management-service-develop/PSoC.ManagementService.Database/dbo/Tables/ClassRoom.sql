CREATE TABLE [dbo].[Classroom] (
    [ClassroomID] UNIQUEIDENTIFIER NOT NULL,
    [ClassroomName] NVARCHAR (50) NOT NULL,
    [BuildingName] NVARCHAR (50) NULL,
    [DistrictID] UNIQUEIDENTIFIER NOT NULL,
    [SchoolID] UNIQUEIDENTIFIER NULL,
    [ClassroomAnnotation] NVARCHAR (200) NULL, 
    [Created] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), 
    CONSTRAINT [PK_Classroom] PRIMARY KEY NONCLUSTERED ([ClassroomID]), 
    CONSTRAINT [FK_ClassRoom_ToDistrict] FOREIGN KEY ([DistrictID]) REFERENCES [District] ([DistrictID]),
    CONSTRAINT [FK_Classroom_ToSchool] FOREIGN KEY ([SchoolID]) REFERENCES [School]([SchoolID])
);
GO

CREATE INDEX [IX_Classroom_ClassroomID] ON [dbo].[Classroom] ([ClassroomID])
GO

CREATE CLUSTERED INDEX [IX_Classroom_Created] ON [dbo].[Classroom] ([Created])
GO

CREATE NONCLUSTERED INDEX [IX_ClassRoom_ClassRoomName]
    ON [dbo].[ClassRoom]([ClassRoomName] ASC);
