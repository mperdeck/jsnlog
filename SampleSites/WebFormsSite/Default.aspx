<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>JSNLog sample - Webforms</title>
    
    <%--Add Configure before scripts that use logging--%>
    <%= JSNLog.JavascriptLogging.Configure() %>

    <script type="text/javascript" src="Scripts/jsnlog_speciallogger.js"></script>
    <script type="text/javascript" src="Scripts/jsnlog_simpletest.js"></script>
    <script type="text/javascript" src="Scripts/jsnlog_batchtest.js"></script>
    <script type="text/javascript" src="Scripts/jsnlog_test.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div>

<h2>JSNLog sample - Webforms</h2>
  <fieldset>
    <legend>jsnlog_simpletest</legend>
      <input type="button" id="jsnlog_simpletest_btnTrace" value="trace" />
      <input type="button" id="jsnlog_simpletest_btnDebug" value="debug" />
      <input type="button" id="jsnlog_simpletest_btnInfo" value="info" />
      <input type="button" id="jsnlog_simpletest_btnWarn" value="warn" />
      <input type="button" id="jsnlog_simpletest_btnError" value="error" />
      <input type="button" id="jsnlog_simpletest_btnFatal" value="fatal" />
  </fieldset>

  <fieldset>
    <legend>jsnlog_batchtest</legend>
      <input type="button" id="jsnlog_batchtest_btnTrace" value="trace" />
      <input type="button" id="jsnlog_batchtest_btnDebug" value="debug" />
      <input type="button" id="jsnlog_batchtest_btnInfo" value="info" />
      <input type="button" id="jsnlog_batchtest_btnWarn" value="warn" />
      <input type="button" id="jsnlog_batchtest_btnError" value="error" />
      <input type="button" id="jsnlog_batchtest_btnFatal" value="fatal" />
  </fieldset>

  <fieldset>
    <legend>jsnlog_speciallogger</legend>
      <input type="button" id="jsnlog_speciallogger_btnStartTimer" value="start timer" /><p />
      <input type="button" id="jsnlog_speciallogger_btnEndTimer" value="end timer and log time, with level INFO" /><p />
      <input type="button" id="jsnlog_speciallogger_btnAssertFalse" value="assert 0==1, with level ERROR" /><p />
      <input type="button" id="jsnlog_speciallogger_btnAssertTrue" value="assert 1==1 (causes no action)" />
  </fieldset>

    </div>
    </form>
</body>
</html>
