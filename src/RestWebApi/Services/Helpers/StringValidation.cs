using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestWebApi.Services.Helpers
{
    public static class StringValidation
    {
        public static string Cut(this string value,bool isfullmsg)
        {
            string returnval = "";

            if (string.IsNullOrEmpty(value))
                return returnval;

            if (value.Length <= 500 && !isfullmsg)
                returnval = value;

            if(value.Length>500 && !isfullmsg)
                returnval = value.Substring(0, 500);


            if (value.Length <= 2048 && isfullmsg)
            {
                returnval = value;
            }
              

            if(value.Length > 2048 && isfullmsg)
            {
                returnval = value.Substring(0, 2048);
            }

            return returnval;

        }
    }
}
