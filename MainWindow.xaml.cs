using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ugralogomb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int eredmeny;
        private Random veletlen;
        private DateTime kezdoIdo;
        private int maxJatekido;
        private bool ervenyes;
        private DispatcherTimer dtIdozito; //idokozonként ad egy jelzést, h tegyük át a gombot új poziba

        public MainWindow()
        {
            InitializeComponent(); //inicializál, ez rajzolja ki/adja be magát az ablakot

            dtIdozito = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500), //egymás utáni időszakok definiálása - 500ms
                IsEnabled = false
            };

            //feliratkozás - nem emberi beavatkozás váltja ki, hanem Tick eseményre iratkozol fel, ez kezel bizonyos változásokat

            dtIdozito.Tick += DtIdozito_Tick;

            slCsuszka.Minimum = 100;
            slCsuszka.Maximum = 1500;
            slCsuszka.TickFrequency = 200;
            slCsuszka.SmallChange = 100;
            slCsuszka.LargeChange = 500;

            llMin.Content = slCsuszka.Minimum + "ms";
            llMax.Content = slCsuszka.Maximum + "ms";

            btKapjEl.IsEnabled = false;
            maxJatekido = 10;

            pbVegrehajtasJelzo.Minimum = 0;
            pbVegrehajtasJelzo.Maximum = maxJatekido;
            pbVegrehajtasJelzo.Value = 0;

            veletlen = new Random();

        }

        double Elteltido()
        {
            DateTime most = DateTime.Now;
            return most.Subtract(kezdoIdo).TotalSeconds;
        }

        private void FeliratKiir()
        {
            Title = $"Találatok: {eredmeny}, Időzítés: {slCsuszka.Value,7:F2} ms" +
                $"Még hátravan: {Math.Max(0, maxJatekido - Elteltido()),5:F2} s";
        }


        private void DtIdozito_Tick(object? sender, EventArgs e)
        {
            FeliratKiir();
            pbVegrehajtasJelzo.Value = Elteltido();
            if (Elteltido() < maxJatekido)
            {
                btKapjEl.SetValue(LeftProperty, veletlen.NextDouble() * (cvLap.ActualWidth - btKapjEl.Width));
                btKapjEl.SetValue(LeftProperty, veletlen.NextDouble() * (cvLap.ActualHeight - btKapjEl.Height));

            }
            else
            {
                dtIdozito.IsEnabled = false;
                btKapjEl.IsEnabled = false;
                btStart.IsEnabled = true;
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            eredmeny = 0;
            kezdoIdo = DateTime.Now;
            pbVegrehajtasJelzo.Value = 0;
            dtIdozito.Interval = new TimeSpan(0, 0, 0, 0, (int)slCsuszka.Value);
            btStart.IsEnabled = false;
            btKapjEl.IsEnabled = true;
            dtIdozito.IsEnabled = true;

        }
    }
}