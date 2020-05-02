var pagination = {
    tableTh: null,
    filterTh: null,
    search: null,
    initUI: function (tableThSelector, filterThSelector, reloadFunction) {
        tableTh = $(tableThSelector);
        filterTh = $(filterThSelector);
        search = reloadFunction;
        this.initPageSize();
        this.initSort();
        this.initTextFilters();
    },
    initPageSize: function () {
        $('#page-size').change(function () {
            pagination.triggerSearch();
        })
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
        var inputs = this.getFilterTextInputs();
        $.each(inputs, function (index, item) {
            $(item).keyup(throttle(function () {
                pagination.textInputHandler(this);
            }, 750));
        })
    },
    getSortIcons: function () {
        return tableTh.find('i.sort-icon');
    },
    getFilterTextInputs: function () {
        return filterTh.find('input.filter-text-input');
    },
    sortClickHandler: function (clickedThCell) {
        var clickedSortIcon = $($(clickedThCell).find('i.sort-icon')[0]);

        var nextState = '';
        if (clickedSortIcon.hasClass('fa-sort'))
            nextState = 'down';
        else if (clickedSortIcon.hasClass('fa-sort-down'))
            nextState = 'up';

        sortIcons = this.getSortIcons();
        $.each(sortIcons, function (index, item) {
            $(item).removeClass('fa-sort-up');
            $(item).removeClass('fa-sort-down');
            $(item).addClass('fa-sort');
        });

        if (nextState != '') {
            clickedSortIcon.removeClass('fa-sort');
            clickedSortIcon.addClass(`fa-sort-${nextState}`);
        }

        pagination.triggerSearch();
    },
    textInputHandler: function (input) {
        pagination.triggerSearch();
    },
    triggerSearch: function () {
        search();
    }
}

function throttle(f, delay) {
    var timer = null;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timer);
        timer = window.setTimeout(function () {
            f.apply(context, args);
        },
            delay || 500);
    };
}