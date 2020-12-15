#include <iostream>
#include <string>
#include <Windows.h>
#include "src/levels.hpp"

int main() {
    gd::decode::DecodeCCFile("LocalLevels", [](unsigned short _p, LPCSTR _s) {
        std::cout << _s << std::endl;
    });
}

typedef void(_stdcall *LPRETCCCONTENT) (LPCSTR s);
typedef void(_stdcall *LPRETSTATUS) (unsigned short prog, LPCSTR str);

extern "C" {
    __declspec(dllexport) void __stdcall DecodeCCFile(const char* _file, LPRETCCCONTENT _ret, LPRETSTATUS _prog) {
        std::string str = gd::decode::DecodeCCFile("LocalLevels", _prog);
        _ret(str.c_str());    // (LPCSTR)gd::decode::DecodeCCFile(_file)
        return;
    }
}