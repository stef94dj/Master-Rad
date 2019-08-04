
function signIn() {

    $('#porukaID').val("");
    $('#porukaID').hide();
    $('#loadScreenID').show();
    
    var dataForController = {
        KorisnickoIme: $('#inputUsername').val(),
        Lozinka: $('#inputPassword').val()
    };

    $.ajax({
        type: 'POST',
        url: '/Home/SignIn',
        dataType: 'json',
        data: dataForController,
        error: function () {
            alert("greska")
        },
        success: function (response) {            
            if (response.result == 'Redirect') {
                isBrowser = false;
                window.location = response.url;
            }
            else {
                $("#porukaID").val(response.msg);
                $('#porukaID').show();
                $('#loadScreenID').hide();
            }
        }
    });
}




