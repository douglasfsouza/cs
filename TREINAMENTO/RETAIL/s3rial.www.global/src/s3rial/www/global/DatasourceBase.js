sap.ui.define([
	"s3rial/www/global/GlobalFunctions",
	"sap/ui/model/json/JSONModel"
], function(Global, JSONModel) {
    "use strict";

    return JSONModel.extend('s3rial.www.global.DatasourceBase', {
        metadata: {
            library: 's3rial.www.global',
            publicMethods: ["get", "list", "insert", "update", "delete",
                            "readFromServer", "postToServer", "getApiName", "getUrlBase"],
        },

        _urlBase: "",
        _apiName: "",
        _tokenName: "",

        constructor: function(apiName) {
            var config = this._readAPIConfig();
            var urlBase = config[apiName];
            
            if (!urlBase)
            {
                urlBase = config.urlBase;
            }

            this._apiName = apiName;
            this._urlBase = urlBase;
            this._tokenName = config.tokenName;
        },

        getApiName: function() {
            return _apiName;
        },

        getUrlBase: function() {
            return _urlBase;
        },

        list: function (method = undefined) {
            var uri = `${this._urlBase}/${this._apiName}`;

            if (method)
            {
                uri = `${uri}/method`;
            }

            return this.readFromServer(uri);
        },
    
        query: function (criteria = undefined, method = undefined) {
            var uri = `${this._urlBase}/${this._apiName}/${method || 'query'}`;

            var request = { criterias: criteria };

            return this.postToServer(uri, "POST", request);
        },

        get: function(id) {
            var uri = `${this._urlBase}/${this._apiName}/${id}`;
            return this.readFromServer(uri);
        },

        post: function(method, payload) {
            var uri = `${this._urlBase}/${this._apiName}/${method}`;
            return this.postToServer(uri, "POST", payload);
        },

        insert: function(record) {
            var uri = `${this._urlBase}/${this._apiName}`;
            return this.postToServer(uri, "POST", record);
        },

        insertBatch: function(records) {
            var uri = `${this._urlBase}/${this._apiName}/postBatch`;
            return this.postToServer(uri, "POST", records);
        },

        update: function(id, record) {
            var uri = `${this._urlBase}/${this._apiName}/${id}`;
            return this.postToServer(uri, "PUT", record);
        },

        delete: function(id, record) {
            var uri = `${this._urlBase}/${this._apiName}/${id}`;

            if (Array.isArray(record))
            {
                uri = `${uri}/deleteBatch`;
            }

            return this.postToServer(uri, "DELETE");
        },

        deleteBatch: function(records) {
            var uri = `${this._urlBase}/${this._apiName}/deleteBatch`;
            return this.postToServer(uri, "POST", records);
        },
    
        readFromServer: async function (uri) {
            var token = await this._getToken();
            var tokenName = this._tokenName;
            
            var headers = {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            };

            headers[tokenName] = token;

            return new Promise((resolve, reject) => {
                if (!token)
                {
                    var result = {
                        success: false,
                        data: null,
                        message: "Acesso não autorizado.\r\nVerifique se o servidor está disponível"
                    };
                    reject(result);
                }
                else
                {
                    fetch(uri, {
                        method: "GET",
                        headers: headers
                    }).then(async (response) => {
                        var result = {
                            success: false,
                            data: null,
                            message: ""
                        };

                        var body = await response.json();

                        if (response.ok)
                        {
                            result.success = body.success;
                            result.data = body.data;
                        }
                        else
                        {
                            result.sucess = false;
                            result.message = `${response.status}=${response.statusText}\r\n${body.message}`;
                        }

                        resolve(result);
                    }).catch((error) => {
                        var result = {
                            success: false,
                            data: null,
                            message:`${error.status}=${error.statusText}`
                        };
                        reject(result);
                    });
                }
            })
        },        

        postToServer: async function (uri, method, body = undefined) {
            var token = await this._getToken();
            var tokenName = this._tokenName;

            var headers = {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
            
            headers[tokenName] = token;

            return new Promise((resolve, reject) => {
                fetch(uri, {
                    method: method,
                    headers: headers,
                    body: JSON.stringify(body)
                }).then(async (response) => {
                    var result = {
                        success: false,
                        data: null,
                        message: ""
                    };

                    var body = await response.json();

                    if (response.ok)
                    {
                        result.success = body.success;
                        result.data = body.data;
                    }
                    else
                    {
                        result.sucess = false;
                        result.message = `${response.status}=${response.statusText}\r\n${body.message}`;
                    }

                    resolve(result);
                }).catch((error) => {
                    var result = {
                        success: false,
                        data: null,
                        message:`${error.status}=${error.statusText}`
                    };
                    reject(result);
                });
            })
        },        

        _getToken: async function() {
            var tokenName = this._tokenName;

            var token = Global.getLocalStorage(tokenName);

            var headers = {
                'Content-Type': 'application/json',
            };

            if (!token)
            {
                // TODO: Implementar chamada da tela de logon

                var url = `${this._urlBase}/login`;
                var credentials = {
                    userName: "manager",
                    password: "acesso19",
                    database: "SBODEMOBR"
                };

                token = await fetch(url, {
                                    method: "POST",
                                    headers: headers,
                                    body: JSON.stringify(credentials)
                              }).then( 
                                  async (response) => {
                                      var json = await response.json();
                                      return json.data;
                                  },
                                  (error) => {
                                      return null;
                                  }
                              );

                if (token)
                {
                    Global.setLocalStorage(tokenName, token);
                }
            }

            return token;
        },

        Login: function(userName, password, database) {
            var headers = {
                'Content-Type': 'application/json',
            };

            var url = `${this._urlBase}/login`;
            var credentials = {
                userName: userName,
                password: password,
                database: database
            };
        
            return fetch(url, {
                                method: "POST",
                                headers: headers,
                                body: JSON.stringify(credentials)
                        }).then ( 
                            (response) => {
                                return response.json();
                            }
                        ).then (
                            (json) => {
                                return json.data;
                            }
                        );
        },

        _readAPIConfig: function () {
            var request = new XMLHttpRequest();
            var url = "/apiconfig.json";
            request.open('GET', url, false);
            request.send(null);
            return JSON.parse(request.response);
        }
    });	
});