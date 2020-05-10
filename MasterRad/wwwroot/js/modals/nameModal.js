var nameModalBuilder = {
    BuildHandler: function () {

        var modalHandler = {
            Selector: null,
            Init: function (modalSelector, onShowHandler, clickHandler) {
                modalHandler.Selector = modalSelector;
                bindModalOnShow(modalSelector, onShowHandler);
                bindModalOnClose(modalSelector, modalHandler.Clear);

                var confirmBtn = $(modalSelector).find('button.modal-confirm-btn')[0];
                $(confirmBtn).click(clickHandler);
            },
            SetInputVal: function (value) {
                var input = $(modalHandler.Selector).find('input')[0];
                $(input).val(value);
            },
            SetTitle: function (value) {
                var title = $(modalHandler.Selector).find('.modal-title')[0];
                $(title).html(value);
            },
            GetInputVal: function () {
                var input = $(modalHandler.Selector).find('input')[0];
                return $(input).val();
            },
            Clear: function () {
                var input = $(modalHandler.Selector).find('input')[0];
                $(input).val('');
                hideModalError(modalHandler.Selector);
            }
        }

        return modalHandler;
    }
}