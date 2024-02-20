﻿using System;
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
        [XmlAttribute]
        public string name { get; set; }

        [XmlAttribute]
        public string sendWithBufferLevel { get; set; }

        [XmlAttribute]
        public string storeInBufferLevel { get; set; }

        [XmlAttribute]
        public uint bufferSize { get; set; }

        [XmlAttribute]
        public uint batchSize { get; set; }

        [XmlAttribute]
        public uint maxBatchSize { get; set; }

        [XmlAttribute]
        public uint batchTimeout { get; set; }

        [XmlAttribute]
        public uint sendTimeout { get; set; }

        public Appender()
        {
            // Do NOT set defaults for level, storeInBufferLevel, sendWithBufferLevel.
            // Method ValidateAppender checks if all of these have been given by the user
            // when at least one has been given (you must set either none or all).
            // It can't distinguish between user supplied values and defaults.
            // Note that jsnlog.js sets defaults itself.

            bufferSize = 0;
            batchSize = 1;
            maxBatchSize = 20;
            batchTimeout = 2147483647;
            sendTimeout = 5000;
        }

        // --------------------------------------------------------

        protected string FieldName { get { return "name"; } }
        protected string FieldSendWithBufferLevel { get { return "sendWithBufferLevel"; } }
        protected string FieldStoreInBufferLevel { get { return "storeInBufferLevel"; } }
        protected string FieldBufferSize { get { return "bufferSize"; } }
        protected string FieldBatchSize { get { return "batchSize"; } }
        protected string FieldMaxBatchSize { get { return "maxBatchSize"; } }
        protected string FieldBatchTimeout { get { return "batchTimeout"; } }
        protected string FieldSendTimeout { get { return "sendTimeout"; } }

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

            if (maxBatchSize < batchSize)
            {
                throw new GeneralAppenderException(name,
                    string.Format("maxBatchSize ({0}) is smaller than batchSize ({1})", maxBatchSize, batchSize));
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
            JavaScriptHelpers.AddJsonField(jsonFields, FieldMaxBatchSize, maxBatchSize);
            JavaScriptHelpers.AddJsonField(jsonFields, FieldBatchTimeout, batchTimeout);
            JavaScriptHelpers.AddJsonField(jsonFields, FieldSendTimeout, sendTimeout);

            base.AddJsonFields(jsonFields, appenderNames, virtualToAbsoluteFunc);
        }
    }
}
