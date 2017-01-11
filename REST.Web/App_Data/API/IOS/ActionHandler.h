

public class ActionHandler
{
    private String url = "HTTP://REST.AEUNION.COM/";
    private static String MerchantCode = "REST01";
    private static String MerchantPassword = "6F2D157B25E916C064CF07BB5B98059D";
    private String MethodName;
    private Object Model;
    
    public ActionHandler()
    {

    }
    
    public ActionHandler(String methodName, Object model)
    {
        this.MethodName = methodName;
        this.Model = model;
    }

    @SuppressLint("SimpleDateFormat")
	private String GetStringData() throws Exception
    {
    	java.util.Date now = new java.util.Date(); 
        Map<String,String> wnd = new TreeMap<String,String>();
        Gson tgson=new Gson();
        String txt = tgson.toJson(Model);
        wnd.put("app_key", MerchantCode);
        wnd.put("method", MethodName);
        wnd.put("parameter", txt);
        wnd.put("timestamp", new java.text.SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(now));
        wnd.put("version", "v1.0");
        wnd.put("sign", WebUtils.SignRequest(wnd, MerchantPassword));
        String x = WebUtils.RESTPost(this.url, wnd);
        System.out.println(x);
        if (x!="")
        {
        	Gson gson = new Gson();
            RESTResponse wnr=gson.fromJson(x, RESTResponse.class);
            return wnr.getBody();
        }
        else
        {
        	return "";
        }
    }
    
    /**
     * 获取返回结果对象
     * @param c 返回对象类型
     * @return 指定类型对象
     * @throws InstantiationException
     * @throws IllegalAccessException
     */
    public <T> T GetModel(Class<T> c) throws InstantiationException,IllegalAccessException
    {
    	try
        {
            String data = GetStringData();
            if (data=="")
            {
                return null;
            }
            Gson gson=new Gson();
            return gson.fromJson(data, c);
        }
        catch (Exception e)
        {
            return null;
        }
    }

	public Type GetModel(Type t) {
		try
        {
            String data = GetStringData();
            if (data=="")
            {
                return null;
            }
            Gson gson=new Gson();
            return gson.fromJson(data, t);
        }
        catch (Exception e)
        {
            return null;
        }
	}
	
	public Object GetModelData(Type type)
    {
        try
        {
            String data = GetStringData();
            if (data=="")
            {
                return null;
            }
            Gson gson=new Gson();
            return gson.fromJson(data, type);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}