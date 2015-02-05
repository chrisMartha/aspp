Grant/Revoke License Request
----------------------------

- EndPoint:	{BaseUrl}/api/v1/devices/{deviceID}
- HTTP Method: PUT
- Request Content-Type:	application/json
- Request Payload: Body
- Response Content-Type:	application/json

Example:
--------

- **PUT** https://psoc-management-service-dct.cloudapp.net/api/v1/devices/9D6FC8C4-02CC-437C-9BE5-D95C4D78AC29

Sample Requests:
----------------
- Refer to JSON in RequestLicenseByDevice.json, ReturnLicenseByDevice.json, Revoke_LicenseByAdmin.json etc. for reference

Business Rules 
--------------
- Please refer to document "PEMS Configurations Settings and expected behavior"                     
https://docs.google.com/a/pearson.com/document/d/1yOrag3Gs_KZHcX3IrZXMgbLKIx7H6O303HnTwVVpItE/edit
- This document provides the initial set of rules, **for MVP Release**, detailing conditions wherein a license
may be granted or denied

API Request Types
-----------------
- There are 4 types of license requests
- Same endpoint handles all the scenarios as defined by LicenseRequestType database table

### Request License (RequestType=1)
- This is request to get a license from Device.
- Sample request json: RequestLicenseByDevice.json
- Required Params: DeviceId (should be a valid Guid), EnvironmentId (represents config code), WifiBSSID, UserId (should be a valid Guid)
- Scenario: Device sends "downloadLicenseRequested" as true and reports "learningContentQueued" as greater than 0 to indicate there is some content queued to be downloaded
- **Note**:	Setting "RequestType" is optional. This is assumed as 1 with above requirements
- Authorization: None

### Return License (RequestType=2)		
- This is request to return a license from Device.
- Sample request json: ReturnLicenseByDevice.json
- Required Params: DeviceId (should be a valid Guid), EnvironmentId (represents config code), WifiBSSID, UserId (should be a valid Guid)
- Scenario: Following cases denote request is to return a license
  - Device sends "downloadLicenseRequested" as false (Setting the "requestType" or value of "learningContentQueued" does not matter)
  - Device sends "learningContentQueued" as 0 (irrespective of the value of "downloadLicenseRequested" and "requestType")
  - Setting "RequestType" explicitly as 2 (irrespective of the value of "downloadLicenseRequested" and "learningContentQueued")
  - **Note**:	Setting "RequestType" is optional. This is assumed as 2 with above (i and ii) requirements.
  - Authorization: None

### Revoke License (RequestType=3)
- This is request to revoke a license for a device by an Admin through the portal/site
- Sample request json: RevokeLicenseByAdmin.json	
-	Required Params: DeviceId (should be a valid Guid), WifiBSSID.
- Scenario: "RequestType" should be set to 3 and is not optional here. Values for other params "downloadLicenseRequested" and "learningContentQueued" are ignored.
- Authorization: Check if the current Principal is authenticated and is an admin

### Server Grant  (RequestType=4) 
- This request is to grant a license, as it becomes available, to a device that was denied earlier.
- This would be implemented as part of Push Notification. 
- This requires that "RequestType" should be set as 4. Currently, throws a NotImplemented Error if invoked. 