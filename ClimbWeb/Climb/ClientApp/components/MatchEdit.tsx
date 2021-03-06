﻿import * as React from "react";

import { ClimbClient } from "../gen/climbClient";
import { Submit } from "./Submit";

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

        if (!match.player1Characters || !match.player2Characters) {
            throw new Error("No match characters found.");
        }
        matchToEdit.player1Characters = match.player1Characters.slice(0);
        matchToEdit.player2Characters = match.player2Characters.slice(0);

        this.state = {
            match: matchToEdit,
        };
    }

    render() {
        const match = this.state.match;
        const game = this.props.game;

        if (!game.characters || !game.stages) {
            throw new Error("No characters or stages loaded.");
        }
        if (!match.player1Characters || !match.player2Characters) {
            throw new Error("No match characters loaded.");
        }

        game.characters.sort((c1, c2) => c1.name.localeCompare(c2.name));
        game.stages.sort((s1, s2) => s1.name.localeCompare(s2.name));

        const characters = game.characters.map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>);
        const canOk = match.player1Score !== match.player2Score;
        const stageInput = this.renderStageInput(game.stages, match.stageID);

        return (
            <div className="container">
                <div className="d-flex justify-content-start">
                    <button className="btn btn-cstm-secondary" onClick={this.props.onDelete}>Delete</button>
                </div>

                <h3>{game.matchName} {match.index + 1}</h3>
                {this.renderPlayerInputs(1, this.props.player1Name, characters, match.player1Characters, game)}
                <hr/>
                {this.renderPlayerInputs(2, this.props.player2Name, characters, match.player2Characters, game)}
                {stageInput}

                <div className="d-flex justify-content-between">
                    <button className="btn btn-cstm-secondary" onClick={this.props.onCancel}>Cancel</button>
                    <button className="btn btn-cstm-primary"
                            disabled={!canOk}
                            onClick={() => this.props.onEdit(this.state.match)}>
                        Ok
                    </button>
                </div>
            </div>
        );
    }

    private renderPlayerInputs(playerNumber: number,
        playerName: string,
        characters: JSX.Element[],
        characterValues: number[],
        game: ClimbClient.GameDto) {
        const match = this.state.match;
        const score = playerNumber === 1 ? match.player1Score : match.player2Score;

        const characterInputs: JSX.Element[] = [];
        for (let i = 0; i < game.charactersPerMatch; i++) {
            const onChange = (e: any) => this.updateCharacter(playerNumber, i, parseInt(e.currentTarget.value, 10));
            characterInputs.push(
                <select className="form-control" key={i} value={characterValues[i]} onChange={onChange}>{characters}</select>
            );
        }

        return (
            <div>
                {
                    /*Player Name*/
                }
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">Player {playerNumber}</label>
                    <div className="col-8">
                        <input type="text" readOnly className="form-control" value={playerName}/>
                    </div>
                </div>
                {
                    /*Score*/
                }
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">{game.scoreName}</label>
                    <div className="col-8">
                        <input className="form-control" type="number"
                               value={score} min="0" max={game.maxMatchPoints}
                               onChange={(e: any) => this.updateScore(playerNumber, parseInt(e.currentTarget.value, 10))}/>
                    </div>
                </div>
                {
                    /*Characters*/
                }
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">{`Character${game.charactersPerMatch > 1 ? "s" : ""}`}</label>
                    <div className="col-8">
                        {characterInputs}
                    </div>
                </div>
            </div>
        );
    }

    private renderStageInput(stageValues: ClimbClient.StageDto[], currentStage: number | undefined) {
        if (stageValues.length === 0) {
            return null;
        }

        const stageOptions = stageValues.map((s: any) => <option key={s.id} value={s.id}>{s.name}</option>);
        stageOptions.splice(0, 0, <option key={0} value={undefined}>{Submit.missingStageName}</option>);

        const elements = (
            <div>
                <hr/>
                <div className="form-group row">
                    <label className="col-form-label col-4 text-right">Stage</label>
                    <div className="col-8">
                        <select className="form-control" value={currentStage}
                                onChange={(e: any) => this.updateStage(parseInt(e.currentTarget.value, 10))}>
                            {stageOptions}
                        </select>
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
        if (characters == null) {
            throw new Error("No characters selected.");
        }
        characters[characterIndex] = characterId;

        this.setState({ match: match });
    }

    private updateStage(stageId: number) {
        const match = this.state.match;
        match.stageID = stageId;
        this.setState({ match: match });
    }
}