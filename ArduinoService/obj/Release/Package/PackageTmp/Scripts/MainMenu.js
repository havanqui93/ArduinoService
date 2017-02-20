$(document).ready(function () {
    EditImage();
    AddImage();
    ActiveMenuAndBreabrum();

    $('.edit-garden').click(function () {
        $('#ModalEditGarden').modal('show');
        var objgarden = $(this).parents('.x_panel');
        var gardenid = $(objgarden).attr('garden_id');
        var gardenname = $(objgarden).find('.x_title').find('h2').text();
        var gardenimg = $(objgarden).find('.x_content').find('img').attr('src');
        var unotypeid = $(objgarden).attr('uno_type');
        $('#edittext-garden').val(gardenname);
        $('#gardenid').val(gardenid);
        $('#unotypeid').val(unotypeid);
        $('#preview-edit-garden').attr('src', gardenimg);
        GetListUnoType($('#unotypeid').val());
        $('#edittext-garden').focus();
    });

    $('.dashboard-div-wrapper').click(function () {
        var isActive = $(this).parent().attr('isactive');
        if (isActive == 1)
            window.location = $(this).attr('url');
        else
            toastr.warning("Khu vườn chưa kích hoạt !");
    })

    $('#editgarden').on('click', function () {
        $(this).html('<i class="fa fa-refresh fa-spin"></i> OK');
        EditGarden();
    });

    $('.delete-garden').on('click', function () {
        var gardenid = $(this).parents('.x_panel').attr('garden_id');
        DeleteGarden(gardenid);
    });


    $('#addgarden').on('click', function () {
        // validate before add
        var gardenname = $('#addtext-garden').val();
        var img = $('#preview-add-garden').attr('src');
        var unotype = $('.uno_type').val();
        var acreage = $('#acreage-garden').val();
        var address = $('#address-garden').val();
        var lat = $('#latite');
        var lon = $('#longitude');
        var description = $('#note-garden');

        $('#addtext-garden').focus();

        if (IsNullOrEmpty(gardenname)) {
            toastr.warning("Vui lòng nhập tên khu vườn !");
            return false;
        }
        AddGarden(gardenname, img, unotype, acreage, address, lat, lon, description);
    });

    function AddGarden(gardenname, img, unotype, acreage, address, lat, lon, description) {
        $.ajax({
            method: "POST",
            url: "/Home/AddGarden",
            dataType: "JSON",
            data: {
                'gardenname': gardenname, 'image': img, 'unotype': unotype, 'acreage': acreage,
                'address': address, 'lat': lat, 'lon': lon, 'description': description
            }
        }).success(function (msg) {
            if (msg.GARDEN_ID != null) {
                var html =
                '<div class="col-md-4 col-sm-4 col-xs-12">'
                + '<div class="x_panel tile fixed_height_275" garden_id=' + msg.GARDEN_ID + ' uno_type=' + msg.UNO_TYPE + ' >'
                + '<div class="x_title">'
                + '<h2>' + msg.GARDEN_NAME + '</h2>'
                + '<ul class="nav navbar-right panel_toolbox">'
                + '<li>'
                + '<a class="edit-garden"><i class="fa fa-pencil"></i></a>'
                + '</li>'
                 + '<li>'
                + '<a class="delete-garden"><i class="glyphicon glyphicon-trash"></i></a>'
                + '</li>'
                + '</ul>'
                + '<div class="clearfix"></div>'
                + '</div>'
                + '<div class="x_content">'
                + '<div class="box-garden">'
                + '<a href="/Home/Garden/' + msg.GARDEN_ID + '">'
                + '<img src="/' + (msg.IMAGE == null ? "Content/Images/bg_no_image.gif" : msg.IMAGE) + '" />'
                + '</a>'
                + '</div>'
                + '</div>'
                + '</div>'
                + '</div>';

                $('#listgarden').append(html);
                $("#ModalGarden").modal("hide");
                $('#addtext-garden').val('');
                $('#preview-add-garden').attr('src', '');
                $('.collapse-link').bind('click');

                $('.edit-garden').click(function () {
                    $('#ModalGarden').modal('show');
                    var objgarden = $(this).parents('.x_panel');
                    var gardenid = $(objgarden).attr('garden_id');
                    var gardenname = $(objgarden).find('.x_title').find('h2').text();
                    var gardenimg = $(objgarden).find('.x_content').find('img').attr('src');
                    var unotypeid = $(objgarden).attr('uno_type');

                    $('#edittext-garden').val(gardenname);
                    $('#gardenid').val(gardenid);
                    $('#unotypeid').val(unotypeid);
                    $('#preview-edit-garden').attr('src', gardenimg);
                    GetListUnoType($('#unotypeid').val());
                    $('#edittext-garden').focus();

                });

                $('.delete-garden').on('click', function () {
                    var gardenid = $(this).parents('.x_panel').attr('garden_id');
                    DeleteGarden(gardenid);
                });
            }
            else
                toastr.error("System error. Please contact administrator !");
        });
    }

    function DeleteGarden(id) {
        var gardenid = id;
        $('#ModalDelete').modal('show');
        $('#deleteok').unbind('click');
        $('#deleteok').on('click', function () {
            $.ajax({
                method: "POST",
                url: "/Home/DeleteGarden",
                dataType: "JSON",
                data: { 'tokenkey': gardenid }
            }).success(function (msg) {
                $('#ModalDelete').modal('hide');
                if (msg) {
                    $('div[garden_id="' + id + '"]').parent().remove();
                }
                else {
                    toastr.error("Error when delete.");
                }
            });
        });
    }

    function EditGarden() {
        var gardenname = $('#edittext-garden').val();
        var img = $('#preview-edit-garden').attr('src');
        var gardenid = $('#gardenid').val();
        var unotype = $('#ModalEditGarden .uno_type').val();

        $.ajax({
            method: "POST",
            url: "/Home/EditGarden",
            dataType: "JSON",
            data: { 'gardenname': gardenname, 'image': img, 'tokenkey': gardenid, 'unotype': unotype }
        }).success(function (msg) {
            $('div[garden_id="' + (IsNullOrEmpty(msg.GARDEN_ID) ? msg.TOKEN_KEY : msg.GARDEN_ID) + '"]').find('.x_title').find('h2').text(msg.GARDEN_NAME);
            $('div[garden_id="' + (IsNullOrEmpty(msg.GARDEN_ID) ? msg.TOKEN_KEY : msg.GARDEN_ID) + '"]').find('.x_content').find('img').attr('src', "/" + (msg.IMAGE == null ? "Content/Images/bg_no_image.gif" : msg.IMAGE));
            $("#ModalEditGarden").modal("hide");
            $('#editext-garden').val('');
            $('#preview-edit-garden').attr('src', '');
            $('div[garden_id="' + (IsNullOrEmpty(msg.GARDEN_ID) ? msg.TOKEN_KEY : msg.GARDEN_ID) + '"]').attr('uno_type', msg.UNO_TYPE);

        });
    }

    function AddImage() {
        $('#fileuploadAdd').fileupload({
            dataType: 'json',
            url: '/Home/UploadFiles',
            autoUpload: true,
            success: function (data) {
                $('#preview-add-garden').attr('src', "/" + data);
            }
        });
    }

    function EditImage() {
        var gardenname = $('#addtext-garden').val();
        var img = $('#preview-add-garden').attr('src');

        $('#fileuploadEdit').fileupload({
            dataType: 'json',
            url: '/Home/UploadFiles',
            autoUpload: true,
            success: function (data) {
                $('#preview-edit-garden').attr('src', "/" + data);
            }
        });
    }

    function GetListMenuLeft() {
        $.ajax({
            method: "GET",
            url: "/Home/GetListGardenMenuLeft",
        }).success(function (msg) {
            var html = '';
            $.each(msg, function (index, value) {
                html += '<li><a><i class="fa fa-wrench"></i>' + value.GARDEN_NAME + '<span class="fa fa-chevron-down"></span></a>';
                html += '<ul class="nav child_menu">';
                html += '<li><a href="/Home/Control/' + value.GARDEN_ID + '">Control System</a></li>';
                html += '<li><a href="/Home/Tracking/' + value.GARDEN_ID + '">Tracking System</a></li>';
                html += '</ul></li>';
            });
            $('#menu-left-garden').html(html);
        });
    }

    function ActiveMenuAndBreabrum() {
        var currentLocation = window.location.pathname;
        var html = '<li><a href="/Home/MainMenu"><i class="fa fa-home"></i>Home</a></li>';
        if (currentLocation == '/Home/MainMenu') {
            $('.breadcrumb').html(html);
        } else {
            var category = currentLocation.split('/')[2];
            var id = currentLocation.split('/')[3];

            if (category == 'Garden') {
                $('#menu-left-garden').find('li[attrgarden="' + id + '"]').addClass('active').find('.child_menu').css('display', 'block');
                var title = $('#menu-left-garden').find('li[attrgarden="' + id + '"]').children('a').text();
                html += '<li class="active">' + title + '</li>';
            }
            else {
                category = category == 'Control' ? 'Control System' : 'Tracking System';
                //$('#menu-left-garden').find('li[attrgarden="' + id + '"]').addClass('active').find('.child_menu').css('display', 'block').find('li').first().addClass('.current-page');
                //var title = $('#menu-left-garden').find('li[attrgarden="' + id + '"]').children('a').text();
                //html += '<li>' + title + '</li>';
                html += '<li class="active">' + category + '</li>';
            }
            $('.breadcrumb').html(html);
        }
    }

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

    function GetTokenKey() {
        var currentLocation = window.location.pathname;
        return currentLocation.split('/')[3];
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

    // Tracking

    $('#add-tracking').on('click', function () {
        $('#ModalAddTracking').modal('show');
    });

    $('#btn-addtracking').on('click', function () {
        AddTracking();
    });

    $('#unit-device').on('change', function () {
        var val = this.value;

        if (val != "1")
            $('#group-sensor2').hide();
        else
            $('#group-sensor2').show();
        switch (val) {
            case "1":
                $('#addtext-device').val("Nhiệt độ");
                break;
            case "2":
                $('#addtext-device').val("Độ ẩm đất");
                break;
            case "3":
                $('#addtext-device').val("Ánh sáng");
                break;
        }
    });


    $('#ModalAddTracking').on('shown.bs.modal', function () {
        var data = GetListUnit('');
        $('#selectcontrol1').html(data);
        $('#selectcontrol2').html(data);
        // set default value
    })

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
        var namecontrol = $('#addtext-device').val();
        var namecontrol2 = $('#addtext-device2').val();

        var unit = $('#unit-device').val();
        var tokenkey = GetTokenKey();

        if (IsNullOrEmpty(namecontrol)) {
            toastr.warning("Vui lòng nhập tên thiết bị !");
            return false;
        }
        $.ajax({
            method: "POST",
            url: "/Home/AddNewControlTracking",
            data: { 'namecontrol': namecontrol, 'namecontrol2': namecontrol2, 'tokenkey': tokenkey, 'deviceid': '', 'unit': unit }
        }).success(function (msg) {
            if (msg == true) {
                console.log('success');
                location.reload();
            }
            else
                console.log('false');
        });
    }


    $('.edit-sensor').on('click', function () {
        var id = $(this).attr('device_id');
        $('#id_devicehidden').val(id);
        var name = $(this).parents('.x_title').children('h2').text();
        $('#edit-device').val(name);

        $('#ModalEditTracking').modal('show');
    })

    function EditTracking() {
        var namecontrol = $('#edit-device').val();
        var tokenkey = GetTokenKey();
        var id = $('#id_devicehidden').val();
        var unit = $('#unit-device').val()

        if (IsNullOrEmpty(namecontrol)) {
            toastr.warning("Vui lòng nhập tên thiết bị !");
            return false;
        }
        $.ajax({
            method: "POST",
            url: "/Home/AddNewControlTracking",
            data: { 'namecontrol': namecontrol, 'tokenkey': tokenkey, 'deviceid': id, 'unit': unit }
        }).success(function (msg) {
            if (msg == true) {
                console.log('success');
                location.reload();
            }
            else
                console.log('false');
        });
    }

    $('.view-detail').on('click', function () {
        var title = $(this).parents('.x_title').children('h2').text();
        var device_id = $(this).attr('device_id');
        ViewDetails(device_id, title);
    })

    function ViewDetails(id, title) {
        $('#ViewDetailTracking').modal('show');
        $('#ViewDetailTracking .modal-title').text(title);

        $.ajax({
            method: "GET",
            url: "/Home/GetListValueTrackingDetails?deviceid=" + id,
            contentType: 'html'
        }).success(function (msg) {
            if (msg != '')
                $('#ViewDetailTracking .list-group').html(msg);
        });

    }

    // get list pin
    function GetListPin(strSelected, tokenkey) {
        $.ajax({
            method: "POST",
            url: "/Home/GetListPin",
            data: { 'strSelected': strSelected, 'tokenkey': tokenkey }
        }).success(function (data) {
            $('#pinuno').html(data);
        });
    }

    // get list pin
    function GetListUnoType(strSelected) {
        $.ajax({
            method: "POST",
            async: false,
            url: "/Home/GetListUnoType",
            data: { 'strSelected': strSelected }
        }).success(function (data) {
            $('#uno_type').html(data);
        });
    }

    $('#ModalGarden').on('show.bs.modal', function (e) {
        GetListUnoType('');
    });


});
