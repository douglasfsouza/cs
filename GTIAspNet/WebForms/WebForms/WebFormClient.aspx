<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebFormClient.aspx.cs" Inherits="WebForms.WebFormClient" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style>
    .linha{
        background-color: aqua;
        padding: 2px;
        display: flex;
        flex-direction: row;
        width: 1200px;
    }
    .campo {
        display: flex;
        flex-direction: column;
        padding: 5px;
    }
    .campoCEP {
        display: flex;
        flex-direction: column;
        padding: 5px 0 5px 5px;
    }
     
    #txtNome{
        width:990px;
    }
    .txtEnderecoCEP{
        width: 100px;
    }
    .txtNumero {
        width: 75px;
    }

    #txtEnderecoCidade{
        width: 140px;
    }

    #txtEnderecoRua{
        width: 300px;
    }

    #txtEnderecoBairro{
        width: 220px;
    }

    .ddlEnderecoUF {
        width: 50px;
        height: 20px;
    }

    #txtDataNascimento {
        width: 150px;
    }

    #txtOrgaoExpedicao{
        width: 130px;
    }

    #ddlUFExpedicao {
        width: 100px;
        height: 20px;
    }

    #ddlSexo {
        width: 100px;
        height: 20px;
    }

    .ddlEstadoCivil {
        width: 120px;
        height: 20px;
    }

    .campoHlp {
        padding: 23px 5px 5px 0;
        height: 30px;
        border:none;
    }   

    .btnHlp{
        background-color: DodgerBlue;  
        border: none;  
        color: white;  
        padding: 0;  
        font-size: 15px; 
        padding: 2px;
        cursor: pointer; 
        height:17px;
    }

    .btnHlp:hover {
        background-color: RoyalBlue;
    }

    .btn {
        background-color: #00ff90;
        border: solid 1px;
        border-radius: 5px;
        border-color: seagreen;
        color: blue;
        padding: 4px;
        font-family: Verdana, Geneva;
        font-size: 15px;        
        cursor: pointer;
        text-decoration: none;
    }
    .btn:hover {
        background-color: #b6ff00;
    }

    .actions {
        width: 155px;
        height: 35px;
        align-items:center;
        justify-content: space-between;
        display: flex;
    }

    .actionsSearch {
        width: 189px;
        height: 35px;
        margin-top: 20px;
        align-items:center;
        justify-content: space-between;
        display: flex;
    }

    .msgSearchContainer{
        display: flex;
        justify-content:center;
        align-content: center;
        vertical-align: middle;
        align-items:center;       
        background-color: mediumaquamarine;
        width: 780px;
        margin-left: 10px;
        margin-top: 22px;
        height: 28px;
        border-radius: 4px;
    }

    .msgAtualizarContainer{
        display: flex;
        justify-content:center;
        align-content: center;
        vertical-align: middle;
        align-items:center;       
        background-color: mediumaquamarine;
        width: 1025px;
        height: 30px;
        padding-left: 10px;
        padding-right: 10px;
        border-radius: 4px;
    }

    .btnActionsAddContainer{        
        display: flex;
        justify-content: space-between;
        align-content: space-between;
        padding: 20px;
    }

    .atualizar{        
        display: flex;
        flex-direction: row;
        justify-content:space-between;
        align-content:space-between;
        vertical-align: middle;
        align-items:center;       
        height: 40px;
        width: 1205px;
    }
