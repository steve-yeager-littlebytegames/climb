import { ClimbClient } from "../gen/climbClient.js";

registerButtons();

function registerButtons() {
    $("#generate-bracket-button").click(generateBracket);
}

function generateBracket() {
    const idString = $("#generate-bracket-button").attr("data-tournament-id");
    if(!idString) return console.error("Could not find Tournament ID.");

    const tournamentId = parseInt(idString);

    const tournamentApi = new ClimbClient.TournamentApi();
    tournamentApi.generateBracket(tournamentId)
        .then(() => window.location.href = `/tournaments/home/${tournamentId}`)
        .catch(() => alert("Could not generate bracket."));
}