sap.ui.define([
	"s3rial/www/global/ComponentBase"
], function(ComponentBase) {
	"use strict";

	return ComponentBase.extend("s3rial.www.global.samples.Sample_05_Full.Component", {
		metadata: {
			manifest: "json"
		},

		init: function() {
			// call the base component's init function
			ComponentBase.prototype.init.apply(this, arguments);

			// create the views based on the url/hash
			//this.getRouter().initialize();
		},

		getMenuItems: function() {
			return [
				{key: "catalog", title: "Cadastros",
					children: [
						{key: "productList", title: "Produtos"}
					]
				},
				{key: "transaction", title: "Transações"}
			];
		}
	});
});