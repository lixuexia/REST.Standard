<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Version.aspx.cs" Inherits="REST.Web.Version" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
<meta http-equiv="Content-Style-Type" content="text/css"/>
<title>REST服务接口说明-<%=VersionTxt%></title>
</head>
<FRAMESET style="border:1px solid #ccc;" cols="20%,80%" title="" onLoad="top.loadFrames()">
    <FRAMESET rows="30%,70%" title="" onLoad="top.loadFrames()">
        <FRAME src="List.aspx?Version=<%=VersionTxt %>" target="vg">
        <FRAME name="vg" target="vd">
    </FRAMESET>
    <FRAMESET>
        <FRAME name="vd" scrolling="yes">
    </FRAMESET>
    <NOFRAMES>
    </NOFRAMES>
</FRAMESET>
</html>