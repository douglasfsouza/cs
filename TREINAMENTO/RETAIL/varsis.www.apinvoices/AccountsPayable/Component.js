sap.ui.define([
	"s3rial/www/global/ComponentBase"
], function(ComponentBase) {
	"use strict";

	return ComponentBase.extend("varsis.www.apinvoices.Component", {
		metadata: {
			manifest: "json"
		},

		getMenuItems: function() {
			return [
				{key: "catalog", title: "Administrativo",
					children: [
						{key: "accountsToPayableList", title: "Contas a Pagar"},
						{key: "accountsToIncomingList", title: "Contas a Receber"}
					]
				}];
		}
	});
});

