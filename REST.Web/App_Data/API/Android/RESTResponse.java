package API;

public class RESTResponse {
	/**
	 * ��Ӧ����
	 */
    private String ErrorCode="";

    /**
     * ��Ӧ����
     */
    private String ErrorMsg="";

    /**
     * ��Ӧ��
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