﻿import { ClimbClient } from "../gen/climbClient.js";

var leaveSeasonButton = document.getElementById("leave-season-button");
if (leaveSeasonButton) {
    const participantIdString = leaveSeasonButton.getAttribute("data-participantID");
    if (participantIdString) {
        const participantId = parseInt(participantIdString);
        leaveSeasonButton.onclick = () => leaveSeason(participantId);
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