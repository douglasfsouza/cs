sap.ui.define([
	's3rial/www/global/ListPageBase',
	'sap/ui/model/json/JSONModel',
	'varsis/www/apinvoices/Enumerators',
], function(ListPageBase, JSONModel) {
	"use strict";

	const THIS_DOMAIN = "varsis/www/apinvoices";

	return ListPageBase.extend(`${THIS_DOMAIN}.controller.Installments`, {
		configurationFile: function() {
			return `${THIS_DOMAIN}/view/Installments.view.json`;
		},

		pageTitle: function() {
			return "Títulos a Pagar";
		},

		targetName: function() {
			return "accountsToPayableList";
		},

		apiName: function() {
			return "APInvoice"
		},
		_addTableDefaultButtons: function() {
			var toolbar = this.byId(this.TABLE_TOOLBAR_NAME);

			var btnExport = new sap.m.Button({
				tooltip: "Exportar",
				icon: "sap-icon://excel-attachment",
				press: [this.onTableRefreshPress, this]
			});

			btnExport.addStyleClass("sapUiSmallMarginEnd");
			toolbar.insertContent(btnExport, 99);

			var btnGroup = new sap.m.Button({
				tooltip: "Agrupar",
				icon: "sap-icon://group-2",
				press: [this.onTableRefreshPress, this]
			});

			btnGroup.addStyleClass("sapUiSmallMarginEnd");
			toolbar.insertContent(btnGroup, 99);
	  			
			var btnSetup = new sap.m.Button({
				tooltip: "Configurar",
				icon: "sap-icon://action-settings",
				press: [this.onTableRefreshPress, this]
			});

			btnSetup.addStyleClass("sapUiSmallMarginEnd");
			toolbar.insertContent(btnSetup, 99);

			var btnTchart = new sap.m.Button({
				tooltip: "Grafico",
				icon: "sap-icon://table-chart",
				press: [this.onTableRefreshPress, this]
			});

			btnTchart.addStyleClass("sapUiNoMargin");
			toolbar.insertContent(btnTchart, 99);
			
			var btnBar = new sap.m.Button({
				tooltip: "Grafico",
				icon: "sap-icon://bar-chart",
				press: [this.onTableRefreshPress, this]
			});

			btnBar.addStyleClass("sapUiNoMargin");
			toolbar.insertContent(btnBar, 99); 
		},
		onInit: function () {
			ListPageBase.prototype.onInit.call(this);

			var user = this._readUser();
			var criterias = [];

			// Força filtro pela aba selecionada
			criterias.push(
				{
					"Field": "slice",
					"Operator": "eq",
					"Value": "open"
				}
			);			

			var oCriteriasModel = new JSONModel({items: criterias});
			this.getView().setModel(oCriteriasModel, "criterias");

			this.setListMethod("listSummary");
		},

		onTableToolbarClicked: function (button) {
			switch(button)			
			{
				case "advance":
					break;
				default:
					break;				
			}
		},

		attachDisplay: function(oEvent) {
			ListPageBase.prototype.attachDisplay.call(this, oEvent);
		},

		onTabBarClicked: function (args) {
			var oCriteriasModel = this.getView().getModel("criterias");
			var oCriteriasData = oCriteriasModel.getData();

			var newCriterias = {items: []};

			oCriteriasData.items.forEach(c => {
				if (c.Field === "slice")
				{
					c.Value = args.key;
				}

				newCriterias.items.push(c);
			});

			oCriteriasModel.setData(oCriteriasData);

			this.getView().setModel(oCriteriasModel, "criterias");

			this._loadData();
		},

		onRowNavigation: function(currentRow) {
			var oRouter = this.getRouter();
			oRouter.getTargets().display("installmentEdit",
			{
				fromTarget: this.targetName(),
				editMode: "none",
				record: currentRow
			});
		},

		_readUser: function() {
			var store = window.localStorage.getItem("s3_logged_user");
			var user = JSON.parse(store);
			return user;
		}
	});
});