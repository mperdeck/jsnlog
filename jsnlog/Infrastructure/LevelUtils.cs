using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Infrastructure
{
    // make LevelUtils public, so the web site project can access this info
    public class LevelUtils
    {
        /// <summary>
        /// Converts a number to a level.
        /// 
        /// Each level is associated with a value. If the number is equal to or lower than a level, but higher than the previous level, than 
        /// that level is used. So if:
        /// 
        /// TRACE = 1000,
        /// DEBUG = 2000,
        /// INFO = 3000,
        /// WARN = 4000,
        /// ERROR = 5000,
        /// FATAL = 6000
        /// 
        /// And the number is: 2500, than this method returns INFO.
        /// 
        /// If the number is greater than FATAL (highest level), than FATAL is returned.
        /// If the number is lower than TRACE, than TRACE is returned.
        /// 
        /// This method assumes that the Level enum is sorted by value!
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Level IntToLevel(int i)
        {
            Array values = Enum.GetValues(typeof(Level));
            int nbrItems = values.Length;

            for(int j = 0; j < nbrItems; j++)
            {
                int value = (int)values.GetValue(j);
                if (value >= i) 
                {
                    Level level = (Level)Enum.Parse(typeof(Level), value.ToString()); 
                    return level;
                }
            }

            // No level found. Return the highest level.
            return HighestLevel();
        }

        /// <summary>
        /// Returns the highest level
        /// as given in Level enum.
        /// </summary>
        /// <returns></returns>
        public static Level HighestLevel()
        {
            Array values = Enum.GetValues(typeof(Level));
            int value = (int)values.GetValue(values.Length - 1);

            Level level = (Level)Enum.Parse(typeof(Level), value.ToString()); 
            return level;
        }

        /// <summary>
        /// Parses a string with the name or value of a level.
        /// </summary>
        /// <param name="levelString"></param>
        /// <returns>
        /// null if levelString is null.
        /// Otherwise, the actual level.
        /// </returns>
        public static Level? ParseLevel(string levelString)
        {
            if (levelString == null)
            {
                return null;
            }

            // See if levelString contains the name of a level. If so, Enum.Parse it.
           if (Enum.IsDefined(typeof(Level), levelString))  
           {
                Level level = (Level)Enum.Parse(typeof(Level), levelString); 
                return level;
           }

            // If levelString contains a number, parse that
            int levelInt;
            bool isInt = int.TryParse(levelString, out levelInt);
            
            if (isInt)
            {
                return IntToLevel(levelInt);
            }

            throw new Exception(string.Format("Unknown level {0}", levelString));
        }

        /// <summary>
        /// Returns the friendly name of the given level if possible.
        /// 
        /// If level is a number matching one of the predefined levels, that level's name is returned.
        /// Otherwise, level is returned.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string PredefinedName(string level)
        {
            int levelInt;
            if (!int.TryParse(level, out levelInt))
            {
                return level;
            }

            Array values = Enum.GetValues(typeof(Level));
            Array names = Enum.GetNames(typeof(Level));
            int nbrItems = values.Length;

            for (int j = 0; j < nbrItems; j++)
            {
                int value = (int)values.GetValue(j);
                if (value == levelInt)
                {
                    return (string)names.GetValue(j);
                }
            }

            return level;
        }

        /// <summary>
        /// Determines the numeric value of a level.
        /// If level is a number, returns the number.
        /// If level is a predefined level name, returns number corresponding to that level.
        /// Otherwise throws exception.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int LevelNumber(string level)
        {
            int levelInt;
            if (int.TryParse(level, out levelInt))
            {
                return levelInt;
            }

            // See if levelString contains the name of a level. If so, Enum.Parse it and returns its number.
            if (Enum.IsDefined(typeof(Level), level))
            {
                Level levelEnum = (Level)Enum.Parse(typeof(Level), level);
                return (int)levelEnum;
            }

            throw new Exception(string.Format("Unknown level {0}", level));
        }

        /// <summary>
        /// Throws exception if level is not a valid level.
        /// </summary>
        /// <param name="level"></param>
        public static void ValidateLevel(string level)
        {
            if (string.IsNullOrEmpty(level)) { return; }

            LevelNumber(level);
        }

        /// <summary>
        /// Returns a string with all named levels, separated by |
        /// 
        /// This is used by the web site project.
        /// </summary>
        /// <returns></returns>
        public static string NamedLevels()
        {
            string[] names = Enum.GetNames(typeof(Level));

            string namedLevels = string.Join("|", names);
            return namedLevels;
        }
    }
}
