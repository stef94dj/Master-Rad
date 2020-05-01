function setActive(pageName) {
    var navItems = $("#main-menu-list").find('li');


    $.each(navItems, function (index, item) {
        var itemText = $(item).find('a')[0].innerHTML;
        if (itemText == pageName)
            $(item).css('font-weight', 'bold');
        else
            $(item).css('font-weight', 'normal');
    });
}