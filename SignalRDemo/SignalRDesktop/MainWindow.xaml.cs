using Microsoft.AspNetCore.SignalR.Client;
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

namespace SignalRDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;

        string userName = $"用户{new Random().Next(1000)}";

        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44322/chatHub")
                .Build();

            //上线、离开的通知（订阅）
            connection.On<string>("online", (msg) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    txtInfo.Text += msg + Environment.NewLine;
                });
            });
            //监听消息（订阅）
            connection.On<string, string>("message", (user, msg) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    txtMsg.Text += $"{user}: {msg}{Environment.NewLine}";
                });
            });

            connection.StartAsync();
        }

        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnIn_Click(object sender, RoutedEventArgs e)
        {
            //调用服务方法
            await connection.InvokeAsync("Login", userName);
            btnSend.IsEnabled = true;
        }

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnOut_Click(object sender, RoutedEventArgs e)
        {
            //调用服务方法
            await connection.InvokeAsync("SignOut", userName);
            await connection.StopAsync();
            this.Close();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSend.Text))
            {
                return;
            }

            //调用服务方法
            await connection.InvokeAsync("SendMessage", userName, txtSend.Text.Trim());
        }
    }
}
