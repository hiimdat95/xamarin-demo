using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TravellerApp.Services
{
    public class BaseService
    {
        protected readonly IRestClient RestClient;

        public BaseService()
        {
            RestClient = DependencyService.Resolve<IRestClient>(DependencyFetchTarget.NewInstance);
        }
    }
}
