
var deleteLinkObj;
//Everytime we press delete in the table row
$('.delete').click(function(e) {
    deleteLinkObj = $(this);
    e.preventDefault();

    $('#ddUserId').html(deleteLinkObj.data('id'));
    $('#ddUsername').html(deleteLinkObj.data('username'));
    $('#ddUserType').html(deleteLinkObj.data('user-type'));    
    $('#ddLastLoginDateTime').html(deleteLinkObj.data('last-login'));
    $('#ddDistrictName').html(deleteLinkObj.data('district-name'));
    $('#ddDistrictId').html(deleteLinkObj.data('district-id'));
    $('#ddSchoolName').html(deleteLinkObj.data('school-name'));
    $('#ddSchoolId').html(deleteLinkObj.data('school-id'));
    $('#ddAdminEmail').html(deleteLinkObj.data('email'));

    if (deleteLinkObj.data('active') == 'True')
    {
        $('#ddActive').addClass('field-success');
        $('#ddActive').removeClass('field-danger');
        $('#ddActive').html('active');
    }
    else
    {
        $('#ddActive').addClass('field-danger');
        $('#ddActive').removeClass('field-success');
        $('#ddActive').html('inactive');
    }

    var deleteMessage = deleteLinkObj.data('user-type') + ' ' + deleteLinkObj.data('username') + ' has been deleted.';
    $('#deleteConfirmBody').html(deleteMessage)
});

$('#deleteConFirm').click(function () {
    deleteLinkObj.closest("tr").hide('fast'); //Hide Row
    $('#deleteConfirmModal').modal('toggle');
    $('#deleteModal').modal('toggle');
});

//Everytime we press sumbit on the modal form...
$('#deleteOk').click(function() {

    var token = $('input[name="__RequestVerificationToken"]').val();    
    var formData = $('form[action="/Admins"]').serialize();

    $.ajax({
        cache: false,
        type: 'POST',
        data: formData,
        url: deleteLinkObj.data('delete-url'),
        success: function (data) {

            if (data == 'True' || data == true) {
                $('#deleteConfirmModal').modal('toggle');
            }
            else {
                $('#deleteConfirmBody').html("Delete failed.  Refresh and try again.");
            }
        }
    });
});

$('#deleteUndo').click(function () {
    deleteLinkObj.closest("tr").show('fast');
});