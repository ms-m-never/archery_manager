//https://msdn.microsoft.com/fr-fr/library/windows/apps/mt243896.aspx
// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Archery_Manager.View
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class NewArcherForm : Bases.MvvmPage
    {        
        public NewArcherForm()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel.NewArcherViewModel(this.PreviewControl);
        }
    }
}

