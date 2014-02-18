/// <reference path="jsnlog_interfaces.d.ts"/>

function JL(loggerName?: string): JSNLogLogger {
    
    // If name is empty, return the root logger
    if (!loggerName) {
        return JL.__;
    }

    var accumulatedLoggerName='';
    var logger: JL.Logger = ('.' + loggerName).split('.').reduce(
        function (prev: JL.Logger, curr: string, idx: number, arr: string[]) { 
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

                <any>JL.Logger.prototype = prev;

                currentLogger = new JL.Logger(accumulatedLoggerName);
                prev['__' + accumulatedLoggerName] = currentLogger;  
            }
            
            return currentLogger;
        }, JL.__);

    return logger;
}

module JL {

    export var enabled: boolean;
    export var maxMessages: number;
    export var clientIP: string;
    export var requestId: string;

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
    function copyProperty(propertyName: string, from: any, to: any): void {
        if (from[propertyName] === undefined) { return; }
        if (from[propertyName] === null) { delete to[propertyName]; return; }
        to[propertyName] = from[propertyName];
    }

    /**
    Returns true if a log should go ahead.
    Does not check level.

    @param filters
        Filters that determine whether a log can go ahead.
    */
    function allow(filters: JSNLogFilterOptions): boolean {
        // If enabled is not null or undefined, then if it is false, then return false
        // Note that undefined==null (!)
        if (!(JL.enabled == null)) {
            if (!JL.enabled) { return false; }
        }

        // If maxMessages is not null or undefined, then if it is 0, then return false.
        // Note that maxMessages contains number of messages that are still allowed to send.
        // It is decremented each time messages are sent. It can be negative when batch size > 1.
        // Note that undefined==null (!)
        if (!(JL.maxMessages == null)) {
            if (JL.maxMessages < 1) { return false; }
        }

        // If the regex contains a bug, that will throw an exception.
        // Ignore this, and pass the log item (better too much than too little).

        try {
            if (filters.userAgentRegex) {
                if (!new RegExp(filters.userAgentRegex).test(navigator.userAgent)) { return false; }
            }
        } catch (e) { }

        try {
            if (filters.ipRegex && JL.clientIP) {
                if (!new RegExp(filters.ipRegex).test(JL.clientIP)) { return false; }
            }
        } catch (e) { }

        return true;
    }

    /**
    Returns true if a log should go ahead, based on the message.

    @param filters
        Filters that determine whether a log can go ahead.

    @param message
        Message to be logged.
    */
    function allowMessage(filters: JSNLogFilterOptions, message: string): boolean {
        // If the regex contains a bug, that will throw an exception.
        // Ignore this, and pass the log item (better too much than too little).

        try {
            if (filters.disallow) {
                if (new RegExp(filters.disallow).test(message)) { return false; }
            }
        } catch (e) { }

        return true;
    }

    export function setOptions(options: JSNLogOptions): JSNLogStatic {
        copyProperty("enabled", options, this);
        copyProperty("maxMessages", options, this);
        copyProperty("clientIP", options, this);
        copyProperty("requestId", options, this);
        return this;
    }

    export function getAllLevel(): number { return -2147483648; }
    export function getTraceLevel(): number { return 1000; }
    export function getDebugLevel(): number { return 2000; }
    export function getInfoLevel(): number { return 3000; }
    export function getWarnLevel(): number { return 4000; }
    export function getErrorLevel(): number { return 5000; }
    export function getFatalLevel(): number { return 6000; }
    export function getOffLevel(): number { return 2147483647; }

    // ---------------------

    export class LogItem {
        // l: level
        // m: message
        // n: logger name
        // t (timeStamp) is number of milliseconds since 1 January 1970 00:00:00 UTC
        //
        // Keeping the property names really short, because they will be sent in the
        // JSON payload to the server.
        constructor(public l: number, public m: string,
            public n: string, public t: number) {}
    }

    // ---------------------

    export class Appender implements JSNLogAppender, JSNLogFilterOptions {
        public level: number = JL.getTraceLevel();
        public ipRegex: string;
        public userAgentRegex: string;
        public disallow: string;

        // set to super high level, so if user increases level, level is unlikely to get 
        // above sendWithBufferLevel
        private sendWithBufferLevel: number = 2147483647; 

        private storeInBufferLevel: number = -2147483648;
        private bufferSize: number = 0; // buffering switch off by default
        private batchSize: number = 1;

        // Holds all log items with levels higher than storeInBufferLevel 
        // but lower than level. These items may never be sent.
        private buffer: LogItem[] = [];

        // Holds all items that we do want to send, until we have a full
        // batch (as determined by batchSize).
        private batchBuffer: LogItem[] = [];

        // sendLogItems takes an array of log items. It will be called when
        // the appender has items to process (such as, send to the server).
        // Note that after sendLogItems returns, the appender may truncate
        // the LogItem array, so the function has to copy the content of the array
        // in some fashion (eg. serialize) before returning.

        constructor(
            public appenderName: string, 
            public sendLogItems: (logItems: LogItem[]) => void) { 
        }

