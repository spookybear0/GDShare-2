#include <string>
#include <ShlObj.h>
#include <windows.h>
#include <vector>
#include <fstream>
#include <regex>
#include <algorithm>
#include <stdio.h>
#include <chrono>
#include <commdlg.h>
#include "methods.hpp"
#include "../ext/ZlibHelper.hpp"
#include "../ext/Base64.hpp"
#include "../ext/dirent.h"
#include "../ext/json.hpp"

namespace gd {
    enum err {
        SUCCESS,
        FILE_DOESNT_EXIST,
        LEVEL_NOT_FOUND,
        FILE_DIALOG
    };

    std::string export_dir = "gdshare_exports";

    namespace decode {
        rapidxml::xml_document<> decoded_data;
        std::vector<rapidxml::xml_node<>*> levels;

        std::string GetCCPath(std::string WHICH) {
            wchar_t* localAppData = 0;
            SHGetKnownFolderPath(FOLDERID_LocalAppData, 0, NULL, &localAppData);

            std::wstring CCW (localAppData);

            std::string RESULT ( CCW.begin(), CCW.end() );
            RESULT += "\\GeometryDash\\CC" + WHICH + ".dat";

            CoTaskMemFree(static_cast<void*>(localAppData));
            
            return RESULT;
        }

        std::vector<uint8_t> readf(std::string const& path) {
            std::vector<uint8_t> buffer;
            std::ifstream file(path, std::ios::ate, std::ios::binary);

            if (file.is_open()) {
                buffer.resize(file.tellg());
                file.seekg(0, std::ios::beg);

                file.read(
                    reinterpret_cast<char*>(buffer.data()),
                    buffer.size());
            }

            return buffer;
        }
        
        void DecodeXOR(std::vector<uint8_t>& BYTES, int KEY) {
            for (auto& b : BYTES)
                b ^= KEY;
        }

        std::vector<uint8_t> DecodeBase64(const std::string& str) {
            gdcrypto::base64::Base64 b64(gdcrypto::base64::URL_SAFE_DICT);
            return b64.decode(str);
        }

        std::string DecompressGZip(const std::vector<uint8_t> str) {
            auto buffer = gdcrypto::zlib::inflateBuffer(str);
            return std::string(buffer.data(), buffer.data() + buffer.size());
        }

        std::string DecodeCCFile(const char* _name, void (*_prog)(unsigned short, LPCSTR) = NULL) {
            methods::perf::start();

            if (_prog != NULL) _prog(0, "Reading file...");

            std::string CCPATH = decode::GetCCPath(_name);
            std::vector<uint8_t> CCCONTENTS = decode::readf(CCPATH);

            std::string c = methods::fread(CCPATH);
            if (c._Starts_with("<?xml version=\"1.0\"?>")) {
                if (_prog != NULL) _prog(100, "Finished!");

                return c;
            }

            if (_prog != NULL) _prog(25, "Decoding XOR...");

            DecodeXOR(CCCONTENTS, 11);

            if (_prog != NULL) _prog(50, "Decoding Base64...");

            auto XOR = std::string(CCCONTENTS.begin(), CCCONTENTS.end());
            std::vector<uint8_t> B64 = DecodeBase64(XOR);

            if (_prog != NULL) _prog(75, "Decompressing GZip...");

            std::string ZLIB = DecompressGZip(B64);

            if (_prog != NULL) _prog(100, ("Finished in " + methods::perf::log() + " !").c_str());

            return ZLIB;
        }

        bool SaveCCLocalLevels() {
            methods::fsave(decode::GetCCPath("LocalLevels"), "<?xml version=\"1.0\"?>\n" + methods::xts(&decode::decoded_data));

            return true;
        }

        rapidxml::xml_document<>* GetCCFileAsXML(const char* _which) {
            decoded_data.parse<0>(methods::stc(DecodeCCFile(_which)));
            return &decoded_data;
        }

        std::string DecodeLevelData(std::string _data) {
            return DecompressGZip(DecodeBase64(_data));
        }
    }

    namespace levels {
        void LoadLevels() {
            if (decode::levels.size() == 0) {
                gd::decode::GetCCFileAsXML("LocalLevels");

                rapidxml::xml_node<>* d = decode::decoded_data.first_node("plist")->first_node("dict")->first_node("d");
                rapidxml::xml_node<>* fs = NULL;
                
                std::vector<rapidxml::xml_node<>*> LIST = {};
                for (rapidxml::xml_node<>* child = d->first_node(); child; child = child->next_sibling())
                    if (std::strcmp(child->name(), "d") == 0)
                        LIST.push_back(child);

                decode::levels = LIST;
            }
        }

