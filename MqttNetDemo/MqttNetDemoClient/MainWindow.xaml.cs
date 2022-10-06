using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MqttNetDemoClient
{
    /// <summary>
    /// https://www.bilibili.com/video/BV1i7411y7VJ/
    /// </summary>
    public partial class MainWindow : Window
    {
        IMqttClient client;
        string clientID = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                return;
            }

            clientID = Guid.NewGuid().ToString();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .WithClientId(clientID)
                .WithCredentials("admin", "123456")
                .Build();

            client = new MqttFactory().CreateMqttClient();
            client.UseConnectedHandler(async c =>
            {
                //订阅服务器消息
                await client.SubscribeAsync(new TopicFilterBuilder()
                                    .WithTopic(clientID)
                                    .Build());
            })
            //断开连接
            .UseDisconnectedHandler(c =>
            {
                ShowLog(c.Exception.Message);
            })
            //订阅消息
            .UseApplicationMessageReceivedHandler(c =>
            {
                string msg = Encoding.UTF8.GetString(c.ApplicationMessage.Payload);
                ShowLog(msg);
            });
            await client.ConnectAsync(options);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (client == null)
            {
                return;
            }

            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(clientID)
                .WithPayload($"hello, world. {DateTime.Now}")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            client.PublishAsync(msg);
        }

        /// <summary>
        /// 日志显示
        /// </summary>
        /// <param name="msg"></param>
        private void ShowLog(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtLog.Text += msg + Environment.NewLine;
            });
        }
    }
}
