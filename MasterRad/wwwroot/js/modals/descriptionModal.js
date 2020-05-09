var descriptionModal = {
    Selector: null,
    Init: function (modalSelector, clickHandler) {
        descriptionModal.Selector = modalSelector;
        bindModalOnShow(modalSelector, descriptionModal.OnShow);

        var confirmBtn = $(modalSelector).find('button.modal-confirm-btn')[0];
        $(confirmBtn).click(clickHandler);
    },
    OnShow: function (element, event) {
        var textarea = $(descriptionModal.Selector).find('textarea')[0];
        $(textarea).val(actionsModal.description ?? "");

        var title = $(descriptionModal.Selector).find('.modal-title')[0];
        $(title).html(`Update description for '${actionsModal.name}'`)
    },
    GetInputVal: function () {
        var textarea = $(descriptionModal.Selector).find('textarea')[0];
        return $(textarea).val();
    }
}