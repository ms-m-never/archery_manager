using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Archery_Manager
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class NewArcherForm : Page
    {
        string ArcherArme;
        public string photoPath = "Assets/cible.png";

        public NewArcherForm()
        {
            this.InitializeComponent();
        }

        private void NewArcherOk(object sender, RoutedEventArgs e)
        {

            if (name.Text.Trim() == string.Empty || name.Text == "Nom, Prénom, Surnom" || cat.SelectedItem == null )
            {
               // error.Text = "Remplissez tout les champs";
                ApplicationHelper.Message("Remplissez tout les champs!");
            }
            else
            {
                string ArcherName = name.Text;
                string ArcherCat = cat.SelectedItem.ToString();
                objets.Archer Newbie = new objets.Archer(ArcherName, ArcherCat, ArcherArme, photoPath );

                ApplicationHelper.MyClub.Donnees.Add(Newbie);
                
                ApplicationHelper.SerializeXML("Data", ApplicationHelper.MyClub);
                
                Frame.Navigate(typeof(MainPage));                
            }
        }

        private void NewArcherNop(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void ClassiqueChecked(object sender, RoutedEventArgs e)
        {
            ArcherArme = "Classique";
        }

        private void PoulieChecked(object sender, RoutedEventArgs e)
        {
            ArcherArme = "Poulie";
        }


        private void name_GotFocus(object sender, RoutedEventArgs e)
        {
            if (name.Text == "Nom, Prénom, Surnom" && name.Focus(FocusState.Programmatic) == true)
            {
                name.Text = "";
                
            }
        }

        private void name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (name.Text == "") { name.Text = "Nom, Prénom, Surnom"; }
        }

        private async void PhotoChose(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
          //  string test =  photo.Path;
            

           
            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }
            else
            {
                IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied);

                SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

                imageArcher.Source = bitmapSource;
                photoPath = photo.Path;
            }



             //  name.Text = stream.;

            // test de sauvegarde // test de sauvegarde // test de sauvegarde 


           /* StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile photoFile = await folder.CreateFileAsync("test.jpg", CreationCollisionOption.ReplaceExisting);

            IRandomAccessStream streamPhoto = await photoFile.OpenAsync(FileAccessMode.ReadWrite);
            
            // Create an encoder with the desired format
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, streamPhoto);

            // Set the software bitmap
            encoder.SetSoftwareBitmap(softwareBitmap);

            // Set additional encoding parameters, if needed
            //encoder.BitmapTransform.ScaledWidth = 320;
            encoder.BitmapTransform.ScaledHeight = 200;
            encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.Clockwise90Degrees;
            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
            encoder.IsThumbnailGenerated = true;

            try
            {
                await encoder.FlushAsync();

            }
            catch (Exception err)
            {
                switch (err.HResult)
                {
                    case unchecked((int)0x88982F81): //WINCODEC_ERR_UNSUPPORTEDOPERATION
                                                     // If the encoder does not support writing a thumbnail, then try again
                                                     // but disable thumbnail generation.
                        encoder.IsThumbnailGenerated = false;
                        break;
                    default:
                        throw err;
                }
            }

            if (encoder.IsThumbnailGenerated == false)
            {
                await encoder.FlushAsync();
            }*/


        }
    
    }
 }

