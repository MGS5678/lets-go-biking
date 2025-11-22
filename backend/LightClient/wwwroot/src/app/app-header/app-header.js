class AppHeader extends HTMLElement {

    constructor() {
        super();
        this.input = "";
        this.debounceTimer = null;
        this.attachShadow({ mode: "open" });
    }

    async connectedCallback() {
        const response = await fetch("../../app-header/app-header.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));

        if (window.location.href.endsWith("homepage.html")) {
            this.shadowRoot.querySelector("#homepage")?.classList.add("selected");
        }
        else if (window.location.href.endsWith("itinerary.html")) {
            this.shadowRoot.querySelector("#itinerary")?.classList.add("selected");
        }
        else if (window.location.href.endsWith("whoweare.html")) {
            this.shadowRoot.querySelector("#whoweare")?.classList.add("selected");
        }
    }
}

customElements.define("app-header", AppHeader);