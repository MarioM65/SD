using System;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace sd
{
    public partial class MainWindow : Window
    {
        private SerialPort serial;
        private int tentativas = 0;
        private const int maxTentativas = 3;
        private bool conectado = false;

        public MainWindow()
        {
            InitializeComponent();
            CarregarPortas();
            CarregarBauds();
        }

        private void CarregarPortas()
        {
            string[] portas = SerialPort.GetPortNames();
            ComboPorta.ItemsSource = portas;
            if (portas.Any()) ComboPorta.SelectedIndex = 0;
        }

        private void CarregarBauds()
        {
            int[] bauds = { 9600, 14400, 19200, 38400, 57600, 115200 };
            ComboBaud.ItemsSource = bauds;
            ComboBaud.SelectedItem = 9600;
        }

        private void BtnConectar_Click(object sender, RoutedEventArgs e)
        {
            if (ComboPorta.SelectedItem == null || ComboBaud.SelectedItem == null)
            {
                StatusText.Text = "Selecione a porta e a velocidade.";
                return;
            }

            string porta = ComboPorta.SelectedItem.ToString();
            int baud = (int)ComboBaud.SelectedItem;

            try
            {
                serial = new SerialPort(porta, baud);
                serial.Open();
                conectado = true;

                StatusText.Text = $"Conectado à {porta} @ {baud} bps.";
                BtnEntrar.IsEnabled = true;
                SenhaBox.IsEnabled = true;
            }
            catch (Exception ex)
            {
                conectado = false;
                StatusText.Text = "Erro ao conectar: " + ex.Message;
            }
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            if (!conectado || serial == null || !serial.IsOpen)
            {
                StatusText.Text = "Conecte-se primeiro!";
                return;
            }

            string senha = SenhaBox.Password;

            if (senha == "1234")
            {
                tentativas = 0;
                EnviarSerial("ABRIR");
                StatusText.Text = "✅ Acesso concedido.";
            }
            else
            {
                tentativas++;
                StatusText.Text = $"❌ Senha incorreta. Tentativa {tentativas}/{maxTentativas}";

                if (tentativas >= maxTentativas)
                {
                    EnviarSerial("ALERTA");
                    EnviarEmail();
                    StatusText.Text = "⚠️ Muitas tentativas. Alerta enviado!";
                    tentativas = 0;
                }
            }
        }

        private void EnviarSerial(string comando)
        {
            try
            {
                serial.WriteLine(comando);
            }
            catch
            {
                StatusText.Text = "Erro ao enviar para o Arduino.";
            }
        }

        private void EnviarEmail()
        {
            try
            {
                var from = new MailAddress("sistemadedetacaoealertadeacide@gmail.com", "Sistema de Acesso");
                var to = new MailAddress("mariomuondo65@gmail.com");

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("sistemadedetacaoealertadeacide@gmail.com", "ulih etge izhy crrg"),
                    EnableSsl = true
                };

                var msg = new MailMessage(from, to)
                {
                    Subject = "Tentativas de acesso incorretas",
                    Body = $"O sistema detectou múltiplas tentativas inválidas em {DateTime.Now}."
                };

                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                StatusText.Text = "Erro ao enviar e-mail.";
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (serial != null && serial.IsOpen)
                serial.Close();
            base.OnClosed(e);
        }
    }
}
