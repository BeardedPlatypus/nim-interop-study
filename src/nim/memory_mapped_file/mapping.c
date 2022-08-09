#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include <tchar.h>

// TCHAR bufferName[]=TEXT("Global\\NimBuffer");
TCHAR bufferName[]=TEXT("Local\\NimBuffer");

void CreateBufferFile() {
    HANDLE hMapFile;

    hMapFile = CreateFileMapping(
        INVALID_HANDLE_VALUE,
        NULL,
        PAGE_READWRITE,
        0,
        sizeof(int),
        bufferName);
    return hMapFile;
}

LPVOID CreateBuffer(HANDLE hMapFile) {
    LPVOID p = MapViewOfFileEx(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(int), NULL);
    return p;
}

void FreeBuffer(LPVOID pBuffer) {
    UnmapViewOfFile(pBuffer);
}

void FreeBufferFile(HANDLE hMapFile) {
    CloseHandle(hMapFile);
}