<template>
    <div>
        <a @click="redirect" class="header-elements button-header" href="/Chat">Chat</a>
        <a @click=logout() class="header-elements button-header" href="/">Logout</a>
    </div>
</template>

<script>
    import $ from 'jquery';
    export default ({
        props: {
            url: ""
        },
        data() {
            return {
            }
        },
        methods: {
            logout() {
                $.ajax({
                    url: "/api/Users/Logout",
                    method: "POST",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function () {
                        location.href = "/";
                    }
                });
            },
            redirect() {
                location.href = "/Chat"
            }
        }
    })
</script>