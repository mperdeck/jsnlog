/// <reference path="jquery.d.ts"/>
/// <reference path="../../../../jsnlog.js/jsnlog.ts"/>

module TestUtils {
    export function Check(checkAppenderUrlPath: string, checkNbr: number, expected: JL.LogItem[]) {

        var checkAppenderUrl = 'http://dummy.com/' + checkAppenderUrlPath;

		// An appender only calls beforeSend when it tries to send a log request.
		// So if the appender never tries to send anything, than actual will be undefined.
        var actual: JL.LogItem[] = (<any>window)[checkAppenderUrl] || [];

        var resultDiv: JQuery;

        var expectedString = JSON.stringify(expected);
        var actualString = JSON.stringify(actual);

        // class="error-occurred" is used by the integration tests.
        // If an element with that class exists on the page, the test is taken to have failed.

        var comparisonResult: string = LogItemArraysCompareResult(expected, actual);
        if (comparisonResult) {
            resultDiv = $('<table style="border-top: 3px red solid" class="error-occurred" />');
            resultDiv.append('<tr><td>Error at Check</td><td>' + checkNbr + '</td></tr>');
            resultDiv.append('<tr><td valign="top" colspan=\'2\'>' + comparisonResult + '</td></tr>');
            resultDiv.append('<tr><td valign="top">Expected:</td><td>' + expectedString + '</td></tr>');
            resultDiv.append('<tr><td valign="top">Actual:</td><td>' + actualString + '</td></tr></table>');
        } else {
            resultDiv = $('<div style="border-top: 3px green solid" >Passed: ' + checkNbr + '</div>');
        }

        $('body').append(resultDiv);

        (<any>window)[checkAppenderUrl] = [];
    }

	export function beforeSend(xhr: any) {
		var appenderThis = this;
        xhr.send = function (json) {

            if (!(<any>window)[appenderThis.url]) { (<any>window)[appenderThis.url] = []; }

            var item = JSON.parse(json);
            (<any>window)[appenderThis.url] = (<any>window)[appenderThis.url].concat(item.lg);
		};
	}

    function FormatResult(idx: number, fieldName: string, expected: string, actual: string): string {
        return "idx: " + idx + "</br>field: " + fieldName + "</br>expected: " + expected +"</br>actual: "+ actual;
    }

    // Returns string with comparison result. 
    // Returns empty string if expected and actual are equal.
    function LogItemArraysCompareResult(expected: JL.LogItem[], actual: JL.LogItem[]): string {
        var nbrLogItems = expected.length;
        var i;

        if (nbrLogItems != actual.length) {
            return "Actual nbr log items (" +
                actual.length + ") not equal expected nbr log items (" +
                nbrLogItems + ")";
        }

        for (i = 0; i < nbrLogItems; i++) {
            if (expected[i].l != actual[i].l) {
                return FormatResult(i, "level", expected[i].l.toString(), actual[i].l.toString());
            }

            var m: any = expected[i].m;
            var match = false;

            if (m instanceof RegExp)
            {
                match = m.test(actual[i].m);
            }
            else
            {
                match = (expected[i].m == actual[i].m);
            }

            if (!match)
            {
                return FormatResult(i, "msg", expected[i].m, actual[i].m);
            }

            if (expected[i].n != actual[i].n)
            {
                return FormatResult(i, "logger name", expected[i].n, actual[i].n);
            }

            // Timestamps are precise to the ms.
			// Allow a small difference between actual and expected, because we record the timestamp
			// a bit later then when jsnlog produces the log request.

			var allowedDifferenceMs = 10; 
			
            if (Math.abs(expected[i].t - actual[i].t) > allowedDifferenceMs) {
                return FormatResult(i, "timestamp", expected[i].t.toString(), actual[i].t.toString());
            }
        }

        return "";
    }
}

