// Name:        MicrosoftAjaxTemplates.debug.js
// Assembly:    System.Web.Extensions
// Version:     4.0.0.0
// FileVersion: 4.0.20227.0
//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// MicrosoftAjaxTemplates.js
// Microsoft AJAX Templating Framework.
/// <reference name="MicrosoftAjaxComponentModel.js" />
/// <reference name="MicrosoftAjaxSerialization.js" />
Type._registerScript("MicrosoftAjaxTemplates.js", ["MicrosoftAjaxComponentModel.js", "MicrosoftAjaxSerialization.js"]);
Type.registerNamespace("Sys.Net");
Sys.Net.WebServiceOperation = function Sys$Net$WebServiceOperation(operation, parameters, httpVerb) {
    /// <summary locid="M:J#Sys.Net.WebServiceOperation.#ctor" />
    /// <param name="operation"></param>
    /// <param name="parameters" type="Object" mayBeNull="true" optional="true"></param>
    /// <param name="httpVerb" type="String" mayBeNull="true" optional="true"></param>
    /// <field name="operation" locid="F:J#Sys.Net.WebServiceOperation.operation"></field>
    /// <field name="parameters" type="Object" mayBeNull="true" locid="F:J#Sys.Net.WebServiceOperation.parameters"></field>
    /// <field name="httpVerb" type="String" mayBeNull="true" locid="F:J#Sys.Net.WebServiceOperation.httpVerb"></field>
    var e = Function._validateParams(arguments, [
        {name: "operation"},
        {name: "parameters", type: Object, mayBeNull: true, optional: true},
        {name: "httpVerb", type: String, mayBeNull: true, optional: true}
    ]);
    if (e) throw e;
    if (typeof(operation) === "undefined") {
        operation = null;
    }
    this.operation = operation;
    this.parameters = parameters || null;
    this.httpVerb = httpVerb || null;
}
Sys.Net.WebServiceOperation.prototype = {
    operation: null,
    parameters: null,
    httpVerb: null
}
Sys.Net.WebServiceOperation.registerClass("Sys.Net.WebServiceOperation");
Sys.Net.WebRequestEventArgs = function Sys$Net$WebRequestEventArgs(executor, error, result) {
    /// <summary locid="M:J#Sys.Net.WebRequestEventArgs.#ctor" />
    /// <param name="executor" type="Sys.Net.WebRequestExecutor" mayBeNull="true"></param>
    /// <param name="error" type="Sys.Net.WebServiceError" optional="true" mayBeNull="true"></param>
    /// <param name="result" optional="true" mayBeNull="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "executor", type: Sys.Net.WebRequestExecutor, mayBeNull: true},
        {name: "error", type: Sys.Net.WebServiceError, mayBeNull: true, optional: true},
        {name: "result", mayBeNull: true, optional: true}
    ]);
    if (e) throw e;
    this._executor = executor;
    this._error = error || null;
    this._result = typeof (result) === "undefined" ? null : result;
    Sys.Net.WebRequestEventArgs.initializeBase(this);
}
    function Sys$Net$WebRequestEventArgs$get_error() {
        /// <value type="Sys.Net.WebServiceError" mayBeNull="true" locid="P:J#Sys.Net.WebRequestEventArgs.error"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._error || null;
    }
    function Sys$Net$WebRequestEventArgs$get_executor() {
        /// <value type="Sys.Net.WebRequestExecutor" mayBeNull="true" locid="P:J#Sys.Net.WebRequestEventArgs.executor"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._executor;
    }
    function Sys$Net$WebRequestEventArgs$get_result() {
        /// <value mayBeNull="true" locid="P:J#Sys.Net.WebRequestEventArgs.result"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._result;
    }
