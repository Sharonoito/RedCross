
let activeConversation;

let currentLength=0;

let conversartions = [];

let interval = 2500;

let iteration = 0;


const GetMyConversations=()=>{
    $.post("/Conversation/GetMyConversations").then(response => {

        if (response.success) {
            let data = response.responseData;

            if (data.length == 0) {
                //Show the no messages available

                conversartions = data;

            } else {
                console.log("update the UI")
                // update the interface 
                UpdateUI(data);
            }
        }
        
      
    });
}

const UpdateUI=(data)=>{

    console.log("Updating the UI");

    if (conversartions.length > currentLength) {


       

        console.log("The Ui is being Updated", currentLength)
        console.log("length", conversartions.length)

        for (let i = currentLength; i < conversartions.length; i++) {


            AppendToSidebar(conversartions[i]);
        }
    } else {

        //check for internal changes in the array -> conversations
       
        if (data != undefined)

            for (let i = 0; i < data.length; i++) {
                AppendToSidebar(data[i])
            }

        if (iteration % 5 == 0) {
            interval * 10;
        }

        iteration++;
    }

}

const AppendToSidebar = conv => {
    //this is to append the conversation ton the side bar

    console.log("Append",conv)

    $("#chat-list").append(`
                      <li class="chat-contact-list-item active">
                        <a class="d-flex align-items-center">
                            <div class="flex-shrink-0 avatar avatar-busy">
                                <span class="avatar-initial rounded-circle bg-label-success">CM</span>
                            </div>
                            <div class="chat-contact-info flex-grow-1 ms-3">
                                <h6 class="chat-contact-name text-truncate m-0">${conv.persona.chatID}</h6>
                                <p class="chat-contact-status text-truncate mb-0 text-muted">${conv.reason}</p>
                            </div>
                            <small class="text-muted mb-auto moment-date" data-date='${conv.dateCreated}'></small>
                        </a>
                    </li>`);

    CalculateDates();
}

function CalculateDates() {

    let cDate = new Date();

    const getDateObject = cDate => {

        return [cDate.getFullYear(), cDate.getMonth(), cDate.getDate(), cDate.getHours(), cDate.getMinutes(), cDate.getMilliseconds()]
    }

   

    $(".moment-date").toArray().forEach(e => {

        var now = moment(new Date()); //todays date

        var end = moment($(e).data('date')); // another date

        now.diff(end,'hours')

        var duration = moment.duration(now.diff(end));
        var days = duration.asHours();

        if (now.diff(end, 'minutes') <= 59) {

            $(e).html(now.diff(end, 'minutes')+" Minutes")
            return;
        }
        if (now.diff(end, 'hours') <=24 ) {

            $(e).html(now.diff(end, 'hours') + " hours")

            return
        }

        if (now.diff(end, 'days') <= 7) {

            $(e).html(now.diff(end, 'days') + " days")

            return
        }

        console.log('hours', now.diff(end, 'hours'))
        console.log('minutes', now.diff(end, 'minutes'))

        /**console.log(a.diff(b, 'minutes')) // 44700
console.log(a.diff(b, 'hours')) // 745
console.log(a.diff(b, 'days')) // 31
console.log(a.diff(b, 'weeks')) // 4 */
    })
}

$(document).ready(function () {

    GetMyConversations();

   /* setInterval(function () {

        GetMyConversations();

        console.log(iteration, interval);

    }, interval);*/
});