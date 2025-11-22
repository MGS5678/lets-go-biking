const retrievedStartPoint = JSON.parse(localStorage.getItem("startPoint"));
const retrievedEndPoint = JSON.parse(localStorage.getItem("endPoint"));

let startAddress = "";
let endAddress = "";

document.addEventListener("DOMContentLoaded", () => {
    const startInput = document.getElementById("start-input");
    const endInput = document.getElementById("end-input");

    startInput.value = retrievedStartPoint;
    startAddress = retrievedStartPoint;
    endInput.value = retrievedEndPoint;
    endAddress = retrievedEndPoint;

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
            console.log("do smth");
        }
    }

    updateStatus();
});