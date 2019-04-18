<%@ Page Title="MD_Base" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="md_base.aspx.cs" Inherits="mdHttpHandler.md_base" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <p>Current Time: <%=DateTime.Now.ToLongTimeString() %></p>
    <p>FilePath: <%=Request.FilePath %></p>

    <%=ProcessMarkdownFile(Server.MapPath(Request.FilePath)) %>
</asp:Content>
