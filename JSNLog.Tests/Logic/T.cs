using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JSNLog.Tests.Logic
{
    /// <summary>
    /// A single T defines both an action and an expected outcome.
    /// You would combine a number of Ts into a test.
    /// 
    /// When you set Level and Logger, that generates logging a generated message with the given logger and level.
    /// 
    /// If you set checkNbr, a check will be done whether all expected log messages have been processed by the appender with javascript variable name CheckAppender.
    /// 
    /// The check is just a number. It will be checked whether all Ts where checkExpected equals the check number have been
    /// processed by the appender.
    /// </summary>
    public class T
    {
        public int Level { get; private set; }
        public string Logger { get; private set; }
        public int CheckNbr { get; private set; }
        public string CheckAppender { get; private set; }
        public int CheckExpected { get; private set; }
        public string Header { get; private set; }

        public T(int level = -1, string logger = "", int checkExpected = -1, int checkNbr = -1, string checkAppender = "a0", string header = "")
        {
            Level = level;
            Logger = logger;
            CheckExpected = checkExpected;
            CheckNbr = checkNbr;
            CheckAppender = checkAppender;
            Header = header;
        }
    }
}