Sys.Net.WebRequestEventArgs.prototype = {
    get_error: Sys$Net$WebRequestEventArgs$get_error,
    get_executor: Sys$Net$WebRequestEventArgs$get_executor,
    get_result: Sys$Net$WebRequestEventArgs$get_result
};
Sys.Net.WebRequestEventArgs.registerClass("Sys.Net.WebRequestEventArgs", Sys.EventArgs);
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
Sys.Data.DataContext = function Sys$Data$DataContext() {
    Sys.Data.DataContext.initializeBase(this);
    this._dataChangedDel = Function.createDelegate(this, this._dataChanged);
    this._items = {};
    this._methods = {};
}
    function Sys$Data$DataContext$get_changes() {
        /// <value type="Array" elementType="Sys.Data.ChangeOperation" locid="P:J#Sys.Data.DataContext.changes"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        var changes = this._changelist;
        if (!changes) {
            this._changelist = changes = [];
        }
        return changes;
    }
    function Sys$Data$DataContext$get_createEntityMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.createEntityMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.createEntity || null;
    }
    function Sys$Data$DataContext$set_createEntityMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        this._methods.createEntity = value;
    }
    function Sys$Data$DataContext$get_getIdentityMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.getIdentityMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.getIdentity || null;
    }
    function Sys$Data$DataContext$set_getIdentityMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        if (this.get_isInitialized() && ((this._getIdentityMethod && !value) || (!this._getIdentityMethod && value))) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.commonNotAfterInit, "DataContext", "getIdentityMethod"));
        }
        this._methods.getIdentity = value;
        this._useIdentity = !!value;
    }
    function Sys$Data$DataContext$get_handleSaveChangesResultsMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.handleSaveChangesResultsMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.handleSaveResults || null;
    }
    function Sys$Data$DataContext$set_handleSaveChangesResultsMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        this._methods.handleSaveResults = value;
    }
    function Sys$Data$DataContext$get_isDeferredPropertyMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.isDeferredPropertyMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.isDeferredProperty || null;
    }
    function Sys$Data$DataContext$set_isDeferredPropertyMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        this._methods.isDeferredProperty = value;
    }
    function Sys$Data$DataContext$get_getNewIdentityMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.getNewIdentityMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.getNewIdentity || null;
    }
    function Sys$Data$DataContext$set_getNewIdentityMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        this._methods.getNewIdentity = value;
    }
    function Sys$Data$DataContext$get_getDeferredPropertyFetchOperationMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.getDeferredPropertyFetchOperationMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.getDeferredQuery || null;
    }
    function Sys$Data$DataContext$set_getDeferredPropertyFetchOperationMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        this._methods.getDeferredQuery = value;
    }
    function Sys$Data$DataContext$get_items() {
        /// <value type="Object" locid="P:J#Sys.Data.DataContext.items"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._items;
    }
    function Sys$Data$DataContext$get_lastFetchDataResults() {
        /// <value mayBeNull="true" locid="P:J#Sys.Data.DataContext.lastFetchDataResults"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._lastResults || null;
    }
    function Sys$Data$DataContext$get_hasChanges() {
        /// <value type="Boolean" locid="P:J#Sys.Data.DataContext.hasChanges"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._hasChanges;
    }
    function Sys$Data$DataContext$get_fetchDataMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.fetchDataMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.fetchData || null;
    }
    function Sys$Data$DataContext$set_fetchDataMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        this._methods.fetchData = value;
    }
    function Sys$Data$DataContext$get_mergeOption() {
        /// <value type="Sys.Data.MergeOption" locid="P:J#Sys.Data.DataContext.mergeOption"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._mergeOption;
    }
    function Sys$Data$DataContext$set_mergeOption(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Sys.Data.MergeOption}]);
        if (e) throw e;
        this._mergeOption = value;
    }
    function Sys$Data$DataContext$get_saveChangesMethod() {
        /// <value type="Function" mayBeNull="true" locid="P:J#Sys.Data.DataContext.saveChangesMethod"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._methods.saveChanges || null;
    }
    function Sys$Data$DataContext$set_saveChangesMethod(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Function, mayBeNull: true}]);
        if (e) throw e;
        this._methods.saveChanges = value;
    }
    function Sys$Data$DataContext$get_saveOperation() {
        /// <value type="String" mayBeNull="true" locid="P:J#Sys.Data.DataContext.saveOperation"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._saveOperation || "";
    }
    function Sys$Data$DataContext$set_saveOperation(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String, mayBeNull: true}]);
        if (e) throw e;
        this._saveOperation = value;
    }
    function Sys$Data$DataContext$get_saveHttpVerb() {
        /// <value type="String" locid="P:J#Sys.Data.DataContext.saveHttpVerb"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._saveHttpVerb || "POST";
    }
    function Sys$Data$DataContext$set_saveHttpVerb(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String}]);
        if (e) throw e;
        this._saveHttpVerb = value;
    }
    function Sys$Data$DataContext$get_saveParameters() {
        /// <value type="Object" mayBeNull="true" locid="P:J#Sys.Data.DataContext.saveParameters"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._saveParameters;
    }
    function Sys$Data$DataContext$set_saveParameters(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Object, mayBeNull: true}]);
        if (e) throw e;
        this._saveParameters = value;
    }
    function Sys$Data$DataContext$get_saveChangesTimeout() {
        /// <value type="Number" integer="true" locid="P:J#Sys.Data.DataContext.saveChangesTimeout"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._saveTimeout;
    }
    function Sys$Data$DataContext$set_saveChangesTimeout(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Number, integer: true}]);
        if (e) throw e;
        this._saveTimeout = value;
    }
    function Sys$Data$DataContext$get_isSaving() {
        /// <value type="Boolean" locid="P:J#Sys.Data.DataContext.isSaving"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._saving;
    }
    function Sys$Data$DataContext$get_serviceUri() {
        /// <value type="String" mayBeNull="true" locid="P:J#Sys.Data.DataContext.serviceUri"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._serviceUri || "";
    }
    function Sys$Data$DataContext$set_serviceUri(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String, mayBeNull: true}]);
        if (e) throw e;
        this._serviceUri = value;
    }
    function Sys$Data$DataContext$addLink(sourceEntity, sourcePropertyName, targetEntity) {
        /// <summary locid="M:J#Sys.Data.DataContext.addLink" />
        /// <param name="sourceEntity" type="Object"></param>
        /// <param name="sourcePropertyName" type="String"></param>
        /// <param name="targetEntity" type="Object"></param>
        var e = Function._validateParams(arguments, [
            {name: "sourceEntity", type: Object},
            {name: "sourcePropertyName", type: String},
            {name: "targetEntity", type: Object}
        ]);
        if (e) throw e;
        var toggled = this._toggleLink(sourceEntity, sourcePropertyName, targetEntity),
            valueSet = this._setLinkField(true, sourceEntity, sourcePropertyName, targetEntity);
        if (!toggled || (toggled.action !== Sys.Data.ChangeOperationType.remove)) {
            if (valueSet || (toggled && (toggled.action === Sys.Data.ChangeOperationType.insert))) {
                this._registerChange(new Sys.Data.ChangeOperation(Sys.Data.ChangeOperationType.insert,
                    null, sourceEntity, sourcePropertyName, targetEntity));
            }
        }
    }
    function Sys$Data$DataContext$removeLink(sourceEntity, sourcePropertyName, targetEntity) {
        /// <summary locid="M:J#Sys.Data.DataContext.removeLink" />
        /// <param name="sourceEntity" type="Object"></param>
        /// <param name="sourcePropertyName" type="String"></param>
        /// <param name="targetEntity" type="Object"></param>
        var e = Function._validateParams(arguments, [
            {name: "sourceEntity", type: Object},
            {name: "sourcePropertyName", type: String},
            {name: "targetEntity", type: Object}
        ]);
        if (e) throw e;
        var toggled = this._toggleLink(sourceEntity, sourcePropertyName, targetEntity),
            valueSet = this._setLinkField(true, sourceEntity, sourcePropertyName, targetEntity, true);
            
        if (!toggled || (toggled.action !== Sys.Data.ChangeOperationType.insert)) {
            if (valueSet || (toggled && (toggled.action === Sys.Data.ChangeOperationType.remove))) {
                this._registerChange(new Sys.Data.ChangeOperation(Sys.Data.ChangeOperationType.remove,
                    null, sourceEntity, sourcePropertyName, targetEntity));
            }
        }
    }
    function Sys$Data$DataContext$setLink(sourceEntity, sourcePropertyName, targetEntity) {
        /// <summary locid="M:J#Sys.Data.DataContext.setLink" />
        /// <param name="sourceEntity" type="Object"></param>
        /// <param name="sourcePropertyName" type="String"></param>
        /// <param name="targetEntity" type="Object" mayBeNull="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "sourceEntity", type: Object},
            {name: "sourcePropertyName", type: String},
            {name: "targetEntity", type: Object, mayBeNull: true}
        ]);
        if (e) throw e;
        this._toggleLink(sourceEntity, sourcePropertyName, targetEntity);
        this._setLinkField(false, sourceEntity, sourcePropertyName, targetEntity);
        this._registerChange(new Sys.Data.ChangeOperation(Sys.Data.ChangeOperationType.update,
            null, sourceEntity, sourcePropertyName, targetEntity));
    }
    function Sys$Data$DataContext$abortSave() {
        /// <summary locid="M:J#Sys.Data.DataContext.abortSave" />
        if (arguments.length !== 0) throw Error.parameterCount();
        if (this._saverequest) {
            this._saverequest.get_executor().abort();
            this._saverequest = null;
        }
        if (this._saving) {
            this._saving = false;
            this.raisePropertyChanged("isSaving");
        }
    }
    function Sys$Data$DataContext$clearChanges() {
        /// <summary locid="M:J#Sys.Data.DataContext.clearChanges" />
        if (arguments.length !== 0) throw Error.parameterCount();
        this._edits = this._deletes = this._inserts = null;
        if (this._changelist) {
            Sys.Observer.clear(this._changelist);
        }
        if (this._hasChanges) {
            this._hasChanges = false;
            this.raisePropertyChanged("hasChanges");
        }
    }
    function Sys$Data$DataContext$clearData() {
        /// <summary locid="M:J#Sys.Data.DataContext.clearData" />
        if (arguments.length !== 0) throw Error.parameterCount();
        this._clearData();
    }
    function Sys$Data$DataContext$createEntity(entitySetName) {
        /// <summary locid="M:J#Sys.Data.DataContext.createEntity" />
        /// <param name="entitySetName" type="String" optional="true" mayBeNull="true"></param>
        /// <returns type="Object"></returns>
        var e = Function._validateParams(arguments, [
            {name: "entitySetName", type: String, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var getter = this.get_createEntityMethod();
        if (!getter) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.requiredMethodProperty, "createEntityMethod", "createEntity"));
        }
        return getter(this, entitySetName);
    }
    function Sys$Data$DataContext$dispose() {
        /// <summary locid="M:J#Sys.Data.DataContext.dispose" />
        if (arguments.length !== 0) throw Error.parameterCount();
        if (this._disposed) return;
        this._disposed = true;
        if (this.get_isSaving()) {
            this.abortSave();
        }
        this.clearData();
        this._lastResults = null;
        this._saverequest = null;
        this._methods = {};        
        Sys.Data.DataContext.callBaseMethod(this, "dispose");
    }
    function Sys$Data$DataContext$initialize() {
        /// <summary locid="M:J#Sys.Data.DataContext.initialize" />
        if (arguments.length !== 0) throw Error.parameterCount();
        this.updated();
        Sys.Data.DataContext.callBaseMethod(this, "initialize");
    }
    function Sys$Data$DataContext$fetchDeferredProperty(entity, propertyName, mergeOption, succeededCallback, failedCallback, timeout, userContext) {
        /// <summary locid="M:J#Sys.Data.DataContext.fetchDeferredProperty" />
        /// <param name="entity" type="Object"></param>
        /// <param name="propertyName" type="String"></param>
        /// <param name="mergeOption" type="Sys.Data.MergeOption" optional="true" mayBeNull="true"></param>
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="timeout" type="Number" integer="true" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "entity", type: Object},
            {name: "propertyName", type: String},
            {name: "mergeOption", type: Sys.Data.MergeOption, mayBeNull: true, optional: true},
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "timeout", type: Number, mayBeNull: true, integer: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var getter = this.get_getDeferredPropertyFetchOperationMethod();
        if (!getter) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.requiredMethodProperty, "getDeferredPropertyFetchOperationMethod", "fetchDeferredProperty"));
        }
        var _this = this, query = getter(this, entity, propertyName, userContext);
        if (query && query.operation) {
            function done(result) {
                _this._setField(entity, propertyName, null, result, null, true);
                if (succeededCallback) {
                    succeededCallback(result, userContext, propertyName);
                }
            }
            function fail(error) {
                if (failedCallback) {
                    failedCallback(error, userContext, propertyName);
                }
            }
            if (typeof(userContext) === "undefined") {
                userContext = null;
            }
            if ((typeof(mergeOption) === "undefined") || (mergeOption === null)) {
                mergeOption = this.get_mergeOption();
            }
            return this.fetchData(query.operation, query.parameters || null, mergeOption, query.httpVerb || "POST", done, fail, timeout || 0, userContext);
        }
    }
    function Sys$Data$DataContext$getNewIdentity(entity, entitySetName) {
        /// <summary locid="M:J#fail" />
        /// <param name="entity"></param>
        /// <param name="entitySetName" type="String" mayBeNull="true"></param>
        /// <returns mayBeNull="true"></returns>
        var e = Function._validateParams(arguments, [
            {name: "entity"},
            {name: "entitySetName", type: String, mayBeNull: true}
        ]);
        if (e) throw e;
        var getter = this.get_getNewIdentityMethod();
        return getter ? (getter(this, entity, entitySetName) || null) : null;
    }
    function Sys$Data$DataContext$insertEntity(entity, entitySetName) {
        /// <summary locid="M:J#fail" />
        /// <param name="entity"></param>
        /// <param name="entitySetName" type="String" optional="true" mayBeNull="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "entity"},
            {name: "entitySetName", type: String, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        var identity = null;
        if (this._useIdentity) {
            identity = this.getIdentity(entity);
            if (identity === null) {
                identity = this.getNewIdentity(entity, entitySetName || null);
            }
            if (!identity) {
                throw Error.invalidOperation(Sys.UI.TemplatesRes.requiredIdentity);
            }
            if (this._items[identity]) {
                throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.entityAlreadyExists, identity));
            }
            this._storeEntity(identity, entity);
        }
        else {
            this._captureEntity(entity);
        }
        this._inserts = this._pushChange(this._inserts, entity, identity);
        this._registerChange(new Sys.Data.ChangeOperation(Sys.Data.ChangeOperationType.insert, entity));
    }
    function Sys$Data$DataContext$removeEntity(entity) {
        /// <summary locid="M:J#fail" />
        /// <param name="entity"></param>
        var e = Function._validateParams(arguments, [
            {name: "entity"}
        ]);
        if (e) throw e;
        if (this._ignoreChange) return;
        var identity = this.getIdentity(entity);
        if (identity !== null) {
            entity = this._items[identity];
            if (typeof (entity) === "undefined") {
                return;
            }
            delete this._items[identity];
        }
        this._releaseEntity(entity);
        var _this = this,
            changelist = this.get_changes(),
            hadChange = this._hasChanges;
        function unregister() {
            for (var i = 0, l = changelist.length; i < l; i++) {
                if (changelist[i].item === entity) {
                    Sys.Observer.removeAt(changelist, i);
                    _this._hasChanges = !!changelist.length;
                    return;
                }
            }
        }
        if (this._peekChange(this._inserts, entity, identity, true)) {
            unregister();
        }
        else {
            this._deletes = this._pushChange(this._deletes, entity, identity);
            if (this._peekChange(this._edits, entity, identity, true)) {
                unregister();
            }
            Sys.Observer.add(changelist, new Sys.Data.ChangeOperation(Sys.Data.ChangeOperationType.remove, entity));
            this._hasChanges = true;
        }
        if (this._hasChanges !== hadChange) {
            this._raiseChanged("hasChanges");
        }
    }
    function Sys$Data$DataContext$fetchData(operation, parameters, mergeOption, httpVerb, succeededCallback, failedCallback, timeout, userContext) {
        /// <summary locid="M:J#unregister" />
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
        var _this = this;
        if ((typeof(mergeOption) === "undefined") || (mergeOption === null)) {
            mergeOption = this.get_mergeOption();
        }
        function done(data) {
            if (_this._disposed) return;
            var trackedData = _this.trackData(data, mergeOption);
            if (succeededCallback) {
                if ((data instanceof Array) && (trackedData === data)) {
                    trackedData = Array.clone(trackedData);
                }
                succeededCallback(trackedData, userContext, operation);
            }
        }
        function fail(error) {
            if (_this._disposed) return;
            if (failedCallback) {
                failedCallback(error, userContext, operation);
            }
        }
        if (typeof(userContext) === "undefined") {
            userContext = null;
        }
        return (this.get_fetchDataMethod() || Sys.Data.DataContext._fetchWSP)
            (this, this.get_serviceUri(), operation, parameters || null, httpVerb || "POST", done, fail, timeout || 0, userContext);
    }
    function Sys$Data$DataContext$_clearData(newData) {
        if (this._useIdentity) {
            for (var identity in this._items) {
                var entity = this._items[identity];
                this._releaseEntity(entity);
            }
        }
        else if (this._lastResults) {
            this._release(this._lastResults);
        }
        this._items = {};
        var oldData = this._lastResults;
        this._lastResults = newData || null;
        this.clearChanges();
        if (newData) {
            this._capture(newData);
        }
        if (oldData !== null) {
            this._raiseChanged("lastFetchDataResults");
        }
    }
    function Sys$Data$DataContext$_combineParameters(params1, params2) {
        var x, params = {};
        for (x in params1) {
            params[x] = params1[x];
        }
        for (x in params2) {
            params[x] = params2[x];
        }
        return params;
    }
    function Sys$Data$DataContext$_fixAfterSave(change, entity, result) {
        if (this._useIdentity) {
            var oldIdentity = this.getIdentity(entity),
                newIdentity = this.getIdentity(result);
            this._combine(entity, result);
            if (oldIdentity !== newIdentity) {
                delete this._items[oldIdentity];
                this._items[newIdentity] = entity;
            }
        }
        else {
            this._combine(entity, result);
            if (change.action === Sys.NotifyCollectionChangedAction.add) {
                this._captureEntity(item);
            }
        }
    }
    function Sys$Data$DataContext$trackData(data, mergeOption) {
        /// <summary locid="M:J#fail" />
        /// <param name="data" mayBeNull="true"></param>
        /// <param name="mergeOption" type="Sys.Data.MergeOption" mayBeNull="true" optional="true"></param>
        /// <returns mayBeNull="true"></returns>
        var e = Function._validateParams(arguments, [
            {name: "data", mayBeNull: true},
            {name: "mergeOption", type: Sys.Data.MergeOption, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        if (this._useIdentity) {
            if ((typeof(mergeOption) === "undefined") || (mergeOption === null)) {
                mergeOption = this.get_mergeOption();
            }
            var trackedData;
            if (data instanceof Array) {
                data = this._storeEntities(data, mergeOption);
            }
            else if ((typeof(data) !== "undefined") && (data !== null)) {
                trackedData = this._storeEntities([data], mergeOption);
                if (trackedData.length === 0) {
                    data = null;
                }
            }
            var oldData = this._lastResults;
            this._lastResults = data;
            if (oldData !== null) {
                this._raiseChanged("lastFetchDataResults");
            }
        }
        else {
            this._clearData(data);
        }
        return data;
    }
    function Sys$Data$DataContext$_processResults(dataContext, changes, results) {
        if (results && results.length === changes.length) {
            dataContext._ignoreChange = true;
            try {
                for (var i = 0, l = results.length; i < l; i++) {
                    var result = results[i], change = changes[i], item = change.item;
                    if (result && typeof(result) === "object") {
                        dataContext._fixAfterSave(change, item, result);
                    }
                }
            }
            finally {
                dataContext._ignoreChange = false;
            }
        }
    }
    function Sys$Data$DataContext$_peekChange(changearray, entity, identity, remove) {
        if (!changearray) return false;
        if (identity !== null) {
            var key = "id$" + identity,
                i = changearray[key];
            if (i) {
                if (remove) {
                    changearray[key] = null;
                }
                return true;
            }
        }
        else {
            if (remove) {
                return Array.remove(changearray, entity);
            }
            else {
                return Array.contains(changearray, entity);
            }
        }
    }
    function Sys$Data$DataContext$_pushChange(changearray, entity, identity) {
        if (!changearray) {
            changearray = [];
        }
        if (identity === null) {
            changearray[changearray.length] = entity;
        }
        else {
            changearray["id$" + identity] = true;
        }
        return changearray;
    }
    function Sys$Data$DataContext$_registerChange(change) {
        Sys.Observer.add(this.get_changes(), change);
        if (!this._hasChanges) {
            this._hasChanges = true;
            this.raisePropertyChanged("hasChanges");
        }        
    }
    function Sys$Data$DataContext$saveChanges(succeededCallback, failedCallback, userContext) {
        /// <summary locid="M:J#Sys.Data.DataContext.saveChanges" />
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
        var delay = false,
            uri = this.get_serviceUri(),
            saveOperation = this.get_saveOperation(),
            _this = this,
            changes;
        function done(results) {
            if (_this._disposed) return;
            if (!delay) {
                delay = true;
                window.setTimeout(function() { done(results) }, 0);
            }
            else {
                _this.clearChanges();
                var processor = _this.get_handleSaveChangesResultsMethod();
                (processor || _this._processResults)(_this, changes, results);
                _this._saverequest = null;
                _this._saving = false;
                _this._raiseChanged("isSaving");
                if (succeededCallback) {
                    succeededCallback(results, userContext, saveOperation);
                }
            }
        }
        function failed(error) {
            if (_this._disposed) return;
            if (!delay) {
                delay = true;
                window.setTimeout(function() { failed(error) }, 0);
            }
            else {
                _this._saverequest = null;
                _this._saving = false;
                _this._raiseChanged("isSaving");
                if (failedCallback) {
                    failedCallback(error, userContext, saveOperation);
                }
            }
        }
        if (!this._hasChanges) {
            done(null);
            return null;
        }
        changes = Array.clone(this.get_changes());        
        if (changes.length === 0) {
            done(null);
            return null;
        }
        if (!uri) return;
        if (this.get_isSaving()) {
            this.abortSave();
        }
        this._saving = true;
        this._raiseChanged("isSaving");
        
        var filteredChanges = this._filterLinks(changes);
        this._saverequest = (this.get_saveChangesMethod() || this._saveInternal)(this, filteredChanges, done, failed, userContext);
        delay = true;
        return this._saverequest;
    }
    function Sys$Data$DataContext$_isDeleted(entity) {
        var i, l, change, changes = this.get_changes(), identity = this.getIdentity(entity);
        for (i = 0, l = changes.length; i < l; i++) {
            change = changes[i];
            if ((change.action === Sys.Data.ChangeOperationType.remove) && change.item &&
                ((change.item === entity) || (this.getIdentity(change.item) === identity)) ) {
                return true;
            }
        }
        return false;
    }
    function Sys$Data$DataContext$_removeChanges(entity, linkField) {
        var i, l, toRemove, change, changes = this.get_changes();
        for (i = 0, l = changes.length; i < l; i++) {
            change = changes[i];
            if ((linkField && (change.linkSource === entity) && (change.linkSourceField === linkField)) ||
                (!linkField && change.item && (typeof(change.item) === "object") &&
                    ((change.item === entity) || (this.getIdentity(change.item) === this.getIdentity(entity))))) {
                if (!toRemove) {
                    toRemove = [change];
                }
                else {
                    toRemove[toRemove.length] = change;
                }
            }
        }
        if (toRemove) {
            Sys.Observer.beginUpdate(changes);
            for (i = 0, l = toRemove.length; i < l; i++) {
                Sys.Observer.remove(changes, toRemove[i]);
            }
            Sys.Observer.endUpdate(changes);
            if (changes.length === 0) {
                this._hasChanges = false;
                this.raisePropertyChanged("hasChanges");
            }
        }
    }
    function Sys$Data$DataContext$_setLinkField(isArray, source, field, target, isRemove) {
        if (isArray) {
            var value = source[field];
            if (value === null || this._getValueType(source, field, value) !== 2) {
                if (isRemove) {
                    return false;
                }
                source[field] = value = [];
            }
            else if (!(value instanceof Array)) {
                throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.mustBeArray, field));
            }
            this._ignoreChange = true;
            try {
                if (Array.contains(value, target)) {
                    if (isRemove) {
                        Sys.Observer.remove(value, target);
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    if (isRemove) {
                        return false;
                    }
                    else {
                        Sys.Observer.add(value, target);
                        return true;
                    }
                }
            }
            finally {
                this._ignoreChange = false;
            }
        }
        else {
            this._ignoreChange = true;
            try {
                if (isRemove) {
                    Sys.Observer.setValue(source, field, null);
                }
                else {
                    Sys.Observer.setValue(source, field, target);
                }
                return true;
            }
            finally {
                this._ignoreChange = false;
            }
        }
    }
    function Sys$Data$DataContext$_toggleLink(source, field, target, removing) {
        var change, changes = this.get_changes();
        for (var i = 0, l = changes.length; i < l; i++) {
            change = changes[i];
            if ((change.linkSourceField === field) && (change.linkSource === source) &&
                ((change.linkTarget === target) || (change.action === Sys.Data.ChangeOperationType.update))) {
                Sys.Observer.remove(changes, change);
                var hadChange = this._hasChanges;
                this._hasChanges = !!changes.length;
                if (hadChange !== this._hasChanges) {
                    this.raisePropertyChanged("hasChanges");
                }
                return change;
            }
        }
        return null;
    }
    function Sys$Data$DataContext$updated() {
        /// <summary locid="M:J#Sys.Data.DataContext.updated" />
        if (arguments.length !== 0) throw Error.parameterCount();
        if (this._dirty) {
            this._dirty = false;
            this.raisePropertyChanged("");
        }
    }
    function Sys$Data$DataContext$_capture(data) {
        if (data instanceof Array) {
            for (var i = 0, l = data.length; i < l; i++) {
                this._captureEntity(data[i]);
            }
        }
        else if (data !== null) {
            this._captureEntity(data);
        }
    }
    function Sys$Data$DataContext$_captureEntity(item) {
        if (this._isCaptureable(item)) {
            Sys.Observer.addPropertyChanged(item, this._dataChangedDel);
        }
    }
    function Sys$Data$DataContext$_dataChanged(object, args) {
        if (this._ignoreChange) return;
        var changelist = this.get_changes();
        var identity = this.getIdentity(object);
        if (!this._peekChange(this._inserts, object, identity)) {
            var alreadyChanged = this._peekChange(this._edits, object, identity);
            if (!alreadyChanged) {
                Sys.Observer.add(changelist, new Sys.Data.ChangeOperation(Sys.Data.ChangeOperationType.update, object));
                this._edits = this._pushChange(this._edits, object, identity);
                if (!this._hasChanges) {
                    this._hasChanges = true;
                    this.raisePropertyChanged("hasChanges");
                }
            }
        }
    }
    function Sys$Data$DataContext$_isActive() {
        return this.get_isInitialized() && !this.get_isUpdating();
    }
    function Sys$Data$DataContext$_isCaptureable(item) {
        if (item === null) return false;
        var type = typeof(item);
        return (type === "object" || type === "unknown");
    }
    function Sys$Data$DataContext$_raiseChanged(name) {
        if (this._isActive()) {
            this.raisePropertyChanged(name);
            return true;
        }
        else {
            this._dirty = true;
            return false;
        }
    }
    function Sys$Data$DataContext$_release(data) {
        if (data instanceof Array) {
            for (var i = 0, l = data.length; i < l; i++) {
                this._releaseEntity(data[i]);
            }
        }
        else if (data !== null) {
            this._releaseEntity(data);
        }
    }
    function Sys$Data$DataContext$_releaseEntity(item) {
        if (this._isCaptureable(item)) {
            Sys.Observer.removePropertyChanged(item, this._dataChangedDel);
        }
    }
    function Sys$Data$DataContext$_saveInternal(dc, changes, succeededCallback, failedCallback, context) {
        if (!Type._checkDependency("MicrosoftAjaxWebServices.js")) {
            throw Error.invalidOperation(Sys.UI.TemplatesRes.requiresWebServices);
        }
        var parameters = dc.get_saveParameters();
        return Sys.Net.WebServiceProxy.invoke(dc.get_serviceUri(),
            dc.get_saveOperation() || "",
            dc.get_saveHttpVerb() === "GET",
            parameters ? dc._combineParameters(parameters, { changeSet: changes }) : { changeSet: changes },
            succeededCallback,
            failedCallback,
            context,
            dc.get_saveChangesTimeout() || 0);
    }
    function Sys$Data$DataContext$_filterLinks(changeSet) {
        if (!this._useIdentity) return changeSet;
        var i, l = changeSet.length,
            newChangeSet = new Array(l);
        for (i = 0; i < l; i++) {
            var change = changeSet[i], item = change.item, linkSource = change.linkSource, linkTarget = change.linkTarget;
            if (item) {
                item = this._getEntityOnly(item);
            }
            if (linkSource) {
                linkSource = this._getEntityOnly(linkSource);
            }
            if (linkTarget) {
                linkTarget = this._getEntityOnly(linkTarget);
            }
            newChangeSet[i] = new Sys.Data.ChangeOperation(change.action, item, linkSource, change.linkSourceField, linkTarget);
        }
        return newChangeSet;
    }
    function Sys$Data$DataContext$_getEntityOnly(source) {
        var target = {};
        this._combine(target, source, null, true);
        return target;
    }
    function Sys$Data$DataContext$getIdentity(entity) {
        /// <summary locid="M:J#failed" />
        /// <param name="entity" type="Object" mayBeNull="false"></param>
        /// <returns mayBeNull="true" type="String"></returns>
        var e = Function._validateParams(arguments, [
            {name: "entity", type: Object}
        ]);
        if (e) throw e;
        if (entity === null) return null;
        var getter = this.get_getIdentityMethod();
        return getter ? (getter(this, entity) || null) : null;
    }
    function Sys$Data$DataContext$isDeferredProperty(entity, propertyName) {
        /// <summary locid="M:J#failed" />
        /// <param name="entity" type="Object"></param>
        /// <param name="propertyName" type="String"></param>
        /// <returns type="Boolean"></returns>
        var e = Function._validateParams(arguments, [
            {name: "entity", type: Object},
            {name: "propertyName", type: String}
        ]);
        if (e) throw e;
        var getter = this.get_isDeferredPropertyMethod();
        return getter ? (getter(this, entity, propertyName) || false) : false;
    }
    function Sys$Data$DataContext$_getValueType(parent, name, object) {
        var type = typeof(object);
        if (type === "undefined") return 0;
        if ((object === null) || (type !== "object")) return 2;
        if (this.isDeferredProperty(parent, name)) return 1;
        return 2;
    }
    function Sys$Data$DataContext$_setField(target, name, source, value, mergeOption, isLink) {
        var doSet = true, isArray = (target instanceof Array), appendOnly = (mergeOption === Sys.Data.MergeOption.appendOnly);
        if (!isArray) {
            var targetField = target[name], valueType = this._getValueType(target, name, targetField);
            if (appendOnly) {
                if (valueType === 2) {
                    doSet = false;
                }
            }
            else if ((valueType === 2) && value && source && (this._getValueType(source, name, value) === 1)) {
                doSet = false;
            }
        }
        if (doSet) {
            if (isArray) {
                target[name] = value;
            }
            else {
                this._ignoreChange = true;
                try {
                    Sys.Observer.setValue(target, name, value);
                }
                finally {
                    this._ignoreChange = false;
                }
            }
            if (isLink && !appendOnly) {
                this._removeChanges(target, name);
            }
        }
        return doSet;
    }
    function Sys$Data$DataContext$_combine(target, source, mergeOption, excludeEntities) {
        var removedChanges = false;
        for (var name in source) {
            var field = source[name], type = typeof(field);
            if (type === "function") continue;
            if (this._useIdentity && (field instanceof Array)) {
                if (!excludeEntities) {
                    field = this._storeEntities(field, mergeOption);
                    if (target) {
                        this._setField(target, name, source, field, mergeOption, true);
                    }
                }
            }
            else {
                var identity = null;
                if (field && (type === "object"))  {
                    identity = this.getIdentity(field);
                }
                if (identity !== null) {
                    if (!excludeEntities) {
                        this._storeEntity(identity, field, target, name, source, mergeOption);
                    }
                }
                else if (target) {
                    var targetField = target[name];
                    if (targetField && (typeof(targetField) === "object") && this.getIdentity(targetField)) {
                        continue;
                    }
                    if (this._setField(target, name, source, field, mergeOption) && !removedChanges &&
                        ((typeof(mergeOption) !== "number") || (mergeOption === Sys.Data.MergeOption.overwriteChanges))) {
                        removedChanges = true;
                        this._removeChanges(target);
                    }
                }
            }
        }
    }
    function Sys$Data$DataContext$_storeEntity(identity, entity, parent, parentField, source, mergeOption) {
        var updated = true, storedEntity = this._items[identity];
        if ((typeof (storedEntity) !== "undefined")) {
            if (storedEntity === entity) {
                updated = false;
            }
            else {
                this._combine(storedEntity, entity, mergeOption);
            }
        }
        else {
            this._items[identity] = storedEntity = entity;
            this._captureEntity(entity);
            this._combine(null, entity, mergeOption);
        }
        if (parent && (parent[parentField] !== storedEntity)) {
            this._setField(parent, parentField, source, storedEntity, mergeOption, true);
        }
        return updated;
    }
    function Sys$Data$DataContext$_storeEntities(entities, mergeOption) {
        var i, l, filtered, deleted, appendOnly = (mergeOption === Sys.Data.MergeOption.appendOnly);
        for (i = 0, l = entities.length; i < l; i++) {
            var entity = entities[i], isObject = (entity && (typeof(entity) === "object"));
            if (isObject) {
                if (appendOnly) {
                    if (this._isDeleted(entity)) {
                        if (!deleted) {
                            deleted = [entity]
                        }
                        else {
                            deleted[deleted.length] = entity;
                        }
                        continue;
                    }
                }
                var identity = this.getIdentity(entity);
                if (identity !== null) {
                    if (this._storeEntity(identity, entity, entities, i, null, mergeOption) && !appendOnly) {
                        this._removeChanges(entity);
                    }
                }
            }
        }
        if (deleted) {
            filtered = Array.clone(entities);
            for (i = 0, l = deleted.length; i < l; i++) {
                Array.remove(filtered, deleted[i]);
            }
        }
        return filtered || entities;
    }
Sys.Data.DataContext.prototype = {
    _useIdentity: false,
    _dirty: false,
    _lastResults: null,
    _items: null,
    _ignoreChange: false,
    _inserts: null,
    _edits: null,
    _deletes: null,
    _changelist: null,
    _hasChanges: false,
    _mergeOption: Sys.Data.MergeOption.overwriteChanges,
    _saverequest: null,
    _saving: false,
    _serviceUri: null,
    _saveOperation: null,
    _saveParameters: null,
    _saveHttpVerb: null,
    _saveTimeout: 0,
    _methods: null,
    get_changes: Sys$Data$DataContext$get_changes,
    get_createEntityMethod: Sys$Data$DataContext$get_createEntityMethod,
    set_createEntityMethod: Sys$Data$DataContext$set_createEntityMethod,
    get_getIdentityMethod: Sys$Data$DataContext$get_getIdentityMethod,
    set_getIdentityMethod: Sys$Data$DataContext$set_getIdentityMethod,
    get_handleSaveChangesResultsMethod: Sys$Data$DataContext$get_handleSaveChangesResultsMethod,
    set_handleSaveChangesResultsMethod: Sys$Data$DataContext$set_handleSaveChangesResultsMethod,
    get_isDeferredPropertyMethod: Sys$Data$DataContext$get_isDeferredPropertyMethod,
    set_isDeferredPropertyMethod: Sys$Data$DataContext$set_isDeferredPropertyMethod,
    get_getNewIdentityMethod: Sys$Data$DataContext$get_getNewIdentityMethod,
    set_getNewIdentityMethod: Sys$Data$DataContext$set_getNewIdentityMethod,
    get_getDeferredPropertyFetchOperationMethod: Sys$Data$DataContext$get_getDeferredPropertyFetchOperationMethod,
    set_getDeferredPropertyFetchOperationMethod: Sys$Data$DataContext$set_getDeferredPropertyFetchOperationMethod,
    get_items: Sys$Data$DataContext$get_items,
    get_lastFetchDataResults: Sys$Data$DataContext$get_lastFetchDataResults,
    get_hasChanges: Sys$Data$DataContext$get_hasChanges,
    get_fetchDataMethod: Sys$Data$DataContext$get_fetchDataMethod,
    set_fetchDataMethod: Sys$Data$DataContext$set_fetchDataMethod,
    get_mergeOption: Sys$Data$DataContext$get_mergeOption,
    set_mergeOption: Sys$Data$DataContext$set_mergeOption,
    get_saveChangesMethod: Sys$Data$DataContext$get_saveChangesMethod,
    set_saveChangesMethod: Sys$Data$DataContext$set_saveChangesMethod,
    get_saveOperation: Sys$Data$DataContext$get_saveOperation,
    set_saveOperation: Sys$Data$DataContext$set_saveOperation,
    get_saveHttpVerb: Sys$Data$DataContext$get_saveHttpVerb,
    set_saveHttpVerb: Sys$Data$DataContext$set_saveHttpVerb,
    get_saveParameters: Sys$Data$DataContext$get_saveParameters,
    set_saveParameters: Sys$Data$DataContext$set_saveParameters,    
    get_saveChangesTimeout: Sys$Data$DataContext$get_saveChangesTimeout,
    set_saveChangesTimeout: Sys$Data$DataContext$set_saveChangesTimeout,        
    get_isSaving: Sys$Data$DataContext$get_isSaving,
    get_serviceUri: Sys$Data$DataContext$get_serviceUri,
    set_serviceUri: Sys$Data$DataContext$set_serviceUri,
    addLink: Sys$Data$DataContext$addLink,
    removeLink: Sys$Data$DataContext$removeLink,
    setLink: Sys$Data$DataContext$setLink,    
    abortSave: Sys$Data$DataContext$abortSave,
    clearChanges: Sys$Data$DataContext$clearChanges,
    clearData: Sys$Data$DataContext$clearData,
    createEntity: Sys$Data$DataContext$createEntity,
    dispose: Sys$Data$DataContext$dispose,
    initialize: Sys$Data$DataContext$initialize,
    fetchDeferredProperty: Sys$Data$DataContext$fetchDeferredProperty,
    getNewIdentity: Sys$Data$DataContext$getNewIdentity,
    insertEntity: Sys$Data$DataContext$insertEntity,
    removeEntity: Sys$Data$DataContext$removeEntity,
    fetchData: Sys$Data$DataContext$fetchData,
    _clearData: Sys$Data$DataContext$_clearData,
    _combineParameters: Sys$Data$DataContext$_combineParameters,
    _fixAfterSave: Sys$Data$DataContext$_fixAfterSave,
    trackData: Sys$Data$DataContext$trackData,
    _processResults: Sys$Data$DataContext$_processResults,
    _peekChange: Sys$Data$DataContext$_peekChange,
    _pushChange: Sys$Data$DataContext$_pushChange,
    _registerChange: Sys$Data$DataContext$_registerChange,
    saveChanges: Sys$Data$DataContext$saveChanges,
    _isDeleted: Sys$Data$DataContext$_isDeleted,
    _removeChanges: Sys$Data$DataContext$_removeChanges,
    _setLinkField: Sys$Data$DataContext$_setLinkField,
    _toggleLink: Sys$Data$DataContext$_toggleLink,
    updated: Sys$Data$DataContext$updated,
    _capture: Sys$Data$DataContext$_capture,
    _captureEntity: Sys$Data$DataContext$_captureEntity,
    _dataChanged: Sys$Data$DataContext$_dataChanged,
    _isActive: Sys$Data$DataContext$_isActive,
    _isCaptureable: Sys$Data$DataContext$_isCaptureable,
    _raiseChanged: Sys$Data$DataContext$_raiseChanged,
    _release: Sys$Data$DataContext$_release,
    _releaseEntity: Sys$Data$DataContext$_releaseEntity,
    _saveInternal: Sys$Data$DataContext$_saveInternal,
    _filterLinks: Sys$Data$DataContext$_filterLinks,
    _getEntityOnly: Sys$Data$DataContext$_getEntityOnly,
    getIdentity: Sys$Data$DataContext$getIdentity,
    isDeferredProperty: Sys$Data$DataContext$isDeferredProperty,
    _getValueType: Sys$Data$DataContext$_getValueType,
    _setField: Sys$Data$DataContext$_setField,
    _combine: Sys$Data$DataContext$_combine,
    _storeEntity: Sys$Data$DataContext$_storeEntity,
    _storeEntities: Sys$Data$DataContext$_storeEntities
}
Sys.Data.DataContext.registerClass("Sys.Data.DataContext", Sys.Component, Sys.Data.IDataProvider);
Sys.Data.DataContext._fetchWSP = function Sys$Data$DataContext$_fetchWSP(dataContext, uri, operation, parameters, httpVerb, succeededCallback, failedCallback, timeout, context) {
    if (!Type._checkDependency("MicrosoftAjaxWebServices.js")) {
        throw Error.invalidOperation(Sys.UI.TemplatesRes.requiresWebServices);
    }
    if (!uri) {
        throw Error.invalidOperation(Sys.UI.TemplatesRes.requiredUri);
    }
    if (!operation) {
        throw Error.argumentNull("operation");
    }
    return Sys.Net.WebServiceProxy.invoke(
        uri, operation,
        httpVerb === "GET", parameters, succeededCallback,
        failedCallback, context, timeout);
}
Sys.Data.DataEventArgs = function Sys$Data$DataEventArgs(data) {
    /// <summary locid="M:J#Sys.Data.DataEventArgs.#ctor" />
    /// <param name="data" mayBeNull="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "data", mayBeNull: true}
    ]);
    if (e) throw e;
    this._data = data;
    Sys.Data.DataEventArgs.initializeBase(this);
}
    function Sys$Data$DataEventArgs$get_data() {
        /// <value mayBeNull="true" locid="P:J#Sys.Data.DataEventArgs.data"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        var d = this._data;
        return (typeof(d) === "undefined" ? null : d);
    }
    function Sys$Data$DataEventArgs$set_data(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        this._data = value;
    }
