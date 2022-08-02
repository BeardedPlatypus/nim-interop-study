#ifndef FUNCTION_EXPORT
#define FUNCTION_EXPORT
#define DLLAPI __declspec( dllexport )

// The following lines the define the different capi function calls exposed to the dll
DLLAPI float __cdecl cadd(float a, float b);
DLLAPI int __cdecl get_name_size();
DLLAPI void __cdecl get_name(char* buffer);

#endif