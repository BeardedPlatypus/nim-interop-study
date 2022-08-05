namespace type_description_export.common

module Records =
    type public FieldDescription = 
      { fieldName : string
        typeName : string
      }
    type ComponentDescription = 
      { componentName : string
        fields : FieldDescription list
      }

