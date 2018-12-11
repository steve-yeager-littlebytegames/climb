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
        const rankTrend1 = this.getRankClass(player1.rankTrend);
        const rankTrend2 = this.getRankClass(player2.rankTrend);

        return (
            <div className="card">
                <div className="card-header">
                    <span>{set.leagueName} - {ClimbClient.SetTypes[set.setType]} - {set.dueDate.toLocaleDateString()}</span>
                </div>

                <div className="p-2">
                    <div className="d-flex justify-content-between">
                        <div className="d-flex">
                            <img src={player1.profilePicture} width="100" height="100"/>
                            <div className="ml-2">
                                <i className={`fas ${rankTrend1} rank-trend`}></i>
                                <h3>{player1.rank === 0 ? "•" : player1.rank}</h3>
                            </div>
                        </div>

                        <h1 className="align-self-center">vs</h1>

                        <div className="d-flex">
                            <div className="mr-2">
                                <i className={`fas ${rankTrend2} rank-trend`}></i>
                                <h3>{player2.rank === 0 ? "•" : player2.rank}</h3>
                            </div>
                            <img src={player2.profilePicture} width="100" height="100"/>
                        </div>
                    </div>

                    <div className="d-flex justify-content-between">
                        <a href={`/users/home/${player1.userID}`}>{player1.username}</a>
                        <a href={`/users/home/${player2.userID}`}>{player2.username}</a>
                    </div>
                </div>
            </div>
        );
    }

    private getRankClass(rankTrend: ClimbClient.RankTrends): string {
        switch (rankTrend) {
        case ClimbClient.RankTrends.None:
            return "fa-minus";
        case ClimbClient.RankTrends.Down:
            return "fa-arrow-down";
        case ClimbClient.RankTrends.Up:
            return "fa-arrow-up";
        }

        throw new Error();
    }
}