        public setOptions(options: JSNLogAppenderOptions): JSNLogAppender {
            copyProperty("level", options, this);
            copyProperty("ipRegex", options, this);
            copyProperty("userAgentRegex", options, this);
            copyProperty("disallow", options, this);
            copyProperty("sendWithBufferLevel", options, this);
            copyProperty("storeInBufferLevel", options, this);
            copyProperty("bufferSize", options, this);
            copyProperty("batchSize", options, this);

            if (this.bufferSize < this.buffer.length) { this.buffer.length = this.bufferSize; }

            return this;
        }

        /**
        Called by a logger to log a log item.
        If in response to this call one or more log items need to be processed
        (eg., sent to the server), this method calls this.sendLogItems
        with an array with all items to be processed.
        */
        public log(level: number, message: string, loggerName: string): void {
            var logItem: LogItem;

            if (!allow(this)) { return; }
            if (!allowMessage(this, message)) { return; }

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
        }

        // Processes the batch buffer
        private sendBatch(): void {
            if (this.batchBuffer.length == 0) {
                return;
            }

            if (!(JL.maxMessages == null)) {
                if (JL.maxMessages < 1) { return; }
            }



            // If maxMessages is not null or undefined, then decrease it by the batch size.
            // This can result in a negative maxMessages.
            // Note that undefined==null (!)
            if (!(JL.maxMessages == null)) {
                JL.maxMessages -= this.batchBuffer.length;
            }

            this.sendLogItems(this.batchBuffer);
            this.batchBuffer.length = 0;
        }
    }

    // ---------------------

    export class AjaxAppender extends Appender implements JSNLogAjaxAppender {
        private url: string = "jsnlog.logger";

        public setOptions(options: JSNLogAjaxAppenderOptions): JSNLogAjaxAppender {
            copyProperty("url", options, this);
            super.setOptions(options);
            return this;
        }

        public sendLogItemsAjax(logItems: LogItem[]): void {
            // JSON.stringify is only supported on IE8+
            // Use try-catch in case we get an exception here.
            try {
                var json: string = JSON.stringify({
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
            } catch (e) { }
        }

        constructor(appenderName: string) {
            super(appenderName, AjaxAppender.prototype.sendLogItemsAjax);
        }
    }

    // --------------------

    export class Logger implements JSNLogLogger, JSNLogFilterOptions {
        public appenders: Appender[];

        // Array of strings with regular expressions. Used to stop duplicate messages.
        // If a message matches a regex
        // that has been matched before, that message will not be sent.
        public onceOnly: string[];

        public level: number;
        public userAgentRegex: string;
        public ipRegex: string;
        public disallow: string;

        // Used to remember which regexes in onceOnly have been successfully 
        // matched against a message. Index into this array is same as index
        // in onceOnly of the corresponding regex.
        // When a regex has never been matched, the corresponding entry in this
        // array is undefined, which is falsey.
        private seenRegexes: boolean[];

        constructor(public loggerName: string) {
            // Create seenRexes, otherwise this logger will use the seenRexes
            // of its parent via the prototype chain.
            this.seenRegexes = [];
        }

        private stringifyLogObject(logObject: any): string
        {
            switch (typeof logObject)
            {
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
                  if ((logObject instanceof RegExp) ||
                      (logObject instanceof String) ||
                      (logObject instanceof Number) ||
                      (logObject instanceof Boolean)) {
                    return logObject.toString();
                  } else {
                    return JSON.stringify(logObject);
                  }
                default:
                  return "unknown";
            }
        }

        public setOptions(options: JSNLogLoggerOptions): JSNLogLogger {
            copyProperty("level", options, this);
            copyProperty("userAgentRegex", options, this);
            copyProperty("disallow", options, this);
            copyProperty("ipRegex", options, this);
            copyProperty("appenders", options, this);
            copyProperty("onceOnly", options, this);

            // Reset seenRegexes, in case onceOnly has been changed.
            this.seenRegexes = [];

            return this;
        }

        public log(level: number, logObject: any): JSNLogLogger {
            var i: number = 0;
            var message: string;

            // If we can't find any appenders, do nothing
            if (!this.appenders) { return this; }

            if (((level >= this.level)) && allow(this)) {
                message = this.stringifyLogObject(logObject);

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
        }

        public trace(logObject: any): JSNLogLogger { return this.log(getTraceLevel(), logObject); }
        public debug(logObject: any): JSNLogLogger { return this.log(getDebugLevel(), logObject); }
        public info(logObject: any): JSNLogLogger { return this.log(getInfoLevel(), logObject); }
        public warn(logObject: any): JSNLogLogger { return this.log(getWarnLevel(), logObject); }
        public error(logObject: any): JSNLogLogger { return this.log(getErrorLevel(), logObject); }
        public fatal(logObject: any): JSNLogLogger { return this.log(getFatalLevel(), logObject); }
    }

    // -----------------------

    var defaultAppender = new AjaxAppender("");

    // Create root logger
    //
    // Note that this is the parent of all other loggers.
    // Logger "x" will be stored at
    // JL.__.x
    // Logger "x.y" at
    // JL.__.x.y
    export var __ = new JL.Logger("");
    JL.__.setOptions(
        {
            level: JL.getDebugLevel(),
            appenders: [defaultAppender]
        });

    export function createAjaxAppender(appenderName: string): JSNLogAjaxAppender {
        return new AjaxAppender(appenderName);
    }
}



