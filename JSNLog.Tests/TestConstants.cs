using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSNLog.Tests
{
    public class TestConstants
    {
        public const string _jsnlogDirectory = @"C:\Dev\JSNLog\";

        // All tests here write a partial with the complete demo html.
        // The partials are written to directory:
        public const string _demosDirectory = _jsnlogDirectory + @"jsnlog.website\WebSite\Views\Shared\Demos";

        public const string _jsnlogDllDirectory = _jsnlogDirectory + @"jsnlog\artifacts\bin\JSNLog\Debug\dnx451\JSNLog.dll";
    }
}
