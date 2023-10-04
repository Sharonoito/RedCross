
let counter = 1;

function ShowHandOverRequest() {

    Swal.fire({
        title: "Human Hand Over",
        text: "A client needs Mental Support!",
        icon: "warning",
        position: "top-end",
        showCancelButton: !1,
        confirmButtonText: "Handle Case",
        showClass: {
            popup: "animate__animated animate__tada"
        },
        customClass: {
            confirmButton: "btn btn-primary ",
            cancelButton: "btn btn-label-secondary"
        },
        buttonsStyling: !1
    }).then(function (t) {
        t.value && Swal.fire({
            icon: "success",
            title: "Deleted!",
            text: "Your file has been deleted.",
            customClass: {
                confirmButton: "btn btn-success"
            }
        })
    })

}

setInterval(function () {

    CheckForHumanHandOverRequests();

    counter++;

}, 2000);


function CheckForHumanHandOverRequests() {

    $.post("/Conversation/CheckHandOverRequests/").then(response => {
        console.log("high_response",response)
    })

}

$(document).ready(function () {


})
