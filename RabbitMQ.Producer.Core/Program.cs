using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Producer.Core.Configuration;
using RabbitMQ.Producer.Core.Query;
using RabbitMQ.Producer.Core.Utils;
using System;
using System.Text;

namespace RabbitMQ.Producer.Core
{
    public class Program
    {
        public class JsonObjeto
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }

        public static void Main(string[] args)
        {
            var appSettings = FileUtils.ReadFileFromPath("AppSettings.json");
            var producerCoreConfiguration = JsonConvert.DeserializeObject<ProducerCoreConfiguration>(appSettings);

            var factory = new ConnectionFactory()
            {
                HostName = producerCoreConfiguration.RabbitMQHostName,
                VirtualHost = producerCoreConfiguration.RabbitMQVirtualHost,
                Password = producerCoreConfiguration.RabbitMQVPassword,
                UserName = producerCoreConfiguration.RabbitMQUserName
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Q.Producer.Core",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var objectJson = LeituraPlanilhaExcelQuery.GetLinesExcel(producerCoreConfiguration.PathFileExcel);

                foreach (var enfileirando in objectJson)
                {
                    var devolucao = JsonConvert.SerializeObject(enfileirando);

                    string message = devolucao;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "Producer.E",
                                         routingKey: "Q.Producer.Core.Wait",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}