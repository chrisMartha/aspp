CREATE TABLE [dbo].[Course] (
    [CourseLearningResourceID]  UNIQUEIDENTIFIER NOT NULL,
    [CourseName]                NVARCHAR (50)    NULL,
    [Grade]                     NVARCHAR (2)     NULL,
    [Subject]                   NVARCHAR (20)    NULL,
    [CourseAnnotation]          NVARCHAR (80)    NULL,
	[Created]                   DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    PRIMARY KEY NONCLUSTERED ([CourseLearningResourceID] ASC),
);
GO

CREATE NONCLUSTERED INDEX [IX_Course_LearningResourceID]
    ON [dbo].[Course]([CourseLearningResourceID] ASC);
GO

CREATE CLUSTERED INDEX [IX_Course_Created]
    ON [dbo].[Course] ([Created] ASC);
GO

