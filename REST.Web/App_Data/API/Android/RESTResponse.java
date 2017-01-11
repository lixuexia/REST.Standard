package API;

public class RESTResponse {
	/**
	 * 响应编码
	 */
    private String ErrorCode="";

    /**
     * 响应描述
     */
    private String ErrorMsg="";

    /**
     * 响应体
     */
    private String Body="";

	public String getErrorCode() {
		return ErrorCode;
	}

	public void setErrorCode(String errorCode) {
		ErrorCode = errorCode;
	}

	public String getErrorMsg() {
		return ErrorMsg;
	}

	public void setErrorMsg(String errorMsg) {
		ErrorMsg = errorMsg;
	}

	public String getBody() {
		return Body;
	}

	public void setBody(String body) {
		Body = body;
	}
}