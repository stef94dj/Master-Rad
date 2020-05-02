var pagination = {
    tableTh: null,
    filterTh: null,
    initUI: function (tableThSelector, filterThSelector) {
        tableTh = $(tableThSelector);
        filterTh = $(filterThSelector);
        this.initSort();
        this.initTextFilters();
    },
    initSort: function () {
        sortIcons = tableTh.find('i.sort-icon');
        $.each(sortIcons, function (index, item) {
            $(item).parent().click(function () {
                pagination.sortClickHandler(this);
            });
        });
    },
    initTextFilters: function () {
        
    },
    sortClickHandler: function (clickedThCell) {
        var nextState = '';
        var sortIcon = $($(clickedThCell).find('i.sort-icon')[0]);
        if (sortIcon.hasClass('fa-sort')) { //none
            nextState = 'desc';
        }
        else if (sortIcon.hasClass('fa-sort-desc')) {
            nextState = 'asc';
        }

        sortIcons = tableTh.find('i.sort-icon');
        $.each(sortIcons, function (index, item) {
            $(item).removeClass('fa-sort-asc');
            $(item).removeClass('fa-sort-desc');
            $(item).addClass('fa-sort');
            $(item).css('color', '#909090');
        });

        if (nextState != '' ) {
            sortIcon.removeClass('fa-sort');
            sortIcon.addClass(`fa-sort-${nextState}`);
            sortIcon.css('color', '#ff3d4f');
        }
    }
}