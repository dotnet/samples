// Name:        MicrosoftAjaxAdoNet.debug.js
// Assembly:    System.Web.Extensions
// Version:     4.0.0.0
// FileVersion: 4.0.20227.0
//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// MicrosoftAjaxAdoNet.js
// Microsoft ADO.NET Data Service AJAX Framework.
/// <reference name="MicrosoftAjaxWebServices.js" />
Type._registerScript("MicrosoftAjaxAdoNet.js", ["MicrosoftAjaxWebServices.js"]);
Type.registerNamespace("Sys.Data");
if (!Sys.Data.IDataProvider) {
Sys.Data.IDataProvider = function Sys$Data$IDataProvider() {
    throw Error.notImplemented();
}
    function Sys$Data$IDataProvider$fetchData(operation, parameters, mergeOption, httpVerb, succeededCallback, failedCallback, timeout, userContext) {
        /// <summary locid="M:J#Sys.Data.IDataProvider.fetchData" />
        /// <param name="operation"></param>
        /// <param name="parameters" type="Object" mayBeNull="true" optional="true"></param>
        /// <param name="mergeOption" type="Sys.Data.MergeOption" mayBeNull="true" optional="true"></param>
        /// <param name="httpVerb" type="String" mayBeNull="true" optional="true"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="timeout" type="Number" integer="true" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "operation"},
            {name: "parameters", type: Object, mayBeNull: true, optional: true},
            {name: "mergeOption", type: Sys.Data.MergeOption, mayBeNull: true, optional: true},
            {name: "httpVerb", type: String, mayBeNull: true, optional: true},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "timeout", type: Number, mayBeNull: true, integer: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        throw Error.notImplemented();
    }
Sys.Data.IDataProvider.prototype = {
    fetchData: Sys$Data$IDataProvider$fetchData
}
Sys.Data.IDataProvider.registerInterface("Sys.Data.IDataProvider");
}
if (!Sys.Data.MergeOption) {
Sys.Data.MergeOption = function Sys$Data$MergeOption() {
    /// <summary locid="M:J#Sys.Data.MergeOption.#ctor" />
    /// <field name="appendOnly" type="Number" integer="true" static="true" locid="F:J#Sys.Data.MergeOption.appendOnly"></field>
    /// <field name="overwriteChanges" type="Number" integer="true" static="true" locid="F:J#Sys.Data.MergeOption.overwriteChanges"></field>
    if (arguments.length !== 0) throw Error.parameterCount();
    throw Error.notImplemented();
}
Sys.Data.MergeOption.prototype = {
    appendOnly: 0,
    overwriteChanges: 1
}
Sys.Data.MergeOption.registerEnum("Sys.Data.MergeOption");
}
Sys.Data.AdoNetQueryBuilder = function Sys$Data$AdoNetQueryBuilder(uri) {
    /// <summary locid="M:J#Sys.Data.AdoNetQueryBuilder.#ctor" />
    /// <param name="uri" type="String"></param>
    var e = Function._validateParams(arguments, [
        {name: "uri", type: String}
    ]);
    if (e) throw e;
    this._queryParameters = {};
    this._uri = uri;
    var idxQuery = uri.indexOf('?');
    if (idxQuery >= 0) {
        this._uri = uri.substr(0, idxQuery);
        var params = uri.substr(idxQuery + 1).split('&');
        for (var i in params) {
            param = params[i];
            var idxValue = param.indexOf('=');
            if (idxValue >= 0) {
                this._queryParameters[decodeURIComponent(param.substr(0, idxValue))] = decodeURIComponent(param.substr(idxValue + 1));
            }
            else {
                this._queryParameters[decodeURIComponent(param)] = "";
            }
        }
    }
}
    function Sys$Data$AdoNetQueryBuilder$get_skip() {
        /// <value type="Number" integer="true" mayBeNull="true" locid="P:J#Sys.Data.AdoNetQueryBuilder.skip"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._getIntParam("$skip");
    }
    function Sys$Data$AdoNetQueryBuilder$set_skip(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Number, mayBeNull: true, integer: true}]);
        if (e) throw e;
        this._setParam("$skip", value);
    }
    function Sys$Data$AdoNetQueryBuilder$get_top() {
        /// <value type="Number" integer="true" mayBeNull="true" locid="P:J#Sys.Data.AdoNetQueryBuilder.top"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._getIntParam("$top");
    }
    function Sys$Data$AdoNetQueryBuilder$set_top(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Number, mayBeNull: true, integer: true}]);
        if (e) throw e;
        this._setParam("$top", value);
    }
    function Sys$Data$AdoNetQueryBuilder$get_orderby() {
        /// <value type="String" mayBeNull="true" locid="P:J#Sys.Data.AdoNetQueryBuilder.orderby"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._getStringParam("$orderby");
    }
    function Sys$Data$AdoNetQueryBuilder$set_orderby(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String, mayBeNull: true}]);
        if (e) throw e;
        this._setParam("$orderby", value);
    }
    function Sys$Data$AdoNetQueryBuilder$get_filter() {
        /// <value type="String" mayBeNull="true" locid="P:J#Sys.Data.AdoNetQueryBuilder.filter"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._getStringParam("$filter");
    }
    function Sys$Data$AdoNetQueryBuilder$set_filter(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String, mayBeNull: true}]);
        if (e) throw e;
        this._setParam("$filter", value);
    }
    function Sys$Data$AdoNetQueryBuilder$get_expand() {
        /// <value type="String" mayBeNull="true" locid="P:J#Sys.Data.AdoNetQueryBuilder.expand"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._getStringParam("$expand");
    }
    function Sys$Data$AdoNetQueryBuilder$set_expand(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String, mayBeNull: true}]);
        if (e) throw e;
        this._setParam("$expand", value);
    }
    function Sys$Data$AdoNetQueryBuilder$get_resourcePath() {
        /// <value type="string" locid="P:J#Sys.Data.AdoNetQueryBuilder.resourcePath"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._uri;
    }
    function Sys$Data$AdoNetQueryBuilder$get_queryParameters() {
        /// <value type="Object" locid="P:J#Sys.Data.AdoNetQueryBuilder.queryParameters"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._queryParameters;
    }
    function Sys$Data$AdoNetQueryBuilder$set_queryParameters(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Object}]);
        if (e) throw e;
        this._queryParameters = value;
    }
    function Sys$Data$AdoNetQueryBuilder$toString() {
        /// <summary locid="M:J#Sys.Data.AdoNetQueryBuilder.toString" />
        /// <returns type="string"></returns>
        if (arguments.length !== 0) throw Error.parameterCount();
        var key, i, value, params = new Array();
        for (key in this._queryParameters) {
            if (!Array.contains(Sys.Data.AdoNetQueryBuilder._queryOptions, key)) {
                value = this._queryParameters[key];
                if (value != null) {
                    Array.add(params, { key: key, value: value });
                }
            }
        }
        for (i in Sys.Data.AdoNetQueryBuilder._queryOptions) {
            key = Sys.Data.AdoNetQueryBuilder._queryOptions[i];
            value = this._queryParameters[key];
            if (value != null) {
                Array.add(params, { key: key, value: value });
            }
        }
        var sb = new Sys.StringBuilder(this._uri);
        var firstElement = true;
        for (i in params) {
            sb.append((firstElement) ? '?' : '&');
            sb.append(encodeURIComponent(params[i].key));
            sb.append('=');
            sb.append(encodeURIComponent(params[i].value));
            firstElement = false;
        }
        return sb.toString();
    }
    function Sys$Data$AdoNetQueryBuilder$_getIntParam(name) {
        var value = parseInt(this._queryParameters[name]);
        return (isNaN(value)) ? null : value;
    }
    function Sys$Data$AdoNetQueryBuilder$_getStringParam(name) {
        var value = this._queryParameters[name];
        return value || null;
    }
    function Sys$Data$AdoNetQueryBuilder$_setParam(name, value) {
        if (typeof(value) === "undefined" || (value === null)) {
            delete this._queryParameters[name];
        }
        else {
            this._queryParameters[name] = value;
        }
    }
