sap.ui.define([
	"s3rial/www/global/ComponentBase"
], function(ComponentBase) {
	"use strict";

	return ComponentBase.extend("s3rial.www.global.samples.Sample_03_EditPage.Component", {
		metadata: {
			manifest: "json"
		},

		getMenuItems: function() {
			return [
				{key: "menu_5", title: "MENU 05",
					children: [
						{key: "sample", title: "MENU 05/01"},
						{key: "menu_5_2", title: "MENU 05/02"},
						{key: "menu_5_3", title: "MENU 05/03"}
					]
				},
				{key: "menu_6", title: "MENU 06"},
				{key: "menu_7", title: "MENU 07"},
				{key: "menu_8", title: "MENU 08"}
			];
		}
	});
});