import { ClimbClient } from "../../gen/climbClient.js";

const createTournamentButton = $("#tournament-button");
createTournamentButton.click(createTournament);

function createTournament() {
    const tournamentApi = new ClimbClient.TournamentApi();
    const request = new ClimbClient.CreateRequest();
    request.leagueID = parseInt(<string>$("#tournament-league-id").val());
    request.name = <string>$("#tournament-name").val();
    request.seasonID = parseInt(<string>$("#tournament-season-id").val());

    tournamentApi.post(request)
        .then(tournament => window.location.href = `/tournaments/home/${tournament.id}`)
        .catch(() => alert("Could not create tournament."));
}