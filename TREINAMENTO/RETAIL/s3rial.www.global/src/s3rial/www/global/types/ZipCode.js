sap.ui.define([
	"sap/ui/model/SimpleType",
    "s3rial/www/global/Mask"
], function (SimpleType, Mask) {
	"use strict";
	return SimpleType.extend("s3rial.www.global.types.ZipCode", {

		formatValue: function (value) {
			var result = Mask.process(value, "99999-999");

            return result.valid ? result.result : "";
		},

		parseValue: function (value) {
			/*
			var newValue = String(value).replace(/\./g, "")
										.replace(/[a-z]/gi, "")
										.replace(/\-/g, "");
			return newValue;
			*/
			return value
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