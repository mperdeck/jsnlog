/// <reference path="jsnlog.js"/>

// DummyAppender implements a send function sendLogItemsDummy that simply appends logItems
// to a public property, so it can be inspected by tests.

(function () {
    var constructor = function (JL) {
        function DummyAppender(appenderName) {
            JL.Appender.call(this, appenderName, DummyAppender.prototype.sendLogItemsDummy);
            this.logItems = [];
        }

        DummyAppender.prototype = new JL.Appender();
        DummyAppender.prototype.sendLogItemsDummy = function (logItems) {
            this.logItems = this.logItems.concat(logItems);
        }

        JL.DummyAppender = DummyAppender;

        // -------------

        function createDummyAppender(appenderName) {
            return new DummyAppender(appenderName);
        }
        JL.createDummyAppender = createDummyAppender;
    }

    if (typeof define == 'function' && define.amd) {
        define(["jsnlog"], function (JL) {
            constructor(JL);
            return JL;
        });
    } else {
        constructor(JL);
    }
})();


