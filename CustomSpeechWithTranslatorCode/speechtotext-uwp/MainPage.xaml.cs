//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage;

namespace MicrosoftSpeechSDKSamples.UwpSpeechRecognitionSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Setting localSetting = new Setting();
        private bool updated = false;

        public MainPage()
        {
            this.InitializeComponent();
            UpdateSettingUI();
            EnableMicrophone();
        }

        private void UpdateSettingUI()
        {
            try
            {
                StandardRegionComboBox.SelectedValue = localSetting.StandardRegion;
                CustomRegionComboBox.SelectedValue = localSetting.CustomRegion;
                TranslatorRegionComboBox.SelectedValue = localSetting.TranslateRegion;
                TranslatorFromLanguageComboBox.SelectedValue = localSetting.TranslateFromLanguage;
                TranslatorToLanguageComboBox.SelectedValue = localSetting.TranslateToLanguage;
                SpeechServerTypeComboBox.SelectedValue = localSetting.SpeechServerType;

                this.StandardSubscriptionKeyTextBox.Text = localSetting.StandardSubscriptionKey;
                this.CustomSubscriptionKeyTextBox.Text = localSetting.CustomSubscriptionKey;
                this.TranslatorSubscriptionKeyTextBox.Text = localSetting.TranslateSubscriptionKey;
                this.CustomDeploymentIDTextBox.Text = localSetting.CustomDeploymentID;

                updated = true;
                if (string.IsNullOrEmpty(localSetting.StandardRegion))
                    StandardRegionComboBox.SelectedIndex = 0;
                if (string.IsNullOrEmpty(localSetting.CustomRegion))
                    CustomRegionComboBox.SelectedIndex = 0;
                if (string.IsNullOrEmpty(localSetting.TranslateRegion))
                    TranslatorRegionComboBox.SelectedIndex = 3;
                if (string.IsNullOrEmpty(localSetting.TranslateFromLanguage))
                    TranslatorFromLanguageComboBox.SelectedIndex = 0;
                if (string.IsNullOrEmpty(localSetting.TranslateToLanguage))
                    TranslatorToLanguageComboBox.SelectedIndex = 2;
                if (string.IsNullOrEmpty(localSetting.SpeechServerType))
                    SpeechServerTypeComboBox.SelectedIndex = 0;

            }
            catch (System.FormatException ex)
            {
                NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        private async void SpeechRecognitionFromMicrophone()
        {
            string key = string.Empty;
            string region = string.Empty;
            string deployID = string.Empty;
            string language = string.Empty;
            bool isStandard = true;
            if (this.localSetting.SpeechServerType.Equals("Standard Speech Server"))
            {
                key = this.localSetting.StandardSubscriptionKey;
                region = this.localSetting.StandardRegion;
                language = this.localSetting.StandardRecognitionLanguage;
            }
            else if (this.localSetting.SpeechServerType.Equals("Custom Speech Server"))
            {
                isStandard = false;
                key = this.localSetting.CustomSubscriptionKey;
                region = this.localSetting.CustomRegion;
                language = this.localSetting.CustomRecognitionLanguage;
                deployID = this.localSetting.CustomDeploymentID;
            }

            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(region))
            {
                NotifyUser("Subscription key or region is missing!", NotifyType.ErrorMessage);
                return;
            }
            else if (string.IsNullOrEmpty(deployID) && isStandard == false)
            {
                NotifyUser("Deploym ID is missing!", NotifyType.ErrorMessage);
                return;
            }
            else if (string.IsNullOrEmpty(this.localSetting.TranslateSubscriptionKey))
            {
                NotifyUser("Translator key is missing!", NotifyType.ErrorMessage);
                return;
            }
            else
            {
                NotifyUser(" ", NotifyType.StatusMessage);
                ShowResult(TBSRResult, string.Empty);
                ShowResult(TBTranslation, string.Empty);
            }

            try
            {
                UpdateGridBackground(MicInputGrid, Windows.UI.Colors.Green);
                // Creates an instance of a speech factory with specified
                var factory = SpeechFactory.FromSubscription(key, region);
                // Creates a speech recognizer using microphone as audio input. The default language is "en-us".
                using (var recognizer = factory.CreateSpeechRecognizer(language))
                {
                    // Replace with the CRIS deployment id of your customized model.
                    recognizer.DeploymentId = deployID;

                    // Starts recognition. It returns when the first utterance has been recognized.
                    NotifyUser("Starts recognition.", NotifyType.StatusMessage);
                    var result = await recognizer.RecognizeAsync().ConfigureAwait(false);
                    // Checks result.

                    NotifyUser($"Stop recognition.", NotifyType.StatusMessage);
                    if (result.RecognitionStatus != RecognitionStatus.Recognized)
                    {
                        string str = $"Speech Recognition Failed. '{result.RecognitionStatus.ToString()}'";
                        NotifyUser(str, NotifyType.StatusMessage);
                    }
                    else
                    {
                        string str = result.Text;
                        ShowResult(TBSRResult, str);
                        TranslateAsync(str);
                    }
                }
                UpdateGridBackground(MicInputGrid, Windows.UI.Colors.LightGray);
            }
            catch (System.Exception ex)
            {
                NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        // The TaskCompletionSource to stop recognition.
        private TaskCompletionSource<int> stopRecognitionTaskCompletionSource;

        private async void SpeechRecogntionFromStream()
        {
            string key = string.Empty;
            string region = string.Empty;
            string deployID = string.Empty;
            string language = string.Empty;
            bool isStandard = true;
            if (this.localSetting.SpeechServerType.Equals("Standard Speech Server"))
            {
                key = this.localSetting.StandardSubscriptionKey;
                region = this.localSetting.StandardRegion;
                language = this.localSetting.StandardRecognitionLanguage;
            }
            else if (this.localSetting.SpeechServerType.Equals("Custom Speech Server"))
            {
                isStandard = false;
                key = this.localSetting.CustomSubscriptionKey;
                region = this.localSetting.CustomRegion;
                language = this.localSetting.CustomRecognitionLanguage;
                deployID = this.localSetting.CustomDeploymentID;
            }

            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(region))
            {
                NotifyUser("Subscription key or region is missing!", NotifyType.ErrorMessage);
                return;
            }
            else if (string.IsNullOrEmpty(deployID) && isStandard == false)
            {
                NotifyUser("Deploym ID is missing!", NotifyType.ErrorMessage);
                return;
            }
            else if (string.IsNullOrEmpty(this.localSetting.TranslateSubscriptionKey))
            {
                NotifyUser("Translator key is missing!", NotifyType.ErrorMessage);
                return;
            }
            else
            {
                NotifyUser(string.Empty, NotifyType.StatusMessage);
                ShowResult(TBSRResult, string.Empty);
                ShowResult(TBTranslation, string.Empty);
            }

            stopRecognitionTaskCompletionSource = new TaskCompletionSource<int>();
            AudioInputStream audioStream = null;
            BinaryReader reader = null;
            Stream stream = null;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".wav");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                string s = string.Format("Can't open it!");
                NotifyUser(s, NotifyType.ErrorMessage);
                return;
            }
            try
            {
                stream = (await file.OpenReadAsync()).AsStreamForRead();
                reader = new BinaryReader(stream);

                // Create an audio stream from a wav file.
                audioStream = Helper.OpenWaveFile(reader);

                // Creates an instance of a speech factory with specified and service region (e.g., "westus").
                var factory = SpeechFactory.FromSubscription(key, region);
                // Creates a speech recognizer using file as audio input. The default language is "en-us".
                using (var recognizer = factory.CreateSpeechRecognizerWithStream(audioStream, language))
                {
                    // Replace with the CRIS deployment id of your customized model.
                    recognizer.DeploymentId = deployID;
                    // Subscribes to events.
                    recognizer.IntermediateResultReceived += (s, ee) =>
                    {
                        NotifyUser(ee.Result.Text, NotifyType.StatusMessage);
                    };
                    recognizer.FinalResultReceived += (s, ee) =>
                    {
                        string str;
                        if (ee.Result.RecognitionStatus == RecognitionStatus.Recognized)
                        {
                            str = ee.Result.Text;
                            ShowResult(TBSRResult, str);
                            TranslateAsync(str);
                        }
                        else
                        {
                            str = $"Final result: Status: {ee.Result.RecognitionStatus.ToString()}, FailureReason: {ee.Result.RecognitionFailureReason}.";
                            NotifyUser(str, NotifyType.StatusMessage);
                        }
                    };
                    recognizer.RecognitionErrorRaised += (s, ee) =>
                    {
                        NotifyUser($"An error occurred. Status: {ee.Status.ToString()}, FailureReason: {ee.FailureReason}", NotifyType.StatusMessage);
                    };
                    recognizer.OnSessionEvent += (s, ee) =>
                    {
                        NotifyUser($"Session event. Event: {ee.EventType.ToString()}.", NotifyType.StatusMessage);
                        // Stops translation when session stop is detected.
                        if (ee.EventType == SessionEventType.SessionStoppedEvent)
                        {
                            NotifyUser($"Stop recognition.", NotifyType.StatusMessage);
                            stopRecognitionTaskCompletionSource.TrySetResult(0);
                        }
                    };
                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                    // Waits for completion.
                    await stopRecognitionTaskCompletionSource.Task.ConfigureAwait(false);
                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
            catch (System.FormatException ex)
            {
                NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
                if (audioStream != null)
                {
                    audioStream.Dispose();
                }
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Open Microphone
        /// </summary>
        private async void EnableMicrophone()
        {
            bool isMicAvailable = true;
            try
            {
                var mediaCapture = new Windows.Media.Capture.MediaCapture();
                var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio;
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception)
            {
                isMicAvailable = false;
            }
            if (!isMicAvailable)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"));
            }
            else
            {
                NotifyUser("Microphone was enabled", NotifyType.StatusMessage);
            }
        }

        private void InputGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var cb = sender as Grid;
            if (cb.Name.Equals("MicInputGrid"))
            {
                SpeechRecognitionFromMicrophone();
            }
            else if (cb.Name.Equals("FileInputGrid"))
            {
                SpeechRecogntionFromStream();
            }
        }

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updated)
                return;
            var cb = sender as ComboBox;
            string str = ((ComboBoxItem)cb.SelectedItem).Tag.ToString();
            switch (cb.Name)
            {
                case "StandardRegionComboBox":
                    this.localSetting.StandardRegion = str;
                    break;
                case "CustomRegionComboBox":
                    this.localSetting.CustomRegion = str;
                    break;
                case "TranslatorRegionComboBox":
                    this.localSetting.TranslateRegion = str;
                    break;
                case "TranslatorFromLanguageComboBox":
                    this.localSetting.TranslateFromLanguage = str;
                    break;
                case "TranslatorToLanguageComboBox":
                    this.localSetting.TranslateToLanguage = str;
                    break;
                case "SpeechServerTypeComboBox":
                    this.localSetting.SpeechServerType = str;
                    break;
                default:
                    break;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!updated)
                return;
            var sk = sender as TextBox;
            string str = sk.Text;
            if (sk.Name.Equals("StandardSubscriptionKeyTextBox"))
                this.localSetting.StandardSubscriptionKey = str;
            else if (sk.Name.Equals("CustomSubscriptionKeyTextBox"))
                this.localSetting.CustomSubscriptionKey = str;
            else if (sk.Name.Equals("TranslatorSubscriptionKeyTextBox"))
                this.localSetting.TranslateSubscriptionKey = str;
            else if (sk.Name.Equals("CustomDeploymentIDTextBox"))
                this.localSetting.CustomDeploymentID = str;
        }

        private enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };

        /// <summary>
        /// Display a message to the user.
        /// This method may be called from any thread.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        private void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }

        /// <summary>
        /// Update status of message in StatusBorder.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }
            StatusBlock.Text = strMessage;
            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }
            // Raise an event if necessary to enable a screen reader to announce the status update.
            var peer = Windows.UI.Xaml.Automation.Peers.FrameworkElementAutomationPeer.FromElement(StatusBlock);
            if (peer != null)
            {
                peer.RaiseAutomationEvent(Windows.UI.Xaml.Automation.Peers.AutomationEvents.LiveRegionChanged);
            }
        }

        /// <summary>
        /// Display result in textbox.
        /// This method may be called from any thread.
        /// </summary>
        /// <param name="strMessage"></param>
        private void ShowResult(TextBlock tb, string strMessage)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                tb.Text = strMessage;
            }
            else
            {
                var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { tb.Text = strMessage; });
            }
        }

        /// <summary>
        /// Update background of grid.
        /// This method may be called from any thread.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="color"></param>
        private void UpdateGridBackground(Grid grid, Windows.UI.Color color)
        {
            if (Dispatcher.HasThreadAccess)
            {
                grid.Background = new SolidColorBrush(color);
            }
            else
            {
                var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    grid.Background = new SolidColorBrush(color);
                });
            }
        }

        /// <summary>
        /// Translate text.
        /// </summary>
        /// <param name="text"></param>
        private async void TranslateAsync(string text)
        {
            try
            {
                string host = "https://api.cognitive.microsofttranslator.com";
                string path = "/translate?api-version=3.0";
                string params_ = string.Format("&from={0}&to={1}", this.localSetting.TranslateFromLanguage, this.localSetting.TranslateToLanguage);

                string uri = host + path + params_;

                // NOTE: Replace this example key with a valid subscription key.
                string key = this.localSetting.TranslateSubscriptionKey;
                System.Object[] body = new System.Object[] { new { Text = text } };
                var requestBody = JsonConvert.SerializeObject(body);

                NotifyUser($"Start translate.", NotifyType.StatusMessage);
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                    var response = await client.SendAsync(request);
                    NotifyUser($"Stop translate.", NotifyType.StatusMessage);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        JArray jlist = JArray.Parse(responseBody);
                        string str = jlist[0]["translations"][0]["text"].ToString();
                        ShowResult(TBTranslation, str);
                    }
                    else
                    {
                        NotifyUser($"An error occurred. Status: {response.StatusCode.ToString()}, FailureReason: {response.ReasonPhrase}", NotifyType.StatusMessage);
                    }
                }
            }
            catch (System.FormatException ex)
            {
                NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }
    }
}
