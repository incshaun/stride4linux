#! /bin/bash

clang++ -m64 --shared -I. -Isdk/include -O3 -Wall -Wno-unused-variable -stdlib=libc++ -std=c++17 -fPIC  fbxinterface.cpp sdk/lib/gcc4/x64/debug/libfbxsdk.a -fshort-wchar -o libfbx.so  -ldl -fdeclspec 
cp libfbx.so ../../editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0-windows7.0/

