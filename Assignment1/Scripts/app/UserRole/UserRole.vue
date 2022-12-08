<template>
    <div class="component">
        <button class="button-header header-elements" @click=getDevicesForUser()>View devices</button>
        <div class="card-parent">
            <div class="card-element" v-for="device in devices" :key="device.Id">
                <div><strong>Description: </strong>{{device.description}}</div>
                <div><strong>Address: </strong>{{device.address}}</div>
                <div><strong>Max consumption: </strong>{{device.maxHourlyConsumption}} units/h</div>
                <div><strong>User: </strong>{{user}}</div>
                <button class="button-header header-elements" @click=openSelectTimeModal(device)>View Consumption</button>
            </div>
        </div>
        <div v-show="showSelectTimeModal" id="select-time-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showSelectTimeModal = false;">&times;</span>
                <input type="date" v-model:value="date" required />
                <button class="button-header header-elements" @click=showChart()>Submit</button>
            </div>
        </div>
        <div v-show="showEnergyConsumption" id="consumption-modal" class="modal">
            <div class="modal-content">
                <span class="close-modal" @click="showEnergyConsumption = false;">&times;</span>
                <chart v-if="showEnergyConsumption" :chart-data-prop=chartD></chart>
            </div>
        </div>
        <div id="snackbar">{{message}}</div>
    </div>
</template>

<script>
    import chart from './Chart.vue';
    import $ from 'jquery';
    
    export default ({
        props: {
            user: String
        },
        data() {
            return {
                chartD: {
                    labels: [],
                    datasets: [{
                        label: 'Consumption/h',
                        data: [],
                        backgroundColor: []
                    }]
                },
                devices: [],
                date: "",
                showSelectTimeModal: false,
                showEnergyConsumption: false,
                date: "",
                selectedDevice: {},
                message: ""
            }
        },
        methods: {
            getDevicesForUser() {
                var self = this;
                $.ajax({
                    url: "/api/Device/GetDevicesForUser/" + this.user,
                    method: "GET",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function (data) {
                        self.devices = data;
                    }
                });
            },
            openSelectTimeModal(device) {
                this.showSelectTimeModal = true;

                this.selectedDevice = device;
            },
            showChart() {
                this.showSelectTimeModal = false;
                this.showEnergyConsumption = true;

                this.getConsumption();
            },
            getConsumption() {
                var self = this;
                $.ajax({
                    url: "/api/EnergyConsumption/GetEnergyConsumption/" + this.selectedDevice.id + "&" + this.date,
                    method: "GET",
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    success: function (data) {
                        var parsedDates = [];
                        var consumption = [];
                        var color = [];

                        data.forEach(d => {
                            parsedDates.push(new Date(d.timeStamp).toTimeString().slice(0, 5));
                            consumption.push(d.consumption);
                            color.push("mediumspringgreen")
                        });

                        self.chartD.labels = parsedDates;
                        self.chartD.datasets[0].data = consumption;
                        self.chartD.datasets[0].backgroundColor = color;
                    },
                    error: function () {
                        self.chartD.labels = [];
                        self.chartD.datasets[0].data = [];
                        self.chartD.datasets[0].backgroundColor = [];
                    }
                });
            }
        },
        components: {
            chart
        },
        created: function () {
            self = this;
            this.getDevicesForUser();
            this.connection = new WebSocket((location.protocol == "http:" ? "ws://" : "wss//") + location.host + "/WebSocket/Get")

            this.connection.onmessage = function (event) {
                self.message = event.data;
                var id = self.message.match(/\d+/)[0];


                if (self.devices && 
                    self.devices.filter(element => {
                        if (element.id === id) {
                            return true;
                        }

                        return false;
                    }))
                {
                    var x = document.getElementById("snackbar");

                    x.className = "show";

                    setTimeout(function () { x.className = x.className.replace("show", ""); }, 7000);
                }
            }

            this.connection.onopen = function (event) {
                console.log("Successfully connected to the websocket server...")
            }
        }
    })
</script>