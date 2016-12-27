$(document).ready(function () {
    EditImage();
    AddImage();

    $('.edit-garden').click(function () {
        var objgarden = $(this).parents('.x_panel');
        var gardenid = $(objgarden).attr('garden_id');
        LoadInfoGarden(gardenid);

        $('#ModalGarden').modal('show');
        var objgarden = $(this).parents('.x_panel');
        var gardenname = $(objgarden).find('.x_title').find('h2').text();
        var gardenimg = $(objgarden).find('.x_content').find('img').attr('src');
        $('#preview-add-garden').attr('src', gardenimg);
    });

    $('.dashboard-div-wrapper').click(function () {
        var isActive = $(this).parent().attr('isactive');
        if (isActive == 1)
            window.location = $(this).attr('url');
        else
            toastr.warning("Khu vườn chưa active !");
    })

    $('.delete-garden').on('click', function () {
        var gardenid = $(this).parents('.x_panel').attr('garden_id');
        DeleteGarden(gardenid);
    });


    $('#addgarden').on('click', function () {
        // validate before add
        var tokenkey = $('#garden-id').val();
        var gardenname = $('#addtext-garden').val();
        var img = $('#preview-add-garden').attr('src');
        var unotype = $('#uno_type').val();
        var acreage = $('#acreage-garden').val();
        var address = $('#address-garden').val();
        var description = $('#note-garden').val();

        //get latitude and longitude by address
        //var pos = GetPositionByAddress(address);

        var lat = 0;
        var lon = 0;
        //var lat = $('#latite').val();
        //var lon = $('#longitude').val();
        $('#addtext-garden').focus();

        // validate
        var rowData = {
            'TOKEN_KEY': tokenkey,
            'ACREAGE': acreage,
            'ADDRESS': address,
            'GARDEN_NAME' : gardenname
        };

        var resValidate = ValidateGarden(rowData);
        if (IsNullOrEmpty(resValidate) == false) {
            toastr.warning(resValidate);
            return false;
        }
        AddGarden(gardenname, img, unotype, acreage, address, lat, lon, description, tokenkey);
    });

    function AddGarden(gardenname, img, unotype, acreage, address, lat, lon, description, tokenkey) {
        $.ajax({
            method: "POST",
            url: "/Home/AddGarden",
            dataType: "JSON",
            data: {
                'gardenname': gardenname, 'image': img, 'unotype': unotype, 'acreage': acreage,
                'address': address, 'lat': lat, 'lon': lon, 'description': description, 'tokenkey': tokenkey
            }
        }).success(function (msg) {
            $("#ModalGarden").modal("hide");
            if (msg.GARDEN_ID == null) {
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

                    $('#addtext-garden').val(gardenname);
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
            else {
                // for edit
                $('div[garden_id="' + msg.GARDEN_ID + '"]').find('h2').text(msg.GARDEN_NAME);
                $('div[garden_id="' + msg.GARDEN_ID + '"]').find('.x_content').find('img').attr('src', '/' + (msg.IMAGE == null ? "Content/Images/bg_no_image.gif" : msg.IMAGE));
            }
        });
    }

    function LoadInfoGarden(tokenkey) {
        $.ajax({
            method: "POST",
            url: "/Home/GetGardenById",
            dataType: "JSON",
            data: { 'tokenkey': tokenkey }
        }).success(function (msg) {
            $('#addtext-garden').val(msg.GARDEN_NAME);
            // load arduino type
            GetListUnoType(msg.UNO_TYPE);
            $('#acreage-garden').val(msg.ACREAGE);
            $('#address-garden').val(msg.ADDRESS);
            $('#note-garden').val(msg.DESCRIPTION);
            $('#garden-id').val(msg.TOKEN_KEY);
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

    function GetTokenKey() {
        var currentLocation = window.location.pathname;
        return currentLocation.split('/')[3];
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

    // get current location
    $('#getLocation').on('click', function () {

        // token key : AIzaSyDYwEnWeO39XsxmVB3hydF4r6hlgNQ6wqM
        // set lat,lon

    });

    // Get latitude/longitude in google map API by address.
    var GetPositionByAddress = function (address) {
        var latitude, longitude;
        initMap();
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var latitude = results[0].geometry.location.lat();
                var longitude = results[0].geometry.location.lng();
            }
        });

        return [latitude, longitude];
    };

    function initMap() {
        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 8,
            center: { lat: -34.397, lng: 150.644 }
        });
    }

    function ValidateGarden(rowdata) {
        if (IsNullOrEmpty(rowdata.GARDEN_NAME)) {

            return "Vui lòng nhập tên khu vườn !";
        }
        else if (IsNullOrEmpty(rowdata.ACREAGE)) {

            return "Vui lòng nhập diện tích !";
        }
        else if (IsNullOrEmpty(rowdata.ADDRESS)) {

            return "Vui lòng nhập địa chỉ !";
        }
    }

});
