sap.ui.define([
    "s3rial/www/global/Enumerators"
], function (Enumerators) {
    "use strict";

    const SampleEnumerators = Enumerators.extend('s3rial.www.global.samples.Sample_05_Full.SampleEnumerators', {
        metadata: {
            library: 's3rial.www.global.samples.Sample_05_Full',
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
            0: "Criado",
            1: "Pendente",
            2: "Processado",
            99: "Erro"
        }
    } 

    return SampleEnumerators;
});