using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Archery_Manager
{

    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        public List<objets.Archer> Club;
        public List<string> test;
        public MainPage()

        {
            this.InitializeComponent();
            LoadData();
        }


        private void ClickNewArcher(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewArcherForm)); 
        }

        private void LoadData() {

            var Folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            
            try
            {
               
                ApplicationHelper.MyClub = ApplicationHelper.DeSerializeXML<objets.Club>("Data");
                List<objets.Archer> truc;
                if (ApplicationHelper.MyClub != null)
                {
                    truc = ApplicationHelper.MyClub.Donnees;
                  /*  foreach (objets.Archer tireur in truc)
                    {
                      ListArcher.Items.Add(truc);
                    }*/
                }
                else
                {
                    ApplicationHelper.MyClub = new objets.Club();
                }
            }
            catch(Exception)
            {

            }
            //ListArcher.DataContext = this;
            this.DataContext = ApplicationHelper.MyClub;
        }

        private void ListArcher_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            Frame.Navigate(typeof(ArcherProfile),e.ClickedItem); // Envoyez l'objet cliquer à la nouvelle fenêtre ?? 
           
        }
    }
}
