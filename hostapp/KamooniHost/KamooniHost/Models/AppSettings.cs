using System;
using System.Collections.Generic;
using System.Text;

namespace KamooniHost.Models
{
    public class AppSettings : BaseModel
    {
        private bool checkInUseFrontCamera;
        
        public bool CheckInUseFrontCamera { get => checkInUseFrontCamera; set => SetProperty(ref checkInUseFrontCamera, value); }
    }
}
