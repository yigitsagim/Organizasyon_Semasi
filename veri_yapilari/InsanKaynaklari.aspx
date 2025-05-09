<%@ Page Title="İnsan Kaynakları Departmanı Organizasyon Şeması" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="InsanKaynaklari.aspx.cs" Inherits="veri_yapilari.InsanKaynaklari" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <%-- CSS ve JS aynı kalabilir --%>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <h2 style="text-align:center;">İnsan Kaynakları Departmanı Organizasyon Şeması</h2>

    <div class="zoom-buttons">
        <button onclick="scale += 0.1; updateZoom()">➕ Yakınlaştır</button>
        <button onclick="scale -= 0.1; updateZoom()">➖ Uzaklaştır</button>
    </div>

    <asp:UpdatePanel ID="upIK" runat="server">
        <ContentTemplate>
            <div class="tree-container">
                <div class="tree-zoom">
                    <ul class="tree">
                        <asp:Literal ID="litIKSema" runat="server" />
                    </ul>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
