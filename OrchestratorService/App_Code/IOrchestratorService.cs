using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RouteurLibrary
{
	[ServiceContract]
	public interface IOrchestratorService
	{
		[OperationContract]
		[WebInvoke(UriTemplate="Get?fromlat={fromlat}&fromlng={fromlng}&tolat={tolat}&tolng={tolng}",
			Method = "GET",
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare)]
		Task<string> GetData(double fromlat, double fromlng, double tolat, double tolng);
	}
}