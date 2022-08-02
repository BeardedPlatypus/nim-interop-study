import system
import macros      # used for defining the custom macros
import typetraits  # used for inspecting the types
import std/tables  # used for defining the type mappings

# ComponentDescription describes a single component, which for all intents and purposes
# maps onto a Nim object.
type ComponentDescription* = 
  object
    component_name: string
    field_names: seq[string]
    field_types: seq[int]      # the field_types are stored as integers corresponding
                               # with an index in the index_to_typ_name sequence

# Because these variables are only used to construct the final exported methods, they
# are defined with compileTime pragma's.
var component_descriptions {.compileTime.}: seq[ComponentDescription] = @[]
var type_name_to_index {.compileTime.} = initTable[string, int]()
var index_to_type_name {.compileTime.}: seq[string] = @[]
var n_types {.compileTime.}: int = 0

proc retrieveType(t: NimNode): NimNode {.compileTime.} =
  # see: https://stackoverflow.com/questions/38029742/nim-reflect-on-types-field-types-at-compile-time
  getType(getType(t)[1])

# exportComponent should be used to export types from custom nim files.
macro exportComponent*(t: typedesc): untyped =
  let t_desc = retrieveType(t)
  let t_desc_name : string = $(t)

  var field_names: seq[string] = @[]
  var field_types: seq[int] = @[]

  for c in t_desc[2].children:
    field_names.add($(c))
     
    let ft = getType(c)
    let ft_name = $(ft)

    if (not type_name_to_index.hasKey(ft_name)):
      type_name_to_index[ft_name] = len(index_to_type_name)
      add(index_to_type_name, ft_name)
      n_types += 1
    
    let ft_index: int = type_name_to_index[ft_name]
    field_types.add(ft_index)

  add(component_descriptions, ComponentDescription(component_name: t_desc_name, 
                                                   field_names: field_names, 
                                                   field_types: field_types))

# constructComponentDescriptions is used by the main nim file to construct the descriptions.
macro constructComponentDescriptions*(): untyped =
  let n_components: int = len(component_descriptions)
  let n_components_lit = newLit(n_components)

  type
    DescriptionArray = array[len(component_descriptions), ComponentDescription]
  var description_array: DescriptionArray

  var i: int = 0
  for desc in component_descriptions:
      description_array[i] = desc
      i += 1

  type
    TypeArray = array[n_types, string]

  var type_array: TypeArray

  var j: int = 0
  for t in index_to_type_name:
      type_array[j] = t
      j += 1

  let type_names_lit = newLit(type_array)
  let descriptions_lit = newLit(description_array)

  # Generate the actual c-api.
  quote do:
    let type_names* = `type_names_lit`
    let descriptions* = `descriptions_lit`

    proc get_n_components*(): int {. exportc, dynlib .} =
      `n_components_lit`

    proc get_component_name_length*(component_index: int): int {. exportc, dynlib .} =
      len(descriptions[component_index].component_name) + 1

    proc get_component_name*(component_index: int, buffer: cstring): void {. exportc, dynlib .} =
      let name: cstring = descriptions[component_index].component_name
      let size: int = get_component_name_length(component_index)
      moveMem(buffer[0].unsafeAddr, name[0].unsafeAddr, size)

    proc get_n_fields*(component_index: int): int {.exportc, dynlib .} =
      len(descriptions[component_index].field_names)

    proc get_field_name_length*(component_index: int, field_index: int): int {. exportc, dynlib .} =
      len(descriptions[component_index].field_names[field_index]) + 1

    proc get_field_name*(component_index: int, field_index: int, buffer: cstring): void {. exportc, dynlib .} =
      let name: cstring = descriptions[component_index].field_names[field_index]
      let size: int = get_field_name_length(component_index, field_index)
      moveMem(buffer[0].unsafeAddr, name[0].unsafeAddr, size)

    proc get_field_type_index*(component_index: int, field_index: int): int {. exportc, dynlib .} =
      descriptions[component_index].field_types[field_index]

    proc get_type_name_length*(type_index: int): int {. exportc, dynlib .} =
      len(type_names[type_index]) + 1

    proc get_type_name*(type_index: int, buffer: cstring): void {. exportc, dynlib .} =
      let name: cstring = type_names[type_index]
      let size: int = get_type_name_length(type_index)
      moveMem(buffer[0].unsafeAddr, name[0].unsafeAddr, size)