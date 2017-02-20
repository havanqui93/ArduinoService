$(document).ready(function () {
    $('.dashboard-div-wrapper').click(function () {
        var isActive = $(this).parent().attr('isactive');
        if (isActive == 1)
            window.location = $(this).attr('url');
        else
            toastr.warning("Khu vườn chưa kích hoạt !");
    });
});
