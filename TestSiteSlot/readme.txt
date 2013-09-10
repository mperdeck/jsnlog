While integration testing a nuget package, a site that does not have jsnlog installed is copied into this directory, and then
jsnlog is installed in it using the nuget package (automatically, as part of the test). 

After the test, it is then removed.

So normally, this directory is empty. Do not remove it.

 