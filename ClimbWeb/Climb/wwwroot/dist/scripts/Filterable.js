export class Filterable {
    constructor(element, normalize) {
        this.element = element;
        let dataKey = element.getAttribute(Filterable.keyAttribute);
        if (dataKey) {
            if (normalize) {
                dataKey = dataKey.toLowerCase();
            }
            this.keys = dataKey.split("|");
        }
        else {
            this.keys = new Array();
        }
    }
    isValid(filter) {
        return this.keys.find(k => k.includes(filter)) != undefined;
    }
    tryFilter(filter) {
        if (this.isValid(filter)) {
            this.element.classList.remove(Filterable.filteredClassName);
        }
        else {
            this.element.classList.add(Filterable.filteredClassName);
        }
    }
}
Filterable.className = "filterable";
Filterable.keyAttribute = "data-filter-keys";
Filterable.filteredClassName = "filtered";
export class FilterCollection {
    constructor(filterables) {
        this.filterables = filterables;
    }
    static create(filterBarId = "filter-bar", normalize = true) {
        const filterCollection = new FilterCollection(this.collectFilterables(normalize));
        filterCollection.registerFilter(filterBarId);
        return filterCollection;
    }
    registerFilter(filterBarId) {
        const filterInput = document.getElementById(filterBarId);
        if (!filterInput)
            throw new Error(`Could not find filter-bar`);
        filterInput.onkeyup = e => {
            const filter = filterInput.value.toLowerCase();
            this.filter(filter);
        };
    }
    static collectFilterables(normalize) {
        const filterableElements = Array.from(document.getElementsByClassName(Filterable.className));
        return filterableElements.map(e => new Filterable(e, normalize));
    }
    filter(filter) {
        this.filterables.forEach(x => x.tryFilter(filter));
    }
}
