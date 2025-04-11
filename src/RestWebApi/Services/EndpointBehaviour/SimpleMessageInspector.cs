using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace RestWebApi.Services.EndpointBehaviour
{
    // Client message inspector  
    public class SimpleMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {

            MessageBuffer msgbuf = reply.CreateBufferedCopy(int.MaxValue);
            reply = msgbuf.CreateMessage();
            Message tmpMessage = msgbuf.CreateMessage();
            XmlDictionaryReader xdr = tmpMessage.GetReaderAtBodyContents();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xdr);
            xdr.Close();
            // Now create StringWriter object to get data from xml document.<Type>Item</Type>


            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            string XmlString = sw.ToString();
         
            // Implement this method to inspect/modify messages after a message  
            // is received but prior to passing it back to the client
            Console.WriteLine("AfterReceiveReply called");
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            // Implement this method to inspect/modify messages before they
            // are sent to the service  
            Console.WriteLine("BeforeSendRequest called");
            return null;
        }

      
    }
}