using System.Diagnostics;
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
        private Random veletlen; //random számgenerátor
        private DateTime kezdoIdo;
        private int maxJatekido;
        private bool ervenyes;
        private DispatcherTimer dtIdozito; //idokozonként ad egy jelzést, h tegyük át a gombot új poziba -> csúszkával ezt állítod

        public MainWindow()
        {
            InitializeComponent(); //inicializál, ez rajzolja ki/adja be magát az ablakot

            dtIdozito = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500), //egymás utáni időszakok definiálása -nap,óra,perc,szekundum,miliszekundum - 500ms
                IsEnabled = false
            };

            //feliratkozás - nem emberi beavatkozás váltja ki, hanem a program működésének egy adott ciklusában történik valami pl. Tick eseményre iratkozol fel, ez kezel bizonyos változásokat

            dtIdozito.Tick += DtIdozito_Tick; //+= a feliratkozás valamire, a -= a leiratkozás

            slCsuszka.Minimum = 100;
            slCsuszka.Maximum = 1500;
            slCsuszka.TickFrequency = 200;
            slCsuszka.SmallChange = 100;
            slCsuszka.LargeChange = 500;
            slCsuszka.Value = 500;

            llMin.Content = slCsuszka.Minimum + "ms";
            llMax.Content = slCsuszka.Maximum + "ms";

            btKapjEl.IsEnabled = false; //játék kezdete előtt ne lehessen nyomkodni a gombot
            maxJatekido = 15;

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
            Title = $"Találatok: {eredmeny}";//, Időzítés: {slCsuszka.Value,7:F2} ms, " + $"Még hátravan: {Math.Max(0, maxJatekido - Elteltido()),5:F2} s";


        }


        private void DtIdozito_Tick(object? sender, EventArgs e)
        {
            FeliratKiir();
            pbVegrehajtasJelzo.Value = Elteltido();
            if (Elteltido() < maxJatekido)
            {
                btKapjEl.SetValue(LeftProperty, veletlen.NextDouble() * (cvLap.ActualWidth - btKapjEl.Width)); //az ablak szélességéből és magasságából kivonom a gomb szélességét és magassságát - ne húzhassa ki az ablakból
                btKapjEl.SetValue(TopProperty, veletlen.NextDouble() * (cvLap.ActualHeight - btKapjEl.Height));

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
            FeliratKiir();

        }

        private void btKapjEl_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Kattintás! Érvényes: {ervenyes}");
            //if (!ervenyes) return;
            eredmeny++;
            FeliratKiir();
            //Ha ezt bekapcsoljuk akkor addig letiltja a gombot míg az új pozicióba nem kerül
            // btKapjEl.IsEnabled = false;
        }

        private void btKapjEl_MouseEnter(object sender, MouseEventArgs e)
        {
            //a kattintás érvényes területen történt-e
            ervenyes = true;
        }

        private void btKapjEl_MouseLeave(object sender, MouseEventArgs e)
        {
            //Nem a gombon kattintottunk
            ervenyes = false;
        }

        private void slCsuszka_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Tároljuk, hogy most folyik-e játék. 
            bool vanJatek = dtIdozito.IsEnabled;
            // Időzítő letiltása az intervallum módosítás miatt. 
            dtIdozito.IsEnabled = false;
            // Időzítő intervallumának beállítása. 
            dtIdozito.Interval = new TimeSpan(0, 0, 0, 0, (int)slCsuszka.Value);
            // A játék közben mozgatja a felhasználó a csúszkát, akkor
            if (vanJatek) dtIdozito.IsEnabled = true;
            // Eredmény megjelenítése az ablak fejlécében. 
            FeliratKiir();
        }

    }
}