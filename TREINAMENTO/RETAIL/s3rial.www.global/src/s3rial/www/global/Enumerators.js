sap.ui.define([
    "sap/ui/model/json/JSONModel"
], function (JSONModel) {
    "use strict";

    const Enumerators = JSONModel.extend('s3rial.www.global.Enumerators', {
        metadata: {
            library: 's3rial.www.global',
            publicMethods: ["translate", "list"]
        }
    });

    Enumerators.prototype.translate = function(enumName, value) {
        var enumType = this.getEnumType(enumName);
        return enumType ? enumType[value] : undefined;
    };

    Enumerators.prototype.list = function(enumName) {
        var enumType = this.getEnumType(enumName);

        if (enumType)
        {
            var keys = Object.keys(enumType);
            var list = keys.map((key) => {
                return {key: key, value: enumType[key]}
            });

            return list;
        }
        
        return undefined;
    };

    Enumerators.prototype.getEnumType = function (enumName) {
        console.log("Method not implemented");
    };

    return Enumerators;
});