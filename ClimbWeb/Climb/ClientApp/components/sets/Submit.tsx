﻿import * as React from "react";
import { RouteComponentProps } from "react-router";
import { RingLoader } from "react-spinners";

import { ClimbClient } from "../../gen/climbClient";
import {MatchSummary} from "./MatchSummary";
import {MatchEdit} from "./MatchEdit";

interface ISetSubmitState {
    set: ClimbClient.SetDto | null;
    selectedMatch: ClimbClient.MatchDto | null;
    game: ClimbClient.Game | null;
}

export class Submit extends React.Component<RouteComponentProps<any>, ISetSubmitState> {
    client: ClimbClient.SetClient;
    setId: number;
    setRequest: ClimbClient.SubmitRequest;

    constructor(props: RouteComponentProps<any>) {
        super(props);

        this.client = new ClimbClient.SetClient(window.location.origin);
        this.setId = this.props.match.params.setId;
        this.setRequest = new ClimbClient.SubmitRequest();
        this.setRequest.setID = this.setId;

        this.state = {
            set: null,
            selectedMatch: null,
            game: null,
        };

        this.onSubmit = this.onSubmit.bind(this);
        this.onMatchEdited = this.onMatchEdited.bind(this);
    }

    componentDidMount() {
        this.loadSet();
    }

    render() {
        if (this.state.set == null || this.state.set.matches == null || this.state.game == null) {
            return <RingLoader color={"#123abc"}/>;
        }

        if (this.state.selectedMatch != null) {
            return <MatchEdit
                       match={this.state.selectedMatch}
                       game={this.state.game}
                       onDone={this.onMatchEdited}/>;
        }

        const matches =
            this.state.set.matches.map(
                (m, i) => <MatchSummary key={i} match={m} onSelect={m => this.setState({ selectedMatch: m })}/>);

        return (
            <div>
                <div>{matches}</div>
                <button onClick={this.onSubmit}>Submit</button>
            </div>
        );
    }

    private loadSet() {
        this.client.get(this.setId)
            .then(set => {
                if (set.matches != null) {
                    set.matches.sort((a, b) => a.index - b.index);
                }
                this.setState({ set: set });
                console.log(set);
                this.loadGame(set.gameID);
            })
            .catch(reason => `Could not load set\n${reason}`);
    }

    private loadGame(gameId: number) {
        const gameClient = new ClimbClient.GameClient(window.location.origin);
        gameClient.get(gameId)
            .then(game => this.setState({ game: game }))
            .catch(reason => alert(`Can't load game\n${reason}`));
    }

    private onMatchEdited(match: ClimbClient.MatchDto) {
        const set = this.state.set;
        if (set == null || set.matches == null) throw new Error();

        set.matches[match.index] = match;

        this.setState({
            selectedMatch: null,
            set: set,
        });
    }

    private onSubmit() {
        this.setRequest.matches = new Array<ClimbClient.MatchForm>();
        this.setRequest.matches[0] = new ClimbClient.MatchForm();
        this.setRequest.matches[0].player1Score = 1;
        this.setRequest.matches[0].player2Score = 2;
        this.setRequest.matches[0].player1Characters = [1, 2, 3];
        this.setRequest.matches[0].player2Characters = [2, 3, 1];
        this.setRequest.matches[0].stageID = 2;

        this.setRequest.matches[1] = new ClimbClient.MatchForm();
        this.setRequest.matches[1].player1Score = 0;
        this.setRequest.matches[1].player2Score = 2;
        this.setRequest.matches[1].player1Characters = [1, 2, 3];
        this.setRequest.matches[1].player2Characters = [2, 3, 1];
        this.setRequest.matches[1].stageID = 1;

        this.setRequest.matches[2] = new ClimbClient.MatchForm();
        this.setRequest.matches[2].player1Score = 0;
        this.setRequest.matches[2].player2Score = 2;
        this.setRequest.matches[2].player1Characters = [1, 2, 3];
        this.setRequest.matches[2].player2Characters = [2, 3, 1];
        this.setRequest.matches[2].stageID = 3;

        this.client.submit(this.setRequest)
            .then(() => {
                console.log("Set submitted!");
                window.location.reload();
            })
            .catch(reason => alert(`Could not submit set\n${reason}`));
    }
}