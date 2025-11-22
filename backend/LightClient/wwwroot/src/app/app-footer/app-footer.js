class AppFooter extends HTMLElement {

    constructor() {
        super();
        this.input = "";
        this.debounceTimer = null;
        this.attachShadow({ mode: "open" });
    }

    async connectedCallback() {
        const response = await fetch("../../app-footer/app-footer.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));
    }
}

customElements.define("app-footer", AppFooter);
