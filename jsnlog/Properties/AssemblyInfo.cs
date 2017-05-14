using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("JSNLog")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("JSNLog")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

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

// Allow project JSNLog.Tests to access the internals of this project. Public key is needed because JSNLog has a strong name.
[assembly: InternalsVisibleTo("JSNLog.Tests, PublicKey=" +
    "002400000480000094000000060200000024000052534131000400000100010015488505fd9c86" +
    "9dfb9af3ee7d980a24f9a379fbea34c6311b481f77688c74fae162335ae47a8ef800bfd83c1795" +
    "97ab12c86278065bd9cbc1997863f5bc4a2f03b5a519e0b7097edb5649e7d982b94f7d6c8ef60d" +
    "35f79aaf9785d1f79d5bc0c529edc38fb99dc88cb7475d32946286b0766b3bc32bc4bd9871768f" +
    "70ea49ba")]