Sys.Data.DataEventArgs.prototype = {
    get_data: Sys$Data$DataEventArgs$get_data,
    set_data: Sys$Data$DataEventArgs$set_data
};
Sys.Data.DataEventArgs.registerClass("Sys.Data.DataEventArgs", Sys.CancelEventArgs);
Sys.Data.ChangeOperationType = function Sys$Data$ChangeOperationType() {
    /// <summary locid="M:J#Sys.Data.ChangeOperationType.#ctor" />
    /// <field name="insert" type="Number" integer="true" static="true" locid="F:J#Sys.Data.ChangeOperationType.insert"></field>
    /// <field name="update" type="Number" integer="true" static="true" locid="F:J#Sys.Data.ChangeOperationType.update"></field>
    /// <field name="remove" type="Number" integer="true" static="true" locid="F:J#Sys.Data.ChangeOperationType.remove"></field>
    if (arguments.length !== 0) throw Error.parameterCount();
    throw Error.notImplemented();
}
Sys.Data.ChangeOperationType.prototype = {
    insert: 0,
    update: 1,
    remove: 2
}
Sys.Data.ChangeOperationType.registerEnum("Sys.Data.ChangeOperationType");
Sys.Data.ChangeOperation = function Sys$Data$ChangeOperation(action, item, linkSource, linkSourceField, linkTarget) {
    /// <summary locid="M:J#Sys.Data.ChangeOperation.#ctor" />
    /// <param name="action" type="Sys.Data.ChangeOperationType"></param>
    /// <param name="item" mayBeNull="true"></param>
    /// <param name="linkSource" mayBeNull="true" optional="true"></param>
    /// <param name="linkSourceField" mayBeNull="true" optional="true"></param>
    /// <param name="linkTarget" mayBeNull="true" optional="true"></param>
    /// <field name="action" type="Sys.Data.ChangeOperationType" locid="F:J#Sys.Data.ChangeOperation.action"></field>
    /// <field name="item" mayBeNull="true" locid="F:J#Sys.Data.ChangeOperation.item"></field>
    /// <field name="linkSource" mayBeNull="true" locid="F:J#Sys.Data.ChangeOperation.linkSource"></field>
    /// <field name="linkSourceField" mayBeNull="true" locid="F:J#Sys.Data.ChangeOperation.linkSourceField"></field>
    /// <field name="linkTarget" mayBeNull="true" locid="F:J#Sys.Data.ChangeOperation.linkTarget"></field>
    var e = Function._validateParams(arguments, [
        {name: "action", type: Sys.Data.ChangeOperationType},
        {name: "item", mayBeNull: true},
        {name: "linkSource", mayBeNull: true, optional: true},
        {name: "linkSourceField", mayBeNull: true, optional: true},
        {name: "linkTarget", mayBeNull: true, optional: true}
    ]);
    if (e) throw e;
    this.action = action;
    this.item = item;
    this.linkSourceField = linkSourceField;
    this.linkSource = linkSource;
    this.linkTarget = linkTarget;
}
Sys.Data.ChangeOperation.prototype = {
    action: null,
    item: null,
    linkSource: null,
    linkSourceField: null,
    linkTarget: null
}
Sys.Data.ChangeOperation.registerClass("Sys.Data.ChangeOperation");
Sys.Data.AdoNetDataContext = function Sys$Data$AdoNetDataContext() {
    Sys.Data.AdoNetDataContext.initializeBase(this);
    this.set_getIdentityMethod(this._getIdentity);
    this.set_getNewIdentityMethod(this._getNewIdentity);
    this.set_fetchDataMethod(this._fetchAdoNet);
    this.set_saveChangesMethod(this._saveAdoNet);
    this.set_createEntityMethod(this._createEntity);
    this.set_handleSaveChangesResultsMethod(this._processResultsAdoNet);
    this.set_getDeferredPropertyFetchOperationMethod(this._getDeferredQuery);
    this.set_isDeferredPropertyMethod(this._isDeferred);
}
    function Sys$Data$AdoNetDataContext$_createEntity(dataContext, entitySetName) {
        var obj = {};
        dataContext._createMetaData(obj, entitySetName);
        return obj;
    }
    function Sys$Data$AdoNetDataContext$_fetchAdoNet(dataContext, uri, operation, parameters, httpVerb, succeededCallback, failedCallback, timeout, context) {
        if (operation) {
            if (typeof(operation) !== "string") {
                operation = operation.toString();
            }
            var i = operation.indexOf(":");
            if ((i !== -1) && (i < operation.indexOf("/"))) {
                uri = operation;
            }
        }
        var proxy = dataContext._getProxy(uri || "");
        return proxy.fetchData(operation, parameters || null, null, httpVerb || null, succeededCallback || null, failedCallback || null, timeout || 0, context || null);
    }
    function Sys$Data$AdoNetDataContext$_getDeferredQuery(dataContext, entity, propertyName, userContext) {
        var uri = null, value = entity[propertyName];
        if ((value === null) || (typeof(value) === "undefined") || (value instanceof Array)) {
            uri = dataContext.getIdentity(entity);
            uri += (uri.endsWith("/") ? propertyName : ("/" + propertyName));
        }
        else if (typeof(value) === "object") {
            uri = dataContext.getIdentity(value);
            if (!uri) {
                uri = value.__deferred ? value.__deferred.uri : null;
            }
        }
        if (!uri) {
            throw Error.invalidOperation(String.format(Sys.Data.AdoNetRes.propertyNotFound, propertyName));
        }
        return new Sys.Net.WebServiceOperation(uri);
    }
    function Sys$Data$AdoNetDataContext$_getProxy(uri) {
        if (this._puri !== uri) {
            if (!Type._checkDependency("MicrosoftAjaxAdoNet.js")) {
                throw Error.invalidOperation(Sys.UI.TemplatesRes.requiresAdoNetProxy);
            }
            this._proxy = new Sys.Data.AdoNetServiceProxy(uri);
            this._puri = uri;
        }
        return this._proxy;
    }
    function Sys$Data$AdoNetDataContext$_isDeferred(dataContext, entity, propertyName) {
        var value = entity[propertyName];
        return !!(value && (typeof(value) === "object") && value.__deferred);
    }
    function Sys$Data$AdoNetDataContext$_processResultsAdoNet(dataContext, changes, results) {
        if (results && (results.length === changes.length)) {
            for (i = 0, l = results.length; i < l; i++) {
                var change = changes[i], item = change.item,
                    result = results[i], data = result.get_result(),
                    headers = result.get_httpHeaders();
                if (item) {
                    if (data) {
                        dataContext._fixAfterSave(change, item, data);
                    }
                    if (headers.ETag && item.__metadata) {
                        item.__metadata.etag = headers.ETag;
                    }
                }
            }
        }
    }
    function Sys$Data$AdoNetDataContext$_getBatchReference(item, batchField, batchRefPrefix, stripUri) {
        var batchnum = item.__metadata[batchField];
        if (typeof(batchnum) === "number") {
            return batchRefPrefix + "$" + batchnum;
        }
        else {
            var uri = this.getIdentity(item);
            if (!uri) {
                throw Error.invalidOperation(Sys.Data.AdoNetRes.batchLinkBeforeInsert);
            }
            if (stripUri) {
                uri = uri.substr(uri.lastIndexOf("/"));
            }
            return uri;
        }
    }
    function Sys$Data$AdoNetDataContext$_saveAdoNet(dataContext, changes, succeededCallback, failedCallback, context) {
        var i, l, uri = dataContext.get_serviceUri(),
            proxy = dataContext._getProxy(uri),
            sequence = proxy.createActionSequence(),
            batchField = ("__batchNumber" + dataContext._saveCounter++);
        proxy.set_timeout(dataContext.get_saveChangesTimeout());
        for (i = 0, l = changes.length; i < l; i++) {
            var change = changes[i],
                item = change.item;
            switch(change.action) {
                case Sys.Data.ChangeOperationType.insert:
                    if (item) {
                        var originalItem = dataContext.get_items()[dataContext.getIdentity(item)];
                        delete item.__metadata;
                        originalItem.__metadata[batchField] = i;
                        sequence.addInsertAction(item, originalItem.__metadata.entitySet);
                    }
                    else {
                        sequence.addInsertAction({ uri: dataContext._getBatchReference(change.linkTarget, batchField, "") },
                            dataContext._getBatchReference(change.linkSource, batchField, "/") + "/$links/" + change.linkSourceField);
                    }
                    break;
                case Sys.Data.ChangeOperationType.update:
                    if (item) {
                        sequence.addUpdateAction(item);
                    }
                    else {
                        if (change.linkTarget) {
                            sequence.addUpdateAction({ uri: dataContext._getBatchReference(change.linkTarget, batchField, "") },
                                dataContext._getBatchReference(change.linkSource, batchField, "/") + "/$links/" + change.linkSourceField);
                        }
                        else {
                            sequence.addRemoveAction(
                                {__metadata: { uri:
                                    dataContext._getBatchReference(change.linkSource, batchField, "/") + "/$links/" + change.linkSourceField }});
                        }
                    }
                    break;
                case Sys.Data.ChangeOperationType.remove:                
                    if (item) {
                        sequence.addRemoveAction(item);
                    }
                    else {
                        sequence.addRemoveAction(
                            {__metadata: { uri: dataContext._getBatchReference(change.linkSource, batchField, "/") + "/$links" +
                            dataContext._getBatchReference(change.linkTarget, batchField, "/", true) }});
                    }
                    break;
            }
        }
        return sequence.execute(succeededCallback, failedCallback, context);
    }
    function Sys$Data$AdoNetDataContext$_createMetaData(entity, entitySetName) {
        entity.__metadata = { entitySet: entitySetName, uri: entitySetName + "(__new" + this._entityCounter++ + ")" };
    }
    function Sys$Data$AdoNetDataContext$_getNewIdentity(dataContext, entity, entitySetName) {
        if (!entitySetName) {
            throw Error.invalidOperation(Sys.Data.AdoNetRes.entityWithNoResourceSet);
        }
        dataContext._createMetaData(entity, entitySetName);
        return entity.__metadata.uri;
    }
    function Sys$Data$AdoNetDataContext$_getIdentity(dataContext, entity) {
        var metadata = entity.__metadata;
        if (metadata) {
            return metadata.uri || null;
        }
        return null;
    }
