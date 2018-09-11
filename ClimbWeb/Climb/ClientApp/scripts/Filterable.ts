export class Filterable {
    static readonly className = "filterable";
    static readonly keyAttribute = "data-filter-keys";
    static readonly filteredClassName = "filtered";

    private readonly element: Element;
    private readonly keys: string[];

    constructor(element: Element, normalize: boolean) {
        this.element = element;

        let dataKey = element.getAttribute(Filterable.keyAttribute);
        if (dataKey) {
            if (normalize) {
                dataKey = dataKey.toLowerCase();
            }

            this.keys = dataKey.split("|");
        } else {
            this.keys = new Array<string>();
        }
    }

    isValid(filter: string): boolean {
        return this.keys.find(k => k.includes(filter)) != undefined;
    }

    tryFilter(filter: string): void {
        if (this.isValid(filter)) {
            this.element.classList.remove(Filterable.filteredClassName);
        } else {
            this.element.classList.add(Filterable.filteredClassName);
        }
    }
}

export class FilterCollection {
    private readonly filterables: Filterable[];

    private constructor(filterables: Filterable[]) {
        this.filterables = filterables;
    }

    static create(normalize: boolean): FilterCollection {
        return new FilterCollection(this.collectFilterables(normalize));
    }

    private static collectFilterables(normalize: boolean): Filterable[] {
        const filterableElements = Array.from(document.getElementsByClassName(Filterable.className));
        return filterableElements.map(e => new Filterable(e, normalize));
    }
}