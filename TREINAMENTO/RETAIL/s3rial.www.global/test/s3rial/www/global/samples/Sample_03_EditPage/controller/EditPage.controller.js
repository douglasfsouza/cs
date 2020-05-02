sap.ui.define([
	's3rial/www/global/EditPageBase',
	"sap/m/MessageToast",
	"sap/ui/model/json/JSONModel"
], function(EditPageBase, MessageToast, JSONModel) {
	"use strict";

	const THIS_DOMAIN = "s3rial/www/global/samples/Sample_03_EditPage";

	return EditPageBase.extend(`${THIS_DOMAIN}.controller.EditPage`, {
		configurationFile: function() {
			return `${THIS_DOMAIN}/view/EditPage.view.json`;
		},

		onInit: function () {
			EditPageBase.prototype.onInit.call(this);

			var oModel = new JSONModel({
				editing: false
			});

			var oRecord = new JSONModel({
				entityName: "NOME DA ENTIDADE",
				invoiceDate: 1130924,
			});

			this.getView().setModel(oModel);
			this.getView().setModel(oRecord, "record");
		},

		showMessage: function(message) {
			MessageToast.show(message, {duration: 2000});
		}

	});
});