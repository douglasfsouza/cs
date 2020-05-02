sap.ui.define([
	"sap/ui/model/json/JSONModel",
    "s3rial/www/global/Mask",
    "sap/ui/core/format/DateFormat"
],
function(JSONModel, Mask, DateFormat) {
    "use strict";

    const Formatting = JSONModel.extend("s3rial.www.global.Formatting", {
        metadata: {
            library: 's3rial.www.global',
            publicMethods: ['formatZipCodeBR']
        }
    });

    var _EnumSources = {};

    Formatting.prototype.formatZipCodeBR = function (value) {
        return new Promise((resolve, fail) => {
            var result = Mask.process(value, "00000-000");
            resolve(result.valid ? result.result : value);
        });
    };

    Formatting.prototype.formatCPFCNPJ = function(value) {
        return new Promise((resolve, fail) => {
            var result = Mask.process(value, "00.000.000/0000-00");

            if (!result.valid)
            {
                result = Mask.process(value, "000.000.000-00");
            }
            
            resolve(result.valid ? result.result : value);
        });
    };

    Formatting.prototype.formatRMS7 = function(value) {
        if (!this._formatter)
        {
            this._formatter = DateFormat.getDateInstance(
                {
                    source: {
                        pattern: 'yyyy-MM-ddTHH:mm:ss'
                    },
                    pattern: 'dd/MM/yyyy'
                }                    
            );
        }

        return new Promise((resolve, fail) => {
            if (isNaN(value))
            {
                resolve("");
            }
            else
            {
                var rmsDate = parseInt(value);
                var yy = Math.trunc(rmsDate / 10000);
                var mm = Math.trunc((rmsDate - (yy * 10000)) / 100);
                var dd = (rmsDate - (yy * 10000) - (mm * 100));

                var date = new Date(yy + 1900, mm-1, dd-1);

                var result = this._formatter.format(date);

                resolve(result);
            }
        });
    };
    
    Formatting.prototype.formatEnum = function(value, enumSource, enumName) {
        return new Promise((resolve, error) => {
            var enumClass = enumSource.split(".").join("/");
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

            var translate = enumObj.translate(enumName, value);
            resolve(translate);
        });
    };

    if (!this._formatting)
    {
        this._formatting = new Formatting();
    }

    return this._formatting;
}, true);
