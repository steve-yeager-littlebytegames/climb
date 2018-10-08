import { ClimbClient } from "../gen/climbClient.js";
import * as Filter from "./Filterable.js";

registerButtons();
Filter.FilterCollection.create();
initDescription();

function registerButtons(): void {
    const leaveSeasonButton = document.getElementById("leave-season-button");
    if (leaveSeasonButton) {
        const participantIdString = leaveSeasonButton.getAttribute("data-participantID");
        if (participantIdString) {
            const participantId = parseInt(participantIdString);
            leaveSeasonButton.onclick = () => leaveSeason(participantId);
        }
    }
}

function leaveSeason(participantId: number): void {
    const seasonApi = new ClimbClient.SeasonApi();
    seasonApi.leave(participantId)
        .then(() => {
            window.location.reload();
        })
        .catch((reason: any) => alert(`Could not leave season.\n${reason}`));
}

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