var constantsKey = ['Nhiệt độ', 'Độ ẩm không khí', 'Độ ẩm đất', 'Ánh sáng'];

$(document).ready(function () {
    //$(".modal-dialog").draggable({
    //    handle: ".modal-header"
    //});

    $('#add-tracking').on('click', function () {
        $('#group-sensor').attr('disabled', false);
        $('#cbopin').attr('disabled', false);
        $('#id_device').val('');

        var data = GetListUnit('');
        $('#selectcontrol1').html(data);
        $('#selectcontrol2').html(data);
        // set default value
        $('#cbopin').html(GetListPin('', tokenkey, 2));

        $('#ModaTracking').modal('show');
    });

    $('#btn-addtracking').on('click', function () {
        AddTracking();
    });

    $('#group-sensor').on('change', function () {
        var val = this.value;

        if (val != "1")
            $('#group-sensor2').hide();
        else
            $('#group-sensor2').show();
        switch (val) {
            //case "1":
            //    $('#addtext-device').val("Nhiệt độ");
            //    break;
            case "2":
                $('#prefix-addtext-device').val("[Độ ẩm đất]");
                break;
            case "3":
                $('#prefix-addtext-device').val("[Ánh sáng]");
                break;
        }
    });

    function GetListUnit(selected) {
        var str = '';
        $.ajax({
            async: false,
            method: "POST",
            url: "/Home/GetListUnitAjax",
            data: { 'selected': selected }
        }).success(function (msg) {
            str = msg;
        });
        return str;
    }

    function AddTracking() {
        var pre_namecontrol = $('#prefix-addtext-device').val();
        var pre_namecontrol2 = $('#prefix-addtext-device2').val();

        var namecontrol = $('#addtext-device').val();
        var namecontrol2 = $('#addtext-device2').val();

        var groupsensor = $('#group-sensor').val();
        var unit = $('#selectcontrol1').val();
        var unit2 = $('#selectcontrol2').val();
        var pinid = $('#cbopin').val();
        var tokenkey = GetTokenKey();

        namecontrol = pre_namecontrol + ' ' + namecontrol;
        if (groupsensor == '1')
            namecontrol2 = pre_namecontrol2 + ' ' + namecontrol2;

        var id_device = $('#id_device').val();
        // validate
        var rowData = {
            'NAME_CONTROL_1': namecontrol,
            'NAME_CONTROL_2': namecontrol2,
            'GROUP_SENSOR': groupsensor,
            'DEVICE_ID': id_device
        };

        var resValidate = ValidateTracking(rowData);
        if (IsNullOrEmpty(resValidate) == false) {
            toastr.warning(resValidate);
            return false;
        }


        $.ajax({
            method: "POST",
            url: "/Home/AddNewControlTracking",
            data: { 'device_id': id_device, 'namecontrol': namecontrol, 'namecontrol2': namecontrol2 == null ? '' : namecontrol2, 'tokenkey': tokenkey, 'groupsensor': groupsensor, 'unit': unit, 'pinid': pinid, 'unit2': (unit2 == null ? '1' : unit2) }
        }).success(function (msg) {
            if (msg == true) {
                console.log('success');
                location.reload();
            }
            else
                console.log('false');
        });
    }


    function GetTokenKey() {
        var currentLocation = window.location.pathname;
        return currentLocation.split('/')[3];
    }
    var tokenkey = GetTokenKey();

    GetDeciveDetails(1, '');
    function GetDeciveDetails(index, date) {
        //ShowLoading();
        $.ajax({
            url: '/Home/GetDeviceDetails',
            type: 'GET',
            dataType: 'json',
            data: { 'tokenkey': tokenkey, 'index': index, 'filterDate': date },
            success: function (data) {
                var html = '<table class="table"><thead class="thead-inverse"><tr><th>Thời gian</th>';

                for (var i = 0; i < data.listDevice.length; i++) {
                    html += '<th attrid=' + data.listDevice[i].DEVICE_ID + '>' + data.listDevice[i].DEVICE_NAME + '<span class="icon-setting fa fa-cog"></span></th>';
                }
                html += '</tr></thead>';

                for (var i = 0; i < data.listItem[0].length; i++) {
                    html += '<tr>';
                    for (var j = 0; j <= data.listDevice.length; j++) {
                        var x = data.listItem[j][i];
                        html += '<td>' + (x == 'NULL' || x == null ? '-' : x) + '</td>';
                    }
                    html += '</tr>';
                }

                html += '</table>';
                $('.details').html(html);
                // resize table
                $('.details').width($(document).outerWidth() - $('.left_col').outerWidth() - 20);

                $('.icon-setting').on('click', function () {
                    // modal setting tracking
                    $('#ModaldetailTracking').modal('show');
                    var id = $(this).parent().attr('attrid');
                    $('#id_device').val(id);
                });
                //HideLoading();
            }
        });
    }

    $('#edittracking').on('click', function () {
        var id = $('#id_device').val();
        $('#group-sensor').attr('disabled', true);
        $('#cbopin').attr('disabled', true);
        $('#ModaldetailTracking').modal('hide');
        LoadInfoDevice(id);
    });

    $('#deltracking').on('click', function () {
        $('#ModaldetailTracking').modal('hide');
        ShowModalConfirm('Delete', 'Bạn có chắc chắc muốn xóa thiết bị này ?',
            function () {
                $.ajax({
                    method: "POST",
                    url: "/Home/DeleteDevice",
                    data: { 'id': $('#id_device').val() }
                }).success(function (msg) {
                    if (msg == true)
                        location.reload();
                    else
                        toastr.error("Delete error !");
                });
            })
    });

    function ReplaceFirstNameControl(name) {
        var result;
        if (name.indexOf('[Độ ẩm đất]') != -1) {
            result = name.replace('[Độ ẩm đất]', '').trim();
        } else if (name.indexOf('[Độ ẩm không khí]') != -1) {
            result = name.replace('[Độ ẩm không khí]', '').trim();
        } else if (name.indexOf('[Nhiệt độ]') != -1) {
            result = name.replace('[Nhiệt độ]', '').trim();
        } else if (name.indexOf('[Ánh sáng]') != -1) {
            result = name.replace('[Ánh sáng]', '').trim();
        }
        return result;
    }

    function LoadInfoDevice(deviceid) {
        $.ajax({
            method: "POST",
            url: "/Home/LoadInfoDevice",
            dataType: "JSON",
            data: { 'deviceid': deviceid }
        }).success(function (msg) {
            $('#group-sensor').val(msg[0].GROUP_SENSOR_ID);
            $('#group-sensor').trigger('change');

            $('#addtext-device').val(ReplaceFirstNameControl(msg[0].DEVICE_NAME));
            $('#selectcontrol1').html(GetListUnit(msg[0].UNIT));
            $('#cbopin').html(GetListPin(msg[0].PIN_ID, tokenkey, 2));
            if (msg.length > 1) {
                // group
                // Load group HT
                $('#addtext-device2').val(ReplaceFirstNameControl(msg[1].DEVICE_NAME));
                $('#selectcontrol2').html(GetListUnit(msg[1].UNIT));
            }
            $('.modal-title').text('Sửa thiết bị');
            $('#ModaTracking').modal('show');
        });
    }

    // get list pin
    function GetListPin(strSelected, tokenkey, device_category) {
        var result = '';
        $.ajax({
            async: false,
            method: "POST",
            url: "/Home/GetListPin",
            data: { 'strSelected': strSelected, 'tokenkey': tokenkey, 'device_category': device_category }
        }).success(function (data) {
            result = data;
        });
        return result;
    }

    // validate
    function ValidateTracking(rowdata) {
        if (IsNullOrEmpty(rowdata.NAME_CONTROL_1)) {

            return "Vui lòng nhập tên thiết bị !";
        }
        else if (IsNullOrEmpty(rowdata.NAME_CONTROL_2) && rowdata.GROUP_SENSOR == '1') {

            return "Vui lòng nhập tên thiết bị !";
        }
        else if (CheckExistsNameTracking(rowdata.NAME_CONTROL_1, rowdata.DEVICE_ID) == true && rowdata.GROUP_SENSOR == '1') {

            return "Tên thiết bị đã được đăng kí. Vui lòng nhập tên thiết bị khác !";
        }
        else if (CheckExistsNameTracking(rowdata.NAME_CONTROL_1, rowdata.DEVICE_ID) == true && rowdata.GROUP_SENSOR != '1') {

            return "Tên thiết bị đã được đăng kí. Vui lòng nhập tên thiết bị khác !";
        }
        else if (CheckExistsNameTracking(rowdata.NAME_CONTROL_2, rowdata.DEVICE_ID) == true && rowdata.GROUP_SENSOR == '1') {

            return "Tên thiết bị đã được đăng kí. Vui lòng nhập tên thiết bị khác !";
        }
    }

    function CheckExistsNameTracking(name, deviceid) {
        var result = false;
        $.ajax({
            async: false,
            method: "POST",
            url: "/Home/CheckExistsNameTracking",
            data: { 'name': name, 'deviceid': deviceid }
        }).success(function (data) {
            result = data;
        });
        return result;
    }


});
