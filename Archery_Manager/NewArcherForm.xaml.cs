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
using Windows.Devices.Enumeration;
using System.Linq;
using Windows.System.Display;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage.FileProperties;
using Windows.Media.MediaProperties;
using System.Diagnostics;
using Windows.Devices.Sensors;
using Windows.UI.Core;

//https://msdn.microsoft.com/fr-fr/library/windows/apps/mt243896.aspx
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

            if (name.Text.Trim() == string.Empty || name.Text == "Nom, Prénom, Surnom" || cat.SelectedItem == null)
            {
                // error.Text = "Remplissez tout les champs";
                ApplicationHelper.Message("Remplissez tout les champs!");
            }
            else
            {
                string ArcherName = name.Text;
                string ArcherCat = cat.SelectedItem.ToString();
                objets.Archer Newbie = new objets.Archer(ArcherName, ArcherCat, ArcherArme, photoPath);

                ApplicationHelper.MyClub.Archers.Add(Newbie);

                ApplicationHelper.SerializeXML("Data", ApplicationHelper.MyClub);

                Frame.Navigate(typeof(View.MainPage));
            }
        }

        private void NewArcherNop(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(View.MainPage));
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
        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        private MediaCapture _mediaCapture; //transformer en propriété
        private bool _isInitialized = false;
        private bool _externalCamera;
        private bool _mirroringPreview;
        public bool _isPreviewing { get; set; }
        public FlowDirection _FlowDirection { get; set; }
        private async void PhotoChose(object sender, RoutedEventArgs e)
        {
            /*
            CameraCaptureUI captureUI = new CameraCaptureUI();                                  //prise de la photo
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;                   //format jpeg
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);                   //format carré 200x200
            
            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);    //capture de la photo
           
            if (photo != null)                                                                  //si photo ok
            {
                IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);        //ouverture de la photo
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);                
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8,BitmapAlphaMode.Premultiplied);
                               
                SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(softwareBitmap);
                
                imageArcher.Source = bitmapSource;
                photoPath = photo.Path;
            }*/
        }
        private async Task InitializeCameraAsync()
        {
            _mediaCapture = new MediaCapture();
            if (_mediaCapture == null)
            {
                // Get available devices for capturing pictures
                var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                // Get the desired camera by panel
                DeviceInformation cameraDevice =
                    allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null &&
                    x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);

                // If there is no camera on the specified panel, get any camera
                cameraDevice = cameraDevice ?? allVideoDevices.FirstOrDefault();

                if (cameraDevice == null)
                {
                    ApplicationHelper.Message("No camera device found.");
                    return;
                }

                // Create MediaCapture and its settings
                _mediaCapture = new MediaCapture();

                // Register for a notification when video recording has reached the maximum time and when something goes wrong
                //_mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;

                var mediaInitSettings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
                // Initialize MediaCapture
                try
                {
                    await _mediaCapture.InitializeAsync(mediaInitSettings);
                    _isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    ApplicationHelper.Message("The app was denied access to the camera");
                }
                catch (Exception ex)
                {
                    ApplicationHelper.Message(string.Format("Exception when initializing MediaCapture with {0}: {1}", cameraDevice.Id, ex.ToString()));
                }

                // If initialization succeeded, start the preview
                if (_isInitialized)
                {
                    // Figure out where the camera is located
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                    {
                        // No information on the location of the camera, assume it's an external camera, not integrated on the device
                        _externalCamera = true;
                    }
                    else
                    {
                        // Camera is fixed on the device
                        _externalCamera = false;

                        // Only mirror the preview if the camera is on the front panel
                        _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                    }

                    await StartPreviewAsync();

                    UpdateCaptureControls();
                }
            }
        }
        private async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            _displayRequest.RequestActive();

            // Set the preview source in the UI and mirror it if necessary
            //PreviewControl.Source = _mediaCapture; remplacer par propriété
            _FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start the preview
            try
            {
                await _mediaCapture.StartPreviewAsync();
                _isPreviewing = true;
            }
            catch (Exception ex)
            {
                ApplicationHelper.Message(string.Format("Exception when starting the preview: {0}", ex.ToString()));
            }

            // Initialize the preview to the current orientation
            if (_isPreviewing)
            {
                await SetPreviewRotationAsync();
            }
        }
        private async Task SetPreviewRotationAsync()
        {
            // Only need to update the orientation if the camera is mounted on the device
            if (_externalCamera) return;

            // Populate orientation variables with the current state
            _displayOrientation = _displayInformation.CurrentOrientation;

            // Calculate which way and how far to rotate the preview
            int rotationDegrees = ConvertDisplayOrientationToDegrees(_displayOrientation);

            // The rotation direction needs to be inverted if the preview is being mirrored
            if (_mirroringPreview)
            {
                rotationDegrees = (360 - rotationDegrees) % 360;
            }

            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, rotationDegrees);
            await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);

        }
        private static int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return 90;
                case DisplayOrientations.LandscapeFlipped:
                    return 180;
                case DisplayOrientations.PortraitFlipped:
                    return 270;
                case DisplayOrientations.Landscape:
                default:
                    return 0;
            }
        }
        private async Task TakePhotoAsync()
        {
            var stream = new InMemoryRandomAccessStream();

            try
            {
                Debug.WriteLine("Taking photo...");
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                Debug.WriteLine("Photo taken!");

                var photoOrientation = ConvertOrientationToPhotoOrientation(GetCameraOrientation());
                await ReencodeAndSavePhotoAsync(stream, "photo.jpg", photoOrientation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: {0}", ex.ToString());
            }

        }
        private SimpleOrientation GetCameraOrientation()
        {
            if (_externalCamera)
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (_mirroringPreview)
            {
                // This only affects the 90 and 270 degree cases, because rotating 0 and 180 degrees is the same clockwise and counter-clockwise
                switch (_deviceOrientation)
                {
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        return SimpleOrientation.Rotated270DegreesCounterclockwise;
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        return SimpleOrientation.Rotated90DegreesCounterclockwise;
                }
            }

            return _deviceOrientation;
        }
        private static PhotoOrientation ConvertOrientationToPhotoOrientation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return PhotoOrientation.Rotate90;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return PhotoOrientation.Rotate180;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return PhotoOrientation.Rotate270;
                case SimpleOrientation.NotRotated:
                default:
                    return PhotoOrientation.Normal;
            }
        }
        private static async Task ReencodeAndSavePhotoAsync(IRandomAccessStream stream, string filename, PhotoOrientation photoOrientation)
        {
            using (var inputStream = stream)
            {
                var decoder = await BitmapDecoder.CreateAsync(inputStream);

                var file = await KnownFolders.PicturesLibrary.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                    var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                    await encoder.FlushAsync();
                }
            }
        }
        private async Task StopPreviewAsync()
        {
            // Stop the preview
            try
            {
                _isPreviewing = false;
                await _mediaCapture.StopPreviewAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when stopping the preview: {0}", ex.ToString());
            }

            // Use the dispatcher because this method is sometimes called from non-UI threads
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Cleanup the UI
                PreviewControl.Source = null;

                // Allow the device screen to sleep now that the preview is stopped
                _displayRequest.RequestRelease();
            });
        }
        private async Task CleanupCameraAsync()
        {
            Debug.WriteLine("CleanupCameraAsync");

            if (_isInitialized)
            {
                // If a recording is in progress during cleanup, stop it to save the recording
                //if (_isRecording)
                //{
                //    await StopRecordingAsync();
                //}

                if (_isPreviewing)
                {
                    // The call to MediaCapture.Dispose() will automatically stop the preview
                    // but manually stopping the preview is good practice
                    await StopPreviewAsync();
                }

                _isInitialized = false;
            }

            if (_mediaCapture != null)
            {
                _mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
                _mediaCapture.Failed -= MediaCapture_Failed;
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }
        private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            await CleanupCameraAsync();
        }
    }
}