Sys.Data.AdoNetDataContext.prototype = {
    _proxy: null,
    _puri: null,
    _entityCounter: 0,
    _saveCounter: 1,
    _createEntity: Sys$Data$AdoNetDataContext$_createEntity,
    _fetchAdoNet: Sys$Data$AdoNetDataContext$_fetchAdoNet,
    _getDeferredQuery: Sys$Data$AdoNetDataContext$_getDeferredQuery,
    _getProxy: Sys$Data$AdoNetDataContext$_getProxy,
    _isDeferred: Sys$Data$AdoNetDataContext$_isDeferred,
    _processResultsAdoNet: Sys$Data$AdoNetDataContext$_processResultsAdoNet,
    _getBatchReference: Sys$Data$AdoNetDataContext$_getBatchReference,
    _saveAdoNet: Sys$Data$AdoNetDataContext$_saveAdoNet,
    _createMetaData: Sys$Data$AdoNetDataContext$_createMetaData,
    _getNewIdentity: Sys$Data$AdoNetDataContext$_getNewIdentity,
    _getIdentity: Sys$Data$AdoNetDataContext$_getIdentity
}
Sys.Data.AdoNetDataContext.registerClass("Sys.Data.AdoNetDataContext", Sys.Data.DataContext);
Type.registerNamespace("Sys.UI");
Sys.UI.DomElement._oldGetElementById = Sys.UI.DomElement.getElementById;
Sys.UI.DomElement.getElementById = function Sys$UI$DomElement$getElementById(id, element) {
    /// <summary locid="M:J#Sys.UI.DomElement.getElementById" />
    /// <param name="id" type="String"></param>
    /// <param name="element" domElement="true" optional="true" mayBeNull="true"></param>
    /// <returns domElement="true" mayBeNull="true"></returns>
    var e = Function._validateParams(arguments, [
        {name: "id", type: String},
        {name: "element", mayBeNull: true, domElement: true, optional: true}
    ]);
    if (e) throw e;
    var el = Sys.UI.DomElement._oldGetElementById(id, element);
    if (!el && !element && Sys.UI.Template._contexts.length) {
        var contexts = Sys.UI.Template._contexts;
        for (var i = 0, l = contexts.length; i < l; i++) {
            var context = contexts[i];
            for (var j = 0, m = context.length; j < m; j++) {
                var c = context[j];
                if (c.nodeType === 1) {
                    if (c.id === id) return c;
                    el = Sys.UI.DomElement._oldGetElementById(id, c);
                    if (el) return el;
                }
            }
        }
    }
    return el;
}
if ($get === Sys.UI.DomElement._oldGetElementById) {
    $get = Sys.UI.DomElement.getElementById;
}
Sys.Application.registerMarkupExtension = function Sys$Application$registerMarkupExtension(extensionName, extension, isExpression) {
    /// <summary locid="M:J#Sys.Application.registerMarkupExtension" />
    /// <param name="extensionName" type="String"></param>
    /// <param name="extension" type="Function"></param>
    /// <param name="isExpression" type="Boolean" optional="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "extensionName", type: String},
        {name: "extension", type: Function},
        {name: "isExpression", type: Boolean, optional: true}
    ]);
    if (e) throw e;
    if (!this._extensions) {
        this._extensions = {};
    }
    isExpression = ((typeof (isExpression) === "undefined") || (isExpression === true));
    this._extensions[extensionName] = { expression: isExpression, extension: extension };
}
Sys.Application._getMarkupExtension = function Sys$Application$_getMarkupExtension(name) {
    var extension = this._extensions ? this._extensions[name] : null;
    if (!extension) {
        throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.cannotFindMarkupExtension, name));
    }
    return extension;
}
Sys.Application._caseIndex = {};
Sys.Application._prototypeIndex = {};
Sys.Application._indexOf = function Sys$Application$_indexOf(array, item) {
    for (var i = 0, l = array.length; i < l; i++) {
        if (array[i] === item) {
            return i;
        }
    }
    return -1;
}
Sys.Application.activateElement = function Sys$Application$activateElement(element, bindingContext, recursive) {
    /// <summary locid="M:J#Sys.Application.activateElement" />
    /// <param name="element" domElement="true"></param>
    /// <param name="bindingContext" optional="true" mayBeNull="true"></param>
    /// <param name="recursive" optional="true" mayBeNull="false"></param>
    /// <returns type="Array" elementType="Sys.Component"></returns>
    var e = Function._validateParams(arguments, [
        {name: "element", domElement: true},
        {name: "bindingContext", mayBeNull: true, optional: true},
        {name: "recursive", optional: true}
    ]);
    if (e) throw e;
    var context = { userContext: bindingContext, localContext: {} };
    return Sys.Application._activateElementWithMappings(
        Sys.Application._getNamespaceMappings(null, [element]),
        null, null, element, context, recursive);
}
Sys.Application.activateElements = function Sys$Application$activateElements(elements, bindingContext, recursive) {
    /// <summary locid="M:J#Sys.Application.activateElements" />
    /// <param name="elements" type="Array" elementDomElement="true"></param>
    /// <param name="bindingContext" optional="true" mayBeNull="true"></param>
    /// <param name="recursive" optional="true" mayBeNull="false"></param>
    /// <returns type="Array" elementType="Sys.Component"></returns>
    var e = Function._validateParams(arguments, [
        {name: "elements", type: Array, elementDomElement: true},
        {name: "bindingContext", mayBeNull: true, optional: true},
        {name: "recursive", optional: true}
    ]);
    if (e) throw e;
    return Sys.Application._activateElements(elements, null, null, bindingContext, recursive);
}
Sys.Application._activateElements = function Sys$Application$_activateElements(elements, elementIds, namespaceMappings, context, recursive) {
    var element, components = [];
    context = { userContext: context, localContext: {} };
    for (var i = 0, l = elements.length; i < l; i++) {
        element = elements[i];
        if (element.nodeType !== 1) continue;
        Array.addRange(components,
            Sys.Application._activateElementWithMappings(namespaceMappings || Sys.Application._getNamespaceMappings(null, [element]),
            elements, elementIds, element, context, recursive));
        element.__msajaxactivated = true;
    }
    return components;
}
Sys.Application._activateElementWithMappings = function Sys$Application$_activateElementWithMappings(namespaceMappings, elements, elementIds, element, context, recursive) {
    var i, l, components = [], useDirect = Sys.Browser.agent === Sys.Browser.InternetExplorer;
    if (!element.__msajaxactivated) {
        Sys.Application._activateElementInternal(useDirect, element, namespaceMappings, components, context);
        if (recursive || (typeof(recursive) === "undefined")) {
            if (!Sys.UI.Template._isTemplate(element)) {
                var allElements = element.getElementsByTagName("*");
                for (i = 0, l = allElements.length; i < l; i++) {
                    var node = allElements[i], skip = node.__msajaxactivated;
                    if (!skip) {
                        if (elementIds) {
                            if (node.id && Sys.Application._indexOf(elementIds, node.id) !== -1) {
                                skip = true;
                            }
                        }
                        else if (elements && Sys.Application._indexOf(elements, node) !== -1) {
                            skip = true;
                        }
                    }
                    if (!skip) {
                        Sys.Application._activateElementInternal(useDirect, node, namespaceMappings, components, context);
                    }
                    if (skip || Sys.UI.Template._isTemplate(node)) {
                        var next = node.nextSibling;
                        while (next && (next.nodeType !== 1)) {
                            next = next.nextSibling;
                        }
                        while (!next) {                        
                            node = node.parentNode;
                            if (node === element) {
                                break;
                            }
                            next = node.nextSibling;
                            while (next && (next.nodeType !== 1)) {
                                next = next.nextSibling;
                            }
                        }
                        if (!next || (next.nodeType !== 1)) {
                            break;
                        }
                        do {
                            node = allElements[i+1];
                            if (node === next) break;
                            i++;
                        }
                        while (i < l);
                    }
                }
            }
        }
    }    
    for (i = components.length - 1; i > -1; i--) {
        var component = components[i];
        if (Sys.Component.isInstanceOfType(component)) {
            component.endUpdate();
        }
    }
    return components;
}
Sys.Application._activateElementInternal = function Sys$Application$_activateElementInternal(useDirect, element, namespaceMappings, components, context) {
    var i, l, instance, types = null, key = null;
    try {
        types = useDirect ? element[namespaceMappings.types] : element.getAttribute(namespaceMappings.types);
    }
    catch (err) {
    }
    try {
        key = useDirect ? element[namespaceMappings.sysKey] : element.getAttribute(namespaceMappings.sysKey);
    }
    catch (err) {
    }
    if (key) {
        context.localContext[key] = element;
    }
    if (types) {
        element.__msajaxactivated = true;
        var typeList = types.split(',');
        var index = {},
            localComponents = [];
        for (i = 0, l = typeList.length; i < l; i++) {
            var typeName = typeList[i].trim();
            if (index[typeName]) continue; 
            var type = namespaceMappings.namespaces[typeName];
            if (!type) {
                throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.invalidAttach, namespaceMappings.types, typeName));
            }
            var isComponent = type.inheritsFrom(Sys.Component),
                isControlOrBehavior = isComponent && (type.inheritsFrom(Sys.UI.Behavior) || type.inheritsFrom(Sys.UI.Control));
            instance = isControlOrBehavior ? new type(element) : new type();
            if (isComponent) {
                localComponents.push(instance);
                instance.beginUpdate();
            }
            if (!isControlOrBehavior) {
                Sys.Application._registerComponent(element, instance);
            }
            index[typeName] = { instance: instance, typeName: typeName, type: type};
            components.push(instance);
            var sysKey = null;
            try {
                sysKey = useDirect ? element[typeName + ":sys-key"] : element.getAttribute(typeName + ":sys-key");
            }
            catch (err) {
            }
            if (sysKey) {
                context.localContext[sysKey] = instance;
            }
        }
        for (i = 0, l = element.attributes.length; i < l; i++) {
            var attribute = element.attributes[i];
            if (!attribute.specified) continue;
            var nodeName = attribute.nodeName;
            if ((nodeName === namespaceMappings.sysKey) || (nodeName === namespaceMappings.types)) continue;
            var attrib = Sys.Application._splitAttribute(nodeName),
                ns = attrib.ns;
            if (!ns) continue;
            var entry = index[ns];
            if (!entry) continue;
            if (attrib.name !== "sys-key") {
                Sys.Application._setProperty(entry.instance, entry.type, attrib.name, attribute.nodeValue, context);
            }
        }
        var app = Sys.Application, creatingComponents = app.get_isCreatingComponents();
        for (i = 0, l = localComponents.length; i < l; i++) {
            instance = localComponents[i];
            if (instance.get_id()) {
                app.addComponent(instance);
            }
            if (creatingComponents) {
                app._createdComponents[app._createdComponents.length] = instance;
            }
        }
    }
    var command = Sys.Application._getCommandProperties(useDirect, namespaceMappings.sysCommand, element, context);
    if (command) {
        var commandArg = Sys.Application._getCommandProperties(useDirect, namespaceMappings.sysCommandArgument, element, context), 
            commandTarget = Sys.Application._getCommandProperties(useDirect, namespaceMappings.sysCommandTarget, element, context);
        Sys.UI.DomEvent.addHandler(element, 'click', Sys.UI.Template._getCommandHandler(command, commandArg, commandTarget));
    }
}
Sys.Application._getCommandProperties = function Sys$Application$_getCommandProperties(useDirect, namespaceMapping, element, context) {
    var property = null;
    try {
        property = useDirect ? element[namespaceMapping] : element.getAttribute(namespaceMapping);
    }
    catch (err) {
    }
    return property ? Sys.Application._getPropertyValue(null, null, property, context, null, true) : null;
}
Sys.Application._splitAttribute = function Sys$Application$_splitAttribute(attributeName) {
    var nameParts = attributeName.split(':'),
            ns = nameParts.length > 1 ? nameParts[0] : null,
            name = nameParts[ns ? 1 : 0];
    return { ns: ns, name: name };
}
Sys.Application._getBodyNamespaceMapping = function Sys$Application$_getBodyNamespaceMapping() {
    if (Sys.Application._bodyNamespaceMapping) {
        return Sys.Application._bodyNamespaceMapping;
    }
    var namespaceMapping = {
        sysNamespace: "sys", types: "sys:attach", sysId: "sys:id", sysKey: "sys:key",
        sysActivate: "sys:activate", sysChecked: "sys:checked", styleNamespace: "style",
        classNamespace: "class", namespaces: {},
        sysCommandArgument: "sys:commandargument", sysCommand: "sys:command", sysCommandTarget: "sys:commandtarget",
        codeNamespace: "code", codeIf: "code:if", codeBefore: "code:before", codeAfter: "code:after"
    };
    Sys.Application._getNamespaceMapping(namespaceMapping, document.body);
    Sys.Application._bodyNamespaceMapping = namespaceMapping;
    return namespaceMapping;
}
Sys.Application._getNamespaceMappings = function Sys$Application$_getNamespaceMappings(existingMapping, elements) {
    var namespaceMappings = existingMapping || Sys.Application._getBodyNamespaceMapping();
    for (var i = 0, l = elements.length; i < l; i++) {
        Sys.Application._getNamespaceMapping(namespaceMappings, elements[i]);
    }
    return namespaceMappings;
}
Sys.Application._getNamespaceMapping = function Sys$Application$_getNamespaceMapping(namespaceMapping, element) {
    var attributes = element.attributes;
    for (var i = 0, l = attributes.length; i < l; i++) {
        var attribute = attributes[i];
        if (!attribute.specified) continue;
        var attrib = Sys.Application._splitAttribute(attribute.nodeName);
        if (attrib.ns !== "xmlns") continue;
        var name = attrib.name;
        var value = attribute.nodeValue.trim();
        if (value.toLowerCase().startsWith("javascript:")) {
            value = value.substr(11).trimStart();
            if (value === "Sys") {
                with(namespaceMapping) {
                    sysNamespace = name;
                    types = name + ":attach";
                    sysId = name + ":id";
                    sysChecked = name + ":checked";
                    sysActivate = name + ":activate";
                    sysKey = name + ":key";
                    sysCommandArgument = name + ":commandargument";
                    sysCommand = name + ":command";
                    sysCommandTarget = name + ":commandtarget";
                }
            }
            else {
                try {
                    namespaceMapping.namespaces[name] = Type.parse(value);
                }
                catch(e) {
                    throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.invalidTypeNamespace, value));
                }
            }
        }
        else if (value === "http://schemas.microsoft.com/aspnet/style") {
            namespaceMapping.styleNamespace = name;
        }
        else if (value === "http://schemas.microsoft.com/aspnet/class") {
            namespaceMapping.classNamespace = name;
        }
        else if (value === "http://schemas.microsoft.com/aspnet/code") {
            with(namespaceMapping) {
                codeNamespace = name;
                codeIf = name + ":if";
                codeBefore = name + ":before";
                codeAfter = name + ":after";
            }
        }
    }
}
Sys.Application._getExtensionCode = function Sys$Application$_getExtensionCode(extension, doEval, context) {
    extension = extension.trim();
    var name, properties, propertyBag = {}, spaceIndex = extension.indexOf(' ');
    if (spaceIndex !== -1) {
        name = extension.substr(0, spaceIndex);
        properties = extension.substr(spaceIndex + 1);
        if (properties) {
            properties = properties.replace(/\\,/g, '\u0000').split(",");
            for (var i = 0, l = properties.length; i < l; i++) {
                var property = properties[i].replace(/\u0000/g, ","),
                        equalIndex = property.indexOf('='),
                        pValue, pName;
                if (equalIndex !== -1) {
                    pName = property.substr(0, equalIndex).trim();
                    pValue = property.substr(equalIndex + 1).trim();
                    if (doEval) {
                        pValue = this._getPropertyValue(null, null, pValue, context, true);
                    }
                }
                else {
                    pName = "$default";
                    pValue = property.trim();
                }
                propertyBag[pName] = pValue;
            }
        }
    }
    else {
        name = extension;
    }
    return { instance: Sys.Application._getMarkupExtension(name), name: name, properties: propertyBag };
}
Sys.Application._getPropertyValue = function Sys$Application$_getPropertyValue(target, name, value, context, inExtension, suppressExtension) {
    var propertyValue = value;
    if (value.startsWith("{{") && value.endsWith("}}")) {
        propertyValue = this._evaluateExpression(value.slice(2, -2), context);
    }
    else if (!suppressExtension && !inExtension && value.startsWith("{") && value.endsWith("}")) {
        var extension = this._getExtensionCode(value.slice(1, -1), true, context);
        propertyValue = extension.instance.extension(target, name, extension.properties);
    }
    return propertyValue;
}
Sys.Application._setProperty = function Sys$Application$_setProperty(target, type, name, value, context) {
    var map = Sys.Application._translateName(name, type),
        mapname = map.name;
    value = Sys.Application._getPropertyValue(target, mapname, value, context);
    if (typeof(value) === "undefined") {
        return;
    }
    if (map.type === 1) {
        map.setter.call(target, value);
    }
    else if (map.type === 2) {
        map.setter.call(target, typeof(value) === "function" ? value : new Function("sender", "args", value));
    }
    else {
        target[mapname] = value;
    }
}
Sys.Application._tryName = function Sys$Application$_tryName(name, type) {
    var prototype = type.prototype,
        setterName = "set_" + name, setter = prototype[setterName];
    if (setter) {
        return { name: name, setterName: setterName, setter: setter, type: 1 };
    }
    if (name.startsWith('on')) {
        setterName = "add_" + name.substr(2);
        var adder = prototype[setterName];
        if (adder) {
            return { name: name, setterName: setterName, setter: adder, type: 2 };
        }
    }
    if (typeof(prototype[name]) !== "undefined") {
        return { name: name };
    }
    return null;
}
Sys.Application._translateName = function Sys$Application$_translateName(name, type) {
    if (name && (name !== name.toLowerCase())) {
        throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.invalidAttributeName, name));
    }
    var cache, index = Sys.Application._prototypeIndex[type.__typeName];
    if (index) {
        cache = index[name];
        if (cache) return cache;
    }
    else {
        index = {};
    }    
    type.resolveInheritance();
    cache = Sys.Application._tryName(name, type);
    if (!cache) {
        var casedName = Sys.Application._mapToPrototype(name, type);
        if (casedName && (casedName !== name)) {
            cache = Sys.Application._tryName(casedName, type);
        }
        if (!cache) {
            cache = { name: name };
        }
    }
    index[name] = cache;
    return cache;
}
Sys.Application._mapToPrototype = function Sys$Application$_mapToPrototype(name, type) {
    var fixedName, caseIndex = Sys.Application._caseIndex[type.__typeName];
    if (!caseIndex) {
        caseIndex = {};
        type.resolveInheritance();
        for (var memberName in type.prototype) {
            if (memberName.startsWith("get_") || memberName.startsWith("set_") || memberName.startsWith("add_")) {
                memberName = memberName.substr(4);
            }
            else if (memberName.startsWith("remove_")) {
                memberName = memberName.substr(7);
            }
            caseIndex[memberName.toLowerCase()] = memberName;
        }
        Sys.Application._caseIndex[type.__typeName] = caseIndex;
    }
    name = name.toLowerCase();
    if (name.startsWith('on')) {
        fixedName = caseIndex[name.substr(2)];
        if (fixedName) {
            fixedName = "on" + fixedName;
        }
        else {
            fixedName = caseIndex[name];
        }
    }
    else {
        fixedName = caseIndex[name];
    }
    return fixedName;
}
Sys.Application._doEval = function Sys$Application$_doEval($expression, $context) {
    with($context.localContext) {
        with($context.userContext || {}) {
            return eval("(" + $expression + ")");
        }
    }
}
Sys.Application._evaluateExpression = function Sys$Application$_evaluateExpression($expression, $context) {
    return Sys.Application._doEval.call($context.userContext, $expression, $context);
}
Sys.Application._activateOnPartial = function Sys$Application$_activateOnPartial(panel, rendering) {
    var match = Sys.Application._activateList;
    this._updatePanelOld(panel, rendering);
    if (match && match.length) {
        var activatedNode,
            index = panel.id ? Array.indexOf(match, panel.id) : -1;
        if ((index === -1) && (match.length > 1 || match[0] !== "*")) {
            var node = panel;
            do {
                node = node.parentNode;
                if (node && node.id) {
                    index = Array.indexOf(match, node.id);
                    if (index !== -1) {
                        activatedNode = node;
                        break;
                    }
                }
            }
            while (node);
        }
        else {
            activatedNode = panel;
        }
        if (activatedNode || Array.contains(match, "*")) {
            var namespaceMappings;
            if (!activatedNode || (activatedNode === document.body)) {
                namespaceMappings = Sys.Application._getBodyNamespaceMapping();
            }
            else {
                namespaceMappings = Sys.Application._getNamespaceMappings(null, [activatedNode]);
            }
            Sys.Application._activateElements(panel.childNodes, null, namespaceMappings, { localContext: {} }, true);
        }
    }
}
Sys.Application._activateDOM = function Sys$Application$_activateDOM() {
    var namespaceMapping = Sys.Application._getBodyNamespaceMapping(),
        activateList = document.body.getAttribute(namespaceMapping.sysActivate),
        initialList = Sys.Application._activateList;
    activateList = activateList ? activateList.split(',') : [];
    if (initialList) {
        Array.addRange(activateList, initialList);
    }
    Sys.Application._activateList = activateList;
    if (!activateList.length) return;
    var id, elements = [];
    for (var i = 0, l = activateList.length; i < l; i++) {
        activateList[i] = id = activateList[i].trim();
        if (id === "*") {
            elements.push(document.body);
        }
        else {
            var e = document.getElementById(id);
            if (!e) {
                throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.cannotActivate, id));
            }
            elements.push(e);
        }
    }
    Sys.Application._activateElements(elements, activateList);
    if (Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.Application._activateList = activateList;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm._updatePanelOld = prm._updatePanel;
        prm._updatePanel = Sys.Application._activateOnPartial;
    }
}
Sys.Application._registerComponent = function Sys$Application$_registerComponent(element, component) {
    var components = element._components;
    if (!components) {
        element._components = components = [];
    }
    components[components.length] = component;
}
Sys.Application._raiseInit = function Sys$Application$_raiseInit() {
    this.beginCreateComponents();
    var handler = this.get_events().getHandler("init");
    if (handler) {
        handler(this, Sys.EventArgs.Empty);
    }
    this._activateDOM();
    this.endCreateComponents();
}
Sys.UI.Template = function Sys$UI$Template(element) {
    /// <summary locid="M:J#Sys.UI.Template.#ctor" />
    /// <param name="element" domElement="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "element", domElement: true}
    ]);
    if (e) throw e;
    this._element = element;
    this._instantiateIn = null;
    this._instanceId = 0;
}
    function Sys$UI$Template$get_element() {
        /// <value domElement="true" locid="P:J#Sys.UI.Template.element"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._element;
    }
    function Sys$UI$Template$dispose() {
        this._element = null;
        this._instantiateIn = null;
    }
    function Sys$UI$Template$_appendTextNode(code, storeElementCode, text) {
        code.push(storeElementCode + "document.createTextNode(" +
                    Sys.Serialization.JavaScriptSerializer.serialize(text) +
                    "));\n");
    }
    function Sys$UI$Template$_appendAttributeSetter(namespaceMappings, code, typeIndex, attrib, expression, isExpression, booleanValue) {
        var ns = attrib.ns, name = attrib.name, restricted = (!ns && Sys.UI.Template._isRestricted(name));
        if (restricted) {
            expression = "Sys.UI.Template._checkAttribute('" + name + "', " + expression + ")";
        }
        switch (attrib.type) {
            case 1: 
                if (isExpression) {
                    code.push("  $component = $element;\n  $element." + name + " = " + expression + ";\n;");
                }
                else {
                    code.push("  $component = $element;\n  " + expression + ";\n;");
                }
                return;
            case 2: 
                name = Sys.Serialization.JavaScriptSerializer.serialize(name);
                code.push("  $component = $element;\n    (" + expression +
                            ") ? Sys.UI.DomElement.addCssClass($element, " + name +
                            ") : Sys.UI.DomElement.removeCssClass($element, " + name + ");\n");
                return;
            case 3: 
                code.push("  __context[" + expression + "] = $component;\n");            
                return;
            case 4: 
                code.push("  $component = __componentIndex['" + ns + "'];\n");
                if (isExpression) {
                    var map = attrib.map;
                    if (map.type === 1) {
                        code.push("  $component." + map.setterName + "(" + expression + ");\n");
                    }
                    else if (map.type === 2) {
                        code.push("  __f = " + expression + ";\n");
                        code.push("  $component." + map.setterName + '(typeof(__f) === "function" ? __f : new Function("sender", "args", __f));\n');
                    }
                    else {
                        code.push("  $component." + map.name + " = " + expression + ";\n");
                    }
                }
                else {
                    code.push("  " + expression + ";\n");
                }
                return;
            case 5: 
                this["_" + name] = expression;
                return;
            default: 
                if (isExpression) {
                    var lowerName = name.toLowerCase();
                    if (lowerName.startsWith('on')) {
                        code.push("  $component = $element;\n  $element." + name + " = new Function(" + expression + ");\n");
                    }
                    else if (lowerName === "style") {
                        code.push("  $component = $element;\n  $element.style.cssText = " + expression + ";\n");
                    }
                    else {
                        if (booleanValue) {
                            code.push("  $component = $element;\n  if (" + expression +
                                        ") {\n    __e = document.createAttribute('" + name +
                                        "');\n    __e.nodeValue = \"" + booleanValue + "\";\n    $element.setAttributeNode(__e);\n  }\n");
                        }
                        else {
                            code.push("  $component = $element;\n  __e = document.createAttribute('" + name + "');\n  __e.nodeValue = " +
                                    expression + ";\n  $element.setAttributeNode(__e);\n");
                        }
                    }
                }
                else {
                    code.push("  $component = $element;\n  " + expression + ";\n");
                }
                return;
        }
    }
    function Sys$UI$Template$_translateStyleName(name) {
        if (name.indexOf('-') === -1) return name;
        var parts = name.toLowerCase().split('-');
        var newName = parts[0];
        for (var i = 1, l = parts.length; i < l; i++) {
            var part = parts[i];
            newName += part.substr(0, 1).toUpperCase() + part.substr(1);
        }
        return newName;
    }
    function Sys$UI$Template$_processAttribute(namespaceMappings, code, typeIndex, attrib, value, booleanValue) {
        value = this._getAttributeExpression(attrib, value);
        if (value) {
            this._appendAttributeSetter(namespaceMappings, code, typeIndex, attrib,
                value.code, value.isExpression, booleanValue);
        }
    }
    function Sys$UI$Template$_getAttributeExpression(attrib, value, noSerialize) {
        var type = typeof(value);
        if (type === "undefined") return null;
        if (value === null) return { isExpression: true, code: "null" };
        if (type === "string") {
            if (value.startsWith("{{") && value.endsWith("}}")) {
                return { isExpression: true, code: value.slice(2, -2).trim() };
            }
            else if (value.startsWith("{") && value.endsWith("}")) {
                var jss = Sys.Serialization.JavaScriptSerializer,
                    ext = Sys.Application._getExtensionCode(value.slice(1, -1)),
                    properties = ext.properties;
                var props = "";
                for (var name in properties) {
                    var subValue = this._getAttributeExpression(attrib, properties[name]);
                    if (subValue && subValue.isExpression) {
                        props += "," + jss.serialize(name) + ":" + subValue.code;
                    }
                }
                return { isExpression: ext.instance.expression,
                    code: "__app._getMarkupExtension(" + jss.serialize(ext.name) + ").extension($component, " +
                        (attrib.type === 2 ? "class:" : "") + jss.serialize(attrib.name) +
                        ", {$dataItem:$dataItem,$index:$index,$id:$id" + props + "})" };
            }
        }
        return { isExpression: true, code: (noSerialize ? value : 
                                            Sys.Serialization.JavaScriptSerializer.serialize(value)) };
    }
    function Sys$UI$Template$_processBooleanAttribute(element, namespaceMappings, code, typeIndex, name) {
        var value, node = element.getAttributeNode(namespaceMappings.sysNamespace + ":" + name);
        if (!node) {
            node = element.getAttributeNode(name);
            var nodeValue = node ? node.nodeValue : null;
            if (nodeValue && (typeof(nodeValue) === "string") &&
                nodeValue.startsWith("{") && nodeValue.endsWith("}")) {
                throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.attributeDoesNotSupportExpressions, name));
            }
            if (node && (node.specified || (node.nodeValue === true))) {
                value = true;
            }
            else if (element.getAttribute(name) === name) {
                value = true;
            }
            else {
                return;
            }
        }
        else {
            value = node.nodeValue;
            if (value === "true") {
                value = true;
            }
            else if (value === "false") {
                return;
            }
        }
        this._processAttribute(namespaceMappings, code, typeIndex, { name: name }, value, name);
    }
    function Sys$UI$Template$_processBooleanAttributes(element, namespaceMappings, code, typeIndex, attributes) {
        var name, node, value;
        for (var i = 0, l = attributes.length; i < l; i++) {
            this._processBooleanAttribute(element, namespaceMappings, code, typeIndex, attributes[i]);
        }
    }
    function Sys$UI$Template$_processCodeBlock(name, element, code, namespaceMappings) {
        var attribValue = this._getExplicitAttribute(null, null, null, element, name);
        if (attribValue) {
            var codeValue = this._getAttributeExpression({ name: name }, attribValue, true).code;
            code.push((name === namespaceMappings.codeIf) ? ("  if (" + codeValue + ") {\n")
                        : ("  " + codeValue + "\n"));
            return true;
        }
        return false;
    }
    function Sys$UI$Template$_getExplicitAttribute(namespaceMappings, code, typeIndex, element, name, processName) {
        var node;
        try {
            node = element.getAttributeNode(name);
        }
        catch (e) {
            return null;
        }
        if (!node || !node.specified) {
            return null;
        }
        if (processName) {
            var value = (name === "style" ? element.style.cssText : node.nodeValue);
            this._processAttribute(namespaceMappings, code, typeIndex, { name: processName }, value);
        }
        return node.nodeValue;
    }
    function Sys$UI$Template$_buildTemplateCode(nestedTemplates, namespaceMappings, element, code, depth) {
        var i, j, l, m, typeName, isInput,
            expressionRegExp = Sys.UI.Template._expressionRegExp,
            storeElementCode = "  " + (depth ? ("__p[__d-1].appendChild(") : "__topElements.push(");
        code.push("  __d++;\n");
        for (i = 0, l = element.childNodes.length; i < l; i++) {
            var childNode = element.childNodes[i], text = childNode.nodeValue;
            if (childNode.nodeType === 8) {
                code.push(storeElementCode + "document.createComment(" +
                    Sys.Serialization.JavaScriptSerializer.serialize(text) + "));\n");
            }
            else if (childNode.nodeType === 3) {
                var trimText = text.trim();
                if (trimText.startsWith("{") && trimText.endsWith("}") && (!trimText.startsWith("{{") || !trimText.endsWith("}}"))) {
                    var attribName, setComponentCode;
                    if (element.tagName.toLowerCase() === "textarea") {
                        attribName = "value";
                        setComponentCode = '$component=$element;\n';
                    }
                    else {
                        attribName = "nodeValue";
                        setComponentCode = storeElementCode + '$element=$component=document.createTextNode(""));\n';
                    }
                    var expr = this._getAttributeExpression({name:attribName}, trimText);
                    if (expr.isExpression) {
                        code.push(storeElementCode + "document.createTextNode(" + expr.code + "));\n");
                    }
                    else {
                        code.push(setComponentCode + '  ' + expr.code + ';\n');
                    }
                }
                else {
                    var match = expressionRegExp.exec(text), lastIndex = 0;
                    while (match) {
                        var catchUpText = text.substring(lastIndex, match.index);
                        if (catchUpText) {
                            this._appendTextNode(code, storeElementCode, catchUpText);
                        }
                        code.push(storeElementCode + "document.createTextNode(" + match[1] + "));\n");
                        lastIndex = match.index + match[0].length;
                        match = expressionRegExp.exec(text);
                    }
                    if (lastIndex < text.length) {
                        this._appendTextNode(code, storeElementCode, text.substr(lastIndex));
                    }
                }
            }
            else {
                var attributes = childNode.attributes,
                    typeNames = null, sysAttribute = null, typeIndex = {},
                    tagName = childNode.tagName.toLowerCase(),
                    booleanAttributes,  dp1 = depth + 1;
                if (tagName === "script") {
                    continue;
                }
                var isCodeIfGenerated = this._processCodeBlock(namespaceMappings.codeIf, childNode, 
                                                                code, namespaceMappings);
                this._processCodeBlock(namespaceMappings.codeBefore, childNode, code, namespaceMappings);
                isInput = (tagName === "input");
                if (isInput) {
                    var typeExpression = this._getAttributeExpression({ name: "type" }, childNode.getAttribute("type"));
                    var nameExpression = this._getAttributeExpression({ name: "name" }, childNode.getAttribute("name"));
                    if (!typeExpression.isExpression || !nameExpression.isExpression) {
                        throw Error.invalidOperation(Sys.UI.TemplatesRes.mustSetInputElementsExplicitly);
                    }
                    code.push("  $element=__p[__d]=Sys.UI.Template._createInput(" + typeExpression.code + ", " + nameExpression.code + ");\n");
                    booleanAttributes = Sys.UI.Template._inputBooleanAttributes;
                    this._processBooleanAttributes(childNode, namespaceMappings, code, typeIndex, booleanAttributes);
                }
                else {
                    code.push("  $element=__p[__d]=document.createElement('" + childNode.nodeName + "');\n");
                }
                typeNames = this._getExplicitAttribute(namespaceMappings, code, typeIndex, childNode, namespaceMappings.types);
                if (typeNames) {
                    typeNames = typeNames.split(',');
                    code.push("  __componentIndex = {}\n");
                    for (j = 0, m = typeNames.length; j < m; j++) {
                        typeName = typeNames[j].trim();
                        if (typeIndex[typeName]) continue; 
                        var type = namespaceMappings.namespaces[typeName];
                        if (!type) {
                            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.invalidAttach, namespaceMappings.types, typeName));
                        }
                        var isComponent = type.inheritsFrom(Sys.Component),
                            isControlOrBehavior = (isComponent && (type.inheritsFrom(Sys.UI.Behavior) || type.inheritsFrom(Sys.UI.Control))),
                            isContext = type.implementsInterface(Sys.UI.ITemplateContextConsumer);
                        typeIndex[typeName] = { type: type, isComponent: isComponent };
                        code.push("  __components.push(__componentIndex['" + typeName + "'] = $component = new " + type.getName());
                        if (isControlOrBehavior) {
                            code.push("($element));\n");
                        }
                        else {
                            code.push("());\n  Sys.Application._registerComponent($element, $component);\n");
                        }
                        sysAttribute = this._getExplicitAttribute(namespaceMappings, code,
                            typeIndex, childNode, typeName + ":sys-key");
                        if (sysAttribute) {
                            this._processAttribute(namespaceMappings, code, typeIndex,
                                { ns: typeName, name: "sys-key", type: 3 }, sysAttribute);
                        }
                        if (isComponent) {
                            code.push("  $component.beginUpdate();\n");
                        }
                        if (isContext) {
                            code.push("  $component.set_templateContext(__tc);\n");
                        }
                    }
                }
                
                sysAttribute = this._getExplicitAttribute(namespaceMappings, code, typeIndex, childNode, namespaceMappings.sysKey);
                if (sysAttribute) {
                    code.push("  __context[" +
                                Sys.Serialization.JavaScriptSerializer.serialize(sysAttribute) + "] = $element;\n");
                }
                this._getExplicitAttribute(namespaceMappings, code, typeIndex, childNode, namespaceMappings.sysId, "id");
                this._getExplicitAttribute(namespaceMappings, code, typeIndex, childNode, "style", "style");
                this._getExplicitAttribute(namespaceMappings, code, typeIndex, childNode, "class", "class");
                
                if (!isInput) {
                    booleanAttributes = Sys.UI.Template._booleanAttributes[tagName] ||
                        Sys.UI.Template._commonBooleanAttributes;
                    this._processBooleanAttributes(childNode, namespaceMappings, code, typeIndex, booleanAttributes);
                }
                
                var isSelect = (tagName === "select"),
                    delayedAttributes = null;
                for (j = 0, m = attributes.length; j < m; j++) {
                    var attribute = attributes[j], name = attribute.nodeName, lowerName = name.toLowerCase();
                    if (!attribute.specified && (!isInput || lowerName !== "value")) continue;
                    if ((lowerName === "class") || (lowerName === "style")) continue;
                    if (Array.indexOf(booleanAttributes, lowerName) !== -1) continue;
                    if (isInput && (Array.indexOf(Sys.UI.Template._inputRequiredAttributes, lowerName) !== -1)) continue;
                    var attrib = Sys.Application._splitAttribute(name),
                        ns = attrib.ns,
                        value = attribute.nodeValue;
                    name = attrib.name;
                    if (isSelect &&
                        (!ns || ns === namespaceMappings.sysNamespace)) {
                        if (name.toLowerCase() === "selectedindex") {
                            attrib.type = 1;
                            attrib.name = "selectedIndex";
                        }
                        else if (name === "value") {
                            attrib.type = 1;
                        }
                        if (attrib.type === 1) {
                            if (!delayedAttributes) {
                                delayedAttributes = [[attrib,value]];
                            }
                            else {
                                delayedAttributes[delayedAttributes.length] = [attrib,value];
                            }
                            continue;
                        }
                    }
                    if (ns) {
                        if (ns === namespaceMappings.codeNamespace) continue;
                        if (ns === namespaceMappings.sysNamespace) {
                            if (Array.indexOf(Sys.UI.Template._sysAttributes, name) !== -1) continue;
                            if ((name === "command") || (name === "commandargument") || (name === "commandtarget")) {
                                attrib.type = 5; 
                            }
                            attrib.ns = null;
                        }
                        else if (ns === namespaceMappings.styleNamespace) {
                            attrib.name = "style." + this._translateStyleName(name);
                            attrib.ns = null;
                            attrib.type = 1;
                        }
                        else if (ns === namespaceMappings.classNamespace) {
                            attrib.type = 2;
                        }
                        else {
                            var index = typeIndex[ns];
                            if (index) {
                                if (name === "sys-key") {
                                    continue; 
                                }
                                else {
                                    attrib.type = 4;
                                    attrib.map = Sys.Application._translateName(attrib.name, index.type);
                                    attrib.name = attrib.map.name;
                                }
                            }
                            else {
                                attrib.name = ns + ":" + name;
                                attrib.ns = null;
                            }
                        }
                    }
                    this._processAttribute(namespaceMappings, code, typeIndex, attrib, value);
                }
                if (this._command) {
                    if (!this._commandargument) {
                        this._commandargument = 'null';
                    }
                    if (!this._commandtarget) {
                        this._commandtarget = 'null';
                    }
                    code.push(" Sys.UI.DomEvent.addHandler($element, 'click', Sys.UI.Template._getCommandHandler(" 
                              +  this._command + ", " + this._commandargument + ", " + this._commandtarget + "));\n");
                    this._command = null;
                }
                this._commandargument = null;
                this._commandtarget = null;
                code.push(storeElementCode + "$element);\n");
                for (typeName in typeIndex) {
                    index = typeIndex[typeName];
                    if (index.isComponent) {
                        code.push("  if (($component=__componentIndex['" + typeName + "']).get_id()) __app.addComponent($component);\nif (__creatingComponents) __app._createdComponents[__app._createdComponents.length] = $component;\n");
                    }
                }
                if (Sys.UI.Template._isTemplate(childNode)) {
                    var nestedTemplate = new Sys.UI.Template(childNode);
                    nestedTemplate.compile();
                    nestedTemplates.push(childNode._msajaxtemplate);
                    code.push("  $element._msajaxtemplate = this.get_element()._msajaxtemplate[1][" + (nestedTemplates.length-1) + "];\n");
                }
                else {
                    this._buildTemplateCode(nestedTemplates, namespaceMappings, childNode, code, dp1);
                    code.push("  $element=__p[__d];\n");
                }
                if (delayedAttributes) {
                    for (j = 0, m = delayedAttributes.length; j < m; j++) {
                        attribute = delayedAttributes[j];
                        this._processAttribute(namespaceMappings, code, typeIndex, attribute[0], attribute[1]);
                    }
                }
                this._processCodeBlock(namespaceMappings.codeAfter, childNode, code, namespaceMappings);
                if (isCodeIfGenerated) {
                    code.push("  }\n");
                }
            }
        }
        code.push("  --__d;\n");
    }
    function Sys$UI$Template$compile() {
        /// <summary locid="M:J#Sys.UI.Template.compile" />
        if (arguments.length !== 0) throw Error.parameterCount();
        if (!this._instantiateIn) {
            var element = this.get_element();
            if (element._msajaxtemplate) {
                this._instantiateIn = element._msajaxtemplate[0];
            }
            else {
                var code = [" $index = (typeof($index) === 'number' ? $index : __instanceId);\n var __context = {}, $component, __app = Sys.Application, __creatingComponents = __app.get_isCreatingComponents(), __components = [], __componentIndex, __e, __f, __topElements = [], __d = 0, __p = [__containerElement], $id = Sys.UI.Template._getIdFunction($index), $element = __containerElement;\n  var __tc = new Sys.UI.TemplateContext();\n __tc.components = __components;\n __tc.nodes = __topElements;\n __tc.dataItem = $dataItem;\n __tc.index = $index;\n __tc.parentTemplateContext = $parentContext;\n __tc.keys = __context;\n __tc.getInstanceId = $id;\n __tc.containerElement = __containerElement;\n__tc.template = this;\n  Sys.UI.Template._contexts.push(__topElements);\n with(__context) { with($dataItem || {}) {\n"];
                var namespaceMappings = Sys.Application._getNamespaceMappings(null, [element]);
                var nestedTemplates = [];
                this._buildTemplateCode(nestedTemplates, namespaceMappings, element, code, 0);
                code.push("} }\n  for (var __i = 0, __l = __topElements.length; __i < __l; __i++) {\n  __containerElement.insertBefore(__topElements[__i], __referenceNode);\n }\n");
                code.push(" Sys.UI.Template._contexts.pop();\n"); 
                code.push(" return __tc;");
                code = code.join('');
                element._msajaxtemplate = [this._instantiateIn = new Function("__containerElement", "$dataItem", "$index", "__referenceNode", "$parentContext", "__instanceId", code), nestedTemplates];
            }
        }
    }
    function Sys$UI$Template$instantiateIn(containerElement, dataItem, dataIndex, nodeToInsertTemplateBefore, parentTemplateContext) {
        /// <summary locid="M:J#Sys.UI.Template.instantiateIn" />
        /// <param name="containerElement"></param>
        /// <param name="dataItem" optional="true" mayBeNull="true"></param>
        /// <param name="dataIndex" optional="true" mayBeNull="true" type="Number" integer="true"></param>
        /// <param name="nodeToInsertTemplateBefore" optional="true" mayBeNull="true"></param>
        /// <param name="parentTemplateContext" type="Sys.UI.TemplateContext" optional="true" mayBeNull="true"></param>
        /// <returns type="Sys.UI.TemplateContext"></returns>
        var e = Function._validateParams(arguments, [
            {name: "containerElement"},
            {name: "dataItem", mayBeNull: true, optional: true},
            {name: "dataIndex", type: Number, mayBeNull: true, integer: true, optional: true},
            {name: "nodeToInsertTemplateBefore", mayBeNull: true, optional: true},
            {name: "parentTemplateContext", type: Sys.UI.TemplateContext, mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        containerElement = Sys.UI.DomElement.resolveElement(containerElement);
        nodeToInsertTemplateBefore = (nodeToInsertTemplateBefore ? Sys.UI.DomElement.resolveElement(nodeToInsertTemplateBefore) : null);
        this.compile();
        return this._instantiateIn(containerElement, dataItem, dataIndex, nodeToInsertTemplateBefore, parentTemplateContext, this._instanceId++);
    }
Sys.UI.Template.prototype = {
    get_element: Sys$UI$Template$get_element,
    dispose: Sys$UI$Template$dispose,
    _appendTextNode: Sys$UI$Template$_appendTextNode,
    _appendAttributeSetter: Sys$UI$Template$_appendAttributeSetter,
    _translateStyleName: Sys$UI$Template$_translateStyleName,
    _processAttribute: Sys$UI$Template$_processAttribute,
    _getAttributeExpression: Sys$UI$Template$_getAttributeExpression,
    _processBooleanAttribute: Sys$UI$Template$_processBooleanAttribute,
    _processBooleanAttributes: Sys$UI$Template$_processBooleanAttributes,
    _processCodeBlock: Sys$UI$Template$_processCodeBlock,
    _getExplicitAttribute: Sys$UI$Template$_getExplicitAttribute,
    _buildTemplateCode: Sys$UI$Template$_buildTemplateCode,
    compile: Sys$UI$Template$compile,
    instantiateIn: Sys$UI$Template$instantiateIn
}
Sys.UI.Template._isRestricted = function Sys$UI$Template$_isRestricted(name) {
    var restricted = Sys.UI.Template._getRestrictedIndex();
    return restricted.attributes[name.toLowerCase()];
}
Sys.UI.Template._checkAttribute = function Sys$UI$Template$_checkAttribute(name, value) {
    if (!value) return value;
    var newValue = value, restricted = Sys.UI.Template._getRestrictedIndex();
    if (restricted.attributes[name.toLowerCase()]) {
        if (typeof(value) !== "string") {
            value = value.toString();
        }
        var match = Sys.UI.Template._protocolRegExp.exec(value.toLowerCase());
        if (match) {
            if (!restricted.protocols[match[1]]) {
                newValue = "";
            }
        }
    }
    return newValue;
}
Sys.UI.Template._getCommandHandler = function Sys$UI$Template$_getCommandHandler(name, argument, target) {
    return function() {
        if (target) {
            var control = (typeof(target) === "string") ? Sys.Application.findComponent(target) : target;
            if (!Sys.UI.Control.isInstanceOfType(control)) {
                throw Error.InvalidOperation(Sys.UI.TemplatesRes.invalidCommandTarget);
            }
            Sys.UI.DomElement._raiseBubbleEventFromControl(control, this, 
                                                            new Sys.CommandEventArgs(name, argument, this));
        }
        else {
            Sys.UI.DomElement.raiseBubbleEvent(this, new Sys.CommandEventArgs(name, argument, this)); 
        }
    }
}
Sys.UI.Template._getIdFunction = function Sys$UI$Template$_getIdFunction(instance) {
    return function(prefix) {
        return prefix + instance;
    }
}
Sys.UI.Template._createInput = function Sys$UI$Template$_createInput(type, name) {
    var element, dynamic = Sys.UI.Template._dynamicInputs;
    if (dynamic === true) {
        element = document.createElement('input');
        if (type) {
            element.type = type;
        }
        if (name) {
            element.name = name;
        }
    }
    else {
        var html = "<input ";
        if (type) {
            html += "type='" + type + "' ";
        }
        if (name) {
            html += "name='" + name + "' ";
        }
        html += "/>";
        try {
            element = document.createElement(html);
        }
        catch (err) {
            Sys.UI.Template._dynamicInputs = true;
            return Sys.UI.Template._createInput(type, name);
        }
        if (dynamic !== false) {
            if (element.tagName.toLowerCase() === "input") {
                Sys.UI.Template._dynamicInputs = false;
            }
            else {
                Sys.UI.Template._dynamicInputs = true;
                return Sys.UI.Template._createInput(type, name);
            }
        }
    }
    return element;
}
Sys.UI.Template._isTemplate = function Sys$UI$Template$_isTemplate(element) {
    var className = element.className;
    return (className && ((className === "sys-template") || Array.contains(className.split(' '), "sys-template")));
}
Sys.UI.Template._contexts = [];
Sys.UI.Template._inputRequiredAttributes = ["type", "name"];
Sys.UI.Template._commonBooleanAttributes = ["disabled"];
Sys.UI.Template._inputBooleanAttributes = ["disabled", "checked", "readonly"];
Sys.UI.Template._booleanAttributes = {
    "input": Sys.UI.Template._inputBooleanAttributes,
    "select": ["disabled", "multiple"],
    "option": ["disabled", "selected"],
    "img": ["disabled", "ismap"],
    "textarea": ["disabled", "readonly"]
};
Sys.UI.Template._sysAttributes = ["attach", "id", "key",
    "disabled", "checked", "readonly", "ismap", "multiple", "selected"];
Sys.UI.Template._expressionRegExp = /\{\{\s*([\w\W]*?)\s*\}\}/g;
Sys.UI.Template.allowedProtocols = [
    "http",
    "https"
];
Sys.UI.Template.restrictedAttributes = [
    "src",
    "href",
    "codebase",
    "cite",
    "background",
    "action",
    "longdesc",
    "profile",
    "usemap",
    "classid",
    "data"
];
Sys.UI.Template._getRestrictedIndex = function Sys$UI$Template$_getRestrictedIndex() {
    var i, l, protocolIndex, attributeIndex,
        protocols = Sys.UI.Template.allowedProtocols || [],
        attributes = Sys.UI.Template.restrictedAttributes || [],
        index = Sys.UI.Template._restrictedIndex;
    if (!index || (index.allowedProtocols !== protocols) || (index.restrictedAttributes !== attributes)) {
        index = { allowedProtocols: protocols, restrictedAttributes: attributes };
        index.protocols = protocolIndex = {};
        for (i = 0, l = protocols.length; i < l; i++) {
            protocolIndex[protocols[i]] = true;
        }
        index.attributes = attributeIndex = {};
        for (i = 0, l = attributes.length; i < l; i++) {
            attributeIndex[attributes[i]] = true;
        }
        Sys.UI.Template._restrictedIndex = index;
    }
    return index;
}
Sys.UI.Template._protocolRegExp = /^\s*([a-zA-Z0-9\+\-\.]+)\:/;
Sys.UI.Template.registerClass("Sys.UI.Template", null, Sys.IDisposable);
Sys.UI.TemplateContext = function Sys$UI$TemplateContext() {
    /// <summary locid="M:J#Sys.UI.TemplateContext.#ctor" />
    /// <field name="dataItem" locid="F:J#Sys.UI.TemplateContext.dataItem"></field>
    /// <field name="index" type="Number" integer="true" locid="F:J#Sys.UI.TemplateContext.index"></field>
    /// <field name="getInstanceId" type="Function" locid="F:J#Sys.UI.TemplateContext.getInstanceId"></field>
    /// <field name="parentTemplateContext" type="Sys.UI.TemplateContext" locid="F:J#Sys.UI.TemplateContext.parentTemplateContext"></field>
    /// <field name="containerElement" domElement="true" locid="F:J#Sys.UI.TemplateContext.containerElement"></field>
    /// <field name="components" type="Array" elementType="Object" locid="F:J#Sys.UI.TemplateContext.components"></field>
    /// <field name="nodes" type="Array" elementDomElement="true" locid="F:J#Sys.UI.TemplateContext.nodes"></field>
    /// <field name="keys" type="Object" locid="F:J#Sys.UI.TemplateContext.keys"></field>
    if (arguments.length !== 0) throw Error.parameterCount();
}
    function Sys$UI$TemplateContext$dispose() {
        /// <summary locid="M:J#Sys.UI.TemplateContext.dispose" />
        if (arguments.length !== 0) throw Error.parameterCount();
        var nodes = this.nodes;
        if (nodes) {
            for (var i = 0, l = nodes.length; i < l; i++) {
                var element = nodes[i];
                if (element.nodeType === 1) {
                    Sys.Application.disposeElement(element, false);
                }
            }
        }
        this.nodes = this.dataItem = this.components = this.getInstanceId =
        this.containerElement = this.parentTemplateContext = this.keys = null;
    }
    function Sys$UI$TemplateContext$getElementById(id) {
        /// <summary locid="M:J#Sys.UI.TemplateContext.getElementById" />
        /// <param name="id" type="String"></param>
        /// <returns domElement="true"></returns>
        var e = Function._validateParams(arguments, [
            {name: "id", type: String}
        ]);
        if (e) throw e;
        var instanceId = this.getInstanceId(id),
            nodes = this.nodes,
            el, i, l;
        for (i = 0, l = nodes.length; i < l; i++) {
            el = nodes[i];
            if (el.nodeType !== 1) continue;
            if (el.id === instanceId) return el;
            el = Sys.UI.DomElement.getElementById(instanceId, el);
            if (el) return el;
        }
        for (i = 0, l = nodes.length; i < l; i++) {
            el = nodes[i];
            if (el.nodeType !== 1) continue;
            if (el.id === id) return el;
            el = Sys.UI.DomElement.getElementById(id, el);
            if (el) return el;
        }
        return null;
    }
    function Sys$UI$TemplateContext$getObjectByKey(key) {
        /// <summary locid="M:J#Sys.UI.TemplateContext.getObjectByKey" />
        /// <param name="key" type="String"></param>
        /// <returns></returns>
        var e = Function._validateParams(arguments, [
            {name: "key", type: String}
        ]);
        if (e) throw e;
        if (!this.keys) return null;
        return this.keys[key] || null;
    }
    function Sys$UI$TemplateContext$initializeComponents() {
        /// <summary locid="M:J#Sys.UI.TemplateContext.initializeComponents" />
        if (arguments.length !== 0) throw Error.parameterCount();
        var components = this.components;
        if (components) {
            for (var i = components.length - 1; i > -1; i--) {
                var component = components[i];
                if (Sys.Component.isInstanceOfType(component)) {
                    if (component.get_isUpdating()) {
                        component.endUpdate();
                    }
                    else if (!component.get_isInitialized()) {
                        component.initialize();
                    }
                }
            }
        }
    }
Sys.UI.TemplateContext.prototype = {
    dataItem: null,
    index: 0,
    getInstanceId: null,
    parentTemplateContext: null,
    containerElement: null,
    components: null,
    nodes: null,
    keys: null,
    dispose: Sys$UI$TemplateContext$dispose,
    getElementById: Sys$UI$TemplateContext$getElementById,
    getObjectByKey: Sys$UI$TemplateContext$getObjectByKey,
    initializeComponents: Sys$UI$TemplateContext$initializeComponents
}
Sys.UI.TemplateContext.registerClass("Sys.UI.TemplateContext", null, Sys.IDisposable);
Sys.UI.ITemplateContextConsumer = function Sys$UI$ITemplateContextConsumer() {
    throw Error.notImplemented();
}
    function Sys$UI$ITemplateContextConsumer$get_templateContext() {
        /// <value type="Sys.UI.TemplateContext" mayBeNull="true" locid="P:J#Sys.UI.ITemplateContextConsumer.templateContext"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        throw Error.notImplemented();
    }
    function Sys$UI$ITemplateContextConsumer$set_templateContext(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Sys.UI.TemplateContext, mayBeNull: true}]);
        if (e) throw e;
        throw Error.notImplemented();
    }
