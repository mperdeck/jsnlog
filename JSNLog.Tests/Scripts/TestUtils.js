/// <reference path="jquery.d.ts"/>
/// <reference path="../../JSNLog/Scripts/jsnlog.ts"/>
var TestUtils;
(function (TestUtils) {
    function Check(checkAppender, checkNbr, expected) {
        var actual = checkAppender.logItems;
        var resultDiv;

        var expectedString = JSON.stringify(expected);
        var actualString = JSON.stringify(actual);

        // class="error-occurred" is used by the integration tests.
        // If an element with that class exists on the page, the test is taken to have failed.
        var comparisonResult = LogItemArraysCompareResult(expected, actual);
        if (comparisonResult) {
            resultDiv = $('<table style="border-top: 3px red solid" class="error-occurred" />');
            resultDiv.append('<tr><td>Error at Check</td><td>' + checkNbr + '</td></tr>');
            resultDiv.append('<tr><td colspan=\'2\'>' + comparisonResult + '</td></tr>');
            resultDiv.append('<tr><td>Expected:</td><td>' + expectedString + '</td></tr>');
            resultDiv.append('<tr><td>Actual:</td><td>' + actualString + '</td></tr>');
        } else {
            resultDiv = $('<div style="border-top: 3px green solid" >Passed: ' + checkNbr + '</div>');
        }

        $('body').append(resultDiv);

        checkAppender.logItems = [];
    }
    TestUtils.Check = Check;

    function FormatResult(idx, fieldName, expected, actual) {
        return "idx: " + idx + ", field: " + fieldName + ", expected: " + expected + ", actual: " + actual;
    }

    // Returns string with comparison result.
    // Returns empty string if expected and actual are equal.
    function LogItemArraysCompareResult(expected, actual) {
        var nbrLogItems = expected.length;
        var i;

        if (nbrLogItems != actual.length) {
            return "Actual nbr log items (" + actual.length + ") not equal expected nbr log items (" + nbrLogItems + ")";
        }

        for (i = 0; i < nbrLogItems; i++) {
            if (expected[i].l != actual[i].l) {
                return FormatResult(i, "level", expected[i].l, actual[i].l);
            }

            if (expected[i].m != actual[i].m) {
                return FormatResult(i, "msg", expected[i].m, actual[i].m);
            }

            if (expected[i].n != actual[i].n) {
                return FormatResult(i, "logger name", expected[i].n, actual[i].n);
            }

            if (Math.floor(expected[i].t / 10) != Math.floor(actual[i].t / 10)) {
                return FormatResult(i, "timestamp", expected[i].t, actual[i].t);
            }
        }

        return "";
    }
})(TestUtils || (TestUtils = {}));
