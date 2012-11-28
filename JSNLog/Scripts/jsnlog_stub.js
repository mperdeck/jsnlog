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
*
* This file is a derivative of the stub version of the production edition of 
* log4javascript.
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

var jsnlog_stub = (function () {
	var jsnlog;

	function ff() {
		return function() {};
	}
	function copy(obj, props) {
		for (var i in props) {
			obj[i] = props[i];
		}
	}
	var f = ff();

	// Loggers
	var Logger = ff();
	copy(Logger.prototype, {
		addChild: f,
		getEffectiveAppenders: f,
		invalidateAppenderCache: f,
		getAdditivity: f,
		setAdditivity: f,
		addAppender: f,
		removeAppender: f,
		removeAllAppenders: f,
		log: f,
		setLevel: f,
		getLevel: f,
		getEffectiveLevel: f,
		trace: f,
		debug: f,
		info: f,
		warn: f,
		error: f,
		fatal: f,
		isEnabledFor: f,
		isTraceEnabled: f,
		isDebugEnabled: f,
		isInfoEnabled: f,
		isWarnEnabled: f,
		isErrorEnabled: f,
		isFatalEnabled: f,
		callAppenders: f,
		group: f,
		groupEnd: f,
		time: f,
		timeEnd: f,
		assertEqual: f,
		parent: new Logger()
	});

	var getLogger = function() {
		return new Logger();
	};

	function EventSupport() {}

	copy(EventSupport.prototype, {
		setEventTypes: f,
		addEventListener: f,
		removeEventListener: f,
		dispatchEvent: f,
		eventTypes: [],
		eventListeners: {}
	});

	function JSNLog() {}
	JSNLog.prototype = new EventSupport();
	jsnlog = new JSNLog();

	jsnlog = {
		isStub: true,
		version: "1.0.0",
        setDocumentReady: f,
		setEventTypes: f,
		addEventListener: f,
		removeEventListener: f,
		dispatchEvent: f,
		eventTypes: [],
		eventListeners: {},
		logLog: {
			setQuietMode: f,
			setAlertAllErrors: f,
			debug: f,
			displayDebug: f,
			warn: f,
			error: f
		},
		handleError: f,
		setEnabled: f,
		isEnabled: f,
		setTimeStampsInMilliseconds: f,
		isTimeStampsInMilliseconds: f,
		evalInScope: f,
		setShowStackTraces: f,
		getLogger: getLogger,
		getDefaultLogger: getLogger,
		getNullLogger: getLogger,
		getRootLogger: getLogger,
		resetConfiguration: f,
		Level: ff(),
		LoggingEvent: ff(),
		Layout: ff(),
		Appender: ff()
	};

	// LoggingEvents
	jsnlog.LoggingEvent.prototype = {
		getThrowableStrRep: f,
		getCombinedMessages: f
	};

	// Levels
	jsnlog.Level.prototype = {
		toString: f,
		equals: f,
		isGreaterOrEqual: f
	};
	var level = new jsnlog.Level();
	copy(jsnlog.Level, {
		ALL: level,
		TRACE: level,
		DEBUG: level,
		INFO: level,
		WARN: level,
		ERROR: level,
		FATAL: level,
		OFF: level
	});

	// Layouts
	jsnlog.Layout.prototype = {
		defaults: {},
		format: f,
		ignoresThrowable: f,
		getContentType: f,
		allowBatching: f,
		getDataValues: f,
		setCustomField: f,
		hasCustomFields: f,
		setTimeStampsInMilliseconds: f,
		isTimeStampsInMilliseconds: f,
		getTimeStampValue: f,
		toString: f
	};

	// Appenders
	jsnlog.Appender = ff();
	jsnlog.Appender.prototype = new EventSupport();

	copy(jsnlog.Appender.prototype, {
	    layout: new jsnlog.JsonLayout(false, false),
		threshold: jsnlog.Level.ALL,
		loggers: [],
		doAppend: f,
		append: f,
		setLayout: f,
		getLayout: f,
		setThreshold: f,
		getThreshold: f,
		setAddedToLogger: f,
		setRemovedFromLogger: f,
		group: f,
		groupEnd: f,
		toString: f
	});
	// JsonLayout
	jsnlog.JsonLayout = ff();
	jsnlog.JsonLayout.prototype = new jsnlog.Layout();
	copy(jsnlog.JsonLayout.prototype, {
		isReadable: f,
		isCombinedMessages: f
	});
	// AjaxAppender
	jsnlog.AjaxAppender = ff();
	jsnlog.AjaxAppender.prototype = new jsnlog.Appender();
	copy(jsnlog.AjaxAppender.prototype, {
		getSessionId: f,
		setSessionId: f,
		isTimed: f,
		setTimed: f,
		getTimerInterval: f,
		setTimerInterval: f,
		isWaitForResponse: f,
		setWaitForResponse: f,
		getBatchSize: f,
		setBatchSize: f,
		isSendAllOnUnload: f,
		setSendAllOnUnload: f,
		setRequestSuccessCallback: f,
		setFailCallback: f,
		getPostVarName: f,
		setPostVarName: f,
		sendAll: f,
		defaults: {
			requestSuccessCallback: null,
			failCallback: null
		}
	});
	return jsnlog;
})();
if (typeof window.jsnlog == "undefined") {
    var jsnlog = jsnlog_stub;
}
