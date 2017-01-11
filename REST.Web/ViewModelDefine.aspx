<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewModelDefine.aspx.cs" Inherits="REST.Web.ViewModelDefine" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title></title>
</head>
<body>
    <div style="margin:0 auto;">
    <%=Txt %>
    </div>
</body>
</html>
<script type="text/javascript" language="javascript">
    function OutModelJava(obj, i) {
        obj.setAttribute('href', obj.getAttribute('href') + "&packagename=" + document.getElementById("PackageName" + i).value);
    }
</script>