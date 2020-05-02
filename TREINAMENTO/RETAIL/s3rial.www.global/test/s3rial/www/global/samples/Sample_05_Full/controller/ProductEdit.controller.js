sap.ui.define([
	's3rial/www/global/EditPageBase',
	"sap/m/MessageToast",
	"sap/ui/model/json/JSONModel",
	's3rial/www/global/DatasourceBase',
], function(EditPageBase, MessageToast, JSONModel, Datasource) {
	"use strict";

	const THIS_DOMAIN = "s3rial/www/global/samples/Sample_05_Full";

	return EditPageBase.extend(`${THIS_DOMAIN}.controller.ProductEdit`, {
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
			return `${THIS_DOMAIN}/view/ProductEdit.view.json`;
		},

		targetName: function() {
			return "productEdit";
		},

		canEditRecord: function() {
			return true;
		},

		onInit: function () {
			EditPageBase.prototype.onInit.call(this);

			this._datasource = new Datasource("ImportProduct");
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
						this.showMessage("Produto", "Registro gravado com sucesso !");
					}
					else
					{
						this.showError("Gravar produto", response.message);
					}
					this.getView().setBusy(false);
					return true;
				},
				(error) => {
					this.getView().setBusy(false);
					this.showError("Gravar produto", error.message);
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
						var data = response.data;

						data.zipCode = "06654140";

						data.listForn = [
							{"codigo": "00001", "nome": "Fornecedor 0001"},
							{"codigo": "00002", "nome": "Fornecedor 0002"},
							{"codigo": "00003", "nome": "Fornecedor 0003"}
						];

						data.dataRms7 = "1130925";

						var oModel = new JSONModel(data);
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