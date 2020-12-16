#include <iostream>
#include <string>
#include <Windows.h>
#include <algorithm>
#include "src/levels.hpp"

typedef void(_stdcall *LPRETCCCONTENT) (LPCSTR s);
typedef void(_stdcall *LPRETSTATUS) (unsigned short prog, LPCSTR str);
typedef void(_stdcall *LPRETLEVELS) (LPCSTR s);
typedef void(_stdcall *LPRETSUCCESS) (bool s);

extern "C" {
    __declspec(dllexport) void __stdcall DecodeCCFile(const char* _file, LPRETCCCONTENT _ret, LPRETSTATUS _prog) {
        std::string str = gd::decode::DecodeCCFile("LocalLevels", _prog);
        _ret(str.c_str());
        return;
    }

    __declspec(dllexport) void __stdcall GetLevels(const char* _CCData, LPRETLEVELS _ret) {
        std::string lvls = "";
        gd::levels::LoadLevels(&lvls);

        _ret(lvls.substr(0, lvls.length() - 1).c_str());
        return;
    }

    __declspec(dllexport) void __stdcall ExportLevel(const char* _name, const char* _path, LPRETSUCCESS _ret) {
        _ret(gd::levels::ExportLevel_X(_name, _path) == gd::err::SUCCESS);
        return;
    }
}