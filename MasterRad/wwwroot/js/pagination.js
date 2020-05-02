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
        sortIcons = this.getSortIcons();
        $.each(sortIcons, function (index, item) {
            $(item).parent().click(function () {
                pagination.sortClickHandler(this);
            });
        });
    },
    initTextFilters: function () {

    },
    sortClickHandler: function (clickedThCell) {
        var clickedSortIcon = $($(clickedThCell).find('i.sort-icon')[0]);

        var nextState = '';
        if (clickedSortIcon.hasClass('fa-sort'))
            nextState = 'desc';
        else if (clickedSortIcon.hasClass('fa-sort-desc'))
            nextState = 'asc';

        sortIcons = this.getSortIcons();
        $.each(sortIcons, function (index, item) {
            $(item).removeClass('fa-sort-asc');
            $(item).removeClass('fa-sort-desc');
            $(item).addClass('fa-sort');
        });

        if (nextState != '') {
            clickedSortIcon.removeClass('fa-sort');
            clickedSortIcon.addClass(`fa-sort-${nextState}`);
        }
    },
    getSortIcons: function () {
        return tableTh.find('i.sort-icon');
    }
}