Sys.Data.AdoNetQueryBuilder.prototype = {
    _queryParameters: null,
    _uri: null,
    get_skip: Sys$Data$AdoNetQueryBuilder$get_skip,
    set_skip: Sys$Data$AdoNetQueryBuilder$set_skip,
    get_top: Sys$Data$AdoNetQueryBuilder$get_top,
    set_top: Sys$Data$AdoNetQueryBuilder$set_top,
    get_orderby: Sys$Data$AdoNetQueryBuilder$get_orderby,
    set_orderby: Sys$Data$AdoNetQueryBuilder$set_orderby,
    get_filter: Sys$Data$AdoNetQueryBuilder$get_filter,
    set_filter: Sys$Data$AdoNetQueryBuilder$set_filter,
    get_expand: Sys$Data$AdoNetQueryBuilder$get_expand,
    set_expand: Sys$Data$AdoNetQueryBuilder$set_expand,
    get_resourcePath: Sys$Data$AdoNetQueryBuilder$get_resourcePath,
    
    get_queryParameters: Sys$Data$AdoNetQueryBuilder$get_queryParameters,
    set_queryParameters: Sys$Data$AdoNetQueryBuilder$set_queryParameters,
    toString: Sys$Data$AdoNetQueryBuilder$toString,
    _getIntParam: Sys$Data$AdoNetQueryBuilder$_getIntParam,
    _getStringParam: Sys$Data$AdoNetQueryBuilder$_getStringParam,
    _setParam: Sys$Data$AdoNetQueryBuilder$_setParam
}
Sys.Data.AdoNetQueryBuilder._queryOptions = ["$filter", "$orderby", "$skip", "$top"];
Sys.Data.AdoNetQueryBuilder.registerClass("Sys.Data.AdoNetQueryBuilder");
Sys.Data._AdoNetUtil = function Sys$Data$_AdoNetUtil() {
    throw Error.invalidOperation();
}
Sys.Data._AdoNetUtil.concatUris = function Sys$Data$_AdoNetUtil$concatUris(serviceUri, resourceUri) {
    if (resourceUri.indexOf("//") >= 0) {
        return resourceUri;
    }
    
    if (serviceUri.endsWith('/')) {
        serviceUri = serviceUri.substr(0, serviceUri.length - 1);
    }
    if (resourceUri.startsWith('/')) {
        resourceUri = resourceUri.substr(1);
    }
    return serviceUri + '/' + resourceUri;
}
Sys.Data._AdoNetUtil.extractETag = function Sys$Data$_AdoNetUtil$extractETag(item) {
    return (item.__metadata) ? (item.__metadata.etag || null) : null;
}
Sys.Data._AdoNetUtil.extractUri = function Sys$Data$_AdoNetUtil$extractUri(item) {
    return (item.__metadata) ? (item.__metadata.uri || null) : null;
}
Sys.Data._AdoNetUtil.registerClass("Sys.Data._AdoNetUtil");    
Sys.Data.AdoNetActionResult = function Sys$Data$AdoNetActionResult(result, httpHeaders, actionContext, operation) {
    /// <summary locid="M:J#Sys.Data.AdoNetActionResult.#ctor" />
    /// <param name="result" mayBeNull="true"></param>
    /// <param name="httpHeaders" mayBeNull="true"></param>
    /// <param name="actionContext" mayBeNull="true"></param>
    /// <param name="operation" type="String"></param>
    var e = Function._validateParams(arguments, [
        {name: "result", mayBeNull: true},
        {name: "httpHeaders", mayBeNull: true},
        {name: "actionContext", mayBeNull: true},
        {name: "operation", type: String}
    ]);
    if (e) throw e;
    this._result = result;
    this._headers = httpHeaders || {};
    this._actionContext = actionContext;
    this._operation = operation;
}
    function Sys$Data$AdoNetActionResult$get_httpHeaders() {
        /// <value type="Object" locid="P:J#Sys.Data.AdoNetActionResult.httpHeaders"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._headers;
    }
    function Sys$Data$AdoNetActionResult$get_actionContext() {
        /// <value mayBeNull="true" locid="P:J#Sys.Data.AdoNetActionResult.actionContext"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._actionContext;
    }
    function Sys$Data$AdoNetActionResult$get_operation() {
        /// <value type="String" locid="P:J#Sys.Data.AdoNetActionResult.operation"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._operation;
    }
    function Sys$Data$AdoNetActionResult$get_result() {
        /// <value type="Object" mayBeNull="true" locid="P:J#Sys.Data.AdoNetActionResult.result"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._result;
    }
