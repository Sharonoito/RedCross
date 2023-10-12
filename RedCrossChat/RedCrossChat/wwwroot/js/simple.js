
let activeConversation;

let currentLength=0;

let conversartions = [];

let interval = 2500;

let iteration = 0;

let backgrounds = ['primary', 'secondary', 'primary', 'danger', 'info', 'success', 'dark','primary','info','success']



const GetMyConversations = (func) => {
     
    $.post("/Conversation/GetMyConversations").then(response => {

        if (response.success) {
            let data = response.responseData;

            if (data.length == 0) {

                conversartions = data;

                $('#no-chats').fadeIn()

            } else {
              
                UpdateUI(data);
            }

            if (func != undefined)
                func();

        }
    });
}

const UpdateUI=(data)=>{

    if (conversartions.length > currentLength) {
        for (let i = currentLength; i < conversartions.length; i++) {
            AppendToSidebar(conversartions[i]);
        }
    } else if (data != undefined){

        //check for internal changes in the array -> conversations
        for (let i = 0; i < data.length; i++) {
            AppendToSidebar(data[i])
        }

        conversartions = data;

        if (iteration % 5 == 0) {
            interval * 10;
        }

        iteration++;
    }

}

const AppendToSidebar = ({ persona, id, dateCreated,reason }) => {
    //this is to append the conversation ton the side bar

    console.log(id, persona)
  

    $("#chat-list").append(`
                      <li class="chat-contact-list-item " data-id='${id}'>
                        <a class="d-flex align-items-center">
                            <div class="flex-shrink-0 avatar avatar-busy">
                                ${username(persona.chatID)}
                            </div>
                            <div class="chat-contact-info flex-grow-1 ms-3">
                                <h6 class="chat-contact-name text-truncate m-0">${persona.chatID}</h6>
                                <p class="chat-contact-status text-truncate mb-0 text-muted">${reason}</p>
                            </div>
                            <small class="text-muted mb-auto moment-date" data-date='${dateCreated}'></small>
                        </a>
                    </li>`);

    CalculateDates();
}

const username = str => str != null ? `<span class="avatar-initial rounded-circle bg-label-${backgrounds[str.charAt(str.length - 1)]}">${str.charAt(0)}${str.charAt(str.length - 1)}</span>` : '';

function CalculateDates() {

    let cDate = new Date();

    const getDateObject = cDate => {

        return [cDate.getFullYear(), cDate.getMonth(), cDate.getDate(), cDate.getHours(), cDate.getMinutes(), cDate.getMilliseconds()]
    }

   

    $(".moment-date").toArray().forEach(e => {

        
        var now = moment(new Date()); //todays date

        var end = moment($(e).data('date')); // another date

        if ($(e).data('format') != undefined) {

            $(e).html(end.format($(e).data('format')));

            return;
        }

        if (now.diff(end, 'minutes') <= 59) {

            $(e).html(now.diff(end, 'minutes') + (" Minutes" + now.diff(end, 'minutes') !=1? 's':''))
            return;
        }
        if (now.diff(end, 'hours') <=24 ) {

            $(e).html(now.diff(end, 'hours') + (" hour" + now.diff(end, 'days') !=1 ? "s":''))

            return
        }

        if (now.diff(end, 'days') <= 7) {

            $(e).html(now.diff(end, 'days') + (" day" + now.diff(end, 'days') !=1 ? 's':''))

            return
        }
       
    })
}

function UpdateChatActiveConversation(data) {


    if (activeConversation == undefined)
        return;

    const { id, reason, persona, rawConversations } = activeConversation;  

    const a = $("#ConvList");

    let lastConv;

    if (data != undefined && rawConversations.length < data.length) {

        lastConv = data[data.length - 1];

        activeConversation.rawConversations.push(lastConv);

        a.append(message(lastConv))

     
        return;
       // 
    } else if (data != undefined && rawConversations.length == data.length) {

        lastConv = data[data.length - 1];

        let it = rawConversations[rawConversations.length - 1];

       

        if (it.message == null && lastConv.message != null) {

            success("You have a message !")

            a.append(message(lastConv,true))

            rawConversations[rawConversations.length - 1] = lastConv;

            activeConversation.rawConversations = rawConversations;
        }

        return;
    }

    $("#chat-contact-info").html(`<i class="bx bx-menu bx-sm cursor-pointer d-lg-none d-block me-2" data-bs-toggle="sidebar" data-overlay="" data-target="#app-chat-contacts"></i>
                            <div class="flex-shrink-0 avatar">
                                ${username(persona.chatID)}
                            </div>
                            <div class="chat-contact-info flex-grow-1 ms-3" >
                                <h6 class="m-0">${persona.chatID}</h6>
                                <small class="user-status text-muted">${reason}</small>
                            </div>`);


    a.html('');

    for (let i = 0; i < rawConversations.length; i++) {

        const conv = rawConversations[i];

        attachMessage(conv,a)

        CalculateDates()
    }
}

