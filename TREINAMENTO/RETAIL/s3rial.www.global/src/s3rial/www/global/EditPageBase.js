sap.ui.define([
	"s3rial/www/global/ControllerBase",
	"s3rial/www/global/GlobalFunctions",
	"sap/ui/model/json/JSONModel"
], function(ControllerBase, Global, JSONModel) {
	"use strict";

	return ControllerBase.extend("s3rial.www.global.EditPageBase", {
		metadata: {
			library : "s3rial.www.global",
			publicMethods: [
				"configurationFile", "pageTitle", "pageSections", "canEditRecord", "formatNumber"
			]
		},

		_pageTitle: null,
		_pageSections: null,
		_recordOriginal: null,
		_pageConfig: null,
	
		pageConfig: function() {
			console.log("Method Not Implemented");
		},

		/**
		 * Retorna o título da página
		 * 
		 * Obrigatório quando configurationFile não estiver implementado
		 */
		pageTitle: function() {
			console.log("Method Not Implemented");
		},

		/**
		 * Retorna as seções da página
		 * 
		 * Obrigatório
		 */
		pageSections: function() {
			console.log("Method Not Implemented");
		},

		/**
		 * Indica se o processo de edição está ativado
		 */
		canEditRecord: function() {
			return true;
		},

		onInit: function () {
			ControllerBase.prototype.onInit.call(this);
			
			var oRouter = this.getRouter();

			if (this.targetName())
			{
				var oTarget = oRouter.getTarget(this.targetName());
				oTarget.attachDisplay(this.attachDisplay, this);
			}

			//
			// --> Obtém as configurações da página
			// 
			var configurationFile = this.configurationFile();

			//--> Obtém de um arquivo de configuração externo
			if (configurationFile)
			{
				var config = Global.GetConfPage(this.configurationFile());
				this._pageConfig = config.Pagina;
				this._pageTitle = config.Pagina.Titulo.toString();
				this._pageSections = config.Formulario;
			}
			else
			//--> Se não há um arquivo, tenta recuperar da classe estendida
			{
				this._pageConfig = this.pageConfig();
				this._pageTitle = this.pageTitle();
				this._pageSections = this.pageSections();
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
			// --> Carrega o model com dados da configuração
			//
			var oModel = new JSONModel({
				pageTitle: this._pageTitle,
				pageSubTitle: "",
				editing: false
			});

			this.getView().setModel(oModel, "layout");
		},

		attachDisplay: function(oEvent) {
			ControllerBase.prototype.attachDisplay.call(this, oEvent);

			var parameters = this.getPageParameters();

			var oModel = new JSONModel({
				pageTitle: this._pageConfig.Titulo,
				pageSubTitle: this._pageConfig.Subtitulo,
				editing: parameters.editMode === "insert" ? true : false
			});

			var buttonVisible = parameters.editMode !== "insert";
			var button = this.byId("editButton");
			if (button)
			{
				this.byId("editButton").setVisible(buttonVisible);
			}

			this.getView().setModel(oModel, "layout");
		},

		_mountDefaultLayout: function()
		{
			var layout = this.getView();

			layout.addStyleClass("sapUiSizeCompact sapUiNoMarginBegin");

			var page = this._createPage();

			layout.addContent(page);
		},

		_configureLayout: function()
		{
			//Global.addTituloNaPagina(this._pageTitle, this, false);
			this.addSections(this, this._pageSections);
		},

		_createPage: function() {
			var page = new sap.uxap.ObjectPageLayout({
				id: this.createId("editPage"),
				showTitleInHeaderContent: true,
				//useIconTabBar: true,
				upperCaseAnchorBar: false,
				showFooter: "{layout>/editing}",
				isChildPage: true
			});

			var headerTitle = this._createHeaderTitle();
			var headerContent = this._createHeaderContent();
			var toolbar = this._createFooterToolbar();

			page.setHeaderTitle(headerTitle);
			page.setFooter(toolbar);

			if (headerContent)
			{
				page.addHeaderContent(headerContent);
			}

			return page;
		},

		_createHeaderTitle: function() {
			var headerTitle = new sap.uxap.ObjectPageDynamicHeaderTitle(this.createId("headerTitle"));

			var headingBox = new sap.m.FlexBox({
				alignItems: "Start"
			});

			var buttonBox = new sap.m.FlexBox({
				direction: "Column",
				alignItems: "Center",
				justifyContent: "Start"
			});
			buttonBox.addStyleClass("sapUiNoMarginTop sapUiTinyMarginEnd");

			var titleBox =  new sap.m.FlexBox({
				direction: "Column",
				alignItems: "Start"
			});

			var title = new sap.m.Title({
				text: this._pageConfig.Titulo
			});

			var subTitle = new sap.m.Text({
				text: this._pageConfig.Subtitulo
			});
		
			var backButton = new sap.m.Button({
				icon: "sap-icon://nav-back", 
				type: "Transparent",
				press: [this.onNavBackPress, this]
			});
			backButton.addStyleClass("sapUiNoContentPadding");

			buttonBox.addItem(backButton);
			titleBox.addItem(title);

			this._pageConfig.Subtitulo && titleBox.addItem(subTitle);

			headingBox.addItem(buttonBox);
			headingBox.addItem(titleBox);

			headerTitle.setHeading(headingBox);

			if (this.canEditRecord())
			{
				headerTitle.addAction(new sap.uxap.ObjectPageHeaderActionButton(this.createId("editButton"), {
					text: "Editar",
					type: "Emphasized",
					tooltip: "Editar o registro",
					press: [this.onEditPress, this],
					enabled: "{=${layout>/editing} ? false : true}",
					hideText: false
				}));
			}

			return headerTitle;
		},

		_createHeaderContent: function() {
			var hlayout = new sap.ui.layout.HorizontalLayout({allowWrapping: true});

			var hasFields = false;
			
			this._pageConfig.BlocosDeCampos && 
			this._pageConfig.BlocosDeCampos.forEach((block) => {
				var vlayout = new sap.ui.layout.VerticalLayout();
				vlayout.addStyleClass("sapUiLargeMarginEnd");

				block.Campos.forEach((field) => {
					var formatContent = this.createFormat(field);

					switch(field.tipo.toLowerCase())	
					{
						case "attribute":
							var label = new sap.m.Label({
								id: this.createId(`header-${field.bdName}`),
								text: field.texto
							});
							
							var element = {};

							switch(field.masc.toLowerCase())
							{
								case "number":
									var element = new sap.m.ObjectNumber({
										number: formatContent
									});
									break;

								default:
									var element = new sap.m.ObjectStatus({
										text: formatContent
									});
									element.addStyleClass("sapMObjectStatusLarge");
									break;
							}

							vlayout.addContent(label);
							vlayout.addContent(element);
							break;

						default:
							var attr = new sap.m.ObjectAttribute({
								title: field.texto,
								text: formatContent
							});
							vlayout.addContent(attr);
							break;	
					}

					hasFields = true;
				});

				if (hasFields)
				{
					hlayout.addContent(vlayout);
				}

			});

			return hasFields ? hlayout : undefined;
		},

		_createFooterToolbar: function() {
			var toolbar = new sap.m.OverflowToolbar(this.createId("footerToolbar"));

			toolbar.addContent(new sap.m.ToolbarSpacer());

			toolbar.addContent(new sap.m.Button({
				type: "Accept",
				text: "Salvar",
				tooltip: "Salvar",
				width: "120px",
				press: [this.onSavePress, this]
			}));

			toolbar.addContent(new sap.m.Button({
				type: "Reject",
				text: "Cancelar",
				tooltip: "Cancelar",
				width: "120px",
				press: [this.onCancelPress, this]
			}));

			return toolbar;
		},

		onEditPress: function (oEvent) {
			var original = this.getView().getModel("record").getData();
			this._recordOriginal = Object.assign({}, original);
			this._setModel({editing: true}, "layout");
		},

		onSavePress: function (oEvent) {
			this._recordOriginal = null;
			this._setModel({editing: false}, "layout");

			var parameters = this.getPageParameters();
			var buttonVisible = parameters.editMode !== "insert";
			var button = this.byId("editButton");
			if (button)
			{
				this.byId("editButton").setVisible(buttonVisible);
			}
		},

		onCancelPress: function (oEvent) {
			var oModel = new JSONModel(this._recordOriginal);
			this.getView().setModel(oModel, "record");
			this._setModel({editing: false}, "layout");

			var parameters = this.getPageParameters();
			if (parameters.editMode === "insert")
			{
				this.onNavBackPress(undefined);
			}
		},

		onNavBackPress: function (oEvent) {
			var parameters = this.getPageParameters();	

			// in some cases we could display a certain target when the back button is pressed
			if (parameters && parameters.fromTarget) {
				this.getRouter().getTargets().display(parameters.fromTarget);
				return;
			}
		},

		_setModel: function(content, name)
		{
			var oModel = this.getView().getModel(name);
			var oData = oModel.getData();

			var newData = Object.assign(oData, content);
			oModel.setData(newData);

			this.getView().setModel(oModel, name);
		},

		addSections: function (form, sections) {
			sections.forEach((section) => {
				var page = this.byId("editPage");

				//criar Section
				var pageSection = new sap.uxap.ObjectPageSection({
					title: section.Nome,
					titleUppercase: false
				});
				
				page.addSection(pageSection);

				//criar subsection
				var pageSubSection = new sap.uxap.ObjectPageSubSection({
					title: section.Nome
				});

				pageSection.addSubSection(pageSubSection);

				if (section.Dados) 
				{
					this.addSectionDados(pageSubSection, section.Dados);
				}
				else if (section.Grid)
				{
					this.addSectionGrid(pageSubSection, section.Grid);
				}
			});
		},

		_onTableToolbarClickedInternal: function(oEvent) {
			var buttonId = oEvent.getSource().getId();
			this.onTableToolbarClicked(buttonId);
		},

		onTableToolbarClicked: function (buttonId) {
			console.log("onTableToolbarClicked - Not Implemented");
		},

		onTableSelectionChanged: function(sender, selectedRows) {
			console.log("onSelectionChanged - Method Not Implemented");
		},

		_onResponsiveSelectionChange: function(oEvent) {
			var sender = oEvent.getSource().getId();
			var contexts = oEvent.getSource()
								 .getSelectedContexts();
								 
			var list = contexts.map((context) => {
				return context.getObject(context.getPath());
			});

			this.onTableSelectionChanged(sender, list);
		},

		_onGridSelectionChange: function(oEvent) {
			var sender = oEvent.getSource().getId();
			var table = this.byId(sender);
			var indexes = table.getSelectedIndices();

			var list = indexes.map((index) => {
				var context = table.getContextByIndex(index);
				return context.getObject(context.getPath()); 
			});

			this.onTableSelectionChanged(sender, list);
		},

		addSectionGrid: function(pageSubSection, config) {
			var tableConfig  = config.tableConfig;

			if (tableConfig.Buttons && tableConfig.Buttons.length != 0)
			{
				tableConfig.toolbarName = `${config.bdName}_toolbar`;
				tableConfig.onToolbarClick = [this._onTableToolbarClickedInternal, this];
			}

			switch(tableConfig.Tipo)
			{
				case this.TABLE_RESPONSIVE:
					tableConfig.onSelectionChange = [this._onResponsiveSelectionChange, this];
					break;
				
				case this.TABLE_GRID:
					tableConfig.onSelectionChange = [this._onGridSelectionChange, this];
					break;
			}

			var elemento = this.createTable(config.bdName, config.tableConfig);
			elemento.bindRows(`record>/${config.bdName}/`);
			pageSubSection.addBlock(elemento);
		},

		addSectionDados: function(pageSubSection, config)
		{
			config.forEach((content) => {
				var simpleForm = new sap.ui.layout.form.SimpleForm({
					title: content.Nome,
					editable: this.canEditRecord(),
					//layout:"ResponsiveGridLayout",
					layout:"ColumnLayout",
					singleContainerFullSize: false,

					adjustLabelSpan: false,

					labelSpanXL: 12,
					labelSpanL: 12,
					labelSpanM: 12,
					labelSpanS: 12,

					emptySpanXL:1,
					emptySpanL:1,
					emptySpanM:1,
					emptySpanS:0,

					columnsXL:4,
					columnsL:3,
					columnsM:2
				});

				simpleForm.addStyleClass("sapUiSizeCompact");

				if(content.Colunas && Array.isArray(content.Colunas))
				{
					if(content.Colunas.length >= 1)
					{
						simpleForm.setColumnsXL(content.Colunas[0]);
					}
					if(content.Colunas.length >= 2)
					{
						simpleForm.setColumnsL(content.Colunas[1]);
					}
					if(content.Colunas.length >= 3)
					{
						simpleForm.setColumnsM(content.Colunas[2]);
					}
				}

				if(content.CelulasVazias && Array.isArray(content.CelulasVazias))
				{
					if(content.CelulasVazias.length >= 1)
					{
						simpleForm.setEmptySpanXL(content.CelulasVazias[0]);
					}
					if(content.CelulasVazias.length >= 2)
					{
						simpleForm.setEmptySpanL(content.CelulasVazias[1]);
					}
					if(content.CelulasVazias.length >= 3)
					{
						simpleForm.setEmptySpanM(content.CelulasVazias[2]);
					}
				}

				//var vbox = new sap.m.VBox();
				//vbox.addItem(simpleForm);
				//pageSubSection.addBlock(vbox);
				pageSubSection.addBlock(simpleForm);

				content.Detalhe.forEach((group) => {
					if (group.Grupo.trim() !== "")
					{
						var form = new sap.ui.core.Title({
							text: group.Grupo
						});
						simpleForm.addContent(form);
					}

					group.Campos.forEach((field) => {
						var elemento = undefined;

						if (field.texto) {
							simpleForm.addContent(
								new sap.m.Label({
									text: field.texto
								})
							);
						}
	
						switch (field.tipo) {
							case 'input':
								field.noAlign = true;

								var editElement = this.createInputEl(`edit_${field.bdName}`, field);
								//var dispElement = this.createDisplayEl(`disp_${field.bdName}`, field);

								//elemento = new sap.m.VBox();
								//elemento.addItem(editElement);
								//elemento.addItem(dispElement);
								//simpleForm.addContent(elemento);

								simpleForm.addContent(editElement);
								//simpleForm.addContent(dispElement);

								break;
								
							case 'selectlist':
								field.noAlign = true;
								var elemento = this.createSelectListEl(`edit_${field.bdName}`, field);
								simpleForm.addContent(elemento);
								break;

							case 'text':
								field.noAlign = true;
								elemento = this.createTextEl(`disp_${field.bdName}`, field);
								simpleForm.addContent(elemento);
								break;

							case 'combobox':
								field.noAlign = true;
								elemento = this.createComboBoxEl(`disp_${field.bdName}`, field);
								simpleForm.addContent(elemento);
								break;

							case 'switch':
								field.noAlign = true;
								elemento = this.createSwitchEl(`disp_${field.bdName}`, field);
								simpleForm.addContent(elemento);
								break;

							default:
								this.showMessage("Configurando página", "Tipo de objeto não mapeado");
						}

					});

				});
			});

		}
		
	});

});