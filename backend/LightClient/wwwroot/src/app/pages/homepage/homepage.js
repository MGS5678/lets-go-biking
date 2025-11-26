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
        sessionStorage.setItem("startPoint", JSON.stringify(startAddress));
        sessionStorage.setItem("endPoint", JSON.stringify(endAddress));
        window.location.href = "../itinerary/itinerary.html";
    }
}