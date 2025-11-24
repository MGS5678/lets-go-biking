using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorService
{
    internal class CorsEnabledBehavior : IEndpointBehavior
    {
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher dispatcher)
        {
            dispatcher.DispatchRuntime.MessageInspectors.Add(new CorsMessageInspector());
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
        public void Validate(ServiceEndpoint endpoint) { }
    }

    public class CorsMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var http = WebOperationContext.Current.IncomingRequest;

            if (http.Method == "OPTIONS")
            {
                var outgoing = WebOperationContext.Current.OutgoingResponse;
                outgoing.Headers.Add("Access-Control-Allow-Origin", "*");
                outgoing.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                outgoing.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                outgoing.StatusCode = System.Net.HttpStatusCode.OK;

                return new object();
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var outgoing = WebOperationContext.Current.OutgoingResponse;

            outgoing.Headers.Add("Access-Control-Allow-Origin", "*");
            outgoing.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            outgoing.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }
    }

}