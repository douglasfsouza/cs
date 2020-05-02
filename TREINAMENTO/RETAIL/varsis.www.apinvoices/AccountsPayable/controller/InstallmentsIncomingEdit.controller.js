sap.ui.define([
	's3rial/www/global/EditPageBase',
	"sap/ui/model/json/JSONModel",
	's3rial/www/global/DatasourceBase',
	'varsis/www/apinvoices/Enumerators'
], function(EditPageBase, JSONModel, Datasource) {
	"use strict";

    const THIS_DOMAIN = "varsis/www/ipinvoices";
    const STATE_NAME = "my_state";

	return EditPageBase.extend(`${THIS_DOMAIN}.controller.InstallmentsIncomingEdit`, {
		_datasource: null,
		_modified: false,
		_grid: null,

		_emptyRecord: {
		},

		configurationFile: function() {
			return `${THIS_DOMAIN}/view/InstallmentsIncomingEdit.view.json`;
		},

		targetName: function() {
			return "installmentIncomingEdit";
		},

		canEditRecord: function() {
			return false;
		},

		onInit: function () {
            EditPageBase.prototype.onInit.call(this);
            
            this._datasource = new Datasource("IPInvoice");

            this._configurePage();
        },

        _initializeState: function () {
            var state = {
                "insert": false,
                "editing": false,
                "recordLoaded": false,
			};
			
            var modelState = new JSONModel(state);
            this.getView().setModel(modelState, STATE_NAME);
        },
        
        _updateModel: function(model) {
            var modelData = this.getView().getModel(STATE_NAME).getData();

            Object.keys(model).forEach((key) => {
                modelData[key] = model[key];
            });

            var newModel = new JSONModel(modelData);
            this.getView().setModel(newModel, STATE_NAME);
        },

        _configurePage: function() {
            // nothing to do here
        },

		attachDisplay: function(oEvent) {
            EditPageBase.prototype.attachDisplay.call(this, oEvent);
            
            this._initializeState();

			var emptyRecord = Object.assign({}, this.emptyRecord);

			var oModel = new JSONModel(emptyRecord);

			this.getView().setModel(oModel, "record");

			var parameters = this.getPageParameters();

			if (parameters.editMode !==  "insert")
			{
				var callModel = new JSONModel(parameters.record);
				this.getView().setModel(callModel, "caller");
				this._loadRecord(parameters.record.code);
                this._updateModel({"insert": false});
			}
			else
			{
				var callModel = new JSONModel({});
				this.getView().setModel(callModel, "caller");
                this._updateModel({"insert": true});
			}
		},

        _getModel: function () {
            var oData = this.getView().getModel(STATE_NAME).getData();
            return oData;
        },

		onNavBackPress: function (oEvent) {
			var parameters = this.getPageParameters();	

			if (this._modified && parameters && parameters.fromTarget) {
				if (parameters.editMode === "insert")
				{
					this.getRouter().getTargets().display(parameters.fromTarget, { recordKey: "***" });
				}
				else
				{
					this.getRouter().getTargets().display(parameters.fromTarget, { recordKey: parameters.recordKey });
				}
			}
			else
			{
				EditPageBase.prototype.onNavBackPress.call(this, oEvent);
			}
		},
		
		_loadRecord: function(recordKey) {
            this.getView().setBusy(true);
            
            this._updateModel({"recordLoaded": false});

            var criterias = [];
            criterias.push({
                "Field": "Code",
                "Operator": "eq",
                "Value": recordKey
            });

			this._datasource.query(criterias, 'listSummary').then(
				(response) => {
					if(response.success)
					{
						var oModel = new JSONModel(response.data[0]);
                        this.getView().setModel(oModel, "record");
						this._updateModel({"recordLoaded": true});
						
						this._loadWithHolding(response.data[0].code);
						this._loadSettlement(response.data[0].code);
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
		},

		_loadWithHolding: function(recordKey) {
			var table = this.getView().byId("withHolding");

			table.setBusy(true);
            
            this._updateModel({"withHoldingLoaded": false});

            var criterias = [];
            criterias.push({
                "Field": "titulo",
                "Operator": "eq",
                "Value": recordKey
			});
			
			var ds = new Datasource("IPInvoiceTax");

			ds.query(criterias).then(
				(response) => {
					if(response.success)
					{
                        var model = this.getView().getModel("record");
                        var data = model.getData();
                        data["withHolding"] = response.data;
                        model.setData(data);

                        this.getView().setModel(model, "record")

						this._updateModel({"withHoldingLoaded": true});
					}
					else
					{
						this.showError("Obter dados de retenção de impostos", response.message);
                    }
                    
					table.setBusy(false);
				},
				(error) => {
					table.setBusy(false);
					this.showError("Obter dados de retenção de impostos", error.message);
				}
			)
		},

		_loadSettlement: function(recordKey) {
			var table = this.getView().byId("settlement");
			
			table.setBusy(true);
            
            this._updateModel({"settlementIncomingLoaded": false});

            var criterias = [];
            criterias.push({
                "Field": "tituloPagar",
                "Operator": "eq",
                "Value": recordKey
			});
			
			var ds = new Datasource("FinancialSettlementIncoming");

			ds.query(criterias).then(
				(response) => {
					if(response.success)
					{
                        var model = this.getView().getModel("record");
                        var data = model.getData();
                        data["settlement"] = response.data;
                        model.setData(data);

                        this.getView().setModel(model, "record")

						this._updateModel({"settlementIncomingLoaded": true});
					}
					else
					{
						this.showError("Obter dados de abatimento", response.message);
					}
					
					table.setBusy(false);
				},
				(error) => {
					table.setBusy(false);
					this.showError("Obter dados de abatimento", error.message);
				}
			)
		}

	});
});