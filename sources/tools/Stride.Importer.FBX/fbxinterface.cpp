// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#include <fbxsdk.h>
#include <iostream>

#ifdef __cplusplus
extern "C" {
#endif

fbxsdk::FbxManager * FbxManagerCreate()
{
  return fbxsdk::FbxManager::Create ();
}

FbxIOSettings * FbxIOSettingsCreate(FbxManager* pManager, const char* pName)
{
  return FbxIOSettings::Create (pManager, pName);
}

void FbxIOSettingsSetBoolProp(FbxIOSettings * ios, const char* pName, bool pValue)
{
  std::cout << "AA " << pName << "\n";
  ios->SetBoolProp(pName, pValue);
}

void FbxManagerSetIOSettings(FbxManager* pManager, FbxIOSettings* pIOSettings)
{
  pManager->SetIOSettings(pIOSettings);
}

FbxImporter * FbxImporterCreate(FbxManager* pManager, const char* pName)
{
  return FbxImporter::Create (pManager, pName);
}

bool FbxImporterInitialize(FbxImporter * imp, const char* pFileName, int pFileFormat=-1, FbxIOSettings * pIOSettings=NULL)
{
  return imp->Initialize(pFileName, pFileFormat, pIOSettings);
}

FbxIOSettings* FbxManagerGetIOSettings(FbxManager * man)
{
  return man->GetIOSettings ();
}

FbxStatus * FbxImporterGetStatus(FbxImporter * imp)
{
  return &imp->GetStatus ();
}

const char* FbxStatusGetErrorString(FbxStatus * sta)
{
  return sta->GetErrorString ();
}

#ifdef __cplusplus
}
#endif
