CREATE TYPE [dbo].[DeviceInstalledCourseTableType] AS TABLE
(
	[DeviceID]					UNIQUEIDENTIFIER NOT NULL,
	[CourseLearningResourceID]  UNIQUEIDENTIFIER NOT NULL,
    [PercentDownloaded]         DECIMAL(38,28)		 NULL,
	[LastUpdated]				DateTime		 NOT NULL
);