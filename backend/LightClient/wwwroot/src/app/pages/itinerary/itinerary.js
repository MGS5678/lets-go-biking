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
});

async function updateStatus() {
    if (!startAddress || !endAddress) return;

    const url = "http://localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService/route?address1=" + encodeURIComponent("15 Rue Dejean, 80000 Amiens") + "&address2=" + encodeURIComponent("181 Av. de Grande Bretagne, 31300 Toulouse");
    //const url = "http://localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService/route?address1=" + encodeURIComponent(startAddress) + "&address2=" + encodeURIComponent(endAddress);

    const response = await fetch(url);
    const json = JSON.parse(await response.json());

    for (const mapObject of mapObjects) {
        mapObject.remove();
    }
    mapObjects = [];

    if (json.mode === "multimodal") {
        const cos1 = json.route.trajet1.metadata.query.coordinates;
        const cos2 = json.route.trajet2.metadata.query.coordinates;
        const cos3 = json.route.trajet3.metadata.query.coordinates;

        mapObjects.push(L.marker([cos1[0][1], cos1[0][0]]).addTo(map).bindPopup("Start point"));
        mapObjects.push(L.geoJSON(json.route.trajet1).addTo(map));
        mapObjects.push(L.marker([cos2[0][1], cos2[0][0]], { icon: bikeIcon }).addTo(map).bindPopup("Get your bike here"));
        mapObjects.push(L.geoJSON(json.route.trajet2).addTo(map));
        mapObjects.push(L.marker([cos2[1][1], cos2[1][0]], { icon: bikeIcon }).addTo(map).bindPopup("Drop your bike here"));
        mapObjects.push(L.geoJSON(json.route.trajet3).addTo(map));
        mapObjects.push(L.marker([cos3[1][1], cos3[1][0]]).addTo(map).bindPopup("End point"));

        map.setView([cos2[0][1], cos2[0][0]], 13);
    } else {
        console.log(json);
        const cos = json.route.metadata.query.coordinates;

        mapObjects.push(L.geoJSON(json.route).addTo(map));
        mapObjects.push(L.marker([cos[0][1], cos[0][0]]).addTo(map));
        mapObjects.push(L.marker([cos[1][1], cos[1][0]]).addTo(map));

        map.setView([cos[0][1], cos[0][0]], 13);
    }
};