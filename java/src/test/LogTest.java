package test;

import jsnlog.JSNLog;

public class LogTest {
	
	public static void main(String[] args) {
		String json = "{r:  \"requestid\", "+
				"lg: [{ l: 1000, m: 'trace', n: 'trace logger', t: 1111 },"+
				"{ l: 2000, m: 'debug', n: 'debug logger', t: 2222 },"+
				"{ l: 3000, m: 'info', n: 'info logger', t: 3333 },"+
				"{ l: 4000, m: 'warn', n: 'warn logger', t: 4444 },"+
				"{ l: 5000, m: 'error', n: 'error logger', t: 5555 },"+
				"{ l: 6000, m: 'fatal', n: 'fatal logger', t: 1386522623 }"+
				"]}";
		
		System.out.println ("Parsing log: "+json);
		JSNLog commonsLog = JSNLog.getApacheCommonsLogger();
		commonsLog.log(json);
		
		JSNLog log4jLog = JSNLog.getLog4JLogger();
		log4jLog.log(json);

	}
}
