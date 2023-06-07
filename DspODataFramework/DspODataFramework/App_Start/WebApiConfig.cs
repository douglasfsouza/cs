using DspODataFramework.infra.attributes;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.OData.Batch;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData;
using DspODataFramework.Controllers;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm.Library.Values;
using System.Web.Http.OData.Routing.Conventions;
using DspODataFramework.infra;
using System.Reflection.Emit;

namespace DspODataFramework
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Serviços e configuração da API da Web

            // Rotas da API da Web
            config.MapHttpAttributeRoutes();

            var cors_allow_origins = ConfigurationManager.AppSettings["cors_allow_origins"];

            var cors = new EnableCorsAttribute(cors_allow_origins, "*", "*");

            cors.SupportsCredentials = true;

            config.EnableCors(cors);

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            // Create the default collection of built-in conventions.
            var conventions = ODataRoutingConventions.CreateDefault();
            // Insert the custom convention at the start of the collection.
            conventions.Insert(0, new CustomRoutingConvention());

            config.MessageHandlers.Insert(0, new JavascriptDateHandler());
            //config.MessageHandlers.Add(new ServiceLoginHandler());
            config.MessageHandlers.Add(new FormatSupportHandler());

            config.Formatters.Add(new TextMediaTypeFormatter());

            var edmModel = WebApiConfig.InitializeODataModel();
            var odataBatchHandler = new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer);


            config.Routes.MapODataServiceRoute(
            routeName: "ODataRoute",
                routePrefix: "v1",
                model: edmModel,
                batchHandler: odataBatchHandler,
                routingConventions: conventions,
                pathHandler: new DefaultODataPathHandler()
            );

            config.AddODataQueryFilter();
        }

        private static IEdmModel InitializeODataModel()
        {
            Version odataVersion2 = new Version(2, 0);

            var builder = new ODataConventionModelBuilder();
            builder.DataServiceVersion = odataVersion2;
            builder.MaxDataServiceVersion = odataVersion2;
            builder.Namespace = "DspODataFramework.Models";

            var controllers = ListControllers();

            foreach (var c in controllers)
            {
                var attributes = c.GetCustomAttributes<ODataEntitySet>();
                var listType = typeof(List<>);

                foreach (var a in attributes)
                {
                    EntityTypeConfiguration entityType = builder.AddEntity(a.EntityType);
                    var entitySet = builder.AddEntitySet(a.EntitySetName, entityType);

                    var props = a.EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(m => m.PropertyType.Name == listType.Name);
                    if (props != null)
                    {
                        foreach (var p in props)
                        {
                            entitySet.EntityType.RemoveProperty(p);
                        }
                    }

                    AddCountMethod(a.EntityType, builder);

                    var methods = c.GetMethods().Where(m => m.GetCustomAttribute<HttpPostAttribute>() != null).ToList();
                    var puts = c.GetMethods().Where(m => m.GetCustomAttribute<HttpPutAttribute>() != null).ToList();
                    var gets = c.GetMethods().Where(m => m.GetCustomAttribute<HttpGetAttribute>() != null).ToList();

                    methods.AddRange(puts);
                    methods.AddRange(gets);

                    if (methods != null)
                    {
                        foreach (var m in methods)
                        {
                            AddExternalMethod(a.EntityType, builder, m);
                        }
                    }
                }
            }

            //var action = builder.Entity<DeliveryListItem>()
            //       .Collection
            //       .Action("GetItemToCheck")
            //       .ReturnsCollectionFromEntitySet<DeliveryListItem>("DeliveryListItems");

            //action.Parameter<string>("key");
            //action.Parameter<string>("barcode");

            IEdmModel edmModel = builder.GetEdmModel();
            edmModel.SetEdmVersion(odataVersion2);
            edmModel.SetEdmxVersion(odataVersion2);

            Dictionary<string, string> namespaces = new Dictionary<string, string>();
            namespaces["sap"] = "http://www.sap.com/Protocols/SAPData";
            namespaces["m"] = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
            edmModel.SetNamespacePrefixMappings(namespaces);

            //
            // Inclui anotations definidas por atributos
            //
            var stringType = EdmCoreModel.Instance.GetString(true);

            foreach (IEdmSchemaElement element in edmModel.SchemaElements)
            {
                EdmEntityType entityType = element as EdmEntityType;

                if (entityType != null)
                {
                    string entityName = entityType.FullName();

                    Type type = FindEntity(entityName);

                    SetSAPODataAnnotations(edmModel, entityType, type, namespaces["sap"]);

                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var p in properties)
                    {
                        EdmStructuralProperty property = entityType.FindProperty(p.Name) as EdmStructuralProperty;
                        if (property != null)
                        {
                            SetODataAnnotations(edmModel, property, p, namespaces["sap"]);
                            SetSAPODataAnnotations(edmModel, property, p, namespaces["sap"]);
                        }
                    }
                }
            }

            return edmModel;
        }

        private static List<Type> ListControllers()
        {
            List<Type> result = new List<Type>();

            string baseNameSpace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;

            Type entity = typeof(ODataController);

            var types = Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(i => (i.FullName.StartsWith(baseNameSpace) &&
                                             i.BaseType?.Name == entity.Name));

            result = types.ToList();

            entity = typeof(ControllerBase<>);

            var typesInherited = Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(i => (i.FullName.StartsWith(baseNameSpace) &&
                                             i.BaseType?.Name == entity.Name));


            result.AddRange(typesInherited.ToList());

            return result.OrderBy(m => m.Name).ToList();
        }

        private static void AddCountMethod(Type type, ODataConventionModelBuilder builder)
        {
            var entityMethod = builder.GetType().GetMethod("Entity").MakeGenericMethod(new Type[] { type });
            var entity = entityMethod.Invoke(builder, null);

            var collectionProperty = entity.GetType().GetProperty("Collection");
            var collection = collectionProperty.GetValue(entity);

            var actionMethod = collection.GetType().GetMethod("Action");

            ActionConfiguration count = (ActionConfiguration)actionMethod.Invoke(collection, new object[] { "$count" });
            count.Parameter<string>("$filter");
            count.Returns<long>();
        }

        private static void AddExternalMethod(Type type, ODataConventionModelBuilder builder, MethodInfo method)
        {
            bool methodForRecord = false;

            ActionNameAttribute actionName = method.GetCustomAttribute<ActionNameAttribute>();

            if (actionName == null || actionName.Name.ToLowerInvariant().Equals("$count"))
            {
                return;
            }

            var entityMethod = builder.GetType().GetMethod("Entity").MakeGenericMethod(new Type[] { type });
            var entity = entityMethod.Invoke(builder, null);

            var collectionProperty = entity.GetType().GetProperty("Collection");
            var collection = collectionProperty.GetValue(entity);

            var actionMethod = collection.GetType().GetMethod("Action");

            ActionConfiguration exMethod = (ActionConfiguration)actionMethod.Invoke(collection, new object[] { actionName.Name });

            foreach (var p in method.GetParameters())
            {
                if (!methodForRecord)
                {
                    var odataAttr = p.GetCustomAttribute<FromODataUriAttribute>();

                    methodForRecord = odataAttr != null;
                }

                var parameterMethod = exMethod.GetType().GetMethod("Parameter").MakeGenericMethod(new Type[] { p.ParameterType });
                ParameterConfiguration parameter = (ParameterConfiguration)parameterMethod.Invoke(exMethod, new object[] { p.Name });
            }

            if (actionName.Name.StartsWith("$$") || methodForRecord)
            {
                collection = entity;
                actionMethod = entity.GetType().GetMethod("Action");
            }
            else
            {
                collectionProperty = entity.GetType().GetProperty("Collection");
                collection = collectionProperty.GetValue(entity);
                actionMethod = collection.GetType().GetMethod("Action");
            }

            MethodInfo returnsMethod = null;
            string entitySetName = string.Empty;

            returnsMethod = exMethod.GetType().GetMethod("Returns").MakeGenericMethod(new Type[] { method.ReturnType });

            if (method.ReturnType.GenericTypeArguments != null && method.ReturnType.GenericTypeArguments.Count() != 0)
            {
                var returnType = method.ReturnType;

                if (method.ReturnType.GenericTypeArguments[0].Namespace == type.Namespace)
                {
                    returnsMethod = exMethod.GetType()
                                            .GetMethods()
                                            .Where(m => m.Name == "ReturnsCollectionFromEntitySet" &&
                                                        m.GetParameters().FirstOrDefault().ParameterType == typeof(string))
                                            .First()
                                            .MakeGenericMethod(new Type[] { type });

                    EntitySetConfiguration exSet = builder.EntitySets.FirstOrDefault(m => m.EntityType.Name == type.Name);
                    exMethod.EntitySet = exSet;

                    returnsMethod.Invoke(exMethod, new object[] { exSet.Name });
                }
            }
            else
            {
                returnsMethod.Invoke(exMethod, null);
            }


            //exMethod.Returns<string>();
        }

        private static Type FindEntity(string entityName)
        {
            string[] name = Assembly.GetExecutingAssembly().GetName().Name.Split('.');
            name = name.Take(name.Length - 1).ToArray();
            string ns = string.Join(".", name);

            var domain = AppDomain.CurrentDomain;

            var type = domain.GetAssemblies()
                            .Where(a => a.GetName().Name.StartsWith(ns))
                            .SelectMany(f => f.GetTypes())
                            .Where(t => t.FullName == entityName)
                            .FirstOrDefault();

            return type;
        }

        private static void SetSAPODataAnnotations(IEdmModel edmModel, EdmEntityType entityType, Type type, string ns)
        {
            var attrSAP = type.GetCustomAttribute<SAPODataEntityTypeAttribute>();
            var attrOData = type.GetCustomAttribute<DgDataTableAttribute>();
            var attrHierarchy = type.GetCustomAttribute<SAPODataHierarchyAttribute>();

            var stringType = EdmCoreModel.Instance.GetString(true);

          

            string label = attrOData?.LogicalName ?? attrSAP?.Label;
            int semanticsId = (int)DgDataTableAttribute.SemanticsEnum.None;

            if (attrOData != null)
            {
                semanticsId = (int)attrOData.Semantics;
            }
            else if (attrSAP != null)
            {
                semanticsId = (int)attrSAP.Semantics;
            }

            bool hasAttr = attrSAP != null || attrOData != null;

            if (hasAttr)
            {
                string name = string.Empty;
                EdmStringConstant value = null;

                if (!string.IsNullOrWhiteSpace(label))
                {
                    name = "label";
                    value = new EdmStringConstant(stringType, label);
                    edmModel.SetAnnotationValue(entityType, ns, name, value);
                }

                if (semanticsId != 0)
                {
                    name = "semantics";
                    switch (semanticsId)
                    {
                        case (int)SAPODataEntityTypeAttribute.SemanticsEnum.VCard:
                            value = new EdmStringConstant(stringType, "vcard");
                            break;
                        case (int)SAPODataEntityTypeAttribute.SemanticsEnum.VEvent:
                            value = new EdmStringConstant(stringType, "vevent");
                            break;
                        case (int)SAPODataEntityTypeAttribute.SemanticsEnum.VTodo:
                            value = new EdmStringConstant(stringType, "vtodo");
                            break;
                        case (int)SAPODataEntityTypeAttribute.SemanticsEnum.Parameters:
                            value = new EdmStringConstant(stringType, "parameters");
                            break;
                        case (int)SAPODataEntityTypeAttribute.SemanticsEnum.Aggregate:
                            value = new EdmStringConstant(stringType, "aggregate");
                            break;
                        case (int)SAPODataEntityTypeAttribute.SemanticsEnum.Variant:
                            value = new EdmStringConstant(stringType, "variant");
                            break;
                    }
                    edmModel.SetAnnotationValue(entityType, ns, name, value);
                }
            }
        }

        private static void SetODataAnnotations(IEdmModel edmModel, EdmStructuralProperty property, PropertyInfo propertyInfo, string ns)
        {
            var stringType = EdmCoreModel.Instance.GetString(true);

            string[] stringTypesToAllowMaxLength = new string[]
            {
                typeof(string).Name
            };

            bool canAnnotateMaxLength = stringTypesToAllowMaxLength.Contains(propertyInfo.PropertyType.Name);

            var stringAttr = propertyInfo.GetCustomAttribute<ODataStringProperty>();
            if (stringAttr != null)
            {
                EdmStringConstant value;

                if (canAnnotateMaxLength)
                {
                    long maxLength = stringAttr.MaxLength > 0 ? stringAttr.MaxLength : 1000;
                    value = new EdmStringConstant(stringType, maxLength.ToString());
                    edmModel.SetAnnotationValue(property, "", "MaxLength", value);
                }

                if (!string.IsNullOrWhiteSpace(stringAttr.Tooltip))
                {
                    string name = "quickinfo";
                    value = new EdmStringConstant(stringType, stringAttr.Tooltip);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (!string.IsNullOrWhiteSpace(stringAttr.Label))
                {
                    string name = "label";
                    value = new EdmStringConstant(stringType, stringAttr.Label);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
            }

            string[] numericTypesToAllowPrecision = new string[]
            {
                typeof(decimal).Name
            };

            bool canAnnotatePrecision = numericTypesToAllowPrecision.Contains(propertyInfo.PropertyType.Name);

            var numericAttr = propertyInfo.GetCustomAttribute<ODataNumericProperty>();
            if (numericAttr != null && numericAttr.Precision != 0)
            {
                if (canAnnotatePrecision)
                {
                    var precision = new EdmStringConstant(stringType, numericAttr.Precision.ToString());
                    var scale = new EdmStringConstant(stringType, numericAttr.Scale.ToString());

                    edmModel.SetAnnotationValue(property, "", "Precision", precision);
                    edmModel.SetAnnotationValue(property, "", "Scale", scale);
                }

                if (!string.IsNullOrWhiteSpace(numericAttr.Tooltip))
                {
                    string name = "quickinfo";
                    var value = new EdmStringConstant(stringType, numericAttr.Tooltip);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (!string.IsNullOrWhiteSpace(numericAttr.Label))
                {
                    string name = "label";
                    var value = new EdmStringConstant(stringType, numericAttr.Label);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
            }

            var dateAttr = propertyInfo.GetCustomAttribute<ODataDateProperty>();
            if (dateAttr != null)
            {
                if (!string.IsNullOrWhiteSpace(dateAttr.Label))
                {
                    string name = "label";
                    var value = new EdmStringConstant(stringType, dateAttr.Label);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (!string.IsNullOrWhiteSpace(dateAttr.Tooltip))
                {
                    string name = "quickinfo";
                    var value = new EdmStringConstant(stringType, dateAttr.Tooltip);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
            }
        }

        private static void SetSAPODataAnnotations(IEdmModel edmModel, EdmStructuralProperty property, PropertyInfo propertyInfo, string ns)
        {
            var attr = propertyInfo.GetCustomAttribute<SAPODataPropertyAttribute>();
            var stringType = EdmCoreModel.Instance.GetString(true);

            if (attr != null)
            {
                string name = string.Empty;
                EdmStringConstant value = null;

                if (attr.DisplayFormat != null && attr.DisplayFormat != string.Empty)
                {
                    name = "display-format";
                    value = new EdmStringConstant(stringType, attr.DisplayFormat);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (attr.AgregationRole != SAPODataPropertyAttribute.AgregationRoleEnum.None)
                {
                    name = "aggregation-role";
                    switch (attr.AgregationRole)
                    {
                        case SAPODataPropertyAttribute.AgregationRoleEnum.Dimension:
                            value = new EdmStringConstant(stringType, "dimension");
                            break;
                        case SAPODataPropertyAttribute.AgregationRoleEnum.Measure:
                            value = new EdmStringConstant(stringType, "measure");
                            break;
                    }
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (!attr.Creatable)
                {
                    name = "creatable";
                    value = new EdmStringConstant(stringType, "false");
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (!attr.Updatable)
                {
                    name = "updatable";
                    value = new EdmStringConstant(stringType, "false");
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (!attr.Filterable)
                {
                    name = "filterable";
                    value = new EdmStringConstant(stringType, "false");
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (!attr.Sortable)
                {
                    name = "sortable";
                    value = new EdmStringConstant(stringType, "false");
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (attr.Semantics != null && attr.Semantics.Trim() != string.Empty)
                {
                    name = "semantics";
                    value = new EdmStringConstant(stringType, attr.Semantics);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (attr.Tooltip != null && attr.Tooltip.Trim() != string.Empty)
                {
                    name = "quickinfo";
                    value = new EdmStringConstant(stringType, attr.Tooltip);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (attr.Label != null && attr.Label.Trim() != string.Empty)
                {
                    name = "label";
                    value = new EdmStringConstant(stringType, attr.Label);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (attr.Header != null && attr.Header.Trim() != string.Empty)
                {
                    name = "heading";
                    value = new EdmStringConstant(stringType, attr.Header);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }

                if (attr.FilterRestriction != null && attr.FilterRestriction.Trim() != string.Empty)
                {
                    name = "filter-restriction";
                    value = new EdmStringConstant(stringType, attr.FilterRestriction);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
            }

            var hierAttr = propertyInfo.GetCustomAttribute<SAPODataHierarchyAttribute>();
            // Tratamento de hierarquia
            if (hierAttr != null)
            {
                string name = string.Empty;
                EdmStringConstant value = null;

                if (hierAttr.IsKey)
                {
                    name = "hierarchy-node-for";
                    value = new EdmStringConstant(stringType, propertyInfo.Name);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
                else if (!string.IsNullOrWhiteSpace(hierAttr.IsDrillDownFor))
                {
                    name = "hierarchy-drill-state-for";
                    value = new EdmStringConstant(stringType, hierAttr.IsDrillDownFor);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
                else if (!string.IsNullOrWhiteSpace(hierAttr.IsLevelIndicatorFor))
                {
                    name = "hierarchy-level-for";
                    value = new EdmStringConstant(stringType, hierAttr.IsLevelIndicatorFor);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
                else if (!string.IsNullOrWhiteSpace(hierAttr.IsParentFor))
                {
                    name = "hierarchy-parent-node-for";
                    value = new EdmStringConstant(stringType, hierAttr.IsParentFor);
                    edmModel.SetAnnotationValue(property, ns, name, value);
                }
            }
        }



    }
}
