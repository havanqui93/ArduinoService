$(document).ready(function () {
    $('div[style="position: fixed; z-index: 2147483647; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: transparent; margin: 0px; padding: 0px;"]').hide();
    $('div[style="opacity: 0.9; z-index: 2147483647; position: fixed; left: 0px; bottom: 0px; height: 65px; right: 0px; display: block; width: 100%; background-color: #202020; margin: 0px; padding: 0px;"]').hide();


    function GetTokenKey() {
        var currentLocation = window.location.pathname;
        return currentLocation.split('/')[3];
    }

    ActiveMenuAndBreabrum();
    function ActiveMenuAndBreabrum() {
        var currentLocation = window.location.pathname;
        var html = '<li><a href="/Home/MainMenu"><i class="fa fa-home"></i>Trang chủ</a></li>';
        if (currentLocation == '/Home/MainMenu') {
            $('.breadcrumb').html(html);
        } else {
            var category = currentLocation.split('/')[2];
            var id = currentLocation.split('/')[3];

            if (category == 'Garden') {
                $('#menu-left-garden').find('li[attrgarden="' + id + '"]').addClass('active').find('.child_menu').css('display', 'block');
                var title = $('#menu-left-garden').find('li[attrgarden="' + id + '"]').children('a').text();
                html += '<li class="active">Khu vườn</li>';
            }
            else {
                switch (category) {
                    case "Control":
                        html += '<li class="active">Hệ thống điều khiển</li>';
                        break;
                    case "Tracking":
                        html += '<li class="active">Hệ thống theo dõi</li>';
                        break;
                    case "Chart":
                        html += '<li class="active">Biểu đồ thống kê</li>';
                        break;
                    case "SettingGarden":
                        html += '<li class="active">Cài đặt khu vườn</li>';
                        break;
                    default:
                }
            }
            $('.breadcrumb').html(html);
        }
    }

    function GetDateToday() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!

        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        return dd + '/' + mm + '/' + yyyy;
    }

});