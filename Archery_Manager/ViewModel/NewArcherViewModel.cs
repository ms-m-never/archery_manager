using Archery_Manager.Bases;
using System.Windows.Input;
using Archery_Manager.objets;
using Windows.UI.Xaml;
using Windows.Media;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.Graphics.Display;
using Windows.Devices.Sensors;
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using System.Linq;
using Windows.Storage.Streams;
using Windows.Media.MediaProperties;
using System.Diagnostics;
using Windows.Storage.FileProperties;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace Archery_Manager.ViewModel
{
    public class NewArcherViewModel : NotifyPropertyChanged, Bases.Interfaces.INavigable
    {
        public ICommand ValidateCmd { get; set; }
        public ICommand CancelCmd { get; set; }
        public ICommand StartMediaCmd { get; set; }
        public ICommand TakePictureCmd { get; set; }

        private Archer archer;
        public Archer Archer
        {
            get { return archer; }
            set
            {
                if (archer != value)
                {
                    archer = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public NewArcherViewModel(CaptureElement c)
        {
            ce = c;
            Archer = new Archer();
            StartMediaCmd = new RelayCommand(async (o) => { await InitializeCameraAsync(); });
            CancelCmd = new RelayCommand((o) => { ApplicationHelper.RootFrame.Navigate(typeof(View.MainPage)); });
            ValidateCmd = new RelayCommand(
                (o) => {
                    if (!string.IsNullOrEmpty(archer.Nom) && !string.IsNullOrEmpty(archer.Arme) && !string.IsNullOrEmpty(archer.Categorie))
                    {
                        RessourceManager.Instance.Club.Archers.Add(archer);
                        ApplicationHelper.SerializeXML<Club>("Data", RessourceManager.Instance.Club);
                    }
                    else
                    {

                    }
                });
            TakePictureCmd = new RelayCommand(
                async (o) =>
                {
                    try {
                        string temp = await TakePhotoAsync();
                        Archer.Photo = temp;
                        await StopPreviewAsync();
                    }catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
        }

        public void Activate(object parameter)
        {
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
        }

        public void Deactivate(object parameter)
        {

        }

        #region Camera
        // Resources/Drawable/cible.png
        public string photoPath = "";
        public CaptureElement ce;
        private MediaCapture mediaCapture;
        public MediaCapture MediaCapture
        {
            get { return mediaCapture; }
            set {
                if (mediaCapture != value)
                {
                    mediaCapture = value;
                    OnPropertyChanged();
                }
            }
        }
        public FlowDirection PreviewControlFlowDirection;
        private bool IsInitialized { get; set; }
        private bool isPreviewing;
        public bool IsPreviewing {
            get { return isPreviewing; }
            set
            {
                if (isPreviewing != value)
                {
                    isPreviewing = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _externalCamera;
        private bool _mirroringPreview;
        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        private readonly DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();
        private DisplayOrientations _displayOrientation = DisplayOrientations.Portrait;
        private readonly SimpleOrientationSensor _orientationSensor = SimpleOrientationSensor.GetDefault();
        private SimpleOrientation _deviceOrientation = SimpleOrientation.NotRotated;
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

         
        private readonly SystemMediaTransportControls _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
        private async void SystemMediaControls_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Only handle this event if this page is currently being displayed
                if (args.Property == SystemMediaTransportControlsProperty.SoundLevel && ((Frame)Window.Current.Content).CurrentSourcePageType == typeof(View.NewArcherForm))
                {
                    // Check to see if the app is being muted. If so, it is being minimized.
                    // Otherwise if it is not initialized, it is being brought into focus.
                    if (sender.SoundLevel == SoundLevel.Muted)
                    {
                        await CleanupCameraAsync();
                    }
                    else if (!IsInitialized)
                    {
                        await InitializeCameraAsync();
                    }
                }
            });
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (((Frame)Window.Current.Content).CurrentSourcePageType == typeof(View.NewArcherForm))
            {
                var deferral = e.SuspendingOperation.GetDeferral();

                UnregisterOrientationEventHandlers();

                _systemMediaControls.PropertyChanged -= SystemMediaControls_PropertyChanged;

                await CleanupCameraAsync();

                deferral.Complete();
            }
        }
        private async void Application_Resuming(object sender, object o)
        {
            // Handle global application events only if this page is active
            if (((Frame)Window.Current.Content).CurrentSourcePageType == typeof(View.NewArcherForm))
            {
                RegisterOrientationEventHandlers();

                _systemMediaControls.PropertyChanged += SystemMediaControls_PropertyChanged;

                await InitializeCameraAsync();
            }
        }
        private void RegisterOrientationEventHandlers()
        {
            // If there is an orientation sensor present on the device, register for notifications
            if (_orientationSensor != null)
            {
                _orientationSensor.OrientationChanged += OrientationSensor_OrientationChanged;
                _deviceOrientation = _orientationSensor.GetCurrentOrientation();
            }

            _displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
            _displayOrientation = _displayInformation.CurrentOrientation;


        }
        private void UnregisterOrientationEventHandlers()
        {
            if (_orientationSensor != null)
            {
                _orientationSensor.OrientationChanged -= OrientationSensor_OrientationChanged;
            }

            _displayInformation.OrientationChanged -= DisplayInformation_OrientationChanged;
        }
        private void OrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
            {
                _deviceOrientation = args.Orientation;
            }
        }
        private async void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            _displayOrientation = sender.CurrentOrientation;

            if (IsPreviewing)
            {
                await SetPreviewRotationAsync();
            }

        }

        private async Task InitializeCameraAsync()
        {
            if (MediaCapture == null) // ???
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
                MediaCapture = new MediaCapture();

                // Register for a notification when video recording has reached the maximum time and when something goes wrong
                //_mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;

                var mediaInitSettings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
                // Initialize MediaCapture
                try
                {
                    await MediaCapture.InitializeAsync(mediaInitSettings);
                    IsInitialized = true;
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
                if (IsInitialized)
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
            //PreviewControl.Source = MediaCapture; //remplacer par propriété
            ce.Source = MediaCapture;
            //PreviewControl.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            PreviewControlFlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            // Start the preview
            try
            {
                await MediaCapture.StartPreviewAsync();
                IsPreviewing = true;
            }
            catch (Exception ex)
            {
                ApplicationHelper.Message(string.Format("Exception when starting the preview: {0}", ex.ToString()));
            }

            // Initialize the preview to the current orientation
            if (IsPreviewing)
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
            var props = MediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, rotationDegrees);
            await MediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);

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
        private static int ConvertDeviceOrientationToDegrees(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return 180;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return 270;
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return 90;
                default:
                    return 0;
            }
        }
        private async Task<string> TakePhotoAsync()
        {
            var stream = new InMemoryRandomAccessStream();

            try
            {
                Debug.WriteLine("Taking photo...");
                await MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                Debug.WriteLine("Photo taken!");

                var photoOrientation = ConvertOrientationToPhotoOrientation(GetCameraOrientation());
                string fileName = await ReencodeAndSavePhotoAsync(stream, @"Pictures\" + Guid.NewGuid().ToString() + ".jpg", photoOrientation);
                return fileName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: {0}", ex.ToString());
                return "";
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
        private static async Task<string> ReencodeAndSavePhotoAsync(IRandomAccessStream stream, string filename, PhotoOrientation photoOrientation)
        {
            using (var inputStream = stream)
            {
                var decoder = await BitmapDecoder.CreateAsync(inputStream);

                //var file = await KnownFolders.PicturesLibrary.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
                var file = await ApplicationHelper.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
                using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                    var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                    await encoder.FlushAsync();
                    return file.Path;
                }
            }
        }
        private async Task StopPreviewAsync()
        {
            // Stop the preview
            try
            {
                IsPreviewing = false;
                await MediaCapture.StopPreviewAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when stopping the preview: {0}", ex.ToString());
            }

            // Use the dispatcher because this method is sometimes called from non-UI threads
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Cleanup the UI
                // PreviewControl.Source = null;
                MediaCapture = null;

                // Allow the device screen to sleep now that the preview is stopped
                _displayRequest.RequestRelease();
            });
        }
        private async Task CleanupCameraAsync()
        {
            Debug.WriteLine("CleanupCameraAsync");

            if (IsInitialized)
            {
                // If a recording is in progress during cleanup, stop it to save the recording
                //if (_isRecording)
                //{
                //    await StopRecordingAsync();
                //}

                if (IsPreviewing)
                {
                    // The call to MediaCapture.Dispose() will automatically stop the preview
                    // but manually stopping the preview is good practice
                    await StopPreviewAsync();
                }

                IsInitialized = false;
            }

            if (MediaCapture != null)
            {
                //_mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
                MediaCapture.Failed -= MediaCapture_Failed;
                MediaCapture.Dispose();
                MediaCapture = null;
            }
        }
        private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            await CleanupCameraAsync();
        }
        private void UpdateButtonOrientation()
        {
            int device = ConvertDeviceOrientationToDegrees(_deviceOrientation);
            int display = ConvertDisplayOrientationToDegrees(_displayOrientation);

            if (_displayInformation.NativeOrientation == DisplayOrientations.Portrait)
            {
                device -= 90;
            }

            // Combine both rotations and make sure that 0 <= result < 360
            var angle = (360 + display + device) % 360;

            // Rotate the buttons in the UI to match the rotation of the device
            var transform = new RotateTransform { Angle = angle };

            //PhotoButton.RenderTransform = transform;
        }
        private void UpdateCaptureControls()
        {
            // The buttons should only be enabled if the preview started sucessfully
            //PhotoButton.IsEnabled = _isPreviewing;

        }
        #endregion Camera
    }
}
