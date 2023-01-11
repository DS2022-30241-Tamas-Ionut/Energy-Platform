<template>
    <form @submit.prevent class="formular">
        <h2>Welcome to Energy Utility Platform!</h2>
        <div>Crete an account and start to manage your devices.</div>
        <label class="form-label" for="username">Username:</label>
        <input class="form-input" id="username" type="text" v-model:value="username" required/>
        <label class="form-label" for="first-name">First Name:</label>
        <input class="form-input" id="first-name" type="text" v-model:value="firstName" required/>
        <label class="form-label" for="last-name">Last Name:</label>
        <input class="form-input" id="last-name" type="text" v-model:value="lastName" required/>
        <label class="form-label" for="address">Address</label>
        <input class="form-input" id="address" type="text" v-model:value="address" required/>
        <label class="form-label" for="password">Password:</label>
        <input class="form-input" id="password" type="password" v-model:value="password" title="Please have at least one uppercase letter, one lowercase letter, one number, one symbol and at least 8 characters" pattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$" required />
        <button type="submit" class="header-elements button-header" @click="register()">Register</button>
    </form>
</template>

<script>
    import $ from 'jquery'

    export default ({
        data() {
            return {
                username: "",
                password: "",
                address: "",
                firstName: "",
                lastName: ""
            }
        },
        methods: {
            register() {
                if (!this.username || !this.password || !this.address || !this.firstName || !this.lastName) {
                    return;
                }
                $.ajax({
                    type: "POST",
                    url: "/api/Users/Register",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    data: JSON.stringify({
                        Username: this.username,
                        Password: this.password,
                        Address: this.address,
                        FirstName: this.firstName,
                        LastName: this.lastName
                    }),
                    success: function () {
                        location.href = "/Login";
                    },
                    error: function (request, status, errorThrown) {
                        console.log("ai gresit ...");
                    }
                });
            }
        }
    })
</script>