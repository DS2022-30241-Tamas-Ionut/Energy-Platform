<template>
    <div class="component">
        <button class="header-elements button-header" @click=getAllDevices()>Get Devices</button>
        <button class="header-elements button-header" @click=openAddModal()>Add Device</button>
        <div class="card-parent">
            <div class="card-element" v-for="device in devices" :key="device.Id">
                <div><strong>Description: </strong>{{device.description}}</div>
                <div><strong>Address: </strong>{{device.address}}</div>
                <div><strong>Max consumption: </strong>{{device.maxHourlyConsumption}} units/h</div>
                <div><strong>User: </strong>{{device.username}}</div>
                <button class="header-elements button-header" @click=openEditModal(device)>Edit</button>
                <button class="header-elements button-header" @click=openDeleteModal(device)>Delete</button>
                <button class="header-elements button-header" @click=openChangeUserModal(device)>Change User</button>
            </div>
        </div>
        <div v-show="showEditModal" id="edit-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showEditModal = false;">&times;</span>
                <form @submit.prevent>
                    <label class="modal-label" for="description">Description</label>
                    <input id="description" class="modal-input" type="text" placeholder="Description" v-model:value="newDevice.Description" required />
                    <label class="modal-label" for="address">Address</label>
                    <input id="address" type="text" class="modal-input" placeholder="Address" v-model:value="newDevice.Address" required />
                    <label class="modal-label" for="consumption">Max. Consumption</label>
                    <input id="consumption" type="text" class="modal-input" placeholder="Max. Consumption" v-model:value="newDevice.MaxHourlyConsumption" required />
                    <button class="header-elements button-header" type="submit" @click=editDevice(newDevice.Id)>Submit</button>
                </form>
            </div>
        </div>
        <div v-show="showDeleteModal" id="delete-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showDeleteModal = false;">&times;</span>
                <div>Are you sure you want to delete the device?</div>
                <button class="header-elements button-header" @click=deleteDevice(deviceToDelete)>Yes</button>
                <button class="header-elements button-header" @click="showDeleteModal = false">Cancel</button>
            </div>
        </div>
        <div v-show="showAddModal" id="add-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showAddModal = false;">&times;</span>
                <form @submit.prevent>
                    <label class="modal-label" for="description">Description</label>
                    <input id="description" class="modal-input" type="text" placeholder="Description" v-model:value="newDevice.Description" required />
                    <label class="modal-label" for="address">Address</label>
                    <input id="address" type="text" class="modal-input" placeholder="Address" v-model:value="newDevice.Address" required />
                    <label class="modal-label" for="consumption">Max. Consumption</label>
                    <input id="consumption" type="text" class="modal-input" placeholder="Max. Consumption" v-model:value="newDevice.MaxHourlyConsumption" required />
                    <select v-model:name="selectedUser">
                        <option v-for="user in allUsers" :value="user.id">{{user.username}}</option>
                    </select>
                    <button class="header-elements button-header" type="submit" @click=addDevice(newDevice)>Submit</button>
                </form>
            </div>
        </div>
        <div v-show="showChangeUserModal" id="change-user-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showChangeUserModal = false;">&times;</span>
                <div>Please select a user for this device</div>
                <select v-model:name="selectedUser">
                    <option v-for="user in allUsers" :value="user.username">{{user.username}}</option>
                </select>
                <button @click=changeUser(deviceToChange)>Yes</button>
            </div>
        </div>
    </div>
</template>