        std::string GetKey_X(rapidxml::xml_node<>* _lvl, const char* _key) {
            for (rapidxml::xml_node<>* child = _lvl->first_node(); child; child = child->next_sibling())
                if (std::strcmp(child->name(), "k") == 0)
                    if (std::strcmp(child->value(), _key) == 0)
                        return child->next_sibling()->value();
            return "";
        }

        bool SetKey_X(rapidxml::xml_node<>* _lvl, const char* _key, const char* _val, const char* _type = "s") {
            for (rapidxml::xml_node<>* child = _lvl->first_node(); child; child = child->next_sibling())
                if (std::strcmp(child->name(), "k") == 0)
                    if (std::strcmp(child->value(), _key) == 0) {
                        child->next_sibling()->first_node()->value(_val);
                        return true;
                    }
            std::string n_k ("<k>" + std::string (_key) + "</k><" + std::string (_type) + ">" + std::string (_val) + "</" + std::string (_type) + ">");
            rapidxml::xml_document<> n;
            n.parse<0>(methods::stc(n_k));

            rapidxml::xml_node<>* $k = _lvl->document()->clone_node(n.first_node("k"));
            rapidxml::xml_node<>* $t = _lvl->document()->clone_node(n.first_node(_type));
            _lvl->append_node($k);
            _lvl->append_node($t);
            return true;
        }

        std::string WithoutKey(const std::string DATA, std::string KEY) {
            std::regex m ("<k>" + KEY + "</k><.>");
            std::smatch cm;
            std::regex_search(DATA, cm, m);

            if (cm[0] == "") return "";

            std::string T_TYPE = ((std::string)cm[0]).substr(((std::string)cm[0]).find_last_of("<") + 1, 1);

            std::regex tm ("<k>" + KEY + "</k><" + T_TYPE + ">.*?</" + T_TYPE + ">");
            std::smatch tcm;
            std::regex_search(DATA, tcm, tm);

            std::string VAL = tcm[0];

            int L1 = ("<k>" + KEY + "</k><" + T_TYPE + ">").length();
            return DATA.substr(0, L1) + DATA.substr(VAL.find_last_of("</") - L1 - 1);
        }

        rapidxml::xml_node<>* GetLevel(std::string _name, std::string *_err = NULL) {
            LoadLevels();

            for (int i = 0; i < decode::levels.size(); i++) {
                if (methods::lower(GetKey_X(decode::levels[i], "k2")) == methods::lower(_name))
                    return decode::levels[i];
            }
            
            if (_err != NULL)
                *_err = "Could not find level! (Replace spaces in name with _)";
            return NULL;
        }

        int ExportLevel_X(std::string _name, std::string* _res_path = NULL) {
            std::cout << _name << std::endl;
            rapidxml::xml_node<>* lvl = GetLevel(_name);
            if (lvl == NULL) return err::LEVEL_NOT_FOUND;

            if (!methods::fexists(methods::workdir() + "\\" + export_dir))
                _mkdir((methods::workdir() + "\\" + export_dir).c_str());

            std::string path = methods::workdir() + "\\" + export_dir + "\\" + methods::replace(_name, " ", "_") + ".gmd";

            methods::fsave(path, methods::xts(GetLevel(_name)));

            if (_res_path != NULL)
                *_res_path = path;

            return err::SUCCESS;
        }

        int ImportLevel_X(std::string _path, std::string _lvl = "", std::string _name = "") {
            if (!methods::fexists(_path))
                return err::FILE_DOESNT_EXIST;

            std::string lvl;
            if (_lvl == "")
                lvl = methods::fread(_path);
            else lvl = _lvl;

            gd::decode::GetCCFileAsXML("LocalLevels");

            rapidxml::xml_node<>* d = decode::decoded_data.first_node("plist")->first_node("dict")->first_node("d");
            rapidxml::xml_node<>* fs = NULL;
            for (rapidxml::xml_node<>* child = d->first_node(); child; child = child->next_sibling()) {
                if (std::strcmp(child->name(), "k") == 0)
                    if (std::string(child->value()).find("k_") != std::string::npos) {
                        child->first_node()->value(methods::stc("k_" + std::to_string(std::stoi(std::string(child->value()).substr(2)) + 1)));
                        if (fs == NULL) fs = child;
                    }
            }

            rapidxml::xml_document<> lv;
            lv.parse<0>(methods::stc(lvl));
            SetKey_X(lv.first_node(), "k2", _name.c_str());

            rapidxml::xml_node<>* ln = lv.first_node();
            rapidxml::xml_node<>* lnt = decode::decoded_data.clone_node(ln);

            rapidxml::xml_document<> k_ix;
            k_ix.parse<0>(methods::stc("<k>k_0</k>"));
            rapidxml::xml_node<>* lk_ix = decode::decoded_data.clone_node(k_ix.first_node());

            d->insert_node(fs, lk_ix);
            d->insert_node(fs, lnt);

            gd::decode::SaveCCLocalLevels();

            return err::SUCCESS;
        }
    }
}