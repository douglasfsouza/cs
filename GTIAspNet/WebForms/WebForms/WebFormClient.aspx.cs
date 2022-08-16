using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms.ClientServiceReference;
using WebForms.Models;

namespace WebForms
{
    public partial class WebFormClient : System.Web.UI.Page
    {
        ClientServiceReference.ClientServiceClient clientService = new ClientServiceReference.ClientServiceClient();
        ClientServiceReference.Cliente cliente = new ClientServiceReference.Cliente();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("myCookie", "1234");
            cookie.Expires = DateTime.Now.AddMinutes(120);
            Response.Cookies.Add(cookie);
            DataSet ds = new DataSet();
            ds = clientService.GetClienteRecords();
            grdClients.DataSource = ds;
            grdClients.DataBind();

            if (ddlUFExpedicao.Items.Count == 0)
            {
                ddlEstadoCivil.Items.Clear();
                ddlEstadoCivil.Items.Add("Casado");
                ddlEstadoCivil.Items.Add("Solteiro");

                ddlSexo.Items.Clear();
                ddlSexo.Items.Add("Masculino");
                ddlSexo.Items.Add("Feminino");

                CarregarUFs();                
            }                      
        }

        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCPF.Text))
            {
                lblMsgAtualizar.Text = "CPF não informado";
                return;
            }

            if (!Helpers.Helpers.IsCpf(txtCPF.Text.Replace("-", null).Replace(".", null)))
            {
                lblMsgAtualizar.Text = "CPF inválido";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                lblMsgAtualizar.Text = "Nome não informado";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEnderecoCEP.Text))
            {
                lblMsgAtualizar.Text = "CEP do Endereço não informado";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEnderecoNumero.Text))
            {
                lblMsgAtualizar.Text = "Número do Endereço não informado";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEnderecoBairro.Text))
            {
                lblMsgAtualizar.Text = "Bairro do Endereço não informado";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEnderecoCidade.Text))
            {
                lblMsgAtualizar.Text = "Cidade do Endereço não informado";
                return;
            }

            try
            {
                if (Convert.ToDateTime(txtDataNascimento.Value) > DateTime.Today)
                {
                    lblMsgAtualizar.Text = "Data de Nascimento inválida";
                    return;
                }
            }
            catch
            {
                lblMsgAtualizar.Text = "Data de Nascimento inválida";
                return;
            }

            DateTime? dataExpedicao = null;

            if (!string.IsNullOrEmpty(txtDataExpedicao.Value))
            {
                try
                {
                    if (Convert.ToDateTime(txtDataExpedicao.Value) > DateTime.Today)
                    {
                        lblMsgAtualizar.Text = "Data de Expedição inválida";
                        return;
                    }
                    dataExpedicao = Convert.ToDateTime(txtDataExpedicao.Value);
                }
                catch
                {
                    lblMsgAtualizar.Text = "Data de Expedição inválida";
                    return;
                }
            }

            Cliente cliente = new Cliente()
            {
                CPF = txtCPF.Text,
                Nome = txtNome.Text,
                RG = txtRG.Text,
                DataExpedicao = dataExpedicao,
                OrgaoExpedicao = txtOrgaoExpedicao.Text,
                DataNascimento = Convert.ToDateTime(txtDataNascimento.Value),
                Sexo = ddlSexo.Text,
                EstadoCivil = ddlEstadoCivil.Text,
                EnderecoCEP = txtEnderecoCEP.Text,
                EnderecoLogradouro = txtEnderecoRua.Text,
                EnderecoBairro = txtEnderecoBairro.Text,
                EnderecoCidade = txtEnderecoCidade.Text,
                EnderecoComplemento = txtEnderecoComplemento.Text,
                EnderecoNumero = txtEnderecoNumero.Text,
                EnderecoUF = ddlEnderecoUF.Text,
                UF = ddlUFExpedicao.Text       
            };
            
            if (string.IsNullOrEmpty(txtId.Text))
            {
                String result = clientService.Insert(cliente);

                lblMsgAtualizar.Text = "Dados incluidos com sucesso";
            }
            else
            {
                cliente.Id = Convert.ToInt32(txtId.Text);
                string result = clientService.Update(cliente);
                lblMsgAtualizar.Text = "Dados alterados com sucesso";
            }
            LimparCampos();
            AtivarDesativar(false);
            txtId.Text = string.Empty;
            txtId.Enabled = true;
            hlpFindId.Enabled = true;
            btnIncluir.Enabled = true;
            btnAlterar.Enabled = true;
            btnExcluir.Enabled = true;             
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                GetCliente cliente = new GetCliente();
                cliente = clientService.GetInfo();

                DataTable data = new DataTable();
                data = cliente.ClienteTable;
                grdClients.DataSource = data;
                grdClients.DataBind();
            }
            catch (Exception ex)
            {
                lblMsgAtualizar.Text = "Erro ao pesquisar. Erro: " + ex.Message;                 
            } 
        }                

        protected void hlpFindId_Click(object sender, ImageClickEventArgs e)
        {
            lblMsgSearch.Text = string.Empty;
            if (string.IsNullOrEmpty(txtId.Text))
            {
                lblMsgSearch.Text = "Id não informado";
                return;
            }

            DataSet ds = new DataSet();
            cliente.Id = Convert.ToInt32(txtId.Text);
            ds = new DataSet();
            ds = clientService.SearchClient(cliente);
            if (ds.Tables[0].Rows.Count > 0)
            {
                txtCPF.Text = ds.Tables[0].Rows[0]["CPF"].ToString();
                txtNome.Text = ds.Tables[0].Rows[0]["Nome"].ToString();

                txtRG.Text = ds.Tables[0].Rows[0]["RG"].ToString();

                if (ds.Tables[0].Rows[0]["DataExpedicao"] == DBNull.Value)
                {
                    txtDataExpedicao.Value = string.Empty;
                }
                else
                {
                    txtDataExpedicao.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["DataExpedicao"]).ToString("yyyy-MM-dd");
                }

                txtOrgaoExpedicao.Text = ds.Tables[0].Rows[0]["OrgaoExpedicao"].ToString();
                txtDataNascimento.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["DataNascimento"]).ToString("yyyy-MM-dd");
                ddlSexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                ddlEstadoCivil.Text = ds.Tables[0].Rows[0]["EstadoCivil"].ToString();
                txtEnderecoCEP.Text = ds.Tables[0].Rows[0]["EnderecoCEP"].ToString();
                txtEnderecoRua.Text = ds.Tables[0].Rows[0]["EnderecoLogradouro"].ToString();
                txtEnderecoBairro.Text = ds.Tables[0].Rows[0]["EnderecoBairro"].ToString();
                txtEnderecoCidade.Text = ds.Tables[0].Rows[0]["EnderecoCidade"].ToString();
                txtEnderecoComplemento.Text = ds.Tables[0].Rows[0]["EnderecoComplemento"].ToString();
                txtEnderecoNumero.Text = ds.Tables[0].Rows[0]["EnderecoNumero"].ToString();
                ddlEnderecoUF.Text = ds.Tables[0].Rows[0]["EnderecoUF"].ToString();
                ddlUFExpedicao.Text = ds.Tables[0].Rows[0]["UF"].ToString();
            }
            else
            {
                lblMsgSearch.Text = "Id não encontrado";
            }
        }

        private void AtivarDesativar(bool opc)
        {
            txtCPF.Enabled = opc;
            txtNome.Enabled = opc;
            txtRG.Enabled = opc;
            txtDataExpedicao.Disabled = !opc;
            txtOrgaoExpedicao.Enabled = opc;
            txtDataNascimento.Disabled = !opc;
            ddlSexo.Enabled = opc;
            ddlEstadoCivil.Enabled = opc;
            txtEnderecoCEP.Enabled = opc;
            txtEnderecoRua.Enabled = opc;
            txtEnderecoBairro.Enabled = opc;
            txtEnderecoCidade.Enabled = opc;
            txtEnderecoComplemento.Enabled = opc;
            txtEnderecoNumero.Enabled = opc;
            ddlEnderecoUF.Enabled = opc;
            ddlUFExpedicao.Enabled = opc;
            btnCancelar.Enabled = opc;
            btnAtualizar.Enabled = opc;
            blnSearchCEP.Enabled = opc;
        }

        private void LimparCampos()
        {            
            txtCPF.Text = string.Empty;
            txtNome.Text = string.Empty;
            txtRG.Text = string.Empty;
            txtOrgaoExpedicao.Text = string.Empty;
            txtEnderecoCEP.Text = string.Empty;
            txtEnderecoRua.Text = string.Empty;
            txtEnderecoBairro.Text = string.Empty;
            txtEnderecoCidade.Text = string.Empty;
            txtEnderecoComplemento.Text = string.Empty;
            txtEnderecoNumero.Text = string.Empty;            
        }

        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            AtivarDesativar(true);
            LimparCampos();
            lblMsgAtualizar.Text = string.Empty;
            lblMsgSearch.Text = string.Empty;
            txtId.Text = "";
            txtId.Enabled = false;
            hlpFindId.Enabled = false;
            btnIncluir.Enabled = false;
            btnAlterar.Enabled = false;
            btnExcluir.Enabled = false;
            txtCPF.Focus();

        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCPF.Text))
            {
                lblMsgSearch.Text = "Cliente não informado";
                return;
            }

            AtivarDesativar(true);
            txtId.Enabled = false;
            hlpFindId.Enabled = false;
            btnIncluir.Enabled = false;
            btnAlterar.Enabled = false;
            btnExcluir.Enabled = false;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            AtivarDesativar(false);
            LimparCampos();
            lblMsgAtualizar.Text = string.Empty;
            lblMsgSearch.Text = string.Empty;
            txtId.Text = string.Empty;
            txtId.Enabled = true;
            hlpFindId.Enabled = true;
            btnIncluir.Enabled = true;
            btnAlterar.Enabled = true;
            btnExcluir.Enabled = true;
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCPF.Text))
            {
                lblMsgAtualizar.Text = "Cliente não informado";
                lblMsgSearch.Text = "Cliente não informado";
                return;
            }

            string result = clientService.Delete(Convert.ToInt32(txtId.Text));
            lblMsgSearch.Text = "Cliente excluído com sucesso";
            lblMsgAtualizar.Text = "Cliente excluído com sucesso";
            AtivarDesativar(false);
            LimparCampos();
            txtId.Text = string.Empty;
            txtId.Enabled = true;
            hlpFindId.Enabled = true;
        }
        private List<string> getUF()
        {
            return new List<string>()
            {
                "AC", "AL", "AP", "AM", "BA",
                "CE", "DF", "ES", "GO", "MA",
                "MT", "MS", "MG", "PA", "PB",
                "PR", "PE", "PI", "RJ", "RN",
                "RS", "RO", "RR", "SC", "SP",
                "SE", "TO"
            };
        }

        private void CarregarUFs()
        {
            List<string> ufs = getUF();
            ddlUFExpedicao.Items.Clear();
            ddlUFExpedicao.Items.Clear();
            foreach (var uf in ufs)
            {
                ddlUFExpedicao.Items.Add(uf);
                ddlEnderecoUF.Items.Add(uf);
            }
        }

        protected void blnSearchCEP_Click(object sender, ImageClickEventArgs e)
        {
            lblMsgAtualizar.Text = string.Empty;
            try
            {
                var result = clientService.GetCep(txtEnderecoCEP.Text);
                if (string.IsNullOrEmpty(result))
                {
                    lblMsgAtualizar.Text = "Falha ao consultar o CEP";
                }
                else
                {
                    CEP cep = JsonConvert.DeserializeObject<CEP>(result);
                    ddlEnderecoUF.Text = cep.uf;
                    txtEnderecoCidade.Text = cep.localidade;
                    txtEnderecoBairro.Text = cep.bairro;
                    txtEnderecoRua.Text = cep.logradouro;
                    txtEnderecoComplemento.Text = cep.complemento;
                }
            }
            catch  
            {
                lblMsgAtualizar.Text = "CEP não localizado";
            }                   
        }       
       
    }
}