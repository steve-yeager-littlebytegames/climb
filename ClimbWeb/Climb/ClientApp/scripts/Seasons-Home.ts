import { ClimbClient } from "../gen/climbClient.js";
import * as Filter from "./Filterable.js";

registerButtons();
Filter.FilterCollection.create();

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