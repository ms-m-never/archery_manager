using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Archery_Manager.ViewModel
{
    public class MainViewModel : Bases.NotifyPropertyChanged
    {
        public ICommand AddCmd { get; private set; }

        private objets.Club _Club;
        public objets.Club Club {
            get { return _Club; }
            set { if (value != _Club) { _Club = value; OnPropertyChanged(); } }
        }

        public MainViewModel()
        {
            this.AddCmd = new Bases.RelayCommand((o) => { ApplicationHelper.RootFrame.Navigate(typeof(NewArcherForm)); });
            LoadData();
        }
        private void LoadData()
        {
            var Folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                ApplicationHelper.MyClub = ApplicationHelper.DeSerializeXML<objets.Club>("Data");
                List<objets.Archer> truc;
                if (ApplicationHelper.MyClub != null)
                {
                    truc = ApplicationHelper.MyClub.Archers;
                }
                else
                {
                    ApplicationHelper.MyClub = new objets.Club();
                }
            }
            catch (Exception)
            {

            }
            Club = ApplicationHelper.MyClub;
        }
        public void ShowCmd(object sender, object parameter)
        {
            var arg = parameter as Windows.UI.Xaml.Controls.ItemClickEventArgs;
            ApplicationHelper.RootFrame.Navigate(typeof(ArcherProfile), ((Windows.UI.Xaml.Controls.ItemClickEventArgs)parameter).ClickedItem);
        }
    }
}
