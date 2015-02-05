$('select').select2({ dropdownAutoWidth: 'true' });

$('.radioGlobal').change(function () {
    if ($('.radioGlobal').prop('checked')) {
       
        disableDistrict();
        disableSchool();
    }
});

$('.radioDistrict').change(function () {
    if ($('.radioDistrict').prop('checked')) {
        $('.selectDistrict').removeAttr('disabled');
        disableSchool();        
    }
    else {
        disableDistrict();
    }
});

$('.radioSchool').change(function () {
    if ($('.radioSchool').prop('checked')) {
        $('.selectSchool').removeAttr('disabled');
        disableDistrict();
    }
    else {
        disableSchool();
    }
});

function disableDistrict()
{
    $(".selectDistrict").val('').change();
    $('.selectDistrict').prop('disabled', true);
}

function disableSchool() {
    $(".selectSchool").val('').change();
    $('.selectSchool').prop('disabled', true);
}

$(function () {
    $('.activeToggle').bootstrapToggle();
})