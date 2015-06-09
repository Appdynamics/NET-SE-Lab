<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LabSite.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>.NET Self Lab Site</title>
</head>
<body>
    <form id="form1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
       <Services>
            <asp:ServiceReference Path="/Ajax/AjaxLabService.svc" />
       </Services> 
    </asp:ScriptManager>

    <script type="text/javascript">
        function TryValue() {
            var value = document.getElementById('<%= TextBoxSearch.ClientID %>').value;

            AjaxLabService.CheckData(value, OnSuccess);
        }

        function OnSuccess(message) {
            alert(message);
        }

    </script>

    <div style="position: absolute; z-index:100000; background:lightgrey; padding: 20px; top: 30px; left: 30px; border: 3px solid blue">
            <asp:TextBox ID="TextBoxSearch" runat="server" Width="300px"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Seach" />
            <input type="button" value="Try Ajax" onclick="TryValue()" />
            <br />
            <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">Search Statistics</asp:LinkButton>
            <br />
            <asp:LinkButton ID="LinkButton2" runat="server" onclick="LinkButton2_Click">Log Statistics</asp:LinkButton>
            <br />
            <asp:Button ID="Button3" runat="server" onclick="Button3_Click" 
                Text="Clear Search Cache" />
        </div>
    </form>
</body>
</html>