sap.ui.define([
	's3rial/www/global/ListPageBase',
	'varsis/www/apinvoices/Enumerators'
], function(ListPageBase) {

	"use strict";
	const THIS_DOMAIN = "varsis/www/apinvoices";
	return ListPageBase.extend(`${THIS_DOMAIN}.controller.accountsToPayableList`, {
		configurationFile: function() {
			//return `${THIS_DOMAIN}/view/Product.view.json`;
		},

		targetName: function() {
			return "accountsToPayableList";
		},

		apiName: function() {
			return "MockTeste";
		},

		onInit: function () {
			ListPageBase.prototype.onInit.call(this);
			this.setListMethod("GetInvoices");
		},

		pageTitle: function() {
			return "Total em Aberto";
		},

		pageFilter: function() {
			return {
				"tamanhoDoFlex": "190px",
				"Campos": [
					{
						"tipo": "input",
						"texto": "Parceiro de Negócio",
						"masc": "text",
						"bdName": "parceiroDeNegocio",
						"icon": "sap-icon://add-document"
					},
					{
						"tipo": "input",
						"texto": "Data",
						"masc": "date",
						"bdName": "data_inclusao",
						"icon": "sap-icon://add-document",
						"operator": "eq"
					},
				   	{
						"tipo": "input",
						"texto": "Inicio",
						"masc": "date",
						"bdName": "data_inclusao",
						"icon": "sap-icon://add-document",
						"operator": "eq"
					},
				   	{
					   "tipo": "input",
					   "texto": "Fim",
					   "masc": "date",
					   "bdName": "data_inclusao",
					   "icon": "sap-icon://add-document",
					   "operator": "eq"
				   },
				   {
					   "tipo": "input",
					   "texto": "Empresa",
					   "masc": "text",
					   "bdName": "empresa",
					   "icon": "sap-icon://add-document",
					   "operator": "eq"
				   },
				   {
					   "tipo": "input",
					   "texto": "Filial",
					   "masc": "text",
					   "bdName": "filial",
					   "icon": "sap-icon://add-document",
					   "operator": "eq"
				   },
				   {
					   "tipo": "input",
					   "texto": "Conta",
					   "masc": "text",
					   "bdName": "conta",
					   "icon": "sap-icon://add-document",
					   "operator": "eq"
				   },
				   {
					   "tipo": "input",
					   "texto": "Titulo",
					   "masc": "text",
					   "bdName": "tituloT",
					   "icon": "sap-icon://add-document",
					   "operator": "startswith"
				   }
				]
			};
		},
	
		onTableToolbarClicked: function(button) {
			if (button === "add")
			{
				var oRouter = this.getRouter();
				oRouter.getTargets().display("providersEdit", 
					{
						fromTarget: "accountsToPayableList",
						editMode: "insert",
						recordKey: null
					}
				);
			}
			if(button === "edit"){
				
			}
		},

		tableConfig: function() {
			return {
				"ColunaPK": "2",
				"Titulo": "",
				"Tipo": "Grid",
				"Tabs": [
					{
						"key": "1",
						"text": "Em Aberto",
						"campoFiltrado": "status",
						"tipoFiltro": "eq",
						"valorFiltro": "emAberto"
					},
					{
						"key": "2",
						"text": "Agendados",
						"campoFiltrado": "status",
						"tipoFiltro": "eq",
						"valorFiltro": "agendados"
					},
					{
						"key": "3",
						"text": "Liquidados",
						"campoFiltrado": "status",
						"tipoFiltro": "eq",
						"valorFiltro": "liquidados"
					},
					{
						"key": "4",
						"text": "Todos",
						"campoFiltrado": "status",
						"tipoFiltro": "eq",
						"valorFiltro": ""
					}
				],
				Buttons: [
					{id: "Detalhe", text: "Detalhe" },
					{id: "Validar", text: "Validar" },
					{id: "Reogarnizar", text: "Reogarnizar" },
					{id: "Agendar", text: "Agendar" }
				],
				"colunasTabela": [
					{
						"tipo": "Label",
						"texto": "Parceiro de Negócio",
						"masc": "Text",
						"bdName": "parceiroDeNegocio"
					},
					{
						"tipo": "Label",
						"texto": "Empresa",
						"masc": "Text",
						"bdName": "empresa"
					},
					{
						"tipo": "Label",
						"texto": "Filial",
						"masc": "Text",
						"bdName": "filial"
					},
					{
						"tipo": "Label",
						"texto": "Conta",
						"masc": "Text",
						"bdName": "conta"
					},
					{
						"tipo": "Label",
						"texto": "Titulo",
						"bdName": "tituloT",
						"masc": "Text"
					},
					{
						"tipo": "input",
						"texto": "Série",
						"masc": "Text",
						"bdName": "serie"
					},
					{
						"tipo": "input",
						"texto": "Vencimento",
						"masc": "Text",
						"bdName": "vencimento"
					},
					{
						"tipo": "input",
						"texto": "Dias",
						"masc": "dateTime",
						"bdName": "dias"
					},
					{
						"tipo": "input",
						"texto": "Val",
						"masc": "Text",
						"bdName": "val"
					},
					{
						"tipo": "input",
						"texto": "Forma de Pagamento",
						"masc": "Text",
						"bdName": "forma"
					},
					{
						"tipo": "input",
						"texto": "Valor Bruto",
						"masc": "Text",
						"bdName": "valorBruto"
					},
					{
						"tipo": "input",
						"texto": "Valor Liquido",
						"masc": "Text",
						"bdName": "valorLiquido"
					},
					{
						"tipo": "input",
						"texto": "Status",
						"masc": "Text",
						"bdName": "status"
					}
				]
			}			
		},
		_onTabbarClicked: function(oEvent)
		{
			
			var index = oEvent.getParameters("key").selectedKey;
			var filtroSelecionado = this._tableTabs.filter(x => x.key == index);
			var campoFiltrado = filtroSelecionado[0].campoFiltrado;
			var operador = filtroSelecionado[0].tipoFiltro;
			var valorFiltro = filtroSelecionado[0].valorFiltro;

			var table = this.byId(this.TABLE_NAME);
			var filtered = this._dataSet;
			var tableConfig = this.tableConfig();

			filtered = this._dataSet.filter((row) => {
				if(valorFiltro != ""){
					switch(operador) {
						case "eq":
							var cols = tableConfig.colunasTabela.filter(x => x.bdName == campoFiltrado);
							var containsToken = cols.some((col) => String(row[col.bdName]) == valorFiltro);
							if (containsToken){return row;}
							break;
						default:
							var cols = tableConfig.colunasTabela.filter(x => x.bdName == campoFiltrado);
							var containsToken = cols.some((col) => String(row[col.bdName]) == valorFiltro);
							if (containsToken){return row;};
					}
				}
				else{
					var cols = tableConfig.colunasTabela.filter(x => x.bdName == campoFiltrado);
					var containsToken = cols.some((col) => String(row[col.bdName]) == String(row[col.bdName]));
					if (containsToken){return row;}
				}
			});
			this.setData(filtered);

			//define titulo da tabela
			var titulo = parseFloat(0.0);
			for (var prop in filtered) {
				var format = filtered[prop].valorLiquido.replace(".","").replace(",", ".");
				titulo += parseFloat(format);
			}

			this.setTableTitle(titulo.toLocaleString('pt-br',{style: 'currency', currency: 'BRL'}));
			if (this._tableConfig.Tipo === this.TABLE_GRID) {
				table.bindRows("/");
			}
		},

		attachDisplay: function(oEvent) {
			ListPageBase.prototype.attachDisplay.call(this, oEvent);
		},

		onRowNavigation: function(currentRow) {
			var oRouter = this.getRouter();
			oRouter.getTargets().display("accountsToPayableEdit", 
				{
					fromTarget: "accountsToPayableList",
					editMode: "none",
					recordKey: currentRow.recId
				}
			);
		}
	});
});