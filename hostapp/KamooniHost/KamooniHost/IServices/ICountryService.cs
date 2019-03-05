using KamooniHost.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KamooniHost.IServices
{
    public interface ICountryService
    {
        Task<StatesResult> GetStates(string countryCode);
    }
}
