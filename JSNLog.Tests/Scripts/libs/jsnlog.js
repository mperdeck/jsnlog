/// <reference path="jsnlog_interfaces.d.ts"/>
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
function JL(loggerName) {
    // If name is empty, return the root logger
    if (!loggerName) {
        return JL.__;
    }

    var accumulatedLoggerName = '';
    var logger = ('.' + loggerName).split('.').reduce(function (prev, curr, idx, arr) {
        // if loggername is a.b.c, than currentLogger will be set to the loggers
        // root   (prev: JL, curr: '')
        // a      (prev: JL.__, curr: 'a')
        // a.b    (prev: JL.__.__a, curr: 'b')
        // a.b.c  (prev: JL.__.__a.__a.b, curr: 'c')
        // Note that when a new logger name is encountered (such as 'a.b.c'),
        // a new logger object is created and added as a property to the parent ('a.b').
        // The root logger is added as a property of the JL object itself.
        // It is essential that the name of the property containing the child logger
        // contains the full 'path' name of the child logger ('a.b.c') instead of
        // just the bit after the last period ('c').
        // This is because the parent inherits properties from its ancestors.
        // So if the root has a child logger 'c' (stored in a property 'c' of the root logger),
        // then logger 'a.b' has that same property 'c' through inheritance.
        // The names of the logger properties start with __, so the root logger
        // (which has name ''), has a nice property name '__'.
        // accumulatedLoggerName evaluates false ('' is falsy) in first iteration when prev is the root logger.
        // accumulatedLoggerName will be the logger name corresponding with the logger in currentLogger.
        // Keep in mind that the currentLogger may not be defined yet, so can't get the name from
        // the currentLogger object itself.
        if (accumulatedLoggerName) {
            accumulatedLoggerName += '.' + curr;
        } else {
            accumulatedLoggerName = curr;
        }

        var currentLogger = prev['__' + accumulatedLoggerName];

        // If the currentLogger (or the actual logger being sought) does not yet exist,
        // create it now.
        if (currentLogger === undefined) {
            // Set the prototype of the Logger constructor function to the parent of the logger
            // to be created. This way, __proto of the new logger object will point at the parent.
            // When logger.level is evaluated and is not present, the JavaScript runtime will
            // walk down the prototype chain to find the first ancestor with a level property.
            //
            // Note that prev at this point refers to the parent logger.
            JL.Logger.prototype = prev;

            currentLogger = new JL.Logger(accumulatedLoggerName);
            prev['__' + accumulatedLoggerName] = currentLogger;
        }

        return currentLogger;
    }, JL.__);

    return logger;
}

