class SearchInput extends HTMLElement {

    constructor() {
        super();
        this.input = "";
        this.debounceTimer = null;
        this.attachShadow({ mode: "open" });
    }

    async connectedCallback() {
        const response = await fetch("../../search-input/search-input.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));


        const inputElement = this.shadowRoot.querySelector("#input");
        const suggestionsElement = this.shadowRoot.querySelector("#suggestions");

        this.timeout = null;

        inputElement.addEventListener("input", () => { this.searchSuggestions(inputElement, suggestionsElement); });
        suggestionsElement.addEventListener("click", (event) => { this.selectSuggestion(event, inputElement, suggestionsElement); });

        // doesn't work
        document.addEventListener("click", (e) => {
            if (!inputElement.contains(e.target) || !suggestionsElement.contains(e.target)) inputElement.innerHTML = "";
        });
    }

    set value(val) {
        var i = 1;
        const trySet = () => {
            const input = this.shadowRoot.querySelector("#input");
            if (i > 10) return;  // au bout de 10 essais on abandonne la valeur
            if (input) input.value = val;
            else setTimeout(trySet, 50, ++i);
        };
        trySet();
    }

    get value() {
        const input = this.shadowRoot.querySelector("#input");
        return input ? input.value : this._value;
    }

    searchSuggestions(inputElement, suggestionsElement) {
        const query = inputElement.value.trim();
        clearTimeout(this.timeout);

        if (!query) {
            suggestionsElement.innerHTML = "";
            return;
        }

        this.timeout = setTimeout(async () => {
            const url = `https://api-adresse.data.gouv.fr/search/?q=${query}&limit=5`;
            const res = await fetch(url);
            const data = await res.json();

            const suggestions = data.features || [];

            suggestionsElement.innerHTML = suggestions
                .map(s => `<li data-value="${s.properties.label}">${s.properties.label}</li>`)
                .join("");
        }, 500);
    }

    selectSuggestion(event, inputElement, suggestionElement) {
        const li = event.target;
        if (li.tagName === "LI") {
            inputElement.value = li.dataset.value;
            suggestionElement.innerHTML = "";
            this.dispatchEvent(new CustomEvent("address-selected", {
                detail: inputElement.value
            }));
        }
    }
}

customElements.define("search-input", SearchInput);