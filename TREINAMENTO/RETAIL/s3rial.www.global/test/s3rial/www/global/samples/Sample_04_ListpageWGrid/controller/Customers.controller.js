sap.ui.define([
	's3rial/www/global/ListPageBase'
], function(ListPageBase) {
	"use strict";

	const THIS_DOMAIN = "s3rial/www/global/samples/Sample_04_ListpageWGrid";

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
						"textoColuna": "Empresa",
						"campoJson": "entityName",
						"masc": "text"
					},
					{
						"textoColuna": "Código PN",
						"campoJson": "codigO_PN",
						"masc": "text"
					},
					{
						"textoColuna": "Descrição PN",
						"campoJson": "descricaO_PN",
						"masc": "text"
					},
					{
						"textoColuna": "Numero da Nota",
						"campoJson": "numerO_NOTA",
						"masc": "text"
					},
					{
						"textoColuna": "Série",
						"campoJson": "serie",
						"masc": "text"
					},
					{
						"textoColuna": "Valor",
						"campoJson": "valor",
						"masc": "text"
					},
					{
						"textoColuna": "Data de Lançamento",
						"campoJson": "datA_LANCAMENTO",
						"masc": "text"
					},
					{
						"textoColuna": "Emissão da Nota",
						"campoJson": "emissaO_NOTA",
						"masc": "text"
					},
					{
						"textoColuna": "Código Filial RMS",
						"campoJson": "codigO_FILIAL_RMS",
						"masc": "text"
					},
					{
						"textoColuna": "Descrição Filial RMS",
						"campoJson": "descricaO_FILIAL_RMS",
						"masc": "text"
					},
					{
						"textoColuna": "Data da Integração",
						"campoJson": "datA_INTEGRACAO",
						"masc": "text"
					},
					{
						"textoColuna": "Status",
						"campoJson": "status",
						"masc": "text"
					}
				]
			}			
		},

	});
});