</style>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Cadastro de Clientes</title>
</head>
<body style="height: 483px">
    <form id="form1" runat="server">        
        <div>
            <h1>Cliente</h1>
        </div>
        
        <div class="linha">
            <div class="campo">
                <asp:Label ID="Label1" runat="server" Text="Id*"></asp:Label>
                <asp:TextBox ID="txtId" runat="server" style="margin-top: 0px"></asp:TextBox>
            </div>   
            
            <div class="campoHlp">
                <asp:ImageButton ID="hlpFindId" CssClass="btnHlp" ImageUrl="~/search.jpg" runat="server" OnClick="hlpFindId_Click" />
            </div>  
            
            <div class="actionsSearch">
                <asp:Button ID="btnIncluir" CssClass="btn" runat="server" Text="Incluir" OnClick="btnIncluir_Click"/> 
                <asp:Button ID="btnAlterar" CssClass="btn" runat="server" Text="Alterar" OnClick="btnAlterar_Click"/> 
                <asp:Button ID="btnExcluir" CssClass="btn" runat="server" Text="Excluir" OnClick="btnExcluir_Click"/> 
            </div>
            <div class="msgSearchContainer">
                <asp:Label ID="lblMsgSearch" runat="server" Text=""></asp:Label>
            </div>          

        </div>
        <div class="linha">
            <div class="campo">
                <asp:Label ID="Label3" runat="server" Text="CPF*" ></asp:Label>
                <asp:TextBox ID="txtCPF" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campo">
                <asp:Label ID="Label4" runat="server" Text="Nome*"></asp:Label>
                <asp:TextBox ID="txtNome" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>
        </div>        

        <div class="linha">             
            <div class="campo">
                <asp:Label ID="Label5" runat="server" Text="RG"></asp:Label>
                <asp:TextBox ID="txtRG" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>            

             <div class="campo">
                <asp:Label ID="Label6" runat="server" Text="Data Expedição"></asp:Label>
                <input runat="server" type="date" id="txtDataExpedicao" disabled/>
            </div>

            <div class="campo">
                <asp:Label ID="Label7" runat="server" Text="Orgão Expedição"></asp:Label>
                <asp:TextBox ID="txtOrgaoExpedicao" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campo">
                <asp:Label ID="Label8" runat="server" Text="UF Expedição"></asp:Label>
                <asp:DropDownList ID="ddlUFExpedicao" runat="server" style="margin-top: 0px" Enabled="false"></asp:DropDownList>
            </div>

            <div class="campo">
                <asp:Label ID="Label9" runat="server" Text="Data Nascimento*"></asp:Label>
                <input runat="server" id="txtDataNascimento" type="date" disabled />
            </div>

            <div class="campo">
                <asp:Label ID="Label10" runat="server" Text="Sexo*"></asp:Label>
                <asp:DropDownList ID="ddlSexo" runat="server" style="margin-top: 0px" Enabled="false"></asp:DropDownList>
            </div>

             <div class="campo">
                <asp:Label ID="Label11" runat="server" Text="Estado Civil*"></asp:Label>
                <asp:DropDownList ID="ddlEstadoCivil" CssClass="ddlEstadoCivil" runat="server" style="margin-top: 0px" Enabled="false"></asp:DropDownList>
            </div>
        </div>

        <div class="linha">
            
        </div>

        <div>
            <h1>Endereço</h1>
        </div>

        <div class="linha">
            <div class="campo">
                <asp:Label ID="Label12" runat="server" Text="CEP*"></asp:Label>
                <asp:TextBox ID="txtEnderecoCEP" CssClass="txtEnderecoCEP" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campoHlp">
                <asp:ImageButton ID="blnSearchCEP" CssClass="btnHlp" ImageUrl="~/search.jpg" runat="server" OnClick="blnSearchCEP_Click" />
            </div>

            <div class="campo">
                <asp:Label ID="Label13" runat="server" Text="Rua*"></asp:Label>
                <asp:TextBox id="txtEnderecoRua" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campo">
                <asp:Label ID="Label14" runat="server" Text="Número*"></asp:Label>
                <asp:TextBox id="txtEnderecoNumero" CssClass="txtNumero" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campo">
                <asp:Label ID="Label15" runat="server" Text="Complemento"></asp:Label>
                <asp:TextBox id="txtEnderecoComplemento" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campo">
                <asp:Label ID="Label16" runat="server" Text="Bairro*"></asp:Label>
                <asp:TextBox id="txtEnderecoBairro" CssClass="txtEnderecoBairro" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campo">
                <asp:Label ID="Label17" runat="server" Text="Cidade*"></asp:Label>
                <asp:TextBox id="txtEnderecoCidade" runat="server" style="margin-top: 0px" Enabled="false"></asp:TextBox>
            </div>

            <div class="campo">
                <asp:Label ID="Label18" runat="server" Text="UF*"></asp:Label>
                <asp:DropDownList ID="ddlEnderecoUF" CssClass="ddlEnderecoUF" runat="server" Style="margin-top: 0px" Enabled="false"></asp:DropDownList>
            </div>
        </div>

        <div class="atualizar">
            <div class="actions">
                <asp:Button CssClass="btn" ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                <asp:Button CssClass="btn" ID="btnAtualizar" OnClick="btnAtualizar_Click" runat="server" Text="Atualizar" />
            </div> 

            <div class="msgAtualizarContainer" >
                <asp:Label ID="lblMsgAtualizar" Text="Mensagem" runat="server"></asp:Label>
            </div>           
        </div>

        <div>
            <h1>
                Lista de Clientes
            </h1>

            <asp:GridView ID="grdClients" runat="server" AllowSorting="True">
            </asp:GridView>
        </div>
    </form>
    
</body>
</html>
