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
    player1LastCharacters: ClimbClient.CharacterDto[] | null;
    player2LastCharacters: ClimbClient.CharacterDto[] | null;
}

export class Submit extends React.Component<RouteComponentProps<any>, ISetSubmitState> {
    client: ClimbClient.SetApi;
    setId: number;

    static readonly missingStageName = "-----";

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
            player1LastCharacters: null,
            player2LastCharacters: null,
        };

        this.onSubmit = this.onSubmit.bind(this);
        this.onAddMatch = this.onAddMatch.bind(this);
        this.onMatchEdited = this.onMatchEdited.bind(this);
        this.onMatchCancelled = this.onMatchCancelled.bind(this);
        this.onMatchDelete = this.onMatchDelete.bind(this);
        this.onSelectMatch = this.onSelectMatch.bind(this);
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
                                              onSelect={this.onSelectMatch}/>);

        const canSubmit = set.player1Score !== set.player2Score;

        return (
            <div className="pb-4">
                <SetDetails set={set} player1={player1} player2={player2}/>
                <SetCount set={set}/>

                <div className="card-deck">{matches}</div>

                {!set.isLocked &&
                    <div className="mt-4">
                        <div>
                        <button id="add-button" className="btn btn-primary" onClick={this.onAddMatch}>Add {game.matchName}</button>
                        </div>
                
                        <div className="d-flex justify-content-end">
                            <button id="submit-button" className="btn btn-danger mt-4" disabled={!canSubmit} onClick={this.onSubmit}>Submit</button>
                        </div>
                    </div>
                }
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
                this.loadGame(set.gameID)
                    .then(game => {
                        this.setState({ game: game });
                        this.loadPlayers(set.player1ID, set.player2ID, game.charactersPerMatch);
                    })
                    .catch(reason => alert(`Can't load game\n${reason}`));
            })
            .catch(reason => `Could not load set\n${reason}`);
    }

    private loadGame(gameId: number): Promise<ClimbClient.GameDto> {
        const gameClient = new ClimbClient.GameApi(window.location.origin);
        return gameClient.get(gameId);
    }

    private loadPlayers(p1: number, p2: number, characterCount: number) {
        const leagueClient = new ClimbClient.LeagueApi(window.location.origin);
        leagueClient.getUser(p1)
            .then(player1 => this.setState({ player1: player1 }))
            .catch(reason => alert(`Could not load player 1\n${reason}`));
        leagueClient.getUser(p2)
            .then(player2 => this.setState({ player2: player2 }))
            .catch(reason => alert(`Could not load player 2\n${reason}`));

        leagueClient.getRecentCharacters(p1, characterCount)
            .then(characters => this.setState({ player1LastCharacters: characters }))
            .catch(reason => alert(`Could not load player 1 recent characters\n${reason}`));
        leagueClient.getRecentCharacters(p2, characterCount)
            .then(characters => this.setState({ player2LastCharacters: characters }))
            .catch(reason => alert(`Could not load player 2 recent characters\n${reason}`));
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
        const selectedMatch = this.state.selectedMatch;
        if (!selectedMatch) throw new Error("Selected match can't be null.");

        const index = selectedMatch.index;

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
                const referer = $("#referer").attr("data-referer");
                if (referer !== undefined && referer !== "") {
                    window.location.assign(referer);
                } else {
                    window.location.reload();
                }
            })
            .catch(reason => alert(`Could not submit set\n${reason}`));
    }

    private onAddMatch() {
        const set = this.state.set;
        const game = this.state.game;
        if (!set || !set.matches || !game) throw new Error();

        const newMatch = new ClimbClient.MatchDto();
        newMatch.index = set.matches.length;

        newMatch.player1Score = 0;
        newMatch.player2Score = 0;

        if (newMatch.index > 0) {
            const prevMatch = set.matches[newMatch.index - 1];
            if (!prevMatch.player1Characters || !prevMatch.player2Characters) throw new Error();
            newMatch.player1Characters = prevMatch.player1Characters.slice(0);
            newMatch.player2Characters = prevMatch.player2Characters.slice(0);
        } else {
            const characters = game.characters;
            
            newMatch.player1Characters = [];
            newMatch.player2Characters = [];

            const lastCharacters1 = this.state.player1LastCharacters;
            const lastCharacters2 = this.state.player2LastCharacters;

            for (let i = 0; i < game.charactersPerMatch; i++) {
                if (lastCharacters1) {
                    newMatch.player1Characters.push(lastCharacters1[i].id);
                } else {
                    newMatch.player1Characters.push(characters[i].id);
                }
                if (lastCharacters2) {
                    newMatch.player2Characters.push(lastCharacters2[i].id);
                } else {
                    newMatch.player2Characters.push(characters[i].id);
                }
            }
        }

        this.setState({ selectedMatch: newMatch });
    }

    private onSelectMatch(match: ClimbClient.MatchDto) {
        const set = this.state.set;
        if (set && !set.isLocked) {
            this.setState({ selectedMatch: match });
        }
    }
}