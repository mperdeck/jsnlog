/**
* Copyright 2012 Mattijs Perdeck.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

/**
* jsnlog
*
* jsnlog allows you to configure client side logging (in JavaScript programs)
* from your server side web.config. It also makes it easy to have your .Net server
* software receive log messages and log them using your preferred server side logging
* package. Additionally, there are many client specific features, such as enabling
* loggers based on the user agent.
*
* This software is derived from log4javascript 1.4.3
* (see below)
*
* Author: Mattijs Perdeck 
* Version: 1.0.0
* Build date: 18 November 2012
*/

/**
* Copyright 2012 Tim Down.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

/**
 * log4javascript
 *
 * log4javascript is a logging framework for JavaScript based on log4j
 * for Java. This file contains all core log4javascript code and is the only
 * file required to use log4javascript, unless you require support for
 * document.domain, in which case you will also need console.html, which must be
 * stored in the same directory as the main log4javascript.js file.
 *
 * Author: Tim Down <tim@log4javascript.org>
 * Version: 1.4.3
 * Edition: log4javascript_production
 * Build date: 18 September 2012
 * Website: http://log4javascript.org
 */

/* -------------------------------------------------------------------------- */

var jsnlog = (function () {

    function isUndefined(obj) {
        return typeof obj == "undefined";
    }

    /* ---------------------------------------------------------------------- */
    // Custom event support

    function EventSupport() { }

    EventSupport.prototype = {
        eventTypes: [],
        eventListeners: {},
        setEventTypes: function (eventTypesParam) {
            if (eventTypesParam instanceof Array) {
                this.eventTypes = eventTypesParam;
                this.eventListeners = {};
                for (var i = 0, len = this.eventTypes.length; i < len; i++) {
                    this.eventListeners[this.eventTypes[i]] = [];
                }
            } else {
                handleError("jsnlog.EventSupport [" + this + "]: setEventTypes: eventTypes parameter must be an Array");
            }
        },

        addEventListener: function (eventType, listener) {
            if (typeof listener == "function") {
                if (!array_contains(this.eventTypes, eventType)) {
                    handleError("jsnlog.EventSupport [" + this + "]: addEventListener: no event called '" + eventType + "'");
                }
                this.eventListeners[eventType].push(listener);
            } else {
                handleError("jsnlog.EventSupport [" + this + "]: addEventListener: listener must be a function");
            }
        },

        removeEventListener: function (eventType, listener) {
            if (typeof listener == "function") {
                if (!array_contains(this.eventTypes, eventType)) {
                    handleError("jsnlog.EventSupport [" + this + "]: removeEventListener: no event called '" + eventType + "'");
                }
                array_remove(this.eventListeners[eventType], listener);
            } else {
                handleError("jsnlog.EventSupport [" + this + "]: removeEventListener: listener must be a function");
            }
        },

        dispatchEvent: function (eventType, eventArgs) {
            if (array_contains(this.eventTypes, eventType)) {
                var listeners = this.eventListeners[eventType];
                for (var i = 0, len = listeners.length; i < len; i++) {
                    listeners[i](this, eventType, eventArgs);
                }
            } else {
                handleError("jsnlog.EventSupport [" + this + "]: dispatchEvent: no event called '" + eventType + "'");
            }
        }
    };

    /* -------------------------------------------------------------------------- */

    var applicationStartDate = new Date();
    var uniqueId = "jsnlog_" + applicationStartDate.getTime() + "_" +
		Math.floor(Math.random() * 100000000);
    var emptyFunction = function () { };
    var newLine = "\r\n";
    var pageLoaded = false;

    // Create main jsnlog object; this will be assigned public properties
    function JSNLog() { }
    JSNLog.prototype = new EventSupport();

    jsnlog = new JSNLog();
    jsnlog.version = "1.0.0";

    /* -------------------------------------------------------------------------- */
    // Utility functions

    function toStr(obj) {
        if (obj && obj.toString) {
            return obj.toString();
        } else {
            return String(obj);
        }
    }

    function getExceptionMessage(ex) {
        if (ex.message) {
            return ex.message;
        } else if (ex.description) {
            return ex.description;
        } else {
            return toStr(ex);
        }
    }

    // Gets the portion of the URL after the last slash
    function getUrlFileName(url) {
        var lastSlashIndex = Math.max(url.lastIndexOf("/"), url.lastIndexOf("\\"));
        return url.substr(lastSlashIndex + 1);
    }

    // Returns a nicely formatted representation of an error
    function getExceptionStringRep(ex) {
        if (ex) {
            var exStr = "Exception: " + getExceptionMessage(ex);
            try {
                if (ex.lineNumber) {
                    exStr += " on line number " + ex.lineNumber;
                }
                if (ex.fileName) {
                    exStr += " in file " + getUrlFileName(ex.fileName);
                }
            } catch (localEx) {
                logLog.warn("Unable to obtain file and line information for error");
            }
            if (showStackTraces && ex.stack) {
                exStr += newLine + "Stack trace:" + newLine + ex.stack;
            }
            return exStr;
        }
        return null;
    }

    function bool(obj) {
        return Boolean(obj);
    }

    function trim(str) {
        return str.replace(/^\s+/, "").replace(/\s+$/, "");
    }

    function splitIntoLines(text) {
        // Ensure all line breaks are \n only
        var text2 = text.replace(/\r\n/g, "\n").replace(/\r/g, "\n");
        return text2.split("\n");
    }

    var urlEncode = (typeof window.encodeURIComponent != "undefined") ?
		function (str) {
		    return encodeURIComponent(str);
		} :
		function (str) {
		    return escape(str).replace(/\+/g, "%2B").replace(/"/g, "%22").replace(/'/g, "%27").replace(/\//g, "%2F").replace(/=/g, "%3D");
		};

    var urlDecode = (typeof window.decodeURIComponent != "undefined") ?
		function (str) {
		    return decodeURIComponent(str);
		} :
		function (str) {
		    return unescape(str).replace(/%2B/g, "+").replace(/%22/g, "\"").replace(/%27/g, "'").replace(/%2F/g, "/").replace(/%3D/g, "=");
		};

    function array_remove(arr, val) {
        var index = -1;
        for (var i = 0, len = arr.length; i < len; i++) {
            if (arr[i] === val) {
                index = i;
                break;
            }
        }
        if (index >= 0) {
            arr.splice(index, 1);
            return true;
        } else {
            return false;
        }
    }

    function array_contains(arr, val) {
        for (var i = 0, len = arr.length; i < len; i++) {
            if (arr[i] == val) {
                return true;
            }
        }
        return false;
    }

    function extractBooleanFromParam(param, defaultValue) {
        if (isUndefined(param)) {
            return defaultValue;
        } else {
            return bool(param);
        }
    }

    function extractStringFromParam(param, defaultValue) {
        if (isUndefined(param)) {
            return defaultValue;
        } else {
            return String(param);
        }
    }

    function extractIntFromParam(param, defaultValue) {
        if (isUndefined(param)) {
            return defaultValue;
        } else {
            try {
                var value = parseInt(param, 10);
                return isNaN(value) ? defaultValue : value;
            } catch (ex) {
                logLog.warn("Invalid int param " + param, ex);
                return defaultValue;
            }
        }
    }

    function extractFunctionFromParam(param, defaultValue) {
        if (typeof param == "function") {
            return param;
        } else {
            return defaultValue;
        }
    }

    function isError(err) {
        return (err instanceof Error);
    }

    if (!Function.prototype.apply) {
        Function.prototype.apply = function (obj, args) {
            var methodName = "__apply__";
            if (typeof obj[methodName] != "undefined") {
                methodName += String(Math.random()).substr(2);
            }
            obj[methodName] = this;

            var argsStrings = [];
            for (var i = 0, len = args.length; i < len; i++) {
                argsStrings[i] = "args[" + i + "]";
            }
            var script = "obj." + methodName + "(" + argsStrings.join(",") + ")";
            var returnValue = eval(script);
            delete obj[methodName];
            return returnValue;
        };
    }

    if (!Function.prototype.call) {
        Function.prototype.call = function (obj) {
            var args = [];
            for (var i = 1, len = arguments.length; i < len; i++) {
                args[i - 1] = arguments[i];
            }
            return this.apply(obj, args);
        };
    }

    function getListenersPropertyName(eventName) {
        return "__jsnlog_listeners__" + eventName;
    }

    function addEvent(node, eventName, listener, useCapture, win) {
        win = win ? win : window;
        if (node.addEventListener) {
            node.addEventListener(eventName, listener, useCapture);
        } else if (node.attachEvent) {
            node.attachEvent("on" + eventName, listener);
        } else {
            var propertyName = getListenersPropertyName(eventName);
            if (!node[propertyName]) {
                node[propertyName] = [];
                // Set event handler
                node["on" + eventName] = function (evt) {
                    evt = getEvent(evt, win);
                    var listenersPropertyName = getListenersPropertyName(eventName);

                    // Clone the array of listeners to leave the original untouched
                    var listeners = this[listenersPropertyName].concat([]);
                    var currentListener;

                    // Call each listener in turn
                    while ((currentListener = listeners.shift())) {
                        currentListener.call(this, evt);
                    }
                };
            }
            node[propertyName].push(listener);
        }
    }

    function removeEvent(node, eventName, listener, useCapture) {
        if (node.removeEventListener) {
            node.removeEventListener(eventName, listener, useCapture);
        } else if (node.detachEvent) {
            node.detachEvent("on" + eventName, listener);
        } else {
            var propertyName = getListenersPropertyName(eventName);
            if (node[propertyName]) {
                array_remove(node[propertyName], listener);
            }
        }
    }

    function getEvent(evt, win) {
        win = win ? win : window;
        return evt ? evt : win.event;
    }

    function stopEventPropagation(evt) {
        if (evt.stopPropagation) {
            evt.stopPropagation();
        } else if (typeof evt.cancelBubble != "undefined") {
            evt.cancelBubble = true;
        }
        evt.returnValue = false;
    }

    /* ---------------------------------------------------------------------- */
    // Simple logging for jsnlog itself

    var logLog = {
        quietMode: false,

        debugMessages: [],

        setQuietMode: function (quietMode) {
            this.quietMode = bool(quietMode);
        },

        numberOfErrors: 0,

        alertAllErrors: false,

        setAlertAllErrors: function (alertAllErrors) {
            this.alertAllErrors = alertAllErrors;
        },

        debug: function (message) {
            this.debugMessages.push(message);
        },

        displayDebug: function () {
            alert(this.debugMessages.join(newLine));
        },

        warn: function (message, exception) {
        },

        error: function (message, exception) {
            if (++this.numberOfErrors == 1 || this.alertAllErrors) {
                if (!this.quietMode) {
                    var alertMessage = "jsnlog error: " + message;
                    if (exception) {
                        alertMessage += newLine + newLine + "Original error: " + getExceptionStringRep(exception);
                    }
                    alert(alertMessage);
                }
            }
        }
    };
    jsnlog.logLog = logLog;

    jsnlog.setEventTypes(["load", "error"]);

    function handleError(message, exception) {
        jsnlog.dispatchEvent("error", { "message": message, "exception": exception });
    }

    jsnlog.handleError = handleError;

    /* ---------------------------------------------------------------------- */

    var enabled = !((typeof jsnlog_disabled != "undefined") &&
					jsnlog_disabled);

    jsnlog.setEnabled = function (enable) {
        enabled = bool(enable);
    };

    jsnlog.isEnabled = function () {
        return enabled;
    };

    var useTimeStampsInMilliseconds = true;

    jsnlog.setTimeStampsInMilliseconds = function (timeStampsInMilliseconds) {
        useTimeStampsInMilliseconds = bool(timeStampsInMilliseconds);
    };

    jsnlog.isTimeStampsInMilliseconds = function () {
        return useTimeStampsInMilliseconds;
    };


    // This evaluates the given expression in the current scope, thus allowing
    // scripts to access private variables. Particularly useful for testing
    jsnlog.evalInScope = function (expr) {
        return eval(expr);
    };

    var showStackTraces = false;

    jsnlog.setShowStackTraces = function (show) {
        showStackTraces = bool(show);
    };

    /* ---------------------------------------------------------------------- */
    // Levels

    var Level = function (level, name) {
        this.level = level;
        this.name = name;
    };

    Level.prototype = {
        toString: function () {
            return this.name;
        },
        equals: function (level) {
            return this.level == level.level;
        },
        isGreaterOrEqual: function (level) {
            return this.level >= level.level;
        }
    };

    Level.ALL = new Level(Number.MIN_VALUE, "ALL");
    Level.TRACE = new Level(10000, "TRACE");
    Level.DEBUG = new Level(20000, "DEBUG");
    Level.INFO = new Level(30000, "INFO");
    Level.WARN = new Level(40000, "WARN");
    Level.ERROR = new Level(50000, "ERROR");
    Level.FATAL = new Level(60000, "FATAL");
    Level.OFF = new Level(Number.MAX_VALUE, "OFF");

    jsnlog.Level = Level;

    /* ---------------------------------------------------------------------- */
    // Timers

    function Timer(name, level) {
        this.name = name;
        this.level = isUndefined(level) ? Level.INFO : level;
        this.start = new Date();
    }

    Timer.prototype.getElapsedTime = function () {
        return new Date().getTime() - this.start.getTime();
    };

    /* ---------------------------------------------------------------------- */
    // Loggers

    var anonymousLoggerName = "[anonymous]";
    var defaultLoggerName = "[default]";
    var nullLoggerName = "[null]";
    var rootLoggerName = "root";

    function Logger(name) {
        this.name = name;
        this.parent = null;
        this.children = [];

        var appenders = [];
        var loggerLevel = null;
        var isRoot = (this.name === rootLoggerName);
        var isNull = (this.name === nullLoggerName);

        var appenderCache = null;
        var appenderCacheInvalidated = false;

        this.addChild = function (childLogger) {
            this.children.push(childLogger);
            childLogger.parent = this;
            childLogger.invalidateAppenderCache();
        };

        // Additivity
        var additive = true;
        this.getAdditivity = function () {
            return additive;
        };

        this.setAdditivity = function (additivity) {
            var valueChanged = (additive != additivity);
            additive = additivity;
            if (valueChanged) {
                this.invalidateAppenderCache();
            }
        };

        // Create methods that use the appenders variable in this scope
        this.addAppender = function (appender) {
            if (isNull) {
                handleError("Logger.addAppender: you may not add an appender to the null logger");
            } else {
                if (appender instanceof jsnlog.Appender) {
                    if (!array_contains(appenders, appender)) {
                        appenders.push(appender);
                        appender.setAddedToLogger(this);
                        this.invalidateAppenderCache();
                    }
                } else {
                    handleError("Logger.addAppender: appender supplied ('" +
						toStr(appender) + "') is not a subclass of Appender");
                }
            }
        };

        this.removeAppender = function (appender) {
            array_remove(appenders, appender);
            appender.setRemovedFromLogger(this);
            this.invalidateAppenderCache();
        };

        this.removeAllAppenders = function () {
            var appenderCount = appenders.length;
            if (appenderCount > 0) {
                for (var i = 0; i < appenderCount; i++) {
                    appenders[i].setRemovedFromLogger(this);
                }
                appenders.length = 0;
                this.invalidateAppenderCache();
            }
        };

        this.getEffectiveAppenders = function () {
            if (appenderCache === null || appenderCacheInvalidated) {
                // Build appender cache
                var parentEffectiveAppenders = (isRoot || !this.getAdditivity()) ?
					[] : this.parent.getEffectiveAppenders();
                appenderCache = parentEffectiveAppenders.concat(appenders);
                appenderCacheInvalidated = false;
            }
            return appenderCache;
        };

        this.invalidateAppenderCache = function () {
            appenderCacheInvalidated = true;
            for (var i = 0, len = this.children.length; i < len; i++) {
                this.children[i].invalidateAppenderCache();
            }
        };

        this.log = function (level, params) {
            if (enabled && level.isGreaterOrEqual(this.getEffectiveLevel())) {
                // Check whether last param is an exception
                var exception;
                var finalParamIndex = params.length - 1;
                var lastParam = params[finalParamIndex];
                if (params.length > 1 && isError(lastParam)) {
                    exception = lastParam;
                    finalParamIndex--;
                }

                // Construct genuine array for the params
                var messages = [];
                for (var i = 0; i <= finalParamIndex; i++) {
                    messages[i] = params[i];
                }

                var loggingEvent = new LoggingEvent(
					this, new Date(), level, messages, exception);

                this.callAppenders(loggingEvent);
            }
        };

        this.callAppenders = function (loggingEvent) {
            var effectiveAppenders = this.getEffectiveAppenders();
            for (var i = 0, len = effectiveAppenders.length; i < len; i++) {
                effectiveAppenders[i].doAppend(loggingEvent);
            }
        };

        this.setLevel = function (level) {
            // Having a level of null on the root logger would be very bad.
            if (isRoot && level === null) {
                handleError("Logger.setLevel: you cannot set the level of the root logger to null");
            } else if (level instanceof Level) {
                loggerLevel = level;
            } else {
                handleError("Logger.setLevel: level supplied to logger " +
					this.name + " is not an instance of jsnlog.Level");
            }
        };

        this.getLevel = function () {
            return loggerLevel;
        };

        this.getEffectiveLevel = function () {
            for (var logger = this; logger !== null; logger = logger.parent) {
                var level = logger.getLevel();
                if (level !== null) {
                    return level;
                }
            }
        };

        var timers = {};

        this.time = function (name, level) {
            if (enabled) {
                if (isUndefined(name)) {
                    handleError("Logger.time: a name for the timer must be supplied");
                } else if (level && !(level instanceof Level)) {
                    handleError("Logger.time: level supplied to timer " +
						name + " is not an instance of jsnlog.Level");
                } else {
                    timers[name] = new Timer(name, level);
                }
            }
        };

        this.timeEnd = function (name) {
            if (enabled) {
                if (isUndefined(name)) {
                    handleError("Logger.timeEnd: a name for the timer must be supplied");
                } else if (timers[name]) {
                    var timer = timers[name];
                    var milliseconds = timer.getElapsedTime();
                    this.log(timer.level, ["Timer " + toStr(name) + " completed in " + milliseconds + "ms"]);
                    delete timers[name];
                } else {
                    logLog.warn("Logger.timeEnd: no timer found with name " + name);
                }
            }
        };

        this.assertEqual = function (v1, v2) {
            if (enabled && (v1 != v2)) {
                this.log(Level.ERROR, ["assertEqual: " + v1 + " != " + v2]);
            }
        };

        this.toString = function () {
            return "Logger[" + this.name + "]";
        };
    }

    Logger.prototype = {
        trace: function () {
            this.log(Level.TRACE, arguments);
        },

        debug: function () {
            this.log(Level.DEBUG, arguments);
        },

        info: function () {
            this.log(Level.INFO, arguments);
        },

        warn: function () {
            this.log(Level.WARN, arguments);
        },

        error: function () {
            this.log(Level.ERROR, arguments);
        },

        fatal: function () {
            this.log(Level.FATAL, arguments);
        },

        isEnabledFor: function (level) {
            return level.isGreaterOrEqual(this.getEffectiveLevel());
        },

        isTraceEnabled: function () {
            return this.isEnabledFor(Level.TRACE);
        },

        isDebugEnabled: function () {
            return this.isEnabledFor(Level.DEBUG);
        },

        isInfoEnabled: function () {
            return this.isEnabledFor(Level.INFO);
        },

        isWarnEnabled: function () {
            return this.isEnabledFor(Level.WARN);
        },

        isErrorEnabled: function () {
            return this.isEnabledFor(Level.ERROR);
        },

        isFatalEnabled: function () {
            return this.isEnabledFor(Level.FATAL);
        }
    };

    Logger.prototype.trace.isEntryPoint = true;
    Logger.prototype.debug.isEntryPoint = true;
    Logger.prototype.info.isEntryPoint = true;
    Logger.prototype.warn.isEntryPoint = true;
    Logger.prototype.error.isEntryPoint = true;
    Logger.prototype.fatal.isEntryPoint = true;

    /* ---------------------------------------------------------------------- */
    // Logger access methods

    // Hashtable of loggers keyed by logger name
    var loggers = {};
    var loggerNames = [];

    var ROOT_LOGGER_DEFAULT_LEVEL = Level.DEBUG;
    var rootLogger = new Logger(rootLoggerName);
    rootLogger.setLevel(ROOT_LOGGER_DEFAULT_LEVEL);

    jsnlog.getRootLogger = function () {
        return rootLogger;
    };

    jsnlog.getLogger = function (loggerName) {
        // Use default logger if loggerName is not specified or invalid
        if (!(typeof loggerName == "string")) {
            loggerName = anonymousLoggerName;
            logLog.warn("jsnlog.getLogger: non-string logger name " +
				toStr(loggerName) + " supplied, returning anonymous logger");
        }

        // Do not allow retrieval of the root logger by name
        if (loggerName == rootLoggerName) {
            handleError("jsnlog.getLogger: root logger may not be obtained by name");
        }

        // Create the logger for this name if it doesn't already exist
        if (!loggers[loggerName]) {
            var logger = new Logger(loggerName);
            loggers[loggerName] = logger;
            loggerNames.push(loggerName);

            // Set up parent logger, if it doesn't exist
            var lastDotIndex = loggerName.lastIndexOf(".");
            var parentLogger;
            if (lastDotIndex > -1) {
                var parentLoggerName = loggerName.substring(0, lastDotIndex);
                parentLogger = jsnlog.getLogger(parentLoggerName); // Recursively sets up grandparents etc.
            } else {
                parentLogger = rootLogger;
            }
            parentLogger.addChild(logger);
        }
        return loggers[loggerName];
    };

    /* ---------------------------------------------------------------------- */
    // Logging events

    var LoggingEvent = function (logger, timeStamp, level, messages,
			exception) {
        this.logger = logger;
        this.timeStamp = timeStamp;
        this.timeStampInMilliseconds = timeStamp.getTime();
        this.timeStampInSeconds = Math.floor(this.timeStampInMilliseconds / 1000);
        this.milliseconds = this.timeStamp.getMilliseconds();
        this.level = level;
        this.messages = messages;
        this.exception = exception;
    };

    LoggingEvent.prototype = {
        getThrowableStrRep: function () {
            return this.exception ?
				getExceptionStringRep(this.exception) : "";
        },
        getCombinedMessages: function () {
            return (this.messages.length == 1) ? this.messages[0] :
				   this.messages.join(newLine);
        },
        toString: function () {
            return "LoggingEvent[" + this.level + "]";
        }
    };

    jsnlog.LoggingEvent = LoggingEvent;

    /* ---------------------------------------------------------------------- */
    // Layout prototype

    var Layout = function () {
    };

    Layout.prototype = {
        defaults: {
            loggerKey: "logger",
            timeStampKey: "timestamp",
            millisecondsKey: "milliseconds",
            levelKey: "level",
            messageKey: "message",
            exceptionKey: "exception",
            urlKey: "url"
        },
        loggerKey: "logger",
        timeStampKey: "timestamp",
        millisecondsKey: "milliseconds",
        levelKey: "level",
        messageKey: "message",
        exceptionKey: "exception",
        urlKey: "url",
        batchHeader: "",
        batchFooter: "",
        batchSeparator: "",
        returnsPostData: false,
        overrideTimeStampsSetting: false,
        useTimeStampsInMilliseconds: null,

        format: function () {
            handleError("Layout.format: layout supplied has no format() method");
        },

        ignoresThrowable: function () {
            handleError("Layout.ignoresThrowable: layout supplied has no ignoresThrowable() method");
        },

        getContentType: function () {
            return "text/plain";
        },

        allowBatching: function () {
            return true;
        },

        setTimeStampsInMilliseconds: function (timeStampsInMilliseconds) {
            this.overrideTimeStampsSetting = true;
            this.useTimeStampsInMilliseconds = bool(timeStampsInMilliseconds);
        },

        isTimeStampsInMilliseconds: function () {
            return this.overrideTimeStampsSetting ?
				this.useTimeStampsInMilliseconds : useTimeStampsInMilliseconds;
        },

        getTimeStampValue: function (loggingEvent) {
            return this.isTimeStampsInMilliseconds() ?
				loggingEvent.timeStampInMilliseconds : loggingEvent.timeStampInSeconds;
        },

        getDataValues: function (loggingEvent, combineMessages) {
            var dataValues = [
				[this.loggerKey, loggingEvent.logger.name],
				[this.timeStampKey, this.getTimeStampValue(loggingEvent)],
				[this.levelKey, loggingEvent.level.name],
				[this.urlKey, window.location.href],
				[this.messageKey, combineMessages ? loggingEvent.getCombinedMessages() : loggingEvent.messages]
			];
            if (!this.isTimeStampsInMilliseconds()) {
                dataValues.push([this.millisecondsKey, loggingEvent.milliseconds]);
            }
            if (loggingEvent.exception) {
                dataValues.push([this.exceptionKey, getExceptionStringRep(loggingEvent.exception)]);
            }
            if (this.hasCustomFields()) {
                for (var i = 0, len = this.customFields.length; i < len; i++) {
                    var val = this.customFields[i].value;

                    // Check if the value is a function. If so, execute it, passing it the
                    // current layout and the logging event
                    if (typeof val === "function") {
                        val = val(this, loggingEvent);
                    }
                    dataValues.push([this.customFields[i].name, val]);
                }
            }
            return dataValues;
        },

        setCustomField: function (name, value) {
            var fieldUpdated = false;
            for (var i = 0, len = this.customFields.length; i < len; i++) {
                if (this.customFields[i].name === name) {
                    this.customFields[i].value = value;
                    fieldUpdated = true;
                }
            }
            if (!fieldUpdated) {
                this.customFields.push({ "name": name, "value": value });
            }
        },

        hasCustomFields: function () {
            return (this.customFields.length > 0);
        },

        toString: function () {
            handleError("Layout.toString: all layouts must override this method");
        }
    };

    jsnlog.Layout = Layout;

    /* ---------------------------------------------------------------------- */
    // Appender prototype

    var Appender = function () { };

    Appender.prototype = new EventSupport();

    Appender.prototype.layout = new JsonLayout(false, false);
    Appender.prototype.threshold = Level.ALL;
    Appender.prototype.loggers = [];

    // Performs threshold checks before delegating actual logging to the
    // subclass's specific append method.
    Appender.prototype.doAppend = function (loggingEvent) {
        if (enabled && loggingEvent.level.level >= this.threshold.level) {
            this.append(loggingEvent);
        }
    };

    Appender.prototype.append = function (loggingEvent) { };

    Appender.prototype.setLayout = function (layout) {
        if (layout instanceof Layout) {
            this.layout = layout;
        } else {
            handleError("Appender.setLayout: layout supplied to " +
				this.toString() + " is not a subclass of Layout");
        }
    };

    Appender.prototype.getLayout = function () {
        return this.layout;
    };

    Appender.prototype.setThreshold = function (threshold) {
        if (threshold instanceof Level) {
            this.threshold = threshold;
        } else {
            handleError("Appender.setThreshold: threshold supplied to " +
				this.toString() + " is not a subclass of Level");
        }
    };

    Appender.prototype.setAddedToLogger = function (logger) {
        this.loggers.push(logger);
    };

    Appender.prototype.setRemovedFromLogger = function (logger) {
        array_remove(this.loggers, logger);
    };

    Appender.prototype.toString = function () {
        handleError("Appender.toString: all appenders must override this method");
    };

    jsnlog.Appender = Appender;

    /* ---------------------------------------------------------------------- */
    // JsonLayout related

    function escapeNewLines(str) {
        return str.replace(/\r\n|\r|\n/g, "\\r\\n");
    }

    function JsonLayout(readable, combineMessages) {
        this.readable = extractBooleanFromParam(readable, false);
        this.combineMessages = extractBooleanFromParam(combineMessages, true);
        this.batchHeader = this.readable ? "[" + newLine : "[";
        this.batchFooter = this.readable ? "]" + newLine : "]";
        this.batchSeparator = this.readable ? "," + newLine : ",";
        this.colon = this.readable ? ": " : ":";
        this.tab = this.readable ? "\t" : "";
        this.lineBreak = this.readable ? newLine : "";
        this.customFields = [];
    }

    /* ---------------------------------------------------------------------- */
    // JsonLayout

    JsonLayout.prototype = new Layout();

    JsonLayout.prototype.isReadable = function () {
        return this.readable;
    };

    JsonLayout.prototype.isCombinedMessages = function () {
        return this.combineMessages;
    };

    JsonLayout.prototype.format = function (loggingEvent) {
        var layout = this;
        var dataValues = this.getDataValues(loggingEvent, this.combineMessages);
        var str = "{" + this.lineBreak;
        var i, len;

        function formatValue(val, prefix, expand) {
            // Check the type of the data value to decide whether quotation marks
            // or expansion are required
            var formattedValue;
            var valType = typeof val;
            if (val instanceof Date) {
                formattedValue = String(val.getTime());
            } else if (expand && (val instanceof Array)) {
                formattedValue = "[" + layout.lineBreak;
                for (var i = 0, len = val.length; i < len; i++) {
                    var childPrefix = prefix + layout.tab;
                    formattedValue += childPrefix + formatValue(val[i], childPrefix, false);
                    if (i < val.length - 1) {
                        formattedValue += ",";
                    }
                    formattedValue += layout.lineBreak;
                }
                formattedValue += prefix + "]";
            } else if (valType !== "number" && valType !== "boolean") {
                formattedValue = "\"" + escapeNewLines(toStr(val).replace(/\"/g, "\\\"")) + "\"";
            } else {
                formattedValue = val;
            }
            return formattedValue;
        }

        for (i = 0, len = dataValues.length - 1; i <= len; i++) {
            str += this.tab + "\"" + dataValues[i][0] + "\"" + this.colon + formatValue(dataValues[i][1], this.tab, true);
            if (i < len) {
                str += ",";
            }
            str += this.lineBreak;
        }

        str += "}" + this.lineBreak;
        return str;
    };

    JsonLayout.prototype.ignoresThrowable = function () {
        return false;
    };

    JsonLayout.prototype.toString = function () {
        return "JsonLayout";
    };

    JsonLayout.prototype.getContentType = function () {
        return "application/json";
    };

    jsnlog.JsonLayout = JsonLayout;
    /* ---------------------------------------------------------------------- */
    // AjaxAppender related

    var xmlHttpFactories = [
		function () { return new XMLHttpRequest(); },
		function () { return new ActiveXObject("Msxml2.XMLHTTP"); },
		function () { return new ActiveXObject("Microsoft.XMLHTTP"); }
	];

    var getXmlHttp = function (errorHandler) {
        // This is only run the first time; the value of getXmlHttp gets
        // replaced with the factory that succeeds on the first run
        var xmlHttp = null, factory;
        for (var i = 0, len = xmlHttpFactories.length; i < len; i++) {
            factory = xmlHttpFactories[i];
            try {
                xmlHttp = factory();
                getXmlHttp = factory;
                return xmlHttp;
            } catch (e) {
            }
        }
        // If we're here, all factories have failed, so throw an error
        if (errorHandler) {
            errorHandler();
        } else {
            handleError("getXmlHttp: unable to obtain XMLHttpRequest object");
        }
    };

    function isHttpRequestSuccessful(xmlHttp) {
        return (isUndefined(xmlHttp.status) || xmlHttp.status === 0 ||
			(xmlHttp.status >= 200 && xmlHttp.status < 300));
    }

    /* ---------------------------------------------------------------------- */
    // AjaxAppender

    function AjaxAppender(url) {
        var appender = this;
        var isSupported = true;
        if (!url) {
            handleError("AjaxAppender: URL must be specified in constructor");
            isSupported = false;
        }

        var timed = this.defaults.timed;
        var waitForResponse = this.defaults.waitForResponse;
        var batchSize = this.defaults.batchSize;
        var timerInterval = this.defaults.timerInterval;
        var requestSuccessCallback = this.defaults.requestSuccessCallback;
        var failCallback = this.defaults.failCallback;
        var postVarName = this.defaults.postVarName;
        var sendAllOnUnload = this.defaults.sendAllOnUnload;
        var sessionId = null;

        var queuedLoggingEvents = [];
        var queuedRequests = [];
        var headers = [];
        var sending = false;
        var initialized = false;

        // Configuration methods. The function scope is used to prevent
        // direct alteration to the appender configuration properties.
        function checkCanConfigure(configOptionName) {
            if (initialized) {
                handleError("AjaxAppender: configuration option '" +
					configOptionName +
					"' may not be set after the appender has been initialized");
                return false;
            }
            return true;
        }

        this.getSessionId = function () { return sessionId; };
        this.setSessionId = function (sessionIdParam) {
            sessionId = extractStringFromParam(sessionIdParam, null);
            this.layout.setCustomField("sessionid", sessionId);
        };

        this.setLayout = function (layoutParam) {
            if (checkCanConfigure("layout")) {
                this.layout = layoutParam;
                // Set the session id as a custom field on the layout, if not already present
                if (sessionId !== null) {
                    this.setSessionId(sessionId);
                }
            }
        };

        this.isTimed = function () { return timed; };
        this.setTimed = function (timedParam) {
            if (checkCanConfigure("timed")) {
                timed = bool(timedParam);
            }
        };

        this.setTimerInterval = function (timerIntervalParam) {
            if (checkCanConfigure("timerInterval")) {
                timerInterval = extractIntFromParam(timerIntervalParam, timerInterval);
            }
        };

        this.isWaitForResponse = function () { return waitForResponse; };
        this.setWaitForResponse = function (waitForResponseParam) {
            if (checkCanConfigure("waitForResponse")) {
                waitForResponse = bool(waitForResponseParam);
            }
        };

        this.setBatchSize = function (batchSizeParam) {
            if (checkCanConfigure("batchSize")) {
                batchSize = extractIntFromParam(batchSizeParam, batchSize);
            }
        };

        this.isSendAllOnUnload = function () { return sendAllOnUnload; };
        this.setSendAllOnUnload = function (sendAllOnUnloadParam) {
            if (checkCanConfigure("sendAllOnUnload")) {
                sendAllOnUnload = extractBooleanFromParam(sendAllOnUnloadParam, sendAllOnUnload);
            }
        };

        this.setRequestSuccessCallback = function (requestSuccessCallbackParam) {
            requestSuccessCallback = extractFunctionFromParam(requestSuccessCallbackParam, requestSuccessCallback);
        };

        this.setFailCallback = function (failCallbackParam) {
            failCallback = extractFunctionFromParam(failCallbackParam, failCallback);
        };

        this.getPostVarName = function () { return postVarName; };
        this.setPostVarName = function (postVarNameParam) {
            if (checkCanConfigure("postVarName")) {
                postVarName = extractStringFromParam(postVarNameParam, postVarName);
            }
        };

        this.getHeaders = function () { return headers; };
        this.addHeader = function (name, value) {
            headers.push({ name: name, value: value });
        };

        // Internal functions
        function sendAll() {
            if (isSupported && enabled) {
                sending = true;
                var currentRequestBatch;
                if (waitForResponse) {
                    // Send the first request then use this function as the callback once
                    // the response comes back
                    if (queuedRequests.length > 0) {
                        currentRequestBatch = queuedRequests.shift();
                        sendRequest(preparePostData(currentRequestBatch), sendAll);
                    } else {
                        sending = false;
                        if (timed) {
                            scheduleSending();
                        }
                    }
                } else {
                    // Rattle off all the requests without waiting to see the response
                    while ((currentRequestBatch = queuedRequests.shift())) {
                        sendRequest(preparePostData(currentRequestBatch));
                    }
                    sending = false;
                    if (timed) {
                        scheduleSending();
                    }
                }
            }
        }

        this.sendAll = sendAll;

        // Called when the window unloads. At this point we're past caring about
        // waiting for responses or timers or incomplete batches - everything
        // must go, now
        function sendAllRemaining() {
            var sendingAnything = false;
            if (isSupported && enabled) {
                // Create requests for everything left over, batched as normal
                var actualBatchSize = appender.getLayout().allowBatching() ? batchSize : 1;
                var currentLoggingEvent;
                var batchedLoggingEvents = [];
                while ((currentLoggingEvent = queuedLoggingEvents.shift())) {
                    batchedLoggingEvents.push(currentLoggingEvent);
                    if (queuedLoggingEvents.length >= actualBatchSize) {
                        // Queue this batch of log entries
                        queuedRequests.push(batchedLoggingEvents);
                        batchedLoggingEvents = [];
                    }
                }
                // If there's a partially completed batch, add it
                if (batchedLoggingEvents.length > 0) {
                    queuedRequests.push(batchedLoggingEvents);
                }
                sendingAnything = (queuedRequests.length > 0);
                waitForResponse = false;
                timed = false;
                sendAll();
            }
            return sendingAnything;
        }

        function preparePostData(batchedLoggingEvents) {
            // Format the logging events
            var formattedMessages = [];
            var currentLoggingEvent;
            var postData = "";
            while ((currentLoggingEvent = batchedLoggingEvents.shift())) {
                var currentFormattedMessage = appender.getLayout().format(currentLoggingEvent);
                if (appender.getLayout().ignoresThrowable()) {
                    currentFormattedMessage += currentLoggingEvent.getThrowableStrRep();
                }
                formattedMessages.push(currentFormattedMessage);
            }
            // Create the post data string
            if (batchedLoggingEvents.length == 1) {
                postData = formattedMessages.join("");
            } else {
                postData = appender.getLayout().batchHeader +
					formattedMessages.join(appender.getLayout().batchSeparator) +
					appender.getLayout().batchFooter;
            }
            postData = appender.getLayout().returnsPostData ? postData :
				urlEncode(postVarName) + "=" + urlEncode(postData);
            // Add the layout name to the post data
            if (postData.length > 0) {
                postData += "&";
            }
            return postData + "layout=" + urlEncode(appender.getLayout().toString());
        }

        function scheduleSending() {
            window.setTimeout(sendAll, timerInterval);
        }

        function xmlHttpErrorHandler() {
            var msg = "AjaxAppender: could not create XMLHttpRequest object. AjaxAppender disabled";
            handleError(msg);
            isSupported = false;
            if (failCallback) {
                failCallback(msg);
            }
        }

        function sendRequest(postData, successCallback) {
            try {
                var xmlHttp = getXmlHttp(xmlHttpErrorHandler);
                if (isSupported) {
                    if (xmlHttp.overrideMimeType) {
                        xmlHttp.overrideMimeType(appender.getLayout().getContentType());
                    }
                    xmlHttp.onreadystatechange = function () {
                        if (xmlHttp.readyState == 4) {
                            if (isHttpRequestSuccessful(xmlHttp)) {
                                if (requestSuccessCallback) {
                                    requestSuccessCallback(xmlHttp);
                                }
                                if (successCallback) {
                                    successCallback(xmlHttp);
                                }
                            } else {
                                var msg = "AjaxAppender.append: XMLHttpRequest request to URL " +
									url + " returned status code " + xmlHttp.status;
                                handleError(msg);
                                if (failCallback) {
                                    failCallback(msg);
                                }
                            }
                            xmlHttp.onreadystatechange = emptyFunction;
                            xmlHttp = null;
                        }
                    };
                    xmlHttp.open("POST", url, true);
                    try {
                        xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                        for (var i = 0, header; header = headers[i++]; ) {
                            xmlHttp.setRequestHeader(header.name, header.value);
                        }
                    } catch (headerEx) {
                        var msg = "AjaxAppender.append: your browser's XMLHttpRequest implementation" +
							" does not support setRequestHeader, therefore cannot post data. AjaxAppender disabled";
                        handleError(msg);
                        isSupported = false;
                        if (failCallback) {
                            failCallback(msg);
                        }
                        return;
                    }
                    xmlHttp.send(postData);
                }
            } catch (ex) {
                var errMsg = "AjaxAppender.append: error sending log message to " + url;
                handleError(errMsg, ex);
                isSupported = false;
                if (failCallback) {
                    failCallback(errMsg + ". Details: " + getExceptionStringRep(ex));
                }
            }
        }

        this.append = function (loggingEvent) {
            if (isSupported) {
                if (!initialized) {
                    init();
                }
                queuedLoggingEvents.push(loggingEvent);
                var actualBatchSize = this.getLayout().allowBatching() ? batchSize : 1;

                if (queuedLoggingEvents.length >= actualBatchSize) {
                    var currentLoggingEvent;
                    var batchedLoggingEvents = [];
                    while ((currentLoggingEvent = queuedLoggingEvents.shift())) {
                        batchedLoggingEvents.push(currentLoggingEvent);
                    }
                    // Queue this batch of log entries
                    queuedRequests.push(batchedLoggingEvents);

                    // If using a timer, the queue of requests will be processed by the
                    // timer function, so nothing needs to be done here.
                    if (!timed && (!waitForResponse || (waitForResponse && !sending))) {
                        sendAll();
                    }
                }
            }
        };

        function init() {
            initialized = true;
            // Add unload event to send outstanding messages
            if (sendAllOnUnload) {
                var oldBeforeUnload = window.onbeforeunload;
                window.onbeforeunload = function () {
                    if (oldBeforeUnload) {
                        oldBeforeUnload();
                    }
                    if (sendAllRemaining()) {
                        return "Sending log messages";
                    }
                };
            }
            // Start timer
            if (timed) {
                scheduleSending();
            }
        }
    }

    AjaxAppender.prototype = new Appender();

    AjaxAppender.prototype.defaults = {
        waitForResponse: false,
        timed: false,
        timerInterval: 1000,
        batchSize: 1,
        sendAllOnUnload: false,
        requestSuccessCallback: null,
        failCallback: null,
        postVarName: "data"
    };

    AjaxAppender.prototype.toString = function () {
        return "AjaxAppender";
    };

    jsnlog.AjaxAppender = AjaxAppender;

    /* ---------------------------------------------------------------------- */
    // Main load

    jsnlog.setDocumentReady = function () {
        pageLoaded = true;
        jsnlog.dispatchEvent("load", {});
    };

    if (window.addEventListener) {
        window.addEventListener("load", jsnlog.setDocumentReady, false);
    } else if (window.attachEvent) {
        window.attachEvent("onload", jsnlog.setDocumentReady);
    } else {
        var oldOnload = window.onload;
        if (typeof window.onload != "function") {
            window.onload = jsnlog.setDocumentReady;
        } else {
            window.onload = function (evt) {
                if (oldOnload) {
                    oldOnload(evt);
                }
                jsnlog.setDocumentReady();
            };
        }
    }

    // Ensure that the jsnlog object is available in the window. This
    // is necessary for jsnlog to be available in IE if loaded using
    // Dojo's module system
    window.jsnlog = jsnlog;

    return jsnlog;
})();