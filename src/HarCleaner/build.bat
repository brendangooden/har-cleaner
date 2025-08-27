@echo off
echo Building HAR Cleaner...
dotnet build --configuration Release

echo.
echo Publishing self-contained executable...
dotnet publish --configuration Release --self-contained true --runtime win-x64 --output publish/win-x64
dotnet publish --configuration Release --self-contained true --runtime linux-x64 --output publish/linux-x64
dotnet publish --configuration Release --self-contained true --runtime osx-x64 --output publish/osx-x64

echo.
echo Build completed! Executables are in the publish/ directories.
echo.
echo Test with:
echo   publish/win-x64/HarCleaner.exe --help
echo   publish/linux-x64/HarCleaner --help
echo   publish/osx-x64/HarCleaner --help