Sys.Data.AdoNetActionResult.prototype = {
    _actionContext: null,
    _operation: null,
    _result: null,
    _headers: null,
    get_httpHeaders: Sys$Data$AdoNetActionResult$get_httpHeaders,
    get_actionContext: Sys$Data$AdoNetActionResult$get_actionContext,
    get_operation: Sys$Data$AdoNetActionResult$get_operation,
    get_result: Sys$Data$AdoNetActionResult$get_result
}
Sys.Data.AdoNetActionResult.registerClass("Sys.Data.AdoNetActionResult");
Sys.Data.AdoNetActionSequence = function Sys$Data$AdoNetActionSequence(dataService) {
    /// <summary locid="M:J#Sys.Data.AdoNetActionSequence.#ctor" />
    /// <param name="dataService" type="Sys.Data.AdoNetServiceProxy"></param>
    var e = Function._validateParams(arguments, [
        {name: "dataService", type: Sys.Data.AdoNetServiceProxy}
    ]);
    if (e) throw e;
    this._actionQueue = [];
    this._dataService = dataService;
}
    function Sys$Data$AdoNetActionSequence$get_serviceProxy() {
        /// <value type="Sys.Data.AdoNetServiceProxy" locid="P:J#Sys.Data.AdoNetActionSequence.serviceProxy"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._dataService;
    }
    function Sys$Data$AdoNetActionSequence$addInsertAction(item, resourceSetUri, actionContext) {
        /// <summary locid="M:J#Sys.Data.AdoNetActionSequence.addInsertAction" />
        /// <param name="item" type="Object"></param>
        /// <param name="resourceSetUri" type="String" mayBeNull="true"></param>
        /// <param name="actionContext" mayBeNull="true" optional="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "item", type: Object},
            {name: "resourceSetUri", type: String, mayBeNull: true},
            {name: "actionContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var queue = this._actionQueue;
        queue[queue.length] = [0, resourceSetUri, item, actionContext];
    }
    function Sys$Data$AdoNetActionSequence$addUpdateAction(item, updateItemUri, actionContext) {
        /// <summary locid="M:J#Sys.Data.AdoNetActionSequence.addUpdateAction" />
        /// <param name="item" type="Object"></param>
        /// <param name="updateItemUri" type="String" optional="true" mayBeNull="true"></param>
        /// <param name="actionContext" mayBeNull="true" optional="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "item", type: Object},
            {name: "updateItemUri", type: String, mayBeNull: true, optional: true},
            {name: "actionContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var queue = this._actionQueue;
        queue[queue.length] = [1, updateItemUri || null, item, actionContext];
    }
    function Sys$Data$AdoNetActionSequence$addRemoveAction(item, actionContext) {
        /// <summary locid="M:J#Sys.Data.AdoNetActionSequence.addRemoveAction" />
        /// <param name="item" type="Object" mayBeNull="true"></param>
        /// <param name="actionContext" mayBeNull="true" optional="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "item", type: Object, mayBeNull: true},
            {name: "actionContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var queue = this._actionQueue;
        queue[queue.length] = [2, null, item, actionContext];
    }
    function Sys$Data$AdoNetActionSequence$clearActions() {
        /// <summary locid="M:J#Sys.Data.AdoNetActionSequence.clearActions" />
        if (arguments.length !== 0) throw Error.parameterCount();
        this._actionQueue = [];
    }
    function Sys$Data$AdoNetActionSequence$execute(succeededCallback, failedCallback, userContext) {
        /// <summary locid="M:J#Sys.Data.AdoNetActionSequence.execute" />
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var queue = this._actionQueue,
            bw = new Sys.Data._AdoNetBatchWriter(window.location.host),
            ds = this._dataService;
        this._actionQueue = [];
        
        bw.startChangeSet();
        for (var i = 0, l = queue.length; i < l; i++) {
            var action = queue[i],
                uri = action[1],
                item = action[2],
                eTag = Sys.Data._AdoNetUtil.extractETag(item);
            switch (action[0]) {
                case 0:
                    action[0] = "insert";
                    bw.addChange(uri, eTag, "POST", Sys.Serialization.JavaScriptSerializer.serialize(item), i);
                    break;
                case 1:
                    action[0] = "edit";
                    if (!uri) {
                        uri = Sys.Data._AdoNetUtil.extractUri(item);
                    }
                    bw.addChange(uri, eTag, ds.get_replaceOnUpdate() ? "PUT" : "MERGE", Sys.Serialization.JavaScriptSerializer.serialize(item));
                    break;
                case 2:
                    action[0] = "remove";
                    uri = Sys.Data._AdoNetUtil.extractUri(item);
                    bw.addChange(uri, eTag, "DELETE", null);
                    break;
            }
        }
        bw.endChangeSet();
        var r = new Sys.Net.WebRequest();
        r.set_url(Sys.Data._AdoNetUtil.concatUris(ds.get_serviceUri(), "$batch"));
        r.get_headers()["Content-Type"] = "multipart/mixed; boundary=" + bw.get_topBoundary();
        r.set_httpVerb("POST");
        r.set_timeout(ds.get_timeout());
        r.set_body(bw.get_requestBody());
        r.set_userContext({ q: queue, bw: bw, c: userContext, s: succeededCallback, f: failedCallback });
        r.add_completed(Function.createDelegate(this, this._batchCompleted));
        r.invoke();
        return r;
    }
    function Sys$Data$AdoNetActionSequence$_batchCompleted(executor) {
        var data, responses, response,
            ctx = executor.get_webRequest().get_userContext(),
            queue = ctx.q,
            onFailed = ctx.f,
            onSuccess = ctx.s,
            userContext = ctx.c,
            bw = ctx.bw,
            dsError = this._dataService._checkForError(executor, "actionSequence", false);
            
        function checkPartForError() {
            var status = response.status ? parseFloat(response.status.code) : -1;
            if (status < 200 || status > 300) {
                var message;
                if (response.headers["Content-Type"] === "application/json") {
                    var errorObj = Sys.Serialization.JavaScriptSerializer.deserialize(response.body);
                    dsError = Sys.Data.AdoNetServiceError._getError(false, status, null, errorObj, "actionSequence");
                }
                else {
                    dsError = Sys.Data.AdoNetServiceError._getError(false, status, String.format(Sys.Data.AdoNetRes.operationFailed, "actionSequence"));
                }
            }
        }
        function doError() {
            if (onFailed) {
                onFailed(dsError, userContext, "actionSequence");
            }
            else {
                throw Sys.Data.AdoNetServiceProxy._createFailedError("actionSequence", dsError.get_message());
            }
        }
        
        if (dsError) {
            doError();
            return;
        }
        
        responses = Sys.Data._AdoNetBatchReader._parseResponse(executor);
        if (responses.length !== 1) {
            dsError = Sys.Data.AdoNetServiceError._getError(false, -1,
                String.format(Sys.Data.AdoNetRes.invalidBatchResponse, executor.get_webRequest().get_url()));
            doError();
            return;
        }
        responses = responses[0];
        if (responses.length === 1) {
            response = responses[0];
            checkPartForError();
            if (dsError) {
                doError();
                return;
            }
        }
        if (responses.length !== queue.length) {
            dsError = Sys.Data.AdoNetServiceError._getError(false, -1, 
                String.format(Sys.Data.AdoNetRes.invalidBatchResponse, executor.get_webRequest().get_url()));
            doError();
            return;
        }
        
        
        if (onSuccess) {
            var l = responses.length,
                results = new Array(l);
            for (var i = 0; i < l; i++) {
                response = responses[i], body = response.body;
                data = null;
                if (body) {
                    data = Sys.Serialization.JavaScriptSerializer.deserialize(body);
                    if (data && data.d) {
                        data = data.d;
                    }
                }
                var queueEntry = queue[i],
                    actionContext = queueEntry[3],
                    operation = queueEntry[0];
                if (typeof(actionContext) === "undefined") {
                    actionContext = null;
                }
                results[i] = new Sys.Data.AdoNetActionResult(data, response.headers, actionContext, operation);
            }
            onSuccess(results, userContext, "actionSequence");
        }
    }
Sys.Data.AdoNetActionSequence.prototype = {
    get_serviceProxy: Sys$Data$AdoNetActionSequence$get_serviceProxy,
    addInsertAction: Sys$Data$AdoNetActionSequence$addInsertAction,
    addUpdateAction: Sys$Data$AdoNetActionSequence$addUpdateAction,
    addRemoveAction: Sys$Data$AdoNetActionSequence$addRemoveAction,
    clearActions: Sys$Data$AdoNetActionSequence$clearActions,
    execute: Sys$Data$AdoNetActionSequence$execute,
    _batchCompleted: Sys$Data$AdoNetActionSequence$_batchCompleted
}
Sys.Data.AdoNetActionSequence.registerClass("Sys.Data.AdoNetActionSequence");
Sys.Data.AdoNetInvokeParametersBuilder = function Sys$Data$AdoNetInvokeParametersBuilder() {
    /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.#ctor" />
    if (arguments.length !== 0) throw Error.parameterCount();
    
    this._queryBuilder = new Sys.Data.AdoNetQueryBuilder("");
    this._parameters = this._queryBuilder.get_queryParameters();
}
    function Sys$Data$AdoNetInvokeParametersBuilder$get_parameters() {
        /// <value locid="P:J#Sys.Data.AdoNetInvokeParametersBuilder.parameters"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._parameters;
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$addBoolean(name, value) {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.addBoolean" />
        /// <param name="name" type="String"></param>
        /// <param name="value" type="Boolean"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", type: Boolean}
        ]);
        if (e) throw e;
        this._parameters[name] = value.toString();
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$addDate(name, value, includeTimeZone) {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.addDate" />
        /// <param name="name" type="String"></param>
        /// <param name="value" type="Date"></param>
        /// <param name="includeTimeZone" type="Boolean" optional="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", type: Date},
            {name: "includeTimeZone", type: Boolean, optional: true}
        ]);
        if (e) throw e;
        var converted = (includeTimeZone)
            ? value.format("yyyy-MM-ddTHH:mm:ss.fffffffzzz")
            : value.format("yyyy-MM-ddTHH:mm:ss.fffffff");
            
        this._parameters[name] = "datetime'" + converted + "'";
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$addDecimal(name, value) {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.addDecimal" />
        /// <param name="name" type="String"></param>
        /// <param name="value" type="Number"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", type: Number}
        ]);
        if (e) throw e;
        this._parameters[name] = value.toString() + "M";
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$addDouble(name, value) {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.addDouble" />
        /// <param name="name" type="String"></param>
        /// <param name="value" type="Number"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", type: Number}
        ]);
        if (e) throw e;
        this._parameters[name] = value.toString();
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$addGuid(name, value) {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.addGuid" />
        /// <param name="name" type="String"></param>
        /// <param name="value" type="String"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", type: String}
        ]);
        if (e) throw e;
        var matchFound = value.match(Sys.Data.AdoNetInvokeParametersBuilder._guidRegex);
        if (!matchFound) {
            var er = Error.create(Sys.Data.AdoNetRes.invalidGuid, { name: "Sys.Data.AdoNetException" });
            throw er;
        }
        
        this._parameters[name] = "guid'" + value + "'";
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$addInteger(name, value) {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.addInteger" />
        /// <param name="name" type="String"></param>
        /// <param name="value" type="Number" integer="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", type: Number, integer: true}
        ]);
        if (e) throw e;
        this._parameters[name] = value.toString();
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$addString(name, value) {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.addString" />
        /// <param name="name" type="String"></param>
        /// <param name="value" type="String"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", type: String}
        ]);
        if (e) throw e;
        this._parameters[name] = "'" + value.replace(new RegExp("'", "g"), "''") + "'";
    }
    function Sys$Data$AdoNetInvokeParametersBuilder$toString() {
        /// <summary locid="M:J#Sys.Data.AdoNetInvokeParametersBuilder.toString" />
        /// <returns type="String"></returns>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._queryBuilder.toString();
    }
