function jsnlog_batchtest() {
}

jsnlog_batchtest.prototype.initialise = function () {

    document.getElementById("jsnlog_batchtest_btnTrace").onclick = function () {
        jsnlog.getLogger('jsnlog_batchtest.btnTrace').trace("jsnlog_batchtest_btnTrace clicked");
    };

    document.getElementById("jsnlog_batchtest_btnDebug").onclick = function () {
        jsnlog.getLogger('jsnlog_batchtest.btnDebug').debug("jsnlog_batchtest_btnDebug clicked");
    };

    document.getElementById("jsnlog_batchtest_btnInfo").onclick = function () {
        jsnlog.getLogger('jsnlog_batchtest.btnInfo').info("jsnlog_batchtest_btnTrace btnInfo");
    };

    document.getElementById("jsnlog_batchtest_btnWarn").onclick = function () {
        jsnlog.getLogger('jsnlog_batchtest.btnWarn').warn("jsnlog_batchtest_btnWarn clicked");
    };

    document.getElementById("jsnlog_batchtest_btnError").onclick = function () {
        jsnlog.getLogger('jsnlog_batchtest.btnError').error("jsnlog_batchtest_btnError clicked");
    };

    document.getElementById("jsnlog_batchtest_btnFatal").onclick = function () {
        jsnlog.getLogger('jsnlog_batchtest.btnFatal').fatal("jsnlog_batchtest_btnFatal clicked");
    };

}



