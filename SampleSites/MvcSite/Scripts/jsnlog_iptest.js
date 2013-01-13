function jsnlog_iptest() {
}

jsnlog_iptest.prototype.initialise = function () {

    document.getElementById("jsnlog_iptest_btnIpContains127").onclick = function () {
        jsnlog.getLogger('jsnlog_iptest.btnIpContains127').warn("jsnlog_iptest_btnIpContains127 clicked");
    };

    document.getElementById("jsnlog_iptest_btnIpContains127_viaParentLogger").onclick = function () {
        jsnlog.getLogger('jsnlog_iptest.btnIpContains127.viaParentLogger').warn("jsnlog_iptest_btnIpContains127_viaParentLogger clicked");
    };

    document.getElementById("jsnlog_iptest_btnIpContains127_viaAppender").onclick = function () {
        jsnlog.getLogger('jsnlog_iptest.btnIpContains127_viaAppender').warn("jsnlog_iptest_btnIpContains127_viaAppender clicked");
    };
}



