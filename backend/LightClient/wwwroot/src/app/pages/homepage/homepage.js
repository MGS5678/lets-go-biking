const startInput = document.getElementById("start-input");
const endInput = document.getElementById("end-input");

var startAddress = "";
var endAddress = "";

startInput.addEventListener("address-selected", (event) => {
    startAddress = event.detail;
    updateStatus();
});

endInput.addEventListener("address-selected", (event) => {
    endAddress = event.detail;
    updateStatus();
});

function updateStatus() {
    if (startAddress != "" && endAddress != "") {
        localStorage.setItem("startPoint", JSON.stringify(startAddress));
        localStorage.setItem("endPoint", JSON.stringify(endAddress));
        window.location.href = "../itinerary/itinerary.html";
    }
}

(async () => {
    const response = await fetch("http://localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService/contracts?cityName=Toulouse");
    const json = await response.json();
    console.log(json);
})();