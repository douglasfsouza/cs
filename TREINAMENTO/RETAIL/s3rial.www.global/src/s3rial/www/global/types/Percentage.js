sap.ui.define([
	"sap/ui/model/SimpleType"
], function (SimpleType) {
	"use strict";
	return SimpleType.extend("s3rial.www.global.types.Percentage", {

		formatValue: function (value) {
			var formatter = new Intl.NumberFormat('pt-BR', {
				style: 'percent',
				useGrouping: true,
				minimumFractionDigits: 2,
				maximumFractionDigits: 2
			});

			if (isNaN(value))
			{
				value = 0;	
			}

			return formatter.format(value/100);
		},

		parseValue: function (value) {
			console.log(`parseValue antes: ${value}`);

			var newValue = String(value).replace(/%/g, "").replace(/\./g, "").replace(/,/,".");

			console.log(`parseValue depois: ${newValue}`);

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