using System;
using System.Collections.Generic;
using System.Windows.Input;
using Archery_Manager.objets;

namespace Archery_Manager.ViewModel
{
    public class MainViewModel : Bases.NotifyPropertyChanged
    {
        public ICommand AddCmd { get; private set; }
        
        public Club Club {
            get { return RessourceManager.Instance.Club; }
        }

        public MainViewModel()
        {
            this.AddCmd = new Bases.RelayCommand((o) => { ApplicationHelper.RootFrame.Navigate(typeof(View.NewArcherForm)); });
            LoadData();
        }
        private void LoadData()
        {
            var Folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                RessourceManager.Instance.Club = ApplicationHelper.DeSerializeXML<Club>("Data");

            }
            catch (Exception)
            {

            }
        }
        public void ShowCmd(object sender, object parameter)
        {
            var arg = parameter as Windows.UI.Xaml.Controls.ItemClickEventArgs;
            ApplicationHelper.RootFrame.Navigate(typeof(ArcherProfile), ((Windows.UI.Xaml.Controls.ItemClickEventArgs)parameter).ClickedItem);
        }
    }
}
