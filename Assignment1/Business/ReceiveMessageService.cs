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
        private readonly IEnergyConsumptionService _energyConsumptionService;
        private ConnectionFactory ConnectionFactory { get; set; }
        private IConnection Connection { get; set; }
        private IModel Channel { get; set; }
        private EventingBasicConsumer Consumer { get; set; }
        private int Count { get; set; }
        public EnergyConsumptionViewModel EnergyConsumptionRead { get; set; }

        public ReceiveMessageService(IEnergyConsumptionService energyConsumptionService)
        {
            _energyConsumptionService = energyConsumptionService;

            ConnectionFactory = new ConnectionFactory() { HostName = "host.docker.internal" };
            Connection = ConnectionFactory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Consumer = new EventingBasicConsumer(Channel);
            Count = 0;
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

                    Count = 0;

                    _energyConsumptionService.AddEnergyConsumption(EnergyConsumptionRead);

                    EnergyConsumptionRead = new EnergyConsumptionViewModel();
                    EnergyConsumptionRead.Consumption += currentEnergyConsumption.Consumption;
                }

            };
            Channel.BasicConsume(queue: "first-queue",
                                 autoAck: true,
                                 consumer: Consumer);
        }
    }
}
