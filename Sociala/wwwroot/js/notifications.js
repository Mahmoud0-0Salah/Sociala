"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notifications").build();

connection.on("ReceiveMessage", function (message) {

    console.log("received", message, typeof(message));
    var message = JSON.parse(message);
    // Create the parent div element
    var notificationItem = document.createElement('div');
    notificationItem.id = 'notificationItem';
    notificationItem.classList.add('card', 'bg-transparent-card', 'w-100', 'border-0', 'ps-5', 'mb-3');

    // Create the image element and set its attributes
    var img = document.createElement('img');
    img.src = message['imgUrl'];
    img.alt = 'user';
    img.classList.add('w40', 'position-absolute', 'left-0');

    // Create the h5 element and set its text content
    var h5 = document.createElement('h5');
    h5.classList.add('font-xsss', 'text-grey-900', 'mb-1', 'mt-0', 'fw-700', 'd-block');
    h5.textContent = message['userName'];
    // Create the span element for the time and append it to the h5 element
    var span = document.createElement('span');
    span.classList.add('text-grey-400', 'font-xsssss', 'fw-600', 'float-right', 'mt-1');
    span.textContent = ' 3 min';
    h5.appendChild(span);

    // Create the h6 element and set its text content
    var h6 = document.createElement('h6');
    h6.classList.add('text-grey-500', 'fw-500', 'font-xssss', 'lh-4');
    h6.textContent = message['message'];

    // Append child elements to the parent div element
    notificationItem.appendChild(img);
    notificationItem.appendChild(h5);
    notificationItem.appendChild(h6);

    // Append the parent div element to the document body or any other desired container

    var notificationList = document.getElementById("notificationList");
    var h4Element = notificationList.querySelector('h4');
    h4Element.insertAdjacentElement('afterend', notificationItem);
   
});


connection.start().then(function () {
    console.log("Connection Started");
    connection.invoke("SendMessage", "Hi there!!").catch(function (err) {
        return console.error(err.toString());
    });

}).catch(function (err) {
    return console.error(err.toString());
});

