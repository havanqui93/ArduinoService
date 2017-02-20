$(document).ready(function () {

    $('#add-control').on('click', function () {
        $('#ModalAddControl').modal('show');
    });

    $('#btn-addcontrol').on('click', function () {
        AddControl();
    });

    function AddControl() {
        var namecontrol = $('#addtext-device').val();

        var tokenkey = GetTokenKey();
        var pinid = $("#pinuno").val();

        if (IsNullOrEmpty(namecontrol)) {
            toastr.warning("Vui lòng nhập tên thiết bị !");
            return false;
        }

        $.ajax({
            method: "POST",
            url: "/Home/AddOrEditControl",
            data: { 'namecontrol': namecontrol, 'tokenkey': tokenkey, 'deviceid': '', 'pinid': pinid }
        }).success(function (msg) {
            if (msg == true) {
                console.log('success');
                //$('#ModalAddControl').modal('hide');
                location.reload();
            }
            else
                console.log('false');
        });
    }

    var edittingcell = null;
    var $isOpen = 0;

    $('.edit-control').on('click', function () {
        // create text input
        var thistextbox = $(this).parents('.x_title');
        var text = $(thistextbox).children('h2').text();
        var inputtext = '<input type="text" edittingrow=' + $(this).attr('idcontrol') + ' class="form-control" id="txtdevicename" maxlength="100" value="' + text + '">'

        // update editing before add new
        if (SaveControlName($(this).attr('idcontrol')) == true) {
            // change icon
            $('.edit-control').find('.fa').removeClass('fa-check-square-o').addClass('fa-pencil');

            if (edittingcell == $(this).attr('idcontrol') && $('input[edittingrow="' + edittingcell + '"]').length > 0) {
                // save cell current
                text = $('input[edittingrow="' + edittingcell + '"]').val();
                var html = '<h2>' + text + '</h2>';
                $(html).insertBefore($('input[edittingrow="' + edittingcell + '"]').next());
                $('input[edittingrow="' + edittingcell + '"]').remove();
                $(this).find('.fa').removeClass('fa-check-square-o').addClass('fa-pencil');

                // update name
            }
            else {
                if (edittingcell == null || edittingcell != $(this).attr('idcontrol')) {
                    $(inputtext).insertBefore($(thistextbox).children('h2'));
                    $(thistextbox).children('h2').remove();

                    $(this).parents('.x_title').find('#txtdevicename').focus();
                    $(this).find('.fa').removeClass('fa-pencil').addClass('fa-check-square-o');
                }

                if ($isOpen == 1) {
                    $(inputtext).insertBefore($(thistextbox).children('h2'));
                    $(thistextbox).children('h2').remove();

                    $(this).parents('.x_title').find('#txtdevicename').focus();
                    $(this).find('.fa').removeClass('fa-pencil').addClass('fa-check-square-o');
                }


                if (edittingcell == $(this).attr('idcontrol'))
                    $isOpen++;
                else
                    $isOpen = 0;
                $isOpen = $isOpen > 1 ? 0 : $isOpen;

                edittingcell = $(this).attr('idcontrol');
            }

        }

    });

    function SaveControlName(ideditting) {
        if (edittingcell != null && $('input[edittingrow="' + edittingcell + '"]').length > 0) {
            var text = $('input[edittingrow="' + edittingcell + '"]').val();

            // validate beofore save
            if (IsNullOrEmpty(text)) {
                toastr.warning("Vui lòng nhập tên thiết bị");
                $('input[edittingrow="' + edittingcell + '"]').focus();
                return false;
            }
            // update to server
            EditControlName(edittingcell);

            text = $('input[edittingrow="' + edittingcell + '"]').val();
            var html = '<h2>' + text + '</h2>';
            $(html).insertBefore($('input[edittingrow="' + edittingcell + '"]').next());
            $('input[edittingrow="' + edittingcell + '"]').remove();
        }


        return true;
    }

    function EditControlName(edittingcell) {
        var namecontrol = $('#txtdevicename[edittingrow="' + edittingcell + '"]').val();
        $.ajax({
            method: "POST",
            url: "/Home/AddOrEditControl",
            data: { 'namecontrol': namecontrol, 'gardenid': '', 'deviceid': edittingcell }
        }).success(function (msg) {
            if (msg == true) {
                console.log('success');
            }
            else
                console.log('false');
        });
    }

    function FocusEndOfText(id) {
        var el = $("#" + id).get(0);
        var elemLen = el.value.length;
        el.selectionStart = elemLen;
        el.selectionEnd = elemLen;
        el.focus();
    }

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


    $('.btn-control').on('click', function () {
        var device_id = $(this).attr('attrControl');
        var value = $(this).val();
        var hasClass = $(this).hasClass('btn-success');
        if (hasClass)
            return false;

        var data = { 'DEVICE_ID': device_id, 'VALUE': value };
        UpdateData(data);
    });

    var url = "/api/apiControl/";
    function UpdateData(data) {
        $.ajax({
            method: "PUT",
            url: url + data.DEVICE_ID,
            contentType: "application/json",
            data: JSON.stringify(data)
        }).success(function (msg) {
            console.log('update success');
        });
    }

    $('.delete-control').on('click', function () {
        var id = $(this).attr('idcontrol');
        ShowModalConfirm("Xóa thiết bị", "Bạn có chắc chắn muốn xóa thiết bị này ?",
            function () {
                $.ajax({
                    method: "POST",
                    url: "/Home/DeleteDevice",
                    data: { 'id': id }
                }).success(function (msg) {
                    if (msg == true)
                        location.reload();
                    else
                        toastr.error("Delete error !");
                });
            });
    });

    function GetTokenKey() {
        var currentLocation = window.location.pathname;
        return currentLocation.split('/')[3];
    }
    var tokenkey = GetTokenKey();

    // get list pin
    function GetListPin(strSelected, tokenkey, device_category) {
        $.ajax({
            method: "POST",
            url: "/Home/GetListPin",
            data: { 'strSelected': strSelected, 'tokenkey': tokenkey, 'device_category': device_category }
        }).success(function (data) {
            $('#pinuno').html(data);
        });
    }

    $('#ModalAddControl').on('show.bs.modal', function (e) {
        GetListPin('', tokenkey, 1);
    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });



});
