import { ClimbClient } from "../gen/climbClient.js";

var leaveButton = document.getElementById("leave-league-button");
if (leaveButton != null) {
    const leagueUserIdString = leaveButton.getAttribute("data-leagueUserId");
    if (leagueUserIdString) {
        const leagueUserId = parseInt(leagueUserIdString);
        leaveButton.onclick = () => {
            if (confirm("Are you sure you want to leave this league? You'll forfeit any season sets.")) {
                leaveLeague(leagueUserId);
            }
        };
    }
}

function leaveLeague(leagueUserId: number) {
    const leagueApi = new ClimbClient.LeagueApi();
    leagueApi.leave(leagueUserId)
        .then(() => {
            window.location.reload();
        })
        .catch((reason: any) => alert(`Could not leave league.\n${reason}`));
}