import * as React from "react";

import { ClimbClient } from "../gen/climbClient";

interface IMatchSummaryProps {
    game: ClimbClient.GameDto;
    match: ClimbClient.MatchDto;
    onSelect: (match: ClimbClient.MatchDto) => void;
}

export class MatchSummary extends React.Component<IMatchSummaryProps> {
    constructor(props: IMatchSummaryProps) {
        super(props);

        this.renderCharacter = this.renderCharacter.bind(this);
    }

    render() {
        const match = this.props.match;
        const game = this.props.game;

        if (match.player1Characters == null) return null;
        if (match.player2Characters == null) return null;

        const p1Characters = match.player1Characters.map(this.renderCharacter);
        const p2Characters = match.player2Characters.map(this.renderCharacter);

        const stageView = this.renderStage(match, game);

        return (
            <div className="card" onClick={() => this.props.onSelect(match)}>
                <h6 className="card-header mb-1">Match {match.index + 1}</h6>
                <div className="d-flex justify-content-around card-text align-items-center px-2">
                    <div>{p1Characters}</div>
                    <h4 className="match-score">{match.player1Score} - {match.player2Score}</h4>
                    <div>{p2Characters}</div>
                </div>
                {stageView}
            </div>
        );
    }

    private renderCharacter(characterId: number, key: number) : JSX.Element {
        const game = this.props.game;
        if (!game || !game.characters) return <img/>;
        const character = game.characters.find(gC => gC.id === characterId);
        if (!character) return <img/>;
        return <img key={key} src={character.picture} title={character.name} width="32" height="32"/>;
    }

    private renderStage(match: ClimbClient.MatchDto, game: ClimbClient.GameDto) {
        if (match.stageID == null) {
            return <div></div>;
        }

        if (!game.stages) throw new Error("Stages are not loaded.");

        const stage = game.stages.find((s: any) => s.id === match.stageID);
        if (!stage) throw new Error(`Could not find stage with ID '${match.stageID}'.`);
        const stageName = stage.name;

        return <div className="match-summary-stage">{stageName}</div>;
    }
}