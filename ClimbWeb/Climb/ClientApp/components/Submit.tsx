import * as React from "react";
import { RouteComponentProps } from "react-router-dom";
import { ClimbClient } from "../gen/climbClient";
import { SetDetails } from "./SetDetails";
import { MatchSummary } from "./MatchSummary";
import { MatchEdit } from "./MatchEdit";
import { SetCount } from "./SetCount";

interface ISetSubmitState {
    set: ClimbClient.SetDto | null;
    selectedMatch: ClimbClient.MatchDto | null;
    game: ClimbClient.GameDto | null;
    player1: ClimbClient.LeagueUserDto | null;
    player2: ClimbClient.LeagueUserDto | null;
}

export class Submit extends React.Component<RouteComponentProps<any>, ISetSubmitState> {
    client: ClimbClient.SetApi;
    setId: number;

    constructor(props: RouteComponentProps<any>) {
        super(props);

        this.client = new ClimbClient.SetApi(window.location.origin);
        this.setId = this.props.match.params.setID;

        this.state = {
            set: null,
            selectedMatch: null,
            game: null,
            player1: null,
            player2: null,
        };

        this.onSubmit = this.onSubmit.bind(this);
        this.onAddMatch = this.onAddMatch.bind(this);
        this.onMatchEdited = this.onMatchEdited.bind(this);
        this.onMatchCancelled = this.onMatchCancelled.bind(this);
        this.onMatchDelete = this.onMatchDelete.bind(this);
    }

    componentDidMount() {
        this.loadSet();
    }

    render() {
        const game = this.state.game;
        const set = this.state.set;
        const player1 = this.state.player1;
        const player2 = this.state.player2;
        if (!set || !set.matches || !game || !player1 || !player2)
            return <div id="loader">
                       Loading
                   </div>;

        if (this.state.selectedMatch != null) {
            return <MatchEdit match={this.state.selectedMatch}
                              game={game}
                              player1Name={player1.username}
                              player2Name={player2.username}
                              onEdit={this.onMatchEdited}
                              onCancel={this.onMatchCancelled}
                              onDelete={this.onMatchDelete}/>;
        }

        const matches = set.matches.map(
            (m: any, i: any) => <MatchSummary key={i}
                                              game={game}
                                              match={m}
                                              onSelect={match => this.setState({ selectedMatch: match })}/>);

        const canSubmit = set.player1Score !== set.player2Score;

        return (
            <div className="pb-4">
                <SetDetails set={set} player1={player1} player2={player2}/>
                <SetCount set={set}/>

                <div>
                    <div className="card-deck">{matches}</div>
                    <button id="add-button" className="btn btn-primary" onClick={this.onAddMatch}>Add Match</button>
                </div>

                <div className="d-flex justify-content-end">
                    <button id="submit-button" className="btn btn-danger mt-4" disabled={!canSubmit} onClick={this.onSubmit}>Submit</button>
                </div>
            </div>
        );
    }

    private loadSet() {
        this.client.get(this.setId)
            .then(set => {
                if (set.matches != null) {
                    set.matches.sort((a: any, b: any) => a.index - b.index);
                }
                this.setState({ set: set });
                this.loadGame(set.gameID);
                this.loadPlayers(set.player1ID, set.player2ID);
            })
            .catch(reason => `Could not load set\n${reason}`);
    }

    private loadGame(gameId: number) {
        const gameClient = new ClimbClient.GameApi(window.location.origin);
        gameClient.get(gameId)
            .then(game => this.setState({ game: game }))
            .catch(reason => alert(`Can't load game\n${reason}`));
    }

    private loadPlayers(p1: number, p2: number) {
        const leagueClient = new ClimbClient.LeagueApi(window.location.origin);
        leagueClient.getUser(p1)
            .then(player1 => this.setState({ player1: player1 }))
            .catch(reason => alert(`Could not load player 1\n${reason}`));
        leagueClient.getUser(p2)
            .then(player2 => this.setState({ player2: player2 }))
            .catch(reason => alert(`Could not load player 2\n${reason}`));
    }

    private onMatchCancelled() {
        this.setState({ selectedMatch: null });
    }

    private onMatchEdited(match: ClimbClient.MatchDto) {
        const set = this.state.set;
        if (!set || !set.matches) throw new Error();

        set.matches[match.index] = match;

        set.player1Score = set.player2Score = 0;
        for (let i = 0; i < set.matches.length; i++) {
            const match = set.matches[i];
            if (match.player1Score > match.player2Score) {
                ++set.player1Score;
            } else {
                ++set.player2Score;
            }
        }

        this.setState({
            selectedMatch: null,
            set: set,
        });
    }

    private onMatchDelete() {
        if (!this.state.selectedMatch) throw new Error("Selected match can't be null.");

        const index = this.state.selectedMatch.index;

        const set = this.state.set;
        if (!set || !set.matches) throw new Error("Set and Matches can't be null");
        set.matches.splice(index, 1);

        for (let i = 0; i < set.matches.length; i++) {
            set.matches[i].index = i;
        }

        this.setState({
            set: set,
            selectedMatch: null,
        });
    }

    private onSubmit() {
        const set = this.state.set;
        if (!set || !set.matches) throw new Error();

        const setRequest = new ClimbClient.SubmitRequest();
        setRequest.setID = set.id;
        setRequest.matches = new Array<ClimbClient.MatchForm>(set.matches.length);

        for (let i = 0; i < set.matches.length; i++) {
            const match = set.matches[i];
            const matchForm = new ClimbClient.MatchForm();
            matchForm.init(match);
            setRequest.matches[i] = matchForm;
        }

        this.client.submit(setRequest)
            .then(() => {
                console.log("Set submitted!");
                window.location.reload();
            })
            .catch(reason => alert(`Could not submit set\n${reason}`));
    }

    private onAddMatch() {
        const set = this.state.set;
        if (!set || !set.matches) throw new Error();

        const newMatch = new ClimbClient.MatchDto();
        newMatch.index = set.matches.length;

        if (newMatch.index > 0) {
            const prevMatch = set.matches[newMatch.index - 1];
            if (!prevMatch.player1Characters || !prevMatch.player2Characters) throw new Error();
            newMatch.player1Characters = prevMatch.player1Characters.slice(0);
            newMatch.player2Characters = prevMatch.player2Characters.slice(0);
        } else {
            newMatch.player1Characters = [];
            newMatch.player2Characters = [];
        }

        this.setState({ selectedMatch: newMatch });
    }
}