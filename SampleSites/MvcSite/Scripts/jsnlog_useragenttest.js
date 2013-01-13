function jsnlog_useragenttest() {
}

jsnlog_useragenttest.prototype.initialise = function () {

    document.getElementById("jsnlog_useragenttest_btnIEOnly").onclick = function () {
        jsnlog.getLogger('jsnlog_useragenttest.btnIEOnly').warn("jsnlog_useragenttest_btnIEOnly clicked");
    };

    document.getElementById("jsnlog_useragenttest_btnIEOnly_viaParentLogger").onclick = function () {
        jsnlog.getLogger('jsnlog_useragenttest.btnIEOnly.viaParentLogger').warn("jsnlog_useragenttest_btnIEOnly_viaParentLogger clicked");
    };

    document.getElementById("jsnlog_useragenttest_btnIEOnly_viaAppender").onclick = function () {
        jsnlog.getLogger('jsnlog_useragenttest.btnIEOnly_viaAppender').warn("jsnlog_useragenttest_btnIEOnly_viaAppender clicked");
    };
}



