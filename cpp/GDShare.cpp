#include <iostream>
#include <string>
#include <Windows.h>
#include <algorithm>
#include "src/levels.hpp"

typedef void(_stdcall *LPRETCCCONTENT) (LPCSTR s);
typedef void(_stdcall *LPRETSTATUS) (unsigned short prog, LPCSTR str);
typedef void(_stdcall *LPRETLEVELS) (LPCSTR s[]);

extern "C" {
    __declspec(dllexport) void __stdcall DecodeCCFile(const char* _file, LPRETCCCONTENT _ret, LPRETSTATUS _prog) {
        std::string str = gd::decode::DecodeCCFile("LocalLevels", _prog);
        _ret(str.c_str());    // (LPCSTR)gd::decode::DecodeCCFile(_file)
        return;
    }

    __declspec(dllexport) void __stdcall GetLevels(const char* _CCData, LPRETLEVELS _ret) {
        std::vector<std::string> lvls;
        gd::levels::LoadLevels(&lvls);

        const char* lvlls[lvls.size()];

        for (int i = 0; i < lvls.size(); i++)
            lvlls[i] = lvls[i].c_str();

        _ret(lvlls);
        return;
    }
}