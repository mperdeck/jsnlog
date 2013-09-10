Additional installation for JSNLog
	
After installing the JSNLog NuGet package, 2 more things need to be done:
1) Call Configure
2) Install Common.Logging binding for your favorite server side logging package

1) Call Configure
=================
JSNLog exposes a Configure method that generates a script tag for the jsnlog.js library
and that converts JSNLog elements in your web.config JavaScript code that actually configures
the loggers.

You need to call this Configure method on your page before any script tags that load JavaScript
that use JSNLog loggers.

In a razor file (normally used with MVC3 and up), use:
@Html.Raw(JSNLog.JavascriptLogging.Configure())

In an .asxp file, use:
<%= JSNLog.JavascriptLogging.Configure() %>
	
2) Install Common.Logging binding for your favorite server side logging package
===============================================================================
JSNLog needs to pass client side log messages to your server side logging package.
However, it doesn't know what package you use - Log4Net, NLog, Elmah, etc.
	
So it uses a common logging interface, implemented by the Common.Logging NuGet package.
This gets installed along with JSNLog itself.

However, to make this work, you need to install the binding between Common.Logging and whatever
logging package you're using right now server side. This binding is simply another NuGet package.

You'll find these NuGet packages at:
http://nuget.org/packages?q=common.logging

Pick the NuGet package associated with your server side logging package, and install that.

For Log4Net, NLog and Enterprise Library, configure common.logging. See
http://netcommon.sourceforge.net/docs/2.1.0/reference/html/index.html
