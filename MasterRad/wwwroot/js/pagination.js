var pagination = {
    tableTh: null,
    filterTh: null,
    search: null,
    defaultSortKey: 'date_created',
    displayPagesCnt: 3,
    initUI: function (config) {
        tableTh = $(config.tableThSelector);
        filterTh = $(config.filterThSelector);
        search = config.reloadFunction;
        if (config.sortDefaultKey)
            pagination.defaultSortKey = config.sortDefaultKey;
        if (config.displayPagesCnt && config.displayPagesCnt > 0)
            displayPagesCnt = config.displayPagesCnt;

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

        var sortSet = false;
        $.each(sortIcons, function (index, item) {
            if ($(item).hasClass('fa-sort-up') || $(item).hasClass('fa-sort-down')) {
                sortSet = true;
                return false;
            }
        });

        debugger;
        if (!sortSet) {
            $.each(sortIcons, function (index, item) {
                if ($(item).data('table-key') === pagination.defaultSortKey) {
                    $(item).removeClass('fa-sort');
                    $(item).addClass('fa-sort-down');
                    return false;
                }
            });
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