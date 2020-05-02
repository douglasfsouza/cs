sap.ui.define([
    "s3rial/www/global/Enumerators"
], function (Enumerators) {
    "use strict";

    const SampleEnumerators = Enumerators.extend('varsis.www.apinvoices.Enumerators', {
        metadata: {
            library: 'varsis.www.apinvoices',
            publicMethods: ["translate", "list"]
        }
    });

    SampleEnumerators.prototype.getEnumType = function (enumName) {
        var result = {};

        switch(enumName.toLowerCase())
        {
            case "tipoentidade":
                result = this.TipoEntidadeEnum();
                break;
            case "integrationstatus":
                result = this.IntegrationStatusEnum();
                break;
        }

        return result;
    };

    SampleEnumerators.prototype.TipoEntidadeEnum = function () {
        return {
            "L": "Loja",
            "F": "Fornecedor",
            "C": "Cliente",
            "R": "Representante",
            "T": "Transportador"
        }
    };

    SampleEnumerators.prototype.IntegrationStatusEnum = function () {
        return {
            1: "Pendente",
            2: "Processado",
            99: "Erro"
        }
    } 

    return SampleEnumerators;
});