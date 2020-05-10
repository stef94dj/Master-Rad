var actionsModal = {
    Selector: null,
    DataAttributes: null,
    OnShowHandler: null,
    Init: function (modalSelector, attrs, onShowHandler) {
        actionsModal.Selector = modalSelector;
        actionsModal.DataAttributes = attrs;
        actionsModal.OnShowHandler = onShowHandler;

        bindModalOnShow(modalSelector, this.OnShow);
    },
    OnShow: function (element, event) {
        var button = $(event.relatedTarget)

        $.each(actionsModal.DataAttributes, function (index, attr) {
            actionsModal[attr] = button.data(attr);
        });

        actionsModal.OnShowHandler();
    },
    drawActionsBtn: function (modalselector, dataAttributeVals) {
        var dataAttributesHtml = '';

        $.each(actionsModal.DataAttributes, function (index, attr) {
            dataAttributesHtml += ` data-${attr}="${dataAttributeVals[attr]}"`;
        });

        return `<button data-toggle="modal" 
                        data-target="${modalselector}" 
                        ${dataAttributesHtml}  
                        type="button"
                        class="btn btn-outline-dark btn-sm center">
                    Actions
                </button>`;
    }
}