Sys.UI.ITemplateContextConsumer.prototype = {
    get_templateContext: Sys$UI$ITemplateContextConsumer$get_templateContext,
    set_templateContext: Sys$UI$ITemplateContextConsumer$set_templateContext
}
Sys.UI.ITemplateContextConsumer.registerInterface("Sys.UI.ITemplateContextConsumer");
Sys.CollectionChange = function Sys$CollectionChange(action, newItems, newStartingIndex, oldItems, oldStartingIndex) {
    /// <summary locid="M:J#Sys.CollectionChange.#ctor" />
    /// <param name="action" type="Sys.NotifyCollectionChangedAction"></param>
    /// <param name="newItems" optional="true" mayBeNull="true"></param>
    /// <param name="newStartingIndex" type="Number" integer="true" optional="true" mayBeNull="true"></param>
    /// <param name="oldItems" optional="true" mayBeNull="true"></param>
    /// <param name="oldStartingIndex" type="Number" integer="true" optional="true" mayBeNull="true"></param>
    /// <field name="action" type="Sys.NotifyCollectionChangedAction" locid="F:J#Sys.CollectionChange.action"></field>
    /// <field name="newItems" type="Array" mayBeNull="true" elementMayBeNull="true" locid="F:J#Sys.CollectionChange.newItems"></field>
    /// <field name="newStartingIndex" type="Number" integer="true" locid="F:J#Sys.CollectionChange.newStartingIndex"></field>
    /// <field name="oldItems" type="Array" mayBeNull="true" elementMayBeNull="true" locid="F:J#Sys.CollectionChange.oldItems"></field>
    /// <field name="oldStartingIndex" type="Number" integer="true" locid="F:J#Sys.CollectionChange.oldStartingIndex"></field>
    var e = Function._validateParams(arguments, [
        {name: "action", type: Sys.NotifyCollectionChangedAction},
        {name: "newItems", mayBeNull: true, optional: true},
        {name: "newStartingIndex", type: Number, mayBeNull: true, integer: true, optional: true},
        {name: "oldItems", mayBeNull: true, optional: true},
        {name: "oldStartingIndex", type: Number, mayBeNull: true, integer: true, optional: true}
    ]);
    if (e) throw e;
    this.action = action;
    if (newItems) {
        if (!(newItems instanceof Array)) {
            newItems = [newItems];
        }
    }
    this.newItems = newItems || null;
    if (typeof newStartingIndex !== "number") {
        newStartingIndex = -1;
    }
    this.newStartingIndex = newStartingIndex;
    if (oldItems) {
        if (!(oldItems instanceof Array)) {
            oldItems = [oldItems];
        }
    }
    this.oldItems = oldItems || null;
    if (typeof oldStartingIndex !== "number") {
        oldStartingIndex = -1;
    }
    this.oldStartingIndex = oldStartingIndex;
}
Sys.CollectionChange.registerClass("Sys.CollectionChange");
Sys.NotifyCollectionChangedAction = function Sys$NotifyCollectionChangedAction() {
    /// <summary locid="M:J#Sys.NotifyCollectionChangedAction.#ctor" />
    /// <field name="add" type="Number" integer="true" static="true" locid="F:J#Sys.NotifyCollectionChangedAction.add"></field>
    /// <field name="remove" type="Number" integer="true" static="true" locid="F:J#Sys.NotifyCollectionChangedAction.remove"></field>
    /// <field name="reset" type="Number" integer="true" static="true" locid="F:J#Sys.NotifyCollectionChangedAction.reset"></field>
    if (arguments.length !== 0) throw Error.parameterCount();
    throw Error.notImplemented();
}
Sys.NotifyCollectionChangedAction.prototype = {
    add: 0,
    remove: 1,
    reset: 2
}
Sys.NotifyCollectionChangedAction.registerEnum('Sys.NotifyCollectionChangedAction');
Sys.NotifyCollectionChangedEventArgs = function Sys$NotifyCollectionChangedEventArgs(changes) {
    /// <summary locid="M:J#Sys.NotifyCollectionChangedEventArgs.#ctor" />
    /// <param name="changes" type="Array" elementType="Sys.CollectionChange"></param>
    var e = Function._validateParams(arguments, [
        {name: "changes", type: Array, elementType: Sys.CollectionChange}
    ]);
    if (e) throw e;
    this._changes = changes;
    Sys.NotifyCollectionChangedEventArgs.initializeBase(this);
}
    function Sys$NotifyCollectionChangedEventArgs$get_changes() {
        /// <value type="Array" elementType="Sys.CollectionChange" locid="P:J#Sys.NotifyCollectionChangedEventArgs.changes"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._changes || [];
    }
