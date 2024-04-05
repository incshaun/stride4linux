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

FbxScene * FbxSceneCreate(FbxManager* pManager, const char* pName)
{
  return FbxScene::Create (pManager, pName);
}

bool FbxImporterImport(FbxImporter * imp, FbxDocument* pDocument, bool pNonBlocking=false)
{
  return imp->Import(pDocument, pNonBlocking);
}

double FbxTimeGetFrameRate(FbxTime::EMode pTimeMode)
{
  return FbxTime::GetFrameRate(pTimeMode);
}

FbxTime::EMode FbxGlobalSettingsGetTimeMode(FbxGlobalSettings * glo)
{
  return glo->GetTimeMode ();
}

FbxGlobalSettings* FbxSceneGetGlobalSettings(FbxScene * sce)
{
  return &sce->GetGlobalSettings ();
}

FbxNode* FbxSceneGetRootNode(FbxScene * sce)
{
  return sce->GetRootNode ();
}

void FbxNodeResetPivotSetAndConvertAnimation(FbxNode * nod, double pFrameRate=30.0, bool pKeyReduce=false, bool pToNodeCenter=true, bool pForceResetLimits=false)
{
  nod->ResetPivotSetAndConvertAnimation(pFrameRate, pKeyReduce, pToNodeCenter, pForceResetLimits);
}

int FbxNodeGetNodeAttributeCount(FbxNode * nod)
{
  return nod->GetNodeAttributeCount ();
}

FbxNodeAttribute* FbxNodeGetNodeAttributeByIndex(FbxNode * nod, int pIndex)
{
  return nod->GetNodeAttributeByIndex(pIndex);
}

FbxNodeAttribute::EType FbxNodeAttributeGetAttributeType(FbxNodeAttribute * att)
{
  return att->GetAttributeType ();
}

int FbxNodeGetChildCount(FbxNode * nod, bool pRecursive = false)
{
  return nod->GetChildCount(pRecursive);
}

FbxNode* FbxNodeGetChild(FbxNode * nod, int pIndex)
{
  return nod->GetChild(pIndex);
}

FbxNode* FbxMeshGetNode(FbxMesh * mes, int pIndex = 0)
{
  return mes->GetNode(pIndex);
}

const char* FbxNodeGetName(FbxNode * nod)
{
  return nod->GetName ();
}

int FbxSceneGetMaterialCount(FbxScene * sce)
{
  return sce->GetMaterialCount ();
}

FbxSurfaceMaterial* FbxSceneGetMaterialByName(FbxScene * sce, char* pName)
{
  return sce->GetMaterial(pName);
}

FbxSurfaceMaterial* FbxSceneGetMaterial(FbxScene * sce, int pIndex)
{
  return sce->GetMaterial(pIndex);
}

// Not very robust, but should be good enough for intended use.
FbxAxisSystem axisSystem;
FbxAxisSystem * FbxGlobalSettingsGetAxisSystem(FbxGlobalSettings * set)
{
  axisSystem = set->GetAxisSystem ();
  return &axisSystem;
}

FbxSystemUnit systemUnit;
FbxSystemUnit * FbxGlobalSettingsGetSystemUnit(FbxGlobalSettings * set) 
{
  systemUnit = set->GetSystemUnit ();
  return &systemUnit;
}

double FbxSystemUnitGetScaleFactor(FbxSystemUnit * syu)
{
  return syu->GetScaleFactor ();
}

int FbxAxisSystemGetUpVector(FbxAxisSystem * axi, int & pSign )
{
  return axi->GetUpVector(pSign);
}

int FbxAxisSystemGetFrontVector(FbxAxisSystem * axi, int & pSign ) 
{
  return axi->GetFrontVector(pSign);
}

fbxsdk::FbxAxisSystem::ECoordSystem FbxAxisSystemGetCoordSystem(FbxAxisSystem * axi)
{
  return axi->GetCoorSystem ();
}

double * FbxNodeEvaluateLocalTransform(FbxNode * nod, long pTime=FBXSDK_TC_INFINITY, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
{
  auto time = FbxTime(pTime);
  return (double *) (&nod->EvaluateLocalTransform(time, pPivotSet, pApplyTarget, pForceEval));
}




#ifdef __cplusplus
}
#endif
