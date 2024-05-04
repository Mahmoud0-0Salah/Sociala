"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notifications").build();

connection.on("Message", function (message) {

    var audio = new Audio('/message.mp3'); // Replace '/path/to/notification-sound.mp3' with the actual path to your notification sound file
    audio.play();
    // No need to parse the message since it's already in JSON format
    var messageData = JSON.parse(message);

    // Create the message container
    var messageContainer = document.createElement('div');
    messageContainer.classList.add('message');

    // Create the message content element
    var messageContent = document.createElement('div');
    messageContent.classList.add('message-content', 'font-xssss', 'lh-24', 'fw-500');
    messageContent.textContent = messageData.Content;

    // Append message content to message container
    messageContainer.appendChild(messageContent);

    // Append message container to messages section at the end
    var messagesSection = document.querySelector('.messages');
    messagesSection.insertBefore(messageContainer, messagesSection.firstChild);

    // Scroll to bottom of messages
    messagesSection.scrollTop = messagesSection.scrollHeight;

});

connection.start().then(function () {
    console.log("Connection Started");
}).catch(function (err) {
    return console.error(err.toString());
});

function SendMessage(content, receiverId) {
    connection.invoke("CreateMessage", receiverId, content).catch(function (err) {
        return console.error(err.toString());
    });
}
