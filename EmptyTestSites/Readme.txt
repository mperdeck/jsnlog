The web sites in this directory are used during integration testing, when testing the nuget package.

They all do client side logging, but the nuget package has not been imported in them.

During a test, a copy is made in a scratch area, and then the nuget package is imported into the copy. 
The site it then loaded in a browser and the logs examined to see if they are as expected.


