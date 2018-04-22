﻿/* tslint:disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v11.17.2.0 (NJsonSchema v9.10.45.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

export module ClimbClient {
export class BaseClass {
    getAuthorizationToken() {
        return localStorage.getItem("jwt");
    }
}

export class AccountClient extends BaseClass {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super();
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl ? baseUrl : "http://localhost:52912";
    }

    register(email: string | null, password: string | null, confirmPassword: string | null | undefined): Promise<ApplicationUser | null> {
        let url_ = this.baseUrl + "/api/v1/account/register?";
        if (email === undefined)
            throw new Error("The parameter 'email' must be defined.");
        else
            url_ += "email=" + encodeURIComponent("" + email) + "&"; 
        if (password === undefined)
            throw new Error("The parameter 'password' must be defined.");
        else
            url_ += "password=" + encodeURIComponent("" + password) + "&"; 
        if (confirmPassword !== undefined)
            url_ += "confirmPassword=" + encodeURIComponent("" + confirmPassword) + "&"; 
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "POST",
            headers: {
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processRegister(_response);
        });
    }

    protected processRegister(response: Response): Promise<ApplicationUser | null> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 400) {
            return response.text().then((_responseText) => {
            return throwException("A server error occurred.", status, _responseText, _headers);
            });
        } else if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? ApplicationUser.fromJS(resultData200) : <any>null;
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ApplicationUser | null>(<any>null);
    }

    logIn(email: string | null, password: string | null): Promise<LoginResponse> {
        let url_ = this.baseUrl + "/api/v1/account/logIn?";
        if (email === undefined)
            throw new Error("The parameter 'email' must be defined.");
        else
            url_ += "email=" + encodeURIComponent("" + email) + "&"; 
        if (password === undefined)
            throw new Error("The parameter 'password' must be defined.");
        else
            url_ += "password=" + encodeURIComponent("" + password) + "&"; 
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "POST",
            headers: {
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processLogIn(_response);
        });
    }

    protected processLogIn(response: Response): Promise<LoginResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 400) {
            return response.text().then((_responseText) => {
            return throwException("A server error occurred.", status, _responseText, _headers);
            });
        } else if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? LoginResponse.fromJS(resultData200) : new LoginResponse();
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<LoginResponse>(<any>null);
    }

    test(authorization: string | null | undefined, userID: string | null): Promise<string | null> {
        let url_ = this.baseUrl + "/api/v1/account/test?";
        if (userID === undefined)
            throw new Error("The parameter 'userID' must be defined.");
        else
            url_ += "userID=" + encodeURIComponent("" + userID) + "&"; 
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            headers: {
                "Authorization": authorization !== undefined && authorization !== null ? "" + authorization : "", 
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processTest(_response);
        });
    }

    protected processTest(response: Response): Promise<string | null> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 !== undefined ? resultData200 : <any>null;
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<string | null>(<any>null);
    }

    logout(authorization: string | null | undefined): Promise<string | null> {
        let url_ = this.baseUrl + "/api/v1/account/logOut";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "POST",
            headers: {
                "Authorization": authorization !== undefined && authorization !== null ? "" + authorization : "", 
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processLogout(_response);
        });
    }

    protected processLogout(response: Response): Promise<string | null> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 !== undefined ? resultData200 : <any>null;
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<string | null>(<any>null);
    }
}

export class GameClient extends BaseClass {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super();
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl ? baseUrl : "http://localhost:52912";
    }

    create(name: string | null | undefined): Promise<Game> {
        let url_ = this.baseUrl + "/api/v1/games/create?";
        if (name !== undefined)
            url_ += "name=" + encodeURIComponent("" + name) + "&"; 
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "POST",
            headers: {
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processCreate(_response);
        });
    }

    protected processCreate(response: Response): Promise<Game> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 201) {
            return response.text().then((_responseText) => {
            let result201: any = null;
            let resultData201 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result201 = resultData201 ? Game.fromJS(resultData201) : new Game();
            return result201;
            });
        } else if (status === 400) {
            return response.text().then((_responseText) => {
            let result400: any = null;
            let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result400 = resultData400 !== undefined ? resultData400 : <any>null;
            return throwException("A server error occurred.", status, _responseText, _headers, result400);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<Game>(<any>null);
    }

    listAll(): Promise<Game[]> {
        let url_ = this.baseUrl + "/api/v1/games";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            headers: {
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processListAll(_response);
        });
    }

    protected processListAll(response: Response): Promise<Game[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (resultData200 && resultData200.constructor === Array) {
                result200 = [];
                for (let item of resultData200)
                    result200.push(Game.fromJS(item));
            }
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<Game[]>(<any>null);
    }
}

export class UserClient extends BaseClass {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super();
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl ? baseUrl : "http://localhost:52912";
    }

    index(): Promise<FileResponse | null> {
        let url_ = this.baseUrl + "/user";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "POST",
            headers: {
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processIndex(_response);
        });
    }

    protected processIndex(response: Response): Promise<FileResponse | null> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
            const fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
            const fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            return response.blob().then(blob => { return { fileName: fileName, data: blob, status: status, headers: _headers }; });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<FileResponse | null>(<any>null);
    }
}

export class SampleDataClient extends BaseClass {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super();
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl ? baseUrl : "http://localhost:52912";
    }

    weatherForecasts(): Promise<WeatherForecast[] | null> {
        let url_ = this.baseUrl + "/api/v1/SampleData/WeatherForecasts";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "POST",
            headers: {
                "Content-Type": "application/json", 
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processWeatherForecasts(_response);
        });
    }

    protected processWeatherForecasts(response: Response): Promise<WeatherForecast[] | null> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (resultData200 && resultData200.constructor === Array) {
                result200 = [];
                for (let item of resultData200)
                    result200.push(WeatherForecast.fromJS(item));
            }
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<WeatherForecast[] | null>(<any>null);
    }
}

export class IdentityUserOfString implements IIdentityUserOfString {
    id?: string | undefined;
    userName?: string | undefined;
    normalizedUserName?: string | undefined;
    email?: string | undefined;
    normalizedEmail?: string | undefined;
    emailConfirmed: boolean;
    passwordHash?: string | undefined;
    securityStamp?: string | undefined;
    concurrencyStamp?: string | undefined;
    phoneNumber?: string | undefined;
    phoneNumberConfirmed: boolean;
    twoFactorEnabled: boolean;
    lockoutEnd?: Date | undefined;
    lockoutEnabled: boolean;
    accessFailedCount: number;

    constructor(data?: IIdentityUserOfString) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["id"];
            this.userName = data["userName"];
            this.normalizedUserName = data["normalizedUserName"];
            this.email = data["email"];
            this.normalizedEmail = data["normalizedEmail"];
            this.emailConfirmed = data["emailConfirmed"];
            this.passwordHash = data["passwordHash"];
            this.securityStamp = data["securityStamp"];
            this.concurrencyStamp = data["concurrencyStamp"];
            this.phoneNumber = data["phoneNumber"];
            this.phoneNumberConfirmed = data["phoneNumberConfirmed"];
            this.twoFactorEnabled = data["twoFactorEnabled"];
            this.lockoutEnd = data["lockoutEnd"] ? new Date(data["lockoutEnd"].toString()) : <any>undefined;
            this.lockoutEnabled = data["lockoutEnabled"];
            this.accessFailedCount = data["accessFailedCount"];
        }
    }

    static fromJS(data: any): IdentityUserOfString {
        data = typeof data === 'object' ? data : {};
        let result = new IdentityUserOfString();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["userName"] = this.userName;
        data["normalizedUserName"] = this.normalizedUserName;
        data["email"] = this.email;
        data["normalizedEmail"] = this.normalizedEmail;
        data["emailConfirmed"] = this.emailConfirmed;
        data["passwordHash"] = this.passwordHash;
        data["securityStamp"] = this.securityStamp;
        data["concurrencyStamp"] = this.concurrencyStamp;
        data["phoneNumber"] = this.phoneNumber;
        data["phoneNumberConfirmed"] = this.phoneNumberConfirmed;
        data["twoFactorEnabled"] = this.twoFactorEnabled;
        data["lockoutEnd"] = this.lockoutEnd ? this.lockoutEnd.toISOString() : <any>undefined;
        data["lockoutEnabled"] = this.lockoutEnabled;
        data["accessFailedCount"] = this.accessFailedCount;
        return data; 
    }
}

export interface IIdentityUserOfString {
    id?: string | undefined;
    userName?: string | undefined;
    normalizedUserName?: string | undefined;
    email?: string | undefined;
    normalizedEmail?: string | undefined;
    emailConfirmed: boolean;
    passwordHash?: string | undefined;
    securityStamp?: string | undefined;
    concurrencyStamp?: string | undefined;
    phoneNumber?: string | undefined;
    phoneNumberConfirmed: boolean;
    twoFactorEnabled: boolean;
    lockoutEnd?: Date | undefined;
    lockoutEnabled: boolean;
    accessFailedCount: number;
}

export class IdentityUser extends IdentityUserOfString implements IIdentityUser {

    constructor(data?: IIdentityUser) {
        super(data);
    }

    init(data?: any) {
        super.init(data);
        if (data) {
        }
    }

    static fromJS(data: any): IdentityUser {
        data = typeof data === 'object' ? data : {};
        let result = new IdentityUser();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        super.toJSON(data);
        return data; 
    }
}

export interface IIdentityUser extends IIdentityUserOfString {
}

export class ApplicationUser extends IdentityUser implements IApplicationUser {

    constructor(data?: IApplicationUser) {
        super(data);
    }

    init(data?: any) {
        super.init(data);
        if (data) {
        }
    }

    static fromJS(data: any): ApplicationUser {
        data = typeof data === 'object' ? data : {};
        let result = new ApplicationUser();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        super.toJSON(data);
        return data; 
    }
}

export interface IApplicationUser extends IIdentityUser {
}

export class LoginResponse implements ILoginResponse {
    token?: string | undefined;

    constructor(data?: ILoginResponse) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.token = data["token"];
        }
    }

    static fromJS(data: any): LoginResponse {
        data = typeof data === 'object' ? data : {};
        let result = new LoginResponse();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["token"] = this.token;
        return data; 
    }
}

export interface ILoginResponse {
    token?: string | undefined;
}

export class Game implements IGame {
    id: number;
    name?: string | undefined;
    characters?: Character[] | undefined;
    stages?: Stage[] | undefined;

    constructor(data?: IGame) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["id"];
            this.name = data["name"];
            if (data["characters"] && data["characters"].constructor === Array) {
                this.characters = [];
                for (let item of data["characters"])
                    this.characters.push(Character.fromJS(item));
            }
            if (data["stages"] && data["stages"].constructor === Array) {
                this.stages = [];
                for (let item of data["stages"])
                    this.stages.push(Stage.fromJS(item));
            }
        }
    }

    static fromJS(data: any): Game {
        data = typeof data === 'object' ? data : {};
        let result = new Game();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["name"] = this.name;
        if (this.characters && this.characters.constructor === Array) {
            data["characters"] = [];
            for (let item of this.characters)
                data["characters"].push(item.toJSON());
        }
        if (this.stages && this.stages.constructor === Array) {
            data["stages"] = [];
            for (let item of this.stages)
                data["stages"].push(item.toJSON());
        }
        return data; 
    }
}

export interface IGame {
    id: number;
    name?: string | undefined;
    characters?: Character[] | undefined;
    stages?: Stage[] | undefined;
}

export class Character implements ICharacter {
    id: number;
    name?: string | undefined;

    constructor(data?: ICharacter) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["id"];
            this.name = data["name"];
        }
    }

    static fromJS(data: any): Character {
        data = typeof data === 'object' ? data : {};
        let result = new Character();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["name"] = this.name;
        return data; 
    }
}

export interface ICharacter {
    id: number;
    name?: string | undefined;
}

export class Stage implements IStage {
    id: number;
    name?: string | undefined;

    constructor(data?: IStage) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["id"];
            this.name = data["name"];
        }
    }

    static fromJS(data: any): Stage {
        data = typeof data === 'object' ? data : {};
        let result = new Stage();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["name"] = this.name;
        return data; 
    }
}

export interface IStage {
    id: number;
    name?: string | undefined;
}

export class WeatherForecast implements IWeatherForecast {
    dateFormatted?: string | undefined;
    temperatureC: number;
    summary?: string | undefined;
    temperatureF: number;

    constructor(data?: IWeatherForecast) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.dateFormatted = data["dateFormatted"];
            this.temperatureC = data["temperatureC"];
            this.summary = data["summary"];
            this.temperatureF = data["temperatureF"];
        }
    }

    static fromJS(data: any): WeatherForecast {
        data = typeof data === 'object' ? data : {};
        let result = new WeatherForecast();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["dateFormatted"] = this.dateFormatted;
        data["temperatureC"] = this.temperatureC;
        data["summary"] = this.summary;
        data["temperatureF"] = this.temperatureF;
        return data; 
    }
}

export interface IWeatherForecast {
    dateFormatted?: string | undefined;
    temperatureC: number;
    summary?: string | undefined;
    temperatureF: number;
}

export interface FileResponse {
    data: Blob;
    status: number;
    fileName?: string;
    headers?: { [name: string]: any };
}

export class SwaggerException extends Error {
    message: string;
    status: number; 
    response: string; 
    headers: { [key: string]: any; };
    result: any; 

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isSwaggerException = true;

    static isSwaggerException(obj: any): obj is SwaggerException {
        return obj.isSwaggerException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if(result !== null && result !== undefined)
        throw result;
    else
        throw new SwaggerException(message, status, response, headers, null);
}

}