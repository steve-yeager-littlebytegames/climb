﻿import * as React from "react";
import { Route } from "react-router-dom";

import { ClimbClient } from "../../gen/climbClient";
import ApplicationUser = ClimbClient.ApplicationUser;

import { Navbar } from "../_common/Navbar";
import { Home } from "./Home";

interface ILayoutProps {
    user : ApplicationUser | null;
}

export class Layout extends React.Component<ILayoutProps> {
    render() {
        return (
            <div id="container">
                <Navbar user={this.props.user}/>
                <h1 id="title">Climb</h1>
                <Route exact path="/seasons/:seasonId" component={ Home } />
            </div>
        );
    }
}