# JSNLog

JSNLog is a JavaScript logging library with strong integration in .Net. Configure your JavaScript loggers in your server side web.config. Receive log messages on the server and have them logged by your server side logging package (Log4Net, NLog, etc.) without writing a line of code.

## Background

You probably use a server side logging package (NLog, Log4Net, etc.) to log exceptions in your C# or Visual Basic code, and to find out what's going on inside your code. You'll probably configure your loggers in your web.config or via a configuration file - setting logger levels, determining where log messages get stored, etc.

With more and more functionality running in the browser instead of the server, we need the same logging goodness in our JavaScript code. However, traditionally, logging packages for JavaScript haven't allowed you to configure the JavaScript loggers from your web.config. And you had to write code to capture their logging output and store it on the server.

JSNLog (JavaScript .Net Logging) changes all this:

* Insert loggers in your JavaScript.
* Log any object, not just strings.
* Configure your JavaScript loggers in your web.config.
* Capture the output from your JavaScript loggers on your server, and pass it on to your server side logging package for storage - without writing code.
* Batch log messages, to reduce the number of requests being sent to your server.

## Getting Started

Visit [jsnlog.com](http://www.jsnlog.com):

* Install JSNLog with the JSNLog NuGet package; 
* Get started quickly with JSNLog;
* Get full documentation of all JavaScript methods and web.config elements.

