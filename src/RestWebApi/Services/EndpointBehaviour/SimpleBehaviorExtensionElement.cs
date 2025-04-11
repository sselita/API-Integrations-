using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web;

namespace RestWebApi.Services.EndpointBehaviour
{
    // Configuration element
    public class SimpleBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SimpleEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            // Create the  endpoint behavior that will insert the message  
            // inspector into the client runtime  
            return new SimpleEndpointBehavior();
        }
    }
}