<script>
    export default({
        data() {
            return {
                devices: [],
                allUsers: [],
                showEditModal: false,
                showDeleteModal: false,
                showAddModal: false,
                showChangeUserModal: false,
                deviceToDelete: {},
                deviceToChange: {},
                newDevice: {
                    Id: 0,
                    Description: "",
                    Address: "",
                    MaxHourlyConsumption: 0,
                    UserId: ""
                },
                selectedUser: ""
            }
        },
        methods: {
            getAllDevices() {
                var self = this;
                $.ajax({
                    url: "/api/Device/GetDevices",
                    method: "GET",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function (data) {
                        self.getUsers();

                        self.devices = [];

                        data.forEach(device => {
                            self.devices.push({
                                "description": device.description,
                                "address": device.address,
                                "maxHourlyConsumption": device.maxHourlyConsumption,
                                "username": self.allUsers.filter(u => u.id === device.userId)[0].username,
                                "id": device.id
                                })
                        });
                    }
                });
            },
            openEditModal(device) {
                this.showEditModal = true;
                this.newDevice.Id = device.id;
                this.newDevice.Address = device.address;
                this.newDevice.Description = device.description;
                this.newDevice.MaxHourlyConsumption = device.maxHourlyConsumption;
                this.newDevice.UserId = device.userId;
            },
            editDevice(deviceId) {
                if (!this.newDevice.UserId || !this.newDevice.MaxHourlyConsumption || !this.newDevice.Address || !this.newDevice.Description) {
                    return;
                }

                var self = this;
                $.ajax({
                    url: "/api/Device/UpdateDevice",
                    method: "PUT",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    data: JSON.stringify({
                        Id: deviceId,
                        Description: this.newDevice.Description,
                        Address: this.newDevice.Address,
                        MaxHourlyConsumption: this.newDevice.MaxHourlyConsumption,
                        UserId: this.newDevice.UserId
                    }),
                    success: function (data) {
                        self.devices.forEach((dev) => {
                            if (dev.id === deviceId) {
                                dev.address = self.newDevice.Address;
                                dev.description = self.newDevice.Description;
                                dev.maxHourlyConsumption = self.newDevice.MaxHourlyConsumption;
                            }
                        })

                        self.showEditModal = false;
                    }
                });
            },
            openDeleteModal(device) {
                this.showDeleteModal = true;

                this.deviceToDelete = device;
            },
            deleteDevice(device) {
                var self = this;
                $.ajax({
                    url: "/api/Device/DeleteDevice/" + device.id,
                    method: "DELETE",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function (data) {
                        var index = self.devices.indexOf(device);
                        self.devices.splice(index, 1);

                        self.showDeleteModal = false;
                    }
                });
            },
            getUsers() {
                var self = this;
                $.ajax({
                    url: "/api/Users/GetUsers",
                    async: false,
                    method: "GET",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function (data) {
                        self.allUsers = data.filter(u => u.roles[0] !== "Admin");
                    }
                });
            },
            openAddModal() {
                this.showAddModal = true;
                this.getUsers();

                this.newDevice.Address = "";
                this.newDevice.Description = "";
                this.newDevice.MaxHourlyConsumption = 0;
                this.newDevice.UserId = "";
            },
            addDevice() {
                if (!this.newDevice.MaxHourlyConsumption || !this.newDevice.Address || !this.newDevice.Description || !this.selectedUser) {
                    return;
                }
                var self = this;
                $.ajax({
                    url: "/api/Device/CreateDevice",
                    method: "POST",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    data: JSON.stringify({
                        Description: this.newDevice.Description,
                        Address: this.newDevice.Address,
                        MaxHourlyConsumption: this.newDevice.MaxHourlyConsumption,
                        UserId: this.selectedUser
                    }),
                    success: function (data) {
                        self.getAllDevices();

                        var username = self.allUsers.filter(u => u.id === self.selectedUser)[0].username;
                        self.usersToDevices.push(username);
                        self.devices.push(data);

                        self.showAddModal = false;
                    }
                });
            },
            openChangeUserModal(device) {
                this.getUsers();
                this.showChangeUserModal = true;
                this.selectedUser = device.username;

                this.deviceToChange = device;
            },
            changeUser() {
                var self = this;
                $.ajax({
                    url: "/api/Device/DesignateUserToDevice",
                    method: "POST",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    data: JSON.stringify({
                        DeviceId: this.deviceToChange.id,
                        UserName: this.selectedUser
                    }),
                    success: function (data) {
                        self.devices.forEach(device => {
                            if (device.id === self.deviceToChange.id) {
                                device.username = self.selectedUser
                            }
                        });

                        self.showChangeUserModal = false;
                    }
                });
            }
        }
    })

</script>