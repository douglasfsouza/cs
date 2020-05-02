sap.ui.define([
	's3rial/www/global/ListPageBase',
	"sap/m/MessageToast"
], function(ListPageBase, MessageToast) {
	"use strict";

	const THIS_DOMAIN = "s3rial/www/global/samples/Sample_02_MasterPage";

	return ListPageBase.extend(`${THIS_DOMAIN}.controller.Customers`, {
		configurationFile: function() {
			return `${THIS_DOMAIN}/view/Customers.view.json`;
		},

		_selectedRows: [],

		onInit: function () {
			ListPageBase.prototype.onInit.call(this);

			this.setTableTitle(`${this.getTableTitle()} - ALTERADA`);
			this.changeButtonsStatus();

			var rows = [];
			for(var i = 0; i < 10; i++)
			{
				rows.push({
					id: i
				});
			}

			this.setData(rows);
		},

		onRowChanged: function(currentRow){
			var row = JSON.stringify(currentRow);
			this.showMessage(`O usuário navegou para a linha "${row}"`);
		},

		onSelectionChanged: function(selectedRows) {
			this.showMessage(`O usuário selecionou "${selectedRows.length}" linhas da tabela`);

			this._selectedRows = selectedRows;
			this.changeButtonsStatus();
		},

		onTableToolbarClicked: function(buttonId) {
			this.showMessage(`O usuário clicou no botão "${buttonId}"`);
		},

		cancelFilter: function(filter) {
			console.log(filter);
			this.showMessage(`O usuário disparou o filtro`);
		},

		changeButtonsStatus() {
			var canEdit = this._selectedRows.length === 1;
			var canDelete = this._selectedRows.length > 0;
			var canAdd = this._selectedRows.length === 0;

			this.changeTableToolbar([
				{id: "add", enabled: canAdd},
				{id: "edit", enabled: canEdit},
				{id: "delete", enabled: canDelete}
			]);
		},

		showMessage: function(message) {
			MessageToast.show(message, {duration: 2000});
		}

	});
});