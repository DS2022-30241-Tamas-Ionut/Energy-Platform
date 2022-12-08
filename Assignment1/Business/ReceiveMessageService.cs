using Assignment1.Business.Interfaces;
using Assignment1.Models;
using Assignment1.ViewModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Assignment1.Business
{
    public class ReceiveMessageService : IReceiveMessageService
    {
        private readonly IServiceProvider _services;
        private readonly IEnergyConsumptionService _energyConsumptionService;
        private readonly IDeviceService _deviceService;
        private ConnectionFactory ConnectionFactory { get; set; }
        private IConnection Connection { get; set; }
        private IModel Channel { get; set; }
        private EventingBasicConsumer Consumer { get; set; }
        private int Count { get; set; }
        public EnergyConsumptionViewModel EnergyConsumptionRead { get; set; }
        public string Message { get; set; }

        public ReceiveMessageService(IServiceProvider services)
        {
            _services = services;
            _deviceService = _services.CreateScope().ServiceProvider.GetService<IDeviceService>();
            _energyConsumptionService = _services.CreateScope().ServiceProvider.GetService<IEnergyConsumptionService>();
            
            ConnectionFactory = new ConnectionFactory() { HostName = "host.docker.internal" };
            Connection = ConnectionFactory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.QueueDeclare(queue: "first-queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Consumer = new EventingBasicConsumer(Channel);

            Count = 0;
            Message = "";
            EnergyConsumptionRead = new EnergyConsumptionViewModel();
        }

        public void ReceiveMessage()
        {
            Consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var currentEnergyConsumption = (EnergyConsumptionMessageViewModel) JsonSerializer.Deserialize(message, typeof(EnergyConsumptionMessageViewModel));
                Count++;

                Console.WriteLine(" [x] Received {0}", message);

                if (Count <= 5)
                {
                    EnergyConsumptionRead.Consumption += currentEnergyConsumption.Consumption;
                }
                else
                {
                    EnergyConsumptionRead.DeviceId = currentEnergyConsumption.DeviceId;
                    EnergyConsumptionRead.TimeStamp = DateTime.Now;
                    EnergyConsumptionRead.Consumption += currentEnergyConsumption.Consumption;

                    Count = 0;

                    var device = _deviceService.GetDevice(currentEnergyConsumption.DeviceId);
                    
                    if(device.MaxHourlyConsumption < EnergyConsumptionRead.Consumption)
                    {
                        Message = $"Warning! Consumption exeeded max value for device {currentEnergyConsumption.DeviceId}. Actual value: {EnergyConsumptionRead.Consumption} Max value: {device.MaxHourlyConsumption}";
                    }
                    else
                    {
                        _energyConsumptionService.AddEnergyConsumption(EnergyConsumptionRead);
                    }

                    EnergyConsumptionRead = new EnergyConsumptionViewModel();
                }

            };

            Channel.BasicConsume(queue: "first-queue",
                                 autoAck: true,
                                 consumer: Consumer);
        }

        async public Task<string> ConsumeMessage()
        {
            var message = "";
            
            await Task.Run(() =>
            {
                message = Message;
                Message = "";
            });
            
            return message;
        }
    }
}
