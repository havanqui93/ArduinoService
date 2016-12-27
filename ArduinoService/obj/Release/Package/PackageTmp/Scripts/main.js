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

});