var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
/// <reference path="jsnlog_interfaces.d.ts"/>
function JL(loggerName) {
    if (!loggerName) {
        return JL.__;
    }

    var accumulatedLoggerName = '';
    var logger = ('.' + loggerName).split('.').reduce(function (prev, curr, idx, arr) {
        if (accumulatedLoggerName) {
            accumulatedLoggerName += '.' + curr;
        } else {
            accumulatedLoggerName = curr;
        }

        var currentLogger = prev['__' + accumulatedLoggerName];

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
    JL.clientIP;
    JL.requestId;

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
        if (!(JL.enabled == null)) {
            if (!JL.enabled) {
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

    function setOptions(options) {
        copyProperty("enabled", options, this);
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

            logItem = new LogItem(level, message, loggerName, (new Date()).getTime());

            if (level < this.level) {
                if (this.bufferSize > 0) {
                    this.buffer.push(logItem);

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
            this.url = "jsnlog.logger";
        }
        AjaxAppender.prototype.setOptions = function (options) {
            copyProperty("url", options, this);
            _super.prototype.setOptions.call(this, options);
            return this;
        };

        AjaxAppender.prototype.sendLogItemsAjax = function (logItems) {
            try  {
                var json = JSON.stringify({
                    r: JL.requestId,
                    lg: logItems
                });

                // Send the json to the server.
                // Note that there is no event handling here. If the send is not
                // successful, nothing can be done about it.
                var xhr = new XMLHttpRequest();
                xhr.open('POST', this.url);

                xhr.setRequestHeader('Content-Type', 'application/json');
                xhr.send(json);
            } catch (e) {
            }
        };
        return AjaxAppender;
    })(Appender);
    JL.AjaxAppender = AjaxAppender;

    // --------------------
    var Logger = (function () {
        function Logger(loggerName) {
            this.loggerName = loggerName;
            // Create seenRexes, otherwise this logger will use the seenRexes
            // of its parent via the prototype chain.
            this.seenRegexes = [];
        }
        Logger.prototype.stringifyLogObject = function (logObject) {
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
                        return this.stringifyLogObject(logObject());
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
        };

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

        Logger.prototype.log = function (level, logObject) {
            var i = 0;
            var message;

            if (!this.appenders) {
                return this;
            }

            if (((level >= this.level)) && allow(this)) {
                message = this.stringifyLogObject(logObject);

                if (allowMessage(this, message)) {
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
        return Logger;
    })();
    JL.Logger = Logger;

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

    function createAjaxAppender(appenderName) {
        return new AjaxAppender(appenderName);
    }
    JL.createAjaxAppender = createAjaxAppender;
})(JL || (JL = {}));
