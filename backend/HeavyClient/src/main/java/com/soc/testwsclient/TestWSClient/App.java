package com.soc.testwsclient.TestWSClient;

import com.soap.ws.client.generated.IOrchestratorService;
import com.soap.ws.client.generated.OrchestratorService;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
        OrchestratorService service = new OrchestratorService();
        IOrchestratorService port = service.getBasicHttpBindingIOrchestratorService();

        String result = port.getStations("Toulouse");
        System.out.println(result);
    }
}
