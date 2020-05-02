sap.ui.define([
	's3rial/www/global/EditPageBase',
	"sap/m/MessageToast",
	"sap/ui/model/json/JSONModel",
	's3rial/www/global/DatasourceBase'
], function(EditPageBase, MessageToast, JSONModel, Datasource) {
	"use strict";

	const THIS_DOMAIN = "varsis/www/apinvoices";

	return EditPageBase.extend(`${THIS_DOMAIN}.controller.accountsToPayableEdit`, {
		_datasource: null,
		_modified: false,
		_emptyRecord: {
			id: "",
			fullName: "",
			shortName: "",
			price: 0.0,
			quantity: 0.0
		},

		configurationFile: function() {
			return `${THIS_DOMAIN}/view/accountsToPayableEdit.view.json`;
		},

		targetName: function() {
			return "accountsToPayableEdit";
		},
		canEditRecord: function() {
			return false;
		},

		onInit: function () {
			EditPageBase.prototype.onInit.call(this);

			this._datasource = new Datasource("cadentidade");
		},

		attachDisplay: function(oEvent) {
			EditPageBase.prototype.attachDisplay.call(this, oEvent);

			var oModel = new JSONModel(this._emptyRecord);
			this.getView().setModel(oModel, "record");

			var parameters = this.getPageParameters();

			if (parameters.editMode !==  "insert")
			{
				this._loadRecord(parameters.recordKey);
			}
		},

		onSavePress: function (oEvent) {
			this._saveRecord().then(
				result => {
					if (result)
					{
						this._modified = true;
						EditPageBase.prototype.onSavePress.call(this, oEvent);
					}
				}
			)
		},

		onNavBackPress: function (oEvent) {
			var parameters = this.getPageParameters();	

			if (this._modified && parameters && parameters.fromTarget) {

				if (parameters.editMode === "insert")
				{
					this.getRouter().getTargets().display(parameters.fromTarget, { recordKey: "***" });
				}
				else
				{
					this.getRouter().getTargets().display(parameters.fromTarget, { recordKey: parameters.recordKey });
				}
			}
			else
			{
				EditPageBase.prototype.onNavBackPress.call(this, oEvent);
			}
		},

		_saveRecord: function() {
			this.getView().setBusy(true);
		
			var parameters = this.getPageParameters();
			var insertMode = parameters.editMode === "insert";
			var oModel = this.getView().getModel("record");
			var record = oModel.getData();
			var recordKey = parameters.recordKey;
			var method = null;

			return (insertMode ? this._datasource.insert(record) 
			                   : this._datasource.update(recordKey, record)).then(
				(response) => {
					if(response.success)
					{
						this.showMessage("Fornecedor", "Registro gravado com sucesso !");
					}
					else
					{
						this.showError("Gravar fornecedor", response.message);
					}
					this.getView().setBusy(false);
					return true;
				},
				(error) => {
					this.getView().setBusy(false);
					this.showError("Gravar fornecedor", error.message);
					return false;
				}
			)
		},
		
		_loadRecord: function(recordKey) {
			this.getView().setBusy(true);

			this._datasource.get(recordKey).then(
				(response) => {
					if(response.success)
					{
						var oModel = new JSONModel(response.data);
						this.getView().setModel(oModel, "record");
					}
					else
					{
						this.showError("Obter dados do servidor", response.message);
					}
					this.getView().setBusy(false);
				},
				(error) => {
					this.getView().setBusy(false);
					this.showError("Obter dados do servidor", error.message);
				}
			)
		}
	});
});