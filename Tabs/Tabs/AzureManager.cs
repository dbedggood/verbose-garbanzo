using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabs.DataModels;

namespace Tabs
{
    class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<DogDetectorModel> dogDetectorTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://dogdetector.azurewebsites.net");
            this.dogDetectorTable = this.client.GetTable<DogDetectorModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<DogDetectorModel>> GetDogInformation()
        {
            return await this.dogDetectorTable.ToListAsync();
        }

        public async Task PostDogInformation(DogDetectorModel dogModel)
        {
            await this.dogDetectorTable.InsertAsync(dogModel);
        }

    }
}
