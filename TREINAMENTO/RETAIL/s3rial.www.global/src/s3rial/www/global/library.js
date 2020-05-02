/*
 * ${copyright}
 */

sap.ui.define([], function() {
    'use strict';
  
    /**
     * OpenUI5 library: s3rial.www.global
     *
     * @namespace
     * @name s3rial.www.global
     * @author marcelo.cuin
     * @version ${version}
     * @public
     */
    return sap.ui.getCore().initLibrary({
      name: 's3rial.www.global',
      dependencies: [
        'sap.f',
        'sap.m',
        'sap.tnt',
        'sap.ui.core',
        'sap.ui.layout',
        'sap.ui.table'
      ],
      controls: [
        's3rial.www.global.GlobalFunctions',
        's3rial.www.global.Formatting',
        's3rial.www.global.ControllerBase',
        's3rial.www.global.Enumerators',
        's3rial.www.global.ListPageBase',
        's3rial.www.global.ComponentBase', 
        's3rial.www.global.DatasourceBase',
        's3rial.www.global.Mask'
      ],
      noLibraryCSS: true,
      version: '${version}',
    });
  });