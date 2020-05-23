var nameModal = null;
$(document).ready(function () {
    setActive("Exercises");

    initializeTooltips();

    nameModal = nameModalBuilder.BuildHandler();
    nameModal.Init('#create-instance-modal', onInstanceModalShow, createInstance);
});


function onInstanceModalShow() {

}

function createInstance() {

}