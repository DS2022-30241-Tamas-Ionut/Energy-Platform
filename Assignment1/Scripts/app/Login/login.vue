<template>
    <form @submit.prevent class="formular">
        <h2>Welcome to Energy Utility Platform!</h2>
        <div>Please log in to view energy consumption of your devices.</div>
        <label class="form-label" for="username">Username:</label>
        <input class="form-input" id="username" type="text" v-model:value="username" required/>
        <label class="form-label" for="password">Password:</label>
        <input class="form-input" id="password" type="password" v-model:value="password" required/>
        <button type="submit" class="header-elements button-header" @click="login()">Login</button>
    </form>
</template>

<script>
    import $ from 'jquery'

    export default ({
        data() {
            return {
                username: "",
                password: ""
            }
        },
        methods: {
            login() {
                if (!username || !password) {
                    return;
                }
                $.ajax({
                    type: "POST",
                    url: "/api/Users/Login",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    data: JSON.stringify({
                        Username: this.username,
                        Password: this.password
                    }),
                    success: function (data) {
                        location.href = data;
                    }
                });
            }
        }
    })
</script>