using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;
using RestWebApi.Services.EndpointBehaviour;

namespace RestWebApi.Services.EndpointBehaviour
{

    public class SimpleEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // No implementation necessary  
        }

        private SimpleMessageInspector myMessageInspector = new SimpleMessageInspector();

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(myMessageInspector);
        }

   
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // No implementation necessary  
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // No implementation necessary  
        }
   
    }
}