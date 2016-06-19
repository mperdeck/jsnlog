using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using JSNLog.Exceptions;
using JSNLog.Infrastructure;
using JSNLog.ValueInfos;

namespace JSNLog
{
    public class Appender : FilterOptions, ICanCreateJsonFields 
    {
#if SUPPORTSXML
        [XmlAttribute]
#endif
        public string name { get; set; }

#if SUPPORTSXML
        [XmlAttribute]
#endif
        public string sendWithBufferLevel { get; set; }

#if SUPPORTSXML
        [XmlAttribute]
#endif
        public string storeInBufferLevel { get; set; }

#if SUPPORTSXML
        [XmlAttribute]
#endif
        public uint bufferSize { get; set; }

#if SUPPORTSXML
        [XmlAttribute]
#endif
        public uint batchSize { get; set; }

        public Appender()
        {
            // Do NOT set defaults for level, storeInBufferLevel, sendWithBufferLevel.
            // Method ValidateAppender checks if all of these have been given by the user
            // when at least one has been given (you must set either none or all).
            // It can't distinguish between user supplied values and defaults.
            // Note that jsnlog.js sets defaults itself.

            bufferSize = 0;
            batchSize = 1;
        }

        // --------------------------------------------------------

        protected string FieldName { get { return "name"; } }
        protected string FieldSendWithBufferLevel { get { return "sendWithBufferLevel"; } }
        protected string FieldStoreInBufferLevel { get { return "storeInBufferLevel"; } }
        protected string FieldBufferSize { get { return "bufferSize"; } }
        protected string FieldBatchSize { get { return "batchSize"; } }

        protected void CreateAppender(StringBuilder sb, Dictionary<string, string> appenderNames, int sequence,
            Func<string, string> virtualToAbsoluteFunc, string jsCreateMethodName, string configurationObjectName)
        {
            try
            {
                ValidateAppender(configurationObjectName);

                string appenderVariableName = string.Format("{0}{1}", Constants.JsAppenderVariablePrefix, sequence);
                appenderNames[name] = appenderVariableName;

                JavaScriptHelpers.GenerateCreate(appenderVariableName, jsCreateMethodName, name, sb);

                JavaScriptHelpers.GenerateSetOptions(appenderVariableName, this, appenderNames, virtualToAbsoluteFunc,
                    sb, null);
            }
            catch (Exception e)
            {
                throw new ConfigurationException(name, e);
            }
        }

        private void ValidateAppender(string configurationObjectName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new MissingAttributeException(configurationObjectName, FieldName);
            }

            // Ensure that if any of the buffer specific attributes are provided, they are all provided, and that they make sense.

            bool levelGiven = !string.IsNullOrEmpty(level);
            bool sendWithBufferLevelGiven = !string.IsNullOrEmpty(sendWithBufferLevel);
            bool storeInBufferLevelGiven = !string.IsNullOrEmpty(storeInBufferLevel);
            bool bufferSizeGiven = (bufferSize > 0);

            if (sendWithBufferLevelGiven ||
                storeInBufferLevelGiven ||
                bufferSizeGiven)
            {
                if ((!sendWithBufferLevelGiven) ||
                    (!storeInBufferLevelGiven) ||
                    (!bufferSizeGiven))
                {
                    throw new GeneralAppenderException(name, string.Format(
                        "If any of {0}, {1} or {2} is specified, than the other two need to be specified as well",
                        FieldSendWithBufferLevel, FieldStoreInBufferLevel, FieldBufferSize));
                }

                int levelNumber =
                    levelGiven ?
                    LevelUtils.LevelNumber(level) :
                    (int)Constants.DefaultAppenderLevel;

                int storeInBufferLevelNumber = LevelUtils.LevelNumber(storeInBufferLevel);
                int sendWithBufferLevelNumber = LevelUtils.LevelNumber(sendWithBufferLevel);

                if ((storeInBufferLevelNumber > levelNumber) || (levelNumber > sendWithBufferLevelNumber))
                {
                    throw new GeneralAppenderException(name, string.Format(
                        "{0} must be equal or greater than {1} and equal or smaller than {2}",
                        FieldLevel, FieldStoreInBufferLevel, FieldSendWithBufferLevel));
                }
            }
        }

        // Implement ICanCreateJsonFields
        public override void AddJsonFields(IList<string> jsonFields, Dictionary<string, string> appenderNames, Func<string, string> virtualToAbsoluteFunc)
        {
            var levelValue = new LevelValue();

            JavaScriptHelpers.AddJsonField(jsonFields, FieldSendWithBufferLevel, sendWithBufferLevel, levelValue);
            JavaScriptHelpers.AddJsonField(jsonFields, FieldStoreInBufferLevel, storeInBufferLevel, levelValue);
            JavaScriptHelpers.AddJsonField(jsonFields, FieldBufferSize, bufferSize);
            JavaScriptHelpers.AddJsonField(jsonFields, FieldBatchSize, batchSize);
            
            base.AddJsonFields(jsonFields, appenderNames, virtualToAbsoluteFunc);
        }
    }
}
