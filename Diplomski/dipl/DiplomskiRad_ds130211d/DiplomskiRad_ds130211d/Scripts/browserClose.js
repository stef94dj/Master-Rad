var isBrowser = true;

//$(document).on("click", "a", function () {
//    debugger;
//    isBrowser = false;
//})

//$(document).on("click", "button", function(){
//    debugger;
//    isBrowser = false;
//})

$('form').on('submit', function () {
    debugger;
    isBrowser = false;
});

$(document).on("click", ".redirectClass", function () {
    debugger;
    isBrowser = false;
})

function unload() {
    debugger;
    if (isBrowser) {
        $.ajax({
            url: '/Home/BrowserClose'
        });
    }
}

