
let counter = 1;

function ShowHandOverRequest(request) {

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

        console.log("Swal", t.value);

        if (t.value) {
            $.post("/Conversation/UpdateHandOverRequest/?id=" + request.id).then(response => {

                console.log("Conversation",response)
                window.location ="/Conversation/"
            })
        }
    })

}

setInterval(function () {

    CheckForHumanHandOverRequests();

    counter++;

}, 2000);

let isShowing = false;


function CheckForHumanHandOverRequests() {

    $.post("/Conversation/CheckHandOverRequests/").then(response => {
        if (response.responseData.length > 0) {

            let request = response.responseData[response.responseData.length - 1];

            if (!isShowing) {
                ShowHandOverRequest(request);

                isShowing = !isShowing; 
            }
        }
    })

}

$(document).ready(function () {


})
