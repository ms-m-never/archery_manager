using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Archery_Manager
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ArcherProfile : Page
    {
        public ArcherProfile()
        {
            this.InitializeComponent();
            // Chargé les données envoyées? 
            
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is objets.Archer)
            {
                objets.Archer Tireur = e.Parameter as objets.Archer;
                ArcherInfo.Text = Tireur.name+"\n"+Tireur.cat+"\n"+Tireur.arme + "\n";

                this.DataContext = Tireur;

                if (Tireur.perf == null){
                    
                }else{


                }
            }
            else
            {
                ArcherInfo.Text = "Probleme";
            }
            base.OnNavigatedTo(e);
        }

        private void NewScoreClick(object sender, RoutedEventArgs e )
        {
            Frame.Navigate(typeof(NewScore), this.DataContext);
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void ModifierClick(object sender, RoutedEventArgs e)
        {

        }

        private void SuprimeClick(object sender, RoutedEventArgs e)
        {
            ApplicationHelper.MyClub.Donnees.Remove((objets.Archer)this.DataContext);
            ApplicationHelper.SerializeXML("Data", ApplicationHelper.MyClub);
            Frame.Navigate(typeof(MainPage));
        }

        private void ScoreClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to la page des scores
            
        }

        private void SupScore(object sender, RoutedEventArgs e)
        {
            objets.Archer tireur = this.DataContext as objets.Archer;
            if (tireur != null)
            {
                objets.Score score = ((Button)sender).DataContext as objets.Score;
                if (score != null)
                    tireur.perf.Remove(score);

                ApplicationHelper.SerializeXML("Data", ApplicationHelper.MyClub);
                Frame.Navigate(typeof(ArcherProfile),tireur);
            }
            // objets.Archer Tireur = this.DataContext as objets.Archer;
            // objets.Archer.Remove(Tireur,  as objets.Score);
            // ApplicationHelper.SerializeXML("Data", ApplicationHelper.MyClub);
        }
    }
}
