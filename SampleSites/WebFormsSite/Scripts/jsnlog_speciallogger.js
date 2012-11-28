function jsnlog_speciallogger() {
}

jsnlog_speciallogger.prototype.initialise = function () {

    document.getElementById("jsnlog_speciallogger_btnStartTimer").onclick = function () {
        jsnlog.getLogger('jsnlog_speciallogger.timer').time('timer1', jsnlog.Level.INFO);
    };

    document.getElementById("jsnlog_speciallogger_btnEndTimer").onclick = function () {
        jsnlog.getLogger('jsnlog_speciallogger.timer').timeEnd('timer1');
    };

    document.getElementById("jsnlog_speciallogger_btnAssertFalse").onclick = function () {
        jsnlog.getLogger('jsnlog_speciallogger.assertFalse').assertEqual(0, 1);
    };

    document.getElementById("jsnlog_speciallogger_btnAssertTrue").onclick = function () {
        jsnlog.getLogger('jsnlog_speciallogger.assertTrue').assertEqual(1, 1);
    };
}



