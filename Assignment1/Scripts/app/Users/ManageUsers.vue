<template>
    <div class="component">
        <button class="header-elements button-header" @click=getUsers()>Get Users</button>
        <button class="header-elements button-header" @click=openAddModal()>Add User</button>
        <div class="card-parent">
            <div class="card-element" v-for="(user, index) in users" :key="user.id">
                <div><strong>Username: </strong>{{user.username}}</div>
                <div><strong>First Name: </strong>{{user.firstName}}</div>
                <div><strong>Last Name: </strong>{{user.lastName}}</div>
                <div><strong>Address: </strong>{{user.address}}</div>
                <button class="header-elements button-header" @click=openEditModal(user)>Edit</button>
                <button class="header-elements button-header" @click=openDeleteModal(user)>Delete</button>
            </div>
        </div>

        <div v-show="showEditModal" id="edit-user-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showEditModal = false;">&times;</span>
                <form @submit.prevent>
                    <label class="modal-label" for="username">Username</label>
                    <input id="username" class="modal-input" type="text" placeholder="Username" v-model:value="newUser.Username" required />
                    <label class="modal-label" for="first-name">First Name</label>
                    <input id="first-name" type="text" class="modal-input" placeholder="First Name" v-model:value="newUser.FirstName" required />
                    <label class="modal-label" for="last-name">Last Name</label>
                    <input id="last-name" type="text" class="modal-input" placeholder="Last Name" v-model:value="newUser.LastName" required />
                    <label class="modal-label" for="user-address">Address</label>
                    <input id="user-address" type="text" class="modal-input" placeholder="Address" v-model:value="newUser.Address" required />
                    <button class="header-elements button-header" @click=editUser(newUser.Id)>Submit</button>
                </form>
            </div>
        </div>
        <div v-show="showDeleteModal" id="delete-user-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showDeleteModal = false;">&times;</span>
                <div>Are you sure you want to delete the user?</div>
                <button class="header-elements button-header" @click=deleteUser(userToDelete)>Yes</button>
                <button class="header-elements button-header" @click="showDeleteModal = false">Cancel</button>
            </div>
        </div>
        <div v-show="showAddModal" id="add-user-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showAddModal = false;">&times;</span>
                <form @submit.prevent>
                    <label class="modal-label" for="username">Username</label>
                    <input id="username" class="modal-input" type="text" placeholder="Username" v-model:value="newUser.Username" required />
                    <label class="modal-label" for="first-name">First Name</label>
                    <input id="first-name" type="text" class="modal-input" placeholder="First Name" v-model:value="newUser.FirstName" required />
                    <label class="modal-label" for="last-name">Last Name</label>
                    <input id="last-name" type="text" class="modal-input" placeholder="Last Name" v-model:value="newUser.LastName" required />
                    <label class="modal-label" for="user-address">Address</label>
                    <input id="user-address" type="text" class="modal-input" placeholder="Address" v-model:value="newUser.Address" required />
                    <label class="modal-label" for="password">Password</label>
                    <input id="password" type="password" class="modal-input" placeholder="Password" v-model:value="newUser.Password" required />
                    <button class="header-elements button-header" type="submit" @click=addUser()>Submit</button>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
    export default({
        data() {
            return {
                users: [],
                newUser: {
                    Id: "",
                    Username: "",
                    FirstName: "",
                    LastName: "",
                    Address: ""
                },
                showEditModal: false,
                showAddModal: false,
                showDeleteModal: false,
                userToDelete: {}
            }
        },
        methods: {
            getUsers() {
                var self = this;
                $.ajax({
                    url: "/api/Users/GetUsers",
                    method: "GET",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function (data) {
                        var filtered = data.filter(u => u.roles[0] !== "Admin");
                        self.users = filtered.filter(u => u.username !== "none");
                    }
                });
            },
            openEditModal(user) {
                this.showEditModal = true;

                this.newUser.Id = user.id;
                this.newUser.Address = user.address;
                this.newUser.FirstName = user.firstName;
                this.newUser.LastName = user.lastName;
                this.newUser.Username = user.username;
            },
            editUser(userId) {
                if (!this.newUser.Address || !this.newUser.FirstName || !this.newUser.LastName || !this.newUser.Username) {
                    return;
                }
                var self = this;
                $.ajax({
                    url: "/api/Users/UpdateUser",
                    method: "PUT",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    data: JSON.stringify({
                        Id: userId,
                        FirstName: this.newUser.FirstName,
                        Address: this.newUser.Address,
                        LastName: this.newUser.LastName,
                        Username: this.newUser.Username,
                        Roles: []
                    }),
                    success: function (data) {
                        self.users.forEach(u => {
                            if (u.id === userId) {
                                u.address = self.newUser.Address;
                                u.firstName = self.newUser.FirstName;
                                u.lastName = self.newUser.LastName;
                                u.username = self.newUser.Username;
                            }
                        })

                        self.showEditModal = false;
                    }
                });
            },
            openDeleteModal(user) {
                this.showDeleteModal = true;

                this.userToDelete = user;
            },
            deleteUser(user) {
                var self = this;
                $.ajax({
                    url: "/api/Users/DeleteUser/" + user.id,
                    method: "DELETE",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function (data) {
                        var index = self.users.indexOf(user);
                        self.users.splice(index, 1);

                        self.showDeleteModal = false;
                    }
                });
            },
            
            openAddModal() {
                this.showAddModal = true;

                this.newUser.Address = "";
                this.newUser.FirstName = "";
                this.newUser.LastName = "";
                this.newUser.Username = "";
                this.newUser.Password = "";
            },
            addUser() {
                if (!this.newUser.Address || !this.newUser.FirstName || !this.newUser.LastName || !this.newUser.Username || !this.newUser.Password) {
                    return;
                }
                var self = this;
                $.ajax({
                    url: "/api/Users/Register",
                    method: "POST",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    data: JSON.stringify({
                        Username: this.newUser.Username,
                        FirstName: this.newUser.FirstName,
                        LastName: this.newUser.LastName,
                        Address: this.newUser.Address,
                        Password: this.newUser.Password
                    }),
                    success: function (data) {
                        self.getUsers();
                        self.users.push(data);

                        self.showAddModal = false;
                    }
                });
            }
        }
    })

</script>