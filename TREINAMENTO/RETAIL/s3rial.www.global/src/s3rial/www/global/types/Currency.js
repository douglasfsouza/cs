sap.ui.define([
	"sap/ui/model/SimpleType"
], function (SimpleType) {
	"use strict";
	return SimpleType.extend("s3rial.www.global.types.Currency", {

		formatValue: function (value) {
			var formatter = new Intl.NumberFormat('pt-BR', {
				style: 'decimal',
				useGrouping: true,
				minimumFractionDigits: 2,
				maximumFractionDigits: 2
			});

			if (isNaN(value))
			{
				value = 0;	
			}

			return formatter.format(value);
		},

		parseValue: function (value) {
			var newValue = String(value).replace(/\./g, "").replace(/,/,".");
			return parseFloat(newValue);
		},
		
		/**
		 * Validates the value to be parsed
		 *
		 * @public
		 * Since there is only true and false, no client side validation is required
		 * @returns {boolean} true
		 */
		validateValue: function () {
			return true;
		}

	});
});