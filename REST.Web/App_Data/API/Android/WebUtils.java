package API;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.security.MessageDigest;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.Set;
import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.params.CoreConnectionPNames;

public class WebUtils {
    private static int TimeOut = 100000;

    public static int getTimeOut()
    {
         return TimeOut; 
    }
    
    public static void setTimeOut(int value)
    { 
    	TimeOut = value; 
    }

    public static String SignRequest(Map<String, String> parameters, String secret)
    {
        Map<String, String> sortedParams = parameters;
        StringBuilder query = new StringBuilder();
        Set<String> keySet = sortedParams.keySet(); 
        Iterator<String> iter = keySet.iterator(); 
        while(iter.hasNext())
        {
        	String key = iter.next();
        	if (key!="")
            {
                query.append(key).append(sortedParams.get(key));
            }
        }
        query.append(secret);
        return Md5(query.toString());
    }
    
	private static String Md5(String plainText ) 
    { 
    	try
    	{ 
	    	MessageDigest md = MessageDigest.getInstance("MD5"); 
	    	md.update(plainText.getBytes()); 
	    	byte b[] = md.digest(); 
	    	int i;
	    	StringBuffer buf = new StringBuffer(""); 
	    	for (int offset = 0; offset < b.length; offset++) 
	    	{ 
		    	i = b[offset]; 
		    	if(i<0) i+= 256; 
		    	if(i<16) 
		    	buf.append("0"); 
		    	buf.append(Integer.toHexString(i)); 
	    	}
	    	return buf.toString().toUpperCase(Locale.getDefault());
    	}
    	catch (Exception e)
    	{ 
    		System.out.println(e.getMessage());
    		return "";
    	}
    }
    
    /**
     * 向服务器发送POST请求
     * @param url 请求地址
     * @param postParameters POST数据
     * @return 返回字符串文本
     */
	public static String RESTPost(String url, Map<String, String> postParameters) {
		try {
			HttpClient client = new DefaultHttpClient();
			client.getParams().setParameter(CoreConnectionPNames.SO_TIMEOUT, TimeOut);
			HttpPost httpPost = new HttpPost(url);
			List<NameValuePair> parameters = new ArrayList<NameValuePair>();
			Set<String> keySet=postParameters.keySet();
        	Iterator<String> dem = keySet.iterator();
        	while(dem.hasNext())
        	{
        		String key=dem.next();
        		String value=postParameters.get(key);
        		parameters.add(new BasicNameValuePair(key, value));
        	}
			UrlEncodedFormEntity entity= new UrlEncodedFormEntity(parameters,"UTF-8");
			httpPost.setEntity(entity);
			HttpResponse response = client.execute(httpPost);
			int code = response.getStatusLine().getStatusCode();
			if (code == 200) {
				InputStream is = response.getEntity().getContent();
				return readStream(is);
			} else {
				return null;
			}
		} catch (Exception e) {
			e.printStackTrace();
			return null;
		}
	}
	
	private static String readStream(InputStream is){
		try {
			ByteArrayOutputStream baos = new ByteArrayOutputStream();
			int len = 0;
			byte[] buffer = new byte[1024];
			while((len = is.read(buffer))!=-1){
				baos.write(buffer, 0, len);
			}
			is.close();
			baos.close();
			byte[] result = baos.toByteArray();
			return new String(result);
		} catch (IOException e) {
			e.printStackTrace();
			return null;
		}
	}
}