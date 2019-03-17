import * as React from "react";
import * as ReactDOM from "react-dom";
import { AppContainer } from "react-hot-loader";
import * as ReactRouterDOM from "react-router-dom";
import { Submit } from "./Submit";

function renderApp() {
    ReactDOM.render(
        <AppContainer>
            <ReactRouterDOM.BrowserRouter>
                <ReactRouterDOM.Route path="/sets/fight/:setID" component={Submit}/>
            </ReactRouterDOM.BrowserRouter>
        </AppContainer>,
        document.getElementById("react-app")
    );
}

renderApp();