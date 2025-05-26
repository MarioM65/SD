using System;
using System.IO.Ports;
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
        }

        private void BtnConectar_Click(object sender, RoutedEventArgs e)
        {
            string porta = TxtPorta.Text.Trim();
            int baud;

            if (!int.TryParse(TxtVelocidade.Text.Trim(), out baud))
            {
                StatusText.Text = "Velocidade inválida.";
                return;
            }

            try
            {
                serial = new SerialPort(porta, baud);
                serial.Open();
                conectado = true;
                StatusText.Text = $"Conectado à {porta} @ {baud} bps.";
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
                StatusText.Text = "Acesso concedido.";
            }
            else
            {
                tentativas++;
                StatusText.Text = $"Senha incorreta. Tentativa {tentativas}/{maxTentativas}";

                if (tentativas >= maxTentativas)
                {
                    EnviarSerial("ALERTA");
                    EnviarEmail();
                    StatusText.Text = "Muitas tentativas. Alerta enviado!";
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
