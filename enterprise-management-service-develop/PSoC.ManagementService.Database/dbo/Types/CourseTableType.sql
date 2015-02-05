CREATE TYPE [dbo].[CourseTableType] AS TABLE
(
	[CourseLearningResourceID]  UNIQUEIDENTIFIER NOT NULL,
    [CourseName]                NVARCHAR (50)    NULL,
    [Grade]                     NVARCHAR (2)     NULL,
    [Subject]                   NVARCHAR (20)    NULL,
    [CourseAnnotation]          NVARCHAR (80)    NULL
);