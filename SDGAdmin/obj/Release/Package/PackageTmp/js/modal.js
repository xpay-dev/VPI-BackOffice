function getModal(responseCode, strResponse) {
    $('.swal-btn-success').click(function (e) {
        e.preventDefault();
        swal({
            title: "Success",
            text: "Success",
            type: "success",
            confirmButtonClass: "btn-success",
            confirmButtonText: "Success"
        });
    });

    $('.swal-btn-cancel').click(function (e) {
        e.preventDefault();
        swal({
            title: "Failed",
            text: "",
            type: "error",
            confirmButtonClass: "btn-danger"
        });
    });

    if (responseCode == 0) {
        $('.swal-btn-success').click();
    } else {
        $('.swal-btn-cancel').click();
    }

    $('.text-muted').text(strResponse);

    $('.confirm ').hide()

    setInterval(function () {
        $('.confirm ').click()
    }, 2000);
}