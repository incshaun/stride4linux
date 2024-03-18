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
  cp ../../x64/libVHACD.so ../../../../../deps/VHACD/linux-x64
)

# FreeImage
if [ ! -f ../externals/FreeImage/libfreeimage-3.16.0.so ]; then
  git clone --recursive https://github.com/stride3d/freeimage.git ../externals/FreeImage
  (
  cd ../externals/FreeImage
  sed -i -e 's/-fvisibility=hidden -Wno-ctor-dtor-privacy/-fvisibility=hidden -std=c++14 -Wno-narrowing -Wno-ctor-dtor-privacy/g' Makefile.gnu
  make
  mkdir ../../deps/FreeImage/Release/linux-x64/
  cp libfreeimage-3.16.0.so ../../deps/FreeImage/Release/linux-x64/libFreeImage.so
  )
fi
