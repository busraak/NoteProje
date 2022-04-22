using Note.Common;
using Note.Domain;
using Note.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Note.WebApp.Init
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            NotUser user = CurrentSession.User;

            if (user != null)
                return user.Username;
            else
                return "system";
            
            
        }
    }
}