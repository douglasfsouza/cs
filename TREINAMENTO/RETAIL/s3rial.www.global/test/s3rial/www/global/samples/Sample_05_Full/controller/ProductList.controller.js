sap.ui.define([
	's3rial/www/global/ListPageBase',
	's3rial/www/global/samples/Sample_05_Full/SampleEnumerators',
], function(ListPageBase) {
	"use strict";

	const THIS_DOMAIN = "s3rial/www/global/samples/Sample_05_Full";

	return ListPageBase.extend(`${THIS_DOMAIN}.controller.ProductList`, {
		configurationFile: function() {
			//return `${THIS_DOMAIN}/view/Product.view.json`;
		},

		targetName: function() {
			return "productList";
		},

		apiName: function() {
			return "ImportProduct";
		},

		onInit: function () {
			ListPageBase.prototype.onInit.call(this);

			//this.setListMethod("listaProd");
		},

		pageTitle: function() {
			return "Cadastro de Produtos";
		},

		pageFilter: function() {
			return {
				"tamanhoDoFlex": "190px",
				"Campos": [
					{
						"tipo": "input",
						"texto": "Data de início",
						"masc": "date",
						"bdName": "data",
						"operator": "startswith"
					},
					{
						"tipo": "input",
						"texto": "Nome completo",
						"masc": "text",
						"bdName": "descricao1",
						"icon": "sap-icon://add-document"
					},
					{
						"tipo": "selectlist",
						"masc": "branch",
						"texto": "Filial",
						"bdName": "BPLId",
						"apiName": "businessPlaces",
						"listMethod": "getSelectList"
					},
					{
						"tipo": "selectlist",
						"texto": "Tipo Entidade",
						"bdName": "tipoEntidade",
						"masc": "enum",
						"enumName": "tipoEntidade",
						"enumSource": "s3rial.www.global.samples.Sample_05_Full.SampleEnumerators"
					},
					{
						"tipo": "combobox",
						"texto": "Tipo Entidade",
						"bdName": "tipoEntidadeCombo",
						"masc": "enum",
						"enumName": "tipoEntidade",
						"enumSource": "s3rial.www.global.samples.Sample_05_Full.SampleEnumerators",
					},
					{
						"tipo": "input",
						"texto": "Nome completo",
						"masc": "text",
						"bdName": "descricao4",
						"icon": "sap-icon://add-document"
					}
				]
			};
		},
	
		onTableToolbarClicked: function(button) {
			if (button === "add")
			{
				var oRouter = this.getRouter();
				oRouter.getTargets().display("productEdit", 
					{
						fromTarget: "productList",
						editMode: "insert",
						recordKey: null
					}
				);
			}
		},

		tableConfig: function() {
			return {
				"ColunaPK": "2",
				"Titulo": "Titulo da Tabela",
				"Tipo": "Grid",
				"Tabs": [
				],
				Buttons: [
					{id: "add", text: "Novo" }
				],
				"colunasTabela": [
					{
						"tipo": "Label",
						"texto": "Código",
						"masc": "Integer",
						"bdName": "cod_item",
						"largura": 10
					},
					{
						"tipo": "Label",
						"texto": "Nome completo",
						"masc": "Text",
						"bdName": "descricao",
						"largura": 60
					},
					{
						"tipo": "Label",
						"texto": "Nome reduzido",
						"masc": "Text",
						"bdName": "desc_reduz",
						"largura": 30
					},
					{
						"tipo": "Label",
						"texto": "Status",
						"bdName": "status",
						"masc": "enum",
						"enumName": "integrationStatus",
						"enumSource": "s3rial.www.global.samples.Sample_05_Full.SampleEnumerators",
					}
				]
			}			
		},

		attachDisplay: function(oEvent) {
			ListPageBase.prototype.attachDisplay.call(this, oEvent);
		},

		onRowNavigation: function(currentRow) {
			var oRouter = this.getRouter();
			oRouter.getTargets().display("productEdit", 
				{
					fromTarget: "productList",
					editMode: "none",
					recordKey: currentRow.recId
				}
			);
		}

	});
});