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
            if (data == 2) {
                toastr.error('Bạn chưa đăng ký gói đặt lịch. Vui lòng nâng cấp để được sử dụng gói đặt lịch.');
                $('#btnshedule').prop('disabled', false);
                return false;
            }
            else {
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
            }
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

    RegisterTimepicker();
    function RegisterTimepicker() {
        $('.timeshedule').datetimepicker({
            format: "HH:mm"
        });
    }

    // 
    $('.btnaddsetting').on('click', function () {
        var parent = $(this).parents('.panel-table');
        var htmlAdd = '<tr><td><input class="form-control displaynone" value="" type="text" hidden></td>'
                    + '<td><input class="form-control timeshedule" value="06:00" type="text"></td><td>_</td>'
                    + '<td><input class="form-control timeshedule" value="18:00" type="text"></td>'
                    + '<td><button type="button" class="btn btn-default btnremovesetting"><span class="glyphicon glyphicon-minus"></span></button></td></tr>';

        $(parent).find('.panel-body').find('tbody').append(htmlAdd);
        $('.btnremovesetting').on('click', function () {
            $(this).parent().parent().remove();
        });
        RegisterTimepicker();
    })

    $('.btnsavesetting').on('click', function () {
        // Get array object
        var arrayPush = [];
        var parent = $(this).parents('.panel-table');
        var listtrObject = $(parent).find('.panel-body').find('tbody');
        $.each((listtrObject).children(), function (index, data) {
            var rowData = {
                SETTING_CONTROL_ID: $(data).find('input[type="text"]:eq(0)').val(),
                TIME_ON: $(data).find('input[type="text"]:eq(1)').val(),
                TIME_OFF: $(data).find('input[type="text"]:eq(2)').val(),
            };

            arrayPush.push(rowData);
        });

        // validate
        // Tat ca start > 0h, tat ca end < 23h
        // khoang thoi gian start va end ko giao nhau.
        // 
        console.log(arrayPush);


        SaveDataSetting(arrayPush, $(this).attr('deviceid'));

    });

    function ValidateDataSetting(rowdata) {
        if (rowdata.length != 0) {
            
        }
    }

    function SaveDataSetting(data, deviceid) {
        $.ajax({
            method: "POST",
            url: "/Home/SaveSettingControl",
            data: { strArr: JSON.stringify(data), deviceid: deviceid }
        }).success(function (data) {
            if (data == true)
                toastr.info("Cập nhật thành công !");
            else
                toastr.info("Cập nhật thất bại !");
        });
    }

    $('.btnremovesetting').on('click', function () {
        $(this).parent().parent().remove();
    });

    $('#btnAutomatic').on('click', function () {
        toastr.info("Chức năng đang xây dựng. Vui lòng quay lại sau !");
    });
    
});

