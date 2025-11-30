using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorService
{
    [ServiceContract]
    public interface IOrchestratorService
    {

        [OperationContract]
        [WebGet(UriTemplate = "coords?address={address}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetCoords(string address);

        [OperationContract]
        [WebGet(UriTemplate = "route?address1={address1}&address2={address2}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetRouteFromAddresses(string address1, string address2);
    }
}