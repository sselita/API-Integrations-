using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Exceptions
{
    [Serializable]
    public class InvalidPosCodeException : Exception
    {
        public InvalidPosCodeException()
        {

        }

        public InvalidPosCodeException(string name)
            : base(String.Format("Pos No already exists in navision: {0}", name))
        {

        }
    
    }
}