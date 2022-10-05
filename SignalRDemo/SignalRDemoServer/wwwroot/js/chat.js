"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
document.getElementById("sendButton").disabled = true;

//用户的进入、离开（订阅）
connection.on("online", function (msg) {
    var li = document.createElement("li");
    li.textContent = msg;
    document.getElementById("messagesList").appendChild(li);
});

//接收到消息后，显示到界面（订阅）
connection.on("message", function (user, msg) {
    var encodedMsg = user + ": " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

//点击按钮时，发送消息（发布）
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    //发送消息（调用服务方法）
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

//点击按钮时，发送消息（系统通知）（发布）
document.getElementById("sendButtonServer").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    //发送消息（调用服务方法）
    connection.invoke("SendMessageByServer", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});