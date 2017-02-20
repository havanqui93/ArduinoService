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
            toastr.warning("Khu vườn chưa kích hoạt !");
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
        $.ajaxSetup({ async: false });
        GetPositionByAddress(address);
        $.ajaxSetup({ async: true });

        var lat = $('#latite').val();
        var lon = $('#longitude').val();
        $('#addtext-garden').focus();

        // Set default
        if (lat == '') {
            var lat = -34.397;
            var lon = 150.644;
        }

        // validate
        var rowData = {
            'TOKEN_KEY': tokenkey,
            'ACREAGE': acreage,
            'ADDRESS': address,
            'GARDEN_NAME': gardenname
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
            if (msg.IS_ADD == true) {
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


                    var objgarden = $(this).parents('.x_panel');
                    var gardenid = $(objgarden).attr('garden_id');
                    LoadInfoGarden(gardenid);

                    $('#ModalGarden').modal('show');
                    var objgarden = $(this).parents('.x_panel');
                    var gardenname = $(objgarden).find('.x_title').find('h2').text();
                    var gardenimg = $(objgarden).find('.x_content').find('img').attr('src');
                    $('#preview-add-garden').attr('src', gardenimg);
                    //var gardenname = $(objgarden).find('.x_title').find('h2').text();
                    //var gardenimg = $(objgarden).find('.x_content').find('img').attr('src');
                    //var unotypeid = $(objgarden).attr('uno_type');

                    //$('#addtext-garden').val(gardenname);
                    //$('#gardenid').val(gardenid);
                    //$('#unotypeid').val(unotypeid);
                    //$('#preview-edit-garden').attr('src', gardenimg);
                    //GetListUnoType($('#unotypeid').val());
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
            ResetModal();
        });
    }

    function ResetModal() {
        $('#garden-id').val('');
        $('#addtext-garden').val('');
        $('#acreage-garden').val('1');
        $('#address-garden').val('');
        $('#note-garden').val('');
        $('#preview-add-garden').attr('src', '');

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
            $('#latite').val(msg.LATITUDE);
            $('#longitude').val(msg.LONGITUDE);
            // disable uno type
            $('#uno_type').attr('disabled', true);
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
        ResetModal();
        GetListUnoType('');
        $('#uno_type').attr('disabled', false);
    });

    // get current location
    $('#getLocation').on('click', function () {
        // token key : AIzaSyDYwEnWeO39XsxmVB3hydF4r6hlgNQ6wqM
        // set lat,lon
        GetMyPosition();
        // Set address
        $.ajaxSetup({ async: false });
        GetAddress();
        $.ajaxSetup({ async: true });
    });

    function GetAddress() {
        var lat = $('#latite').val();
        var lng = $('#longitude').val();
        var latlng = new google.maps.LatLng(lat, lng);
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[1]) {
                    // Set location to textbox address
                    //alert("Location: " + results[1].formatted_address);
                    $('#address-garden').val(results[1].formatted_address);
                }
            }
        });
    }


    // Get latitude/longitude my position currrent.
    function GetMyPosition() {
        var map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: -34.397, lng: 150.644 },
            zoom: 6
        });
        var infoWindow = new google.maps.InfoWindow({ map: map });

        // Try HTML5 geolocation.
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var pos = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };

                //infoWindow.setPosition(pos);
                //infoWindow.setContent('Location found.');
                //map.setCenter(pos);
                $('#latite').val(pos.lat);
                $('#longitude').val(pos.lng);
            }, function () {
                handleLocationError(true, infoWindow, map.getCenter());
            });
        } else {
            // Browser doesn't support Geolocation
            handleLocationError(false, infoWindow, map.getCenter());
        }
    }

    function handleLocationError(browserHasGeolocation, infoWindow, pos) {
        //infoWindow.setPosition(pos);
        toastr.error(browserHasGeolocation ?
                              'Error: The Geolocation service failed.' :
                              'Error: Your browser doesn\'t support geolocation.');
    }


    // Get latitude/longitude in google map API by address.
    function GetPositionByAddress(address) {
        getLatitudeLongitude(showResult, address);
    };

    /* This showResult function is used as the callback function*/
    function showResult(result) {
        if (IsNullOrEmpty(result) == false) {
            $('#latite').val(result.geometry.location.lat());
            $('#longitude').val(result.geometry.location.lng());
        }
        else {
            GetPositionByAddress('Hồ Chí Minh');
        }
    }
    function getLatitudeLongitude(callback, address) {
        try {
            // If adress is not supplied, use default value 'Ferrol, Galicia, Spain'
            address = address || 'Hồ Chí Minh';
            // Initialize the Geocoder
            geocoder = new google.maps.Geocoder();
            if (geocoder) {
                geocoder.geocode({
                    'address': address
                }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        callback(results[0]);
                    }
                    else if (status == google.maps.GeocoderStatus.ZERO_RESULTS) {
                        callback(results[0]);
                    }
                });
            }
        } catch (e) {
            $('#latite').val(-34.397);
            $('#longitude').val(150.644);
            //$('#address-garden').val('Hồ Chí');
        }
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
