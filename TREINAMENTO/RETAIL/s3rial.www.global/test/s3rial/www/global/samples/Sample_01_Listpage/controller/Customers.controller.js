sap.ui.define([
	's3rial/www/global/ListPageBase'
], function(ListPageBase) {
	"use strict";

	const THIS_DOMAIN = "s3rial/www/global/samples/Sample_01_Listpage";

	return ListPageBase.extend(`${THIS_DOMAIN}.controller.Customers`, {
		configurationFile: function() {
			//return `${THIS_DOMAIN}/view/Customers.view.json`;
		},

		apiName: function() {
			return "product";
		},

		onInit: function () {
			ListPageBase.prototype.onInit.call(this);
		},

		pageTitle: function() {
			return "Nota Fiscal Entrada";
		},

		pageFilter: function() {
			return {
				"tamanhoDoFlex": "190px",
				"Campos": [
					/*
					{
						"tipo": "input",
						"texto": "PN",
						"masc": "text",
						"bdName": "entityName",
						"icon": "sap-icon://add-document"
					},
					{
						"tipo": "input",
						"texto": "Empresa",
						"masc": "text",
						"bdName": "entityName",
						"icon": "sap-icon://add-document"
					},
					{
						"tipo": "input",
						"texto": "Tipo",
						"masc": "text",
						"bdName": "invoiceDirection",
						"icon": "select box, Emissão, Lançamento, vencimento"
					},
					{
						"tipo": "input",
						"texto": "Filial",
						"masc": "text",
						"bdName": "invoiceId",
						"icon": "sap-icon://decline"
					},
					{
						"tipo": "input",
						"texto": "Titulo",
						"masc": "text",
						"bdName": "invoiceSeries",
						"icon": "sap-icon://decline"
					},
					{
						"tipo": "input",
						"texto": "Status",
						"masc": "text",
						"bdName": "invoiceSeries",
						"icon": "select box, não integrados, com erro"
					},
					{
						"tipo": "input",
						"texto": "Data de Integração",
						"masc": "date",
						"bdName": "invoiceSeries",
						"icon": "select box, não integrados, com erro"
					},
					{
						"tipo": "input",
						"texto": "Data de Emissão",
						"masc": "date",
						"bdName": "invoiceSeries",
						"icon": "select box, não integrados, com erro"
					},
					{
						"tipo": "input",
						"texto": "Data de Recepção",
						"masc": "date",
						"bdName": "invoiceSeries",
						"icon": "select box, não integrados, com erro"
					},
					{
						"tipo": "input",
						"texto": "Data de Vencimento",
						"masc": "date",
						"bdName": "invoiceSeries",
						"icon": "select box, não integrados, com erro"
					},
					{
						"tipo": "button",
						"texto": "Ir",
						"masc": "text",
						"bdName": "recId",
						"icon": "sap-icon://decline",
						"click": "filtrar"
					}
					*/
				]
			};
		},

		tableConfig: function() {
			return {
				"ColunaPK": "2",
				"Titulo": "Titulo da Tabela",
				"Tipo": "Grid",
				"Tabs": [
				],
				"colunasTabela": [
					{
						"tipo": "Label",
						"texto": "Nome completo",
						"masc": "Text",
						"bdName": "fullName"
					},
					{
						"tipo": "Label",
						"texto": "Nome reduzido",
						"masc": "Text",
						"bdName": "shortName"
					},
					{
						"tipo": "Label",
						"texto": "Preço",
						"masc": "Number",
						"bdName": "price"
					},
					{
						"tipo": "Label",
						"texto": "Data Preço",
						"masc": "DateTime",
						"bdName": "priceLastUpdate"
					},
					{
						"tipo": "Label",
						"texto": "Quantidade",
						"masc": "Quantity",
						"bdName": "quantity"
					}
				]
			}			
		},

	});
});