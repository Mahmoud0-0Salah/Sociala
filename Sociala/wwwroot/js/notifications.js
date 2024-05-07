"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notifications").build();
export default connection;
var notificationSound = new Audio('/notification.mp3');
var soundActivator = document.getElementById("AvtivateNotificationSound");
function createNotificationItem(notification) {
    // Create the parent div element
    var notificationItem = document.createElement('div');
    notificationItem.id = notification['id'];
    notificationItem.classList.add('card', 'bg-transparent-card', 'w-100', 'border-0', 'ps-5', 'mb-3', 'notificationItem');
    console.log(notification['id']);


    // Create the image element and set its attributes
    var img = document.createElement('img');
    img.src = notification['imgUrl'];
    img.alt = 'user';
    img.classList.add('w40', 'position-absolute', 'left-0');

    // Create the h5 element and set its text content
    var h5 = document.createElement('h5');
    h5.classList.add('font-xsss', 'text-grey-900', 'mb-1', 'mt-0', 'fw-700', 'd-block');
    h5.textContent = notification['userName'];
    // Create the span element for the time and append it to the h5 element
    var span = document.createElement('span');
    span.classList.add('text-grey-400', 'font-xsssss', 'fw-600', 'float-right', 'mt-1');
    var createdAt = notification['createdAt'];
    createdAt = new Date(createdAt);
    console.log("1");
    var formattedDate = createdAt.toLocaleString('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    });

    createdAt = formattedDate;
    console.log("2");
    span.textContent = createdAt;
    console.log("3");
    h5.appendChild(span);
    console.log("span created!");

    // Create the h6 element and set its text content
    var h6 = document.createElement('h6');
    h6.classList.add('text-grey-500', 'fw-500', 'font-xssss', 'lh-4');
    h6.textContent = notification['message'];

    // Create the anchor element
    var span = document.createElement("span");
    span.classList.add('float-right', 'font-xsssss', 'float-end', 'mark-link');
    span.textContent = "Mark As Read";
    span.style.color = 'blue';
    span.setAttribute('data-notification-id', notification['id']);
    
    console.log(notification['seen'], typeof (notification['seen']));
    if (!notification['seen']) {
        notificationItem.style.backgroundColor = '#eff5f5';
        h6.appendChild(span);
    }
    // Append child elements to the parent div element
    notificationItem.appendChild(h5);
    notificationItem.appendChild(h6);
    notificationItem.appendChild(img);


    return notificationItem;
}
connection.on("ReceiveMessage", function (message) {

    console.log("received", message, typeof(message));
    var message = JSON.parse(message);
    var notificationItem = createNotificationItem(message);

    var allNotifications = document.getElementById('allNotifications');
    allNotifications.after(notificationItem);
    soundActivator.click();

    var notificationItems = document.querySelectorAll('.notificationItem');
    if (notificationItems.length > 5) {
        
        for (var i = 5; i < notificationItems.length; i++) {
     
            notificationItems[i].remove();
        }
    }

   
});


connection.start().then(function () {
    console.log("Connection Started");
    connection.invoke("SendAllNotifications").catch(function (err) {
        return console.error(err.toString());
    });

}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveNotificationList", function (notificationList) {
    //console.log(notificationList, typeof (notificationList))

    let allNotifications = [];
    for (var notification of notificationList) {
        let notificationObj = JSON.parse(notification);


        allNotifications.push(notificationObj);
    }
    for (var notification of allNotifications) {
        console.log(notification['createdAt']);
        var notificationItem = createNotificationItem(notification);
        var notificationList = document.getElementById("notificationList");
        notificationList.appendChild(notificationItem);

    }

});

connection.on("ActivateNotificationIcon", function () {
    var icon = document.querySelector('#notification-active');
    icon.style.display = 'block';
    console.log(icon);

});

connection.on("DeActivateNotificationIcon", function () {
    var icon = document.querySelector('#notification-active');
    icon.style.display = 'none';
    console.log(icon);

});

connection.on("MarkAsRead", function (notificationId) {
    var notificationItem = document.getElementById(`${notificationId}`);
    notificationItem.style.backgroundColor = 'white';
    var markLinks = notificationItem.getElementsByClassName('mark-link');
    for (var span of markLinks) {
        console.log(span);
        span.remove();
    }
});

soundActivator.addEventListener("click", function () {
    notificationSound.play();
});