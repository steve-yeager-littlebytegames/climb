import * as React from "react";

import { ClimbClient } from "../gen/climbClient";

interface IMatchEditProps {
    game: ClimbClient.GameDto;
    match: ClimbClient.MatchDto;
    player1Name: string;
    player2Name: string;
    onEdit: (match: ClimbClient.MatchDto) => void;
    onCancel: () => void;
    onDelete: () => void;
}

interface IMatchEditState {
    match: ClimbClient.MatchDto;
}

export class MatchEdit extends React.Component<IMatchEditProps, IMatchEditState> {
    constructor(props: IMatchEditProps) {
        super(props);

        const match = this.props.match;
        const matchToEdit = new ClimbClient.MatchDto(match);

        if (!match.player1Characters || !match.player2Characters) throw new Error();
        matchToEdit.player1Characters = match.player1Characters.slice(0);
        matchToEdit.player2Characters = match.player2Characters.slice(0);

        this.state = {
            match: matchToEdit,
        }
    }

    render() {
        const match = this.state.match;
        const game = this.props.game;

        if (!game.characters || !game.stages) throw new Error();
        if (!match.player1Characters || !match.player2Characters) throw new Error();

        const characters = game.characters.map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>);
        const canOk = match.player1Score !== match.player2Score;
        const stageInput = this.renderStageInput(game.hasStages, game.stages, match.stageID);

        return (
            <div className="container">
                <div className="d-flex justify-content-start">
                    <button className="btn btn-sm btn-danger" onClick={this.props.onDelete}>Delete</button>
                </div>

                <h3>Match {match.index + 1}</h3>
                {this.renderPlayerInputs(1, this.props.player1Name, characters, match.player1Characters, game.charactersPerMatch)}
                <hr/>
                {this.renderPlayerInputs(2, this.props.player2Name, characters, match.player2Characters, game.charactersPerMatch)}
                {stageInput}

                <div className="d-flex justify-content-between">
                    <button className="btn btn-secondary" onClick={this.props.onCancel}>Cancel</button>
                    <button className="btn btn-primary" disabled={!canOk} onClick={() => this.props.onEdit(this.state.match)}>Ok</button>
                </div>
            </div>
        );
    }

    private renderPlayerInputs(playerNumber: number, playerName: string, characters: JSX.Element[], characterValues: number[], characterCount: number) {
        const match = this.state.match;
        const score = playerNumber === 1 ? match.player1Score : match.player2Score;
        
        let characterInputs:JSX.Element[] = [];
        for (let i = 0; i < characterCount; i++) {
            characterInputs.push(
                <select className="form-control" value={characterValues[i]} onChange={(e: any) => this.updateCharacter(playerNumber, i, parseInt(e.currentTarget.value))}>{characters}</select>
            );
        }

        return (
            <div>
                {/*Player Name*/}
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">Player {playerNumber}</label>
                    <div className="col-8">
                        <input type="text" readOnly className="form-control" value={playerName} />
                    </div>
                </div>

                {/*Score*/}
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">Score</label>
                    <div className="col-8">
                        <input className="form-control" type="number" value={score} min="0" max="2" onChange={(e: any) => this.updateScore(playerNumber, parseInt(e.currentTarget.value))}/>
                    </div>
                </div>

                {/*Characters*/}
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">{`Character${characterCount > 1 ? "s" : ""}`}</label>
                    <div className="col-8">
                        {characterInputs}
                    </div>
                </div>
            </div>
        );
    }

    private renderStageInput(hasStages: boolean, stageValues: ClimbClient.StageDto[], currentStage: number | undefined) {
        if (!hasStages) {
            return null;
        }

        const stageOptions = stageValues.map((s: any) => <option key={s.id} value={s.id}>{s.name}</option>);

        const elements = (
            <div>
                <hr/>
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">Stage</label>
                    <div className="col-8">
                        <select className="form-control" value={currentStage} onChange={(e: any) => this.updateStage(parseInt(e.currentTarget.value))}>{stageOptions}</select>
                    </div>
                </div>
            </div>);

        return elements;
    }

    private updateScore(playerNumber: number, score: number) {
        const match = this.state.match;
        if (playerNumber === 1) {
            match.player1Score = score;
        } else {
            match.player2Score = score;
        }

        this.setState({ match: match });
    }

    private updateCharacter(playerNumber: number, characterIndex: number, characterId: number) {
        const match = this.state.match;

        const characters = playerNumber === 1 ? match.player1Characters : match.player2Characters;
        if (characters == null) throw new Error();
        characters[characterIndex] = characterId;

        this.setState({ match: match });
    }

    private updateStage(stageId: number) {
        const match = this.state.match;
        match.stageID = stageId;
        this.setState({ match: match });
    }
}