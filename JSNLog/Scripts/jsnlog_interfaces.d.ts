/**
* Copyright 2013 Mattijs Perdeck.
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

interface JSNLogOptions {
    enabled?: bool;
    maxMessages?: number;
    clientIP?: string;
    requestId?: string;
}

interface JSNLogFilterOptions {
    level?: number;
    ipRegex?: string;
    userAgentRegex?: string;
    disallow?: string;
}

interface JSNLogLoggerOptions extends JSNLogFilterOptions {
    appenders?: JSNLogAppender[];
    onceOnly?: string[];
}

// Base for all appender options types
interface JSNLogAppenderOptions extends JSNLogFilterOptions {
    sendWithBufferLevel?: number;
    storeInBufferLevel?: number;
    bufferSize?: number;
    batchSize?: number;
}

interface JSNLogAjaxAppenderOptions extends JSNLogAppenderOptions {
    url?: string;
}

interface JSNLogLogger {
    setOptions(options: JSNLogLoggerOptions): JSNLogLogger;

    trace(logObject: any): JSNLogLogger;
    debug(logObject: any): JSNLogLogger;
    info(logObject: any): JSNLogLogger;
    warn(logObject: any): JSNLogLogger;
    error(logObject: any): JSNLogLogger;
    fatal(logObject: any): JSNLogLogger;
    log(level: number, logObject: any): JSNLogLogger;
}

interface JSNLogAppender {
    setOptions(options: JSNLogAppenderOptions): JSNLogAppender;
}

interface JSNLogAjaxAppender extends JSNLogAppender {
    setOptions(options: JSNLogAjaxAppenderOptions): JSNLogAjaxAppender;
}

interface JSNLogStatic {
    (loggerName?: string): JSNLogLogger;

    setOptions(options: JSNLogOptions): JSNLogStatic;
    createAjaxAppender(appenderName: string): JSNLogAjaxAppender;

    getTraceLevel(): number;
    getDebugLevel(): number;
    getInfoLevel(): number;
    getWarnLevel(): number;
    getErrorLevel(): number;
    getFatalLevel(): number;
}



