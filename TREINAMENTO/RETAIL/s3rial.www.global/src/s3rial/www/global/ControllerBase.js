/*!
 * ${copyright}
 */
sap.ui.define([
	"sap/ui/core/mvc/Controller",
	"sap/m/MessageToast",
    "s3rial/www/global/Mask",
    "s3rial/www/global/Formatting",
    "s3rial/www/global/GlobalFunctions",
    "sap/ui/model/json/JSONModel",
    "sap/m/MessageBox",
    "s3rial/www/global/types/Boolean",
    "s3rial/www/global/types/Currency",
    "s3rial/www/global/types/CpfCnpj",
    "s3rial/www/global/types/ZipCode",
    "s3rial/www/global/types/Percentage"
], function(Controller, MessageToast, Mask, Formatting, Global, JSONModel, MessageBox) {
	"use strict";

	return Controller.extend("s3rial.www.global.ControllerBase", {
        metadata: {
            library: 's3rial.www.global',
            publicMethods: ['getJsonItemsValue', 'addTituloNaPagina', 'addDadosTabela', 'addColunasDaTabela',
            'addTabs', 'addTabsToCards', 'addTable', 'addTableFilterColunas', 'addFieldsDetail', 
            'addFields',
            'setLocalStorage', 'getLocalStorage', 'getUrlParam', 'requestToServer',
            'TransformJsonToForm', 'TransformFormToJson', 'ConfsToView',
            'GetMock', 'ConfigAPI', 'GetMenu', 'GetConfPage', 'NavigateBetweenSystems', 'checkCookies',
            'createInputEl', 'createTextEl' ],
        },

        TABLE_RESPONSIVE: "Responsive",
        TABLE_GRID: "Grid",

        _pageParameters: null,

        /** 
         * Retorna o arquivo que contém a configuração geral da tela
         * 
         * Se não for implementado, deve-se implementar os métodos pageTitle, pageFilter e tableConfig
         */
        configurationFile: function() {
            console.log("Method Not Implemented");
        },

		/**
		 * Retorna o nome da api para comunicação com o backend
		 */
		apiName: function() {
			console.log("Method Not Implemented");
		},
            
		getRouter : function () {
            return this.getOwnerComponent().getRouter();
        },
        
        getPageParameters: function () {
            return this._pageParameters;
        },

		targetName: function() {
			console.log("Method Not Implemented");
		},

		showMessage: function(title, message, buttons) {
            MessageBox.information(message, {
                title: title,
				onClose: function (sAction) {
                    if (sAction === MessageBox.Action.OK)
                    {
                        me._remove(record);
                    }
				}
			});
		},

		showError: function(title, message, buttons) {
            MessageBox.error(message, {
                title: title,
				onClose: function (sAction) {
                    if (sAction === MessageBox.Action.OK)
                    {
                        me._remove(record);
                    }
				}
			});
		},

		showAlert: function(title, message, buttons) {
            MessageBox.alert(message, {
                title: title,
				onClose: function (sAction) {
                    if (sAction === MessageBox.Action.OK)
                    {
                        me._remove(record);
                    }
				}
			});
		},

		onInit: function () {
			var oRouter = this.getRouter();
            var oTarget = oRouter.getTarget(this.targetName());
            if (oTarget)
            {
                oTarget.attachDisplay(this.attachDisplay, this);
            }
        },

        onBeforeRendering: function () {
            // PLACEHOLDER
        },

		attachDisplay: function(oEvent) {
			this._pageParameters = oEvent.getParameter("data");
        },
        
		createTable: function (id, config) {
			var component = null;

			switch(config.Tipo)
			{
				case "Grid":
					component = this._createGrid(id, config);
					break;
				case "Responsive":
					component = this._createResponsive(id, config);
					break;
				default:
					component = this._createResponsive(id, config);
					break;
            }
            
            if (config.Tabs && config.Tabs.length !== 0)
            {
                component.addStyleClass("sapUiSmallMarginBegin sapUiSmallMarginEnd sapUiNoMarginBottom");

                var tabbar = this._createTabBar("tabbar", config);
                tabbar.addContent(component);
                component = tabbar;
            }

			return component;
		},

		_createResponsive: function(id, config) {
			var component =  new sap.m.Table(this.createId(id), {
				includeItemInSelection: false,
				popinLayout: "GridLarge",
				sticky: ["ColumnHeaders", "HeaderToolbar"],
				mode: "MultiSelect",
				growing: "false",
				growingThreshold: 10
			});

            var columnListItem =  new sap.m.ColumnListItem({
                type: "Navigation"
            });

            if (config.toolbarName)
            {
                var toolbar = this._createTableToolbar(config.toolbarName, config);
                component.setHeaderToolbar(toolbar);
            }

            component.itemPress = config.onItemPress;
            component.selectionChange = config.onSelectionChange;

			config.colunasTabela.forEach((column) => {
                var newId = column.bdName.replace(/\//g,".");
                component.addColumn(this._createTableColumn(id, column));	
                columnListItem.addCell(this.createLabelEl(`col_${newId}`, column));
			});

            component.bindItems("/", columnListItem);

			return component;
		},
		
		_createGrid: function (id, config) {
            var cfgComponent = {
				selectionMode: "MultiToggle",
				selectionBehavior: "Row",
				enableCellFilter: true,
				visibleRowCountMode: "Auto",
            };
            if (config.onSelectionChange)
            {
                cfgComponent.rowSelectionChange = config.onSelectionChange;
            }
			var component = new sap.ui.table.Table(this.createId(id), cfgComponent);

            var cfgRowAction = {
                type: "Navigation",
                visible: true
            };
            if (config.onRowNavigation)
            {
                cfgRowAction.press = config.onRowNavigation;
            }
			var template = new sap.ui.table.RowAction({items: [
				new sap.ui.table.RowActionItem(cfgRowAction)
			]});

			config.colunasTabela.forEach((column) => {
				component.addColumn(this._createGridColumn(id, column));
			});

            if (config.toolbarName)
            {
                var toolbar = this._createTableToolbar(config.toolbarName, config);
                component.addExtension(toolbar);
            }
            
			component.setRowActionTemplate(template);
			component.setRowActionCount(1);

			return component;
		},

		_createGridColumn: function (parent, config) {
            var textAlign = this._getColumnAlignment(config.masc);
            var newId = config.bdName.replace(/\//g,".");

            var columnId = `${parent}-${newId}`;

			var column = new sap.ui.table.Column(this.createId(columnId), {
                sortProperty: config.bdName,
                filterProperty: config.bdName,
                hAlign: textAlign
            });

            if (config.largura)
            {
                var largura = this._ajustWidth(config.largura);
                column.setWidth(`${largura}em`);
            }

			var label = this.createLabelEl(`${columnId}-lbl${newId}`, config);

			column.setLabel(new sap.m.Label({text: config.texto }));
			column.setTemplate(label);
			
			return column;
        },

        _ajustWidth: function(width) {
            return width * (13/16);
        },
        
        _getColumnAlignment: function (masc) {
            var align = "Left";

            switch(masc.toLowerCase())
            {
                case "number":
                case "currency":
                case "percent":
                case "quantity":
                case "integer":
                case "integerNoGroup":
                    align = "Right";
                    break;
                case "date":
                case "datetime":
                case "rms7":
                    align = "Center";
                    break;
            }

            return align;
        },

        _createTableColumn: function (config) {
            var textAlign = this._getColumnAlignment(config.masc);

			var column = new sap.m.Column({
                hAlign: textAlign,
                header: new sap.m.Label({
                    text: config.texto
                })                
            });

            if (config.largura)
            {
                column.setWidth(`${config.largura}em`);
            }
		
			return column;
		},

		_createTableToolbar: function(id, config) {
			var component = new sap.m.OverflowToolbar(this.createId(id), {design: "Auto"});
			
			component.addStyleClass("sapMTBHeader-CTX");

			component.insertContent(new sap.m.Title({level: "H2", text: config.tableTitle}), 0);
			component.insertContent(new sap.m.ToolbarSpacer(), 1);

            if (config.Buttons && config.Buttons.length > 0)
            {
                config.Buttons.forEach((cfg) => {
                    var cfgButton = {
                        text: cfg.text,
                        type: "Transparent",
                        
                    };
                    if(config.onToolbarClick)
                    {
                        cfgButton.press = config.onToolbarClick;
                    }
    
                    var button = new sap.m.Button(cfg.id, cfgButton);
                    button.addStyleClass("sapUiSmallMarginEnd");
                    component.insertContent(button, 99);
                });
            }

			return component;
        },
        
		_createTabBar: function(id, config) {
			var component = new sap.m.IconTabBar(this.createId(id), {
				headerMode: "Inline",
				tabDensityMode: "Compact",
				stretchContentHeight: true,
                select: config.onTabbarClick,
                expandable: false
            });

            component.addStyleClass("sapUiResponsiveContentPadding sapUiNoMarginBottom");
            
            config.Tabs.forEach((cfg) => {
                var tab = new sap.m.IconTabFilter({
                    key: cfg.key,
                    text: cfg.text
                });
                component.addItem(tab);
            })

			return component;
        },
        
        createFormat: function(config, modelName = 'record>/') {
            var formatContent = "";
            var path = config.path || "";

            switch (config.masc.toLowerCase())
            {
                case "cnpj":
                case "cpf":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \   
                                      "type": "s3rial.www.global.types.CpfCnpj"}`;

                    //formatContent = {path: `${modelName}${path}${config.bdName}`, formatter: Formatting.formatCPFCNPJ};
                    break;

                case "cep":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \   
                                      "type": "s3rial.www.global.types.ZipCode"}`;

                    //formatContent = {path: `${modelName}${path}${config.bdName}`, formatter: Formatting.formatZipCodeBR};
                    break;
                
                case "rms7":
                    formatContent = {path: `${modelName}${path}${config.bdName}`, formatter: Formatting.formatRMS7};
                    break;

                case "percent":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \   
                                      "type": "s3rial.www.global.types.Percentage"}`;
                    break;

                case "enum":
                    formatContent = {parts: [{path: `${modelName}${path}${config.bdName}`},
                                             {value: config.enumSource},
                                             {value: config.enumName}
                                            ], 
                                     formatter: Formatting.formatEnum};
                    break;

                case "integer":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \   
                                      "type": "sap.ui.model.type.Float", \
                                      "formatOptions": { \
                                            "decimals": 0, \
                                            "groupingSeparator": "." \
                                        }}`;
                    break;
                    
                case "integernogroup":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \   
                                      "type": "sap.ui.model.type.Integer"}`;
                    break;

                case "currency":
                case "number":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \ 
                                      "type": "s3rial.www.global.types.Currency"}`;
                    break;

                case "del_number":
                    formatContent = `{path: '${modelName}${path}${config.bdName}', \   
                                        type: 'sap.ui.model.type.Float', \
                                        formatOptions: { \
                                            decimals: 2, \
                                            decimalSeparator: ',', \
                                            groupingSeparator: '.' \
                                        }}`;
                    break;

                case "quantity":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \
                                        "type": "sap.ui.model.type.Float", \
                                        "formatOptions": { \
                                            "decimals": 3, \
                                            "decimalSeparator": ",", \
                                            "groupingSeparator": "." \
                                        }}`;
                    break;
                
                case "date":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \
                                        "type": "sap.ui.model.type.DateTime", \
                                        "formatOptions": { \
                                        "source": {
                                            "pattern": "yyyy-MM-ddTHH:mm:ss"
                                        },
                                        "pattern": "dd/MM/yyyy"
                                        }}`;
                    break;

                case "time":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \
                                        "type": "sap.ui.model.type.DateTime", \
                                        "formatOptions": { \
                                        "source": {
                                            "pattern": "yyyy-MM-ddTHH:mm:ss'"
                                        },
                                        "pattern": "HH:mm:ss"
                                        }}`;
                    break;

                case "datetime":
                    formatContent = `{"path": "${modelName}${path}${config.bdName}", \
                                        "type": "sap.ui.model.type.DateTime", \
                                        "formatOptions": { \
                                        "source": {
                                            "pattern": "yyyy-MM-ddTHH:mm:ss"
                                        },
                                        "pattern": "dd/MM/yyyy HH:mm:ss"
                                        }}`;
                    break;

                case "bool": 
                    formatContent = `{path: '${modelName}${path}${config.bdName}', \
                                        type: 's3rial.www.global.types.Boolean'}`;
                    break;

                default: 
                    formatContent = `{"path": "${modelName}${path}${config.bdName}"}`;
                    break;
            }

            return formatContent;
        },

        /**
         * 
         * @param {mandatory} id 
         * @param {mandatory} config 
         *{
         *  "tipo": "input",           
         *      input, text, button
         *  "texto": "Código",         
         *      texto do componente 
         *  "masc": "Text",            
         *      forma de apresentação dos dados Date, DateTime, Number, Integer, IntegerNoGroup,  CPF, CNPJ
         *  "bdName": "cod_item"
         *},
         * 
         */
        createInputEl: function (id, config) {
            var newId = String(id).replace(/\//g,".");
            var formatContent = this.createFormat(config);
            var type = undefined;

            var path = config.path || "";

            switch (config.masc.toLowerCase())
            {
                case "currency":
                case "number":
                case "percent":
                    break;
                    
                case "integer":
                case "integernogroup":
                    type="Number";
                    break;
                
                default: 
                    //formatContent = "{" + path + config.bdName + "}";
                    break;
            }

            var inputMask = "";
            switch (config.masc.toLowerCase())
            {
                case "phone":
                    inputMask = "(99) 9999-9999";
                    break;

                case "mobile":
                    inputMask = "(99) 99999-9999";
                    break;
            }

            switch (config.masc.toLowerCase())
            {
                case "date":
                case "datetime":
                    var element = new sap.m.DatePicker({
                        id: this.createId(newId),
                        value: formatContent,
                        valueFormat: "yyyy-MM-dd",
                        editable: "{=${layout>/editing} ? true : false}"
                    });
                    break;
                
                case "phone":
                case "mobile":
                    var element = new sap.m.MaskInput({
                        id: this.createId(newId),
                        value: formatContent,
                        editable: "{=${layout>/editing} ? true : false}",
                        mask: inputMask
                    });
                    break;
            
                default:
                    var element = new sap.m.Input({
                        type: type,
                        id: this.createId(newId),
                        value: formatContent,
                        editable: "{=${layout>/editing} ? true : false}"
                    });
                    break;
            }                

            return element;
        },

        createComboBoxEl: function (id, config) {
            var newId = String(id).replace(/\//g,".");
            var path = config.path || "record>/";
            var element = null;
            var format = this.createFormat(config)

            element = new sap.m.Select({
                id: this.createId(newId),
                forceSelection: false,
                selectedKey: format,
                enabled: "{=${layout>/editing} ? true : false}",
                width: "100%"
            });

            element.bindItems("/",  new sap.ui.core.Item({
                    key: '{key}',
                    text: '{value}'
                })
            );

            Global._loadEnum(element, config).then(
                (config) => {
                    var list = config.enumObj.list(config.enumName);
                    var oModel = new JSONModel(list);
                    config.elemento.setModel(oModel);
                }
            );

            return element;
        },

        createSelectListEl: function (id, config) {
            var newId = String(id).replace(/\//g,".");
            var path = config.path || "record>/";
            var element = null;

            switch (config.masc.toLowerCase())
            {
                case "branch":
                    element = new s3rial.www.global.control.SelectListBranch(newId, {
                        value: `{${path}${config.bdName}}`,
                        enabled: "{=${layout>/editing} ? true : false}"
                    });
                    break;
                
                case "enum":
                    element = new s3rial.www.global.control.SelectListBranch(newId, {
                        origin: 'enum',
                        value: `{${path}${config.bdName}}`,
                        enabled: "{=${layout>/editing} ? true : false}",
                        keyField: "key",
                        textField: "value",
                        enumSource: config.enumSource,
                        enumName: config.enumName
                    });
                    break;

                default:
                    element = new s3rial.www.global.control.SelectList({
                        apiName: config.apiName,
                        listMethod: config.listMethod,
                        keyField: config.keyField || "key",
                        textField: config.textField || "value",
                        title: config.texto,
                        value: `{${path}${config.bdName}}`,
                        enabled: "{=${layout>/editing} ? true : false}"
                    });
                    break;
            }

            return element;
        },

        createDisplayEl: function (id, config) {
            var newId = String(id).replace(/\//g,".");
            return this.createTextEl(newId, config, true);
        },

        createTextEl: function (id, config, hideOnEditing = false) {
            var newId = String(id).replace(/\//g,".");
            var formatContent = this.createFormat(config);
            var textAlign = this._getColumnAlignment(config.masc);

            /*
            var element = new sap.m.Text({
                id: this.createId(newId),
                text: formatContent,
                visible: hideOnEditing ? "{=${layout>/editing} ? false : true}" : true,
                textAlign: config.noAlign ? "Left" : textAlign
            });
            */

            var element = new sap.m.Input({
                id: this.createId(newId),
                value: formatContent,
                visible: hideOnEditing ? "{=${layout>/editing} ? false : true}" : true,
                editable: false,
                textAlign: config.noAlign ? "Left" : textAlign
            });

            return element;
        },

        createSwitchEl: function (id, config, hideOnEditing = false) {
            var newId = String(id).replace(/\//g,".");
            var formatContent = this.createFormat(config);

            var element = new sap.m.Switch({
                id: this.createId(newId),
                type: "Default",
                state: formatContent, 
                customTextOff: "Não",
                customTextOn: "Sim",

                enabled: "{=${layout>/editing} ? true : false}"
            });

            return element;
        },

        createLabelEl: function (id, config) {
            var newId = String(id).replace(/\//g,".");
            var formatContent = this.createFormat(config, "");
            var textAlign = this._getColumnAlignment(config.masc);

            var element = new sap.m.Label({
                id: this.createId(newId),
                text: formatContent,
                textAlign: textAlign
            });

			var layoutData = new sap.m.FlexItemData({
				growFactor: 1
            });
            
            element.setLayoutData(layoutData);

            return element;
        }

    });	
	
}, true);