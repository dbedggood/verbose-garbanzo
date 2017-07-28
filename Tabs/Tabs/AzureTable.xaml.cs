using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using Tabs.DataModels;
using Xamarin.Forms;

namespace Tabs
{
    public partial class AzureTable : ContentPage
    {

        public AzureTable()
        {
            InitializeComponent();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<DogDetectorModel> dogInformation = await AzureManager.AzureManagerInstance.GetDogInformation();
            dogInformation.Reverse();
            DogList.ItemsSource = dogInformation;
        }

    }

}
