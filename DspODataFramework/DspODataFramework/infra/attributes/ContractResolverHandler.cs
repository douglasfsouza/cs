using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
{
    public class ContractResolverHandler : DefaultContractResolver
    {
        private List<string> _ignoreList;
        private List<PropertyInfo> _propertyInfos;
        private Type _modelType;
        private ODataTableAttribute _tableAttribute;
        readonly List<string> _defaultFields;

        public List<string> IgnoreList
        {
            get => _ignoreList;
            set => _ignoreList = value;
        }

        public Type ModelType
        {
            get => _modelType;
            set
            {
                _modelType = value;
                if (_modelType != null)
                {
                    _tableAttribute = _modelType.GetCustomAttribute<ODataTableAttribute>();
                    _propertyInfos = _modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                }
            }
        }

        public ContractResolverHandler()
        {
            _defaultFields = new List<string>();
            _defaultFields.Add("Code");
            _defaultFields.Add("Name");
        }

        public ContractResolverHandler(List<string> ignoreList)
        {
            _ignoreList = ignoreList;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            if (_ignoreList != null && _ignoreList.Contains(jsonProperty.PropertyName))
            {
                jsonProperty.ShouldSerialize = (a) => false;
            }
            else if (_modelType != null)
            {
                var prop = _propertyInfos.FirstOrDefault(p => p.Name == jsonProperty.PropertyName);

                if (prop != null)
                {
                    var attrIgnore = prop.GetCustomAttribute<ODataIgnoreOnSaveAttribute>(true);
                    bool ignoreOnSave = attrIgnore != null;

                    var attr = prop.GetCustomAttribute<ODataPropertyBase>(true);
                    bool isTableCustom = _tableAttribute?.IsCustom ?? true;
                    bool isCustom = isTableCustom;

                    // Se é uma tabela de usuário, assumimos que por padrão os campos 
                    // são de usuário também
                    if (attr != null)
                    {
                        isCustom = attr.IsCustomSetted ? attr.IsCustom : isTableCustom;
                    }

                    // Verifica se a propriedade é um dos campos padrão dos objetos B1
                    if (_defaultFields.Contains(jsonProperty.PropertyName) ||
                        _defaultFields.Contains(attr?.PhysicalName ?? string.Empty))
                    {
                        isCustom = false;
                    }

                    if (ignoreOnSave)
                    {
                        jsonProperty.ShouldSerialize = (a) => false;
                    }
                    else if (attr != null && (attr.OnlyDisplay || attr.IgnoreOnBackend))
                    {
                        jsonProperty.ShouldSerialize = (a) => false;
                    }
                    else if (!string.IsNullOrWhiteSpace(attr?.PhysicalName))
                    {
                        if (isCustom)
                        {
                            jsonProperty.PropertyName = $"U_{attr.PhysicalName}";
                        }
                        else
                        {
                            jsonProperty.PropertyName = $"{attr.PhysicalName}";
                        }
                    }
                }
            }

            return jsonProperty;
        }
    }
}
