using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    [ServiceContract]
    public interface IProxyService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/contractName?city={cityName}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetContractNameFromCity(string cityName);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/stations?contract={contractName}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetStationsJson(string contractName);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/coords?address={address}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetCoordsJson(string address);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/route?coords1={coords1}&coords2={coords2}&meansTransport={meansTransport}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetRoute(string coords1, string coords2, string meansTransport);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/allstations",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Task<string> GetAllStations();
    }

}
