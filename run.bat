@echo off
clang++ cpp/GDShare.cpp cpp/ext/ZlibHelper.cpp -shared -o cpp/GDShare.dll -std=c++17 -lWtsApi32 -luser32 -lDbgHelp -lcpp/zlibd -lShell32 -lole32
if %errorlevel%==0 (
    dotnet run
) else (
    echo Error compiling DLL
)