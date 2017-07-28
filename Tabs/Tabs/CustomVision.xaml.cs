using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tabs.DataModels;
using Xamarin.Forms;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void LoadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            TextOutput.Text = "";


            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Sorry!", "No camera available.", "OK");
                return;
            }

            TextOutput.Text = "Retrieving photo...";

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            TextOutput.Text = "Searching for dog...";

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await MakePredictionRequest(file);

        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "352ea6badea64bb8b78645ccd0a033ef");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/7a883dc6-e77b-4c96-baad-64dac15d24df/image?iterationId=0595fb60-819a-4003-9f7f-c18400e00f3f";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    Model.EvaluationModel responseModel = JsonConvert.DeserializeObject<Model.EvaluationModel>(responseString);

                    var Tag = from p in responseModel.Predictions select p.Tag;
                    var Probability = from p in responseModel.Predictions select p.Probability;

                    TextOutput.Text = "";

                    if (Probability.ElementAt(0) > 0.5)
                    {
                        if (Tag.ElementAt(0) == "Dog")
                        {
                            TextOutput.Text = "Dog detected! Woof!";

                            await postCountAsync();

                            async Task postCountAsync()
                            {

                                DogDetectorModel model = new DogDetectorModel()

                                {
                                    ScanResult = "Found a dog!"

                                };

                                await AzureManager.AzureManagerInstance.PostDogInformation(model);
                            }

                        }

                        else
                        {
                            TextOutput.Text = "That's a cat, silly!";

                            await postCountAsync();

                            async Task postCountAsync()
                            {

                                DogDetectorModel model = new DogDetectorModel()

                                {
                                    ScanResult = "Found a... cat?"

                                };

                                await AzureManager.AzureManagerInstance.PostDogInformation(model);
                            }

                        }

                    }

                    else
                    {
                        TextOutput.Text = "No dog found. :(";

                        await postCountAsync();

                        async Task postCountAsync()
                        {

                            DogDetectorModel model = new DogDetectorModel()

                            {
                                ScanResult = "No dog found. :("

                            };

                            await AzureManager.AzureManagerInstance.PostDogInformation(model);
                        }

                    }

                }

                    file.Dispose();

            }

        }

    }

}