var JL;
(function (JL) {
    JL.enabled;
    JL.maxMessages;
    JL.defaultAjaxUrl;
    JL.clientIP;

    // Initialise requestId to empty string. If you don't do this and the user
    // does not set it via setOptions, then the JSNLog-RequestId header will
    // have value "undefined", which doesn't look good in a log.
    //
    // Note that you always want to send a requestId as part of log requests,
    // otherwise the server side component doesn't know this is a log request
    // and may create a new request id for the log request, causing confusion
    // in the log.
    JL.requestId = '';

    /**
    Copies the value of a property from one object to the other.
    This is used to copy property values as part of setOption for loggers and appenders.
    
    Because loggers inherit property values from their parents, it is important never to
    create a property on a logger if the intent is to inherit from the parent.
    
    Copying rules:
    1) if the from property is undefined (for example, not mentioned in a JSON object), the
    to property is not affected at all.
    2) if the from property is null, the to property is deleted (so the logger will inherit from
    its parent).
    3) Otherwise, the from property is copied to the to property.
    */
    function copyProperty(propertyName, from, to) {
        if (from[propertyName] === undefined) {
            return;
        }
        if (from[propertyName] === null) {
            delete to[propertyName];
            return;
        }
        to[propertyName] = from[propertyName];
    }

    /**
    Returns true if a log should go ahead.
    Does not check level.
    
    @param filters
    Filters that determine whether a log can go ahead.
    */
    function allow(filters) {
        // If enabled is not null or undefined, then if it is false, then return false
        // Note that undefined==null (!)
        if (!(JL.enabled == null)) {
            if (!JL.enabled) {
                return false;
            }
        }

        // If maxMessages is not null or undefined, then if it is 0, then return false.
        // Note that maxMessages contains number of messages that are still allowed to send.
        // It is decremented each time messages are sent. It can be negative when batch size > 1.
        // Note that undefined==null (!)
        if (!(JL.maxMessages == null)) {
            if (JL.maxMessages < 1) {
                return false;
            }
        }

        try  {
            if (filters.userAgentRegex) {
                if (!new RegExp(filters.userAgentRegex).test(navigator.userAgent)) {
                    return false;
                }
            }
        } catch (e) {
        }

        try  {
            if (filters.ipRegex && JL.clientIP) {
                if (!new RegExp(filters.ipRegex).test(JL.clientIP)) {
                    return false;
                }
            }
        } catch (e) {
        }

        return true;
    }

    /**
    Returns true if a log should go ahead, based on the message.
    
    @param filters
    Filters that determine whether a log can go ahead.
    
    @param message
    Message to be logged.
    */
    function allowMessage(filters, message) {
        try  {
            if (filters.disallow) {
                if (new RegExp(filters.disallow).test(message)) {
                    return false;
                }
            }
        } catch (e) {
        }

        return true;
    }

    function stringifyLogObject(logObject) {
        switch (typeof logObject) {
            case "string":
                return logObject;
            case "number":
                return logObject.toString();
            case "boolean":
                return logObject.toString();
            case "undefined":
                return "undefined";
            case "function":
                if (logObject instanceof RegExp) {
                    return logObject.toString();
                } else {
                    return stringifyLogObject(logObject());
                }
            case "object":
                if ((logObject instanceof RegExp) || (logObject instanceof String) || (logObject instanceof Number) || (logObject instanceof Boolean)) {
                    return logObject.toString();
                } else {
                    return JSON.stringify(logObject);
                }
            default:
                return "unknown";
        }
    }

    function setOptions(options) {
        copyProperty("enabled", options, this);
        copyProperty("maxMessages", options, this);
        copyProperty("defaultAjaxUrl", options, this);
        copyProperty("clientIP", options, this);
        copyProperty("requestId", options, this);
        return this;
    }
    JL.setOptions = setOptions;

    function getAllLevel() {
        return -2147483648;
    }
    JL.getAllLevel = getAllLevel;
    function getTraceLevel() {
        return 1000;
    }
    JL.getTraceLevel = getTraceLevel;
    function getDebugLevel() {
        return 2000;
    }
    JL.getDebugLevel = getDebugLevel;
    function getInfoLevel() {
        return 3000;
    }
    JL.getInfoLevel = getInfoLevel;
    function getWarnLevel() {
        return 4000;
    }
    JL.getWarnLevel = getWarnLevel;
    function getErrorLevel() {
        return 5000;
    }
    JL.getErrorLevel = getErrorLevel;
    function getFatalLevel() {
        return 6000;
    }
    JL.getFatalLevel = getFatalLevel;
    function getOffLevel() {
        return 2147483647;
    }
    JL.getOffLevel = getOffLevel;

    // ---------------------
    var Exception = (function () {
        // data replaces message. It takes not just strings, but also objects and functions, just like the log function.
        // internally, the string representation is stored in the message property (inherited from Error)
        //
        // inner: inner exception. Can be null or undefined.
        function Exception(data, inner) {
            this.inner = inner;
            this.name = "JL.Exception";
            this.message = stringifyLogObject(data);
        }
        return Exception;
    })();
    JL.Exception = Exception;

    // Derive Exception from Error (a Host object), so browsers
    // are more likely to produce a stack trace for it in their console.
    //
    // Note that instanceof against an object created with this constructor
    // will return true in these cases:
    // <object> instanceof JL.Exception);
    // <object> instanceof Error);
    Exception.prototype = new Error();

    // ---------------------
    var LogItem = (function () {
        // l: level
        // m: message
        // n: logger name
        // t (timeStamp) is number of milliseconds since 1 January 1970 00:00:00 UTC
        //
        // Keeping the property names really short, because they will be sent in the
        // JSON payload to the server.
        function LogItem(l, m, n, t) {
            this.l = l;
            this.m = m;
            this.n = n;
            this.t = t;
        }
        return LogItem;
    })();
    JL.LogItem = LogItem;

    // ---------------------
    var Appender = (function () {
        // sendLogItems takes an array of log items. It will be called when
        // the appender has items to process (such as, send to the server).
        // Note that after sendLogItems returns, the appender may truncate
        // the LogItem array, so the function has to copy the content of the array
        // in some fashion (eg. serialize) before returning.
        function Appender(appenderName, sendLogItems) {
            this.appenderName = appenderName;
            this.sendLogItems = sendLogItems;
            this.level = JL.getTraceLevel();
            // set to super high level, so if user increases level, level is unlikely to get
            // above sendWithBufferLevel
            this.sendWithBufferLevel = 2147483647;
            this.storeInBufferLevel = -2147483648;
            this.bufferSize = 0;
            this.batchSize = 1;
            // Holds all log items with levels higher than storeInBufferLevel
            // but lower than level. These items may never be sent.
            this.buffer = [];
            // Holds all items that we do want to send, until we have a full
            // batch (as determined by batchSize).
            this.batchBuffer = [];
        }
        Appender.prototype.setOptions = function (options) {
            copyProperty("level", options, this);
            copyProperty("ipRegex", options, this);
            copyProperty("userAgentRegex", options, this);
            copyProperty("disallow", options, this);
            copyProperty("sendWithBufferLevel", options, this);
            copyProperty("storeInBufferLevel", options, this);
            copyProperty("bufferSize", options, this);
            copyProperty("batchSize", options, this);

            if (this.bufferSize < this.buffer.length) {
                this.buffer.length = this.bufferSize;
            }

            return this;
        };

        /**
        Called by a logger to log a log item.
        If in response to this call one or more log items need to be processed
        (eg., sent to the server), this method calls this.sendLogItems
        with an array with all items to be processed.
        */
        Appender.prototype.log = function (level, message, loggerName) {
            var logItem;

            if (!allow(this)) {
                return;
            }
            if (!allowMessage(this, message)) {
                return;
            }

            if (level < this.storeInBufferLevel) {
                // Ignore the log item completely
                return;
            }

            logItem = new LogItem(level, message, loggerName, (new Date).getTime());

            if (level < this.level) {
                // Store in the hold buffer. Do not send.
                if (this.bufferSize > 0) {
                    this.buffer.push(logItem);

                    // If we exceeded max buffer size, remove oldest item
                    if (this.buffer.length > this.bufferSize) {
                        this.buffer.shift();
                    }
                }

                return;
            }

            if (level < this.sendWithBufferLevel) {
                // Want to send the item, but not the contents of the buffer
                this.batchBuffer.push(logItem);
            } else {
                // Want to send both the item and the contents of the buffer.
                // Send contents of buffer first, because logically they happened first.
                if (this.buffer.length) {
                    this.batchBuffer = this.batchBuffer.concat(this.buffer);
                    this.buffer.length = 0;
                }
                this.batchBuffer.push(logItem);
            }

            if (this.batchBuffer.length >= this.batchSize) {
                this.sendBatch();
                return;
            }
        };

        // Processes the batch buffer
        Appender.prototype.sendBatch = function () {
            if (this.batchBuffer.length == 0) {
                return;
            }

            if (!(JL.maxMessages == null)) {
                if (JL.maxMessages < 1) {
                    return;
                }
            }

            // If maxMessages is not null or undefined, then decrease it by the batch size.
            // This can result in a negative maxMessages.
            // Note that undefined==null (!)
            if (!(JL.maxMessages == null)) {
                JL.maxMessages -= this.batchBuffer.length;
            }

            this.sendLogItems(this.batchBuffer);
            this.batchBuffer.length = 0;
        };
        return Appender;
    })();
    JL.Appender = Appender;

    // ---------------------
    var AjaxAppender = (function (_super) {
        __extends(AjaxAppender, _super);
        function AjaxAppender(appenderName) {
            _super.call(this, appenderName, AjaxAppender.prototype.sendLogItemsAjax);
        }
        AjaxAppender.prototype.setOptions = function (options) {
            copyProperty("url", options, this);
            _super.prototype.setOptions.call(this, options);
            return this;
        };

        AjaxAppender.prototype.sendLogItemsAjax = function (logItems) {
            try  {
                // Only determine the url right before you send a log request.
                // Do not set the url when constructing the appender.
                //
                // This is because the server side component sets defaultAjaxUrl
                // in a call to setOptions, AFTER the JL object and the default appender
                // have been created.
                var ajaxUrl = "/jsnlog.logger";

                // This evaluates to true if defaultAjaxUrl is null or undefined
                if (!(JL.defaultAjaxUrl == null)) {
                    ajaxUrl = JL.defaultAjaxUrl;
                }

                if (this.url) {
                    ajaxUrl = this.url;
                }

                var json = JSON.stringify({
                    r: JL.requestId,
                    lg: logItems
                });

                // Send the json to the server.
                // Note that there is no event handling here. If the send is not
                // successful, nothing can be done about it.
                var xhr = new XMLHttpRequest();
                xhr.open('POST', ajaxUrl);

                xhr.setRequestHeader('Content-Type', 'application/json');
                xhr.setRequestHeader('JSNLog-RequestId', JL.requestId);
                xhr.send(json);
            } catch (e) {
            }
        };
        return AjaxAppender;
    })(Appender);
    JL.AjaxAppender = AjaxAppender;

    // ---------------------
    var ConsoleAppender = (function (_super) {
        __extends(ConsoleAppender, _super);
        function ConsoleAppender(appenderName) {
            _super.call(this, appenderName, ConsoleAppender.prototype.sendLogItemsConsole);
        }
        ConsoleAppender.prototype.clog = function (logEntry) {
            console.log(logEntry);
        };

        ConsoleAppender.prototype.cerror = function (logEntry) {
            if (console.error) {
                console.error(logEntry);
            } else {
                this.clog(logEntry);
            }
        };

        ConsoleAppender.prototype.cwarn = function (logEntry) {
            if (console.warn) {
                console.warn(logEntry);
            } else {
                this.clog(logEntry);
            }
        };

        ConsoleAppender.prototype.cinfo = function (logEntry) {
            if (console.info) {
                console.info(logEntry);
            } else {
                this.clog(logEntry);
            }
        };

        // IE11 has a console.debug function. But its console doesn't have
        // the option to show/hide debug messages (the same way Chrome and FF do),
        // even though it does have such buttons for Error, Warn, Info.
        //
        // For now, this means that debug messages can not be hidden on IE.
        // Live with this, seeing that it works fine on FF and Chrome, which
        // will be much more popular with developers.
        ConsoleAppender.prototype.cdebug = function (logEntry) {
            if (console.debug) {
                console.debug(logEntry);
            } else {
                this.cinfo(logEntry);
            }
        };

        ConsoleAppender.prototype.sendLogItemsConsole = function (logItems) {
            try  {
                if (!console) {
                    return;
                }

                var i;
                for (i = 0; i < logItems.length; ++i) {
                    var li = logItems[i];
                    var msg = li.n + ": " + li.m;

                    if (li.l <= JL.getDebugLevel()) {
                        this.cdebug(msg);
                    } else if (li.l <= JL.getInfoLevel()) {
                        this.cinfo(msg);
                    } else if (li.l <= JL.getWarnLevel()) {
                        this.cwarn(msg);
                    } else {
                        this.cerror(msg);
                    }
                }
            } catch (e) {
            }
        };
        return ConsoleAppender;
    })(Appender);
    JL.ConsoleAppender = ConsoleAppender;

    // --------------------
    var Logger = (function () {
        function Logger(loggerName) {
            this.loggerName = loggerName;
            // Create seenRexes, otherwise this logger will use the seenRexes
            // of its parent via the prototype chain.
            this.seenRegexes = [];
        }
        Logger.prototype.setOptions = function (options) {
            copyProperty("level", options, this);
            copyProperty("userAgentRegex", options, this);
            copyProperty("disallow", options, this);
            copyProperty("ipRegex", options, this);
            copyProperty("appenders", options, this);
            copyProperty("onceOnly", options, this);

            // Reset seenRegexes, in case onceOnly has been changed.
            this.seenRegexes = [];

            return this;
        };

        // Turns an exception into an object that can be sent to the server.
        Logger.prototype.buildExceptionObject = function (e) {
            var excObject = {};

            if (e.stack) {
                excObject.stack = e.stack;
            } else {
                excObject.e = e;
            }
            if (e.message) {
                excObject.message = e.message;
            }
            if (e.name) {
                excObject.name = e.name;
            }
            if (e.data) {
                excObject.data = e.data;
            }
            if (e.inner) {
                excObject.inner = this.buildExceptionObject(e.inner);
            }

            return excObject;
        };

        // Logs a log item.
        // Parameter e contains an exception (or null or undefined).
        //
        // Reason that processing exceptions is done at this low level is that
        // 1) no need to spend the cpu cycles if the logger is switched off
        // 2) fatalException takes both a logObject and an exception, and the logObject
        //    may be a function that should only be executed if the logger is switched on.
        //
        // If an exception is passed in, the contents of logObject is attached to the exception
        // object in a new property logData.
        // The resulting exception object is than worked into a message to the server.
        //
        // If there is no exception, logObject itself is worked into the message to the server.
        Logger.prototype.log = function (level, logObject, e) {
            var i = 0;
            var message;
            var excObject;

            // If we can't find any appenders, do nothing
            if (!this.appenders) {
                return this;
            }

            if (((level >= this.level)) && allow(this)) {
                // logObject could be a function, so process independently from the exception.
                message = stringifyLogObject(logObject);

                if (e) {
                    excObject = this.buildExceptionObject(e);
                    excObject.logData = message;

                    message = JSON.stringify(excObject);
                }

                if (allowMessage(this, message)) {
                    // See whether message is a duplicate
                    if (this.onceOnly) {
                        i = this.onceOnly.length - 1;
                        while (i >= 0) {
                            if (new RegExp(this.onceOnly[i]).test(message)) {
                                if (this.seenRegexes[i]) {
                                    return this;
                                }

                                this.seenRegexes[i] = true;
                            }

                            i--;
                        }
                    }

                    // Pass message to all appenders
                    i = this.appenders.length - 1;
                    while (i >= 0) {
                        this.appenders[i].log(level, message, this.loggerName);
                        i--;
                    }
                }
            }

            return this;
        };

        Logger.prototype.trace = function (logObject) {
            return this.log(getTraceLevel(), logObject);
        };
        Logger.prototype.debug = function (logObject) {
            return this.log(getDebugLevel(), logObject);
        };
        Logger.prototype.info = function (logObject) {
            return this.log(getInfoLevel(), logObject);
        };
        Logger.prototype.warn = function (logObject) {
            return this.log(getWarnLevel(), logObject);
        };
        Logger.prototype.error = function (logObject) {
            return this.log(getErrorLevel(), logObject);
        };
        Logger.prototype.fatal = function (logObject) {
            return this.log(getFatalLevel(), logObject);
        };
        Logger.prototype.fatalException = function (logObject, e) {
            return this.log(getFatalLevel(), logObject, e);
        };
        return Logger;
    })();
    JL.Logger = Logger;

    function createAjaxAppender(appenderName) {
        return new AjaxAppender(appenderName);
    }
    JL.createAjaxAppender = createAjaxAppender;

    function createConsoleAppender(appenderName) {
        return new ConsoleAppender(appenderName);
    }
    JL.createConsoleAppender = createConsoleAppender;

    // -----------------------
    var defaultAppender = new AjaxAppender("");

    // Create root logger
    //
    // Note that this is the parent of all other loggers.
    // Logger "x" will be stored at
    // JL.__.x
    // Logger "x.y" at
    // JL.__.x.y
    JL.__ = new JL.Logger("");
    JL.__.setOptions({
        level: JL.getDebugLevel(),
        appenders: [defaultAppender]
    });
})(JL || (JL = {}));

// Support CommonJS module format
var exports;
if (typeof exports !== 'undefined') {
    exports.JL = JL;
}

// Support AMD module format
var define;
if (typeof define == 'function' && define.amd) {
    define('jsnlog', [], function () {
        return JL;
    });
}

// If the __jsnlog_configure global function has been
// created, call it now. This allows you to create a global function
// setting logger options etc. inline in the page before jsnlog.js
// has been loaded.
if (typeof __jsnlog_configure == 'function') {
    __jsnlog_configure();
}
//# sourceMappingURL=jsnlog.js.map
