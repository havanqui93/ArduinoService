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
        $('#edittext-garden').val(gardenname);
        $('#gardenid').val(gardenid);
        $('#preview-edit-garden').attr('src', gardenimg);
    });

    $('.dashboard-div-wrapper').click(function () {
        var url = $(this).attr('url');
        window.location = url;
    })

    $('#editgarden').on('click', function () {
        EditGarden();
    })

    $('#addgarden').on('click', function () {
        // validate before add
        var gardenname = $('#addtext-garden').val();
        var img = $('#preview-add-garden').attr('src');
        if (IsNullOrEmpty(gardenname)) {
            toastr.warning("Vui lòng nhập tên khu vườn !");
            return false;
        }
        AddGarden(gardenname, img);
    });

    function AddGarden(gardenname, img) {
        $.ajax({
            method: "POST",
            url: "/Home/AddGarden",
            dataType: "JSON",
            data: { 'gardenname': gardenname, 'image': img }
        }).success(function (msg) {
            var html =
            '<div class="col-md-4 col-sm-4 col-xs-12">'
            + '<div class="x_panel tile fixed_height_275">'
            + '<div class="x_title">'
            + '<h2>' + msg.GARDEN_NAME + '</h2>'
            + '<ul class="nav navbar-right panel_toolbox">'
            + '<li>'
            + '<a class="collapse-link"><i class="fa fa-chevron-up"></i></a>'
            + '</li>'
            + '<li>'
            + '<a class="edit-garden"><i class="fa fa-pencil"></i></a>'
            + '</li>'
            + '</ul>'
            + '<div class="clearfix"></div>'
            + '</div>'
            + '<div class="x_content">'
            + '<div class="box-garden">'
            + '<a href="/Home/Garden/' + msg.GARDEN_ID + '">'
            + '<img src="' + msg.IMAGE + '" />'
            + '</a>'
            + '</div>'
            + '</div>'
            + '</div>'
            + '</div>';
            $('#listgarden').append(html);
            $("#ModalAddGarden").modal("hide");
            $('#addtext-garden').val('');
            $('#preview-add-garden').attr('src', '');
            $('.collapse-link').bind('click');
        });
    }

    function EditGarden() {
        var gardenname = $('#edittext-garden').val();
        var img = $('#preview-edit-garden').attr('src');
        var gardenid = $('#gardenid').val();

        $.ajax({
            method: "POST",
            url: "/Home/EditGarden",
            dataType: "JSON",
            data: { 'gardenname': gardenname, 'image': img, 'gardenid': gardenid }
        }).success(function (msg) {
            $('div[garden_id="' + msg.GARDEN_ID + '"]').find('.x_title').find('h2').text(msg.GARDEN_NAME);
            $('div[garden_id="' + msg.GARDEN_ID + '"]').find('.x_content').find('img').attr('src', msg.IMAGE);
            $("#ModalEditGarden").modal("hide");
            $('#editext-garden').val('');
            $('#preview-edit-garden').attr('src', '');
        });
    }

    function AddImage() {
        $('#fileuploadAdd').fileupload({
            dataType: 'json',
            url: '/Home/UploadFiles',
            autoUpload: true,
            success: function (data) {
                $('#preview-add-garden').attr('src', data);
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
                $('#preview-edit-garden').attr('src', data);
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
                html += '<li><a href="/Home/Control/' + value.GARDEN_ID + '">Hệ thống điều khiển</a></li>';
                html += '<li><a href="/Home/Tracking/' + value.GARDEN_ID + '">Hệ thống theo dõi</a></li>';
                html += '</ul></li>';
            });
            $('#menu-left-garden').html(html);
        });
    }

    function ActiveMenuAndBreabrum() {
        var currentLocation = window.location.pathname;
        var html = '<li><a href="#"><i class="fa fa-home"></i>Home</a></li>';
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
                category = category == 'Control' ? 'Hệ thống điều khiển' : 'Hệ thống theo dõi';
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
        var gardenid = GetGardenId();

        if (IsNullOrEmpty(namecontrol)) {
            toastr.warning("Vui lòng nhập tên khu thiết bị !");
            return false;
        }
        $.ajax({
            method: "POST",
            url: "/Home/AddNewControl",
            data: { 'namecontrol': namecontrol, 'gardenid': gardenid, 'deviceid': '' }
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

    function GetGardenId() {
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
            url: "/Home/AddNewControl",
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

    function AddTracking() {
        var namecontrol = $('#addtext-device').val();
        var gardenid = GetGardenId();

        if (IsNullOrEmpty(namecontrol)) {
            toastr.warning("Vui lòng nhập tên thiết bị !");
            return false;
        }
        $.ajax({
            method: "POST",
            url: "/Home/AddNewControlTracking",
            data: { 'namecontrol': namecontrol, 'gardenid': gardenid, 'deviceid': '' }
        }).success(function (msg) {
            if (msg == true) {
                console.log('success');
                location.reload();
            }
            else
                console.log('false');
        });
    }


});