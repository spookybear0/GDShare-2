#include <iostream>
#include <string>
#include <Windows.h>
#include "src/levels.hpp"

extern "C" {
    __declspec(dllexport) std::string DecodeCCFile(std::string _file) {
        return gd::decode::DecodeCCLocalLevels();
    }
}