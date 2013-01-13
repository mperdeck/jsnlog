
(function () {
    window.onload = function () {
        var simpletest = new jsnlog_simpletest();
        simpletest.initialise();

        var batchtest = new jsnlog_batchtest();
        batchtest.initialise();

        var speciallogger = new jsnlog_speciallogger();
        speciallogger.initialise();

        var useragenttest = new jsnlog_useragenttest();
        useragenttest.initialise();

        var iptest = new jsnlog_iptest();
        iptest.initialise();
    }
} ());
