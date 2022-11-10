import userRole from "./UserRole.vue"
import barChart from "./Chart.vue"
import Vue from 'vue'

new Vue({
    el: "#user-view",
    data: {
        user: window.user
    },
    components: {
        barChart,
        userRole
    }
})