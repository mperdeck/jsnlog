function jsnlog_simpletest() {
}

jsnlog_simpletest.prototype.initialise = function () {

    document.getElementById("jsnlog_simpletest_btnTrace").onclick = function () {
        jsnlog.getLogger('jsnlog_simpletest.btnTrace').trace("jsnlog_simpletest_btnTrace clicked");
    };

    document.getElementById("jsnlog_simpletest_btnDebug").onclick = function () {
        jsnlog.getLogger('jsnlog_simpletest.btnDebug').debug("jsnlog_simpletest_btnDebug clicked");
    };

    document.getElementById("jsnlog_simpletest_btnInfo").onclick = function () {
        jsnlog.getLogger('jsnlog_simpletest.btnInfo').info("jsnlog_simpletest_btnTrace btnInfo");
    };

    document.getElementById("jsnlog_simpletest_btnWarn").onclick = function () {
        jsnlog.getLogger('jsnlog_simpletest.btnWarn').warn("jsnlog_simpletest_btnWarn clicked");
    };

    document.getElementById("jsnlog_simpletest_btnError").onclick = function () {
        jsnlog.getLogger('jsnlog_simpletest.btnError').error("jsnlog_simpletest_btnError clicked");
    };

    document.getElementById("jsnlog_simpletest_btnFatal").onclick = function () {
        jsnlog.getLogger('jsnlog_simpletest.btnFatal').fatal("jsnlog_simpletest_btnFatal clicked");
    };

}



