#! /bin/sh

# Bullet physics
if [ ! -f ../externals/BulletSharpPInvoke/src/x64/libbulletc.so ]; then
  git clone --recursive git@github.com:Eideren/BulletSharpPInvoke.git ../externals/BulletSharpPInvoke
  (
    # Any issues, try a variation of: sudo apt install libc++abi-dev libstdc++-dev
    cd ../externals/BulletSharpPInvoke/src
    sed -i -e 's/-Wno-unused-variable -std=c++14/-Wno-unused-variable -stdlib=libc++ -std=c++14/g' Makefile # make clang++ happy
    sed -i -e 's/COMM = x64\/libbulletc.$(EXTENSION) x86\/libbulletc.$(EXTENSION)/COMM = x64\/libbulletc.$(EXTENSION)/g' Makefile # only 64 bit version.
    mkdir x86
    mkdir x64
    make
    cp x64/libbulletc.so ../../../deps/BulletPhysics/dotnet/linux-x64/
  )
fi


# VHACD (within bullet directory)
if [ ! -f ../externals/BulletSharpPInvoke/src/x64/libVHACD.so ]; then
(
  cd ../externals/BulletSharpPInvoke/src/VHACD_Lib/src/
  ln -s ../inc/vhacdGraph.h ../inc/VHACDGraph.h
  ln -s ../inc/vhacdMeshDecimator.h ../inc/VHACDMeshDecimator.h 
  ln -s ../inc/vhacdMesh.h ../inc/VHACDMesh.h 
  ln -s ../inc/vhacdVersion.h ../inc/VHACDVersion.h 
  ln -s ../inc/vhacdVector.h ../inc/VHACDVector.h 
  ln -s ../inc/vhacdSArray.h ../inc/VHACDSArray.h 
  ln -s ../inc/vhacdVector.inl ../inc/VHACDVector.inl
  ln -s ../inc/vhacdMaterial.h ../inc/VHACDMaterial.h 
  ln -s ../inc/vhacdHACD.h ../inc/VHACDHACD.h 
  touch ../inc/omp.h
  clang++ -m64 --shared -I../inc/ -O3 -Wall -Wno-unused-variable -stdlib=libc++ -std=c++14 -fPIC  *.cpp -o ../../x64/libVHACD.so -ldl
  mkdir ../../../../../deps/VHACD/linux-x64
  cp ../../x64/libVHACD.so ../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/runtimes/linux-x64/native
  cp ../../x64/libVHACD.so ../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/
  cp ../../x64/libVHACD.so ../../../../../deps/VHACD/linux-x64
)
fi

# FreeImage
if [ ! -f ../externals/FreeImage/libfreeimage-3.16.0.so ]; then
  git clone --recursive https://github.com/stride3d/freeimage.git ../externals/FreeImage
  (
  cd ../externals/FreeImage
  sed -i -e 's/-fvisibility=hidden -Wno-ctor-dtor-privacy/-fvisibility=hidden -std=c++14 -Wno-narrowing -Wno-ctor-dtor-privacy -fshort-wchar/g' Makefile.gnu
  make
  mkdir ../../deps/FreeImage/Release/linux-x64/
  cp libfreeimage-3.16.0.so ../../deps/FreeImage/Release/linux-x64/libFreeImage.so
  cp libfreeimage-3.16.0.so ../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/runtimes/linux-x64/native/libFreeImage.so
  cp libfreeimage-3.16.0.so ../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/libFreeImage.so
  )
fi

# PVR Tex lib
if [ ! -f PVRTT/linux-x64/libPVRTexLib.so ]; then
  (
  cd PVRTT/source/
  mkdir ../linux-x64
  clang++ -m64 --shared -I../include/ -O3 -Wall -Wno-unused-variable -stdlib=libc++ -std=c++14 -fPIC  *.cpp -o ../linux-x64/libPVRTexLib.so -ldl
  # copy to somewhere relevant.
  cp PVRTT/linux-x64/libPVRTexLib.so ../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/runtimes/linux-x64/native/
  cp PVRTT/linux-x64/libPVRTexLib.so ../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/

  )
fi

# DirectXTex
(
  git clone --recursive https://github.com/microsoft/DirectXTex.git ../externals/DirectXTex
  git clone --recursive https://github.com/microsoft/DirectX-Headers.git ../externals/DirectXTex/DirectX-Headers-main
  git clone --recursive https://github.com/microsoft/DirectXMath.git ../externals/DirectXTex/DirectXMath-main
  cp DxtWrapper/* ../externals/DirectXTex/DirectXTex
  mkdir ../externals/DirectXTex/linux-x64
  #  https://github.com/microsoft/DirectXMath
  # https://github.com/microsoft/DirectX-Headers
  # https://raw.githubusercontent.com/tpn/winsdk-10/master/Include/10.0.10240.0/shared/sal.h
  # clang++ -m64 --shared -I. -I../DirectXMath-main/Inc/ -I../DirectX-Headers-main/include/ -I../DirectX-Headers-main/include/wsl/stubs -I../include/ -O3 -Wall -Wno-unused-variable -stdlib=libc++ -std=c++17 -fPIC  dxt_wrapper.cpp DirectXTexDDS.cpp DirectXTexConvert.cpp DirectXTexCompress.cpp DirectXTexMipmaps.cpp DirectXTexResize.cpp DirectXTexNormalMaps.cpp -o ../linux-x64/libDxtWrapper.so  -ldl -fdeclspec
  cd ../externals/DirectXTex/DirectXTex
  clang++ -m64 --shared -I. -I../DirectXMath-main/Inc/ -I../DirectX-Headers-main/include/ -I../DirectX-Headers-main/include/wsl/stubs -I../include/ -O3 -Wall -Wno-unused-variable -stdlib=libc++ -std=c++17 -fPIC  dxt_wrapper.cpp DirectXTexDDS.cpp DirectXTexConvert.cpp DirectXTexCompress.cpp DirectXTexMipmaps.cpp DirectXTexResize.cpp DirectXTexNormalMaps.cpp DirectXTexImage.cpp DirectXTexUtil.cpp BC6HBC7.cpp BC.cpp BC4BC5.cpp DirectXTexTGA.cpp DirectXTexPMAlpha.cpp -fshort-wchar -o ../linux-x64/libDxtWrapper.so  -ldl -fdeclspec -Wl,-z,defs
  cp ../linux-x64/libDxtWrapper.so ../../../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/DxtWrapper.so 
  cp ../linux-x64/libDxtWrapper.so ../../../sources/editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0/runtimes/linux-x64/native/DxtWrapper.so
)
