Regarding EmptyTestSiteLog4Net:

The web sites in this directory are used during integration testing, when testing the nuget package.

They all do client side logging, but the nuget package has not been imported in them.

During a test, a copy is made in a scratch area, and then the nuget package is imported into the copy. 
The site it then loaded in a browser and the logs examined to see if they are as expected.


========================

The other sites are very simple sites that log 1 entry, using log4net, nlog, elmah.
Copy to other directory, and use to make sure that manual installation works well.

========================

All the MVC sites use MVC 5.