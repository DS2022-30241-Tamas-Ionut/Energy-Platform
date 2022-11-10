<script>
    import { Bar } from 'vue-chartjs'
    export default {
        extends: Bar,
        props: {
            chartDataProp: {
                labels: [],
                datasets: [{
                    label: 'Bar Chart',
                    data: []
                }]
            },
        },
        data() {
            return {
                chartData: this.chartDataProp, 
                options: {
                    legend: {
                        display: true
                    },
                    responsive: true,
                    maintainAspectRatio: false
                }
            }
        },
        mounted() {
            this.renderChart(this.chartData, this.options)
        }
    }
</script>