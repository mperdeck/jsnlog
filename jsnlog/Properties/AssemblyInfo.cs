using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

// Get rid of the below attributes, because 1) When compiling for NetStandard they get copied from somewhere and the compiler 
// complains the below are duplicates; and 2) they are not used anyway.

//[assembly: AssemblyTitle("JSNLog")]
//[assembly: AssemblyDescription("")]
//[assembly: AssemblyConfiguration("")]
//[assembly: AssemblyCompany("")]
//[assembly: AssemblyProduct("JSNLog")]
//[assembly: AssemblyCopyright("Copyright ©  2016")]
//[assembly: AssemblyTrademark("")]
//[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("dc2f1e35-c7cc-4054-8671-424e801a104e")]

// Get the compiler to create a strong named assembly, using the key pair in this file.
// Note that they key file gets checked into Github, as per
// http://stackoverflow.com/questions/36141302/why-is-it-recommended-to-include-the-private-key-used-for-assembly-signing-in-op
[assembly: AssemblyKeyFileAttribute("../jsnlog.strongname.snk")]

// Allow project JSNLog.Tests to access the internals of this project. 
// Have to add the public key of JSNLog.Tests (which lives in the JSNLog.Tests solution), 
// because this JSNLog project is strongly signed.
//
// See
// https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.internalsvisibletoattribute?view=net-8.0&redirectedfrom=MSDN 
// https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-runtime-compilerservices-internalsvisibletoattribute
// To get the public key, in Visual Studio command window, run:
// sn -p jsnlog.tests.strongname.snk public.out
// sn -tp public.out
//
[assembly: InternalsVisibleTo("JSNLog.Tests, PublicKey=" +
    "002400000480000094000000060200000024000052534131000400000100010089dd0d93cfa65f" +
    "cebdc52181975e43cbe31d26ed803efeaa3ba5df99b602c5ca0dfc5131d14f15f8e57845171632" +
    "53ffb1f070476dd13062903dda4fa93deb0982bf3e7956da923e94f40ac22b42c356d8c4b9434f" +
    "a81d2c528afa798d7ff7ff3691a19e270acc184ed7c77a0c74991558be3e06ce8a08e8169d1ba3" +
    "78d17283")]

