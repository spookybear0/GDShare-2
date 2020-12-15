@echo off
if "%1"=="-r" goto run

clang++ cpp/GDShare.cpp cpp/ext/ZlibHelper.cpp -shared -o cpp/GDShare.dll -std=c++17 -lWtsApi32 -luser32 -lDbgHelp -lcpp/zlib -lShell32 -lole32 -O3
if %errorlevel%==0 (
    goto run
) else (
    echo Error compiling DLL
    goto done
)

:run
dotnet run

:done