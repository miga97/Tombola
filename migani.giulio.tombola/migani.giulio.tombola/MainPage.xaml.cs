using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace migani.giulio.tombola
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool estrazioneTerminata;
        public List<string> NumeriEstratti { get; set; }
        public SolidColorBrush ColoreNumeriEstratti { get; set; }
        public Dictionary<string,Control> Bottoni { get; set; }
        public MainPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            estrazioneTerminata = true;
            NumeriEstratti = new List<string>();
            Bottoni = new Dictionary<string, Control>();
            GruppoEstrazioneManuale.Visibility = Visibility.Collapsed;
            ColoreNumeriEstratti = new SolidColorBrush(Colors.Yellow);
            foreach (Control c in gridBottoni.Children)
            {
                if (c.Name.Split('_')[0] == "btn")
                    Bottoni.Add(c.Name.Split('_')[1], c);
            }
        }
        private void btnEstraiManualmente_Click(object sender, RoutedEventArgs e)
        {
            int numero = 0;
            int.TryParse(txtEstrazineManuale.Text, out numero);
            string number;
            if (numero > 0 && numero <= 90)
            {
                if (numero < 10)
                    number = "0" +numero.ToString();
                else
                    number = numero.ToString();
                if (Bottoni[number].Background != ColoreNumeriEstratti)
                {
                    
                    btnNumeroPrecedente.Content = btnNumeroEstratto.Content;
                    btnNumeroEstratto.Content = number;
                    Bottoni[number].Background = ColoreNumeriEstratti;
                    NumeriEstratti.Add(btnNumeroEstratto.Content.ToString());
                    if (NumeriEstratti.Count == 90)
                        btnEstrai.Content = "Ricomincia";
                }
            }
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control c in Bottoni.Values)
            {
                c.Background = new SolidColorBrush(Colors.White);
            }
            btnEstrai.Content = "Estrai Numero";
            NumeriEstratti.Clear();
            btnNumeroEstratto.Content = "X";
            btnNumeroPrecedente.Content = "X";
            pivotMain.SelectedIndex = 1;
        }
        private void btnAnnullaUltimoNumero_Click(object sender, RoutedEventArgs e)
        {
            if (NumeriEstratti.Count > 0)
            {
                Bottoni[NumeriEstratti[NumeriEstratti.Count - 1]].Background = new SolidColorBrush(Colors.White);
                if (NumeriEstratti.Count > 1)
                    btnNumeroEstratto.Content = NumeriEstratti[NumeriEstratti.Count - 2];
                else
                    btnNumeroEstratto.Content = "X";
                if (NumeriEstratti.Count > 2)
                    btnNumeroPrecedente.Content = NumeriEstratti[NumeriEstratti.Count - 3];
                else
                    btnNumeroPrecedente.Content = "X";
                NumeriEstratti.RemoveAt(NumeriEstratti.Count - 1);
            }
        }
        private void tglsAbilitazioneEstrazioneManuale_Toggled(object sender, RoutedEventArgs e)
        {
            if (tglsAbilitazioneEstrazioneManuale.IsOn)
                GruppoEstrazioneManuale.Visibility = Visibility.Visible;
            else
                GruppoEstrazioneManuale.Visibility = Visibility.Collapsed;
        }
        private async void btnEstrai_Click(object sender, RoutedEventArgs e)
        {
            bool OK;
            int decina, unita, numeroE,ripetizione;
            Random r = new Random();
            if (estrazioneTerminata)
                if (NumeriEstratti.Count < 90)
                {
                    estrazioneTerminata = false;
                    btnNumeroPrecedente.Content = btnNumeroEstratto.Content;

                    #region Animazione
                    ripetizione = r.Next(50, 101);
                    for (int i = 0; i < ripetizione; i++)
                    {
                        await Task.Delay(10);
                        numeroE = r.Next(1, 91);
                        if (numeroE < 10)
                            btnNumeroEstratto.Content = "0" + numeroE.ToString();
                        else
                            btnNumeroEstratto.Content = numeroE.ToString();
                    }
                    #endregion
                    do
                    {
                        OK = true;
                        decina = r.Next(0, 10);
                        unita = r.Next(0, 10);
                        if (decina == 9)
                            if (unita != 0)
                                OK = false;
                        if (decina == 0 && unita == 0)
                            OK = false;
                        numeroE = (decina * 10) + unita;
                        if (OK)
                            if (Bottoni[decina.ToString() + unita.ToString()].Background == ColoreNumeriEstratti)
                                OK = false;
                    }
                    while (OK == false);
                    if (numeroE < 10)
                        btnNumeroEstratto.Content = "0" + numeroE.ToString();
                    else
                        btnNumeroEstratto.Content = numeroE.ToString();

                    Bottoni[decina.ToString() + unita.ToString()].Background = ColoreNumeriEstratti;
                    NumeriEstratti.Add(btnNumeroEstratto.Content.ToString());
                    if (NumeriEstratti.Count == 90)
                        btnEstrai.Content = "Ricomincia";
                    estrazioneTerminata = true;
                }
                else
                    btnReset_Click(null, null);   
        }
        private async void btnEstraiVincitore_Click(object sender, RoutedEventArgs e)
        {          
            for (int i = 0; i < 100; i++)
            {
                Random r = new Random();
                await Task.Delay(20);
                btnNumeroVincente.Content = r.Next(1, cmbNumeroPersone.SelectedIndex +3).ToString();
            }
        }
    }
}
