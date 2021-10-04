
// Run this code once the page is fully loaded (including all JavaScript files)
window.addEventListener("load", function () {
	
	// Log with every severity
	JL("jsLogger").debug("debug client log message");
	JL("jsLogger").info("info client log message");
	JL("jsLogger").warn({ msg: 'warn client log message - logging object', x: 5, y: 88 });
	JL("jsLogger").error(function() { return "error client log message - returned by function"; });
	JL("jsLogger").fatal("fatal client log message");

	// Log caught exception
	try {
		// ReferenceError: xyz is not defined
		xyz;
	} catch (e) {
		// Log the exception
		JL().fatalException("Something went wrong!", e);
	}

	// ReferenceError: xyz2 is not defined.
	xyz2;
});