Sys.NotifyCollectionChangedEventArgs.prototype = {
    get_changes: Sys$NotifyCollectionChangedEventArgs$get_changes
}
Sys.NotifyCollectionChangedEventArgs.registerClass("Sys.NotifyCollectionChangedEventArgs", Sys.EventArgs);
Sys.Observer = function Sys$Observer() {
    throw Error.invalidOperation();
}
Sys.Observer.registerClass("Sys.Observer");
Sys.Observer.makeObservable = function Sys$Observer$makeObservable(target) {
    /// <summary locid="M:J#Sys.Observer.makeObservable" />
    /// <param name="target" mayBeNull="false"></param>
    /// <returns></returns>
    var e = Function._validateParams(arguments, [
        {name: "target"}
    ]);
    if (e) throw e;
    var isArray = target instanceof Array,
        o = Sys.Observer;
    Sys.Observer._ensureObservable(target);
    if (target.setValue === o._observeMethods.setValue) return target;
    o._addMethods(target, o._observeMethods);
    if (isArray) {
        o._addMethods(target, o._arrayMethods);
    }
    return target;
}
Sys.Observer._ensureObservable = function Sys$Observer$_ensureObservable(target) {
    var type = typeof target;
    if ((type === "string") || (type === "number") || (type === "boolean") || (type === "date")) {
        throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.notObservable, type));
    }
}
Sys.Observer._addMethods = function Sys$Observer$_addMethods(target, methods) {
    for (var m in methods) {
        if (target[m] && (target[m] !== methods[m])) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.observableConflict, m));
        }
        target[m] = methods[m];
    }
}
Sys.Observer._addEventHandler = function Sys$Observer$_addEventHandler(target, eventName, handler) {
    Sys.Observer._getContext(target, true).events.addHandler(eventName, handler);
}
Sys.Observer.addEventHandler = function Sys$Observer$addEventHandler(target, eventName, handler) {
    /// <summary locid="M:J#Sys.Observer.addEventHandler" />
    /// <param name="target"></param>
    /// <param name="eventName" type="String"></param>
    /// <param name="handler" type="Function"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "eventName", type: String},
        {name: "handler", type: Function}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._addEventHandler(target, eventName, handler);
}
Sys.Observer._removeEventHandler = function Sys$Observer$_removeEventHandler(target, eventName, handler) {
    Sys.Observer._getContext(target, true).events.removeHandler(eventName, handler);
}
Sys.Observer.removeEventHandler = function Sys$Observer$removeEventHandler(target, eventName, handler) {
    /// <summary locid="M:J#Sys.Observer.removeEventHandler" />
    /// <param name="target"></param>
    /// <param name="eventName" type="String"></param>
    /// <param name="handler" type="Function"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "eventName", type: String},
        {name: "handler", type: Function}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._removeEventHandler(target, eventName, handler);
}
Sys.Observer._raiseEvent = function Sys$Observer$_raiseEvent(target, eventName, eventArgs) {
    var ctx = Sys.Observer._getContext(target);
    if (!ctx) return;
    var handler = ctx.events.getHandler(eventName);
    if (handler) {
        handler(target, eventArgs);
    }
}
Sys.Observer.raiseEvent = function Sys$Observer$raiseEvent(target, eventName, eventArgs) {
    /// <summary locid="M:J#Sys.Observer.raiseEvent" />
    /// <param name="target"></param>
    /// <param name="eventName" type="String"></param>
    /// <param name="eventArgs" type="Sys.EventArgs"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "eventName", type: String},
        {name: "eventArgs", type: Sys.EventArgs}
    ]);
    if (e) throw e;
    Sys.Observer._raiseEvent(target, eventName, eventArgs);
}
Sys.Observer.addPropertyChanged = function Sys$Observer$addPropertyChanged(target, handler) {
    /// <summary locid="M:J#Sys.Observer.addPropertyChanged" />
    /// <param name="target" mayBeNull="false"></param>
    /// <param name="handler" type="Function"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "handler", type: Function}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._addEventHandler(target, "propertyChanged", handler);
}
Sys.Observer.removePropertyChanged = function Sys$Observer$removePropertyChanged(target, handler) {
    /// <summary locid="M:J#Sys.Observer.removePropertyChanged" />
    /// <param name="target" mayBeNull="false"></param>
    /// <param name="handler" type="Function"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "handler", type: Function}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._removeEventHandler(target, "propertyChanged", handler);
}
Sys.Observer._beginUpdate = function Sys$Observer$_beginUpdate(target) {
    Sys.Observer._getContext(target, true).updating = true;
}
Sys.Observer.beginUpdate = function Sys$Observer$beginUpdate(target) {
    /// <summary locid="M:J#Sys.Observer.beginUpdate" />
    /// <param name="target" mayBeNull="false"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._beginUpdate(target);
}
Sys.Observer._endUpdate = function Sys$Observer$_endUpdate(target) {
    var ctx = Sys.Observer._getContext(target);
    if (!ctx || !ctx.updating) return;
    ctx.updating = false;
    var dirty = ctx.dirty;
    ctx.dirty = false;
    if (dirty) {
        if (target instanceof Array) {
            var changes = ctx.changes;
            ctx.changes = null;
            Sys.Observer.raiseCollectionChanged(target, changes);
        }
        Sys.Observer.raisePropertyChanged(target, "");
    }
}
Sys.Observer.endUpdate = function Sys$Observer$endUpdate(target) {
    /// <summary locid="M:J#Sys.Observer.endUpdate" />
    /// <param name="target" mayBeNull="false"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._endUpdate(target);
}
Sys.Observer._isUpdating = function Sys$Observer$_isUpdating(target) {
    var ctx = Sys.Observer._getContext(target);
    return ctx ? ctx.updating : false;
}
Sys.Observer.isUpdating = function Sys$Observer$isUpdating(target) {
    /// <summary locid="M:J#Sys.Observer.isUpdating" />
    /// <param name="target" mayBeNull="false"></param>
    /// <returns type="Boolean"></returns>
    var e = Function._validateParams(arguments, [
        {name: "target"}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    return Sys.Observer._isUpdating(target);
}
Sys.Observer._setValue = function Sys$Observer$_setValue(target, propertyName, value) {
    var getter, setter, mainTarget = target, path = propertyName.split('.');
    for (var i = 0, l = (path.length - 1); i < l ; i++) {
        var name = path[i];
        getter = target["get_" + name]; 
        if (typeof (getter) === "function") {
            target = getter.call(target);
        }
        else {
            target = target[name];
        }
        var type = typeof (target);
        if ((target === null) || (type === "undefined")) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.nullReferenceInPath, propertyName));
        }
    }    
    var currentValue, lastPath = path[l];
    getter = target["get_" + lastPath];
    setter = target["set_" + lastPath];
    if (typeof(getter) === 'function') {
        currentValue = getter.call(target);
    }
    else {
        currentValue = target[lastPath];
    }
    if (typeof(setter) === 'function') {
        setter.call(target, value);
    }
    else {
        target[lastPath] = value;
    }
    if (currentValue !== value) {
        var ctx = Sys.Observer._getContext(mainTarget);
        if (ctx && ctx.updating) {
            ctx.dirty = true;
            return;
        };
        Sys.Observer.raisePropertyChanged(mainTarget, path[0]);
    }
}
Sys.Observer.setValue = function Sys$Observer$setValue(target, propertyName, value) {
    /// <summary locid="M:J#Sys.Observer.setValue" />
    /// <param name="target" mayBeNull="false"></param>
    /// <param name="propertyName" type="String"></param>
    /// <param name="value" mayBeNull="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "propertyName", type: String},
        {name: "value", mayBeNull: true}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._setValue(target, propertyName, value);
}
Sys.Observer.raisePropertyChanged = function Sys$Observer$raisePropertyChanged(target, propertyName) {
    /// <summary locid="M:J#Sys.Observer.raisePropertyChanged" />
    /// <param name="target" mayBeNull="false"></param>
    /// <param name="propertyName" type="String"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "propertyName", type: String}
    ]);
    if (e) throw e;
    Sys.Observer._ensureObservable(target);
    Sys.Observer._raiseEvent(target, "propertyChanged", new Sys.PropertyChangedEventArgs(propertyName));
}
Sys.Observer.addCollectionChanged = function Sys$Observer$addCollectionChanged(target, handler) {
    /// <summary locid="M:J#Sys.Observer.addCollectionChanged" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    /// <param name="handler" type="Function"></param>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true},
        {name: "handler", type: Function}
    ]);
    if (e) throw e;
    Sys.Observer._addEventHandler(target, "collectionChanged", handler);
}
Sys.Observer.removeCollectionChanged = function Sys$Observer$removeCollectionChanged(target, handler) {
    /// <summary locid="M:J#Sys.Observer.removeCollectionChanged" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    /// <param name="handler" type="Function"></param>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true},
        {name: "handler", type: Function}
    ]);
    if (e) throw e;
    Sys.Observer._removeEventHandler(target, "collectionChanged", handler);
}
Sys.Observer._collectionChange = function Sys$Observer$_collectionChange(target, change) {
    var ctx = Sys.Observer._getContext(target);
    if (ctx && ctx.updating) {
        ctx.dirty = true;
        var changes = ctx.changes;
        if (!changes) {
            ctx.changes = changes = [change];
        }
        else {
            changes.push(change);
        }
    }
    else {
        Sys.Observer.raiseCollectionChanged(target, [change]);
        Sys.Observer.raisePropertyChanged(target, 'length');
    }
}
Sys.Observer._add = function Sys$Observer$_add(target, item) {
    var change = new Sys.CollectionChange(Sys.NotifyCollectionChangedAction.add, [item], target.length);
    Array.add(target, item);
    Sys.Observer._collectionChange(target, change);
}
Sys.Observer.add = function Sys$Observer$add(target, item) {
    /// <summary locid="M:J#Sys.Observer.add" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    /// <param name="item" mayBeNull="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true},
        {name: "item", mayBeNull: true}
    ]);
    if (e) throw e;
    Sys.Observer._add(target, item);
}
Sys.Observer._addRange = function Sys$Observer$_addRange(target, items) {
    var change = new Sys.CollectionChange(Sys.NotifyCollectionChangedAction.add, items, target.length);
    Array.addRange(target, items);
    Sys.Observer._collectionChange(target, change);
}
Sys.Observer.addRange = function Sys$Observer$addRange(target, items) {
    /// <summary locid="M:J#Sys.Observer.addRange" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    /// <param name="items" type="Array" elementMayBeNull="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true},
        {name: "items", type: Array, elementMayBeNull: true}
    ]);
    if (e) throw e;
    Sys.Observer._addRange(target, items);
}
Sys.Observer._clear = function Sys$Observer$_clear(target) {
    var oldItems = Array.clone(target);
    Array.clear(target);
    Sys.Observer._collectionChange(target, new Sys.CollectionChange(Sys.NotifyCollectionChangedAction.reset, null, -1, oldItems, 0));
}
Sys.Observer.clear = function Sys$Observer$clear(target) {
    /// <summary locid="M:J#Sys.Observer.clear" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true}
    ]);
    if (e) throw e;
    Sys.Observer._clear(target);
}
Sys.Observer._insert = function Sys$Observer$_insert(target, index, item) {
    Array.insert(target, index, item);
    Sys.Observer._collectionChange(target, new Sys.CollectionChange(Sys.NotifyCollectionChangedAction.add, [item], index));
}
Sys.Observer.insert = function Sys$Observer$insert(target, index, item) {
    /// <summary locid="M:J#Sys.Observer.insert" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    /// <param name="index" type="Number" integer="true"></param>
    /// <param name="item" mayBeNull="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true},
        {name: "index", type: Number, integer: true},
        {name: "item", mayBeNull: true}
    ]);
    if (e) throw e;
    Sys.Observer._insert(target, index, item);
}
Sys.Observer._remove = function Sys$Observer$_remove(target, item) {
    var index = Array.indexOf(target, item);
    if (index !== -1) {
        Array.remove(target, item);
        Sys.Observer._collectionChange(target, new Sys.CollectionChange(Sys.NotifyCollectionChangedAction.remove, null, -1, [item], index));
        return true;
    }
    return false;
}
Sys.Observer.remove = function Sys$Observer$remove(target, item) {
    /// <summary locid="M:J#Sys.Observer.remove" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    /// <param name="item" mayBeNull="true"></param>
    /// <returns type="Boolean"></returns>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true},
        {name: "item", mayBeNull: true}
    ]);
    if (e) throw e;
    return Sys.Observer._remove(target, item);
}
Sys.Observer._removeAt = function Sys$Observer$_removeAt(target, index) {
    if ((index > -1) && (index < target.length)) {
        var item = target[index];
        Array.removeAt(target, index);
        Sys.Observer._collectionChange(target, new Sys.CollectionChange(Sys.NotifyCollectionChangedAction.remove, null, -1, [item], index));
    }
}
Sys.Observer.removeAt = function Sys$Observer$removeAt(target, index) {
    /// <summary locid="M:J#Sys.Observer.removeAt" />
    /// <param name="target" type="Array" elementMayBeNull="true"></param>
    /// <param name="index" type="Number" integer="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "target", type: Array, elementMayBeNull: true},
        {name: "index", type: Number, integer: true}
    ]);
    if (e) throw e;
    Sys.Observer._removeAt(target, index);
}
Sys.Observer.raiseCollectionChanged = function Sys$Observer$raiseCollectionChanged(target, changes) {
    /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
    /// <param name="target"></param>
    /// <param name="changes" type="Array" elementType="Sys.CollectionChange"></param>
    var e = Function._validateParams(arguments, [
        {name: "target"},
        {name: "changes", type: Array, elementType: Sys.CollectionChange}
    ]);
    if (e) throw e;
    Sys.Observer._raiseEvent(target, "collectionChanged", new Sys.NotifyCollectionChangedEventArgs(changes));
}
Sys.Observer._observeMethods = {
    add_propertyChanged: function(handler) {
        Sys.Observer._addEventHandler(this, "propertyChanged", handler);
    },
    remove_propertyChanged: function(handler) {
        Sys.Observer._removeEventHandler(this, "propertyChanged", handler);
    },
    addEventHandler: function(eventName, handler) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="eventName" type="String"></param>
        /// <param name="handler" type="Function"></param>
        var e = Function._validateParams(arguments, [
            {name: "eventName", type: String},
            {name: "handler", type: Function}
        ]);
        if (e) throw e;
        Sys.Observer._addEventHandler(this, eventName, handler);
    },
    removeEventHandler: function(eventName, handler) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="eventName" type="String"></param>
        /// <param name="handler" type="Function"></param>
        var e = Function._validateParams(arguments, [
            {name: "eventName", type: String},
            {name: "handler", type: Function}
        ]);
        if (e) throw e;
        Sys.Observer._removeEventHandler(this, eventName, handler);
    },
    get_isUpdating: function() {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <returns type="Boolean"></returns>
        if (arguments.length !== 0) throw Error.parameterCount();
        return Sys.Observer._isUpdating(this);
    },
    beginUpdate: function() {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        if (arguments.length !== 0) throw Error.parameterCount();
        Sys.Observer._beginUpdate(this);
    },
    endUpdate: function() {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        if (arguments.length !== 0) throw Error.parameterCount();
        Sys.Observer._endUpdate(this);
    },
    setValue: function(name, value) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="name" type="String"></param>
        /// <param name="value" mayBeNull="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String},
            {name: "value", mayBeNull: true}
        ]);
        if (e) throw e;
        Sys.Observer._setValue(this, name, value);
    },
    raiseEvent: function(eventName, eventArgs) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="eventName" type="String"></param>
        /// <param name="eventArgs" type="Sys.EventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "eventName", type: String},
            {name: "eventArgs", type: Sys.EventArgs}
        ]);
        if (e) throw e;
        Sys.Observer._raiseEvent(this, eventName, eventArgs);
    },
    raisePropertyChanged: function(name) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="name" type="String"></param>
        var e = Function._validateParams(arguments, [
            {name: "name", type: String}
        ]);
        if (e) throw e;
        Sys.Observer._raiseEvent(this, "propertyChanged", new Sys.PropertyChangedEventArgs(name));
    }
}
Sys.Observer._arrayMethods = {
    add_collectionChanged: function(handler) {
        Sys.Observer._addEventHandler(this, "collectionChanged", handler);
    },
    remove_collectionChanged: function(handler) {
        Sys.Observer._removeEventHandler(this, "collectionChanged", handler);
    },
    add: function(item) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="item" mayBeNull="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "item", mayBeNull: true}
        ]);
        if (e) throw e;
        Sys.Observer._add(this, item);
    },
    addRange: function(items) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="items" type="Array" elementMayBeNull="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "items", type: Array, elementMayBeNull: true}
        ]);
        if (e) throw e;
        Sys.Observer._addRange(this, items);
    },
    clear: function() {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        if (arguments.length !== 0) throw Error.parameterCount();
        Sys.Observer._clear(this);
    },
    insert: function(index, item) { 
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="index" type="Number" integer="true"></param>
        /// <param name="item" mayBeNull="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "index", type: Number, integer: true},
            {name: "item", mayBeNull: true}
        ]);
        if (e) throw e;
        Sys.Observer._insert(this, index, item);
    },
    remove: function(item) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="item" mayBeNull="true"></param>
        /// <returns type="Boolean"></returns>
        var e = Function._validateParams(arguments, [
            {name: "item", mayBeNull: true}
        ]);
        if (e) throw e;
        return Sys.Observer._remove(this, item);
    },
    removeAt: function(index) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="index" type="Number" integer="true"></param>
        var e = Function._validateParams(arguments, [
            {name: "index", type: Number, integer: true}
        ]);
        if (e) throw e;
        Sys.Observer._removeAt(this, index);
    },
    raiseCollectionChanged: function(changes) {
        /// <summary locid="M:J#Sys.Observer.raiseCollectionChanged" />
        /// <param name="changes" type="Array" elementType="Sys.CollectionChange"></param>
        var e = Function._validateParams(arguments, [
            {name: "changes", type: Array, elementType: Sys.CollectionChange}
        ]);
        if (e) throw e;
        Sys.Observer._raiseEvent(this, "collectionChanged", new Sys.NotifyCollectionChangedEventArgs(changes));
    }
}
Sys.Observer._getContext = function Sys$Observer$_getContext(obj, create) {
    var ctx = obj._observerContext;
    if (ctx) return ctx();
    if (create) {
        return (obj._observerContext = Sys.Observer._createContext())();
    }
    return null;
}
Sys.Observer._createContext = function Sys$Observer$_createContext() {
    var ctx = {
        events: new Sys.EventHandlerList()
    };
    return function() {
        return ctx;
    }
}
Sys.BindingMode = function Sys$BindingMode() {
}
Sys.BindingMode.prototype = {
    auto: 0,
    oneTime: 1,
    oneWay: 2,
    twoWay: 3,
    oneWayToSource: 4
}
Sys.BindingMode.registerEnum("Sys.BindingMode");
Sys.Binding = function Sys$Binding() {
    Sys.Binding.initializeBase(this);
}
    function Sys$Binding$get_convert() {
        /// <value mayBeNull="true" locid="P:J#Sys.Binding.convert"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._convert || null;
    }
    function Sys$Binding$set_convert(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
       this._convert = value;
       this._convertFn = this._resolveFunction(value);
    }
    function Sys$Binding$get_convertBack() {
        /// <value mayBeNull="true" locid="P:J#Sys.Binding.convertBack"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._convertBack || null;
    }
    function Sys$Binding$set_convertBack(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
       this._convertBack = value;
       this._convertBackFn = this._resolveFunction(value);
    }
    function Sys$Binding$get_ignoreErrors() {
        /// <value type="Boolean" mayBeNull="false" locid="P:J#Sys.Binding.ignoreErrors"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._ignoreErrors;
    }
    function Sys$Binding$set_ignoreErrors(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Boolean}]);
        if (e) throw e;
       this._ignoreErrors = value;
    }
    function Sys$Binding$get_mode() {
        /// <value type="Sys.BindingMode" mayBeNull="false" locid="P:J#Sys.Binding.mode"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._mode;
    }
    function Sys$Binding$set_mode(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Sys.BindingMode}]);
        if (e) throw e;
        if (this.get_isInitialized()) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.commonNotAfterInit, "Binding", "mode"));
        }
        this._mode = value;
    }
    function Sys$Binding$get_defaultValue() {
        /// <value mayBeNull="true" locid="P:J#Sys.Binding.defaultValue"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._defaultValue;
    }
    function Sys$Binding$set_defaultValue(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        this._defaultValue = value;
    }
    function Sys$Binding$get_source() {
        /// <value mayBeNull="true" locid="P:J#Sys.Binding.source"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._source || null;
    }
    function Sys$Binding$set_source(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        if (this.get_isInitialized()) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.commonNotAfterInit, "Binding", "source"));
        }
        this._source = value;
    }
    function Sys$Binding$get_path() {
        /// <value type="String" mayBeNull="true" locid="P:J#Sys.Binding.path"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._path || "";
    }
    function Sys$Binding$set_path(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String, mayBeNull: true}]);
        if (e) throw e;
        if (this.get_isInitialized()) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.commonNotAfterInit, "Binding", "path"));
        }
        this._path = value;
        this._pathArray = value ? value.split('.') : null;
    }
    function Sys$Binding$get_target() {
        /// <value mayBeNull="true" locid="P:J#Sys.Binding.target"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._target || null;
    }
    function Sys$Binding$set_target(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        if (this.get_isInitialized()) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.commonNotAfterInit, "Binding", "target"));
        }
        this._target = value;
    }
    function Sys$Binding$get_targetProperty() {
        /// <value type="String" mayBeNull="true" locid="P:J#Sys.Binding.targetProperty"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._targetProperty || "";
    }
    function Sys$Binding$set_targetProperty(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String, mayBeNull: true}]);
        if (e) throw e;
        if (this.get_isInitialized()) {
            throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.commonNotAfterInit, 
                                                       "Binding", "targetProperty"));
        }
        this._targetProperty = value;
        this._targetPropertyArray = value ? value.split('.') : null;
    }
    function Sys$Binding$_addBinding(element) {
        if (element.nodeType === 3) {
            element = element.parentNode;
            if (!element) return;
        }
        var bindings = element._msajaxBindings;
        if (!bindings) {
           element._msajaxBindings = [this];
        }
        else {
           bindings.push(this);
        }
        if (typeof(element.dispose) !== "function") {
            element.dispose = Sys.Binding._disposeBindings;
        }
    }
    function Sys$Binding$_disposeHandlers() {
        for (var i = 0, l = this._handlers.length; i < l; i++) {
            var entry = this._handlers[i], object = entry[2];
            switch (entry[0]) {
                case "click": 
                case "keyup": 
                case "change": 
                    Sys.UI.DomEvent.removeHandler(object, entry[0], entry[1]);
                    break;
                case "propertyChanged": 
                    if (object.remove_propertyChanged) { 
                        object.remove_propertyChanged(entry[1]);
                    }
                    else {
                        Sys.Observer.removePropertyChanged(object, entry[1]);
                    }
                    break;
                case "disposing": 
                    object.remove_disposing(entry[1]);
                    break;
            }
        }
    }
    function Sys$Binding$dispose() {
        this._disposed = true;
        if (this._handlers) {
            this._disposeHandlers();
            delete this._handlers;
        }
        this._convert = null;
        this._convertBack = null;
        this._convertFn = null;
        this._convertBackFn = null;
        this._lastSource = null;
        this._lastTarget = null;
        this._source = null;
        this._target = null;
        this._pathArray = null;
        this._defaultValue = null;
        this._targetPropertyArray = null;
        Sys.Binding.callBaseMethod(this, 'dispose');
    }
    function Sys$Binding$_getDefaultMode(target) {
        if (Sys.UI.DomElement.isDomElement(target)) {
            if (target.nodeType === 1) { 
                var tag = target.tagName ? target.tagName.toLowerCase() : null;
                if ((tag === "input") || (tag === "select") || (tag === "textarea")) {
                    return Sys.BindingMode.twoWay;
                }
            }
        }
        else {
            if (Sys.INotifyPropertyChange.isImplementedBy(target)) { 
                return Sys.BindingMode.twoWay;
            }
        }
        return Sys.BindingMode.oneWay;
    }
    function Sys$Binding$_getPropertyFromIndex(obj, path, index) {
        for (var i = 0; i <= index; i++) {
            if (obj === null || typeof(obj) === "undefined") {
                return null;
            }
            obj = this._getPropertyData(obj, path[i]);
        }
        return obj;
    }
    function Sys$Binding$_getPropertyData(obj, name) {
        if (typeof (obj["get_" + name]) === "function") {
            return obj["get_" + name]();
        }
        else {
            return obj[name];
        }
    }
    function Sys$Binding$_hookEvent(object, handlerMethod, componentHandlerMethod) {
        var thisHander;
        if (Sys.UI.DomElement.isDomElement(object)) {
            thisHandler = Function.createDelegate(this, handlerMethod);
            Array.add(this._handlers, ["propertyChanged", thisHandler, object]); 
            if (object.add_propertyChanged) { 
                object.add_propertyChanged(thisHandler);
            }
            else {
                Sys.Observer.addPropertyChanged(object, thisHandler);
            }
            var tag = object.tagName ? object.tagName.toLowerCase() : null;
            if ((tag === "input") || (tag === "select") || (tag === "textarea")) {
                var type = object.type;
                if ((tag === "input") && type && 
                    ((type.toLowerCase() === "checkbox") || (type.toLowerCase() === "radio"))) {
                        thisHandler = Function.createDelegate(this, handlerMethod);
                        Array.add(this._handlers, ["click", thisHandler, object]); 
                        Sys.UI.DomEvent.addHandler(object, "click", thisHandler);
                }
                if (tag === "select") {
                    thisHandler = Function.createDelegate(this, handlerMethod);
                    Array.add(this._handlers, ["click", thisHandler, object]); 
                    Sys.UI.DomEvent.addHandler(object, "click", thisHandler);
                }
                if (tag === "select") {
                    thisHandler = Function.createDelegate(this, handlerMethod);
                    Array.add(this._handlers, ["keyup", thisHandler, object]); 
                    Sys.UI.DomEvent.addHandler(object, "keyup", thisHandler);
                }
                thisHandler = Function.createDelegate(this, handlerMethod);
                Array.add(this._handlers, ["change", thisHandler, object]); 
                Sys.UI.DomEvent.addHandler(object, "change", thisHandler);
                this._addBinding(object);
            }
        }
        else {
            thisHandler = Function.createDelegate(this, componentHandlerMethod);
            Array.add(this._handlers, ["propertyChanged", thisHandler, object]); 
            if (object.add_propertyChanged) { 
                object.add_propertyChanged(thisHandler);
            }
            else {
                Sys.Observer.addPropertyChanged(object, thisHandler);
            }
            
            if (Sys.INotifyDisposing.isImplementedBy(object)) {
                thisHandler = Function.createDelegate(this, this._onDisposing);
                Array.add(this._handlers, ["disposing", thisHandler, object]); 
                object.add_disposing(thisHandler);
            }
        }
    }
    function Sys$Binding$_onDisposing() {
        this.dispose();
    }
    function Sys$Binding$_resolveFunction(value) {
        var ret;
        if (typeof(value) === 'function') { 
            ret = value;
        }
        else {
            if (typeof(value) !== "string") {
                throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.invalidFunctionName, value));
            }
            try {
                ret = Type.parse(value);
            }
            catch (e) {
                throw Error.invalidOperation(String.format(Sys.UI.TemplatesRes.functionNotFound, value));
            }
        }
        return ret;
    }
    function Sys$Binding$update(mode) {
        /// <summary locid="M:J#Sys.Binding.update" />
        /// <param name="mode" optional="true" mayBeNull="false"></param>
        var e = Function._validateParams(arguments, [
            {name: "mode", optional: true}
        ]);
        if (e) throw e;
        if (!this.get_isInitialized()) {
            throw Error.invalidOperation(Sys.UI.TemplatesRes.updateBeforeInit);
        }
        mode = mode || this.get_mode();
        if (mode === Sys.BindingMode.oneWayToSource) {
            this._onTargetPropertyChanged(true);
        }
        else{
            this._onSourcePropertyChanged(true);
        }
    }
    function Sys$Binding$initialize() {
        var source = this.get_source(), target = this.get_target(), mode = this.get_mode();
        if (this.get_isInitialized()) {
            throw Error.invalidOperation(Sys.UI.TemplatesRes.initializeAfterInit);
        }
        var msg = Sys.UI.TemplatesRes.bindingPropertyNotSet;
        if (!source) {
            throw Error.invalidOperation(String.format(msg,"source"));
        }
        if (!target) {
            throw Error.invalidOperation(String.format(msg,"target"));
        }
        if (!this.get_path()) {
            throw Error.invalidOperation(String.format(msg,"path"));
        }
        if (!this.get_targetProperty()) {
            throw Error.invalidOperation(String.format(msg,"targetProperty"));
        }
        Sys.Binding.callBaseMethod(this, 'initialize');
        if (mode === Sys.BindingMode.auto) {
            mode = this._getDefaultMode(target);
        }
        this.update(mode);
        if (mode !== Sys.BindingMode.oneTime) {
            this._handlers = [];
            if (mode !== Sys.BindingMode.oneWayToSource) {
                this._hookEvent(source, this._onSourcePropertyChanged, this._onComponentSourceChanged);
            }
            else {
                if (Sys.UI.DomElement.isDomElement(source)) {
                    this._addBinding(source);
                }
            }
            if (mode !== Sys.BindingMode.oneWay) {
                this._hookEvent(target, this._onTargetPropertyChanged, this._onComponentTargetChanged);
            }
            else {
                if (Sys.UI.DomElement.isDomElement(target)) {
                    this._addBinding(target);
                }
            }
        }
    }
    function Sys$Binding$_onComponentSourceChanged(sender, args) {
        if (this._disposed) return;
        var name = args.get_propertyName();
        if ((name === "") || (name === this._pathArray[0])) {
            this._onSourcePropertyChanged();
        }
    }
    function Sys$Binding$_onComponentTargetChanged(sender, args) {
        if (this._disposed) return;
        var name = args.get_propertyName();
        if ((name === "") || (name ===  this._targetPropertyArray[0])) {
            this._onTargetPropertyChanged();
        }
    }
    function Sys$Binding$_onSourcePropertyChanged(force) {
        if (this._disposed) return;
        var target = this.get_target(),
            source = this._getPropertyFromIndex(this.get_source(), this._pathArray, 
                                                this._pathArray.length - 1);
        if (!this._updateSource && (force || (source !== this._lastSource))) {
            try {
                this._updateTarget = true;
                this._lastSource = this._lastTarget = source;
                if (this._convertFn) {
                    if (this._ignoreErrors) {
                        try {
                            source = this._convertFn(source, this);
                        }
                        catch (e) {}
                    }
                    else {
                        source = this._convertFn(source, this);
                    }
                }
                if ((source === null) || (typeof(source) === "undefined")) {
                    source = this.get_defaultValue();
                }
                if (this._targetProperty && this._targetProperty.startsWith("class:")) {
                    var className = this._targetProperty.substr(6).trim();
                    source ? Sys.UI.DomElement.addCssClass(target, className) : Sys.UI.DomElement.removeCssClass(target, className);
                }
                else {
                    var targetArrayLength = this._targetPropertyArray.length;
                    target = this._getPropertyFromIndex(target, this._targetPropertyArray, targetArrayLength - 2);
                    if ((target !== null) && (typeof(target) !== "undefined")) {
                        var name = this._targetPropertyArray[targetArrayLength - 1];
                        if (Sys.UI.DomElement.isDomElement(target)) {
                            source = Sys.UI.Template._checkAttribute(name, source);
                        }
                        Sys.Observer.setValue(target, name, source);
                    }
                }
            }
            finally {
                this._updateTarget = false;
            }
        }
    }
    function Sys$Binding$_onTargetPropertyChanged(force) {
        if (this._disposed) return;
        var target = this._getPropertyFromIndex(this.get_target(), this._targetPropertyArray, 
                                                this._targetPropertyArray.length - 1);
        if (!this._updateTarget && (force || (target !== this._lastTarget))) {
            try {
                this._updateSource = true;
                this._lastTarget = this._lastSource = target;
                if (this._convertBackFn) {
                    if (this._ignoreErrors) {
                        try {
                            target = this._convertBackFn(target, this);
                        }
                        catch (e) {}
                    }
                    else {
                        target = this._convertBackFn(target, this);
                    }
                }
                var sourceArrayLength = this._pathArray.length,
                    source = this._getPropertyFromIndex(this.get_source(), this._pathArray, sourceArrayLength - 2);
                if ((source !== null) && (typeof(source) !== "undefined")) {                     
                    var name = this._pathArray[sourceArrayLength - 1];
                    if (Sys.UI.DomElement.isDomElement(source)) {
                        target = Sys.UI.Template._checkAttribute(name, target);
                    }
                    Sys.Observer.setValue(source, name, target);
                }
            }
            finally {
                this._updateSource = false;
            }
        }
    }
