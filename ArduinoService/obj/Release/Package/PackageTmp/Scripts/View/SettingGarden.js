$(document).ready(function () {
    function GetTokenKey() {
        var currentLocation = window.location.pathname;
        return currentLocation.split('/')[3];
    }

    $('#btnshedule').on('click', function () {
        $('#btnshedule').prop('disabled', true);
        $.ajax({
            method: "POST",
            async: false,
            url: "/Home/UpdateShedule",
            data: { 'tokenkey': GetTokenKey() }
        }).success(function (data) {
            if (data != -1) {
                if (data == '1') {
                    $('#btnshedule').val('Bật');
                    $('#settingshedule').hide();
                    $('#btnshedule').attr('isshedule', 0);
                }
                else {
                    $('#settingshedule').show();
                    $('#btnshedule').val('Tắt');
                    $('#btnshedule').attr('isshedule', 1);
                }
            }
            else
                toastr.error('Error system. Please contact administrator');
            $('#btnshedule').prop('disabled', false);
        });

    });

    $('#btnaddshedule').on('click', function () {
        var htmltr = '<tr><td><input class="form-control" type="text" /></td><td>_</td><td><input class="form-control" type="text" /></td><td><input class="form-control removetr" type="button" value="-" /></td></tr>';
        $('#settingshedule').find('table').find('tbody').append(htmltr);

        $('.removetr').unbind('click');
        $('.removetr').on('click', function () {
            $(this).parent().parent().remove();
        });

    });

    $('#btnUpdateshedule').on('click', function () {


        $.ajax({
            method: "POST",
            url: "/Home/",
            data: {}
        }).success(function (data) {
            if (data == true)
                console.log('true');
            else
                console.log('false');
        });
    });




});

