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
using MQTTnet;
using MQTTnet.Server;

namespace MqttNetDemoServer
{
    /// <summary>
    /// https://www.bilibili.com/video/BV1i7411y7VJ/
    /// </summary>
    public partial class MainWindow : Window
    {
        IMqttServer server;
        List<UserInstance> users;

        public MainWindow()
        {
            InitializeComponent();
            users = new List<UserInstance>();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (server != null)
            {
                return;
            }

            var optionBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1883)
                .WithDefaultEndpoint()
                //连接认证
                .WithConnectionValidator(c =>
                {
                    var flag = !string.IsNullOrWhiteSpace(c.Username) && !string.IsNullOrWhiteSpace(c.Password);
                    if (!flag)
                    {
                        c.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                        return;
                    }

                    c.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
                    //加入用户
                    users.Add(new UserInstance()
                    {
                        clientID = c.ClientId,
                        userName = c.Username,
                        password = c.Password,
                    });
                    ShowLog($"{DateTime.Now}, 帐号:{c.Username}已订阅！");
                })
                //消息订阅
                .WithSubscriptionInterceptor(c =>
                {
                    if (c == null) return;
                    c.AcceptSubscription = true;
                    ShowLog($"{DateTime.Now}, 订阅者:{c.ClientId}");
                })
                //消息接收
                .WithApplicationMessageInterceptor(c =>
                {
                    if (c == null) return;
                    c.AcceptPublish = true;
                    string msg = c.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(c.ApplicationMessage?.Payload);
                    ShowLog($"{DateTime.Now}, 消息:{msg}, 来自:{c.ClientId}");
                });

            server = new MqttFactory().CreateMqttServer();
            //客户端断开连接
            server.UseClientDisconnectedHandler(c =>
            {
                var user = users.FirstOrDefault(p => p.clientID == c.ClientId);
                if (user != null)
                {
                    users.Remove(user);
                    ShowLog($"{DateTime.Now}, 订阅者:{c.ClientId} 已退出！");
                }
            });
            await server.StartAsync(optionBuilder.Build());
        }

        /// <summary>
        /// 发消息给客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (server == null)
            {
                return;
            }

            users.ForEach(async p =>
            {
                //Topic就是订阅者ID，循环发送，所以每个订阅者都能收到一次
                await server.PublishAsync(new MqttApplicationMessage()
                {
                    Topic = p.clientID,
                    QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce,
                    Retain = false,
                    Payload = Encoding.UTF8.GetBytes($"服务器：各位好啊~ {DateTime.Now}"),
                });
            });
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
