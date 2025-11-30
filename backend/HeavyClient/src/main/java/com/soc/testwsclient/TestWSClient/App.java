package com.soc.testwsclient.TestWSClient;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.soap.ws.client.generated.IOrchestratorService;
import com.soap.ws.client.generated.OrchestratorService;
import com.sun.xml.ws.fault.ServerSOAPFaultException;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
        Scanner scanner = new Scanner(System.in);
        OrchestratorService service = new OrchestratorService();
        IOrchestratorService port = service.getBasicHttpBindingIOrchestratorService();

        String startAddress = "";
        String endAddress = "";
        List<JsonNode> routes = new ArrayList<>();

        System.out.println("Choisi l'adresse de départ :");
        startAddress = scanner.nextLine();
        System.out.println("Choisi l'address d'arrivée :");
        endAddress = scanner.nextLine();

        String json = "";
        try {
            json = port.getRouteFromAddresses(startAddress, endAddress);
        } catch (ServerSOAPFaultException e) {
            System.out.println("Les addresses que vous avez fournies sont fausses.");
        }
        try {
            ObjectMapper mapper = new ObjectMapper();
            JsonNode root = mapper.readTree(json);
            for (int i = 0; i < root.size(); i++) {  // looping over routes
                routes.add(root.get(i).get("features")
                        .get(0)
                        .get("properties")
                        .get("segments")
                        .get(0)
                        .get("steps"));
            }
        } catch (JsonProcessingException e) {
            e.printStackTrace();
        }

        for (JsonNode route : routes) {
            int size = route.size();
            for (int i = 0; i < size; i++) {
                System.out.println(route.get(i).get("instruction").asText());
            }
        }
    }
}
