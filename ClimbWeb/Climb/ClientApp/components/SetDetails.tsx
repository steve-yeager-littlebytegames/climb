import * as React from "react";
import { ClimbClient } from "../gen/climbClient";

interface ISetDetailsProps {
    set: ClimbClient.SetDto;
    player1: ClimbClient.LeagueUserDto;
    player2: ClimbClient.LeagueUserDto;
}

export class SetDetails extends React.Component<ISetDetailsProps> {
    render() {
        const set = this.props.set;
        const player1 = this.props.player1;
        const player2 = this.props.player2;
        const setType = set.seasonIndex == null ? "Challenge" : `Season ${set.seasonIndex}`;

        return (
            <div className="card">
                {/*Set info*/}
                <div className="card-header">
                    <span>{set.leagueName} - {setType} - {set.dueDate.toLocaleDateString()}</span>
                </div>

                <div className="p-2">
                    <div className="d-flex justify-content-between">
                        {/*Player 1*/}
                        <div className="d-flex">
                            <img src="https://cdn2.iconfinder.com/data/icons/professions/512/user_boy_avatar-64.png"/>
                            <div className="ml-2">
                                <div>↓</div>
                                <h3>{player1.rank === 0 ? "•" : player1.rank}</h3>
                            </div>
                        </div>

                        <h1 className="align-self-center">vs</h1>

                        {/*Player 2*/}
                        <div className="d-flex">
                            <div className="mr-2">
                                <div>↑</div>
                                <h3>{player2.rank === 0 ? "•" : player2.rank}</h3>
                            </div>
                            <img src="https://cdn2.iconfinder.com/data/icons/professions/512/user_boy_avatar-64.png"/>
                        </div>
                    </div>

                    {/*Usernames*/}
                    <div className="d-flex justify-content-between">
                        <div>{player1.username}</div>
                        <div>{player2.username}</div>
                    </div>
                </div>
            </div>
        );
    }
}