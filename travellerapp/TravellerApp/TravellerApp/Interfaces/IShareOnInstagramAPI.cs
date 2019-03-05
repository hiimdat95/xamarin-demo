using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TravellerApp.Interfaces
{
    public interface IShareOnInstagramAPI
    {
        Task<bool> OpenShareOnInstagram(string path, string content);
    }
}
