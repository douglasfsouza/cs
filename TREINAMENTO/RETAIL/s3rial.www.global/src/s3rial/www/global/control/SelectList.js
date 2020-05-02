sap.ui.define([
	"sap/ui/core/XMLComposite",
	"sap/ui/model/json/JSONModel",
    'sap/ui/model/Filter',
], function (XMLComposite, JSONModel, Filter) {
	"use strict";

    const ORIGIN_API = "api";
    const ORIGIN_ENUM = "enum";

    return XMLComposite.extend("s3rial.www.global.control.SelectList", {
        _inputControlId: "_inputSelect",
        _isDataLoaded: false,
        
		metadata: {
			properties: {
                origin: {type: "string", defaultValue: "api"},  // api || enum 
				apiName: { type : "string"},
                listMethod: { type : "string", defaultValue : "query"},
                keyField: { type: "string", defaultValue: "key"},
                textField: { type: "string", defaultValue: "text"},
                title: { type: "string", defaultValue : "lista" },
                value: { type: "any", defaultValue: ""},
                enumSource: {type: "string"},
                enumName: {type: "string"},
                enabled: {type: "boolean", defaultValue: true}
            },
            interfaces: ["sap.ui.core.IFormContent"],
			events: {
				itemSelected : {
					parameters : {
						item : {type : "object"}
					}
				}
			}
        },

        constructor: function(sId, mSettings) {
            XMLComposite.apply(this, arguments)
        },

        getFormDoNotAdjustWidth: function() {
            return false;
        },

        fixListItems: function() {
            var key = this.getProperty("keyField");
            var text = this.getProperty("textField");

            if (this._itemsFixed)
            {
                return;
            }

            this._Input.bindAggregation("suggestionItems", "/items", (sId, oInfo) => {
                return new sap.ui.core.ListItem({
                    key: `{${key}}`,
                    text: `{${text}}`,
                    additionalText: `{${key}}`
                });
            });

            this._createDialog();

            this._itemsFixed = true;
        },

        onBeforeRendering: function () {
            XMLComposite.prototype.onBeforeRendering.call(this);
            this._Input = this.byId("_inputSelect");
            this._Input.setEnabled(this.getEnabled());
            this.fixListItems();
        },

		onAfterRendering: function() {
            XMLComposite.prototype.onAfterRendering.call(this);
			this.refreshControl();
        },

        setEnabled: function(value)
        {
            this.setProperty("enabled", value);
            if (this._Input)
            {
                this._Input.setEnabled(value);
            }
        },

        setValue: function(value)
        {
            this.setProperty("value", value);
            if (this._Input)
            {
                this._Input.setSelectedKey(value);
            }
        },

		refreshControl: function () {
            var origin = this.getProperty("origin");

            if (!this._isDataLoaded)
            {
                if (origin === ORIGIN_API)
                {
                    this._loadFromApi();
                }
                else if (origin === ORIGIN_ENUM)
                {
                    this._loadFromEnum();
                }
            }
        },
        
        _loadFromApi: function() {
            var apiName = this.getProperty("apiName");
            var listMethod = this.getProperty("listMethod");
            var me = this;

			if (apiName && listMethod)
			{
                this._Input.setBusy(true);
                
                var ds = new s3rial.www.global.DatasourceBase(apiName);
                ds.query(undefined, listMethod).then(
                    (response) => {
                        me._Input.setBusy(false);
                        if(response.success)
                        {
                            var oModel = new JSONModel({items: response.data});
                            oModel.setSizeLimit(10000);
                            me.setModel(oModel);
                            me._Input.setSelectedKey(this.getValue());
                            me._isDataLoaded = true;
                        }
                        else
                        {
                            //this.showError("Obter dados do servidor", response.message);
                        }
                    },
                    (error) => {
                        me._Input.setBusy(false);
                        //this.showError("Obter dados do servidor", error.message);
                    }
                );
            }
        },

        _loadFromEnum: function () {
            var enumSource = this.getProperty("enumSource");
            var enumName = this.getProperty("enumName");
            var me = this;

            if (enumSource && enumName)
            {
                this._Input.setBusy(true);

                this._loadEnum(enumSource).then(
                    (enumObj) => {
                        this._Input.setBusy(false);
                        var list = enumObj.list(enumName);
                        var oModel = new JSONModel({items: list});
                        me.setModel(oModel);
                        me._Input.setSelectedKey(this.getValue());
                        me._isDataLoaded = true;
                    },

                    (error) => {
                        this._Input.setBusy(false);
                    }
                )
            }
        },

        _loadEnum: function(enumSource) {
            return new Promise((resolve, error) => {
                var enumClass = enumSource.split(".").join("/");
                var enumType = sap.ui.require(enumClass);

                if (!enumType)
                {
                    error("***");
                    return;
                }

                var enumObj = new enumType();
                if (!enumObj)
                {
                    error("***");
                    return;
                }

                resolve(enumObj);
            });
        },

		_createDialog: function () {
			// eslint-disable-next-line consistent-this
            //var me = this;

            var key = this.getProperty("keyField");
            var text = this.getProperty("textField");
            var title = this.getProperty("title");

            var dialog = new sap.m.SelectDialog({
                title: title,
                contentWidth: "70vw",
                contentHeight: "70vh",
                search: [this.handleValueHelpSearch, this],
                confirm: [this.handleValueHelpClose, this],
                cancel: [this.handleValueHelpClose, this]
            });

            this._Dialog = dialog;
            this._Dialog.bindAggregation("items", "/items",  (sId, oContext) => {
                return new sap.m.StandardListItem({
                    title: `{${text}}`,
                    description: `Código: {${key}}`
                });
            });
            this.addDependent(dialog);
		},

		handleValueHelp : function (oEvent) {
			var sInputValue = oEvent.getSource().getValue();

			// create a filter for the binding
			this._Dialog.getBinding("items").filter([new Filter(
				this.getProperty("textField"),
				sap.ui.model.FilterOperator.Contains, sInputValue
			)]);
			
			// open value help dialog filtered by the input value
			this._Dialog.open(sInputValue);
		},

		handleValueHelpSearch : function (evt) {
			var sValue = evt.getParameter("value");
			var oFilter = new Filter(
				this.getProperty("textField"),
				sap.ui.model.FilterOperator.Contains, sValue
			);
			evt.getSource().getBinding("items").filter([oFilter]);
		},

		handleValueHelpClose : function (evt) {
			var oSelectedItem = evt.getParameter("selectedItem");
            var key = this.getProperty("keyField");
            var text = this.getProperty("textField");

			this.setValue("");

			if (oSelectedItem) {
                var context = oSelectedItem.getBindingContext();
                var obj = context.getObject(context.getPath());

                this._Input.setSelectedKey(obj[key]);
                this.setValue(obj[key]);

			    this.fireEvent("change", {value: obj[text]});
                this.fireItemSelected({key: obj[key], text: obj[text]});
			}
			evt.getSource().getBinding("items").filter([]);
        },
        
		suggestionItemSelected: function (evt) {
			var oItem = evt.getParameter('selectedItem'),
				sKey = oItem ? oItem.getKey() : '';

            this._Input.setSelectedKey(sKey);
            this.setValue(sKey);
            
            this.fireEvent("change", {value: sKey});
        },

	});

});