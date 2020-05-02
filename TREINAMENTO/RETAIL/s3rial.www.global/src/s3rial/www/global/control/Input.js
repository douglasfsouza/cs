sap.ui.define([
    "sap/m/Input",
    "sap/ui/core/IconPool",
    "sap/m/InputBaseRenderer"
], function (Input, IconPool) {
	"use strict";

	return Input.extend("s3rial.www.global.control.Input", {
        metadata: {
			properties: {
				icon: { type : "string", defaultValue: "filter"}
			}
        },

        init: function() {
            Input.prototype.init.apply(this, arguments);
        },

        onBeforeRendering: function() {
            if (!this._iconCreated)
            {
                const icon = this.addEndIcon({
                    id: this.getId() + "-icon",
                    src: IconPool.getIconURI(this.getProperty("icon")),
                    decorative: true
                    //tooltip: "Information",
                    //press: this.onEndButtonPress.bind(this),
                }); 

                this._iconCreated = true;
            }
        },

        renderer: "sap.m.InputBaseRenderer",

	});

});