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
        [WebGet(UriTemplate = "route?address1={address1}&address2={address2}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetRouteFromAddresses(string address1, string address2);

        [OperationContract]
        [WebGet(UriTemplate = "meteo?coords={coords}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetMeteoFromCoords(string coords);
    }
}