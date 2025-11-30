const bikeIcon = L.icon({
    iconUrl: "../../../assets/images/bike.png",  // path to your bike image
    iconSize: [32, 32],
    iconAnchor: [16, 32]
});

const retrievedStartPoint = JSON.parse(sessionStorage.getItem("startPoint"));
const retrievedEndPoint = JSON.parse(sessionStorage.getItem("endPoint"));

let startAddress = "";
let endAddress = "";


let mapObjects = [];

document.addEventListener("DOMContentLoaded", () => {
    const startInput = document.getElementById("start-input");
    const endInput = document.getElementById("end-input");

    startInput.value = retrievedStartPoint;
    startAddress = retrievedStartPoint;
    endInput.value = retrievedEndPoint;
    endAddress = retrievedEndPoint;

    startInput.addEventListener("address-selected", (event) => {
        startAddress = event.detail;
        sessionStorage.setItem("startPoint", JSON.stringify(startAddress));
        updateStatus();
    });

    endInput.addEventListener("address-selected", (event) => {
        endAddress = event.detail;
        sessionStorage.setItem("endPoint", JSON.stringify(endAddress));
        updateStatus();
    });

    if (startAddress && endAddress) {
        updateStatus();
    }
    initActiveMQ();
});

function updateMeteoOnMap(meteoData) {
    console.log("Received meteo data:", meteoData);
    console.log("#################Updating meteo info on map...");
    console.log("mapObjects count:", mapObjects.length);

    for (const item of meteoData.data) {
        console.log("Checking meteo for position:", item.meteo.latitude, item.meteo.longitude);
        let found = false;

        for (const obj of mapObjects) {
            if (obj.getLatLng) {
                const pos = obj.getLatLng();
                console.log("  Marker at:", pos.lat, pos.lng);
                if (Math.abs(pos.lat - item.meteo.latitude) < 0.1 && Math.abs(pos.lng - item.meteo.longitude) < 0.1) {
                    console.log("  -> MATCH FOUND!");

                    const originalText = obj.getPopup() ? obj.getPopup().getContent() : "";
                    const baseText = originalText.split('<div class="meteo-info"')[0].trim();

                    const temp = item.meteo.current_weather.temperature;
                    const wind = item.meteo.current_weather.windspeed;

                    const popupContent = `
                        <div class="popup-content">
                            <strong>${baseText}</strong>
                            <div class="meteo-info" style="margin-top: 8px; padding: 8px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 8px; color: white;">
                                <div style="display: flex; align-items: center; gap: 6px; margin-bottom: 4px;">
                                    <span style="font-size: 20px;">🌡️</span>
                                    <span style="font-size: 16px; font-weight: bold;">${temp}°C</span>
                                </div>
                                <div style="display: flex; align-items: center; gap: 6px;">
                                    <span style="font-size: 20px;">💨</span>
                                    <span style="font-size: 14px;">${wind} km/h</span>
                                </div>
                            </div>
                        </div>
                    `;

                    obj.bindPopup(popupContent);
                    found = true;
                    break;
                }
            }
        }
        if (!found) {
            console.log("  -> NO MATCH for this meteo position!");
        }
    }
}

function initActiveMQ() {
    console.log("######################Initializing ActiveMQ connection...");
    if (window.WebSocket) {
        var client, destination;

        var url = "ws://localhost:61614";
        destination = "/queue/MeteoNotifications";

        client = Stomp.client(url);

        client.debug = function (str) {
            $("#debug").append(document.createTextNode(str + "\n"));
        };

        client.connect(function (frame) {
            client.debug("connected to MeteoNotifs");
            client.subscribe(destination, function (message) {
                console.log("Received message:", message.body);
                updateMeteoOnMap(JSON.parse(message.body));

            });
        });

    } else {
        $("#connect").html("<h1>Get a new Web Browser!</h1><p>Your browser does not support WebSockets. This example will not work properly.<br>Please use a Web Browser with WebSockets support (WebKit or Google Chrome).</p>");
    }
}

async function updateStatus() {
    if (!startAddress || !endAddress) return;

    const url = "http://localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService/route?address1=" + encodeURIComponent(startAddress) + "&address2=" + encodeURIComponent(endAddress);
    //const url = "localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService/route?address1=" + encodeURIComponent("") + "&address2=" + encodeURIComponent("");

    const response = await fetch(url);
    const json = JSON.parse(await response.json());
    console.log(json);
    for (const mapObject of mapObjects) {
        mapObject.remove();
    }
    mapObjects = [];

    for (let i = 0; i < json.length; i++) {
        const cos = json[i].metadata.query.coordinates;
        console.log(`Trajet ${i + 1}:`, json[i].metadata.query.profile, cos); //  Debug ajout�

        // walk
        if (json[i].metadata.query.profile === "foot-walking") {
            // first & last routes, walking marker
            if (i == 0) {
                mapObjects.push(L.marker([cos[0][1], cos[0][0]]).addTo(map).bindPopup("Start point"));
                map.setView([cos[0][1], cos[0][0]], 13);
            }
            if (i == json.length - 1) {
                mapObjects.push(L.marker([cos[1][1], cos[1][0]]).addTo(map).bindPopup("End point")); //  Correction: "End point" au lieu de "Start point"
            }

            mapObjects.push(L.geoJSON(json[i]).addTo(map));
        }

        // bicycle
        if (json[i].metadata.query.profile === "cycling-regular") {
            mapObjects.push(L.marker([cos[0][1], cos[0][0]], { icon: bikeIcon }).addTo(map).bindPopup("Get your bike here"));
            mapObjects.push(L.geoJSON(json[i]).addTo(map));
            mapObjects.push(L.marker([cos[1][1], cos[1][0]], { icon: bikeIcon }).addTo(map).bindPopup("Drop your bike here"));
        }
    }
}