const attachMessage=(conv, a)=>{

    a.append(message(conv)).append(message(conv, true));
}

const message=(conv, isReply=false) => isReply ? response(conv) : question(conv) 

const response = (conv) => `<li class="chat-message">
              <div class="d-flex overflow-hidden">
                <div class="user-avatar flex-shrink-0 me-3">
                  <div class="avatar avatar-sm">
                    <span class="avatar-initial rounded-circle bg-label-success">${ username(activeConversation.persona.chatID) }</span>
                  </div>
                </div>
                <div class="chat-message-wrapper flex-grow-1">
                  <div class="chat-message-text">
                    <p class="mb-0">${ conv.message}</p>
                  </div>
                  <div class="text-muted mt-1">
                        <small class='moment-date' data-format='h:mm' data-date='${ conv.responseTimeStamp}'></small> 
                  </div>
                </div>
              </div>
            </li>`;

const question = (conv) => `<li class="chat-message chat-message-right ">
                            <div class="d-flex overflow-hidden">
                                <div class="chat-message-wrapper flex-grow-1">
                                    <div class="chat-message-text">
                                        <p class="mb-0">${ conv.question }</p>
                                    </div>
                                    <div class="text-end text-muted mt-1">
                                        <i class="bx bx-check-double text-success"></i>
                                        <small class='moment-date' data-format='h:mm' data-date='${conv.questionTimeStamp}'></small>
                                    </div>
                                </div>
                                <div class="user-avatar flex-shrink-0 ms-3">
                                    <div class="avatar avatar-sm">
                                        <span class="avatar-initial rounded-circle bg-label-success">${'BOT'}</span>
                                    </div>
                                </div>
                            </div>
                        </li>`;


const GetConversation = id => {

    $.get("/Conversation/GetConversation?id=" + id).then(resp => {

        activeConversation = resp.responseData;

        UpdateChatActiveConversation();

    })
}
const StartConversation=(id)=>{

    $('.chat-contact-list-item').toArray().forEach(chat => {
        $(chat).removeClass('active');
    });


    if (conversartions.length == 0) {

        console.log("fetch the conv", conversartions);

        return GetConversation(id);
    }


    for (let i = 0; i < conversartions.length; i++) {

        if ( id == conversartions[i].id) {
            activeConversation = conversartions[i];
        }
    }

    console.log("ActiveConversation", conversartions)

    UpdateChatActiveConversation();


    $(".chat-contact-list-item").find("[data-id='" + id + "']").addClass('active')

}

$(document).on('click', '.chat-contact-list-item', function () {

    let current = this;

    StartConversation($(current).data('id'));
});

$(document).on('submit', '.fsend-message', function (e) {

    e.preventDefault();
    
    const a = $("#fsend-imessage");

    if (activeConversation == undefined) {
        error("there is no active conversation");
    }

    //$("#ConvList").append(message({ question: a.val(), questionTimeStamp:"" } , true))

    $.post("/Conversation/CreateResponse", { conversationId: activeConversation.id, question: a.val() }).then(resp => {
        a.val('');
        CheckForUpdatesActiveConversation();

    });


});


function CheckForUpdatesActiveConversation() {

    //GetRawConversations

    if (activeConversation == undefined)
        return;

    $.post("/Conversation/GetRawConversations/?id=" + activeConversation.id).then(resp => {

        
        UpdateChatActiveConversation(resp.responseData)
    })
}



$(document).ready(function () {

    if (GetActiveConversation() != null) {

        console.log("CurrentActiveConversation",GetActiveConversation());

        StartConversation(GetActiveConversation());

    }


    GetMyConversations();

    setInterval(function () {
        CalculateDates();

        CheckForUpdatesActiveConversation();
    }, 5000);

});