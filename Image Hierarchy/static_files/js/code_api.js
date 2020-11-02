var tokenKey = "accessToken";
var refreshTokenKey = "refreshToken";
var __userId = 0;

class ServerAPI {
    host() {
        return location.origin;
    }

    //is authorized
    async isAuthorized() {
        return await client.getToken() != null;
    }

    async getToken() {
        let token = sessionStorage.getItem(tokenKey);
        let valid;
        if (token != null && token != "") {
            valid = (await fetch(client.host() + "/api/valid?access_token=" + encodeURI(token), {
                method: "GET",
            }).then(data => data.json()).then(json => json.valid));

            if (!valid)
                sessionStorage.removeItem(tokenKey);
        }

        return valid ? token : null;
    }

    getRefreshToken() {
        return sessionStorage.getItem(refreshTokenKey);
    }

    async verifyUser() {
        return false;
    }

    userId() { return __userId; }

    async login(login, password) { return {}; }
    async register(args) { return {}; }
    logout() {}

    refreshToken() { return ""; }

    async getDisplayName() { return ""; }
    async getEmail() { return ""; }
    async getPhoneNumber() { return ""; }

    async getImages() { return [""]; }

    async setEmail() {}
    async setDisplayName() {}
    async setPhoneNumber() {}

    async uploadImage(content, progressCallback, readyCallback, errorCallback) {}

    async changePassword() {}
};

var client = new ServerAPI();

async function apiAuth_GET(localUrl = "test") {
    let token = await client.getToken();

    function status(response) {
        if (response.ok) {
            return Promise.resolve(response);
        } else {
            return Promise.reject(new Error(response.statusText));
        }
    }

    function json(response) {
        return response.json();
    }

    if (token == null) {
        token = "";
    }

    return await fetch(client.host() + "/api/" + localUrl, {
            method: "GET",
            headers: {
                "Accept": "application/json",
                "Authorization": "Bearer " + token
            }
        })
        .then(status)
        .then(json);
}

async function apiAuth_POST(localUrl = "test", data = {}, callbackComplete = undefined, callbackError = undefined) {
    let token = await client.getToken();

    function status(response) {
        if (response.ok) {
            return Promise.resolve(response);
        } else {
            return Promise.reject(new Error(response.statusText));
        }
    }

    function json(response) {
        return response.json();
    }

    if (token == null) {
        token = "";
    }

    return await fetch(client.host() + "/api/" + localUrl, {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Authorization": "Bearer " + token,
            },
            body: JSON.stringify(data, null, null)
        })
        .then(status)
        .then(json);
}

function __GET_Sync(localUrl = "test") {
    let result;
    let completed = false;
    let exData;
    let exError;
    const promise = apiAuth_GET(localUrl, function(data) {
        completed = true;
        exData = dataReturn;
    }, function(error) {
        completed = true;
        exError = error;
    });

    return { exData, exError }
}

//server API defines

ServerAPI.prototype.login = async function(username = "", password = "") {
    return fetch(client.host() + "/api/signin", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "username": username,
                "password": password
            })
        })
        .then(response => response.json())
        .then(json => {
            sessionStorage.setItem(tokenKey, json.access_token);
            sessionStorage.setItem(refreshTokenKey, json.refresh_token);
            __userId = json.userId;
            return json;
        })
        .catch(err => {
            document.getElementById("errorMessage").innerText = err;
            console.log(err);
            return err;
        });
};

ServerAPI.prototype.register = function(args) {
    return null;

}

ServerAPI.prototype.logout = function() {
    sessionStorage.removeItem(tokenKey);
    sessionStorage.removeItem(refreshTokenKey);
    __userId = -1;
}
ServerAPI.prototype.getDisplayName = async function() {
    response = await apiAuth_GET("GetDisplayName");
    if (response !== undefined && response.result === true) {
        return response.content;
    }
    return undefined;
}

ServerAPI.prototype.getEmail = async function() {
    response = await apiAuth_GET("GetEmail");
    if (response !== undefined && response.result === true) {
        return response.content;
    }
    return undefined;
}

ServerAPI.prototype.getPhoneNumber = async function() {
    response = await apiAuth_GET("GetPhoneNumber");
    if (response !== undefined && response.result === true) {
        return response.content;
    }
    return undefined;
}

ServerAPI.prototype.verifyUser = async function() {
    return apiAuth_GET("GetVerifyUser").then(data => (data.userId == this.userId() && data.verify));
}

ServerAPI.prototype.uploadImage = async function(content,
    progressCallback = undefined,
    readyCallback = undefined,
    errorCallback = undefined) {
    // if(!verifyImageUpload())
    // {
    //     return 
    // }

    var xhtp = new XMLHttpRequest();
    xhtp.upload.onprogress = (e) => {

        progressCallback({ total: e.total, progress: e.loaded });
    };
    xhtp.onreadystatechange = (e) => {
        if (e.readState == 4 && errorCallback != undefined) {
            readyCallback();
        }
    };
    xhtp.open("POST", "/api/Upload");
    xhtp.send(content);
}

ServerAPI.prototype.getImages = async function() {
    return await apiAuth_GET("GetUserImages");
}