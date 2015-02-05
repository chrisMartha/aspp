
$(document).ready(function () {
    var isGlobalAdmin = (typeof userType != 'undefined' && userType === "GlobalAdmin");
    var isDistrictAdmin = (typeof userType != 'undefined' && userType === "DistrictAdmin");
    var userName = $('body div.navbar div.pull-right span#username').text();
    var statusTable = $('#statusTable').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bPaginate": true,  //Enable Pagination
        "bFilter": false,   //Hide Search
        "pagingType": "full_numbers", // 'First', 'Previous', 'Next' and 'Last' buttons, plus page numbers
        "lengthMenu": [10, 25, 50, 100],  //Page size dropdown
        //"lengthMenu": [[5, 10, 15, -1], [5, 10, 15, "All"]],  //Page size dropdown
        "info": false,
        //"ordering": false,  //Enable Sorting
        "bStateSave": false,
        "sAjaxSource": "/Admin/Dashboard",
        "columns": [
                        { "data": "Created" },                                  //0
                        { "data": "LastContentUpdate" },                        //1
                        { "data": "DistrictName" },                             //2
                        { "data": "SchoolName" },                               //3
                        { "data": "DeviceId", "sClass": "device" },             //4
                        { "data": "DeviceName" },                               //5
                        { "data": "DeviceType" },                               //6
                        { "data": "DeviceOSVersion" },                          //7
                        { "data": "Username", "sClass": "uname" },              //8
                        { "data": "UserType", "sClass": "utype" },              //9
                        { "data": "ConfiguredGrade" },                          //10
                        { "data": "LocationName" },                             //11
                        { "data": "WifiBSSID" },                                //12
                        { "data": "WifiSSID" },                                 //13
                        { "data": "DownloadRequestType", "sClass": "reqType" }, //14
                        { "data": "DownloadRequestCount" },                     //15
                        { "data": "CanRevoke" }                                 //16
        ],
        "columnDefs": [
            {
                //Format the date for Date Created
                "targets": [0],
                "data": null,
                "render": function (data, type, full, meta) {
                    return renderUtcDateAsLocalDate(data);
                }
            },
            {
                //Format the date for LastContentUpdatedAt
                "targets": [1],
                "data": null,
                "render": function (data, type, full, meta) {
                    return renderUtcDateAsLocalDate(data);
                }
            },
            {
                //Hide District Name
                "targets": [2],
                "visible": isGlobalAdmin ? true : false,
            },
            {
                 //Hide School Name
                 "targets": [3],
                 "visible": isGlobalAdmin || isDistrictAdmin ? true : false,
            },
            {
                //Revoke Button based on CanRevoke
                "targets": [16],
                "data": null,
                "render": function (data, type, full, meta) {
                    return renderLicenseAction(data);
                }
            }
        ]
    });

    $("#statusTable tbody").on("click", "button.licenseAction", function () {
        var revokeButton = $(this);
        var deviceId = revokeButton.closest('tr').children('td.device').text();
        var requestPayload = { "deviceId": deviceId, "RequestType": "3" };

        $("#expire-dialog").dialog({
            title: "Confirmation",
            buttons: {
                Yes: function () {
                    $.ajax({
                        type: "PUT",
                        contentType: "application/json; charset=utf-8",
                        url: "/api/v1/devices/" + deviceId,
                        data: JSON.stringify(requestPayload),
                        dataType: "json",
                        success: function (data) {
                            var response = JSON.parse(JSON.stringify(data));
                            if (response != null && !response.CanDownloadLearningContent) {
                                //alert('License revoked for device: ' + deviceId);
                                revokeButton.closest('tr').children('td.utype').text(userType);
                                revokeButton.closest('tr').children('td.uname').text(userName);
                                revokeButton.closest('tr').children('td.reqType').text('Revoked License');
                                revokeButton.remove();
                            }
                        },
                        error: function (err) {
                            alert("An error occured. Could not revoke license for the device: " + deviceId);
                        }
                    });

                    $(this).dialog('close');
                },
                No: function () {
                    $(this).dialog('close');
                }
            },
            dialogClass: 'dialog_css',
            width: 400,
            closeOnEscape: false,
            draggable: false,
            resizable: false,
            modal: true
        });

        return (false);
    });
});

function renderLicenseAction(data) {
    var link = '';
    if (data != null && data) {
        link = '<button class="licenseAction">Expire</button>';
    }
    return link;
}

function renderUtcDateAsLocalDate(data) {
    var localDate = '';
    if (data != null && data) {
        var currentDate = new Date();
        var timeZoneOffSet = (currentDate.getTimezoneOffset() / 60) * -1;
        var utcDate = moment(data).utcOffset(timeZoneOffSet);
        if (utcDate != null && utcDate.year() > 1970) {
            localDate = utcDate.format("MM/DD/YYYY hh:mm:ss A");
        }
    }
    return localDate;
}

function TryParseInt(str, defaultValue) {
    var retValue = defaultValue;
    if (str !== null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseInt(str);
            }
        }
    }
    return retValue;
}
