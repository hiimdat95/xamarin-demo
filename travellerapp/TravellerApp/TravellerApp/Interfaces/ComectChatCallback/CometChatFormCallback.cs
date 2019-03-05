using System;
using System.Collections.Generic;
using System.Text;

namespace TravellerApp.Interfaces.ComectChatCallback
{
    public interface CometChatFormCallback
    {
        void SuccessCallback(String username);
        void FailCallback(String jSONObject);
    }
}
