var confirmationModalBuilder = {
    BuildHandler: function () {

        var modalHandler = {
            Selector: null,
            Init: function (modalSelector, onShowHandler, clickHandler) {
                modalHandler.Selector = modalSelector;
                bindModalOnShow(modalSelector, onShowHandler);

                var confirmBtn = $(modalSelector).find('button.modal-confirm-btn')[0];
                $(confirmBtn).click(clickHandler);
            },
            SetText: function (value) {
                var text = $(modalHandler.Selector).find('p.text-content')[0];
                $(text).html(value);
            },
        }

        return modalHandler;
    }
}