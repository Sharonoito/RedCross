
let counter = 1;

let showHandOverPopup=true

const ACTIVE_CONV = "com.redcross.chat.bot-simple-mde"

function SetActiveConversation(id) {

    console.log(ACTIVE_CONV, id);

    localStorage.setItem(ACTIVE_CONV, id)
}

function GetActiveConversation() {
    return localStorage.getItem(ACTIVE_CONV)
}

function ShowHandOverRequest(request) {

    if (showHandOverPopup) 
    Swal.fire({
        title: "Human Handover",
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

                SetActiveConversation(request.conversationId);

                window.location = "/Conversation/"
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

    if (showHandOverPopup)
    $.post("/Conversation/CheckHandOverRequests/").then(response => {

        if (response.success)
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

    if (GetActiveConversation() != undefined || GetActiveConversation() != "") {




    }

})
