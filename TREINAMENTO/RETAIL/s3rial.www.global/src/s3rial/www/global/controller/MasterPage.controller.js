sap.ui.define([
	"s3rial/www/global/ControllerBase",
	"sap/ui/model/json/JSONModel",
	"sap/ui/core/Fragment",
	"sap/ui/core/routing/History"
], function(ControllerBase, JSONModel, Fragment, History) {
	"use strict";

	return ControllerBase.extend("s3rial.www.global.controller.MasterPage", {
		_parameters: null,
		_initialized: false,

		onInit: function () {
			var loadedFromApp = this.getOwnerComponent().loadedFromApp();
			var config = this._readAPIConfig();

			this._loadFragment(loadedFromApp);

			var logoName = sap.ui.require.toUrl(config.logo.headerPage || "s3rial/www/global/image/sap-business-one-logo-2.png");
			var logoSize = config.logo.headerPageSize || "24px";

			var model = new JSONModel({
				logo: logoName,
				logoSize: logoSize
			})

			this.getView().setModel(model);

			if (!loadedFromApp)
			{
				var navigation = this.getView().byId("navigationList");
				var menuItems = this.getOwnerComponent().getMenuItems();
				this._loadMenu(menuItems, navigation);
			}

			this._initialized = true;
		},

		_loadFragment: function(loadedFromApp) {
			var view = this.getView();
			var fragmentName = "s3rial.www.global.view.MasterPageInternal";
			var me = this;

			if (loadedFromApp)
			{
				fragmentName = "s3rial.www.global.view.MasterPageExternal";
			}

			Fragment.load({
				id: view.getId(),
				name: fragmentName
			}).then(function (oFragment) {
				// connect dialog to the root view of this component (models, lifecycle)
				view.addContent(oFragment);

				if (loadedFromApp) {
					me.configureMasterPageExternal();
				}
				else
				{
					me.configureMasterPageInternal();
				}
			});
		},

		configureMasterPageInternal: function() {
			var navButton = this.byId("sideNavigationToggleButton");
			var usrButton = this.byId("userButtonPress");
			var sideNav   = this.byId("sideNavigation");
			var imageLogo = this.byId("imageLogo");
			
			navButton.attachPress(this.onSideNavButtonPress, this);
			usrButton.attachPress(this.onUserNamePress, this);
			sideNav.attachItemSelect(this.onItemSelect, this);
			imageLogo.attachPress(this.navigateToHome, this);

		},

		configureMasterPageExternal: function () {
			var navButton = this.byId("sideNavigationToggleButton");
			var usrButton = this.byId("userButtonPress");
			var imageLogo = this.byId("imageLogo");
			
			navButton.attachPress(this.onNavBackPress, this);
			usrButton.attachPress(this.onUserNamePress, this);
			imageLogo.attachPress(this.navigateToHome, this);
		},

		onSideNavButtonPress: function() {
			var oToolPage = this.byId("toolPage");
			oToolPage.setSideExpanded(!oToolPage.getSideExpanded());
		},

		onNavBackPress: function() {
			var oHistory = History.getInstance();
			var sPreviousHash = oHistory.getPreviousHash();

			if (sPreviousHash !== undefined) {
				window.history.go(-1);
			} else {
				var oRouter = this.getRouter();
				oRouter.navTo("root", {}, true);
			}
		},	

		navigateToHome: function(oEvent) {
			var oRouter = this.getRouter();
			oRouter.navTo("main", {}, true);
		},

		onUserNamePress: function() {
		},

		onItemSelect: function(oEvent) {
			var menuItem = oEvent.getParameter("item");
			var children = menuItem.getItems();

			if (children && children.length !== 0)
			{
				menuItem.setExpanded(!menuItem.getExpanded());
			}
			else
			{
				var sKey = oEvent.getParameter("item").getKey();
				var router = this.getRouter();
				router.navTo(sKey);
			}
		},

		_loadMenu: function(menuItems, parent) {
			menuItems.forEach((item) => {
				var menuItem = new sap.tnt.NavigationListItem({
					key: item.key,
					text: item.title,
					expanded: false,
					icon: item.icon
				});

				parent.addItem(menuItem);

				if (item.children)
				{
					this._loadMenu(item.children, menuItem)
				}
			})
		},

		_readAPIConfig: function () {
            var request = new XMLHttpRequest();
            var url = "/apiconfig.json";
            request.open('GET', url, false);
            request.send(null);
            return JSON.parse(request.response);
        }

	});
});