import { ClimbClient } from "../gen/climbClient.js";

var openButton = document.getElementById("end-season-button");
if (openButton != null) {
    const seasonIdString = openButton.getAttribute("data-seasonID");
    if (seasonIdString) {
        const seasonId = parseInt(seasonIdString, 10);
        openButton.onclick = () => endSeason(seasonId);
    }
}

function endSeason(seasonId: number) {
    const seasonApi = new ClimbClient.SeasonApi();
    seasonApi.end(seasonId)
        .then(() => {
            window.location.reload();
        })
        .catch((reason: any) => alert(`Could not end season.\n${reason}`));
}