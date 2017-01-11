<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signature.aspx.cs" Inherits="REST.Web.Signature" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title></title>
</head>
<body>
<p>
<strong>应用级输入参数</strong>
</p>
<table border="1" width="100%" summary="实例">
    <tr bgcolor="#F2F5A9">
        <td>名称</td>
        <td>类型</td>
        <td>描述</td>
         <td>示例</td>
    </tr>
    <tr>
        <td>parameter</td>
        <td>string</td>
        <td>标准json 类型</td>
        <td>{"CommonString":"001","CommonNumber":0,"CommonDecimal":0.0,"CommonBool":false}</td>
    </tr>
</table>
<br/>
<p>
<b>加密方式：</b><br/><br/>
    &nbsp;&nbsp;采用md5加密方式，加密规则为：<br/>
   <div style="margin-left:50px">
        a)	<b>所有请求参数按照字母先后顺序排序</b><br/>
            &nbsp;&nbsp;例如：将method,app_key,version,timestamp 排序为 app_key,method,timestamp,version  <br/>
        b)	<b>把所有参数名和参数值串在一起</b><br/>
            &nbsp;&nbsp; 例如：app_keyxxxxxxxmethodxxxxxxtimestampxxxxxxversionxxxxx<br/>
        c)	<b>把AppSecret夹在字符串的末端 </b><br/>
            &nbsp;&nbsp;例如：XXXX+AppSecret<br/>
        d)	<b>使用MD5进行加密，再转化成大写</b><br/>
         (其中app_key为分配给商家)。
      <br/>
      <br/>
     <br/>
        <b>签名实例</b><br/>
        &nbsp;&nbsp;调用api：REST.Common.GetAreas，app_key=20032,app_secret:16A774A73558650EC4E2B0C5E39C9F3D<br/>
       <b> 输入参数：</b><br/>
        <div style="margin-left:20px">
            method=REST.Common.GetAreas<br/>
            timestamp=2012-11-01 11:10:04<br/>
            app_key=20032<br/>
            version=1.0<br/>
            parameter={"CommonString":"001","CommonNumber":0,"CommonDecimal":0.0,"CommonBool":false}
            <br/>
        </div>
       <B> 按照参数名称排序</B><br/>
         <div style="margin-left:20px">
            app_key=20032<br />
            method=REST.Common.GetAreas<br/>
            parameter={"CommonString":"001","CommonNumber":0,"CommonDecimal":0.0,"CommonBool":false}<br/>
            timestamp=2012-11-01 11:10:04<br/>
            version=1.0<br/>
           </div>
       <b>  连接参数名与参数值,并在尾加上key</b><br/>
        <div style="margin-left:20px">
          app_key20032methodFEBS.Common.GetAreasparameter{"CommonString":"001","CommonNumber":0,"CommonDecimal":0.0,"CommonBool":false}timestamp2012-11-01 11:10:04version1.016A774A73558650EC4E2B0C5E39C9F3D<br/>
         </div>
        <b> MD5加密后转成大写：</b><br/>
        &nbsp;&nbsp;839E9244C97E2413BED05042130BAD6C
   </div>
</p>
<br/>
</body>
</html>