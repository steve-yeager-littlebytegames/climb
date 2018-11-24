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

    static create(filterBarId: string ="filter-bar", normalize: boolean = true): FilterCollection {
        const filterCollection = new FilterCollection(this.collectFilterables(normalize));
        filterCollection.registerFilter(filterBarId);
        return filterCollection;
    }

    private registerFilter(filterBarId: string): void {
        const filterInput = document.getElementById(filterBarId) as HTMLInputElement;
        if (!filterInput) throw new Error(`Could not find filter-bar`);

        filterInput.onkeyup = () => {
            const filter = filterInput.value.toLowerCase();
            this.filter(filter);
        }
    }

    private static collectFilterables(normalize: boolean): Filterable[] {
        const filterableElements = Array.from(document.getElementsByClassName(Filterable.className));
        return filterableElements.map(e => new Filterable(e, normalize));
    }

    filter(filter: string): void {
        this.filterables.forEach(x => x.tryFilter(filter));
    }
}