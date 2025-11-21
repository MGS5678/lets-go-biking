using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ProxyJCDecaux
{
    [ServiceContract]
    public interface IProxyService
    {
        [OperationContract]
        Task<string> GetContractNameFromCity(string cityName);

        [OperationContract]
        Task<string> GetStationsJson(string contractName);

    }

}
