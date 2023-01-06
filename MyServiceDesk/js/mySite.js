window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 8000);

//Function Anónima para el loader
$(function () {
    $('#loaderbody').addClass('hide');

    $(document).bind('ajaxStart', function () {
        $('#loaderbody').removeClass('hide');
    }).bind('ajaxStop', function () {
        $('#loaderbody').addClass('hide');
    });
});