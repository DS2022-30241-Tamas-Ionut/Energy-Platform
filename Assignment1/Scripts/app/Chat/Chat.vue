<template>
    <div @click="readMessage()" class="chat-parent">
        <div v-show="typingMessage" class="chat-typing">{{typingMessage}}</div>
        <div>
            <ul class="chat-box" id="messagesList">

            </ul>
        </div>
        <div class="send-div">
            <input type="text" v-model:value="message" @keydown="sendIsTyping" class="form-input send-input" placeholder="Type your message here ..."/>
            <button :disabled='isDisabled' @click="sendMessage" class="button-header send-button">Send</button>
        </div>
    </div>
</template>

<script>
    export default ({
        props: {
            user: String
        },
        data() {
            return {
                isDisabled: true,
                message: '',
                connection: {},
                typingMessage: "",
                read: [],
                readMsg: "",
                messageSent: false
            }
        },
        methods: {
            sendMessage() {
                if (this.message != "") {
                    this.connection.invoke("SendMessage", this.user, this.message);
                    this.message = "";
                    this.connection.invoke("NotTyping")
                    this.read = [];
                    this.readMsg = ""
                    this.messageSent = true;
                    var l = document.getElementById('class');
                    l.remove()
                }
            },
            sendIsTyping() {
                if (this.message != "") {
                    this.connection.invoke("SendIsTyping", this.user)
                } else {
                    this.connection.invoke("NotTyping")
                }
            },
            readMessage() {
                if (this.messageSent) {

                    this.connection.invoke("Read", this.user)
                }
            }
        },
        mounted() {
            var self = this
            this.connection = new this.signalR.HubConnectionBuilder().withUrl("/chatHub").build();
            this.connection.on("ReceiveMessage", function (user, message) {
                var li = document.createElement("li");
                var liPers = document.createElement("li");

                liPers.classList.add("chat-person");

                if (user == self.user) {
                    li.classList.add("chat-message-me")
                } else {
                    li.classList.add("chat-message-someone")
                    liPers.classList.add("someone")
                }

                self.messageSent = true;

                document.getElementById("messagesList").appendChild(liPers);
                document.getElementById("messagesList").appendChild(li);
                li.textContent = `${message}`;
                liPers.textContent = `${user}`;
            });

            this.connection.on("ReceiveTyping", function (user, message) {
                if (user != self.user) {
                    self.typingMessage = message
                } else {
                    self.typingMessage = ""
                }
            })

            this.connection.on("StoppedTyping", function () {
                self.typingMessage = ""
            })

            this.connection.on("MessageRead", function (user) {
                self.readMsg = "Read by "
                if (!self.read.includes(user)) {
                    self.read.push(user)
                }

                for (let i = 0; i < self.read.length - 1; i++){
                    self.readMsg += self.read[i] + ", "
                }

                self.readMsg += self.read[self.read.length - 1]
                if (self.read.length > 1) {
                    var l = document.getElementById('class');
                    if (l != null) {
                        l.textContent = self.readMsg
                    }
                } else {
                    var li = document.createElement("li");
                    
                    document.getElementById("messagesList").appendChild(li);
                    li.id = 'class'
                    li.textContent = self.readMsg
                }
            })

            this.connection.start().then(() => { 
                self.isDisabled = false;
            }).catch((e) => {
                console.log(e)
            })
        }
    })
</script>