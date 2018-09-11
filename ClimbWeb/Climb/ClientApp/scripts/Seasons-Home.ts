import { ClimbClient } from "../gen/climbClient.js";
import * as Filter from "./Filterable.js";

registerButtons();
const filterables = Filter.FilterCollection.create(true);
registerFilter();

function registerButtons() {
    const leaveSeasonButton = document.getElementById("leave-season-button");
    if (leaveSeasonButton) {
        const participantIdString = leaveSeasonButton.getAttribute("data-participantID");
        if (participantIdString) {
            const participantId = parseInt(participantIdString);
            leaveSeasonButton.onclick = () => leaveSeason(participantId);
        }
    }
}

function leaveSeason(participantId: number) {
    const seasonApi = new ClimbClient.SeasonApi();
    seasonApi.leave(participantId)
        .then(() => {
            window.location.reload();
        })
        .catch((reason: any) => alert(`Could not leave season.\n${reason}`));
}

function registerFilter() {
    const filterInput = document.getElementById("filter-bar") as HTMLInputElement;
    if (!filterInput) throw new Error(`Could not find filter-bar`);

    filterInput.onkeyup = e => {
        const filter = filterInput.value.toLowerCase();
        console.log(filter);
        filterables.forEach(x => x.tryFilter(filter));
    }
}