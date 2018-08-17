$(document).ready(function () {
    var url = window.location.href;

    $('#navbarList a[href="' + url + '"]').addClass('active');
    $('#navbarList a').filter(function () {
        return this.href === url;
    }).addClass('active');
});
