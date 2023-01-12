import chat from "./Chat.vue"
import Vue from 'vue'
Vue.prototype.signalR = window.signalR;

new Vue({
    el: "#chat",
    data: {
        user: window.user
    },
    components: {
        chat
    }
})