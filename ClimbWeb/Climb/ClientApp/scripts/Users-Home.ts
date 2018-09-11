import { ClimbClient } from "../gen/climbClient.js";
import { FilterCollection } from "./Filterable.js";

FilterCollection.create();

var openButton = document.getElementById("challenge-button");
if (openButton != null) {
    openButton.onclick = () => toggleChallengeModal(true);
}

var closeButton = document.getElementById("challenge-modal-close");
if (closeButton != null) {
    closeButton.onclick = () => toggleChallengeModal(false);
}

var requestButtons = document.getElementsByClassName("request-button");
for (let i = 0; i < requestButtons.length; i++) {
    const button = requestButtons[i] as HTMLButtonElement;
    const requester = button.getAttribute("data-requester");
    const challenged = button.getAttribute("data-challenged");
    const message = document.getElementById(`challenge-message-${challenged}`) as HTMLInputElement;

    if (requester != null && challenged != null) {
        button.onclick = () => sendRequest(parseInt(requester), parseInt(challenged), message.value);
    }
}

var acceptButtons = document.getElementsByClassName("request-accept");
for (let i = 0; i < acceptButtons.length; i++) {
    const button = acceptButtons[i] as HTMLButtonElement;
    const request = button.getAttribute("data-requestId");
    if (request != null) {
        button.onclick = () => respondToRequest(parseInt(request), true);
    }
}

var declineButtons = document.getElementsByClassName("request-decline");
for (let i = 0; i < declineButtons.length; i++) {
    const button = declineButtons[i] as HTMLButtonElement;
    const request = button.getAttribute("data-requestId");
    if (request != null) {
        button.onclick = () => respondToRequest(parseInt(request), false);
    }
}

function toggleChallengeModal(open: Boolean) {
    const challengeModal = document.getElementById("challenge-modal");
    if (challengeModal == null) {
        return;
    }

    challengeModal.hidden = !open;
}

function sendRequest(requesterId: number, challengedId: number, message: string) {
    const setApi = new ClimbClient.SetApi();
    setApi.challengeUser(requesterId, challengedId, message)
        .then(() => {
            window.location.reload();
        })
        .catch((reason: any) => alert(`Could not challenge user.\n${reason}`));
}

function respondToRequest(requestId: number, accept: boolean) {
    const setApi = new ClimbClient.SetApi();
    setApi.respondToChallenge(requestId, accept)
        .then(() => {
            window.location.reload();
        })
        .catch((reason: any) => alert(`Could not respond to challenge request.\n${reason}`));

}