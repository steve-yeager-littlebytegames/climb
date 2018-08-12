import * as React from "react";
import { ClimbClient } from "../gen/climbClient";

interface ISetCountProps {
    set: ClimbClient.SetDto;
}

export class SetCount extends React.Component<ISetCountProps> {
    render() {
        const set = this.props.set;

        return (
            <div className="d-flex justify-content-center">
                <h1>{set.player1Score || 0} - {set.player2Score || 0}</h1>
            </div>
        );
    }
}