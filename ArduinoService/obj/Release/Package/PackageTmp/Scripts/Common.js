
// Check value is null or empty , true : null, false : not null.
function IsNullOrEmpty(value) {
    if (value == '' || value == null || value == undefined)
        return true;
    return false;
}

function ShowLoading() {
    $('#ModalLoading').modal('show');
}

function HideLoading() {
    $('#ModalLoading').modal('hide');
}

function ShowModalConfirm(title, content, functionOk) {
    $('#ModalConfirm .modal-title').text(title);
    $('#ModalConfirm .modal-body p').text(content);
    $('#ModalConfirm').modal('show');
    $('#ModalConfirm').on('shown.bs.modal', function () {
        $('#ModalConfirm .btn-primary').on('click', function () {
            functionOk();
        });
    });
}