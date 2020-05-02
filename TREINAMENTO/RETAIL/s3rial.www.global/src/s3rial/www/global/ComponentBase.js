sap.ui.define([
	"sap/ui/core/UIComponent"
], function(UIComponent) {
	"use strict";

	return UIComponent.extend("s3rial.www.global.ComponentBase", {

		metadata: {
			manifest: "json"
		},

		/**
		 * The component is initialized by UI5 automatically during the startup of the app and calls the init method once.
		 * @public
		 * @override
		 */
		init: function() {
			// call the base component's init function
			UIComponent.prototype.init.apply(this, arguments);

			// create the views based on the url/hash
			this.getRouter().initialize();
		},

		getMenuItems: function() {
			return [
				{key: "menu_1", title: "MENU 01"},
				{key: "menu_2", title: "MENU 02"},
				{key: "menu_3", title: "MENU 03"},
				{key: "menu_4", title: "MENU 04"}
			];
		},

		getMainComponent: function() {
			/*
			Name of main component is defined in componentData of componentUsages
			in main component manifest. For example ...

			"componentUsages": {
				"myreuse": {
					"name": "reuse.component1",
					"componentData": {
						"parentComponentName": "mydemo.Component"
					}
				}

			*/
			let oElement = this.oContainer
			while (oElement && !this._mainComponent) {
				oElement = oElement.getParent();
				if (this.oComponentData && this.oComponentData.parentComponentName)
				{
					if (oElement.getMetadata().getName() === this.oComponentData.parentComponentName) {
						this._mainComponent = oElement;
					}
				}
			}
			return this._mainComponent ? this._mainComponent : this;
		},

		getMainRouter: function() {
			return this.getMainComponent().getRouter()
		},

		loadedFromApp: function () {
			if (this.oComponentData && this.oComponentData.parentComponentName) {
				return true;
			}
			else
			{
				return false;
			}
		}
	});
});