Sys.Binding.prototype = {
    _convert: null,
    _convertBack: null,
    _convertFn: null,
    _convertBackFn: null,
    _handlers: null,
    _ignoreErrors: false,
    _mode: Sys.BindingMode.auto,
    _path: null,
    _targetProperty: null,
    _source: null,
    _target: null,
    _updateSource: false,
    _updateTarget: false,
    _defaultValue: null,
    get_convert: Sys$Binding$get_convert,
    set_convert: Sys$Binding$set_convert,
    get_convertBack: Sys$Binding$get_convertBack,
    set_convertBack: Sys$Binding$set_convertBack,
    get_ignoreErrors: Sys$Binding$get_ignoreErrors,
    set_ignoreErrors: Sys$Binding$set_ignoreErrors,
    get_mode: Sys$Binding$get_mode,
    set_mode: Sys$Binding$set_mode,
    get_defaultValue: Sys$Binding$get_defaultValue,
    set_defaultValue: Sys$Binding$set_defaultValue,
    get_source: Sys$Binding$get_source,
    set_source: Sys$Binding$set_source,
    get_path: Sys$Binding$get_path,
    set_path: Sys$Binding$set_path,
    get_target: Sys$Binding$get_target,
    set_target: Sys$Binding$set_target,
    get_targetProperty: Sys$Binding$get_targetProperty,
    set_targetProperty: Sys$Binding$set_targetProperty,
    _addBinding: Sys$Binding$_addBinding,
    _disposeHandlers: Sys$Binding$_disposeHandlers,
    dispose: Sys$Binding$dispose,
    _getDefaultMode: Sys$Binding$_getDefaultMode,
    _getPropertyFromIndex: Sys$Binding$_getPropertyFromIndex,
    _getPropertyData: Sys$Binding$_getPropertyData,
    _hookEvent: Sys$Binding$_hookEvent,
    _onDisposing: Sys$Binding$_onDisposing,
    _resolveFunction: Sys$Binding$_resolveFunction,
    update: Sys$Binding$update,
    initialize: Sys$Binding$initialize,
    _onComponentSourceChanged: Sys$Binding$_onComponentSourceChanged,
    _onComponentTargetChanged: Sys$Binding$_onComponentTargetChanged,
    _onSourcePropertyChanged: Sys$Binding$_onSourcePropertyChanged,
    _onTargetPropertyChanged: Sys$Binding$_onTargetPropertyChanged
}
Sys.Binding._disposeBindings = function Sys$Binding$_disposeBindings() {
    var bindings = this._msajaxBindings;    
    if (bindings) {
        for(var i = 0, l = bindings.length; i < l; i++) {
            bindings[i].dispose();
        }
    }
    this._msajaxBindings = null;
    
    if (this.control && typeof(this.control.dispose) === "function") {
        this.control.dispose();
    }
    if (this.dispose === Sys.Binding._disposeBindings) {
        this.dispose = null;
    }
}
Sys.Binding.registerClass("Sys.Binding", Sys.Component);
Sys.Application.registerMarkupExtension(
"binding", 
function(component, targetProperty, properties) {
    var mode = properties.mode, ignoreErrors = properties.ignoreErrors,
        binding = new Sys.Binding();
    if (mode) {
        if (typeof(mode) === "string") {
            mode = Sys.BindingMode.parse(mode);
        }
    }
    else {
        mode = Sys.BindingMode.auto;
    }
    
    binding.set_source(properties.source || properties.$dataItem);
    binding.set_path(properties.path || properties.$default);
    binding.set_target(component);
    binding.set_targetProperty(targetProperty);
    binding.set_mode(mode);
    if (properties.convert) {
        binding.set_convert(properties.convert);
    }
    if (properties.convertBack) {
        binding.set_convertBack(properties.convertBack);
    }
    if (typeof(properties.defaultValue) !== "undefined") {
        binding.set_defaultValue(properties.defaultValue);
    }
    if (ignoreErrors) {
        if (typeof(ignoreErrors) === "string") {
            ignoreErrors = Boolean.parse(ignoreErrors);
        }
        else {
            ignoreErrors = !!ignoreErrors;
        }
        binding.set_ignoreErrors(ignoreErrors);
    }   
    binding.initialize();
}, 
false);
Sys.UI.DataView = function Sys$UI$DataView(element) {
    /// <summary locid="M:J#Sys.UI.DataView.#ctor" />
    /// <param name="element"></param>
    var e = Function._validateParams(arguments, [
        {name: "element"}
    ]);
    if (e) throw e;
    Sys.UI.DataView.initializeBase(this, [element]);
}
    function Sys$UI$DataView$add_command(handler) {
    /// <summary locid="E:J#Sys.UI.DataView.command" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("command", handler);
    }
    function Sys$UI$DataView$remove_command(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("command", handler);
    }
    function Sys$UI$DataView$add_dataLoading(handler) {
    /// <summary locid="E:J#Sys.UI.DataView.dataLoading" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("dataLoading", handler);
    }
    function Sys$UI$DataView$remove_dataLoading(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("dataLoading", handler);
    }
    function Sys$UI$DataView$add_itemCreated(handler) {
    /// <summary locid="E:J#Sys.UI.DataView.itemCreated" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("itemCreated", handler);
    }
    function Sys$UI$DataView$remove_itemCreated(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("itemCreated", handler);
    }
    function Sys$UI$DataView$add_fetchFailed(handler) {
    /// <summary locid="E:J#Sys.UI.DataView.fetchFailed" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("fetchFailed", handler);
    }
    function Sys$UI$DataView$remove_fetchFailed(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("fetchFailed", handler);
    }
    function Sys$UI$DataView$add_fetchSucceeded(handler) {
    /// <summary locid="E:J#Sys.UI.DataView.fetchSucceeded" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("fetchSucceeded", handler);
    }
    function Sys$UI$DataView$remove_fetchSucceeded(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("fetchSucceeded", handler);
    }
    function Sys$UI$DataView$get_data() {
        /// <value mayBeNull="true" locid="P:J#Sys.UI.DataView.data"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._data;
    }
    function Sys$UI$DataView$set_data(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        if (!this._setData || (this._data !== value)) {
            this._loadData(value);
        }
    }
    function Sys$UI$DataView$get_dataProvider() {
        /// <value mayBeNull="true" locid="P:J#Sys.UI.DataView.dataProvider"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._provider || null;
    }
    function Sys$UI$DataView$set_dataProvider(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        this._dataContext = this._dataProvider = this._wsp = this._wspClass = null;
        if (Sys.Data.DataContext.isInstanceOfType(value)) {
            this._dataContext = value;
            this._dataProvider = value;
        }
        else if (Sys.Data.IDataProvider.isImplementedBy(value)) {
            this._dataProvider = value;
        }
        else if (Sys.Net.WebServiceProxy.isInstanceOfType(value)) {
            this._wsp = value;
        }
        else if (Type.isClass(value) && value.inheritsFrom(Sys.Net.WebServiceProxy) && typeof(value.get_path) === "function") {
            this._wspClass = value;
        }
        else if ((value !== null) && (typeof(value) !== "string")) {
            throw Error.argument("dataProvider", Sys.UI.TemplatesRes.invalidDataProviderType);
        }
        this._provider = value;
        if (this.get_autoFetch() && this._isActive()) {
            if (value) {
                this._doAutoFetch();
            }
        }
        else {
            this._stale = true;
        }
    }
    function Sys$UI$DataView$get_autoFetch() {
        /// <value locid="P:J#Sys.UI.DataView.autoFetch"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._autoFetch;
    }
    function Sys$UI$DataView$set_autoFetch(value) {
        var e = Function._validateParams(arguments, [{name: "value"}]);
        if (e) throw e;
        var was = this._autoFetch;
        if (typeof(value) === "string") {
            value = Boolean.parse(value);
        }
        else if (typeof(value) !== "boolean") {
            throw Error.invalidOperation(Sys.UI.TemplatesRes.stringOrBoolean);
        }
        this._autoFetch = value;
        if (this._isActive() && this._stale && !was && value) {
            this._doAutoFetch();
        }
    }
    function Sys$UI$DataView$get_isFetching() {
        /// <value type="Boolean" locid="P:J#Sys.UI.DataView.isFetching"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._fetching;
    }
    function Sys$UI$DataView$get_httpVerb() {
        /// <value type="String" locid="P:J#Sys.UI.DataView.httpVerb"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._httpVerb || "POST";
    }
    function Sys$UI$DataView$set_httpVerb(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String}]);
        if (e) throw e;
        this._httpVerb = value;
    }
    function Sys$UI$DataView$get_items() {
        /// <value type="Array" elementType="Sys.UI.TemplateContext" locid="P:J#Sys.UI.DataView.items"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._results;
    }
    function Sys$UI$DataView$get_fetchParameters() {
        /// <value type="Object" mayBeNull="true" locid="P:J#Sys.UI.DataView.fetchParameters"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._fetchParameters;
    }
    function Sys$UI$DataView$set_fetchParameters(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Object, mayBeNull: true}]);
        if (e) throw e;
        if (this._fetchParameters !== value) {
            this._fetchParameters = value;
            if (this.get_autoFetch() && this._isActive()) {
                this._doAutoFetch();
            }
            else {
                this._stale = true;
            }
        }
    }
    function Sys$UI$DataView$get_selectedData() {
        /// <value mayBeNull="true" locid="P:J#Sys.UI.DataView.selectedData"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        var selectedIndex = this.get_selectedIndex();
        if (selectedIndex > -1) {
            var data = this.get_data();
            if ((data instanceof Array) && (selectedIndex < data.length)) {
                return data[selectedIndex];
            }
        }
        return null;
    }
    function Sys$UI$DataView$get_selectedIndex() {
        /// <value locid="P:J#Sys.UI.DataView.selectedIndex"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._selectedIndex;
    }
    function Sys$UI$DataView$set_selectedIndex(value) {
        var e = Function._validateParams(arguments, [{name: "value"}]);
        if (e) throw e;
        value = this._validateIndexInput(value);
        if (value < -1) {
            throw Error.argumentOutOfRange("value", value);
        }
        if (!this.get_isInitialized() || !this._setData) {
            this._selectedIndex = value;
        }
        else {
            this._applySelectedIndex(value);
        }
    }
    function Sys$UI$DataView$get_initialSelectedIndex() {
        /// <value locid="P:J#Sys.UI.DataView.initialSelectedIndex"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._initialSelectedIndex;
    }
    function Sys$UI$DataView$set_initialSelectedIndex(value) {
        var e = Function._validateParams(arguments, [{name: "value"}]);
        if (e) throw e;
        value = this._validateIndexInput(value);
        if (value < -1) {
            throw Error.argumentOutOfRange("value", value);
        }
        if (value !== this.get_initialSelectedIndex()) {
            this._initialSelectedIndex = value;
            this._raiseChanged("initialSelectedIndex");
        }
    }
    function Sys$UI$DataView$get_selectedItemClass() {
        /// <value type="String" locid="P:J#Sys.UI.DataView.selectedItemClass"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._selectedItemClass || "";
    }
    function Sys$UI$DataView$set_selectedItemClass(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String}]);
        if (e) throw e;
        var name = this.get_selectedItemClass();
        if (value !== name) {
            var index = this.get_selectedIndex();
            this._addRemoveCssClass(index, name, Sys.UI.DomElement.removeCssClass);
            this._addRemoveCssClass(index, value, Sys.UI.DomElement.addCssClass);
            this._selectedItemClass = value;
        }
    }
    function Sys$UI$DataView$get_timeout() {
        /// <value type="Number" integer="true" locid="P:J#Sys.UI.DataView.timeout"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._timeout;
    }
    function Sys$UI$DataView$set_timeout(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Number, integer: true}]);
        if (e) throw e;
        this._timeout = value;
    }
    function Sys$UI$DataView$get_fetchOperation() {
        /// <value mayBeNull="true" locid="P:J#Sys.UI.DataView.fetchOperation"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._query || "";
    }
    function Sys$UI$DataView$set_fetchOperation(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        if (this._query !== value) {
            this._query = value;
            if (this.get_autoFetch() && this._isActive()) {
                if (value) {
                    this._doAutoFetch();
                }
            }
            else {
                this._stale = true;
            }
        }
    }
    function Sys$UI$DataView$get_itemPlaceholder() {
        /// <value mayBeNull="true" locid="P:J#Sys.UI.DataView.itemPlaceholder"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._placeholder || null;
    }
    function Sys$UI$DataView$set_itemPlaceholder(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        if (this._placeholder !== value) {
            this._placeholder = value;
            this._container = null;
            this._dirty = true;
            this._raiseChanged("itemPlaceholder");
        }
    }
    function Sys$UI$DataView$get_templateContext() {
        /// <value mayBeNull="true" type="Sys.UI.TemplateContext" locid="P:J#Sys.UI.DataView.templateContext"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._parentContext || null;
    }
    function Sys$UI$DataView$set_templateContext(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Sys.UI.TemplateContext, mayBeNull: true}]);
        if (e) throw e;
        if (this._parentContext !== value) {
            this._parentContext = value;
            this._dirty = true;
            this._raiseChanged("templateContext");
        }
    }
    function Sys$UI$DataView$get_itemTemplate() {
        /// <value mayBeNull="true" locid="P:J#Sys.UI.DataView.itemTemplate"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._template || null;
    }
    function Sys$UI$DataView$set_itemTemplate(value) {
        var e = Function._validateParams(arguments, [{name: "value", mayBeNull: true}]);
        if (e) throw e;
        if (this._template !== value) {
            this._template = value;
            this._dirty = true;
            if (this._dvTemplate) {
                this._dvTemplate.dispose();
                this._dvTemplate = null;
            }
            if (this._isActive()) {
                this.raisePropertyChanged("itemTemplate");
                if (this._setData) {
                    this.refresh();
                }
            }
            else {
                this._changed = true;
            }
        }
    }
    function Sys$UI$DataView$_applySelectedIndex(value, force) {
        var currentIndex = this.get_selectedIndex();
        if (force || (value !== currentIndex)) {
            var data = this.get_data(), data = data instanceof Array ? data : [data],
                outOfRange = (value < -1) || (value > (data instanceof Array ? data.length-1 : -1));
            if (outOfRange) {
                throw Error.argumentOutOfRange("value", value);
            }
            this._selectedIndex = value;
            var previousData = this._currentSelectedData;
            this._currentSelectedData = ((value === -1) || outOfRange) ? null : data[value];
            var className = this.get_selectedItemClass();
            this._addRemoveCssClass(currentIndex, className, Sys.UI.DomElement.removeCssClass);
            this._addRemoveCssClass(value, className, Sys.UI.DomElement.addCssClass);
            if (!this.get_isUpdating()) {
                if (value !== currentIndex) {
                    this.raisePropertyChanged('selectedIndex');
                }
                if (previousData !== this._currentSelectedData) {
                    this.raisePropertyChanged('selectedData');
                }
            }
            else {
                this._changed = true;
            }
        }
    }
    function Sys$UI$DataView$_addRemoveCssClass(index, className, addRemove) {
        if (className && (index > -1)) {
            var items = this.get_items(), l = items ? items.length : -1;
            if (l && (index < l)) {
                var elementsSet = items[index].nodes;
                if (elementsSet) {
                    for (var i = 0, len = elementsSet.length; i < len; i++) {
                        var element = elementsSet[i];
                        if (element.nodeType === 1) {
                            addRemove(element, className);
                        }
                    }
                }
            }
        }
    }
    function Sys$UI$DataView$_clearContainer(c, ref, count) {
        if (ref === 0) {
            this._clearElement(c);
        }
        else {
            this._removeChildren(c, ref, count, true);
        }
    }
    function Sys$UI$DataView$_clearElement(e) {
        Sys.Application.disposeElement(e, true);
        if (this._useRemove) {
            this._removeChildren(e);
        }
        else {
            try {
                e.innerHTML = "";
            }
            catch (err) {
                this._removeChildren(e);
                this._useRemove = true;
            }
        }
    }
    function Sys$UI$DataView$_collectionChanged(sender, args) {
        var data = this.get_data(), changes = args.get_changes(),
            selectedIndex = this.get_selectedIndex(), oldIndex = selectedIndex;
        if (this._isActive()) {
            this.refresh();
        }
        else {
            this._dirty = true;
            return;
        }
        if ((selectedIndex !== -1) && (selectedIndex < data.length) &&
            (data[selectedIndex] === this._currentSelectedData)) {
            return;
        }
        for (var i = 0, l = changes.length; i < l; i++) {
            var change = changes[i];
            if (change.action === Sys.NotifyCollectionChangedAction.add) {
                if (selectedIndex >= change.newStartingIndex) {
                    selectedIndex += change.newItems.length;
                }
            }
            else {
                var index = change.oldStartingIndex, len = change.oldItems.length, lastIndex = index + len - 1;
                if (selectedIndex > lastIndex) {
                    selectedIndex -= len;
                }
                else if (selectedIndex >= index) {
                    selectedIndex = -1;
                    break;
                }
            }
        }
        if (selectedIndex !== oldIndex) {
            this.set_selectedIndex(selectedIndex);
        }
    }
    function Sys$UI$DataView$_elementContains(container, element, excludeSelf) {
        if (container === element) {
            return !excludeSelf;
        }
        do {
            element = element.parentNode;
            if (element === container) return true;
        }
        while (element);
        return false;
    }
    function Sys$UI$DataView$_raiseChanged(name) {
        if (this._isActive()) {
            this.raisePropertyChanged(name);
        }
        else {
            this._changed = true;
        }
    }
    function Sys$UI$DataView$_raiseFailed(request, error) {
	    var args = new Sys.Net.WebRequestEventArgs(request ? request.get_executor() : null, error);
        this.onFetchFailed(args);
        this._raiseEvent("fetchFailed", args);
    }
    function Sys$UI$DataView$_raiseSucceeded(request, result) {
	    var args = new Sys.Net.WebRequestEventArgs(request ? request.get_executor() : null, null, result);
        this.onFetchSucceeded(args);
        this._raiseEvent("fetchSucceeded", args);
    }
    function Sys$UI$DataView$_removeChildren(e, ref, count, dispose) {
        if ((ref === 0) || typeof(count) === "undefined") {
            while (e.firstChild) {
                if (dispose) {
                    Sys.Application.disposeElement(e.firstChild, false);
                }
                e.removeChild(e.firstChild);
            }
        }
        else {
            while (count--) {
                var c = ref ? ref.previousSibling : e.lastChild;
                if (dispose) {
                    Sys.Application.disposeElement(c, false);
                }
                e.removeChild(c);
            }
        }
    }
    function Sys$UI$DataView$_getTemplate() {
        if (this._dvTemplate) return this._dvTemplate;
        var template = this.get_itemTemplate();
        if (!template) {
            var element = this.get_element();
            if (Sys.UI.Template._isTemplate(element)) {
                this._dvTemplate = template = new Sys.UI.Template(element);
            }
        }
        else if (!Sys.UI.Template.isInstanceOfType(template)) {
            template = Sys.UI.DomElement.resolveElement(template);
            var e = this.get_element();
            if ((e !== template) && this._elementContains(e, template, true)) {
                throw Error.invalidOperation(Sys.UI.TemplatesRes.misplacedTemplate);
            }
            this._dvTemplate = template = new Sys.UI.Template(template);
        }
        else {
            if (this._elementContains(this.get_element(), template.get_element(), true)) {
                throw Error.invalidOperation(Sys.UI.TemplatesRes.misplacedTemplate);
            }
        }
        return template;
    }
    function Sys$UI$DataView$_loadData(value) {
        var args = new Sys.Data.DataEventArgs(value);
        this.onDataLoading(args);
        this._raiseEvent("dataLoading", args);
        if (args.get_cancel()) {
            return false;
        }
        value = args.get_data();
        var currentSelectedData = this.get_selectedData();
        this._swapData(this._data, value);
        this._data = value;
        var setData = this._setData;
        this._stale = false;
        this._dirty = this._setData = true;
        if (this.get_isInitialized()) {
            var isReset;
            if (!setData) {
                var selectedIndex = this.get_selectedIndex();
                if (selectedIndex !== -1) {
                    this._applySelectedIndex(selectedIndex, true);
                }
                else {
                    isReset = this._resetSelectedIndex();
                }
            }
            else {
                isReset = this._resetSelectedIndex();
            }
            if (!this.get_isUpdating()) {
                this.refresh();
                this.raisePropertyChanged("data");
                if (!isReset && (currentSelectedData !== this.get_selectedData())) {
                    this.raisePropertyChanged("selectedData");
                }
                return true;
            }
        }
        this._changed = true;
        return true;
    }
    function Sys$UI$DataView$_resetSelectedIndex() {
        var data = this.get_data(), initialSelectedIndex = this.get_initialSelectedIndex(),
            selectedIndex = this.get_selectedIndex();
        if (!(data instanceof Array) || (initialSelectedIndex >= data.length)) {
            if (selectedIndex !== -1) {
                this.set_selectedIndex(-1);
                return true;
            }
        } 
        else if (selectedIndex !== initialSelectedIndex) {
            this.set_selectedIndex(initialSelectedIndex);
            return true;
        }
        return false;
    }
    function Sys$UI$DataView$_resolveContainer() {
        if (!this._container) {
            var e, ref, container, ph = Sys.UI.DomElement.resolveElement(this.get_itemPlaceholder());
            if (ph) {
                if (!this._elementContains(this.get_element(), ph)) {
                    throw Error.invalidOperation(Sys.UI.TemplatesRes.misplacedPlaceholder);
                }
                container = ph.parentNode;
                ref = ph.nextSibling;
            }
            else {
                container = this.get_element();
                ref = 0; 
            }
            this._container = container;
            this._refNode = ref;
        }
    }
    function Sys$UI$DataView$_initializeResults() {
        for (var i = 0, l = this._results.length; i < l; i++) {
            this._results[i].initializeComponents();
        }
    }
    function Sys$UI$DataView$_isActive() {
        return (this.get_isInitialized() && !this.get_isUpdating());
    }
    function Sys$UI$DataView$_raiseEvent(name, args) {
        var handler = this.get_events().getHandler(name);
        if (handler) {
            handler(this, args);
        }
    }
    function Sys$UI$DataView$_raiseCommand(args) {
        this.onCommand(args);
        this._raiseEvent("command", args);
    }
    function Sys$UI$DataView$_raiseItemCreated(args) {
        this.onItemCreated(args);
        this._raiseEvent("itemCreated", args);
    }
    function Sys$UI$DataView$abortFetch() {
        /// <summary locid="M:J#Sys.UI.DataView.abortFetch" />
        if (arguments.length !== 0) throw Error.parameterCount();
        if (this._request) {
            this._request.get_executor().abort();
            this._request = null;
        }
        if (this._fetching) {
            this._fetching = false;
            this._raiseChanged("isFetching");
        }
    }
    function Sys$UI$DataView$onBubbleEvent(source, args) {
        /// <summary locid="M:J#Sys.UI.DataView.onBubbleEvent" />
        /// <param name="source"></param>
        /// <param name="args" type="Sys.EventArgs"></param>
        /// <returns type="Boolean"></returns>
        var e = Function._validateParams(arguments, [
            {name: "source"},
            {name: "args", type: Sys.EventArgs}
        ]);
        if (e) throw e;
        if (Sys.CommandEventArgs.isInstanceOfType(args)) {
            this._raiseCommand(args);
            if (args.get_cancel()) {
                return true;
            }
            else {
                var name = args.get_commandName();
                if (name && (name.toLowerCase() === "select")) {
                    var index = this._getItemIndex(source);
                    if (index !== -1) {
                        this.set_selectedIndex(index);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    function Sys$UI$DataView$onDataLoading(args) {
        /// <summary locid="M:J#Sys.UI.DataView.onDataLoading" />
        /// <param name="args" type="Sys.Data.DataEventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.Data.DataEventArgs}
        ]);
        if (e) throw e;
    }
    function Sys$UI$DataView$onFetchFailed(args) {
        /// <summary locid="M:J#Sys.UI.DataView.onFetchFailed" />
        /// <param name="args" type="Sys.Net.WebRequestEventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.Net.WebRequestEventArgs}
        ]);
        if (e) throw e;
    }
    function Sys$UI$DataView$onFetchSucceeded(args) {
        /// <summary locid="M:J#Sys.UI.DataView.onFetchSucceeded" />
        /// <param name="args" type="Sys.Net.WebRequestEventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.Net.WebRequestEventArgs}
        ]);
        if (e) throw e;
    }
    function Sys$UI$DataView$_doAutoFetch() {
        try {
            if (this._dataProvider || (this._provider && this.get_fetchOperation())) {
                this.fetchData();
                this._stale = false;
            }
        }
        catch (e) {
            this._raiseFailed(null, null);
        }
    }
    function Sys$UI$DataView$_getItemIndex(source) {
        if (source && this._currentContainer) {
            var results = this.get_items();
            if (results) {
                var element;
                if (typeof(source) === "string") {
                    element = Sys.UI.DomElement.getElementById(source);
                } 
                else if (Sys.UI.Control.isInstanceOfType(source) || Sys.UI.Behavior.isInstanceOfType(source)) {
                    element = source.get_element();
                } 
                else if (Sys.UI.DomElement.isDomElement(source)) {
                    element = source;
                }
                if (element) {
                    var parent = element.parentNode, dvElement = this.get_element();
                    while (parent && (parent !== this._currentContainer) && (parent !== dvElement)) {
                        element = parent;
                        parent = parent.parentNode;
                    }
                    if (parent === this._currentContainer) { 
                        for (var i = 0, l = results.length; i < l; i++) {
                            if (Array.contains(results[i].nodes, element)) {
                                return i;
                            }
                        }
                    }
                }
            }
        }
        return -1;
    }
    function Sys$UI$DataView$getItem(source) {
        /// <summary locid="M:J#Sys.UI.DataView.getItem" />
        /// <param name="source"></param>
        /// <returns type="Sys.UI.TemplateContext" mayBeNull="true"></returns>
        var e = Function._validateParams(arguments, [
            {name: "source"}
        ]);
        if (e) throw e;
        if (!source || ((typeof(source) !== "string") && !Sys.UI.Control.isInstanceOfType(source) 
            && !Sys.UI.Behavior.isInstanceOfType(source) && !Sys.UI.DomElement.isDomElement(source))) {
            throw Error.argument(Sys.UI.TemplatesRes.invalidSource);
        }
        var index = this._getItemIndex(source);
        return (index !== -1) ? this.get_items()[index] : null;
    }
    function Sys$UI$DataView$refresh() {
        /// <summary locid="M:J#Sys.UI.DataView.refresh" />
        if (arguments.length !== 0) throw Error.parameterCount();
        var template = this._getTemplate();
        if (!template) return;
        this._dirty = false;
        this._resolveContainer();
        var data = this.get_data(),
            pctx = this.get_templateContext(),
            element = this.get_element(),
            container = this._container,
            currentContainer = this._currentContainer,
            result;
        if (currentContainer) {
            this._clearContainer(currentContainer, this._currentRef, this._currentCount);
        }
        template.compile();
        if (currentContainer !== this._container) {
            this._useRemove = false;
            this._clearContainer(container, this._refNode, 1);
        }
        this._currentContainer = container;
        this._currentRef = this._refNode;
        this._currentCount = 0;
        if (template.get_element() === element) {
            Sys.UI.DomElement.removeCssClass(element, "sys-template");
        }
        if ((data === null) || (typeof(data) === "undefined")) {
            this._results = [];
        }
        else if (data instanceof Array) {
            var len = data.length;
            this._results = new Array(len);
            for (var i = 0; i < len; i++) {
                var item = data[i];
                result = template.instantiateIn(container, item, i, this._currentRef, pctx);
                if (this._currentRef !== 0) {
                    this._currentCount += result.nodes.length;
                }
                this._raiseItemCreated(new Sys.UI.DataViewItemEventArgs(item, result));
                this._results[i] = result;
            }
        }
        else {
            result = template.instantiateIn(container, data, 0, this._currentRef, pctx);
            if (this._currentRef !== 0) {
                this._currentCount = result.nodes.length;
            }
            this.onItemCreated(new Sys.UI.DataViewItemEventArgs(data, result));
            this._results = [result];
        }
        var selectedClass = this.get_selectedItemClass();
        if (selectedClass) {
            var selectedIndex = this.get_selectedIndex();
            if (selectedIndex !== -1) {
                this._addRemoveCssClass(selectedIndex, selectedClass, Sys.UI.DomElement.addCssClass);
            }
        }
        this._initializeResults();
    }
    function Sys$UI$DataView$_swapData(oldData, newData) {
        if (oldData) {
            switch (this._eventType) {
                case 1:
                    oldData.remove_collectionChanged(this._changedHandler);
                    break;
                case 2:
                    Sys.Observer.removeCollectionChanged(oldData, this._changedHandler);
                    break;
            }
        }
        this._eventType = 0;
        if (newData) {
            if (!this._changedHandler) {
                this._changedHandler = Function.createDelegate(this, this._collectionChanged);
            }
            if (typeof(newData.add_collectionChanged) === "function") {
                newData.add_collectionChanged(this._changedHandler);
                this._eventType = 1;
            }
            else if (newData instanceof Array) {
                Sys.Observer.addCollectionChanged(newData, this._changedHandler);
                this._eventType = 2;
            }
        }
    }
    function Sys$UI$DataView$_validateIndexInput(value) {
        var type = typeof(value);
        if (type === "string") {
            value = parseInt(value);
            if (isNaN(value)) {
                throw Error.argument(Sys.UI.TemplatesRes.invalidSelectedIndexValue);
            }
        }
        else if (type !== "number") {
            throw Error.argument(Sys.UI.TemplatesRes.invalidSelectedIndexValue);
        }
        return value;
    }
    function Sys$UI$DataView$dispose() {
        /// <summary locid="M:J#Sys.UI.DataView.dispose" />
        if (arguments.length !== 0) throw Error.parameterCount();
        if (this._currentContainer && !Sys.Application.get_isDisposing()) {
            this._clearContainer(this._currentContainer, this._currentRef, this._currentCount);        
        }
        if (this._dvTemplate) {
            this._dvTemplate.dispose();
        }
        if (this.get_isFetching()) {
            this.abortFetch();
            this._fetching = false;
        }
        this._swapData(this._data, null);
        this._currentSelectedData = this._currentContainer = this._currentRef = this._container = this._placeholder =
        this._results = this._parentContext = this._dvTemplate = this._request = this._dataContext = this._dataProvider =
        this._wsp = this._wspClass = this._provider = this._data = this._fetchParameters = this._query = null;
        Sys.UI.DataView.callBaseMethod(this, "dispose")
    }
    function Sys$UI$DataView$initialize() {
        /// <summary locid="M:J#Sys.UI.DataView.initialize" />
        if (arguments.length !== 0) throw Error.parameterCount();
        Sys.UI.DataView.callBaseMethod(this, "initialize");
        if (this._setData) {
            var selectedIndex = this.get_selectedIndex();
            if (selectedIndex !== -1) {
                this._applySelectedIndex(selectedIndex, true);
            }
            else {
                this._resetSelectedIndex();
            }
        }
        if (this._setData) {
            this.refresh();
        }
        this.updated();
    }
    function Sys$UI$DataView$fetchData(succeededCallback, failedCallback, mergeOption, userContext) {
        /// <summary locid="M:J#Sys.UI.DataView.fetchData" />
        /// <param name="succeededCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="failedCallback" type="Function" mayBeNull="true" optional="true"></param>
        /// <param name="mergeOption" type="Sys.Data.MergeOption" mayBeNull="true" optional="true"></param>
        /// <param name="userContext" mayBeNull="true" optional="true"></param>
        /// <returns type="Sys.Net.WebRequest"></returns>
        var e = Function._validateParams(arguments, [
            {name: "succeededCallback", type: Function, mayBeNull: true, optional: true},
            {name: "failedCallback", type: Function, mayBeNull: true, optional: true},
            {name: "mergeOption", type: Sys.Data.MergeOption, mayBeNull: true, optional: true},
            {name: "userContext", mayBeNull: true, optional: true}
        ]);
        if (e) throw e;
        this._stale = false;
        var request, _this = this;
        function onSuccess(data) {
            _this._loadData(data);
            _this._fetching = false;
            _this._request = null;
            _this._raiseChanged("isFetching");
            _this._raiseSucceeded(request, data);
            if (succeededCallback) {
                succeededCallback(data, userContext, "fetchData");
            }
        }
        function onFail(error) {
            _this._fetching = false;
            _this._request = null;
            _this._raiseChanged("isFetching");
            _this._raiseFailed(request, error);
            if (failedCallback) {
                failedCallback(error, userContext, "fetchData");
            }
        }
        if (this._fetching) {
            this.abortFetch();
        }
        var dataProvider = this._dataProvider,
            wsp = this._wsp,
            wspc =  this._wspClass,
            query = this.get_fetchOperation(),
            parameters = this.get_fetchParameters() || null,
            httpVerb = this.get_httpVerb() || "POST",
            timeout = this.get_timeout() || 0;
        if (typeof(mergeOption) === "undefined") {
            mergeOption = null;
        }
        if (dataProvider) {
            request = dataProvider.fetchData(query, parameters, mergeOption, httpVerb, onSuccess, onFail, timeout, userContext);
        }
        else if (wsp) {
            var path = wsp.get_path();
            if (!path) {
                var type = Object.getType(wsp);
                if (type && (typeof(type.get_path) === "function")) {
                    path = type.get_path();
                }
            }
            request = Sys.Data.DataContext._fetchWSP(null, path, query, parameters, httpVerb, onSuccess, onFail, timeout || wsp.get_timeout());
        }
        else if (wspc) {
            request = Sys.Data.DataContext._fetchWSP(null, wspc.get_path(), query, parameters, httpVerb, onSuccess, onFail, timeout || wspc.get_timeout());
        }
        else {
            request = Sys.Data.DataContext._fetchWSP(null, this._provider, query, parameters, httpVerb, onSuccess, onFail, timeout);
        }
        this._request = request;
        this._fetching = true;
        this._raiseChanged("isFetching");
        return request;
    }
    function Sys$UI$DataView$onCommand(args) {
        /// <summary locid="M:J#Sys.UI.DataView.onCommand" />
        /// <param name="args" type="Sys.CommandEventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.CommandEventArgs}
        ]);
        if (e) throw e;
    }
    function Sys$UI$DataView$onItemCreated(args) {
        /// <summary locid="M:J#Sys.UI.DataView.onItemCreated" />
        /// <param name="args" type="Sys.UI.DataViewItemEventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.UI.DataViewItemEventArgs}
        ]);
        if (e) throw e;
    }
    function Sys$UI$DataView$updated() {
        /// <summary locid="M:J#Sys.UI.DataView.updated" />
        if (arguments.length !== 0) throw Error.parameterCount();
        if (this._stale && this.get_autoFetch()) {
            this._doAutoFetch();
        }
        if (this._dirty && this._setData) {
            this.refresh();
        }
        if (this._changed) {
            this.raisePropertyChanged("");
            this._changed = false;
        }
    }
