CREATE TABLE [dbo].[DeviceInstalledCourse]
(
	[DeviceID] UNIQUEIDENTIFIER NOT NULL , 
    [CourseLearningResourceID] UNIQUEIDENTIFIER NOT NULL, 
	[PercentDownloaded] DECIMAL(38, 28) NULL,
    [LastUpdated] DATETIME NOT NULL , 
    PRIMARY KEY ([DeviceID], [CourseLearningResourceID]), 
    CONSTRAINT [FK_DeviceInstalledCourse_ToDevice] FOREIGN KEY ([DeviceID]) REFERENCES [Device]([DeviceID]), 
    CONSTRAINT [FK_DeviceInstalledCourse_ToCourse] FOREIGN KEY ([CourseLearningResourceID]) REFERENCES [Course]([CourseLearningResourceID])
)

GO

CREATE INDEX [IX_DeviceInstalledCourse_DeviceID] ON [dbo].[DeviceInstalledCourse] ([DeviceID])
