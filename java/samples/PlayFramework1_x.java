import java.io.InputStream;

import jsnlog.JSNLog;
import play.mvc.Controller;
import play.mvc.Http;

public class Application extends Controller {
	public static void jsnLog() {
		InputStream postBody = Http.Request.current().body;
		
		StringBuffer json = new StringBuffer();
		
		byte[] inputBuffer = new byte[1024];
		int size = 0;
		while ((size = postBody.read(inputBuffer, 0, inputBuffer.length)) > 0) {
			json.append(new String(inputBuffer, 0, size));
		}
		
		JSNLog.getPlayFramework1_xLogger().log(json.toString());
		
		renderText("{status: \"success\"}");
	}
}
