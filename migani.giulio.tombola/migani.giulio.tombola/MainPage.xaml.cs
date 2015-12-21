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
        public bool WhiteFont { get; set; }
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
            foreach (Colori c in Enum.GetValues(typeof(Colori)))
                cmbColore.Items.Add(c);
            string a = Colors.Aqua.ToString();
            cmbColore.SelectedIndex = 3;
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
                    btnNumeroEstratto.Background = ColoreNumeriEstratti;
                    btnNumeroPrecedente.Content = btnNumeroEstratto.Content;
                    btnNumeroEstratto.Content = number;
                    ColoraTabella(number, true);
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
                c.Foreground = new SolidColorBrush(Colors.Black);
            }
            btnNumeroEstratto.Background = new SolidColorBrush(Colors.White);
            btnNumeroEstratto.Foreground = new SolidColorBrush(Colors.Black);
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
                ColoraTabella(NumeriEstratti[NumeriEstratti.Count-1], false);
                if (NumeriEstratti.Count > 1)
                    btnNumeroEstratto.Content = NumeriEstratti[NumeriEstratti.Count - 2];
                else
                {
                    btnNumeroEstratto.Content = "X";
                    btnNumeroEstratto.Background = new SolidColorBrush(Colors.White);
                    btnNumeroEstratto.Foreground = new SolidColorBrush(Colors.Black);
                }
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
                    btnNumeroEstratto.Background = new SolidColorBrush(Colors.White);                    
                    btnNumeroEstratto.Foreground = new SolidColorBrush(Colors.Black);
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
                    if (WhiteFont)
                        btnNumeroEstratto.Foreground = new SolidColorBrush(Colors.White);
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
                    ColoraTabella(decina, unita, true);
                    NumeriEstratti.Add(btnNumeroEstratto.Content.ToString());
                    if (NumeriEstratti.Count == 90)
                        btnEstrai.Content = "Ricomincia";
                    btnNumeroEstratto.Background = ColoreNumeriEstratti;
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
        private void cmbColore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SolidColorBrush nuovoColore = new SolidColorBrush(Colors.Yellow);
            WhiteFont = false;
            switch (cmbColore.SelectedIndex)
            {
                case 0: nuovoColore = new SolidColorBrush(Colors.Orange);
                    break;
                case 1: nuovoColore = new SolidColorBrush(Colors.Cyan);
                    break;
                case 2: nuovoColore = new SolidColorBrush(Colors.Blue);
                    break;
                case 3: nuovoColore = new SolidColorBrush(Colors.Yellow);
                    break;
                case 4: nuovoColore = new SolidColorBrush(Colors.Brown);
                    break;
                case 5: nuovoColore = new SolidColorBrush(Colors.Black);
                    WhiteFont = true;
                    break;
                case 6: nuovoColore = new SolidColorBrush(Colors.Red);
                    break;
                case 7: nuovoColore = new SolidColorBrush(Colors.Green);
                    break;
                case 8: nuovoColore = new SolidColorBrush(Colors.Purple);
                    break;
            }

            if (btnNumeroEstratto.Background == ColoreNumeriEstratti)
                btnNumeroEstratto.Background = nuovoColore;
            if (btnNumeroEstratto.Content.ToString() != "X")
            {
                if (WhiteFont)
                    btnNumeroEstratto.Foreground = new SolidColorBrush(Colors.White);
                else
                    btnNumeroEstratto.Foreground = new SolidColorBrush(Colors.Black);
            }
            foreach (Control c in Bottoni.Values)
            {
                if (c.Background == ColoreNumeriEstratti)
                {
                    c.Background = nuovoColore;
                    if (WhiteFont)
                        c.Foreground = new SolidColorBrush(Colors.White);
                    else
                        c.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            ColoreNumeriEstratti = nuovoColore;
        }

        private void ColoraTabella(int decina, int unita, bool estratto)
        {
            if (estratto)
                Bottoni[decina.ToString() + unita.ToString()].Background = ColoreNumeriEstratti;
            else
                Bottoni[decina.ToString() + unita.ToString()].Background = new SolidColorBrush(Colors.White);
            if (WhiteFont && estratto)
                Bottoni[decina.ToString() + unita.ToString()].Foreground = new SolidColorBrush(Colors.White);
            else
                Bottoni[decina.ToString() + unita.ToString()].Foreground = new SolidColorBrush(Colors.Black);
        }
        private void ColoraTabella(string numero, bool estratto)
        {
            ColoraTabella(int.Parse(numero.Substring(0, 1)), int.Parse(numero.Substring(1, 1)), estratto);
        }
    }
}