Sys.Data.AdoNetInvokeParametersBuilder.prototype = {
    //
    //
    _parameters: null,
    _queryBuilder: null,
    //
    //
    get_parameters: Sys$Data$AdoNetInvokeParametersBuilder$get_parameters,
    //
    //
    addBoolean: Sys$Data$AdoNetInvokeParametersBuilder$addBoolean,
    addDate: Sys$Data$AdoNetInvokeParametersBuilder$addDate,
    addDecimal: Sys$Data$AdoNetInvokeParametersBuilder$addDecimal,
    addDouble: Sys$Data$AdoNetInvokeParametersBuilder$addDouble,
    addGuid: Sys$Data$AdoNetInvokeParametersBuilder$addGuid,
    addInteger: Sys$Data$AdoNetInvokeParametersBuilder$addInteger,
    addString: Sys$Data$AdoNetInvokeParametersBuilder$addString,
    toString: Sys$Data$AdoNetInvokeParametersBuilder$toString
}
//
//
Sys.Data.AdoNetInvokeParametersBuilder._guidRegex = /^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$/;
Sys.Data.AdoNetInvokeParametersBuilder.registerClass("Sys.Data.AdoNetInvokeParametersBuilder");
Sys.Data.AdoNetServiceError = function Sys$Data$AdoNetServiceError(timedOut, message, stackTrace, exceptionType, errorObject) {
    /// <summary locid="M:J#Sys.Data.AdoNetServiceError.#ctor" />
    /// <param name="timedOut" type="Boolean"></param>
    /// <param name="message" type="String"></param>
    /// <param name="stackTrace" type="String" mayBeNull="true" optional="true"></param>
    /// <param name="exceptionType" type="String" mayBeNull="true" optional="true"></param>
    /// <param name="errorObject" type="Object" mayBeNull="true" optional="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "timedOut", type: Boolean},
        {name: "message", type: String},
        {name: "stackTrace", type: String, mayBeNull: true, optional: true},
        {name: "exceptionType", type: String, mayBeNull: true, optional: true},
        {name: "errorObject", type: Object, mayBeNull: true, optional: true}
    ]);
    if (e) throw e;
this._errorObject = errorObject || null;
    Sys.Data.AdoNetServiceError.initializeBase(this, [timedOut, message, stackTrace, exceptionType]);
}
    function Sys$Data$AdoNetServiceError$get_errorObject() {
        /// <value type="Object" mayBeNull="true" locid="P:J#Sys.Data.AdoNetServiceError.errorObject"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._errorObject;
    }
