const bikeIcon = L.icon({
  iconUrl: "../../../assets/images/bike.png",
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
});

async function updateStatus() {
    if (!startAddress || !endAddress) return;

    const url = "http://localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService/route?address1=" + encodeURIComponent(startAddress) + "&address2=" + encodeURIComponent(endAddress);

    const response = await fetch(url);
    const json = JSON.parse(await response.json());

    for (const mapObject of mapObjects) {
        mapObject.remove();
    }
    mapObjects = [];

    for (let i = 0; i < json.length; i++) {
        const cos = json[i].metadata.query.coordinates;
        console.log(`Trajet ${i + 1}:`, json[i].metadata.query.profile, cos);

        // walk
        if (json[i].metadata.query.profile === "foot-walking") {
            if (i == 0) {
                mapObjects.push(L.marker([cos[0][1], cos[0][0]]).addTo(map).bindPopup("Start point"));
                map.setView([cos[0][1], cos[0][0]], 13);
            }
            if (i == json.length - 1) {
                mapObjects.push(L.marker([cos[1][1], cos[1][0]]).addTo(map).bindPopup("End point"));
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