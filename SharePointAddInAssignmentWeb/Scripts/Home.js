$(document).ready(function () {
    $('#btn-register').on('click', function (e) {
        $.ajax({
            url: '/Home/Register',
            type: 'GET'
        }).done(function (data) {
            console.log(data);
        });
    });
})