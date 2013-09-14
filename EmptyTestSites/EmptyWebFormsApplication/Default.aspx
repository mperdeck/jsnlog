<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EmptyWebFormsApplication.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<%--    <%= JSNLog.JavascriptLogging.Configure() %>--%>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>EmptyWebFormsApplication</h1>
    <script type="text/ecmascript">
        // JL("jsLogger").fatal("log message");
        alert('EmptyWebFormsApplication');
    </script>
        

    </div>
    </form>
</body>
</html>
