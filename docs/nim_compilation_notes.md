# Notes on compiler usage.
## Nim can be easily compiled as a DLL with the `--app:lib` flag

Nim can be compiled as a DLL with the `--app:lib` flag:

```powershell
nim c --app:lib -d:release "[<path-to-nim-directory>/]<nim-file>.nim"
```

Note that it is also possible to compile the nim files as a static library by
using `staticlib` instead of `lib`. 
See the [Compiler Guide](https://nim-lang.org/docs/nimc.html#compiler-usage-commandminusline-switches) for more details.

## The compiler cache can be removed to force a complete rebuild

During my experimentation I found that at times changes where not properly
propagated, especially when importing C code. In order to force a complete
rebuild the nim cache of the specific project you're building can be removed
and a new build will rebuild the whole project.

The nim cache can be found on windows at `%userprofile%\nimcache`.
See the [Compiler Guide](https://nim-lang.org/docs/nimc.html#compiler-usage-generated-c-code-directory) for more details.
