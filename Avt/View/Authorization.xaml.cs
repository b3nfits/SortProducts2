using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Avt.View
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        private readonly double letterWidth = 40;
        private readonly Random random;
        private readonly string captchaSymbols = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890";
        private readonly Database.TradeEntities entities;
        private Database.User user;
        private bool isRequireCaptcha;
        private string captchaCode;
        
        public Authorization()
        {
            InitializeComponent();
            random = new Random(Environment.TickCount);
            entities = new Database.TradeEntities();
            

        }

        private void OnSignIn(object sender, RoutedEventArgs e)
        {

            ProductView productViewa = new ProductView(entities, user);
           productViewa.Owner = this;
            productViewa.Show();
            Hide();
            return;
            if (isRequireCaptcha && captchaCode.ToLower() != tbCaptcha.Text.Trim().ToLower())
            {
                MessageBox.Show("");
                return;
            }


            string login = tbLogin.Text.Trim();
            string password = tbPassword.Password.Trim();

            user = entities.Users.Where(u => u.UserLogin == login && u.UserPassword == password).FirstOrDefault();
            if (user == null) 
            {
                MessageBox.Show("Некорректный логин или пароль");
                GenerateCaptcha();
                return;
            }
            if (login.Length < 1 || password.Length < 1)
                MessageBox.Show("Введите логин и пароль");
            return;

        }

        private void GenerateCaptcha()
        {
            captchaCode = GetNewCaptchaCode();

            for (int i = 0; i < captchaCode.Length; i++)
            {
                AddCharToCanvas(i, captchaCode[i]);
            }
            GenerageNoize();
        }

        private string GetNewCaptchaCode()
        {

            canvas.Children.Clear();
            string code = "";
            for (int i = 0;i< 4;i++)
            {
                code += captchaSymbols[random.Next(captchaSymbols.Length)];
            }
            return code;
        }

        private void AddCharToCanvas(int index, char ch)
        {

            Label label = new Label();
            label.Content = ch.ToString();
            label.FontSize = random.Next(18, 24);
            label.Width = letterWidth;
            label.Height = 60;
            label.Foreground = new SolidColorBrush(Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)));
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.RenderTransformOrigin = new Point(0.5, 0.5);
            label.RenderTransform = new RotateTransform(random.Next(-20, 15));

            
            canvas.Children.Add(label);

            int startPosition = (int)((canvas.ActualWidth / 2) - (letterWidth * 4 / 2));

            Canvas.SetLeft(label, startPosition + (index * letterWidth));
            
            
        }

        private void GenerageNoize()
        {

            for (int i = 1; i < 100; i++)
            {
                
                

                Ellipse ellipse = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.Black,
                    Stroke = Brushes.Black
                };
                canvas.Children.Add(ellipse);
 

                
            }

            
        }
    }
}
