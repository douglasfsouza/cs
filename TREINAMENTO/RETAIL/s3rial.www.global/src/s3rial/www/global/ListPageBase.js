sap.ui.define([
	"s3rial/www/global/ControllerBase",
	"sap/ui/model/json/JSONModel",
	"sap/m/MessageToast",
	"s3rial/www/global/GlobalFunctions",
	"s3rial/www/global/DatasourceBase",
	"sap/ui/core/UIComponent"	
], function(ControllerBase, JSONModel, MessageToast, Global, Datasource, UIComponent) {
	"use strict";

	return ControllerBase.extend("s3rial.www.global.ListPageBase", {
		metadata: {
			library : "s3rial.www.global",
			publicMethods: [
				"configurationFile", "pageTitle", "pageFilter", "tableConfig",
				"cancelFilter", "onFilterClicked", "setTableTitle", "getTableTitle",
				"onRowChanged", "onRowNavigation", "onSelectionChange", "changeTableToolbar", "getData", "setData",
				"onTableItemPress", "onTableSelectionChange", "showError", "showAlert", "showMessage"
			]
		},

		TABLE_NAME: "maintable",
		TABLE_TOOLBAR_NAME: "tableToolbar",

		_pageTitle: null,
		_pageFilter: null,
		_tableConfig: null,
		_dataSet: null,
		_listMethod: null,

		/**
		 * Retorna o título da página
		 * 
		 * Obrigatório quando configurationFile não estiver implementado
		 */
		pageTitle: function() {
			console.log("Method Not Implemented");
		},

		/**
		 * Retorna configuração para criação dos filtros
		 * 
		 * Opcional
		 */
		pageFilter: function() {
			console.log("Method Not Implemented");
		},

		/**
		 * Retorna configuração da grid
		 * 
		 * Obrigatório quando configurationFile não estiver implementado
		 */
		tableConfig: function() {
			console.log("Method Not Implemented");
		},

		/**
		 * Controla se o filtro disparado pelo usuário pode ser executado
		 * 
		 * Permite validar as opções de filtro selecionadas pelo usuário
		 * 
		 * @param {mandatory} filter any[] - objeto com os valores informados pelo usuário
		 * @returns boolean
		 */
		cancelFilter: function(filter)
		{
			console.log("Method Not Implemented");
		},

		/**
		 * Altera o título da tabela
		 * 
		 * @param {mandatory} title string
		 */
		setTableTitle: function(title) {
			var oModel = new JSONModel({ title: title });
			this.getView().setModel(oModel, "table");
		},

		/**
		 * Retorna o título da tabela
		 */
		getTableTitle: function() {
			var oModel = this.getView().getModel("table");
			if (oModel)
			{
				return oModel.getData().title;
			}
		},

		/**
		 * 
		 * @param {mandatory} listMethod Nome do método da api que deve ser chamado para carregar dados na tela
		 */
		setListMethod: function(listMethod) {
			this._listMethod = listMethod;
		},

		/**
		 * Nome do método da api que deve ser chamado para carregar dados na tela
		 */
		getListMethod: function() {
			return this._listMethod;
		},

		/**
		 * 
		 * @param {mandatory} data any[] Lista de objetos para preencher a grid
		 */
		setData: function(data) {
			var model = new JSONModel(data);
			var table = this.byId(this.TABLE_NAME);

			table.setModel(model);

			if (this._tableConfig.Tipo === this.TABLE_GRID) {
				table.bindRows("/");
			}
		},

		/**
		 * Retorna a lista de objetos atualmente carregada na grid
		 */
		getData: function() {
			var table = this.byId(this.TABLE_NAME);
			var model = table.getModel();
			return model.getData();
		},

		/**
		 * Altera a configuração dos botões da tabela
		 * 
		 * @param {mandatory} buttons any[]: Lista de botões que terão seus atributos modificados
		 */
		changeTableToolbar: function(buttons) {
			var toolbarButtons = this.byId(this.TABLE_TOOLBAR_NAME).getContent();
			toolbarButtons.forEach((control) => {
				var button = buttons.find(m => m.id === control.getId());
				if (button)
				{
					control.setEnabled(button.enabled);
				}
			});
		},

		/**
		 * Informa que houve navegação nas linhas da grid
		 * 
		 * @param {mandatory} currentRow: any
		 */
		onRowChanged: function(currentRow) {
			console.log("Method Not Implemented");
		},

		/**
		 * Informa que houve navegação nas linhas da grid
		 * 
		 * @param {mandatory} currentRow: any
		 */
		onRowNavigation: function(currentRow) {
			console.log("Method Not Implemented");
		},

		/**
		 * Informa que houve seleção de uma ou mais linhas
		 * 
		 * @param {mandatory} selectedRows: any[]
		 */
		onSelectionChanged: function(selectedRows) {
			console.log("Method Not Implemented");
		},

		/**
		 * Informa que um botão da grid foi pressionado pelo usuário
		 * 
		 * @param {mandatory} buttonId string: Id do botão pressionado pelo usuário 
		 */
		onTableToolbarClicked: function(buttonId) {
			console.log("Method Not Implemented");
		},

		onFilterClicked: function(oEvent) {
			
			var panel = this.byId("painelHeader");
			var criteria = this._mountCriteria();
			var cancelled = this.cancelFilter(criteria);

			if (!cancelled)
			{
				this._loadData(criteria);
			}
		},

		_onTableToolbarClickedInternal: function(oEvent) {
			var buttonId = oEvent.getSource().getId();
			this.onTableToolbarClicked(buttonId);
		},

		onTabBarClicked(key) {
			console.log("onTabBarClicked - Method Not Implemented");
		},

		_onTabbarClicked: function(oEvent)
		{
			var key = oEvent.getParameters("key");
			this.onTabBarClicked(key);
		},

		onTableRefreshPress: function(oEvent) {
			this._loadData();
		},

		_onTableItemPress: function(oEvent) {
			var index = oEvent.getSource()
							  .getItemNavigation()
							  .getFocusedIndex() - 1;

			var selectedItem = oEvent.getSource()
									 .getItems()[index];

			var context = selectedItem.getBindingContext();
			var obj = context.getObject(context.getPath());

			this.onRowChanged(obj);
		},

		_onGridNavigation: function(oEvent) {
			var context = oEvent.getSource()
								.getBindingContext();
								
			var obj = context.getObject(context.getPath());

			this.onRowNavigation(obj);
		},

		_onTableSelectionChange: function(oEvent) {
			switch(this._tableConfig.Tipo)
			{
				case this.TABLE_RESPONSIVE:
					this._onResponsiveSelectionChange(oEvent);
					break;
				
				case this.TABLE_GRID:
					this._onGridSelectionChange(oEvent);
					break;
			}
		},

		_onResponsiveSelectionChange: function(oEvent) {
			var contexts = oEvent.getSource()
								 .getSelectedContexts();
								 
			var list = contexts.map((context) => {
				return context.getObject(context.getPath()); 
			});

			this.onSelectionChanged(list);
		},

		_onGridSelectionChange: function(oEvent) {
			var table = this.byId(this.TABLE_NAME);
			var indexes = table.getSelectedIndices();

			var list = indexes.map((index) => {
				var context = table.getContextByIndex(index);
				return context.getObject(context.getPath()); 
			});

			this.onSelectionChanged(list);
		},

		onInit: function () {
			ControllerBase.prototype.onInit.call(this);

			this.getView().addStyleClass("sapUiSizeCompact");

			//
			// --> Obtém as configurações da página
			// 
			var configurationFile = this.configurationFile();

			//--> Obtém de um arquivo de configuração externo
			if (configurationFile)
			{
				var config = Global.GetConfPage(this.configurationFile());
				this._pageTitle = config.Pagina.Titulo.toString();
				this._pageFilter = config.Filtragem;
				this._tableConfig = config.Tabela;
			}
			else
			//--> Se não há um arquivo, tenta recuperar da classe estendida
			{
				this._pageTitle = this.pageTitle();
				this._pageFilter = this.pageFilter();
				this._tableConfig = this.tableConfig();
			}

			//
			//--> Configura o layout padrão
			//
			this._mountDefaultLayout();

			//
			//--> Reconfigura os componentes de acordo com a configuração
			//
			this._configureLayout();

			//
			//--> Inicializa model do paine de parâmetros
			// 
			var oFilter = new JSONModel();
			this.getView().setModel(oFilter, "filter");
		},

		attachDisplay: function(oEvent) {
			ControllerBase.prototype.attachDisplay.call(this, oEvent);

			var parameters = this.getPageParameters();

			if (parameters && parameters.recordKey)
			{
				this._loadData();
			}
		},

		onBeforeRendering: function() {
			ControllerBase.prototype.onBeforeRendering.call(this);
			this._loadData();
		},

		_mountDefaultLayout: function()
		{
			var layout = this.getView();

			layout.addStyleClass("sapUiSizeCompact sapUiNoMarginBegin");

			var page = this._createSemanticPage();

			layout.addContent(page);
		},

		_configureLayout: function()
		{
			//adiciona Titulo na Pagina
			Global.addTituloNaPagina(this._pageTitle, this);
			
			var filter = Object.assign({}, this._pageFilter);

			if (filter.Campos.length != 0)
			{
				filter.Campos.push({
					"tipo": "link",
					"texto": "Adaptar filtro",
					"masc": "text",
					"bdName": this.createId("filterLink")
				});

				filter.Campos.push({
					"tipo": "button",
					"texto": "Ir",
					"masc": "text",
					"bdName": this.createId("filterButton"),
					"click": "onFilterClicked"
				});
			}

			Global.addFields(this, undefined, filter);

			var searchField = this.byId("searchField");
			searchField.attachLiveChange(this._onSearchLive, this);

			this.setTableTitle(this._tableConfig.Titulo);

			this._addTableDefaultButtons();
		},

		_onSearchLive: function(oEvent) {
			var table = this.byId(this.TABLE_NAME);
			var token = oEvent.getParameters("value").value;
			var filtered = this._dataSet;
			var tableConfig = this._tableConfig;

			if (String(token).trim() !== "")
			{
				filtered = this._dataSet.filter((row) => {
					var cols = tableConfig.colunasTabela;
					var searchToken = String(token).toLowerCase();
					var containsToken = cols.some((col) => String(row[col.bdName])
														   .toLowerCase()
														   .includes(searchToken));

					if (containsToken)
					{
						return row;
					}
				});
			}

			this.setData(filtered);
			if (this._tableConfig.Tipo === this.TABLE_GRID) {
				table.bindRows("/");
			}
		},
		
		_createSemanticPage: function() {
			var hasFilter = true; //this._pageFilter && this._pageFilter.Campos && this._pageFilter.Campos.length > 0;
			var hasTable = this._tableConfig ? true : false;
			var hasTableTabs = this._tableConfig && this._tableConfig.Tabs && this._tableConfig.Tabs.length > 0;

			var layoutData = new sap.m.FlexItemData({
				growFactor: 1,
				shrinkFactor: 1,
				baseSize: "0%"	
			});

			var page = new sap.f.semantic.SemanticPage({
				id: this.createId("semanticPage"),
				headerPinnable: hasFilter,
				toggleHeaderOnTitleClick: true,
				preserveHeaderStateOnScroll: false,
				showFooter: false,
				fitContent: true,
				headerExpanded: hasFilter
			});

			page.setLayoutData(layoutData);

			page.addStyleClass("sapUiResponsiveContentPadding sapUiNoMargin");
	
			page.setTitleHeading(new sap.m.FlexBox(this.createId("titulo")));

			page.setTitleSnappedOnMobile(new sap.m.Title(this.createId("tituloSnapped")));

			// Só adiciona o headerContent se houver campos de filtro
			if (hasFilter)
			{
				var headerContent = new sap.m.FlexBox(this.createId("painelHeader"), {
					direction: "Row",
					wrap: "Wrap",
				});

				headerContent.addStyleClass("sapUiNoMargin");

				headerContent.addStyleClass("sapUiTinyMarginBottom");

				page.addHeaderContent(headerContent);
			}
	
			if (hasTable)
			{
				var tableConfig = Object.assign({}, this._tableConfig);

				tableConfig.tableTitle = "{table>/title}";
				tableConfig.onItemPress = [this._onTableItemPress, this];
				tableConfig.onSelectionChange = [this._onTableSelectionChange, this];
				tableConfig.onRowNavigation = [this._onGridNavigation, this];

				tableConfig.toolbarName = this.TABLE_TOOLBAR_NAME;
				tableConfig.onToolbarClick = [this._onTableToolbarClickedInternal, this];

				tableConfig.onTabbarClick = [this._onTabbarClicked, this];

				var table = this.createTable(this.TABLE_NAME, tableConfig);

				page.setContent(table);
			}

			return page;
		},

		_addTableDefaultButtons: function() {
			var toolbar = this.byId(this.TABLE_TOOLBAR_NAME);

			var button = new sap.m.Button({
				tooltip: "Atualizar",
				icon: "sap-icon://refresh",
				press: [this.onTableRefreshPress, this]
			});

			button.addStyleClass("sapUiSmallMarginEnd");
			toolbar.insertContent(button, 99);
		},

		_mountCriteria: function() {
			var record = this.getView().getModel("filter").getData();

			var keys = Object.keys(record);
			var criteria = [];

			keys.forEach((field) => {
				var operador = this._pageFilter.Campos.find(element => element.bdName == field).operator;
				if(!operador){
					operador = 'eq'
				}
				if (String(record[field]).trim() !==  "")
				{
					criteria.push({
						Field: field,
						Operator: operador,
						Value: String(record[field])
					});
				}
			});

			// Verifica se a view atual tem critérios de filtro específicos
			var oCriteriaModel = this.getView().getModel("criterias");
			if (oCriteriaModel)
			{
				var criteriasData = oCriteriaModel.getData();
				if (criteriasData && criteriasData.items)
				{
					criteriasData.items.forEach(c => {
						criteria.push(c);
					});
				}
			}

			return criteria.length > 0 ? criteria : undefined;
		},

		_loadData: function(criteria) {
			this.getView().setBusy(true);

			var ds = new Datasource(this.apiName());
			var method = this.getListMethod();
			var criteriaLocal = criteria || this._mountCriteria();

			ds.query(criteriaLocal, method).then(
				(response) => {
					if(response.success)
					{
						this._dataSet = response.data;
						this.setData(response.data);
					}
					else
					{
						this.showError("Obter dados do servidor", response.message);
					}
					this.getView().setBusy(false);
				},
				(error) => {
					this.getView().setBusy(false);
					this.showError("Obter dados do servidor", error.message);
				}
			)
		}

	});
});