Sys.Data.AdoNetServiceError.prototype = {
    _errorObject: null,
    get_errorObject: Sys$Data$AdoNetServiceError$get_errorObject
}
Sys.Data.AdoNetServiceError.registerClass("Sys.Data.AdoNetServiceError", Sys.Net.WebServiceError);
Sys.Data.AdoNetServiceError._getError = function Sys$Data$AdoNetServiceError$_getError(timedOut, statusCode, message, errorObject, operation) {
    var dserror, e = errorObject ? errorObject.error : null;
    if (!e) {
        dserror = new Sys.Data.AdoNetServiceError(timedOut,
            String.format(message || Sys.Data.AdoNetRes.operationFailed, operation));
    }
    else {
        var m = e.message, innererror = e.innererror, stack, type;
        m = (m && m.value) ? m.value : null;
        if (innererror) {
            type = innererror.type;
            stack = innererror.stacktrace;
        }
        dserror = new Sys.Data.AdoNetServiceError(timedOut,
            String.format(message || m || Sys.Data.AdoNetRes.operationFailed, operation),
            stack || null,
            type || null,
            errorObject);
    }
    dserror._statusCode = statusCode;
    return dserror;
}

Sys.Data.AdoNetServiceProxy = function Sys$Data$AdoNetServiceProxy(serviceUri) {
    /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.#ctor" />
    /// <param name="serviceUri" type="String"></param>
    var e = Function._validateParams(arguments, [
        {name: "serviceUri", type: String}
    ]);
    if (e) throw e;
    this._serviceUri = serviceUri;
    Sys.Data.AdoNetServiceProxy.initializeBase(this);
}
    function Sys$Data$AdoNetServiceProxy$get_path() {
        /// <value type="String" mayBeNull="false" locid="P:J#Sys.Data.AdoNetServiceProxy.path"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this.get_serviceUri();
    }
    function Sys$Data$AdoNetServiceProxy$set_path(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String}]);
        if (e) throw e;
        throw Error.notImplemented(Sys.Data.AdoNetRes.cannotChangePath);
    }
    function Sys$Data$AdoNetServiceProxy$get_replaceOnUpdate() {
        /// <value type="Boolean" mayBeNull="false" locid="P:J#Sys.Data.AdoNetServiceProxy.replaceOnUpdate"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._replaceOnUpdate;
    }
    function Sys$Data$AdoNetServiceProxy$set_replaceOnUpdate(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Boolean}]);
        if (e) throw e;
        this._replaceOnUpdate = value;
    }
    function Sys$Data$AdoNetServiceProxy$get_serviceUri() {
        /// <value type="String" mayBeNull="false" locid="P:J#Sys.Data.AdoNetServiceProxy.serviceUri"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._serviceUri;
    }
    function Sys$Data$AdoNetServiceProxy$createActionSequence() {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.createActionSequence" />
        /// <returns type="Sys.Data.AdoNetActionSequence"></returns>
        if (arguments.length !== 0) throw Error.parameterCount();
        return new Sys.Data.AdoNetActionSequence(this);
    }
    function Sys$Data$AdoNetServiceProxy$insert(item, resourceSetUri, succeededCallback, failedCallback, userContext, webRequest) {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.insert" />
        /// <param name="item" type="Object"></param>
        /// <param name="resourceSetUri" type="String"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <param name="webRequest" type="Sys.Net.WebRequest" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "item", type: Object},
            {name: "resourceSetUri", type: String},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true},
            {name: "webRequest", type: Sys.Net.WebRequest, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var wRequest = this._prepareWebRequest(item, resourceSetUri, "POST", succeededCallback, failedCallback, userContext, "insert", webRequest);
        wRequest.invoke();
        return wRequest;
    }
    function Sys$Data$AdoNetServiceProxy$invoke(operationUri, httpVerb, parameters, succeededCallback, failedCallback, userContext, webRequest) {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.invoke" />
        /// <param name="operationUri" type="String"></param>
        /// <param name="httpVerb" type="String" mayBeNull="true" optional="true"></param>
        /// <param name="parameters" type="Object" mayBeNull="true" optional="true"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <param name="webRequest" type="Sys.Net.WebRequest" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "operationUri", type: String},
            {name: "httpVerb", type: String, mayBeNull: true, optional: true},
            {name: "parameters", type: Object, mayBeNull: true, optional: true},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true},
            {name: "webRequest", type: Sys.Net.WebRequest, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        
        var qb = new Sys.Data.AdoNetQueryBuilder(operationUri);
        if (parameters) {
            var p = qb.get_queryParameters();
            for (key in parameters) {
                p[key] = parameters[key];
            }
        }
        httpVerb = httpVerb || "GET";
        var wRequest = this._prepareWebRequest(null, qb.toString(), httpVerb, succeededCallback, failedCallback, userContext, operationUri, webRequest);
        wRequest.invoke();
        return wRequest;       
    }
    function Sys$Data$AdoNetServiceProxy$fetchData(operation, parameters, mergeOption, httpVerb, succeededCallback, failedCallback, timeout, userContext) {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.fetchData" />
        /// <param name="operation"></param>
        /// <param name="parameters" type="Object" mayBeNull="true" optional="true"></param>
        /// <param name="mergeOption" type="Sys.Data.MergeOption" mayBeNull="true" optional="true"></param>
        /// <param name="httpVerb" type="String" mayBeNull="true" optional="true"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="timeout" type="Number" integer="true" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "operation"},
            {name: "parameters", type: Object, mayBeNull: true, optional: true},
            {name: "mergeOption", type: Sys.Data.MergeOption, mayBeNull: true, optional: true},
            {name: "httpVerb", type: String, mayBeNull: true, optional: true},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "timeout", type: Number, mayBeNull: true, integer: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        if (!this.get_serviceUri()) {
            throw Error.invalidOperation(Sys.Data.AdoNetRes.requiredUri);
        }
        if (!operation) {
            throw Error.argumentNull("operation");
        }
        var request, oldTimeout = null;
        if (typeof(timeout) !== "undefined") {
            oldTimeout = this.get_timeout();
            this.set_timeout(timeout);
        }
        if (parameters) {
            var q = "";
            for (var name in parameters) {
                if (q) {
                    q += "&";
                }
                q += encodeURIComponent(name) + "=" + encodeURIComponent(parameters[name]);
            }
            if (operation.indexOf("?") === -1) {
                operation += "?" + q;
            }
            else {
                operation += "&" + q;
            }
        }
        request = this.query(operation, succeededCallback, failedCallback, userContext);
        if (oldTimeout !== null) {
            this.set_timeout(oldTimeout);
        }
        return request;
    }
    function Sys$Data$AdoNetServiceProxy$fetchDeferredProperty(item, property, succeededCallback, failedCallback, userContext, webRequest) {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.fetchDeferredProperty" />
        /// <param name="item" type="Object"></param>
        /// <param name="property" type="String"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <param name="webRequest" type="Sys.Net.WebRequest" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "item", type: Object},
            {name: "property", type: String},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true},
            {name: "webRequest", type: Sys.Net.WebRequest, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        
        var succeededHelper = Function.createDelegate(this, function(result, context, operation) {
            item[property] = result;
            var callback = succeededCallback || this.get_defaultSucceededCallback();
            if (callback) {
                callback(item, context, operation);
            }
        });
        
        var uri;
        if (item[property] && item[property].__deferred && item[property].__deferred.uri) {
            uri = item[property].__deferred.uri;
        }
        else if (item.__metadata && item.__metadata.uri) {
            uri = item.__metadata.uri + '/' + property;
        }
        else {
            var message = String.format(Sys.Data.AdoNetRes.propertyNotFound, property);
            throw Sys.Data.AdoNetServiceProxy._createFailedError(property, message);
        }
        
        var wRequest = this._prepareWebRequest(null, uri, "GET", succeededHelper, failedCallback, userContext, property, webRequest);
        wRequest.invoke();
        return wRequest;
    }
    function Sys$Data$AdoNetServiceProxy$query(query, succeededCallback, failedCallback, userContext, webRequest) {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.query" />
        /// <param name="query" type="String" mayBeNull="true" optional="true"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <param name="webRequest" type="Sys.Net.WebRequest" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "query", type: String, mayBeNull: true, optional: true},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true},
            {name: "webRequest", type: Sys.Net.WebRequest, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var wRequest = this._prepareWebRequest(null, query, "GET", succeededCallback, failedCallback, userContext, query, webRequest);
        wRequest.invoke();
        return wRequest;
    }
    function Sys$Data$AdoNetServiceProxy$update(item, succeededCallback, failedCallback, userContext, webRequest) {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.update" />
        /// <param name="item" type="Object"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <param name="webRequest" type="Sys.Net.WebRequest" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "item", type: Object},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true},
            {name: "webRequest", type: Sys.Net.WebRequest, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var verb = (this._replaceOnUpdate) ? "PUT" : "MERGE";
        var wRequest = this._prepareWebRequest(item, null, verb, succeededCallback, failedCallback, userContext, "update", webRequest);
        wRequest.invoke();
        return wRequest;
    }
    function Sys$Data$AdoNetServiceProxy$remove(item, succeededCallback, failedCallback, userContext, webRequest) {
        /// <summary locid="M:J#Sys.Data.AdoNetServiceProxy.remove" />
        /// <param name="item" type="Object"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <param name="webRequest" type="Sys.Net.WebRequest" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "item", type: Object},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true},
            {name: "webRequest", type: Sys.Net.WebRequest, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var wRequest = this._prepareWebRequest(item, null, "DELETE", succeededCallback, failedCallback, userContext, "remove", webRequest);
        wRequest.set_body(null);
        delete wRequest.get_headers()["Content-Type"];
        wRequest.invoke();
        return wRequest;
    }
    function Sys$Data$AdoNetServiceProxy$_checkForError(executor, operation, checkVersion) {
        var message, errorObj = null, timedOut = false,
            statusCode = 0;
        if (!executor.get_responseAvailable()) {
            timedOut = executor.get_timedOut();
            message = (timedOut ? Sys.Data.AdoNetRes.operationTimedOut : String.format(Sys.Data.AdoNetRes.operationFailed, operation));
        }
        else {
            statusCode = executor.get_statusCode();        
            if ((statusCode === 1223) || (statusCode === 0)) {
                statusCode = 204;
            }
            if (checkVersion) {
                var versionHeader = executor.getResponseHeader('DataServiceVersion');
                if (!versionHeader.startsWith('1.0;') && statusCode !== 204) {
                    message = (versionHeader.length > 0)
                        ? String.format(Sys.Data.AdoNetRes.serviceVersionTooHigh, this.get_serviceUri())
                        : String.format(Sys.Data.AdoNetRes.uriNotAdoNetService, this.get_serviceUri());
                }
            }
            if (!message && (statusCode < 200 || statusCode >= 300)) {
                var contentType = executor.getResponseHeader("Content-Type");
                if (contentType.startsWith("application/json")) {
                    errorObj = executor.get_object();
                }
                else if (contentType.startsWith("application/xml") || contentType.startsWith("text/xml")) {
                    var xml = executor.get_xml(), elements = xml.documentElement.getElementsByTagName("message");
                    if (elements && elements.length) {
                        var element = elements[0];
                        if (element.childNodes.length) {
                            message = element.childNodes[0].nodeValue;
                        }
                    }
                    if (!message) {
                        message = String.format(Sys.Data.AdoNetRes.uriNotAdoNetService, this.get_serviceUri());
                    }
                }
                else {
                    message = String.format(Sys.Data.AdoNetRes.uriNotAdoNetService, this.get_serviceUri());
                }
            }
        }
        if (message || errorObj) {
            return Sys.Data.AdoNetServiceError._getError(timedOut, statusCode, message, errorObj, operation);
        }
        return null;
    }
    function Sys$Data$AdoNetServiceProxy$_onResponseComplete(executor, onSuccess, onFailure, userContext, operation) {
        var dsError = this._checkForError(executor, operation, true);
        if (dsError) {
            if (onFailure) {
                onFailure(dsError, userContext, operation);
            }
            else {
                throw Sys.Data.AdoNetServiceProxy._createFailedError(operation, dsError.get_message());
            }
        }
        else if (onSuccess) {
            var contentType = executor.getResponseHeader("Content-Type"),
                obj = null;
            if (contentType.startsWith("application/json")) {
                obj = executor.get_object();
                obj = obj.d || obj;
            }
            onSuccess(obj, userContext, operation);
        }
    }
    function Sys$Data$AdoNetServiceProxy$_prepareWebRequest(item, relativeUri, verb, onSuccess, onFailure, context, operation, webRequest) {
        webRequest = webRequest || new Sys.Net.WebRequest();
        webRequest.set_url(Sys.Data._AdoNetUtil.concatUris(this._serviceUri, relativeUri || ""));
        webRequest.set_timeout(this.get_timeout());
        var headers = webRequest.get_headers();
        headers["Accept"] = "application/json";
        headers["DataServiceVersion"] = "1.0;AspNetAjax";
        headers["MaxDataServiceVersion"] = "1.0;";
        webRequest.set_httpVerb(verb);
        if (this._usePostTunneling) {
            var verbUpper = verb.toUpperCase();
            if ((verbUpper === "PUT") || (verbUpper === "DELETE") || (verbUpper === "MERGE")) {
                webRequest.set_httpVerb("POST");
                headers["X-HTTP-Method"] = verbUpper;
            }
        }
        if (item) {
            webRequest.set_body(Sys.Serialization.JavaScriptSerializer.serialize(item));
            headers["Content-Type"] = "application/json";
            var eTag = Sys.Data._AdoNetUtil.extractETag(item);
            if (eTag) {
                headers["If-Match"] = eTag;
            }
            var uri = Sys.Data._AdoNetUtil.extractUri(item);
            if (uri) {
                webRequest.set_url(uri);
            }
        }
        onSuccess = onSuccess || this.get_defaultSucceededCallback();
        onFailure = onFailure || this.get_defaultFailedCallback();
        if ((typeof(context) === "undefined") || (context === null)) {
            context = this.get_defaultUserContext();
        }
        webRequest.add_completed(Function.createDelegate(this, function(executor) {
            this._onResponseComplete(executor, onSuccess, onFailure, context, operation);
        }));
        return webRequest;
    }
Sys.Data.AdoNetServiceProxy.prototype = {
    _replaceOnUpdate: false,
    _serviceUri: null,
    _usePostTunneling: true,
    get_path: Sys$Data$AdoNetServiceProxy$get_path,
    set_path: Sys$Data$AdoNetServiceProxy$set_path,
    get_replaceOnUpdate: Sys$Data$AdoNetServiceProxy$get_replaceOnUpdate,
    set_replaceOnUpdate: Sys$Data$AdoNetServiceProxy$set_replaceOnUpdate,
    get_serviceUri: Sys$Data$AdoNetServiceProxy$get_serviceUri,
    createActionSequence: Sys$Data$AdoNetServiceProxy$createActionSequence,
    insert: Sys$Data$AdoNetServiceProxy$insert,
    invoke: Sys$Data$AdoNetServiceProxy$invoke,
    fetchData: Sys$Data$AdoNetServiceProxy$fetchData,
    fetchDeferredProperty: Sys$Data$AdoNetServiceProxy$fetchDeferredProperty,
    query: Sys$Data$AdoNetServiceProxy$query,
    update: Sys$Data$AdoNetServiceProxy$update,
    remove: Sys$Data$AdoNetServiceProxy$remove,
    _checkForError: Sys$Data$AdoNetServiceProxy$_checkForError,
    _onResponseComplete: Sys$Data$AdoNetServiceProxy$_onResponseComplete,
    _prepareWebRequest: Sys$Data$AdoNetServiceProxy$_prepareWebRequest
}
Sys.Data.AdoNetServiceProxy._createFailedError = function Sys$Data$AdoNetServiceProxy$_createFailedError(operation, errorMessage) {
    var displayMessage = "Sys.Data.AdoNetException: " + errorMessage;
    var e = Error.create(displayMessage, { name: "Sys.Data.AdoNetException", operation: operation });
    e.popStackFrame();
    return e;
}
Sys.Data.AdoNetServiceProxy.registerClass("Sys.Data.AdoNetServiceProxy", Sys.Net.WebServiceProxy, Sys.Data.IDataProvider);
Sys.Data._AdoNetBatchReader = function Sys$Data$_AdoNetBatchReader(responseBody, boundary) {
    if (!responseBody) throw "Argument 'responseBody' missing";
    if (!boundary) throw "Argument 'boundary' missing";
    this._responseBody = responseBody;
    this._boundary = [boundary];
    this._position = 0;
    this._responses = [];
    this._parseParts(this._responses);
}
    function Sys$Data$_AdoNetBatchReader$get_responses() {
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._responses;
    }
    function Sys$Data$_AdoNetBatchReader$_parseParts(responses) {
        if (this._readToMark("--" + this._currentBoundary(), true) === null) return;
        this._readLine();
        var endmark = null;
        while ((endmark !== "--") && !this._eof()) {
            var partHeaders = [];
            this._parseHeaders(partHeaders);
            var partType = partHeaders["Content-Type"];
            if (!partType) throw "Invalid MIME part, no content-type header";
            if (partType.indexOf("multipart/mixed") === 0) {
                var nested = [];
                this._boundary.push(Sys.Data._AdoNetBatchReader._boundaryFromTypeHeader(partType));
                this._parseParts(nested);
                this._boundary.pop();
                responses.push(nested);
                var check = this._readToMark("--" + this._currentBoundary(), true);
                if (check === null) throw "Invalid multi-part document, cannot find closing boundary";
            }
            else if (partType.indexOf("application/http") === 0) {
                responses.push(this._parseHttpResponse());
            }
            else {
                throw "Invalid MIME part type '" + partType + "'";
            }
            endmark = this._peek(2);
            this._readLine();
        }
    }
    function Sys$Data$_AdoNetBatchReader$_parseHttpResponse() {
        var line = this._readLine(),
            status = this._parseStatus(line);
        if (status === null) throw "Invalid HTTP status line '" + line + "'";
        var headers = [];
        this._parseHeaders(headers);
        var body = this._readToMark("--" + this._currentBoundary(), true);
        if (body === null) throw "Invalid multipart document, cannot find closing/next boundary";
        if (body === "\r\n") body = "";
        return { status: status, headers: headers, body: body };
    }
    function Sys$Data$_AdoNetBatchReader$_parseHeaders(target) {
        for (var line = this._readLine(); line; line = this._readLine()) {
            var h = this._parseHeader(line);
            if (h === null) throw "Expected header with format name : value";
            target[h.name] = h.value;
        }
    }
    function Sys$Data$_AdoNetBatchReader$_parseHeader(s) {
        if (s === null) return null;
        var index = s.indexOf(":");
        return (index === -1) ? null :
            { name: s.substring(0, index).trim(), value: s.substring(index + 1).trim() };
    }
    function Sys$Data$_AdoNetBatchReader$_parseStatus(s) {
        var match = Sys.Data._AdoNetBatchReader._statusRegExp.exec(s);
        return match ? ({ code: match[1], text: match[2] }) : null;
    }
    function Sys$Data$_AdoNetBatchReader$_currentBoundary() {
        return this._boundary[this._boundary.length - 1];
    }
    function Sys$Data$_AdoNetBatchReader$_eof() {
        return this._position === -1;
    }
    function Sys$Data$_AdoNetBatchReader$_readLine() {
        return this._readToMark("\r\n", false);
    }
    function Sys$Data$_AdoNetBatchReader$_readToMark(mark, nullIfMissing) {
        if (this._eof()) return null; 
        var r, index = this._responseBody.indexOf(mark, this._position);
        if (index < 0) {
            if (nullIfMissing) {
                r = null;
            }
            else {
                r = this._responseBody.substring(this._position);
                this._position = -1;
            }
        }
        else {
            r = this._responseBody.substring(this._position, index);
            this._position = index + mark.length;
        }
        return r;
    }
    function Sys$Data$_AdoNetBatchReader$_peek(len) {
        if (!len) throw "Argument 'len' missing";
        if (this._eof()) return "";
        return this._responseBody.substring(this._position, this._position + len);
    }
Sys.Data._AdoNetBatchReader.prototype = {
    get_responses: Sys$Data$_AdoNetBatchReader$get_responses,
    _parseParts: Sys$Data$_AdoNetBatchReader$_parseParts,
    _parseHttpResponse: Sys$Data$_AdoNetBatchReader$_parseHttpResponse,
    _parseHeaders: Sys$Data$_AdoNetBatchReader$_parseHeaders,
    _parseHeader: Sys$Data$_AdoNetBatchReader$_parseHeader,
    _parseStatus: Sys$Data$_AdoNetBatchReader$_parseStatus,
    _currentBoundary: Sys$Data$_AdoNetBatchReader$_currentBoundary,
    _eof: Sys$Data$_AdoNetBatchReader$_eof,
    _readLine: Sys$Data$_AdoNetBatchReader$_readLine,
    _readToMark: Sys$Data$_AdoNetBatchReader$_readToMark,
    _peek: Sys$Data$_AdoNetBatchReader$_peek
}
Sys.Data._AdoNetBatchReader._boundaryFromTypeHeader = function Sys$Data$_AdoNetBatchReader$_boundaryFromTypeHeader(header) {
    if (!header) throw "Argument 'header' missing";
    var re = /;\s*boundary=(.*)$/i,
        match = re.exec(header);
    return match ? match[1] : null;
};
Sys.Data._AdoNetBatchReader._parseResponse = function Sys$Data$_AdoNetBatchReader$_parseResponse(executor) {
    var r = new Sys.Data._AdoNetBatchReader(executor.get_responseData(), Sys.Data._AdoNetBatchReader._boundaryFromTypeHeader(executor.getResponseHeader("Content-Type")));
    return r.get_responses();
};
Sys.Data._AdoNetBatchReader._statusRegExp = new RegExp("^HTTP\\/1\\.[01] (\\d{3}) (.*)$", "i");
Sys.Data._AdoNetBatchReader.registerClass("Sys.Data._AdoNetBatchReader");
Sys.Data._AdoNetBatchWriter = function Sys$Data$_AdoNetBatchWriter(host) {
    this._host = host;
    this._content = "";
    this._boundary = null;
    this._changesetBoundary = null;
    this._changesetEntries = null;
    this._contentType = "application/json";
}
    function Sys$Data$_AdoNetBatchWriter$get_contentType() {
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._contentType;
    }
    function Sys$Data$_AdoNetBatchWriter$set_contentType(type) {
        var re = /;\s*charset\s*=/i,
            match = re.exec(type);
        if (match) throw "MIME type cannot include 'charset' parameter";
        this._contentType = type;
    }
    function Sys$Data$_AdoNetBatchWriter$get_requestBody() {
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._content + "--" + this.get_topBoundary() + "--";
    }
    function Sys$Data$_AdoNetBatchWriter$get_topBoundary() {
        if (arguments.length !== 0) throw Error.parameterCount();
        if (!this._boundary) {
            this._boundary = "batch_" + this._createBoundary();
        }
        return this._boundary;
    }
    function Sys$Data$_AdoNetBatchWriter$addChange(targetUri, eTag, method, body, contentId) {
        if (!targetUri) throw "Argument 'targetUri' missing";
        if (!method) throw "Argument 'method' missing";
        if (!this._changesetBoundary) throw "There is no active changeset";
        this._changesetEntries.push({ uri: targetUri, eTag: eTag, method: method, body: body, contentId: contentId });
    }
    function Sys$Data$_AdoNetBatchWriter$addQuery(queryUri) {
        if (!queryUri) throw "Argument 'queryUri' missing";
        this._content += this._startPart(this.get_topBoundary(), "GET", queryUri, null) + "\r\n";
    }
    function Sys$Data$_AdoNetBatchWriter$endChangeSet() {
        if (!this._changesetBoundary) throw "There is no active changeset";
        var changeset = "";
        for (var key in this._changesetEntries) {
            var entry = this._changesetEntries[key];
            changeset += this._startPart(this._changesetBoundary, entry.method, entry.uri, entry.eTag, entry.contentId);
            if (entry.body) {
                changeset += "Content-Type: " + this._contentType + ";charset=utf-8\r\n";
            }
            changeset += "\r\n";
            if (entry.body) {
                changeset += entry.body;
            }
        }
        if (changeset) {
            changeset += "\r\n--" + this._changesetBoundary + "--\r\n";
        }
        this._content += "\r\n--" + this.get_topBoundary() + "\r\nContent-Type: multipart/mixed;boundary=" +
                         this._changesetBoundary + "\r\n\r\n" + changeset;
        this._changesetBoundary = null;
        this._changesetEntries = null;
    }
    function Sys$Data$_AdoNetBatchWriter$startChangeSet() {
        if (this._changesetBoundary) throw "There is an active changeset already";
        this._changesetBoundary = "changeset_" + this._createBoundary();
        this._changesetEntries = [];
    }
    function Sys$Data$_AdoNetBatchWriter$_createBoundary() {
        function hex16() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substr(1);
        }
        return hex16() + "-" + hex16() + "-" + hex16();
    }
    function Sys$Data$_AdoNetBatchWriter$_startPart(boundary, method, uri, eTag, contentId) {
        var start = "\r\n--" + boundary + "\r\nContent-Type: application/http\r\nContent-Transfer-Encoding: binary\r\n\r\n" + method + " " + uri + " HTTP/1.1\r\n";
        if (typeof(contentId) === "number") {
            start += "Content-ID: " + contentId + "\r\n";
        }
        if (eTag) {
            start += "If-Match: " + eTag + "\r\n";
        }
        start += "Host: " + this._host + "\r\nAccept: " + this.get_contentType() + "\r\nAccept-Charset: utf-8\r\n";
        return start;
    }
Sys.Data._AdoNetBatchWriter.prototype = {
    get_contentType: Sys$Data$_AdoNetBatchWriter$get_contentType,
    set_contentType: Sys$Data$_AdoNetBatchWriter$set_contentType,
    get_requestBody: Sys$Data$_AdoNetBatchWriter$get_requestBody,
    get_topBoundary: Sys$Data$_AdoNetBatchWriter$get_topBoundary,
    addChange: Sys$Data$_AdoNetBatchWriter$addChange,
    addQuery: Sys$Data$_AdoNetBatchWriter$addQuery,
    endChangeSet: Sys$Data$_AdoNetBatchWriter$endChangeSet,
    startChangeSet: Sys$Data$_AdoNetBatchWriter$startChangeSet,
    _createBoundary: Sys$Data$_AdoNetBatchWriter$_createBoundary,
    _startPart: Sys$Data$_AdoNetBatchWriter$_startPart
}
Sys.Data._AdoNetBatchWriter.registerClass("Sys.Data._AdoNetBatchWriter");

Type.registerNamespace('Sys.Data');

Sys.Data.AdoNetRes={
'entityWithNoResourceSet':'Entity does not have a resource set. Use createEntity() or set the defaultResourceSet property.',
'cannotChangePath':'Cannot change the path of a AdoNetServiceProxy.',
'requiredUri':'A serviceUri must be set prior to calling fetchData.',
'invalidGuid':'The provided value must be of the form \'dddddddd-dddd-dddd-dddd-dddddddddddd\', where each d is a digit or a character A - F.',
'propertyNotFound':'The property \'{0}\' was not found or did not contain an associated metadata URI.',
'batchLinkBeforeInsert':'The source and target of a link must be inserted before the link is made.',
'uriNotAdoNetService':'The URI \'{0}\' does not point to an ADO.NET Data Service.',
'invalidBatchResponse':'The batch operation failed due to an invalid response from \'{0}\'.',
'operationTimedOut':'The data operation \'{0}\' timed out.',
'operationFailed':'The data operation \'{0}\' failed.',
'serviceVersionTooHigh':'The URI \'{0}\' points to an ADO.NET Data Service of a higher version than is supported by this library.'
};

