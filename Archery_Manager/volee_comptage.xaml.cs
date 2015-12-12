using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Archery_Manager
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class volee_comptage : Page
    {

        public bool flecheUN = false;
        public bool flecheDEUX = false;
        public bool flecheTROIS = false;
        public int total = 0;
        public int totvolee = 0;

        public volee_comptage()
        {
            this.InitializeComponent();
        }

        private void trois_tapped(object sender, TappedRoutedEventArgs e)
        {
            ApplicationHelper.MessageValid(" 3 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(3, abs, ord);


        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {

        }

        private void back_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void calculeTotal(int N, double abs, double ord)
        {
            bool result = await ApplicationHelper.MessageValid(" " + N + " ?");
            if (result)
            {
                if (flecheUN == false)
                {
                    // Creer les objet fleche
                    flecheUN = true;
                    fleche1.Text = N.ToString();
                    total += N;
                    totvolee = N;
                    scoreTotal.Text = total.ToString();
                    volee.Text = totvolee.ToString();
                    Fleche1Point.Margin = new Thickness(abs - 5, ord - 5, 0, 0);
                    Fleche1Point.HorizontalAlignment = HorizontalAlignment.Left;
                    Fleche1Point.VerticalAlignment = VerticalAlignment.Top;
                    Fleche1Point.Height = 10;
                    Fleche1Point.Width = 10;
                }else if (flecheUN == true && flecheDEUX == false)
                {
                    flecheDEUX = true;
                    fleche2.Text = N.ToString();
                    total += N;
                    totvolee += N;
                    scoreTotal.Text = total.ToString();
                    volee.Text = totvolee.ToString();
                    Fleche2Point.Margin = new Thickness(abs - 5, ord - 5, 0, 0);
                    Fleche2Point.HorizontalAlignment = HorizontalAlignment.Left;
                    Fleche2Point.VerticalAlignment = VerticalAlignment.Top;
                    Fleche2Point.Height = 10;
                    Fleche2Point.Width = 10;

                }
                else if (flecheUN == true && flecheDEUX == true && flecheTROIS == false)
                {
                    flecheTROIS = true;
                    fleche3.Text = N.ToString();
                    total += N;
                    totvolee += N;
                    scoreTotal.Text = total.ToString();
                    volee.Text = totvolee.ToString();
                    Fleche3Point.Margin = new Thickness(abs - 5, ord - 5, 0, 0);
                    Fleche3Point.HorizontalAlignment = HorizontalAlignment.Left;
                    Fleche3Point.VerticalAlignment = VerticalAlignment.Top;
                    Fleche3Point.Height = 10;
                    Fleche3Point.Width = 10;

                }
                else
                {



                }
            }
            
        }

        private void Quatre_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //ApplicationHelper.MessageValid(" 4 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(4, abs, ord);
        }

        private void Cinq_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //ApplicationHelper.MessageValid(" 5 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(5, abs, ord);
        }

        private void six_tapped(object sender, TappedRoutedEventArgs e)
        {
            //ApplicationHelper.MessageValid(" 6 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(6, abs, ord);
        }

        private void sept_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //ApplicationHelper.MessageValid(" 7 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(7, abs, ord);
        }

        private void huit_tapped(object sender, TappedRoutedEventArgs e)
        {
            //ApplicationHelper.MessageValid(" 8 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(8, abs, ord);
        }

        private void neuf_tapped(object sender, TappedRoutedEventArgs e)
        {
            //ApplicationHelper.MessageValid(" 9 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(9, abs, ord);
        }

        private void dix_tapped(object sender, TappedRoutedEventArgs e)
        {
            // SI POULIE c'est NEUF
            //ApplicationHelper.MessageValid(" 10 ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(10, abs, ord);
        }

        private void plus_tapped(object sender, TappedRoutedEventArgs e)
        {

            // SI POULIE c'est dix
            //ApplicationHelper.MessageValid(" 10 plus ?");
            Point fleche = e.GetPosition(Cible);
            double abs = fleche.X;
            double ord = fleche.Y;
            //ApplicationHelper.Message("3 "); // Definir message de confirmation if ok // sinon rien ne se passe
            calculeTotal(10, abs, ord);
        }
    }
}
