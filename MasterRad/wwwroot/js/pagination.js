var pagination = {
    tableTh: null,
    filterTh: null,
    initUI: function (tableThSelector, filterThSelector) {
        tableTh = $(tableThSelector);
        filterTh = $(filterThSelector);
        this.initPageSize();
        this.initSort();
        this.initTextFilters();
    },
    initPageSize: function () {
        $('#page-size').change(function () {
            pagination.triggerSearch('page size change');
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

        pagination.triggerSearch(`sort`);
    },
    textInputHandler: function (input) {
        var placeholderTxt = $(input).attr('placeholder');
        pagination.triggerSearch(`filter ${placeholderTxt}`);
    },
    triggerSearch: function (triggeredBy) {
        alert(`search triggerred by ${triggeredBy}`);
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