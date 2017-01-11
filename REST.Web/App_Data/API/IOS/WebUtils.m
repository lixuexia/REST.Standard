#import <CommonCrypto/CommonDigest.h>/*MD5加密引用*/

@implementation WebUtils
	/**
	* 对请求签名
	*/
    + (NSString *)SignRequest : (NSMutableDictionary *)parameters : (NSString *)secret
    {
        NSMutableDictionary* sortedParams = parameters;
        NSString* query = @"";
		NSArray *srcKeys = [parameters allKeys]; 
		NSArray *sortedKeys = [srcKeys sortedArrayUsingSelector:@selector(caseInsensitiveCompare:)];
		for(id key in sortedKeys) {
			id object = [parameters objectForKey:key];
			//[sortedValues addObject:object];
			query  = [query stringByAppendingString : (NSString *)object];
		}
        query = [query stringByAppendingString : secret];
        return Md5(query);
    }

	/**
	* MD5加密
	* @str 要加密的数据
	*/
	+ (NSString *)Md5 : (NSString *)str
	{
		const char *cStr = [str UTF8String];
		unsigned char result[16];
		CC_MD5(cStr, strlen(cStr), result);
		return [NSString stringWithFormat:
			@"%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x",
			result[0], result[1], result[2], result[3], 
			result[4], result[5], result[6], result[7],
			result[8], result[9], result[10], result[11],
			result[12], result[13], result[14], result[15]
			]; 
	}
    
    /**
     * 向服务器发送POST请求
     * @url 请求地址
     * @postParameters POST数据
     * @返回字符串文本
     */
	+ (NSString *)RESTPost : (NSString *)url : (NSMutableDictionary *)postParameters 
	{
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
}
@end