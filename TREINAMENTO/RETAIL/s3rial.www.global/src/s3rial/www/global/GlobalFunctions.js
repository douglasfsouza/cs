/*!
 * ${copyright}
 */

sap.ui.define([
    "sap/m/MessageToast",
    "sap/ui/core/IconPool",
    "sap/ui/model/json/JSONModel",
    "s3rial/www/global/control/Input",
    "s3rial/www/global/control/SelectList",
    "s3rial/www/global/control/SelectListBranch"
],
function(MessageToast, IconPool, JSONModel) {
    "use strict";

    var _EnumSources = {};

    const GlobalFunctions = JSONModel.extend('s3rial.www.global.GlobalFunctions', {
        metadata: {
            library: 's3rial.www.global',
            publicMethods: ['getJsonItemsValue', 'addTituloNaPagina', 'addDadosTabela', 'addColunasDaTabela',
            'addTabs', 'addTabsToCards', 'addTable', 'addTableFilterColunas', 'addFieldsDetail', 
            'addFields',
            'setLocalStorage', 'getLocalStorage', 'getUrlParam', 'requestToServer',
            'TransformJsonToForm', 'TransformFormToJson', 'ConfsToView',
            'GetMock', 'ConfigAPI', 'GetMenu', 'GetConfPage', 'NavigateBetweenSystems', 'checkCookies',
            'createInputEl', 'createTextEl' ],
        }
    });	

    GlobalFunctions.prototype.getJsonItemsValue = function (elementoPai) {
        var newobj = [];
        var oFC = elementoPai;

        var criterias = {
            'criterias': []
        };

        oFC.getItems().forEach(function (flexbox) {
            flexbox.getItems().forEach(function (item) {
                var ref = {};
                var valid = false;
                switch (item.getMetadata().getName()) {
                    case 'sap.m.Input':
                        if (item.getValue() != "") {
                            ref['Field'] = item.getName();
                            ref['Operator'] = "eq";
                            ref['Value'] = item.getValue();
                            valid = true;
                        }
                        break;
                    case 'sap.m.Select':
                        if (item.mProperties.selectedKey != "") {
                            ref['Field'] = item.getName();
                            ref['Operator'] = "eq";
                            ref['Value'] = item.mProperties.selectedKey;
                            valid = true;
                        }
                        break;
                    default:
                    //MessageToast.show("Tipo de objeto não mapeado" + item.getMetadata().getName());
                }
                if (valid) {
                    criterias['criterias'].push(ref);
                }
            });
        });
        return criterias;
    };

    GlobalFunctions.prototype.addTituloNaPagina = function (Titulo, form, replaceContent = true) {
        var elemento = new sap.m.Title({
            text: Titulo
        });
        
        if (replaceContent)
        {
            form.byId("titulo").removeAllItems();
        }

        form.byId("titulo").addItem(elemento);
    };

    GlobalFunctions.prototype.addDadosTabela = function (data) {

    };

    GlobalFunctions.prototype.addColunasDaTabela = function (colunas, view, destino) {
        for (var i = 0; i < colunas.length; i++) {
            view.byId(destino).addColumn(
                new sap.m.Column({
                    header: new sap.m.Label({
                        text: colunas[i]
                    })
                }
                ));
        }
    };
    GlobalFunctions.prototype.addTabs = function (form, Tabs) {
        form.byId("tabBar").removeAllItems();

        for (var i = 0; i < Tabs.length; i++) {
            var tab = new sap.m.IconTabFilter({
                key: Tabs[i].key,
                text: Tabs[i].texto
            });
            form.byId("tabBar").addItem(tab);
        }
        form._mFilters = {};
        for (var i = 0; i < Tabs.length; i++) {
            form._mFilters[Tabs[i].key] = [new sap.ui.model.Filter(Tabs[i].campoFiltrado, Tabs[i].tipoFiltro, Tabs[i].valorFiltro)];
        }
    }
    GlobalFunctions.prototype.addTabsToCards = function (form, Tabs, data) {
        for (var i = 0; i < Tabs.length; i++) {
            var tab = new sap.m.IconTabFilter({
                key: Tabs[i].key,
                text: Tabs[i].texto
            });

            var tile = new sap.m.GenericTile({
                header: "{menu>title}",
                subheader: "{menu>subtitle}",
                press: "onTilePress"
            });

            var flex = new sap.m.FlexBox({
                direction: "Column",
                width: "195px",
                items: tile
            });

            //flex.addItem(tile);
            //tab.addContent(flex);

            flex.bindItems("menu>/", tab);
            //tab.setModel(oModel, modelo);
            form.byId("tabBar").addItem(tab);
        }
    }
    GlobalFunctions.prototype.addTable = function (form, json, tabelaConf, filtroColunas = undefined, toolbarEvent = undefined) {
        //carrega tabs

        if (tabelaConf.Tabs && tabelaConf.Tabs.length > 0)
        {
            form.byId("tabBar").removeAllItems();
            for (var i = 0; i < tabelaConf.Tabs.length; i++) {
                var tab = new sap.m.IconTabFilter({
                    key: tabelaConf.Tabs[i].key,
                    text: tabelaConf.Tabs[i].texto
                });
                form.byId("tabBar").addItem(tab);
            }
            form._mFilters = {};
            for (var i = 0; i < tabelaConf.Tabs.length; i++) {
                form._mFilters[tabelaConf.Tabs[i].key] = [new sap.ui.model.Filter(tabelaConf.Tabs[i].campoFiltrado, tabelaConf.Tabs[i].tipoFiltro, tabelaConf.Tabs[i].valorFiltro)];
            }
        }

        if (tabelaConf.Buttons && tabelaConf.Buttons.length > 0)
        {
            var toolbar = form.byId("tableToolbar")
            tabelaConf.Buttons.forEach((cfg) => {
                var button = new sap.m.Button(cfg.id, {
                    text: cfg.text
                });
                button.attachPress(toolbarEvent, form);
                button.addStyleClass("sapUiSmallMarginEnd");
                toolbar.insertContent(button, 99);
            });
        }

        //carrega colunas
        var elemento = form.byId("tableTransactions");
        elemento.removeAllItems();
        
        var aColumns = [];
        console.log(tabelaConf.colunasTabela);
        for (var p = 0; p < tabelaConf.colunasTabela.length; p++) {
            if(filtroColunas){
                var adicionarColuna = false;
                if(filtroColunas.includes(tabelaConf.colunasTabela[p].texto) ){
                    elemento.addColumn(
                        new sap.m.Column({
                            header: new sap.m.Label({
                                text: tabelaConf.colunasTabela[p].texto
                            })
                        })
                    );
                }
            }
            else{
                elemento.addColumn(
                    new sap.m.Column({
                        header: new sap.m.Label({
                            text: tabelaConf.colunasTabela[p].texto
                        })
                    })
                );
            }
        }

        var celulas = [];
        for (var p = 0; p < tabelaConf.colunasTabela.length; p++) {
            if(filtroColunas){
                if(filtroColunas.includes(tabelaConf.colunasTabela[p].texto) ){
                    var str = "{" + tabelaConf.colunasTabela[p].bdName + "}";
                    celulas.push(
                        new sap.m.Link({
                            text: str
                        })
                    );
                }
            }
            else{
                var str = "{" + tabelaConf.colunasTabela[p].bdName + "}";
                celulas.push(
                    new sap.m.Link({
                        text: str
                    })
                );
            }
        }


        var columnListItem =  new sap.m.ColumnListItem({
            cells: celulas,
            type: "Navigation"
        });

        elemento.bindItems("/", columnListItem);

        elemento.setModel(new sap.ui.model.json.JSONModel(json));
        form.byId("tituloTabela").setText(tabelaConf.Titulo);

        //form.byId("tabBar").addContent(elemento);
    }

    GlobalFunctions.prototype.addTableFilterColunas = function (json) {
        var colunas = json.Tabela.colunasTabela;
        var colunasProntas = [];
        for (var p = 0; p < colunas.length; p++) {
            console.log();
            colunasProntas.push(
                {
                    "nome": colunas[p].textoColuna
                }
            );
        };
        //carrega colunas
        var elemento = sap.ui.getCore().byId("tableFilterColunas");
        elemento.removeAllColumns();
        
        var aColumns = [];
        elemento.addColumn(
            new sap.m.Column({
                header: new sap.m.Label({
                    text: "Coluna"
                })
            })
        );
        var celulas = [];
        for (var p = 0; p < colunasProntas.length; p++) {
            var str = "{nome}";
            celulas.push(
                new sap.m.Link({
                    text: str
                })
            );
        }
        elemento.bindItems("/", new sap.m.ColumnListItem({
            cells: celulas,
        }));
        elemento.setModel(new sap.ui.model.json.JSONModel(colunasProntas));
    }


    GlobalFunctions.prototype.addFieldsDetail = function (form, data = undefined, detalheConf) {
        console.log(data);
        var destino = form.byId('corpo');
        var tabBar = form.byId('tabBar');
        tabBar.removeAllItems();

        for (var g = 0; g < detalheConf.length; g++) {
            //criar tab
            var tab = new sap.m.IconTabFilter({
                text: detalheConf[g].Nome
            });


            for (var j = 0; j < detalheConf[g].Dados.length; j++) {
                var simpleForm = new sap.ui.layout.form.SimpleForm({
                    title: detalheConf[g].Dados[j].Nome,
                    editable: false,
                    layout:"ResponsiveGridLayout",
                    labelSpanXL:6,
                    labelSpanL:6,
                    labelSpanM:6,
                    labelSpanS:6,
                    adjustLabelSpan:false,
                    emptySpanXL:0,
                    emptySpanL:0,
                    emptySpanM:0,
                    emptySpanS:0,
                    columnsXL:1,
                    columnsL:3,
                    columnsM:2
                });

                
                for (var i = 0; i < detalheConf[g].Dados[j].Detalhe.length; i++) {
                    var form = new sap.ui.core.Title({
                        text: detalheConf[g].Dados[j].Detalhe[i].Grupo
                    });
                    var listCampos = [];

                    for (var p = 0; p < detalheConf[g].Dados[j].Detalhe[i].Campos.length; p++) {
                        var elemento = undefined;
                        switch (detalheConf[g].Dados[j].Detalhe[i].Campos[p].tipo) {
                            case 'input':
                                elemento = new sap.m.Input({
                                    placeholder: detalheConf[g].Dados[j].Detalhe[i].Campos[p].texto,
                                    type: detalheConf[g].Dados[j].Detalhe[i].Campos[p].masc,
                                    name: detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName,
                                });
                                break;
                            case 'text':
                                elemento = new sap.m.Text({
                                    text: data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName]
                                })
                                break;
                            case 'table':
                                if (data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName]) {
                                    var aColumns = [];
                                    for (var k = 0; k < Object.keys(data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName][0]).length; k++) {
                                        if (detalheConf[g].Dados[j].Detalhe[i].Campos[p].colunas.includes(Object.keys(data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName][0])[k])) {
                                            aColumns.push(
                                                new sap.m.Column({
                                                    header: new sap.m.Label({
                                                        text: Object.keys(data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName][0])[k]
                                                    })
                                                })
                                            );
                                        }

                                    }
                                    elemento = new sap.m.Table({
                                        columns: aColumns
                                    });
                                    var celulas = [];
                                    for (var u = 0; u < Object.keys(data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName][0]).length; u++) {
                                        var str = "{" + Object.keys(data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName][0])[u] + "}";
                                        if (detalheConf[g].Dados[j].Detalhe[i].Campos[p].colunas.includes(Object.keys(data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName][0])[u])) {
                                            celulas.push(
                                                new sap.m.Text({
                                                    text: str
                                                })
                                            );
                                        }
                                    }

                                    elemento.bindItems("/", new sap.m.ColumnListItem({
                                        cells: celulas
                                    }));

                                    elemento.setModel(new sap.ui.model.json.JSONModel(data[detalheConf[g].Dados[j].Detalhe[i].Campos[p].bdName]));
                                }
                                break;
                            default:
                                MessageToast.show("Tipo de objeto não mapeado");
                        }

                        listCampos.push(
                            new sap.m.Label({
                                text: detalheConf[g].Dados[j].Detalhe[i].Campos[p].texto
                            })
                        );
                        listCampos.push(
                            elemento
                        )
                    };
                    simpleForm.addContent(form);
                    for (var c = 0; c < listCampos.length; c++) {
                        if(listCampos[c]){
                            simpleForm.addContent(listCampos[c]);
                        }
                        
                    }
                }

                var vbox = new sap.m.VBox({});
                vbox.addItem(simpleForm);
                tab.addContent(vbox);
            }
            tabBar.addItem(tab)
        }
    };

    GlobalFunctions.prototype.addFields = function (form, data = undefined, filtragemConf) {
        var destino = form.byId('painelHeader');
        destino.removeAllItems();

        //
        //--> Format caixa de filtros
        //
        var filterBox = new sap.ui.layout.HorizontalLayout({
            layoutData: new sap.m.FlexItemData({
                growFactor: 1,
                shrinkFactor: 1,
                baseSize: "auto"
            })
        });
        filterBox.addStyleClass("sapUiNoMargin");

        var searchField = new s3rial.www.global.control.Input(form.createId("searchField"), {
            placeholder: "Procurar",
            icon: "search"
        });

        var flexSearch = new sap.m.FlexBox({
            direction: "Column",
            width: filtragemConf.tamanhoDoFlex
        });
        
        flexSearch.addStyleClass("sapUiSmallMarginEnd sapUiTinyMarginBottom");
        flexSearch.addItem(new sap.m.Label({ text: "Pesquisa" }));
        flexSearch.addItem(searchField);

        var fieldsBox = new sap.m.FlexBox({direction:"Row", wrap:"Wrap", justifyContent: "Start"});
        fieldsBox.addStyleClass("sapUiTinyMarginEnd sapUiNoMargin");

        var buttonsBox = new sap.m.FlexBox(form.createId("buttons"), {
            direction:"Row", 
            wrap:"Wrap", 
            justifyContent: "End",
            alignItems: "Center",
            width: filtragemConf.tamanhoDoFlex,
            layoutData: new sap.m.FlexItemData({
                growFactor: 1,
                shrinkFactor: 1,
                baseSize: "auto"
            })
        });

        var spacer = new sap.m.FlexBox({
            layoutData: new sap.m.FlexItemData({
                growFactor: 1,
                shrinkFactor: 1,
                baseSize: "auto"
            })
        })

        fieldsBox.addItem(flexSearch);
        //fieldsBox.addItem(spacer);
        fieldsBox.addItem(buttonsBox);

        filterBox.addContent(fieldsBox);

        destino.addItem(filterBox);

        var largura = '90%';

        for (var i = 0; i < filtragemConf.Campos.length; i++) {
            var evento = filtragemConf.Campos[i].click;
            var elemento = undefined;
            var masc = undefined;
            var cfgMasc = filtragemConf.Campos[i].masc || "";

            //define mascara do campo
            switch (cfgMasc.toLowerCase()) {
                case 'date':
                    masc = sap.m.InputType.Date;
                    break;
                case 'number':
                    masc = sap.m.InputType.Number;
                    break;
                case 'text':
                    masc = sap.m.InputType.Text;
                    break;
                default:
                    masc = sap.m.InputType.Text;
                    break;
            }

            var valorCampo = filtragemConf.Campos[i].texto;
            if (filtragemConf.Campos[i].bdName != "" && data != undefined) {
                valorCampo = data[filtragemConf.Campos[i].bdName];
            }

            if (filtragemConf.Campos[i].largura != "") {
                largura = filtragemConf.Campos[i].largura;
            }

            //cria filtragemConf.Campos
            switch (filtragemConf.Campos[i].tipo) {
                case 'button':
                    elemento = new sap.m.Button(filtragemConf.Campos[i].bdName, {
                        text: filtragemConf.Campos[i].texto,
                        type: "Emphasized"
                    });

                    if (evento)    
                    {
                        elemento.attachPress(form[evento], form);
                    }

                    break;

                case 'link':
                    elemento = new sap.m.Link(filtragemConf.Campos[i].bdName, {
                        text: filtragemConf.Campos[i].texto
                    });

                    if (evento)    
                    {
                        elemento.attachPress(form[evento], form);
                    }

                    break;
                    
                case 'input':
                    switch(filtragemConf.Campos[i].masc.toLowerCase())
                    {
                        case "date":
                            elemento = new sap.m.DatePicker({
                                placeholder: valorCampo,
                                name: filtragemConf.Campos[i].bdName,
                                valueFormat: "yyyy-MM-dd",
                                value: {
                                        path: `filter>/${filtragemConf.Campos[i].bdName}`,
                                        }
                            });
                            break;

                        default: 
                            elemento = new s3rial.www.global.control.Input({
                                placeholder: valorCampo,
                                type: masc,
                                name: filtragemConf.Campos[i].bdName,
                                value: `{filter>/${filtragemConf.Campos[i].bdName}}`
                            });
                            break;
                    }

                    break;

                case 'selectlist':
                    switch (filtragemConf.Campos[i].masc.toLowerCase())
                    {
                        case "branch":
                            elemento = new s3rial.www.global.control.SelectListBranch(form.createId("flt_" + filtragemConf.Campos[i].bdName), {
                                value: `{filter>/${filtragemConf.Campos[i].bdName}}`
                            });
                            break;

                        case "enum":
                            elemento = new s3rial.www.global.control.SelectListBranch(form.createId("flt_" + filtragemConf.Campos[i].bdName), {
                                origin: 'enum',
                                value: `{filter>/${filtragemConf.Campos[i].bdName}}`,
                                keyField: "key",
                                textField: "value",
                                enumSource: filtragemConf.Campos[i].enumSource,
                                enumName: filtragemConf.Campos[i].enumName
                            });
                            break;

                        default:
                            elemento = new s3rial.www.global.control.SelectList({
                                apiName: filtragemConf.Campos[i].apiName,
                                listMethod: filtragemConf.Campos[i].listMethod,
                                keyField: "key",
                                textField: "text",
                                title: filtragemConf.Campos[i].texto,
                                value: `{filter>/${filtragemConf.Campos[i].bdName}}`
                            });
                            break;
                    }
                    break;
                
                case 'combobox': 
                    var config = filtragemConf.Campos[i];

                    elemento = new sap.m.Select({
                        forceSelection: false,
                        selectedKey: `{filter>/${config.bdName}}`,
                        width: "100%"
                    });

                    /*
                    elemento.bindAggregation("items", "/",  (sId, oContext) => {
                        return new sap.ui.core.Item({
                            key: '{key}',
                            text: '{value}'
                        });
                    });
                    */

                    elemento.bindItems("/",  new sap.ui.core.Item({
                            key: '{key}',
                            text: '{value}'
                        })
                    );

                    this._loadEnum(elemento, config).then(
                        (config) => {
                            var list = config.enumObj.list(config.enumName);
                            var oModel = new JSONModel(list);
                            config.elemento.setModel(oModel);
                        }
                    )

                    break;

                case 'label':
                    elemento = new sap.m.Label({
                        text: valorCampo
                    })
                    break;
                case 'table':
                    if (data[filtragemConf.Campos[i].bdName] != null) {
                        var aColumns = [];
                        for (var p = 0; p < Object.keys(data[filtragemConf.Campos[i].bdName][0]).length; p++) {
                            aColumns.push(
                                new sap.m.Column({
                                    header: new sap.m.Label({
                                        text: Object.keys(data[filtragemConf.Campos[i].bdName][0])[p]
                                    })
                                })
                            );
                        }
                        elemento = new sap.m.Table({
                            columns: aColumns
                        });
                        var celulas = [];
                        for (var u = 0; u < Object.keys(data[filtragemConf.Campos[i].bdName][0]).length; u++) {
                            console.log(Object.keys(data[filtragemConf.Campos[i].bdName][0])[u])
                            var str = "{" + Object.keys(data[filtragemConf.Campos[i].bdName][0])[u] + "}";
                            celulas.push(
                                new sap.m.Text({
                                    text: str
                                })
                            );
                        }
                        elemento.bindItems("/", new sap.m.ColumnListItem({
                            cells: celulas,
                            title: "teste"
                        }));
                        elemento.setModel(new sap.ui.model.json.JSONModel(data[filtragemConf.Campos[i].bdName]));
                    }
                    break;
                default:
                    MessageToast.show("Tipo de objeto não mapeado");
            }

            var flex = new sap.m.FlexBox({
                direction: "Column",
                width: filtragemConf.Campos[i].tipo === "button" ? "auto" : 
                        filtragemConf.Campos[i].tipo === "link"? "auto" : filtragemConf.tamanhoDoFlex
            });
            
            flex.addStyleClass("sapUiSmallMarginEnd sapUiTinyMarginBottom");
            
            var label = new sap.m.Label({
                text: filtragemConf.Campos[i].texto,
            });

            switch(filtragemConf.Campos[i].tipo)
            {
                case "button":
                case "link":
                    label.setText("");
                    flex.addItem(label);
                    break;
                default:
                    flex.addItem(label);
                    break;
            }

            flex.addItem(elemento);

            if (filtragemConf.Campos[i].tipo === "button" || 
                filtragemConf.Campos[i].tipo === "link") {
                buttonsBox.addItem(flex);
            }
            else
            {
                var position = fieldsBox.getItems().length - 1;
                fieldsBox.insertItem(flex, position);
            }

            //destino.addItem(flex);
        }
    };

    GlobalFunctions.prototype._loadEnum = function(elemento, config) {
        return new Promise((resolve, error) => {
            var enumClass = config.enumSource.split(".").join("/");
            var enumObj = _EnumSources[enumClass];

            if (!enumObj)
            {
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

                _EnumSources[enumClass] = enumObj;
            }

            var result = Object.assign({}, config);
            result["elemento"] = elemento;
            result["enumObj"] = enumObj;

            resolve(result);
        });
    };

    GlobalFunctions.prototype.DEL_addFields = function (form, data = undefined, filtragemConf) {
        var destino = form.byId('painelHeader');
        destino.removeAllItems();

        //
        //--> Format caixa de filtros
        //
        var filterBox = new sap.m.FlexBox();
        filterBox.addStyleClass("sapUiNoMargin");

        var searchField = new sap.m.Input(form.createId("searchField"), {
            placeholder: "Procurar"
        });

        var flexSearch = new sap.m.FlexBox({
            direction: "Column",
            width: filtragemConf.tamanhoDoFlex
        });
        
        flexSearch.addStyleClass("sapUiTinyMarginEnd sapUiTinyMarginBottom");
        flexSearch.addItem(new sap.m.Label({ text: "Pesquisa" }));
        flexSearch.addItem(searchField);

        var fieldsBox = new sap.m.FlexBox({direction:"Row", wrap:"Wrap"});
        fieldsBox.addStyleClass("sapUiTinyMarginEnd sapUiTinyMarginBottom");

        var buttonsBox = new sap.m.FlexBox(form.createId("buttons"), {
            direction:"Row", 
            wrap:"Wrap", 
            justifyContent: "End",
            alignItems: "Center",
            width: filtragemConf.tamanhoDoFlex,
            layoutData: new sap.m.FlexItemData({
                alignSelf: "End",
                growFactor: 1,
                baseSize: filtragemConf.tamanhoDoFlex
            })
        });

        fieldsBox.addItem(flexSearch);
        fieldsBox.addItem(buttonsBox);

        filterBox.addItem(fieldsBox);
        destino.addItem(filterBox);

        var largura = '90%';

        for (var i = 0; i < filtragemConf.Campos.length; i++) {
            var evento = filtragemConf.Campos[i].click;
            var elemento = undefined;
            var masc = undefined;

            //define mascara do campo
            switch (filtragemConf.Campos[i].masc.toLowerCase()) {
                case 'date':
                    masc = sap.m.InputType.Date;
                    break;
                case 'number':
                    masc = sap.m.InputType.Number;
                    break;
                case 'text':
                    masc = sap.m.InputType.Text;
                    break;
                default:
                    MessageToast.show("Tipo de mascara não cadastrada");
            }
            var valorCampo = filtragemConf.Campos[i].texto;
            if (filtragemConf.Campos[i].bdName != "" && data != undefined) {
                valorCampo = data[filtragemConf.Campos[i].bdName];
            }

            if (filtragemConf.Campos[i].largura != "") {
                largura = filtragemConf.Campos[i].largura;
            }

            //cria filtragemConf.Campos
            switch (filtragemConf.Campos[i].tipo) {
                case 'button':
                    elemento = new sap.m.Button(filtragemConf.Campos[i].bdName, {
                        text: filtragemConf.Campos[i].texto,
                        type: "Emphasized"
                    });

                    if (evento)    
                    {
                        elemento.attachPress(form[evento], form);
                    }

                    break;
                case 'link':
                    elemento = new sap.m.Link(filtragemConf.Campos[i].bdName, {
                        text: filtragemConf.Campos[i].texto
                    });

                    if (evento)    
                    {
                        elemento.attachPress(form[evento], form);
                    }

                    break;
                case 'input':
                    elemento = new s3rial.www.global.control.Input({
                        placeholder: valorCampo,
                        type: masc,
                        name: filtragemConf.Campos[i].bdName,
                        value: `{filter>/${filtragemConf.Campos[i].bdName}}`
                    });

                    elemento.addEndIcon({
                        src: IconPool.getIconURI("filter"),
                        noTabStop: true,                            
                    });
                    break;
                case 'select':
                    elemento = new sap.m.Select({
                        items: valorCampo,
                        name: filtragemConf.Campos[i].bdName
                    })
                    break;
                case 'label':
                    elemento = new sap.m.Label({
                        text: valorCampo
                    })
                    break;
                case 'table':
                    if (data[filtragemConf.Campos[i].bdName] != null) {
                        var aColumns = [];
                        for (var p = 0; p < Object.keys(data[filtragemConf.Campos[i].bdName][0]).length; p++) {
                            aColumns.push(
                                new sap.m.Column({
                                    header: new sap.m.Label({
                                        text: Object.keys(data[filtragemConf.Campos[i].bdName][0])[p]
                                    })
                                })
                            );
                        }
                        elemento = new sap.m.Table({
                            columns: aColumns
                        });
                        var celulas = [];
                        for (var u = 0; u < Object.keys(data[filtragemConf.Campos[i].bdName][0]).length; u++) {
                            console.log(Object.keys(data[filtragemConf.Campos[i].bdName][0])[u])
                            var str = "{" + Object.keys(data[filtragemConf.Campos[i].bdName][0])[u] + "}";
                            celulas.push(
                                new sap.m.Text({
                                    text: str
                                })
                            );
                        }
                        elemento.bindItems("/", new sap.m.ColumnListItem({
                            cells: celulas,
                            title: "teste"
                        }));
                        elemento.setModel(new sap.ui.model.json.JSONModel(data[filtragemConf.Campos[i].bdName]));
                    }
                    break;
                default:
                    MessageToast.show("Tipo de objeto não mapeado");
            }

            var flex = new sap.m.FlexBox({
                direction: "Column",
                width: filtragemConf.Campos[i].tipo === "button" ? "auto" : 
                        filtragemConf.Campos[i].tipo === "link"? "auto" : filtragemConf.tamanhoDoFlex
            });
            
            flex.addStyleClass("sapUiTinyMarginEnd sapUiTinyMarginBottom");
            
            var label = new sap.m.Label({
                text: filtragemConf.Campos[i].texto,
            });

            switch(filtragemConf.Campos[i].tipo)
            {
                case "button":
                case "link":
                    label.setText("");
                    flex.addItem(label);
                    break;
                default:
                    flex.addItem(label);
                    break;
            }

            flex.addItem(elemento);

            if (filtragemConf.Campos[i].tipo === "button" || 
                filtragemConf.Campos[i].tipo === "link") {
                buttonsBox.addItem(flex);
            }
            else
            {
                var position = fieldsBox.getItems().length - 1;
                fieldsBox.insertItem(flex, position);
            }

            //destino.addItem(flex);
        }
    };

    function executeFunctionByName(functionName, context) {
        var args = Array.prototype.slice.call(arguments, 2);
        var namespaces = functionName.split(".");
        var func = namespaces.pop();
        for (var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }
        return context[func].apply(context, args);
    }


    //pré carrega função com os valores	
    GlobalFunctions.prototype.ConfigAPI = function () {
        var request = new XMLHttpRequest();
        request.open('GET', '/config.json', false);
        request.send(null);
        return JSON.parse(request.response);
    };

    GlobalFunctions.prototype.GetMenu = function () {
        var request = new XMLHttpRequest();
        request.open('GET', 'menu.json', false);
        request.send(null);
        return JSON.parse(request.response);
    };

    GlobalFunctions.prototype.GetMock = function (nome) {
        var request = new XMLHttpRequest();
        request.open('GET', 'mock/' + nome + '.json', false);
        request.send(null);
        return JSON.parse(request.response);
    };

    GlobalFunctions.prototype.GetConfPage = function (nome) {
        var request = new XMLHttpRequest();
        var url = sap.ui.require.toUrl(nome);
        request.open('GET', url, false);
        request.send(null);
        return JSON.parse(request.response);
    };

    GlobalFunctions.prototype.NavigateBetweenSystems = function (sistema) {
        var token = undefined;
        try {
            token = GlobalFunctions.getLocalStorage('token');
        }
        catch (e) {

        }

        if (token) {
            window.location.href = "../" + sistema + "/index.html";
        }
        else {
            MessageToast.show("Sessão Expirada");
        }
    };

    GlobalFunctions.prototype.checkCookies = function () {
        var Session = undefined;
        var System = undefined;
        var Token = undefined;
        var ck = document.cookie;
        var re = ';';

        var nameList = ck.split(re);
        for (var i = 0; i < nameList.length; i++) {
            if (nameList[i].includes('B1SESSION')) {
                var valor = nameList[i].split("=");
                Session = valor[1];
            }
            if (nameList[i].includes('system')) {
                var valor = nameList[i].split("=");
                System = valor[1];
            }
            if (nameList[i].includes('token')) {
                var valor = nameList[i].split("token=");
                alert(valor);
                Token = valor;
            }
        }
        if (Session == "") {
            MessageToast.show("Sessão Expirada." + System);
        }
        var varListInfos = [];
        varListInfos.push(Session);
        varListInfos.push(System);
        varListInfos.push(Token);
        return varListInfos;
    };


    GlobalFunctions.prototype.setLocalStorage = function (key, value) {
        localStorage.setItem(key, value);
        var val = localStorage.getItem(key);
        MessageToast.show(val);
    };

    GlobalFunctions.prototype.getLocalStorage = function (key) {
        var val = undefined;
        try {
            val = localStorage.getItem(key);
        }
        catch (e) {
            MessageToast.show(e);
        }
        return val;
    };

    GlobalFunctions.prototype.getUrlParam = function () {
        var arrayOfStrings = window.location.href.split('/');
        return arrayOfStrings[arrayOfStrings.length - 1];
    };

    GlobalFunctions.prototype.requestToServer = function (apiBase, methodBase, method = undefined, bodyPo = undefined, msgError = "Erro", msgSuccess = "Sucesso", httpMethod = 'get', showStatusCode = false, keyhead = undefined) {
        var url = "";
        if (method == undefined) {
            url = apiBase + methodBase;
        }
        else {
            url = apiBase + methodBase + '/' + method;
        }

        var headerIn = {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
        if (keyhead != undefined) {
            headerIn['varsis_token'] = keyhead;
        }

        return new Promise((resolve, reject) => {

            var json = [];
            fetch(url, {
                method: httpMethod,
                headers: headerIn,
                body: JSON.stringify(bodyPo)
            }).then(function (response) {
                if (msgSuccess && response.status == 200) {
                    MessageToast.show(msgSuccess);
                }
                else {
                    if (showStatusCode) {
                        MessageToast.show(response.status);
                    }
                }
                resolve(response.json());
            }).catch(function (data) {
                if (msgError) {
                    if (showStatusCode) {
                        MessageToast.show(msgError + ' - ' + data);
                        //console.log(data);
                    }
                    else {
                        MessageToast.show(msgError);
                    }
                }
                reject(data);
            });
        })
            .then(
                requestToServerReturn
            )
            .catch(
                requestToServerReturn
            );
    };

    function requestToServerReturn(retorno) {
        //console.log(retorno);
        return retorno;
    }


    GlobalFunctions.prototype.TransformJsonToForm = function (json) {
        var newobj = [];
        for (var i = 0; i < json.length; i++) {
            var obj = json[i];
            for (var key in obj) {
                var no = {};
                no['coluna'] = key;
                no['valor'] = obj[key];
                newobj.push(no);
            }
        }
        return newobj;
    };

    GlobalFunctions.prototype.TransformFormToJson = function (json) {
        var newobj = [];
        for (var i = 0; i < json.length; i++) {
            var obj = json[i];
            newobj[json[i]['coluna']] = json[i]['valor'];
        }
        return newobj;
    };

    GlobalFunctions.prototype.ConfsToView = function (json) {
        var conf = GlobalFunctionsGetMock('confManagerPayments');
        return conf;
    };

    return new GlobalFunctions();
}, true);