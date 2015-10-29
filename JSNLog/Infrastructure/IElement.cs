using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Infrastructure
{
    /// <summary>
    /// Each element (logger, appenders, etc) that can appear in the jsnlog element in web.config
    /// is represented by a class implementing this interface.
    /// 
    /// The only exception to this is the assembly element.
    /// 
    /// To add an assembly with new elements:
    /// 1) Create new project, with class(es) deriving from IElement. It will reference jsnlog.dll to get this interface and library methods.
    /// 2) Add reference to new project to the project with the web site (not jsnlog.dll)
    /// 3) Add assembly tags within the jsnlog tag. Set their name attribute to the name of the new assembly. 
    /// JSNLog will read the assemblies and read the new elements.
    /// </summary>
    internal interface IElement
    {
        /// <summary>
        /// When an assembly element is processed, all classes in that assembly implementing this interface
        /// are instanteated. Then their Init methods are called.
        /// </summary>
        /// <param name="tagInfo">
        /// Describes the new element. This will be added to the end of the list of tagInfos to be processed.
        /// This means that you can determine the order in which each type of element is processed
        /// via their assembly elements.
        /// </param>
        void Init(out XmlHelpers.TagInfo tagInfo);
    }
}
