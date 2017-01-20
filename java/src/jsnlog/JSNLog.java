package jsnlog;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;

import jsnlog.loggers.CommonsLogger;
import jsnlog.loggers.Log4JLogger;
import jsnlog.loggers.Logger;
import jsnlog.loggers.PlayFramework1_xLogger;

import com.google.gson.Gson;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;

public class JSNLog {
	
	private static HashMap<String, JSNLog> instances = new HashMap<String, JSNLog>(); 
	
	private static String[] supportedLoggers = {
		"ApacheCommons",
		"Log4J"
	};

	private Logger logger;
	
	private DateFormat dateFormat = new SimpleDateFormat("M-d-YYYY hh:mm:ss");
	
	private JsonParser jsonParser = null;
	
	private Gson gson = null;
	
	public static String[] getSupportedLoggers() {
		return supportedLoggers;
	}
	
	private JSNLog(Logger logger) {
		this.logger = logger;
	}
	
	public static JSNLog getInstance(Logger customLogger) {
		String logType = customLogger.getClass().getName();
		
		JSNLog log = instances.get(logType.toLowerCase());
		if (log != null) {
			return log;
		}
		
		log = new JSNLog(customLogger);
		instances.put(logType, log);
		
		return log;
	}
	
	public static JSNLog getInstance(String logType) {
		JSNLog log = instances.get(logType.toLowerCase());
		if (log != null) {
			return log;
		}
		
		if ("ApacheCommons".equalsIgnoreCase(logType)) {
			Logger logger = new CommonsLogger();
			log = new JSNLog(logger);
		} else if ("Log4J".equalsIgnoreCase(logType)) {
			Logger logger = new Log4JLogger();
			log = new JSNLog(logger);
		} else if ("PlayFramework1_x".equalsIgnoreCase(logType)) {
			Logger logger = new PlayFramework1_xLogger();
			log = new JSNLog(logger);
		}
		
		if (log == null) {
			return getInstance("ApacheCommons");
		}
		
		instances.put(logType.toLowerCase(), log);
		
		return log;
	}
	
	public static JSNLog getApacheCommonsLogger() {
		return getInstance("ApacheCommons");
	}
	
	public static JSNLog getLog4JLogger() {
		return getInstance("Log4J");
	}

	public static JSNLog getPlayFramework1_xLogger() {
		return getInstance("PlayFramework1_x");
	}

	public void setDateFormat(DateFormat dateFormat) {
		this.dateFormat = dateFormat;
	}
	
	public void log(String jsnLogData) {
		if (jsonParser == null) {
			jsonParser = new JsonParser();
		}
		
		if (gson == null) {
			gson = new Gson();
		}

		JsonObject headerJson = jsonParser.parse(jsnLogData).getAsJsonObject();
		JsonArray messages = headerJson.getAsJsonArray("lg");
		
		for (int messageNum = 0; messageNum < messages.size(); messageNum++) {
			JsonObject message = messages.get(messageNum).getAsJsonObject();

			int level = message.get("l").getAsInt();
			String messageData = message.get("m").getAsString();
			String loggerName = message.get("n").getAsString();
			long timestamp = message.get("t").getAsLong()*1000;

			if (level<=1000) {
				trace(loggerName, timestamp, messageData);
			} else if (level<=2000) {
				debug(loggerName, timestamp, messageData);
			} else if (level<=3000) {
				info(loggerName, timestamp, messageData);
			} else if (level<=4000) {
				warn(loggerName, timestamp, messageData);
			} else if (level<=5000) {
				error(loggerName, timestamp, messageData);
			} else if (level<=6000) {
				fatal(loggerName, timestamp, messageData);
			}
		}
	}

	private String formatMessage(String loggerName, long timestamp, String message) {
		return loggerName+": "+dateFormat.format(new Date(timestamp))+" - "+message;
	}

	private void trace(String loggerName, long timestamp, String message) {
		if (logger.isLevelEnabled("trace")) {
			logger.trace(formatMessage(loggerName, timestamp, message));
		}
	}

	private void debug(String loggerName, long timestamp, String message) {
		if (logger.isLevelEnabled("debug")) {
			logger.debug(formatMessage(loggerName, timestamp, message));
		}
	}

	private void info(String loggerName, long timestamp, String message) {
		if (logger.isLevelEnabled("info")) {
			logger.info(formatMessage(loggerName, timestamp, message));
		}
	}

	private void warn(String loggerName, long timestamp, String message) {
		if (logger.isLevelEnabled("warn")) {
			logger.warn(formatMessage(loggerName, timestamp, message));
		}
	}

	private void error(String loggerName, long timestamp, String message) {
		if (logger.isLevelEnabled("error")) {
			logger.error(formatMessage(loggerName, timestamp, message));
		}
	}

	private void fatal(String loggerName, long timestamp, String message) {
		if (logger.isLevelEnabled("fatal")) {
			logger.fatal(formatMessage(loggerName, timestamp, message));
		}
	}


}
