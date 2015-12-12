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
    public sealed partial class NewScore : Page
    {
        string ScoreType;


        static DateTime dateScore = DateTime.Today;
        string newDateScore = dateScore.ToString("D");

        public NewScore()
        {
            this.InitializeComponent();
            DateTime dateScore = DateTime.Today;
            Date.Text = dateScore.ToString("D");

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is objets.Archer)
            {
                objets.Archer Tireur = e.Parameter as objets.Archer;

                this.DataContext = Tireur;

                base.OnNavigatedTo(e);
            }
        }

        

        private void click_annule(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void train_Checked(object sender, RoutedEventArgs e)
        {
            ScoreType = "train";
        }

        private void Comp_checked(object sender, RoutedEventArgs e)
        {
            ScoreType = "comp";
        }

        private void Score_click(object sender, RoutedEventArgs e)
        {
            string DistanceScore = Distance.SelectedItem.ToString();


            if (ScoreType == string.Empty || DistanceScore == null)
            {
                // error.Text = "Remplissez tout les champs";
                ApplicationHelper.Message("Selectionez la distance et le type de Score.");
            }
            else
            {

                //ajouter aussi le commentaire si non null
                if (Commentaire.Text == "Commentaire") {
                    objets.Score NewScore = new objets.Score(dateScore, ScoreType, DistanceScore);

                    objets.Archer.Add(this.DataContext as objets.Archer, NewScore);

                    ApplicationHelper.SerializeXML("Data", ApplicationHelper.MyClub);

                    Frame.Navigate(typeof(volee_comptage), this.DataContext);

                    
                }
                else
                {
                    objets.Score NewScore = new objets.Score(dateScore, ScoreType, DistanceScore, Commentaire.Text);

                    objets.Archer.Add(this.DataContext as objets.Archer, NewScore);  

                    ApplicationHelper.SerializeXML("Data", ApplicationHelper.MyClub);
                    Frame.Navigate(typeof(volee_comptage), this.DataContext);

                }
                



            }

        }

        private void com_focus(object sender, RoutedEventArgs e)
        {
            Commentaire.Text = "";
        }

        private void com_lostFoc(object sender, RoutedEventArgs e)
        {
            if (Commentaire.Text == "" && Commentaire.Text == "Commentaire") {
                Commentaire.Text = "Commentaire";

            }
        }
    }
}
