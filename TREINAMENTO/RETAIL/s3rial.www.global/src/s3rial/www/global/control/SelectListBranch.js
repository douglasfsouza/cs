sap.ui.define([
    "s3rial/www/global/control/SelectList"
], function (SelectList) {
	"use strict";

	return SelectList.extend("s3rial.www.global.control.SelectListBranch", {
        fragment: "s3rial.www.global.control.SelectList",
        
        constructor: function(sId, mSettings) {
            SelectList.apply(this, arguments);

            this.setProperty("apiName", "businessPlaces");
            this.setProperty("listMethod", "getSelectList");
            this.setProperty("title", "Lista de filiais");
            this.setProperty("value", mSettings["value"]);
        }
	});
});