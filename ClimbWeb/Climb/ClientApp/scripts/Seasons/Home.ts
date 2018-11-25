import * as Filter from "../Filterable.js";

Filter.FilterCollection.create();
initDescription();

function initDescription(): void {
    const description = $("#description");
    const key = "dismissed-description";

    description.on("close.bs.alert", () => window.localStorage.setItem(key, "true"));

    showDescription(description, key);
}

function showDescription(description: JQuery<HTMLElement>, key: string): void {
    const hasDismissed = window.localStorage.getItem(key);
    if (hasDismissed == null) {
        description.prop("hidden", false);
    }
}