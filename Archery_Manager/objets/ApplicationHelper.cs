using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Archery_Manager
{
    public static class ApplicationHelper
    {
       public static objets.Club MyClub = new objets.Club();
        
        public static Windows.Storage.ApplicationDataContainer LocalSettings { get { return Windows.Storage.ApplicationData.Current.LocalSettings; } }
        public static Windows.Storage.StorageFolder LocalFolder { get { return Windows.Storage.ApplicationData.Current.LocalFolder; } }
        public static Frame RootFrame { get {  return Window.Current.Content as Frame; } }
        public static T DeSerializeXML<T>(string FileName) where T : class
        {
            T temp = null;
            try {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CloseInput = true;
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream fs = new FileStream(LocalFolder.Path + @"\" + FileName + ".xml", FileMode.Open);
                using (XmlReader reader = XmlReader.Create(fs,settings))
                {
                    if(serializer.CanDeserialize(reader))
                        temp = (T)serializer.Deserialize(reader);
                }
            }catch(Exception ex)
            {
                Message("Fichier de données introuvable.");
            }
            return temp;
        }


        public static async void Message(string message)
        {
            var m = new MessageDialog(message);
            await m.ShowAsync();
        }

    
        public static async System.Threading.Tasks.Task<bool> MessageValid(string message)
        {
            var m = new MessageDialog(message);
            m.Commands.Add(new UICommand("Valider") { Id = 1 });
            m.Commands.Add(new UICommand("Annuler") { Id = 0 });
            m.CancelCommandIndex = 0;
            IUICommand result = await m.ShowAsync();
            return (int)result.Id==1;
        }

       /* public async void MessageValid(string message)
         {
            var m = new MessageDialog(message);

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            m.Commands.Add(new UICommand("Valider", new UICommandInvokedHandler()));
            m.Commands.Add(new UICommand("Annuler", new UICommandInvokedHandler()));

            m.DefaultCommandIndex = 0;

            m.CancelCommandIndex = 1;

            await m.ShowAsync();
        }*/



        public static void SerializeXML<T>(string FileName, T objet) where T : class
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CloseOutput = true;
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineOnAttributes = true;
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream fs = new FileStream(LocalFolder.Path + @"\" + FileName + ".xml", FileMode.Create);
                using (XmlWriter writer = XmlWriter.Create(fs,settings))
                {
                    serializer.Serialize(writer, objet);
                }
            }
            catch (Exception ex)
            {
                Message(ex.Message);
            }
        }
    }
}
