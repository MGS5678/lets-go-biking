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
        [WebGet(UriTemplate = "contracts?cityName={cityName}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetContractNameFromCity(string cityName);

        [OperationContract]
        [WebGet(UriTemplate = "stations?contract={contract}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetStations(string contract);

        [OperationContract]
        [WebGet(UriTemplate = "coords?address={address}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetCoords(string address);

        //[OperationContract]
        //[WebGet(UriTemplate = "route?coords1={coords1}&coords2={coords2}&meansTransport={meansTransport}",
        //        ResponseFormat = WebMessageFormat.Json,
        //        BodyStyle = WebMessageBodyStyle.Bare)]
        //Task<string> GetRoute(string coords1, string coords2, string meansTransport);

        [OperationContract]
        [WebGet(UriTemplate = "route?address1={address1}&address2={address2}",
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetRouteFromAddresses(string address1, string address2);
    }
}