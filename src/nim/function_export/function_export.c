#include "function_export.h"

double cadd(double a, double b) 
{
    return a + b;
}

char const* get_name_internal() {
    return "nimrod";
}

// In order to properly return a string, we first need to query the string size
// after which we can provide a buffer of that size in which the string can be written.
int get_name_size() {
    char const* n = get_name_internal();
    return sizeof(n) / sizeof(n[0]);
}

void __cdecl get_name(char* buffer) {
    strncpy(buffer, get_name_internal(), get_name_size());
}