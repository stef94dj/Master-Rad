var pagination = {
    tableTh: null,
    filterTh: null,
    search: null,
    defaultSortKey: 'date_created',
    displayPagesCnt: 3,
    pageList: null,
    initUI: function (config) {
        pageList = $('ul.pagination');
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
    pageClickHandler: function (pageLink) {
        $(pageLink).blur();

        var txt = $(pageLink).html();

        var doSearch = true;
        if (txt != 'Prev' && txt != 'Next') {
            var pageItems = this.getPageItems();
            $.each(pageItems, function (index, item) {
                $(item).removeClass('active');
            })

            $(pageLink).parent().addClass('active');
        }
        else {
            doSearch = this.prevNextHandler(pageLink);
        }

        if (doSearch)
            this.triggerSearch(true);
    },
    prevNextHandler: function (pageLink) {
        debugger;
        var txt = $(pageLink).html();

        var activePage = this.getActivePage();
        currentSection = this.getSection(activePage);

        var section = txt === 'Prev' ? currentSection - 1 : currentSection + 1;
        var pageCnt = pageList.data('total-pages');
        totalSections = Math.floor(pageCnt / displayPagesCnt);
        if ((pageCnt % displayPagesCnt) != 0)
            totalSections = totalSections + 1;

        if (section < 0 || section >= totalSections)
            return false;

        this.clearPagesUI();
        var html = '';

        html += this.drawPageBtn('Prev', true);

        var startPage = this.getFirstPage(section);
        var lastPage = this.getLastPage(section, pageCnt);

        var newActive = txt === 'Prev' ? lastPage : startPage;
        for (var i = startPage; i <= lastPage; i++) {
            html += this.drawPageBtn(i, true, i === newActive);
        }
        html += this.drawPageBtn('Next', true);

        pageList.html(html);
        return true;
    },
    getSection: function (pageNo) {
        var section = Math.floor(pageNo / displayPagesCnt);
        if ((pageNo % displayPagesCnt) === 0)
            section = section - 1;
        return section;
    },
    triggerSearch: function (goToPage) {
        if (!goToPage) {
            this.clearPagesUI();
        }
        search();
    },
    getFirstPage: function (section) {
        return (section * displayPagesCnt) + 1;
    },
    getLastPage: function (section, pageCnt) {
        var lastPage = (section + 1) * displayPagesCnt;
        if (pageCnt < lastPage)
            lastPage = pageCnt;

        return lastPage;
    },
    drawPagesUI: function (pageCnt, pageNo) {
        debugger;
        this.clearPagesUI();
        var html = '';

        html += this.drawPageBtn('Prev', true);

        var section = this.getSection(pageNo);
        var firstPage = this.getFirstPage(section);
        var lastPage = this.getLastPage(section, pageCnt);

        for (var i = firstPage; i <= lastPage; i++) {
            html += this.drawPageBtn(i, true, i === pageNo);
        }
        html += this.drawPageBtn('Next', true);

        pageList.html(html);
        pageList.data('total-pages', pageCnt);
    },
    clearPagesUI: function () {
        pageList.html('');
    },
    drawPageBtn: function (txt, enabled, active) {
        var disabled = enabled ? '' : "disabled";
        var active = active ? 'active' : '';

        return `<li class="page-item ${disabled} ${active}"><a class="page-link" href="javascript:void(0)" onclick="pagination.pageClickHandler(this)">${txt}</a></li>`
    },
    getPageItems: function () {
        return pageList.find('li');
    },
    getSortIcons: function () {
        return tableTh.find('i.sort-icon');
    },
    getFilterTextInputs: function () {
        return filterTh.find('input.filter-text-input');
    },
    getActivePage: function () {
        var res = 1;
        var pageItems = this.getPageItems();
        $.each(pageItems, function (index, item) {
            if ($(item).hasClass('active')) {
                var aTag = $(item).find('a')[0];
                res = parseInt($(aTag).html());
                return false;
            }
        })
        return res;
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