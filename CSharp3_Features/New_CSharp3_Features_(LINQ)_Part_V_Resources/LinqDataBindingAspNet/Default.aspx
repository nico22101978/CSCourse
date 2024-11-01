<%@ Page Title="DataBinding with LINQ" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="LinqDataBindingAspNet._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="Panel1" runat="server">
        <span class="subjectCaption">With LINQ to Objects: Oneway Databinding:</span><asp:GridView 
            ID="gridViewLinqToObjects" runat="server" />
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <span class="subjectCaption">With LINQ to Entities: Twoway Databinding, and you can add objects:</span><asp:GridView 
            ID="gridViewLinqToEntities" runat="server" DataSourceID="EntityDataSource1" 
            onrowupdated="gridViewLinqToEntities_RowUpdated">
            <Columns>
                <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
            </Columns>
        </asp:GridView>
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" 
            DataSourceID="EntityDataSource1" DefaultMode="Insert" Height="50px" 
            Width="125px">
            <Fields>
                <asp:BoundField DataField="Age" HeaderText="Age" ReadOnly="True" 
                    SortExpression="Age" />
                <asp:BoundField DataField="Company" HeaderText="Company" ReadOnly="True" 
                    SortExpression="Company" />
                <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" 
                    SortExpression="Name" />
                <asp:BoundField DataField="State" HeaderText="State" ReadOnly="True" 
                    SortExpression="State" />
                <asp:CommandField ShowInsertButton="True" />
            </Fields>
        </asp:DetailsView>
        <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=ExampleDataEntities"
        DefaultContainerName="ExampleDataEntities" EnableDelete="True" EnableFlattening="False"
        EnableInsert="True" EnableUpdate="True" EntitySetName="Persons" 
        EntityTypeFilter="Person">
        </asp:EntityDataSource>
        <asp:Label ID="lastErrorMessage" runat="server" CssClass="inlineErrorMessage"></asp:Label>
    </asp:Panel>
</asp:Content>