Sys.UI.DataView.prototype = {
    _autoFetch: false,
    _fetching: false,
    _changed: false,
    _container: null,
    _currentContainer: null,
    _currentRef: null,
    _currentSelectedData: null,
    _data: null,
    _dataContext: null,
    _dataProvider: null,
    _wsp: null,
    _wspClass: null,
    _dirty: false,
    _stale: true,
    _dvTemplate: null,
    _eventType: 0,
    _httpVerb: null,
    _initialSelectedIndex: -1,
    _fetchParameters: null,
    _parentContext: null,
    _placeholder: null,
    _query: null,
    _results: null,
    _selectedIndex: -1,
    _selectedItemClass: null,
    _setData: false,
    _template: null,
    _timeout: 0,
    _request: null,
    _useRemove: false,
    add_command: Sys$UI$DataView$add_command,
    remove_command: Sys$UI$DataView$remove_command,
    add_dataLoading: Sys$UI$DataView$add_dataLoading,
    remove_dataLoading: Sys$UI$DataView$remove_dataLoading,
    add_itemCreated: Sys$UI$DataView$add_itemCreated,
    remove_itemCreated: Sys$UI$DataView$remove_itemCreated,
    add_fetchFailed: Sys$UI$DataView$add_fetchFailed,
    remove_fetchFailed: Sys$UI$DataView$remove_fetchFailed,
    add_fetchSucceeded: Sys$UI$DataView$add_fetchSucceeded,
    remove_fetchSucceeded: Sys$UI$DataView$remove_fetchSucceeded,
    get_data: Sys$UI$DataView$get_data,
    set_data: Sys$UI$DataView$set_data,
    get_dataProvider: Sys$UI$DataView$get_dataProvider,
    set_dataProvider: Sys$UI$DataView$set_dataProvider,
    get_autoFetch: Sys$UI$DataView$get_autoFetch,
    set_autoFetch: Sys$UI$DataView$set_autoFetch,
    get_isFetching: Sys$UI$DataView$get_isFetching,
    get_httpVerb: Sys$UI$DataView$get_httpVerb,
    set_httpVerb: Sys$UI$DataView$set_httpVerb,
    get_items: Sys$UI$DataView$get_items,
    get_fetchParameters: Sys$UI$DataView$get_fetchParameters,
    set_fetchParameters: Sys$UI$DataView$set_fetchParameters,
    get_selectedData: Sys$UI$DataView$get_selectedData,
    get_selectedIndex: Sys$UI$DataView$get_selectedIndex,
    set_selectedIndex: Sys$UI$DataView$set_selectedIndex,
    get_initialSelectedIndex: Sys$UI$DataView$get_initialSelectedIndex,
    set_initialSelectedIndex: Sys$UI$DataView$set_initialSelectedIndex,
    get_selectedItemClass: Sys$UI$DataView$get_selectedItemClass,
    set_selectedItemClass: Sys$UI$DataView$set_selectedItemClass,
    get_timeout: Sys$UI$DataView$get_timeout,
    set_timeout: Sys$UI$DataView$set_timeout,
    get_fetchOperation: Sys$UI$DataView$get_fetchOperation,
    set_fetchOperation: Sys$UI$DataView$set_fetchOperation,    
    get_itemPlaceholder: Sys$UI$DataView$get_itemPlaceholder,
    set_itemPlaceholder: Sys$UI$DataView$set_itemPlaceholder,
    get_templateContext: Sys$UI$DataView$get_templateContext,
    set_templateContext: Sys$UI$DataView$set_templateContext,    
    get_itemTemplate: Sys$UI$DataView$get_itemTemplate,
    set_itemTemplate: Sys$UI$DataView$set_itemTemplate,
    _applySelectedIndex: Sys$UI$DataView$_applySelectedIndex,
    _addRemoveCssClass: Sys$UI$DataView$_addRemoveCssClass,
    _clearContainer: Sys$UI$DataView$_clearContainer,
    _clearElement: Sys$UI$DataView$_clearElement,
    _collectionChanged: Sys$UI$DataView$_collectionChanged,
    _elementContains: Sys$UI$DataView$_elementContains,
    _raiseChanged: Sys$UI$DataView$_raiseChanged,
    _raiseFailed: Sys$UI$DataView$_raiseFailed,
    _raiseSucceeded: Sys$UI$DataView$_raiseSucceeded,
    _removeChildren: Sys$UI$DataView$_removeChildren,
    _getTemplate: Sys$UI$DataView$_getTemplate,
    _loadData: Sys$UI$DataView$_loadData,
    _resetSelectedIndex: Sys$UI$DataView$_resetSelectedIndex,
    _resolveContainer: Sys$UI$DataView$_resolveContainer,
    _initializeResults: Sys$UI$DataView$_initializeResults,    
    _isActive: Sys$UI$DataView$_isActive,
    _raiseEvent: Sys$UI$DataView$_raiseEvent,
    _raiseCommand: Sys$UI$DataView$_raiseCommand,
    _raiseItemCreated: Sys$UI$DataView$_raiseItemCreated,
    abortFetch: Sys$UI$DataView$abortFetch,    
    onBubbleEvent: Sys$UI$DataView$onBubbleEvent,
    onDataLoading: Sys$UI$DataView$onDataLoading,
    onFetchFailed: Sys$UI$DataView$onFetchFailed,
    onFetchSucceeded: Sys$UI$DataView$onFetchSucceeded,
    _doAutoFetch: Sys$UI$DataView$_doAutoFetch,
    _getItemIndex: Sys$UI$DataView$_getItemIndex,
    getItem: Sys$UI$DataView$getItem,
    refresh: Sys$UI$DataView$refresh,
    _swapData: Sys$UI$DataView$_swapData,
    _validateIndexInput: Sys$UI$DataView$_validateIndexInput,
    dispose: Sys$UI$DataView$dispose, 
    initialize: Sys$UI$DataView$initialize,
    fetchData: Sys$UI$DataView$fetchData,
    onCommand: Sys$UI$DataView$onCommand,
    onItemCreated: Sys$UI$DataView$onItemCreated,
    updated: Sys$UI$DataView$updated    
}
Sys.UI.DataView.registerClass("Sys.UI.DataView", Sys.UI.Control, Sys.UI.ITemplateContextConsumer);
Sys.UI.DataViewItemEventArgs = function Sys$UI$DataViewItemEventArgs(dataItem, templateContext) {
    /// <summary locid="M:J#Sys.UI.DataViewItemEventArgs.#ctor" />
    /// <param name="dataItem"></param>
    /// <param name="templateContext" type="Sys.UI.TemplateContext"></param>
    var e = Function._validateParams(arguments, [
        {name: "dataItem"},
        {name: "templateContext", type: Sys.UI.TemplateContext}
    ]);
    if (e) throw e;
    Sys.UI.DataViewItemEventArgs.initializeBase(this);
    this._ctx = templateContext || null;
    this._data = dataItem || null;
}
    function Sys$UI$DataViewItemEventArgs$get_dataItem() {
        /// <value locid="P:J#Sys.UI.DataViewItemEventArgs.dataItem"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._data;
    }
    function Sys$UI$DataViewItemEventArgs$get_templateContext() {
        /// <value type="Sys.UI.TemplateContext" locid="P:J#Sys.UI.DataViewItemEventArgs.templateContext"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._ctx;
    }
Sys.UI.DataViewItemEventArgs.prototype = {
    get_dataItem: Sys$UI$DataViewItemEventArgs$get_dataItem,
    get_templateContext: Sys$UI$DataViewItemEventArgs$get_templateContext
}
Sys.UI.DataViewItemEventArgs.registerClass("Sys.UI.DataViewItemEventArgs", Sys.EventArgs);

Type.registerNamespace('Sys.UI');

Sys.UI.TemplatesRes={
'requiresWebServices':'This operation requires a reference to the MicrosoftAjaxWebServices.js library.',
'commonNotAfterInit':'{0} \'{1}\' cannot be set after initialize.',
'stringOrBoolean':'Value must be the string \"true\", \"false\", or a Boolean.',
'elementNotFound':'An element with id \'{0}\' could not be found.',
'updateBeforeInit':'Update cannot be called before initialize.',
'invalidAttributeName':'Invalid attribute name \'{0}\'. Declared attribute names must be in lowercase.',
'invalidFunctionName':'\'{0}\' must be of type Function or the name of a function as a String.',
'observableConflict':'Object already contains a member with the name \'{0}\'.',
'invalidSource':'Value must be a DOM element, DOM element id, control, or behavior.',
'requiresAdoNetServiceProxy':'AdoNetDataContext requires a reference to the MicrosoftAjaxAdoNet.js library.',
'invalidCommandTarget':'Command target is not a control id or not an expression that can be evaluated as a control reference.',
'invalidDataProviderType':'Value must be a service URI, an instance of Sys.Net.WebServiceProxy, or class that implements Sys.Data.IDataProvider.',
'requiredUri':'A serviceUri must be set prior to calling fetchData.',
'invalidAttach':'Invalid attribute \'{0}\', the type \'{1}\' is not a registered namespace.',
'invalidSysAttribute':'Invalid attribute \'{0}\'.',
'initializeAfterInit':'Initialize cannot be called more than once.',
'mustBeArray':'The property \'{0}\' is not an Array.',
'cannotActivate':'Could not activate element with id \'{0}\', the element could not be found.',
'misplacedTemplate':'DataView item template must not be a child element of the DataView.',
'functionNotFound':'A function with the name \'{0}\' could not be found.',
'bindingPropertyNotSet':'Binding \'{0}\' must be set prior to initialize.',
'expectedElementOrId':'Value must be a DOM element or DOM element Id.',
'requiredMethodProperty':'The \'{0}\' property must be set to a function to use the \'{1}\' operation.',
'invalidSelectedIndexValue':'Value must be a Number or a String that can be converted to a Number.',
'attributeDoesNotSupportExpressions':'Attribute \'{0}\' does not support expressions, use \'sys:{0}\' instead.',
'misplacedPlaceholder':'DataView item placeholder must be a child element of the DataView.',
'mustSetInputElementsExplicitly':'Input elements \'type\' and \'name\' attributes must be explicitly set.',
'invalidTypeNamespace':'Invalid type namespace declaration, \'{0}\' is not a valid type.',
'invalidRelationship':'The relationship type \'{0}\' is not valid, use addLink for one-to-one to one-to-many relationships.',
'notObservable':'Instances of type \'{0}\' cannot be observed.',
'cannotFindMarkupExtension':'A markup extension with the name \'{0}\' could not be found.',
'requiredIdentity':'The entity must have an identity or a new identity must be creatable with the set getNewIdentityMethod.',
'nullReferenceInPath':'Null reference while evaluating data path: \'{0}\'.',
'invalidHandler':'Trying to dispose an invalid handler: \'{0}\'.',
'entityAlreadyExists':'Entity \'{0}\' already exists and cannot be added again.'
};

