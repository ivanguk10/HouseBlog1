"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (UserName, Text, DateTime) {
    var d = new Date();
    var s = (d.getMonth() + 1) + '.' + d.getDate() + '.' + d.getFullYear() + ' ' + d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds();
    var encodedMsg = "<p>Логин:".concat(UserName, "</p>", "<p>Коментарий:", Text, "</p>", "<p>Дата:", s, "</p><br/>");
    var li = document.createElement("li");
    li.innerHTML = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    var Article = document.getElementById("PostInput").value;
    connection.invoke('JoinPostGroup', Article);
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var Article = document.getElementById("PostInput").value;
    var Text = document.getElementById("TextInput").value;
    connection.invoke("SendMessage", Article, Text).catch(function (err) {
        return console.error(err.toString());
    });
    
    event.preventDefault();
});