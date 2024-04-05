// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Stride.Core.BuildEngine;
using Stride.Core.Diagnostics;
using Stride.Core.IO;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Serialization.Contents;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;
using Stride.Assets.Materials;
using Stride.Animations;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics;
using Stride.Graphics.Data;
using Stride.Shaders;

using Stride.Importer.Common;

namespace Stride.Importer.FBX
{
    public class MaterialInstantiation
    {
        public MaterialInstantiation()
        {
        }

//         FbxSurfaceMaterial* SourceMaterial;
//         MaterialAsset^ Material;
//         String^ MaterialName;
    };


    public class MeshConverter
    {
        private static StringBuilder IOSROOT = new StringBuilder ("IOSRoot");

        private static StringBuilder IOSN_EXPORT     = new StringBuilder ("Export");
        private static StringBuilder IOSN_IMPORT     = new StringBuilder ("Import");

        private static StringBuilder IOSN_PLUGIN_GRP = new StringBuilder ("PlugInGrp");

        private static StringBuilder IOSN_PLUGIN_UI_WIDTH = new StringBuilder ("PlugInUIWidth");
        private static StringBuilder IOSN_PLUGIN_UI_HEIGHT    = new StringBuilder ("PlugInUIHeight");
        private static StringBuilder IOSN_PLUGIN_VERSIONS_URL = new StringBuilder ("PluginVersionsURL");
        private static StringBuilder IOSN_PI_VERSION = new StringBuilder ("PIVersion");

        private static StringBuilder IOSN_PRESET_SELECTED            = new StringBuilder ("PresetSelected");

        private static StringBuilder IOSN_PRESETS_GRP         = new StringBuilder ("PresetsGrp");
        private static StringBuilder IOSN_STATISTICS_GRP      = new StringBuilder ("StatisticsGrp");
        private static StringBuilder IOSN_UNITS_GRP           = new StringBuilder ("UnitsGrp");
        private static StringBuilder IOSN_INCLUDE_GRP         = new StringBuilder ("IncludeGrp");
        private static StringBuilder IOSN_ADV_OPT_GRP         = new StringBuilder ("AdvOptGrp");
        private static StringBuilder IOSN_AXISCONV_GRP        = new StringBuilder ("AxisConvGrp");
        private static StringBuilder IOSN_CAMERA_GRP = new StringBuilder ("CameraGrp");
        private static StringBuilder IOSN_LIGHT_GRP = new StringBuilder ("LightGrp");
        private static StringBuilder IOSN_EXTRA_GRP                  = new StringBuilder ("ExtraGrp");
        private static StringBuilder IOSN_CONSTRAINTS_GRP = new StringBuilder ("ConstraintsGrp");
        private static StringBuilder IOSN_INPUTCONNECTIONS_GRP       = new StringBuilder ("InputConnectionsGrp");
        private static StringBuilder IOSN_INFORMATION_GRP            = new StringBuilder ("InformationGrp");

        private static StringBuilder IOSN_UP_AXIS = new StringBuilder ("UpAxis");
        private static StringBuilder IOSN_UP_AXIS_MAX = new StringBuilder ("UpAxisMax");
        private static StringBuilder IOSN_ZUPROTATION_MAX            = new StringBuilder ("ZUProtation_max");
        private static StringBuilder IOSN_AXISCONVERSION             = new StringBuilder ("AxisConversion");
        private static StringBuilder IOSN_AUTO_AXIS                  = new StringBuilder ("AutoAxis");
        private static StringBuilder IOSN_FILE_UP_AXIS               = new StringBuilder ("FileUpAxis");

        private static StringBuilder IOSN_PRESETS         = new StringBuilder ("Presets");
        private static StringBuilder IOSN_STATISTICS         = new StringBuilder ("Statistics");
        private static StringBuilder IOSN_UNITS_SCALE            = new StringBuilder ("UnitsScale");
        private static StringBuilder IOSN_TOTAL_UNITS_SCALE_TB   = new StringBuilder ("TotalUnitsScale");

        private static StringBuilder IOSN_SCALECONVERSION            = new StringBuilder ("ScaleConversion");
        private static StringBuilder IOSN_MASTERSCALE                = new StringBuilder ("MasterScale");

        private static StringBuilder IOSN_DYN_SCALE_CONVERSION       = new StringBuilder ("DynamicScaleConversion");
        private static StringBuilder IOSN_UNITSELECTOR = new StringBuilder ("UnitsSelector");

        private static StringBuilder IOSN_AUDIO = new StringBuilder ("Audio");
        private static StringBuilder IOSN_ANIMATION           = new StringBuilder ("Animation");
        private static StringBuilder IOSN_GEOMETRY            = new StringBuilder ("Geometry");
        private static StringBuilder IOSN_DEFORMATION = new StringBuilder ("Deformation");
        private static StringBuilder IOSN_MARKERS = new StringBuilder ("Markers");

        private static StringBuilder IOSN_CHARACTER = new StringBuilder ("Character");
        private static StringBuilder IOSN_CHARACTER_AS_MAYA_HIK      = new StringBuilder ("CharacterAsMayaHIK");
        private static StringBuilder IOSN_CHARACTER_TYPE             = new StringBuilder ("CharacterType");
        private static StringBuilder IOSN_CHARACTER_TYPE_DESC        = new StringBuilder ("CharacterTypeDesc");

        private static StringBuilder IOSN_SETLOCKEDATTRIB = new StringBuilder ("LockedAttribute");
        private static StringBuilder IOSN_TRIANGULATE                = new StringBuilder ("Triangulate");

        private static StringBuilder IOSN_MRCUSTOMATTRIBUTES  = new StringBuilder ("MRCustomAttributes");
        private static StringBuilder IOSN_MESHPRIMITIVE       = new StringBuilder ("MeshPrimitive");
        private static StringBuilder IOSN_MESHTRIANGLE        = new StringBuilder ("MeshTriangle");
        private static StringBuilder IOSN_MESHPOLY            = new StringBuilder ("MeshPoly");
        private static StringBuilder IOSN_NURB                = new StringBuilder ("Nurb");
        private static StringBuilder IOSN_PATCH               = new StringBuilder ("Patch");
        private static StringBuilder IOSN_BIP2FBX             = new StringBuilder ("Bip2Fbx");
        private static StringBuilder IOSN_ASCIIFBX                   = new StringBuilder ("AsciiFbx");

        private static StringBuilder IOSN_TAKE = new StringBuilder ("Take");

        private static StringBuilder IOSN_GEOMETRYMESHPRIMITIVEAS    = new StringBuilder ("GeometryMeshPrimitiveAs");
        private static StringBuilder IOSN_GEOMETRYMESHTRIANGLEAS     = new StringBuilder ("GeometryMeshTriangleAs");
        private static StringBuilder IOSN_GEOMETRYMESHPOLYAS         = new StringBuilder ("GeometryMeshPolyAs");
        private static StringBuilder IOSN_GEOMETRYNURBSAS            = new StringBuilder ("GeometryNurbsAs");

        private static StringBuilder IOSN_GEOMETRYNURBSSURFACEAS     = new StringBuilder ("GeometryNurbsSurfaceAs");
        private static StringBuilder IOSN_GEOMETRYPATCHAS            = new StringBuilder ("GeometryPatchAs");

        private static StringBuilder IOSN_TANGENTS_BINORMALS         = new StringBuilder ("TangentsandBinormals");
        private static StringBuilder IOSN_SMOOTH_MESH                = new StringBuilder ("SmoothMesh");
        private static StringBuilder IOSN_SELECTION_SET              = new StringBuilder ("SelectionSet");
        private static StringBuilder IOSN_ANIMATIONONLY              = new StringBuilder ("AnimationOnly");
        private static StringBuilder IOSN_SELECTIONONLY = new StringBuilder ("SelectionOnly");

        private static StringBuilder IOSN_BONE = new StringBuilder ("Bone");
        private static StringBuilder IOSN_BONEWIDTHHEIGHTLOCK  = new StringBuilder ("BoneWidthHeightLock");
        private static StringBuilder IOSN_BONEASDUMMY          = new StringBuilder ("BoneAsDummy");
        private static StringBuilder IOSN_BONEMAX4BONEWIDTH    = new StringBuilder ("Max4BoneWidth");
        private static StringBuilder IOSN_BONEMAX4BONEHEIGHT   = new StringBuilder ("Max4BoneHeight");
        private static StringBuilder IOSN_BONEMAX4BONETAPER    = new StringBuilder ("Max4BoneTaper");

        private static StringBuilder IOSN_REMOVE_SINGLE_KEY          = new StringBuilder ("RemoveSingleKey");
        private static StringBuilder IOSN_CURVE_FILTER     = new StringBuilder ("CurveFilter");
        private static StringBuilder IOSN_CONSTRAINT     = new StringBuilder ("Constraint");
        private static StringBuilder IOSN_UI     = new StringBuilder ("UI");
        private static StringBuilder IOSN_SHOW_UI_MODE               = new StringBuilder ("ShowUIMode");
        private static StringBuilder IOSN_SHOW_WARNINGS_MANAGER      = new StringBuilder ("ShowWarningsManager");
        private static StringBuilder IOSN_GENERATE_LOG_DATA          = new StringBuilder ("GenerateLogData");

        private static StringBuilder IOSN_PERF_GRP = new StringBuilder ("Performance");
        private static StringBuilder IOSN_REMOVEBADPOLYSFROMMESH     = new StringBuilder ("RemoveBadPolysFromMesh");
        private static StringBuilder IOSN_META_DATA                  = new StringBuilder ("MetaData");

        private static StringBuilder IOSN_CACHE_GRP                  = new StringBuilder ("Cache");
        private static StringBuilder IOSN_CACHE_SIZE                 = new StringBuilder ("CacheSize");

        private static StringBuilder IOSN_MERGE_MODE        = new StringBuilder ("MergeMode");
        private static StringBuilder IOSN_MERGE_MODE_DESCRIPTION     = new StringBuilder ("MergeModeDescription" );
        private static StringBuilder IOSN_ONE_CLICK_MERGE            = new StringBuilder ("OneClickMerge");
        private static StringBuilder IOSN_ONE_CLICK_MERGE_TEXTURE    = new StringBuilder ("OneClickMergeTexture");

        private static StringBuilder IOSN_SAMPLINGPANEL              = new StringBuilder ("SamplingPanel");

        private static StringBuilder IOSN_FILE_FORMAT     = new StringBuilder ("FileFormat");
        private static StringBuilder IOSN_FBX                 = new StringBuilder ("Fbx");
        private static StringBuilder IOSN_DXF                 = new StringBuilder ("Dxf");
        private static StringBuilder IOSN_OBJ                 = new StringBuilder ("Obj");
        private static StringBuilder IOSN_3DS                 = new StringBuilder ("Max_3ds");  // can't start by a number for xml node name
        private static StringBuilder IOSN_COLLADA             = new StringBuilder ("Collada");

        private static StringBuilder IOSN_MOTION_BASE = new StringBuilder ("Motion_Base");  // for commond Motion Readers/Writers stream options
        private static StringBuilder IOSN_BIOVISION_BVH        = new StringBuilder ("Biovision_BVH");
        private static StringBuilder IOSN_MOTIONANALYSIS_HTR   = new StringBuilder ("MotionAnalysis_HTR");
        private static StringBuilder IOSN_MOTIONANALYSIS_TRC   = new StringBuilder ("MotionAnalysis_TRC");
        private static StringBuilder IOSN_ACCLAIM_ASF          = new StringBuilder ("Acclaim_ASF");
        private static StringBuilder IOSN_ACCLAIM_AMC          = new StringBuilder ("Acclaim_AMC");
        private static StringBuilder IOSN_VICON_C3D            = new StringBuilder ("Vicon_C3D");

        private static StringBuilder IOSN_SKINS  = new StringBuilder ("Skins");
        private static StringBuilder IOSN_POINTCACHE           = new StringBuilder ("PointCache");
        private static StringBuilder IOSN_QUATERNION  = new StringBuilder ("Quaternion");
        private static StringBuilder IOSN_NAMETAKE                   = new StringBuilder ("UseSceneName");

        private static StringBuilder IOSN_SHAPE                = new StringBuilder ("Shape");
        private static StringBuilder IOSN_LIGHT = new StringBuilder ("Light");
        private static StringBuilder IOSN_LIGHTATTENUATION            = new StringBuilder ("LightAttenuation");
        private static StringBuilder IOSN_CAMERA = new StringBuilder ("Camera");
        private static StringBuilder IOSN_VIEW_CUBE     = new StringBuilder ("ViewCube");

        private static StringBuilder IOSN_BINDPOSE = new StringBuilder ("BindPose");

        private static StringBuilder IOSN_EMBEDTEXTURE_GRP       = new StringBuilder ("EmbedTextureGrp");
        private static StringBuilder IOSN_EMBEDTEXTURE        = new StringBuilder ("EmbedTexture");
        private static StringBuilder IOSN_EMBEDDED_FOLDER        = new StringBuilder ("ExtractFolder");
        private static StringBuilder IOSN_CONVERTTOTIFF       = new StringBuilder ("Convert_2Tiff");

        private static StringBuilder IOSN_UNLOCK_NORMALS             = new StringBuilder ("UnlockNormals");
        private static StringBuilder IOSN_CREASE              = new StringBuilder ("Crease");
        private static StringBuilder IOSN_FINESTSUBDIVLEVEL = new StringBuilder ("FinestSubdivLevel");

        private static StringBuilder IOSN_BAKEANIMATIONLAYERS = new StringBuilder ("BakeAnimationLayers");
        private static StringBuilder IOSN_BAKECOMPLEXANIMATION = new StringBuilder ("BakeComplexAnimation");

        private static StringBuilder IOSN_BAKEFRAMESTART = new StringBuilder ("BakeFrameStart");
        private static StringBuilder IOSN_BAKEFRAMEEND = new StringBuilder ("BakeFrameEnd");
        private static StringBuilder IOSN_BAKEFRAMESTEP = new StringBuilder ("BakeFrameStep"); 
        private static StringBuilder IOSN_BAKEFRAMESTARTNORESET = new StringBuilder ("BakeFrameStartNoReset");
        private static StringBuilder IOSN_BAKEFRAMEENDNORESET = new StringBuilder ("BakeFrameEndNoReset");
        private static StringBuilder IOSN_BAKEFRAMESTEPNORESET = new StringBuilder ("BakeFrameStepNoReset"); 

        private static StringBuilder IOSN_USEMATRIXFROMPOSE       = new StringBuilder ("UseMatrixFromPose");
        private static StringBuilder IOSN_NULLSTOPIVOT            = new StringBuilder ("NullsToPivot");
        private static StringBuilder IOSN_PIVOTTONULLS               = new StringBuilder ("PivotToNulls");

        private static StringBuilder IOSN_GEOMNORMALPERPOLY    = new StringBuilder ("GeomNormalPerPoly");
        private static StringBuilder IOSN_MAXBONEASBONE = new StringBuilder ("MaxBoneAsBone");
        private static StringBuilder IOSN_MAXNURBSSTEP = new StringBuilder ("MaxNurbsStep");
        private static StringBuilder IOSN_PROTECTDRIVENKEYS          = new StringBuilder ("ProtectDrivenKeys");
        private static StringBuilder IOSN_DEFORMNULLSASJOINTS        = new StringBuilder ("DeformNullsAsJoints");

        private static StringBuilder IOSN_ENVIRONMENT                = new StringBuilder ("Environment");

        // Note this will use IOSN_SAMPLINGRATE 
        private static StringBuilder IOSN_SAMPLINGRATESELECTOR       = new StringBuilder ("SamplingRateSelector");

        private static StringBuilder IOSN_SAMPLINGRATE               = new StringBuilder ("CurveFilterSamplingRate");
        private static StringBuilder IOSN_APPLYCSTKEYRED             = new StringBuilder ("CurveFilterApplyCstKeyRed");
        private static StringBuilder IOSN_CSTKEYREDTPREC = new StringBuilder ("CurveFilterCstKeyRedTPrec");
        private static StringBuilder IOSN_CSTKEYREDRPREC = new StringBuilder ("CurveFilterCstKeyRedRPrec");
        private static StringBuilder IOSN_CSTKEYREDSPREC             = new StringBuilder ("CurveFilterCstKeyRedSPrec");
        private static StringBuilder IOSN_CSTKEYREDOPREC             = new StringBuilder ("CurveFilterCstKeyRedOPrec");
        private static StringBuilder IOSN_APPLYKEYREDUCE             = new StringBuilder ("CurveFilterApplyKeyReduce");
        private static StringBuilder IOSN_KEYREDUCEPREC              = new StringBuilder ("CurveFilterKeyReducePrec");
        private static StringBuilder IOSN_APPLYKEYSONFRM             = new StringBuilder ("CurveFilterApplyKeysOnFrm");
        private static StringBuilder IOSN_APPLYKEYSYNC               = new StringBuilder ("CurveFilterApplyKeySync");
        private static StringBuilder IOSN_APPLYUNROLL                = new StringBuilder ("CurveFilterApplyUnroll");
        private static StringBuilder IOSN_UNROLLPREC                 = new StringBuilder ("CurveFilterUnrollPrec"); 
        private static StringBuilder IOSN_UNROLLPATH                 = new StringBuilder ("CurveFilterUnrollPath");
        private static StringBuilder IOSN_UNROLLFORCEAUTO            = new StringBuilder ("CurveFilterUnrollForceAuto");

        private static StringBuilder IOSN_AUTOTANGENTSONLY           = new StringBuilder ("AutoTangentsOnly");

        private static StringBuilder IOSN_SMOOTHING_GROUPS           = new StringBuilder ("SmoothingGroups");
        private static StringBuilder IOSN_HARDEDGES                  = new StringBuilder ("HardEdges");
        private static StringBuilder IOSN_EXP_HARDEDGES              = new StringBuilder ("expHardEdges");
        private static StringBuilder IOSN_BLINDDATA                  = new StringBuilder ("BlindData");
        private static StringBuilder IOSN_INPUTCONNECTIONS           = new StringBuilder ("InputConnections");
        private static StringBuilder IOSN_INSTANCES                  = new StringBuilder ("Instances");
        private static StringBuilder IOSN_REFERENCES                 = new StringBuilder ("References");
        private static StringBuilder IOSN_CONTAINEROBJECTS           = new StringBuilder ("ContainerObjects");
        private static StringBuilder IOSN_BYPASSRRSINHERITANCE       = new StringBuilder ("BypassRrsInheritance");
        private static StringBuilder IOSN_FORCEWEIGHTNORMALIZE       = new StringBuilder ("ForceWeightNormalize");
        private static StringBuilder IOSN_SHAPEANIMATION             = new StringBuilder ("ShapeAnimation");
        private static StringBuilder IOSN_SMOOTHKEYASUSER            = new StringBuilder ("SmoothKeyAsUser");

        private static StringBuilder IOSN_SCALEFACTOR = new StringBuilder ("ScaleFactor");
        private static StringBuilder IOSN_AXISCONVERSIONMETHOD = new StringBuilder ("AxisConversionMethod");
        private static StringBuilder IOSN_UPAXIS = new StringBuilder ("UpAxis");
        private static StringBuilder IOSN_SELECTIONSETNAMEASPOINTCACHE = new StringBuilder ("SelectionSetNameAsPointCache");

        private static StringBuilder IOSN_KEEPFRAMERATE               = new StringBuilder ("KeepFrameRate");
        private static StringBuilder IOSN_ATTENUATIONASINTENSITYCURVE = new StringBuilder ("AttenuationAsIntensityCurve");

        private static StringBuilder IOSN_RESAMPLE_ANIMATION_CURVES = new StringBuilder ("ResampleAnimationCurves");

        private static StringBuilder IOSN_TIMELINE                    = new StringBuilder ("TimeLine");
        private static StringBuilder IOSN_TIMELINE_SPAN = new StringBuilder ("TimeLineSpan");

        private static StringBuilder IOSN_BUTTON_WEB_UPDATE           = new StringBuilder ("WebUpdateButton");
        private static StringBuilder IOSN_BUTTON_EDIT                 = new StringBuilder ("EditButton");
        private static StringBuilder IOSN_BUTTON_OK                   = new StringBuilder ("OKButton");
        private static StringBuilder IOSN_BUTTON_CANCEL               = new StringBuilder ("CancelButton");
        private static StringBuilder IOSN_MENU_EDIT_PRESET            = new StringBuilder ("EditPresetMenu");
        private static StringBuilder IOSN_MENU_SAVE_PRESET            = new StringBuilder ("SavePresetMenu");

        private static StringBuilder IOSN_UIL                         = new StringBuilder ("UILIndex");
        private static StringBuilder IOSN_PLUGIN_PRODUCT_FAMILY       = new StringBuilder ("PluginProductFamily");

        private static StringBuilder IOSN_PLUGIN_UI_XPOS              = new StringBuilder ("PlugInUIXpos");
        private static StringBuilder IOSN_PLUGIN_UI_YPOS              = new StringBuilder ("PlugInUIYpos");

        private static StringBuilder IOSN_FBX_EXTENTIONS_SDK          = new StringBuilder ("FBXExtentionsSDK");
        private static StringBuilder IOSN_FBX_EXTENTIONS_SDK_WARNING  = new StringBuilder ("FBXExtentionsSDKWarning");

        private static StringBuilder IOSN_COLLADA_FRAME_COUNT         = new StringBuilder ("FrameCount");
        private static StringBuilder IOSN_COLLADA_START               = new StringBuilder ("Start");
        private static StringBuilder IOSN_COLLADA_TAKE_NAME           = new StringBuilder ("TakeName");

        private static StringBuilder IOSN_COLLADA_TRIANGULATE         = new StringBuilder ("Triangulate");
        private static StringBuilder IOSN_COLLADA_SINGLEMATRIX        = new StringBuilder ("SingleMatrix");
        private static StringBuilder IOSN_COLLADA_FRAME_RATE          = new StringBuilder ("FrameRate");

        private static StringBuilder IOSN_DXF_TRIANGULATE             = new StringBuilder ("Triangulate");
        private static StringBuilder IOSN_DXF_DEFORMATION             = new StringBuilder ("Deformation");

        private static StringBuilder IOSN_DXF_WELD_VERTICES           = new StringBuilder ("WeldVertices");
        private static StringBuilder IOSN_DXF_OBJECT_DERIVATION       = new StringBuilder ("ObjectDerivation");
        private static StringBuilder IOSN_DXF_REFERENCE_NODE          = new StringBuilder ("ReferenceNode");

        private static StringBuilder IOSN_OBJ_REFERENCE_NODE          = new StringBuilder ("ReferenceNode");
        private static StringBuilder IOSN_OBJ_TRIANGULATE = new StringBuilder ("Triangulate");
        private static StringBuilder IOSN_OBJ_DEFORMATION             = new StringBuilder ("Deformation");

        private static StringBuilder IOSN_3DS_REFERENCENODE = new StringBuilder ("ReferenceNode");
        private static StringBuilder IOSN_3DS_TEXTURE       = new StringBuilder ("Texture");
        private static StringBuilder IOSN_3DS_MATERIAL      = new StringBuilder ("Material");
        private static StringBuilder IOSN_3DS_ANIMATION     = new StringBuilder ("Animation");
        private static StringBuilder IOSN_3DS_MESH          = new StringBuilder ("Mesh");
        private static StringBuilder IOSN_3DS_LIGHT         = new StringBuilder ("Light");
        private static StringBuilder IOSN_3DS_CAMERA        = new StringBuilder ("Camera");
        private static StringBuilder IOSN_3DS_AMBIENT_LIGHT = new StringBuilder ("AmbientLight");
        private static StringBuilder IOSN_3DS_RESCALING     = new StringBuilder ("Rescaling");
        private static StringBuilder IOSN_3DS_FILTER        = new StringBuilder ("Filter");
        private static StringBuilder IOSN_3DS_SMOOTHGROUP   = new StringBuilder ("Smoothgroup");
        private static StringBuilder IOSN_3DS_TAKE_NAME     = new StringBuilder ("TakeName");
        private static StringBuilder IOSN_3DS_TEXUVBYPOLY = new StringBuilder ("TexuvbyPoly");

        // so far, these three are for 3dsMax plug-in only
        private static StringBuilder IOSN_ZOOMEXTENTS = new StringBuilder ("ZoomExtents");
        private static StringBuilder IOSN_GLOBAL_AMBIENT_COLOR = new StringBuilder ("GlobalAmbientColor");
        private static StringBuilder IOSN_EDGE_ORIENTATION = new StringBuilder ("PreserveEdgeOrientation");

        private static StringBuilder IOSN_VERSIONS_UI_ALIAS           = new StringBuilder ("VersionsUIAlias");
        private static StringBuilder IOSN_VERSIONS_COMP_DESCRIPTIONS  = new StringBuilder ("VersionsCompDescriptions");

        // FBX specific 
        private static StringBuilder IOSN_MODEL_COUNT                 = new StringBuilder ("Model_Count");
        private static StringBuilder IOSN_DEVICE_COUNT                = new StringBuilder ("Device_Count");
        private static StringBuilder IOSN_CHARACTER_COUNT             = new StringBuilder ("Character_Count");
        private static StringBuilder IOSN_ACTOR_COUNT                 = new StringBuilder ("Actor_Count");
        private static StringBuilder IOSN_CONSTRAINT_COUNT            = new StringBuilder ("Constraint_Count");
        private static StringBuilder IOSN_MEDIA_COUNT                 = new StringBuilder ("Media_Count");
        private static StringBuilder IOSN_TEMPLATE                    = new StringBuilder ("Template");
        private static StringBuilder IOSN_PIVOT                       = new StringBuilder ("Pivot");
        private static StringBuilder IOSN_GLOBAL_SETTINGS             = new StringBuilder ("Global_Settings");
        private static StringBuilder IOSN_MERGE_LAYER_AND_TIMEWARP    = new StringBuilder ("Merge_Layer_and_Timewarp");
        private static StringBuilder IOSN_GOBO                        = new StringBuilder ("Gobo");
        private static StringBuilder IOSN_LINK                        = new StringBuilder ("Link");
        private static StringBuilder IOSN_MATERIAL                    = new StringBuilder ("Material");
        private static StringBuilder IOSN_TEXTURE                     = new StringBuilder ("Texture");
        private static StringBuilder IOSN_MODEL                       = new StringBuilder ("Model");
        private static StringBuilder IOSN_EMBEDDED                    = new StringBuilder ("EMBEDDED");
        private static StringBuilder IOSN_PASSWORD                    = new StringBuilder ("Password");
        private static StringBuilder IOSN_PASSWORD_ENABLE             = new StringBuilder ("Password_Enable");
        private static StringBuilder IOSN_CURRENT_TAKE_NAME           = new StringBuilder ("Current_Take_Name");
        private static StringBuilder IOSN_COLLAPSE_EXTERNALS          = new StringBuilder ("COLLAPSE EXTERNALS");
        private static StringBuilder IOSN_COMPRESS_ARRAYS             = new StringBuilder ("Compress_Arrays");
        private static StringBuilder IOSN_COMPRESS_LEVEL              = new StringBuilder ("Compress_Level");
        private static StringBuilder IOSN_COMPRESS_MINSIZE            = new StringBuilder ("Compress_Minsize");
        private static StringBuilder IOSN_EMBEDDED_PROPERTIES_SKIP    = new StringBuilder ("Embedded_Skipped_Properties");
        private static StringBuilder IOSN_EXPORT_FILE_VERSION         = new StringBuilder ("ExportFileVersion");
        private static StringBuilder IOSN_SHOW_UI_WARNING = new StringBuilder ("ShowUIWarning");
        private static StringBuilder IOSN_ADD_MATERIAL_TO_EDIT = new StringBuilder ("AddMaterialToEdit");
        private static StringBuilder IOSN_ENABLE_TEX_DISPLAY          = new StringBuilder ("EnableTexDisplay");
        private static StringBuilder IOSN_PREFERED_ENVELOPPE_SYSTEM   = new StringBuilder ("kImportPreferedEnveloppeSystem");
        private static StringBuilder IOSN_FIRST_TIME_RUN_NOTICE       = new StringBuilder ("FirstTimeRunNotice");
        private static StringBuilder IOSN_EXTRACT_EMBEDDED_DATA       = new StringBuilder ("ExtractEmbeddedData");

        // internal usage
        private static StringBuilder IOSN_USETMPFILEPERIPHERAL = new StringBuilder ("UseTmpFilePeripheral");
        private static StringBuilder IOSN_CONSTRUCTIONHISTORY         = new StringBuilder ("ConstructionHistory");
        private static StringBuilder IOSN_RELAXED_FBX_CHECK           = new StringBuilder ("RelaxedFbxCheck");
        
        private static StringBuilder IMP_ADV_OPT_GRP = new StringBuilder (IOSN_IMPORT + "|" + IOSN_ADV_OPT_GRP);
        private static StringBuilder IMP_FILEFORMAT = new StringBuilder (IMP_ADV_OPT_GRP + "|" + IOSN_FILE_FORMAT);

        
        private static StringBuilder IMP_FBX               = new StringBuilder (IMP_FILEFORMAT + "|" + IOSN_FBX);
        private static StringBuilder IMP_FBX_TEMPLATE      = new StringBuilder (IMP_FBX + "|" + IOSN_TEMPLATE);
        private static StringBuilder IMP_FBX_PIVOT            = new StringBuilder (IMP_FBX + "|" + IOSN_PIVOT);
        private static StringBuilder IMP_FBX_GLOBAL_SETTINGS  = new StringBuilder (IMP_FBX + "|" + IOSN_GLOBAL_SETTINGS);
        private static StringBuilder IMP_FBX_CHARACTER        = new StringBuilder (IMP_FBX + "|" + IOSN_CHARACTER);
        private static StringBuilder IMP_FBX_CONSTRAINT       = new StringBuilder (IMP_FBX + "|" + IOSN_CONSTRAINT);
        private static StringBuilder IMP_FBX_GOBO             = new StringBuilder (IMP_FBX + "|" + IOSN_GOBO);
        private static StringBuilder IMP_FBX_SHAPE            = new StringBuilder (IMP_FBX + "|" + IOSN_SHAPE);
        private static StringBuilder IMP_FBX_LINK             = new StringBuilder (IMP_FBX + "|" + IOSN_LINK);
        private static StringBuilder IMP_FBX_MATERIAL         = new StringBuilder (IMP_FBX + "|" + IOSN_MATERIAL);
        private static StringBuilder IMP_FBX_TEXTURE          = new StringBuilder (IMP_FBX + "|" + IOSN_TEXTURE);
        private static StringBuilder IMP_FBX_MODEL            = new StringBuilder (IMP_FBX + "|" + IOSN_MODEL);
        private static StringBuilder IMP_FBX_ANIMATION        = new StringBuilder (IMP_FBX + "|" + IOSN_ANIMATION);
        private static StringBuilder IMP_FBX_EXTRACT_EMBEDDED_DATA = new StringBuilder (IMP_FBX + "|" + IOSN_EXTRACT_EMBEDDED_DATA);
        
        private static long FBXSDK_TC_ZERO = 0;
        
        public enum EType
        {
            eUnknown,
            eNull,
            eMarker,
            eSkeleton,
            eMesh,
            eNurbs,
            ePatch,
            eCamera,
            eCameraStereo,
            eCameraSwitcher,
            eLight,
            eOpticalReference,
            eOpticalMarker,
            eNurbsCurve,
            eTrimNurbsSurface,
            eBoundary,
            eNurbsSurface,
            eShape,
            eLODGroup,
            eSubDiv,
            eCachedEffect,
            eLine
        };
        
        public enum EUpVector
        {
        eXAxis = 1,
        eYAxis = 2,
        eZAxis = 3
        };

        public enum EFrontVector
        {
        eParityEven = 1,
        eParityOdd = 2
        };
        
        public enum ECoordSystem
            {
            eRightHanded,
            eLeftHanded
        };
        
        enum EPivotSet
        {
                eSourcePivot,           //!< The source pivot context.
                eDestinationPivot       //!< The destination pivot context.
        };
        
        [DllImport("fbx")]
        static public extern IntPtr FbxManagerCreate();

        [DllImport("fbx")]
        static public extern IntPtr FbxIOSettingsCreate(IntPtr pManager, StringBuilder pName);
        
        [DllImport("fbx")]
        static public extern void FbxIOSettingsSetBoolProp(IntPtr ios, StringBuilder pName, bool pValue);
        
        [DllImport("fbx")]
        static public extern void FbxManagerSetIOSettings(IntPtr pManager, IntPtr pIOSettings);
        
        [DllImport("fbx")]
        static public extern IntPtr FbxImporterCreate(IntPtr pManager, StringBuilder pName);
        
        [DllImport("fbx")]
        static public extern bool FbxImporterInitialize(IntPtr imp, StringBuilder pFileName, int pFileFormat, IntPtr pIOSettings);
    
        [DllImport("fbx")]
        static public extern IntPtr FbxManagerGetIOSettings(IntPtr man);
        
        [DllImport("fbx")]
        static public extern IntPtr FbxImporterGetStatus(IntPtr imp);
        
        [DllImport("fbx")]
        static public extern StringBuilder FbxStatusGetErrorString(IntPtr sta);
        
        [DllImport("fbx")]
        static public extern IntPtr FbxSceneCreate(IntPtr pManager, StringBuilder pName);
        
        [DllImport("fbx")]
        static public extern bool FbxImporterImport(IntPtr imp, IntPtr pDocument, bool pNonBlocking = false);
        
        [DllImport("fbx")]
        static public extern double FbxTimeGetFrameRate(int pTimeMode);

        [DllImport("fbx")]
        static public extern int FbxGlobalSettingsGetTimeMode(IntPtr glo);

        [DllImport("fbx")]
        static public extern IntPtr FbxSceneGetGlobalSettings(IntPtr sce);
        
        [DllImport("fbx")]
        static public extern IntPtr FbxSceneGetRootNode(IntPtr sce);
        
        [DllImport("fbx")]
        static public extern void FbxNodeResetPivotSetAndConvertAnimation(IntPtr nod, double pFrameRate=30.0, bool pKeyReduce=false, bool pToNodeCenter=true, bool pForceResetLimits=false);
        
        [DllImport("fbx")]
        static public extern int FbxNodeGetNodeAttributeCount(IntPtr nod);

        [DllImport("fbx")]
        static public extern IntPtr FbxNodeGetNodeAttributeByIndex(IntPtr nod, int pIndex);

        [DllImport("fbx")]
        static public extern int FbxNodeAttributeGetAttributeType(IntPtr att);

        [DllImport("fbx")]
        static public extern int FbxNodeGetChildCount(IntPtr nod, bool pRecursive = false);

        [DllImport("fbx")]
        static public extern IntPtr FbxNodeGetChild(IntPtr nod, int pIndex);

        [DllImport("fbx")]
        static public extern IntPtr FbxMeshGetNode(IntPtr mes, int pIndex = 0);

        [DllImport("fbx")]
        static public extern StringBuilder FbxNodeGetName(IntPtr nod);

        [DllImport("fbx")]
        static public extern int FbxSceneGetMaterialCount(IntPtr sce);

        [DllImport("fbx")]
        static public extern IntPtr FbxSceneGetMaterialByName(IntPtr sce, StringBuilder pName);

        [DllImport("fbx")]
        static public extern IntPtr FbxSceneGetMaterial(IntPtr sce, int pIndex);
        
        [DllImport("fbx")]
        static public extern IntPtr FbxGlobalSettingsGetAxisSystem(IntPtr set);

        [DllImport("fbx")]
        static public extern IntPtr FbxGlobalSettingsGetSystemUnit(IntPtr set);

        [DllImport("fbx")]
        static public extern double FbxSystemUnitGetScaleFactor(IntPtr syu);

        [DllImport("fbx")]
        static public extern int FbxAxisSystemGetUpVector(IntPtr axi, ref int pSign );

        [DllImport("fbx")]
        static public extern int FbxAxisSystemGetFrontVector(IntPtr axi, ref int pSign );

        [DllImport("fbx")]
        static public extern int FbxAxisSystemGetCoordSystem(IntPtr axi);

        [DllImport("fbx")]
        static public extern double[] FbxNodeEvaluateLocalTransform(IntPtr nod, long pTime, int pPivotSet=(int) EPivotSet.eSourcePivot, bool pApplyTarget=false, bool pForceEval=false);





        
        
        private static String [] MappingModeName = new String [] { "None", "ByControlPoint", "ByPolygonVertex", "ByPolygon", "ByEdge", "AllSame" };
        private static String [] MappingModeSuggestion = new String [] { "", "", "", "", " Try using ByPolygon mapping instead.", "" };        
// public:
	public bool AllowUnsignedBlendIndices;

	public Logger logger;
// 
// internal:
	internal IntPtr lSdkManager;
	internal IntPtr lImporter;
	internal IntPtr scene;

	internal String inputFilename;
	internal String vfsOutputFilename;
	internal String inputPath;

	internal Model modelData;
// 
 	internal SceneMapping sceneMapping;
// 	
// 	static array<Byte>^ currentBuffer;
// 
// 	static bool WeightGreater(const std.pair<short, float>& elem1, const std.pair<short, float>& elem2)
// 	{
// 		return elem1.second > elem2.second;
// 	}
// 
// 	bool IsGroupMappingModeByEdge(FbxLayerElement* layerElement)
// 	{
// 		return layerElement.GetMappingMode() == FbxLayerElement.eByEdge;
// 	}
// 
// 	template <class T>
// 	int GetGroupIndexForLayerElementTemplate(FbxLayerElementTemplate<T>* layerElement, int controlPointIndex, int vertexIndex, int edgeIndex, int polygonIndex, String^ meshName, bool& firstTimeError)
// 	{
// 		int groupIndex = 0;
// 		if (layerElement.GetMappingMode() == FbxLayerElement.eByControlPoint)
// 		{
// 			groupIndex = (layerElement.GetReferenceMode() == FbxLayerElement.eIndexToDirect)
// 				? layerElement.GetIndexArray().GetAt(controlPointIndex)
// 				: controlPointIndex;
// 		}
// 		else if (layerElement.GetMappingMode() == FbxLayerElement.eByPolygonVertex)
// 		{
// 			groupIndex = (layerElement.GetReferenceMode() == FbxLayerElement.eIndexToDirect)
// 				? layerElement.GetIndexArray().GetAt(vertexIndex)
// 				: vertexIndex;
// 		}
// 		else if (layerElement.GetMappingMode() == FbxLayerElement.eByPolygon)
// 		{
// 			groupIndex = (layerElement.GetReferenceMode() == FbxLayerElement.eIndexToDirect)
// 				? layerElement.GetIndexArray().GetAt(polygonIndex)
// 				: polygonIndex;
// 		}
// 		else if (layerElement.GetMappingMode() == FbxLayerElement.eByEdge)
// 		{
// 			groupIndex = (layerElement.GetReferenceMode() == FbxLayerElement.eIndexToDirect)
// 				? layerElement.GetIndexArray().GetAt(edgeIndex)
// 				: edgeIndex;
// 		}
// 		else if (layerElement.GetMappingMode() == FbxLayerElement.eAllSame)
// 		{
// 			groupIndex = (layerElement.GetReferenceMode() == FbxLayerElement.eIndexToDirect)
// 				? layerElement.GetIndexArray().GetAt(0)
// 				: 0;
// 		}
// 		else if (firstTimeError)
// 		{
// 			firstTimeError = false;
// 			int mappingMode = layerElement.GetMappingMode();
// 			if (mappingMode > (int)FbxLayerElement.eAllSame)
// 				mappingMode = (int)FbxLayerElement.eAllSame;
// 			string layerName = layerElement.GetName();
// 			logger.Warning(String.Format("'{0}' mapping mode for layer '{1}' in mesh '{2}' is not supported by the FBX importer.{3}",
// 				new String(MappingModeName[mappingMode]),
// 				strlen(layerName) > 0 ? new String(layerName) : new String("Unknown"),
// 				meshName,
// 				new String(MappingModeSuggestion[mappingMode])), (CallerInfo^)nullptr);
// 		}
// 
// 		return groupIndex;
// 	}
// 
// 
    public MeshConverter(Logger Logger)
	{
		if(Logger == null)
			Logger = Core.Diagnostics.GlobalLogger.GetLogger("Importer FBX");

		logger = Logger;
		lSdkManager = IntPtr.Zero;
		lImporter = IntPtr.Zero;
	}

	public void Destroy()
	{
// 		//Marshal.FreeHGlobal((IntPtr)lFilename);
// 		currentBuffer = nullptr;
// 
// 		// The file has been imported; we can get rid of the importer.
// 		lImporter.Destroy();
// 
// 		// Destroy the sdk manager and all other objects it was handling.
// 		lSdkManager.Destroy();
// 
		// -----------------------------------------------------
		// TODO: Workaround with FBX SDK not being multithreaded. 
		// We protect the whole usage of this class with a monitor
		//
		// Lock the whole class between Initialize/Destroy
		// -----------------------------------------------------
		System.Threading.Monitor.Exit( globalLock );
		// -----------------------------------------------------
	}
// 
// 	void ProcessMesh(FbxMesh* pMesh, Dictionary<FbxMesh*, String> meshNames, Dictionary<FbxSurfaceMaterial*, int> materials)
// 	{
// 		// Checks normals availability.
// 		bool has_normals = pMesh.GetElementNormalCount() > 0 && pMesh.GetElementNormal(0).GetMappingMode() != FbxLayerElement.eNone;
// 		bool needEdgeIndexing = false;
// 
// 		// Regenerate normals if necessary
// 		if (!has_normals)
// 		{
// 			pMesh.GenerateNormals(true, false, false);
// 		}
// 
// 		FbxVector4* controlPoints = pMesh.GetControlPoints();
// 		FbxGeometryElementNormal* normalElement = pMesh.GetElementNormal();
// 		FbxGeometryElementTangent* tangentElement = pMesh.GetElementTangent();
// 		FbxGeometryElementBinormal* binormalElement = pMesh.GetElementBinormal();
// 		FbxGeometryElementSmoothing* smoothingElement = pMesh.GetElementSmoothing();
// 
// 		// UV set name mapping
// 		Dictionary<String, int> uvElementMapping;
// 		std.vector<FbxGeometryElementUV*> uvElements;
// 
// 		for (int i = 0; i < pMesh.GetElementUVCount(); ++i)
// 		{
// 			var uvElement = pMesh.GetElementUV(i);
// 			uvElements.push_back(uvElement);
// 			needEdgeIndexing |= IsGroupMappingModeByEdge(uvElement);
// 		}
// 
// 		var meshName = new String(meshNames[pMesh].c_str());
// 
// 		bool hasSkinningPosition = false;
// 		bool hasSkinningNormal = false;
// 		int totalClusterCount = 0;
// 		std.vector<std.vector<std.pair<short, float> > > controlPointWeights;
// 
// 		List<MeshBoneDefinition>^ bones = nullptr;
// 
// 		// Dump skinning information
// 		int skinDeformerCount = pMesh.GetDeformerCount(FbxDeformer.eSkin);
// 		if (skinDeformerCount > 0)
// 		{
// 			bones = new List<MeshBoneDefinition>();
// 			for (int deformerIndex = 0; deformerIndex < skinDeformerCount; deformerIndex++)
// 			{
// 				FbxSkin* skin = FbxCast<FbxSkin>(pMesh.GetDeformer(deformerIndex, FbxDeformer.eSkin));
// 				controlPointWeights.resize(pMesh.GetControlPointsCount());
// 
// 				totalClusterCount = skin.GetClusterCount();
// 				for (int clusterIndex = 0 ; clusterIndex < totalClusterCount; ++clusterIndex)
// 				{
// 					FbxCluster* cluster = skin.GetCluster(clusterIndex);
// 					int indexCount = cluster.GetControlPointIndicesCount();
// 					if (indexCount == 0)
// 					{
// 						continue;
// 					}
// 
// 					FbxNode* link = cluster.GetLink();
// 					string boneName = link.GetName();
// 					int *indices = cluster.GetControlPointIndices();
// 					double *weights = cluster.GetControlPointWeights();
// 
// 					FbxAMatrix transformMatrix;
// 					FbxAMatrix transformLinkMatrix;
// 
// 					cluster.GetTransformMatrix(transformMatrix);
// 					cluster.GetTransformLinkMatrix(transformLinkMatrix);
// 					var globalBindposeInverseMatrix = transformLinkMatrix.Inverse() * transformMatrix;
// 
// 					MeshBoneDefinition bone;
// 					int boneIndex = bones.Count;
// 					bone.NodeIndex = sceneMapping.FindNodeIndex(link);
// 					bone.LinkToMeshMatrix = sceneMapping.ConvertMatrixFromFbx(globalBindposeInverseMatrix);
// 
// 					// Check if the bone was not already there, else update it
// 					// TODO: this is not the correct way to handle multiple deformers (additive...etc.)
// 					bool isBoneAlreadyFound = false;
// 					for (int i = 0; i < bones.Count; i++)
// 					{
// 						if (bones[i].NodeIndex == bone.NodeIndex)
// 						{
// 							bones[i] = bone;
// 							boneIndex = i;
// 							isBoneAlreadyFound = true;
// 							break;
// 						}
// 					}
// 
// 					// Gather skin indices and weights
// 					for (int j = 0 ; j < indexCount; j++)
// 					{
// 						int controlPointIndex = indices[j];
// 						controlPointWeights[controlPointIndex].push_back(std.pair<short, float>((short)boneIndex, (float)weights[j]));
// 					}
// 
// 					// Find an existing bone and update it
// 					// TODO: this is probably not correct to do this (we should handle cluster additive...etc. more correctly here)
// 					if (!isBoneAlreadyFound)
// 					{
// 						bones.Add(bone);
// 					}
// 				}
// 
// 				// look for position/normals skinning
// 				if (pMesh.GetControlPointsCount() > 0)
// 				{
// 					hasSkinningPosition = true;
// 					hasSkinningNormal = (pMesh.GetElementNormal() != NULL);
// 				}
// 
// 				for (int i = 0 ; i < pMesh.GetControlPointsCount(); i++)
// 				{
// 					std.sort(controlPointWeights[i].begin(), controlPointWeights[i].end(), WeightGreater);
// 					controlPointWeights[i].resize(4, std.pair<short, float>(0, 0.0f));
// 					float totalWeight = 0.0f;
// 					for (int j = 0; j < 4; ++j)
// 						totalWeight += controlPointWeights[i][j].second;
// 					if (totalWeight == 0.0f)
// 					{
// 						for (int j = 0; j < 4; ++j)
// 							controlPointWeights[i][j].second = (j == 0) ? 1.0f : 0.0f;
// 					}
// 					else
// 					{
// 						totalWeight = 1.0f / totalWeight;
// 						for (int j = 0; j < 4; ++j)
// 							controlPointWeights[i][j].second *= totalWeight;
// 					}
// 				}
// 			}
// 		}
// 
// 		// *********************************************************************************
// 		// Build the vertex declaration
// 		// *********************************************************************************
// 		var vertexElements = new List<VertexElement>();
// 
// 		// POSITION
// 		int vertexStride = 0;
// 		int positionOffset = vertexStride;
// 		vertexElements.Add(VertexElement.Position<Vector3>(0, vertexStride));
// 		vertexStride += 12;
// 
// 		// NORMAL
// 		int normalOffset = vertexStride;
// 		if (normalElement != NULL)
// 		{
// 			vertexElements.Add(VertexElement.Normal<Vector3>(0, vertexStride));
// 			vertexStride += 12;
// 
// 			needEdgeIndexing |= IsGroupMappingModeByEdge(normalElement);
// 		}
// 
// 		int tangentOffset = vertexStride;
// 		if (tangentElement != NULL)
// 		{
// 			vertexElements.Add(VertexElement.Tangent<Vector4>(0, vertexStride));
// 			vertexStride += 16;
// 
// 			needEdgeIndexing |= IsGroupMappingModeByEdge(tangentElement);
// 		}
// 
// 		// TEXCOORD
// 		std.vector<int> uvOffsets;
// 		for (int i = 0; i < (int)uvElements.size(); ++i)
// 		{
// 			uvOffsets.push_back(vertexStride);
// 			vertexElements.Add(VertexElement.TextureCoordinate<Vector2>(i, vertexStride));
// 			vertexStride += 8;
// 			uvElementMapping[pMesh.GetElementUV(i).GetName()] = i;
// 		}
// 
// 		// BLENDINDICES
// 		int blendIndicesOffset = vertexStride;
// 		bool controlPointIndices16 = (AllowUnsignedBlendIndices && totalClusterCount > 256) || (!AllowUnsignedBlendIndices && totalClusterCount > 128);
// 		if (!controlPointWeights.empty())
// 		{
// 			if (controlPointIndices16)
// 			{
// 				if (AllowUnsignedBlendIndices)
// 				{
// 					vertexElements.Add(VertexElement("BLENDINDICES", 0, PixelFormat.R16G16B16A16_UInt, vertexStride));
// 					vertexStride += sizeof(unsigned short) * 4;
// 				}
// 				else
// 				{
// 					vertexElements.Add(VertexElement("BLENDINDICES", 0, PixelFormat.R16G16B16A16_SInt, vertexStride));
// 					vertexStride += sizeof(short) * 4;
// 				}
// 			}
// 			else
// 			{
// 				if (AllowUnsignedBlendIndices)
// 				{
// 					vertexElements.Add(VertexElement("BLENDINDICES", 0, PixelFormat.R8G8B8A8_UInt, vertexStride));
// 					vertexStride += sizeof(unsigned char) * 4;
// 				}
// 				else
// 				{
// 					vertexElements.Add(VertexElement("BLENDINDICES", 0, PixelFormat.R8G8B8A8_SInt, vertexStride));
// 					vertexStride += sizeof(char) * 4;
// 				}
// 			}
// 		}
// 
// 		// BLENDWEIGHT
// 		int blendWeightOffset = vertexStride;
// 		if (!controlPointWeights.empty())
// 		{
// 			vertexElements.Add(VertexElement("BLENDWEIGHT", 0, PixelFormat.R32G32B32A32_Float, vertexStride));
// 			vertexStride += sizeof(float) * 4;
// 		}
// 
// 		// COLOR
// 		var elementVertexColorCount = pMesh.GetElementVertexColorCount();
// 		std.vector<FbxGeometryElementVertexColor*> vertexColorElements;
// 		int colorOffset = vertexStride;
// 		for (int i = 0; i < elementVertexColorCount; i++)
// 		{
// 			var vertexColorElement = pMesh.GetElementVertexColor(i);
// 			vertexColorElements.push_back(vertexColorElement);
// 			vertexElements.Add(VertexElement.Color<Color>(i, vertexStride));
// 			vertexStride += sizeof(Color);
// 			needEdgeIndexing |= IsGroupMappingModeByEdge(vertexColorElement);
// 		}
// 
// 		// USERDATA
// 		// TODO: USERData how to handle then?
// 		//var userDataCount = pMesh.GetElementUserDataCount();
// 		//for (int i = 0; i < userDataCount; i++)
// 		//{
// 		//	var userData = pMesh.GetElementUserData(i);
// 		//	var dataType = userData.GetDataName(0);
// 		//	Console.WriteLine("DataName {0}", new String(dataType));
// 		//}
// 
// 		// Add the smoothing group information at the end of the vertex declaration
// 		// *************************************************************************
// 		// WARNING - DONT PUT ANY VertexElement after SMOOTHINGGROUP
// 		// *************************************************************************
// 		// Iit is important that to be the LAST ELEMENT of the declaration because it is dropped later in the process by partial memcopys
// 		// SMOOTHINGGROUP
// 		int smoothingOffset = vertexStride;
// 		if (smoothingElement != NULL)
// 		{
// 			vertexElements.Add(VertexElement("SMOOTHINGGROUP", 0, PixelFormat.R32_UInt, vertexStride));
// 			vertexStride += sizeof(int);
// 
// 			needEdgeIndexing |= IsGroupMappingModeByEdge(smoothingElement);
// 		}
// 
// 		int polygonCount = pMesh.GetPolygonCount();
// 
// 		FbxGeometryElement.EMappingMode materialMappingMode = FbxGeometryElement.eNone;
// 		FbxLayerElementArrayTemplate<int>* materialIndices = NULL;
// 
// 		if (pMesh.GetElementMaterial())
// 		{
// 			materialMappingMode = pMesh.GetElementMaterial().GetMappingMode();
// 			materialIndices = &pMesh.GetElementMaterial().GetIndexArray();
// 		}
// 
// 		var buildMeshes = new List<BuildMesh^>();
// 
// 		// Count polygon per materials
// 		for (int i = 0; i < polygonCount; i++)
// 		{
// 			int materialIndex = 0;
// 			if (materialMappingMode == FbxGeometryElement.eByPolygon)
// 			{
// 				materialIndex = materialIndices.GetAt(i);
// 			}
// 
// 			// Equivalent to std.vector.resize()
// 			while (materialIndex >= buildMeshes.Count)
// 			{
// 				buildMeshes.Add(nullptr);
// 			}
// 
// 			if (buildMeshes[materialIndex] == nullptr)
// 				buildMeshes[materialIndex] = new BuildMesh();
// 
// 			int polygonSize = pMesh.GetPolygonSize(i) - 2;
// 			if (polygonSize > 0)
// 				buildMeshes[materialIndex].polygonCount += polygonSize;
// 		}
// 
// 		// Create arrays
// 		for each(BuildMesh^ buildMesh in buildMeshes)
// 		{
// 			if (buildMesh == nullptr)
// 				continue;
// 
// 			buildMesh.buffer = new array<Byte>(vertexStride * buildMesh.polygonCount * 3);
// 		}
// 
// 		bool layerIndexFirstTimeError = true;
// 
// 		if (needEdgeIndexing)
// 			pMesh.BeginGetMeshEdgeIndexForPolygon();
// 
// 		// Build polygons
// 		int polygonVertexStartIndex = 0;
// 		for (int i = 0; i < polygonCount; i++)
// 		{
// 			int materialIndex = 0;
// 			if (materialMappingMode == FbxGeometryElement.eByPolygon)
// 			{
// 				materialIndex = materialIndices.GetAt(i);
// 			}
// 
// 			var buildMesh = buildMeshes[materialIndex];
// 			var buffer = buildMesh.buffer;
// 
// 			int polygonSize = pMesh.GetPolygonSize(i);
// 
// 			for (int polygonFanIndex = 2; polygonFanIndex < polygonSize; ++polygonFanIndex)
// 			{
// 				pin_ptr<Byte> vbPointer = &buffer[buildMesh.bufferOffset];
// 				buildMesh.bufferOffset += vertexStride * 3;
// 
// 				int vertexInPolygon[3] = { 0, polygonFanIndex, polygonFanIndex - 1};
// 				int edgesInPolygon[3];
// 
// 				if (needEdgeIndexing)
// 				{
// 					// Default case for polygon of size 3
// 					// Since our polygon order is 0,2,1, edge order is 2 (edge from 0 to 2),1 (edge from 2 to 1),0 (edge from 1 to 0)
// 					// Note: all that code computing edge should change if vertexInPolygon changes
// 					edgesInPolygon[0] = polygonFanIndex;
// 					edgesInPolygon[1] = polygonFanIndex - 1;
// 					edgesInPolygon[2] = 0;
// 
// 					if (polygonSize > 3)
// 					{
// 						// Since we create non-existing edges inside the fan, we might have to use another edge in those cases
// 						// If edge doesn't exist, we have to use edge from (polygonFanIndex-1) to polygonFanIndex (only one that always exists)
// 
// 						// Let's say polygon is 0,4,3,2,1
// 
// 						// First polygons (except last): 0,2,1 (edge doesn't exist, use the one from 2 to 1 so edge 1)
// 						// Last polygon                : 0,4,3 (edge exists:4, from 0 to 4)
// 						if (polygonFanIndex != polygonSize - 1)
// 							edgesInPolygon[0] = polygonFanIndex - 1;
// 
// 						// First polygon: 0,2,1 (edge exists:0, from 1 to 0)
// 						// Last polygons: 0,4,3 (edge doesn't exist, use the one from 4 to 3 so edge 3)
// 						if (polygonFanIndex != 2)
// 							edgesInPolygon[2] = polygonFanIndex - 1;
// 					}
// 				}
// 
// 				//if (polygonSwap)
// 				//{
// 				//	int temp = vertexInPolygon[1];
// 				//	vertexInPolygon[1] = vertexInPolygon[2];
// 				//	vertexInPolygon[2] = temp;
// 				//}
// 				int controlPointIndices[3] = { pMesh.GetPolygonVertex(i, vertexInPolygon[0]), pMesh.GetPolygonVertex(i, vertexInPolygon[1]), pMesh.GetPolygonVertex(i, vertexInPolygon[2]) };
// 
// 				for (int polygonFanVertex = 0; polygonFanVertex < 3; ++polygonFanVertex)
// 				{
// 					int j = vertexInPolygon[polygonFanVertex];
// 					int vertexIndex = polygonVertexStartIndex + j;
// 					int jNext = vertexInPolygon[(polygonFanVertex + 1) % 3];
// 					int vertexIndexNext = polygonVertexStartIndex + jNext;
// 					int controlPointIndex = controlPointIndices[polygonFanVertex];
// 					int edgeIndex = needEdgeIndexing ? pMesh.GetMeshEdgeIndexForPolygon(i, edgesInPolygon[polygonFanVertex]) : 0;
// 
// 					// POSITION
// 					var controlPoint = sceneMapping.ConvertPointFromFbx(controlPoints[controlPointIndex]);
// 					*(Vector3*)(vbPointer + positionOffset) = controlPoint;
// 
// 					// NORMAL
// 					Vector3 normal = Vector3(1, 0, 0);
// 					if (normalElement != NULL)
// 					{
// 						int normalIndex = GetGroupIndexForLayerElementTemplate(normalElement, controlPointIndex, vertexIndex, edgeIndex, i, meshName, layerIndexFirstTimeError);
// 						var src_normal = normalElement.GetDirectArray().GetAt(normalIndex);
// 						var normalPointer = ((Vector3*)(vbPointer + normalOffset));
// 						normal = sceneMapping.ConvertNormalFromFbx(src_normal);
// 						if (isnan(normal.X) || isnan(normal.Y) || isnan(normal.Z) || normal.Length() < FLT_EPSILON)
// 							normal = Vector3(1, 0, 0);
// 						normal = Vector3.Normalize(normal);
// 						*normalPointer = normal;
// 					}
// 
// 					// UV
// 					for (int uvGroupIndex = 0; uvGroupIndex < (int)uvElements.size(); ++uvGroupIndex)
// 					{
// 						var uvElement = uvElements[uvGroupIndex];
// 						int uvIndex = GetGroupIndexForLayerElementTemplate(uvElement, controlPointIndex, vertexIndex, edgeIndex, i, meshName, layerIndexFirstTimeError);
// 						var uv = uvElement.GetDirectArray().GetAt(uvIndex);
// 
// 						((float*)(vbPointer + uvOffsets[uvGroupIndex]))[0] = (float)uv[0];
// 						((float*)(vbPointer + uvOffsets[uvGroupIndex]))[1] = 1.0f - (float)uv[1];
// 					}
// 
// 					// TANGENT
// 					if (tangentElement != NULL)
// 					{
// 						int tangentIndex = GetGroupIndexForLayerElementTemplate(tangentElement, controlPointIndex, vertexIndex, edgeIndex, i, meshName, layerIndexFirstTimeError);
// 						var src_tangent = tangentElement.GetDirectArray().GetAt(tangentIndex);
// 						var tangentPointer = ((Vector4*)(vbPointer + tangentOffset));
// 						Vector3 tangent = sceneMapping.ConvertNormalFromFbx(src_tangent);
// 						if (isnan(tangent.X) || isnan(tangent.Y) || isnan(tangent.Z) || tangent.Length() < FLT_EPSILON)
// 						{
// 							*tangentPointer = Vector4(1, 0, 0, 1);
// 						}
// 						else
// 						{
// 							tangent = Vector3.Normalize(tangent);
// 
// 							int binormalIndex = GetGroupIndexForLayerElementTemplate(binormalElement, controlPointIndex, vertexIndex, edgeIndex, i, meshName, layerIndexFirstTimeError);
// 							var src_binormal = binormalElement.GetDirectArray().GetAt(binormalIndex);
// 							Vector3 binormal = sceneMapping.ConvertNormalFromFbx(src_binormal);
// 							if (isnan(binormal.X) || isnan(binormal.Y) || isnan(binormal.Z) || binormal.Length() < FLT_EPSILON)
// 							{
// 								*tangentPointer = Vector4(tangent.X, tangent.Y, tangent.Z, 1.0f);
// 							}
// 							else
// 							{
// 								// See GenerateTangentBinormal()
// 								*tangentPointer = Vector4(tangent.X, tangent.Y, tangent.Z, Vector3.Dot(Vector3.Cross(normal, tangent), binormal) < 0.0f ? -1.0f : 1.0f);
// 							}
// 						}
// 					}
// 
// 					// BLENDINDICES and BLENDWEIGHT
// 					if (!controlPointWeights.empty())
// 					{
// 						const var& blendWeights = controlPointWeights[controlPointIndex];
// 						for (int i = 0; i < 4; ++i)
// 						{
// 							if (controlPointIndices16)
// 							{
// 								if (AllowUnsignedBlendIndices)
// 									((unsigned short*)(vbPointer + blendIndicesOffset))[i] = (unsigned short)blendWeights[i].first;
// 								else
// 									((short*)(vbPointer + blendIndicesOffset))[i] = (short)blendWeights[i].first;
// 							}
// 							else
// 							{
// 								if (AllowUnsignedBlendIndices)
// 									((unsigned char*)(vbPointer + blendIndicesOffset))[i] = (unsigned char)blendWeights[i].first;
// 								else
// 									((char*)(vbPointer + blendIndicesOffset))[i] = (char)blendWeights[i].first;
// 							}
// 							((float*)(vbPointer + blendWeightOffset))[i] = blendWeights[i].second;
// 						}
// 					}
// 
// 					// COLOR
// 					for (int elementColorIndex = 0; elementColorIndex < elementVertexColorCount; elementColorIndex++)
// 					{
// 						var vertexColorElement = vertexColorElements[elementColorIndex];
// 						var groupIndex = GetGroupIndexForLayerElementTemplate(vertexColorElement, controlPointIndex, vertexIndex, edgeIndex, i, meshName, layerIndexFirstTimeError);
// 						var color = vertexColorElement.GetDirectArray().GetAt(groupIndex);
// 						((Color*)(vbPointer + colorOffset))[elementColorIndex] = Color((float)color.mRed, (float)color.mGreen, (float)color.mBlue, (float)color.mAlpha);
// 					}
// 
// 					// USERDATA
// 					// TODO HANDLE USERDATA HERE
// 
// 					// SMOOTHINGGROUP
// 					if (smoothingElement != NULL)
// 					{
// 						var groupIndex = GetGroupIndexForLayerElementTemplate(smoothingElement, controlPointIndex, vertexIndex, edgeIndex, i, meshName, layerIndexFirstTimeError);
// 						var group = smoothingElement.GetDirectArray().GetAt(groupIndex);
// 						((int*)(vbPointer + smoothingOffset))[0] = (int)group;
// 					}
// 
// 					vbPointer += vertexStride;
// 				}
// 			}
// 
// 			polygonVertexStartIndex += polygonSize;
// 		}
// 
// 		if (needEdgeIndexing)
// 			pMesh.EndGetMeshEdgeIndexForPolygon();
// 
// 		// Create submeshes
// 		for (int i = 0; i < buildMeshes.Count; ++i)
// 		{
// 			var buildMesh = buildMeshes[i];
// 			if (buildMesh == nullptr)
// 				continue;
// 
// 			var buffer = buildMesh.buffer;
// 			var vertexBufferBinding = VertexBufferBinding(GraphicsSerializerExtensions.ToSerializableVersion(new BufferData(BufferFlags.VertexBuffer, buffer)), new VertexDeclaration(vertexElements.ToArray()), buildMesh.polygonCount * 3, 0, 0);
// 			
// 			var drawData = new MeshDraw();
// 			var vbb = new List<VertexBufferBinding>();
// 			vbb.Add(vertexBufferBinding);
// 			drawData.VertexBuffers = vbb.ToArray();
// 			drawData.PrimitiveType = PrimitiveType.TriangleList;
// 			drawData.DrawCount = buildMesh.polygonCount * 3;
// 
// 			// build the final VertexDeclaration removing the declaration element needed only for the buffer's correct construction
// 			var finalVertexElements = new List<VertexElement>();
// 			for each (VertexElement element in vertexElements)
// 			{
// 				if (element.SemanticName != "SMOOTHINGGROUP")
// 					finalVertexElements.Add(element);
// 			}
// 			var finalDeclaration = new VertexDeclaration(finalVertexElements.ToArray());
// 
// 			// Generate index buffer
// 			// For now, if user requests 16 bits indices but it doesn't fit, it
// 			// won't generate an index buffer, but ideally it should just split it in multiple render calls
// 			IndexExtensions.GenerateIndexBuffer(drawData, finalDeclaration);
// 			/*if (drawData.DrawCount < 65536)
// 			{
// 				IndexExtensions.GenerateIndexBuffer(drawData);
// 			}
// 			else
// 			{
// 				logger.Warning("The index buffer could not be generated with --force-compact-indices because it would use more than 16 bits per index.", nullptr, CallerInfo.Get(__FILEW__, __FUNCTIONW__, __LINE__));
// 			}*/
// 
// 			var lMaterial = pMesh.GetNode().GetMaterial(i);
// 		
// 			// Generate TNB
// 			if (tangentElement == NULL && normalElement != NULL && uvElements.size() > 0)
// 				TNBExtensions.GenerateTangentBinormal(drawData);
// 
// 			var meshData = new Mesh();
// 			meshData.NodeIndex = sceneMapping.FindNodeIndex(pMesh.GetNode());
// 			meshData.Draw = drawData;
// 			if (!controlPointWeights.empty())
// 			{
// 				meshData.Skinning = new MeshSkinningDefinition();
// 				meshData.Skinning.Bones = bones.ToArray();
// 			}
// 
// 			var materialIndex = materials.find(lMaterial);
// 			meshData.MaterialIndex = (materialIndex != materials.end()) ? materialIndex.second : 0;
// 
// 			var meshName = meshNames[pMesh];
// 			if (buildMeshes.Count > 1)
// 				meshName = meshName + "_" + std.to_string(i + 1);
// 			meshData.Name = new String(meshName.c_str());
// 			
// 			if (hasSkinningPosition || hasSkinningNormal || totalClusterCount > 0)
// 			{
// 				if (hasSkinningPosition)
// 					meshData.Parameters.Set(MaterialKeys.HasSkinningPosition, true);
// 				if (hasSkinningNormal)
// 					meshData.Parameters.Set(MaterialKeys.HasSkinningNormal, true);
// 			}
// 			modelData.Meshes.Add(meshData);
// 		}
// 	}
// 
// 	// return a boolean indicating whether the built material is transparent or not
// 	MaterialAsset^ ProcessMeshMaterialAsset(FbxSurfaceMaterial* lMaterial, Dictionary<String, size_t>& uvElementMapping)
// 	{
// 		var uvEltMappingOverride = uvElementMapping;
// 		var textureMap = new Dictionary<IntPtr, ComputeTextureColor^>();
// 		Dictionary<String, int> textureNameCount;
// 
// 		var finalMaterial = new Stride.Assets.Materials.MaterialAsset();
// 		
// 		var phongSurface = FbxCast<FbxSurfacePhong>(lMaterial);
// 		var lambertSurface = FbxCast<FbxSurfaceLambert>(lMaterial);
// 
// 		{   // The diffuse color
// 			var diffuseTree = (IComputeColor^)GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sDiffuse, FbxSurfaceMaterial.sDiffuseFactor, finalMaterial);
// 			if(lambertSurface || diffuseTree != nullptr)
// 			{
// 				if(diffuseTree == nullptr)	
// 				{
// 					var diffuseColor = lambertSurface.Diffuse.Get();
// 					var diffuseFactor = lambertSurface.DiffuseFactor.Get();
// 					var diffuseColorValue = diffuseFactor * diffuseColor;
// 
// 					// Create diffuse value even if the color is black
// 					diffuseTree = new ComputeColor(FbxDouble3ToColor4(diffuseColorValue));
// 				}
// 
// 				if (diffuseTree != nullptr)
// 				{
// 					finalMaterial.Attributes.Diffuse = new MaterialDiffuseMapFeature(diffuseTree);
// 					finalMaterial.Attributes.DiffuseModel = new MaterialDiffuseLambertModelFeature();
// 				}
// 			}
// 		}
// 		{   // The emissive color
// 			var emissiveTree = (IComputeColor^)GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sEmissive, FbxSurfaceMaterial.sEmissiveFactor, finalMaterial);
// 			if(lambertSurface || emissiveTree != nullptr)
// 			{
// 				if(emissiveTree == nullptr)	
// 				{
// 					var emissiveColor = lambertSurface.Emissive.Get();
// 					var emissiveFactor = lambertSurface.EmissiveFactor.Get();
// 					var emissiveColorValue = emissiveFactor * emissiveColor;
// 
// 					// Do not create the node if the value has not been explicitly specified by the user.
// 					if(emissiveColorValue != FbxDouble3(0))
// 					{
// 						emissiveTree = new ComputeColor(FbxDouble3ToColor4(emissiveColorValue));
// 					}
// 				}
// 
// 				if (emissiveTree != nullptr)
// 				{
// 					finalMaterial.Attributes.Emissive = new MaterialEmissiveMapFeature(emissiveTree);
// 				}
// 			}
// 		}
// 		// TODO: Check if we want to support Ambient Color
// 		//{   // The ambient color
// 		//	var ambientTree = GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sAmbient, FbxSurfaceMaterial.sAmbientFactor, finalMaterial);
// 		//	if(lambertSurface || ambientTree != nullptr)
// 		//	{
// 		//		if(ambientTree == nullptr)	
// 		//		{
// 		//			var ambientColor = lambertSurface.Emissive.Get();
// 		//			var ambientFactor = lambertSurface.EmissiveFactor.Get();
// 		//			var ambientColorValue = ambientFactor * ambientColor;
// 
// 		//			// Do not create the node if the value has not been explicitly specified by the user.
// 		//			if(ambientColorValue != FbxDouble3(0))
// 		//			{
// 		//				ambientTree = new ComputeColor(FbxDouble3ToColor4(ambientColorValue));
// 		//			}
// 		//		}
// 
// 		//		if(ambientTree != nullptr)
// 		//			finalMaterial.AddColorNode(MaterialParameters.AmbientMap, "ambient", ambientTree);
// 		//	}
// 		//}
// 		{   // The normal map
// 			var normalMapTree = (IComputeColor^)GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sNormalMap, NULL, finalMaterial);
// 			if(lambertSurface || normalMapTree != nullptr)
// 			{
// 				if(normalMapTree == nullptr)	
// 				{
// 					var normalMapValue = lambertSurface.NormalMap.Get();
// 
// 					// Do not create the node if the value has not been explicitly specified by the user.
// 					if(normalMapValue != FbxDouble3(0))
// 					{
// 						normalMapTree = new ComputeFloat4(FbxDouble3ToVector4(normalMapValue));
// 					}
// 				}
// 				
// 				if (normalMapTree != nullptr)
// 				{
// 					finalMaterial.Attributes.Surface = new MaterialNormalMapFeature(normalMapTree);
// 				}
// 			}
// 		}
// 		// TODO: Support for BumpMap
// 		//{   // The bump map
// 		//	var bumpMapTree = GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sBump, FbxSurfaceMaterial.sBumpFactor, finalMaterial);
// 		//	if(lambertSurface || bumpMapTree != nullptr)
// 		//	{
// 		//		if(bumpMapTree == nullptr)	
// 		//		{
// 		//			var bumpValue = lambertSurface.Bump.Get();
// 		//			var bumpFactor = lambertSurface.BumpFactor.Get();
// 		//			var bumpMapValue = bumpFactor * bumpValue;
// 
// 		//			// Do not create the node if the value has not been explicitly specified by the user.
// 		//			if(bumpMapValue != FbxDouble3(0))
// 		//			{
// 		//				bumpMapTree = new MaterialFloat4ComputeColor(FbxDouble3ToVector4(bumpMapValue));
// 		//			}
// 		//		}
// 		//		
// 		//		if (bumpMapTree != nullptr)
// 		//		{
// 		//			finalMaterial.AddColorNode(MaterialParameters.BumpMap, "bumpMap", bumpMapTree);
// 		//		}
// 		//	}
// 		//}
// 		// TODO: Support for Transparency
// 		//{   // The transparency
// 		//	var transparencyTree = GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sTransparentColor, FbxSurfaceMaterial.sTransparencyFactor, finalMaterial);
// 		//	if(lambertSurface || transparencyTree != nullptr)
// 		//	{
// 		//		if(transparencyTree == nullptr)	
// 		//		{
// 		//			var transparencyColor = lambertSurface.TransparentColor.Get();
// 		//			var transparencyFactor = lambertSurface.TransparencyFactor.Get();
// 		//			var transparencyValue = transparencyFactor * transparencyColor;
// 		//			var opacityValue = std.min(1.0f, std.max(0.0f, 1-(float)transparencyValue[0]));
// 
// 		//			// Do not create the node if the value has not been explicitly specified by the user.
// 		//			if(opacityValue < 1)
// 		//			{
// 		//				transparencyTree = new MaterialFloatComputeColor(opacityValue);
// 		//			}
// 		//		}
// 
// 		//		if(transparencyTree != nullptr)
// 		//			finalMaterial.AddColorNode(MaterialParameters.TransparencyMap, "transparencyMap", transparencyTree);
// 		//	}
// 		//}
// 		//// TODO: Support for displacement map
// 		//{   // The displacement map
// 		//	var displacementColorTree = GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sDisplacementColor, FbxSurfaceMaterial.sDisplacementFactor, finalMaterial);
// 		//	if(lambertSurface || displacementColorTree != nullptr)
// 		//	{
// 		//		if(displacementColorTree == nullptr)	
// 		//		{
// 		//			var displacementColor = lambertSurface.DisplacementColor.Get();
// 		//			var displacementFactor = lambertSurface.DisplacementFactor.Get();
// 		//			var displacementValue = displacementFactor * displacementColor;
// 
// 		//			// Do not create the node if the value has not been explicitly specified by the user.
// 		//			if(displacementValue != FbxDouble3(0))
// 		//			{
// 		//				displacementColorTree = new MaterialFloat4ComputeColor(FbxDouble3ToVector4(displacementValue));
// 		//			}
// 		//		}
// 		//		
// 		//		if(displacementColorTree != nullptr)
// 		//			finalMaterial.AddColorNode(MaterialParameters.DisplacementMap, "displacementMap", displacementColorTree);
// 		//	}
// 		//}
// 		{	// The specular color
// 			var specularTree = (IComputeColor^)GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sSpecular, NULL, finalMaterial);
// 			if(phongSurface || specularTree != nullptr)
// 			{
// 				if(specularTree == nullptr)	
// 				{
// 					var specularColor = phongSurface.Specular.Get();
// 		
// 					// Do not create the node if the value has not been explicitly specified by the user.
// 					if(specularColor != FbxDouble3(0))
// 					{
// 						specularTree = new ComputeColor(FbxDouble3ToColor4(specularColor));
// 					}
// 				}
// 						
// 				if (specularTree != nullptr)
// 				{
// 					var specularFeature = new MaterialSpecularMapFeature();
// 					specularFeature.SpecularMap = specularTree;
// 					finalMaterial.Attributes.Specular = specularFeature;
// 
// 					var specularModel = new MaterialSpecularMicrofacetModelFeature();
// 					specularModel.Fresnel = new MaterialSpecularMicrofacetFresnelSchlick();
// 					specularModel.Visibility = new MaterialSpecularMicrofacetVisibilityImplicit();
// 					specularModel.NormalDistribution = new MaterialSpecularMicrofacetNormalDistributionBlinnPhong();
// 
// 					finalMaterial.Attributes.SpecularModel = specularModel;
// 				}
// 			}
// 		}
// 		// TODO REPLUG SPECULAR INTENSITY
// 	//{	// The specular intensity map
// 	//		var specularIntensityTree = (IComputeColor^)GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sSpecularFactor, NULL, finalMaterial);
// 	//		if(phongSurface || specularIntensityTree != nullptr)
// 	//		{
// 	//			if(specularIntensityTree == nullptr)	
// 	//			{
// 	//				var specularIntensity = phongSurface.SpecularFactor.Get();
// 	//	
// 	//				// Do not create the node if the value has not been explicitly specified by the user.
// 	//				if(specularIntensity > 0)
// 	//				{
// 	//					specularIntensityTree = new MaterialFloatComputeNode((float)specularIntensity);
// 	//				}
// 	//			}
// 	//					
// 	//			if (specularIntensityTree != nullptr)
// 	//			{
// 	//				MaterialSpecularMapFeature^ specularFeature;
// 	//				if (finalMaterial.Attributes.Specular == nullptr || finalMaterial.Attributes.Specular.GetType() != MaterialSpecularMapFeature.typeid)
// 	//				{
// 	//					specularFeature = new MaterialSpecularMapFeature();
// 	//				}
// 	//				else
// 	//				{
// 	//					specularFeature = (MaterialSpecularMapFeature^)finalMaterial.Attributes.Specular;
// 	//				}
// 	//				// TODO: Check Specular Intensity and Power
// 	//				specularFeature.Intensity = specularIntensityTree;
// 	//				finalMaterial.Attributes.Specular = specularFeature;
// 	//			}
// 	//		}
// 	//	}
// 	/*			{	// The specular power map
// 			var specularPowerTree = GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sShininess, NULL, finalMaterial);
// 			if(phongSurface || specularPowerTree != nullptr)
// 			{
// 				if(specularPowerTree == nullptr)	
// 				{
// 					var specularPower = phongSurface.Shininess.Get();
// 		
// 					// Do not create the node if the value has not been explicitly specified by the user.
// 					if(specularPower > 0)
// 					{
// 						specularPowerTree = new MaterialFloatComputeColor((float)specularPower);
// 					}
// 				}
// 						
// 				if (specularPowerTree != nullptr)		
// 				{
// 					MaterialSpecularMapFeature^ specularFeature;
// 					if (finalMaterial.Attributes.Specular == nullptr || finalMaterial.Attributes.Specular.GetType() != MaterialSpecularMapFeature.typeid)
// 					{
// 						specularFeature = new MaterialSpecularMapFeature();
// 					}
// 					else
// 					{
// 						specularFeature = (MaterialSpecularMapFeature^)finalMaterial.Attributes.Specular;
// 					}
// 					// TODO: Check Specular Intensity and Power
// 					specularFeature.Intensity = specularPowerTree;
// 					finalMaterial.Attributes.Specular = specularFeature;
// 				}
// 			}
// 		}*/
// 		//// TODO: Support for reflection map
// 		//{   // The reflection map
// 		//	var reflectionMapTree = GenerateSurfaceTextureTree(lMaterial, uvEltMappingOverride, textureMap, textureNameCount, FbxSurfaceMaterial.sReflection, FbxSurfaceMaterial.sReflectionFactor, finalMaterial);
// 		//	if(phongSurface || reflectionMapTree != nullptr)
// 		//	{
// 		//		if(reflectionMapTree == nullptr)	
// 		//		{
// 		//			var reflectionColor = lambertSurface.DisplacementColor.Get();
// 		//			var reflectionFactor = lambertSurface.DisplacementFactor.Get();
// 		//			var reflectionValue = reflectionFactor * reflectionColor;
// 
// 		//			// Do not create the node if the value has not been explicitly specified by the user.
// 		//			if(reflectionValue != FbxDouble3(0))
// 		//			{
// 		//				reflectionMapTree = new ComputeColor(FbxDouble3ToColor4(reflectionValue));
// 		//			}
// 		//		}
// 		//		
// 		//		if(reflectionMapTree != nullptr)
// 		//			finalMaterial.AddColorNode(MaterialParameters.ReflectionMap, "reflectionMap", reflectionMapTree);
// 		//	}
// 		//}
// 		return finalMaterial;
// 	}
// 
// 	bool IsTransparent(FbxSurfaceMaterial* lMaterial)
// 	{
// 		for (int i = 0; i < 2; ++i)
// 		{
// 			var propertyName = i == 0 ? FbxSurfaceMaterial.sTransparentColor : FbxSurfaceMaterial.sTransparencyFactor;
// 			if (propertyName == NULL)
// 				continue;
// 
// 			FbxProperty lProperty = lMaterial.FindProperty(propertyName);
// 			if (lProperty.IsValid())
// 			{
// 				const int lTextureCount = lProperty.GetSrcObjectCount<FbxTexture>();
// 				for (int j = 0; j < lTextureCount; ++j)
// 				{
// 					FbxLayeredTexture *lLayeredTexture = FbxCast<FbxLayeredTexture>(lProperty.GetSrcObject<FbxTexture>(j));
// 					FbxFileTexture *lFileTexture = FbxCast<FbxFileTexture>(lProperty.GetSrcObject<FbxTexture>(j));
// 					if (lLayeredTexture)
// 					{
// 						int lNbTextures = lLayeredTexture.GetSrcObjectCount<FbxFileTexture>();
// 						if (lNbTextures > 0)
// 							return true;
// 					}
// 					else if (lFileTexture)
// 						return true;
// 				}
// 				if (lTextureCount == 0)
// 				{
// 					var val = FbxDouble3ToVector3(lProperty.Get<FbxDouble3>());
// 					if (val == Vector3.Zero || val != Vector3.One)
// 						return true;
// 				}
// 			}
// 		}
// 		return false;
// 	}
// 
// 	IComputeNode^ GenerateSurfaceTextureTree(FbxSurfaceMaterial* lMaterial, Dictionary<String, size_t>& uvElementMapping, Dictionary<IntPtr, ComputeTextureColor^>^ textureMap,
// 												Dictionary<String, int>& textureNameCount, char const* surfaceMaterial, char const* surfaceMaterialFactor,
// 												Stride.Assets.Materials.MaterialAsset^ finalMaterial)
// 	{
// 		var compositionTrees = new cli.array<IComputeColor^>(2);
// 
// 		for (int i = 0; i < 2; ++i)
// 		{
// 			// Scan first for component name, then its factor (i.e. sDiffuse, then sDiffuseFactor)
// 			var propertyName = i == 0 ? surfaceMaterial : surfaceMaterialFactor;
// 			if (propertyName == NULL)
// 				continue;
// 
// 			int compositionCount = 0;
// 			
// 			FbxProperty lProperty = lMaterial.FindProperty(propertyName);
// 			if (lProperty.IsValid())
// 			{
// 				IComputeColor^ previousNode = nullptr;
// 				const int lTextureCount = lProperty.GetSrcObjectCount<FbxTexture>();
// 				for (int j = 0; j < lTextureCount; ++j)
// 				{
// 					FbxLayeredTexture *lLayeredTexture = FbxCast<FbxLayeredTexture>(lProperty.GetSrcObject<FbxTexture>(j));
// 					FbxFileTexture *lFileTexture = FbxCast<FbxFileTexture>(lProperty.GetSrcObject<FbxTexture>(j));
// 					if (lLayeredTexture)
// 					{
// 						int lNbTextures = lLayeredTexture.GetSrcObjectCount<FbxFileTexture>();
// 						for (int k = 0; k < lNbTextures; ++k)
// 						{
// 							FbxFileTexture* lSubTexture = FbxCast<FbxFileTexture>(lLayeredTexture.GetSrcObject<FbxFileTexture>(k));
// 
// 							var uvName = String(lSubTexture.UVSet.Get());
// 							if (uvElementMapping.find(uvName) == uvElementMapping.end())
// 								uvElementMapping[uvName] = uvElementMapping.size();
// 
// 							var currentMaterialReference = GenerateMaterialTextureNodeFBX(lSubTexture, uvElementMapping, textureMap, textureNameCount, finalMaterial);
// 							
// 							if (lNbTextures == 1 || compositionCount == 0)
// 							{
// 								if (previousNode == nullptr)
// 									previousNode = currentMaterialReference;
// 								else
// 									previousNode = new ComputeBinaryColor(previousNode, currentMaterialReference, BinaryOperator.Add); // not sure
// 							}
// 							else
// 							{
// 								var newNode = new ComputeBinaryColor(previousNode, currentMaterialReference, BinaryOperator.Add);
// 								previousNode = newNode;
// 								
// 								FbxLayeredTexture.EBlendMode blendMode;
// 								lLayeredTexture.GetTextureBlendMode(k, blendMode);
// 								newNode.Operator = BlendModeToBlendOperand(blendMode);								
// 							}
// 
// 							compositionCount++;
// 						}
// 					}
// 					else if (lFileTexture)
// 					{
// 						compositionCount++;
// 
// 						var newMaterialReference = GenerateMaterialTextureNodeFBX(lFileTexture, uvElementMapping, textureMap, textureNameCount, finalMaterial);
// 						
// 						if (previousNode == nullptr)
// 							previousNode = newMaterialReference;
// 						else
// 							previousNode = new ComputeBinaryColor(previousNode, newMaterialReference, BinaryOperator.Add); // not sure
// 					}
// 				}
// 
// 				compositionTrees[i] = previousNode;
// 			}
// 		}
// 
// 		// If we only have one of either Color or Factor, use directly, otherwise multiply them together
// 		IComputeColor^ compositionTree;
// 		if (compositionTrees[0] == nullptr) // TODO do we want only the factor??? . delete
// 		{
// 			compositionTree = compositionTrees[1];
// 		}
// 		else if (compositionTrees[1] == nullptr)
// 		{
// 			compositionTree = compositionTrees[0];
// 		}
// 		else
// 		{
// 			compositionTree = new ComputeBinaryColor(compositionTrees[0], compositionTrees[1], BinaryOperator.Multiply);
// 		}
// 
// 		return compositionTree;
// 	}
// 
// 	BinaryOperator BlendModeToBlendOperand(FbxLayeredTexture.EBlendMode blendMode)
// 	{
// 		switch (blendMode)
// 		{
// 		case FbxLayeredTexture.eOver:
// 			return BinaryOperator.Over;
// 		case FbxLayeredTexture.eAdditive:
// 			return BinaryOperator.Add;
// 		case FbxLayeredTexture.eModulate:
// 			return BinaryOperator.Multiply;
// 		//case FbxLayeredTexture.eTranslucent:
// 		//	return BinaryOperator.Multiply;
// 		//case FbxLayeredTexture.eModulate2:
// 		//	return BinaryOperator.Multiply;
// 		//case FbxLayeredTexture.eNormal:
// 		//	return BinaryOperator.Multiply;
// 		//case FbxLayeredTexture.eDissolve:
// 		//	return BinaryOperator.Multiply;
// 		case FbxLayeredTexture.eDarken:
// 			return BinaryOperator.Darken;
// 		case FbxLayeredTexture.eColorBurn:
// 			return BinaryOperator.ColorBurn;
// 		case FbxLayeredTexture.eLinearBurn:
// 			return BinaryOperator.LinearBurn;
// 		//case FbxLayeredTexture.eDarkerColor:
// 		//	return BinaryOperator.Multiply;
// 		case FbxLayeredTexture.eLighten:
// 			return BinaryOperator.Lighten;
// 		case FbxLayeredTexture.eScreen:
// 			return BinaryOperator.Screen;
// 		case FbxLayeredTexture.eColorDodge:
// 			return BinaryOperator.ColorDodge;
// 		case FbxLayeredTexture.eLinearDodge:
// 			return BinaryOperator.LinearDodge;
// 		//case FbxLayeredTexture.eLighterColor:
// 		//	return BinaryOperator.Multiply;
// 		case FbxLayeredTexture.eSoftLight:
// 			return BinaryOperator.SoftLight;
// 		case FbxLayeredTexture.eHardLight:
// 			return BinaryOperator.HardLight;
// 		//case FbxLayeredTexture.eVividLight:
// 		//	return BinaryOperator.Multiply;
// 		//case FbxLayeredTexture.eLinearLight:
// 		//	return BinaryOperator.Multiply;
// 		case FbxLayeredTexture.ePinLight:
// 			return BinaryOperator.PinLight;
// 		case FbxLayeredTexture.eHardMix:
// 			return BinaryOperator.HardMix;
// 		case FbxLayeredTexture.eDifference:
// 			return BinaryOperator.Difference;
// 		case FbxLayeredTexture.eExclusion:
// 			return BinaryOperator.Exclusion;
// 		case FbxLayeredTexture.eSubtract:
// 			return BinaryOperator.Subtract;
// 		case FbxLayeredTexture.eDivide:
// 			return BinaryOperator.Divide;
// 		case FbxLayeredTexture.eHue:
// 			return BinaryOperator.Hue;
// 		case FbxLayeredTexture.eSaturation:
// 			return BinaryOperator.Saturation;
// 		//case FbxLayeredTexture.eColor:
// 		//	return BinaryOperator.Multiply;
// 		//case FbxLayeredTexture.eLuminosity:
// 		//	return BinaryOperator.Multiply;
// 		case FbxLayeredTexture.eOverlay:
// 			return BinaryOperator.Overlay;
// 		default:
// 			logger.Error(String.Format("Material blending mode '{0}' is not supported yet. Multiplying blending mode will be used instead.", new Int32(blendMode)), (CallerInfo^)nullptr);
// 			return BinaryOperator.Multiply;
// 		}
// 	}
// 
// 	ShaderClassSource^ GenerateTextureLayerFBX(FbxFileTexture* lFileTexture, Dictionary<String, int>& uvElementMapping, Mesh^ meshData, int& textureCount, ParameterKey<Texture^>^ surfaceMaterialKey)
// 	{
// 		var texScale = lFileTexture.GetUVScaling();
// 		var texturePath = FindFilePath(lFileTexture);
// 
// 		return TextureLayerGenerator.GenerateTextureLayer(vfsOutputFilename, texturePath, uvElementMapping[String(lFileTexture.UVSet.Get())], Vector2((float)texScale[0], (float)texScale[1]) , 
// 									textureCount, surfaceMaterialKey,
// 									meshData,
// 									nullptr);
// 	}
// 
// 	String^ FindFilePath(FbxFileTexture* lFileTexture)
// 	{		
// 		var relFileName = new String(lFileTexture.GetRelativeFileName());
// 		var absFileName = new String(lFileTexture.GetFileName());
// 
// 		// First try to get the texture filename by relative path, if not valid then use absolute path
// 		// (According to FBX doc, resolved first by absolute name, and relative name if absolute name is not valid)
// 		var fileNameToUse = Path.Combine(inputPath, relFileName);
// 		if(fileNameToUse.StartsWith("\\\\", StringComparison.Ordinal))
// 		{
// 			logger.Warning(String.Format("Importer detected a network address in referenced assets. This may temporary block the build if the file does not exist. [Address='{0}']", fileNameToUse), (CallerInfo^)nullptr);
// 		}
// 		if (!File.Exists(fileNameToUse) && !String.IsNullOrEmpty(absFileName))
// 		{
// 			fileNameToUse = absFileName;
// 		}
// 
// 		// Make sure path is absolute
// 		if (!(new UFile(fileNameToUse)).IsAbsolute)
// 		{
// 			fileNameToUse = Path.Combine(inputPath, fileNameToUse);
// 		}
// 
// 		return fileNameToUse;
// 	}
// 
// 	ComputeTextureColor^ GenerateMaterialTextureNodeFBX(FbxFileTexture* lFileTexture, Dictionary<String, size_t>& uvElementMapping, Dictionary<IntPtr, ComputeTextureColor^>^ textureMap, Dictionary<String, int>& textureNameCount, Stride.Assets.Materials.MaterialAsset^ finalMaterial)
// 	{
// 		var texScale = lFileTexture.GetUVScaling();		
// 		var texturePath = FindFilePath(lFileTexture);
// 		var wrapModeU = lFileTexture.GetWrapModeU();
// 		var wrapModeV = lFileTexture.GetWrapModeV();
// 		var wrapTextureU = (wrapModeU == FbxTexture.EWrapMode.eRepeat) ? TextureAddressMode.Wrap : TextureAddressMode.Clamp;
// 		var wrapTextureV = (wrapModeV == FbxTexture.EWrapMode.eRepeat) ? TextureAddressMode.Wrap : TextureAddressMode.Clamp;
// 		
// 		ComputeTextureColor^ textureValue;
// 		
// 		if (textureMap.TryGetValue(IntPtr(lFileTexture), textureValue))
// 		{
// 			return textureValue;
// 		}
// 		else
// 		{
// 			textureValue = TextureLayerGenerator.GenerateMaterialTextureNode(vfsOutputFilename, texturePath, uvElementMapping[String(lFileTexture.UVSet.Get())], Vector2((float)texScale[0], (float)texScale[1]), wrapTextureU, wrapTextureV, nullptr);
// 
// 			var attachedReference = AttachedReferenceManager.GetAttachedReference(textureValue.Texture);
// 
// 			var textureNamePtr = Marshal.StringToHGlobalAnsi(attachedReference.Url);
// 			String textureName = String((char*)textureNamePtr.ToPointer());
// 			Marshal. FreeHGlobal(textureNamePtr);
// 
// 			var textureCount = GetTextureNameCount(textureNameCount, textureName);
// 			if (textureCount > 1)
// 				textureName = textureName + "_" + std.to_string(textureCount - 1);
// 
// 			var referenceName = new String(textureName.c_str());
// 			//var materialReference = new MaterialReferenceNode(referenceName);
// 			//finalMaterial.AddNode(referenceName, textureValue);
// 			textureMap[IntPtr(lFileTexture)] = textureValue;
// 			return textureValue;
// 		}
// 		
// 		return nullptr;
// 	}
// 
// 	int GetTextureNameCount(Dictionary<String, int>& textureNameCount, String textureName)
// 	{
// 		var textureFound = textureNameCount.find(textureName);
// 		if (textureFound == textureNameCount.end())
// 			textureNameCount[textureName] = 1;
// 		else
// 			textureNameCount[textureName] = textureNameCount[textureName] + 1;
// 		return textureNameCount[textureName];
// 	}
// 
	void ProcessAttribute(/*FbxNode*/IntPtr pNode, /*FbxNodeAttribute*/IntPtr pAttribute, Dictionary<IntPtr, String> meshNames, Dictionary<IntPtr, int> materials)
	{
		if(pAttribute == IntPtr.Zero) return;
 
		if ((EType) FbxNodeAttributeGetAttributeType(pAttribute) == EType.eMesh)
		{
			ProcessMesh(/*(FbxMesh*)*/pAttribute, meshNames, materials);
		}
	}

	void ProcessNodeTransformation(/*FbxNode*/IntPtr pNode)
	{
		var nodeIndex = sceneMapping.FindNodeIndex(pNode);
		var nodes = sceneMapping.Nodes;
		var node = nodes[nodeIndex];

		// Use GlobalTransform instead of LocalTransform

		var fbxMatrix = FbxNodeEvaluateLocalTransform (pNode, FBXSDK_TC_ZERO);
		var matrix = sceneMapping.ConvertMatrixFromFbx(fbxMatrix);

		// Extract the translation and scaling
		Vector3 translation;
		Quaternion rotation;
		Vector3 scaling;
		matrix.Decompose(out scaling, out rotation, out translation);

		// Apply rotation on top level nodes only
		if (node.ParentIndex == 0)
		{
			Vector3.TransformCoordinate(translation, sceneMapping.AxisSystemRotationMatrix, out translation);
			rotation = Quaternion.Multiply(rotation, Quaternion.RotationMatrix(sceneMapping.AxisSystemRotationMatrix));
		}

		// Setup the transform for this node
		node.Transform.Position = translation;
		node.Transform.Rotation = rotation;
		node.Transform.Scale = scaling;

		// Recursively process the children nodes.
		for (int j = 0; j < FbxNodeGetChildCount(pNode); j++)
		{
			ProcessNodeTransformation(FbxNodeGetChild(pNode, j));
		}
	}

	void ProcessNodeAttributes(/*FbxNode*/IntPtr pNode, Dictionary</*FbxMesh*/IntPtr, String> meshNames, Dictionary</*FbxSurfaceMaterial*/IntPtr, int> materials)
	{
		// Process the node's attributes.
		for(int i = 0; i < FbxNodeGetNodeAttributeCount(pNode); i++)
			ProcessAttribute(pNode, FbxNodeGetNodeAttributeByIndex(pNode, i), meshNames, materials);

		// Recursively process the children nodes.
		for(int j = 0; j < FbxNodeGetChildCount(pNode); j++)
		{
			ProcessNodeAttributes(FbxNodeGetChild(pNode, j), meshNames, materials);
		}
	}

// 	ref class BuildMesh
// 	{
// 	public:
// 		array<Byte>^ buffer;
// 		int bufferOffset;
// 		int polygonCount;
// 	};
// 
	public struct ImportConfiguration
	{
	
		public bool ImportTemplates;
		public bool ImportPivots;
		public bool ImportGlobalSettings;
		public bool ImportCharacters;
		public bool ImportConstraints;
		public bool ImportGobos;
		public bool ImportShapes;
		public bool ImportLinks;
		public bool ImportMaterials;
		public bool ImportTextures;
		public bool ImportModels;
		public bool ImportAnimations;
		public bool ExtractEmbeddedData;

	
		public static ImportConfiguration ImportAll()
		{
			var config = new ImportConfiguration();

			config.ImportTemplates = true;
			config.ImportPivots = true;
			config.ImportGlobalSettings = true;
			config.ImportCharacters = true;
			config.ImportConstraints = true;
			config.ImportGobos = true;
			config.ImportShapes = true;
			config.ImportLinks = true;
			config.ImportMaterials = true;
			config.ImportTextures = true;
			config.ImportModels = true;
			config.ImportAnimations = true;
			config.ExtractEmbeddedData = true;

			return config;
		}

		public static ImportConfiguration ImportModelOnly()
		{
			var config = new ImportConfiguration();

			config.ImportTemplates = false;
			config.ImportPivots = false;
			config.ImportGlobalSettings = true;
			config.ImportCharacters = false;
			config.ImportConstraints = false;
			config.ImportGobos = false;
			config.ImportShapes = false;
			config.ImportLinks = false;
			config.ImportMaterials = true;
			config.ImportTextures = false;
			config.ImportModels = true;
			config.ImportAnimations = false;
			config.ExtractEmbeddedData = false;

			return config;
		}

		public static ImportConfiguration ImportMaterialsOnly()
		{
			var config = new ImportConfiguration();

			config.ImportTemplates = false;
			config.ImportPivots = false;
			config.ImportGlobalSettings = true;
			config.ImportCharacters = false;
			config.ImportConstraints = false;
			config.ImportGobos = false;
			config.ImportShapes = false;
			config.ImportLinks = false;
			config.ImportMaterials = true;
			config.ImportTextures = false;
			config.ImportModels = false;
			config.ImportAnimations = false;
			config.ExtractEmbeddedData = false;

			return config;
		}

		public static ImportConfiguration ImportAnimationsOnly()
		{
			var config = new ImportConfiguration();

			config.ImportTemplates = false;
			config.ImportPivots = false;
			config.ImportGlobalSettings = true;
			config.ImportCharacters = false;
			config.ImportConstraints = false;
			config.ImportGobos = false;
			config.ImportShapes = false;
			config.ImportLinks = false;
			config.ImportMaterials = false;
			config.ImportTextures = false;
			config.ImportModels = false;
			config.ImportAnimations = true;
			config.ExtractEmbeddedData = false;

			return config;
		}

		public static ImportConfiguration ImportSkeletonOnly()
		{
			var config = new ImportConfiguration();

			config.ImportTemplates = false;
			config.ImportPivots = false;
			config.ImportGlobalSettings = true;
			config.ImportCharacters = false;
			config.ImportConstraints = false;
			config.ImportGobos = false;
			config.ImportShapes = false;
			config.ImportLinks = false;
			config.ImportMaterials = false;
			config.ImportTextures = false;
			config.ImportModels = false;
			config.ImportAnimations = false;
			config.ExtractEmbeddedData = false;

			return config;
		}

		public static ImportConfiguration ImportTexturesOnly()
		{
            var config = new ImportConfiguration();

			config.ImportTemplates = false;
			config.ImportPivots = false;
			config.ImportGlobalSettings = false;
			config.ImportCharacters = false;
			config.ImportConstraints = false;
			config.ImportGobos = false;
			config.ImportShapes = false;
			config.ImportLinks = false;
			config.ImportMaterials = false;
			config.ImportTextures = true;
			config.ImportModels = false;
			config.ImportAnimations = false;
			config.ExtractEmbeddedData = true;

			return config;
		}

		public static ImportConfiguration ImportEntityConfig()
		{
			var config = new ImportConfiguration();

			config.ImportTemplates = false;
			config.ImportPivots = false;
			config.ImportGlobalSettings = true;
			config.ImportCharacters = false;
			config.ImportConstraints = false;
			config.ImportGobos = false;
			config.ImportShapes = false;
			config.ImportLinks = false;
			config.ImportMaterials = true;
			config.ImportTextures = true;
			config.ImportModels = true;
			config.ImportAnimations = true;
			config.ExtractEmbeddedData = true;

			return config;
		}

		public static ImportConfiguration ImportGlobalSettingsOnly()
		{
			var config = new ImportConfiguration();

			config.ImportGlobalSettings = true;

			return config;
		}
	};

    private	static System.Object globalLock = new System.Object();

	public void Initialize(String inputFilename, String vfsOutputFilename, ImportConfiguration importConfig)
	{
		// -----------------------------------------------------
		// TODO: Workaround with FBX SDK not being multithreaded. 
		// We protect the whole usage of this class with a monitor
		//
		// Lock the whole class between Initialize/Destroy
		// -----------------------------------------------------
		System.Threading.Monitor.Enter(globalLock);
		// -----------------------------------------------------

		this.inputFilename = inputFilename;
		this.vfsOutputFilename = vfsOutputFilename;
		this.inputPath = Path.GetDirectoryName(inputFilename);

		// Initialize the sdk manager. This object handles all our memory management.
		lSdkManager = FbxManagerCreate();

		// Create the io settings object.
		IntPtr ios = FbxIOSettingsCreate(lSdkManager, IOSROOT);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_TEMPLATE, importConfig.ImportTemplates);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_PIVOT, importConfig.ImportPivots);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_GLOBAL_SETTINGS, importConfig.ImportGlobalSettings);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_CHARACTER, importConfig.ImportCharacters);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_CONSTRAINT, importConfig.ImportConstraints);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_GOBO, importConfig.ImportGobos);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_SHAPE, importConfig.ImportShapes);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_LINK, importConfig.ImportLinks);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_MATERIAL, importConfig.ImportMaterials);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_TEXTURE, importConfig.ImportTextures);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_MODEL, importConfig.ImportModels);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_ANIMATION, importConfig.ImportAnimations);
		FbxIOSettingsSetBoolProp(ios, IMP_FBX_EXTRACT_EMBEDDED_DATA, importConfig.ExtractEmbeddedData);
		FbxManagerSetIOSettings(lSdkManager, ios);

		// Create an importer using our sdk manager.
		lImporter = FbxImporterCreate(lSdkManager,new StringBuilder (""));
    
// 		var inputFilenameUtf8 = System.Text.Encoding.UTF8.GetBytes(inputFilename);
// 		pin_ptr<Byte> inputFilenameUtf8Ptr = &inputFilenameUtf8[0];

		if(!FbxImporterInitialize(lImporter, new StringBuilder (inputFilename), -1, FbxManagerGetIOSettings(lSdkManager)))
		{
			throw new InvalidOperationException(String.Format("Call to FbxImporter.Initialize() failed.\nError returned: {0}\n\n", FbxStatusGetErrorString (FbxImporterGetStatus(lImporter)).ToString ()));
		}

		// Create a new scene so it can be populated by the imported file.
		scene = FbxSceneCreate(lSdkManager, new StringBuilder ("myScene"));

		// Import the contents of the file into the scene.
		FbxImporterImport(lImporter, scene);
        
		float framerate = (float)(FbxTimeGetFrameRate(FbxGlobalSettingsGetTimeMode (FbxSceneGetGlobalSettings (scene))));
		FbxNodeResetPivotSetAndConvertAnimation(FbxSceneGetRootNode (scene), framerate, false, false);

        Console.WriteLine ("Framerate: " + framerate);
		// Initialize the node mapping
 		sceneMapping = new SceneMapping(scene);
	}
// 	
// 	bool HasAnimationData(String^ inputFile)
// 	{
// 		try
// 		{
// 			Initialize(inputFile, nullptr, ImportConfiguration.ImportAnimationsOnly());
// 			var animConverter = new AnimationConverter(logger, sceneMapping);
// 			return animConverter.HasAnimationData();
// 		}
// 		finally
// 		{
// 			Destroy();
// 		}
// 	}
	
	void GenerateMaterialNames(Dictionary<IntPtr, String> materialNames)
	{
		var materials = new List</*MaterialAsset*/IntPtr>();
		Dictionary<String, int> materialNameTotalCount = new Dictionary<String, int> ();
		Dictionary<String, int> materialNameCurrentCount = new Dictionary<String, int> ();
		Dictionary</*FbxSurfaceMaterial*/IntPtr, String> tempNames = new Dictionary<IntPtr, String> ();
		var materialCount = FbxSceneGetMaterialCount(scene);
		
		for (int i = 0;  i < materialCount; i++)
		{
			var lMaterial = FbxSceneGetMaterial(scene, i);
			var materialName = FbxNodeGetName(lMaterial).ToString ();
			var materialPart = "";

			int materialNameSplitPosition = materialName.IndexOf('#');
			if (materialNameSplitPosition != -1)
			{
				materialPart = materialName.Substring(materialNameSplitPosition + 1);
				materialName = materialName.Substring(0, materialNameSplitPosition);
			}

			materialNameSplitPosition = materialName.IndexOf("__");
			if (materialNameSplitPosition != -1)
			{
				materialPart = materialName.Substring(materialNameSplitPosition + 2);
				materialName = materialName.Substring(0, materialNameSplitPosition);
			}

			// remove all bad characters
			materialName.Replace (':', '_');
			materialName.Replace ('/', '_');
			materialName.Replace (" ", string.Empty);
			tempNames[lMaterial] = materialName;
			
			if (!materialNameTotalCount.ContainsKey (materialName))
				materialNameTotalCount[materialName] = 1;
			else
				materialNameTotalCount[materialName] = materialNameTotalCount[materialName] + 1;
		}

		for (int i = 0;  i < materialCount; i++)
		{
			var lMaterial = FbxSceneGetMaterial(scene, i);
			var materialName = tempNames[lMaterial];
			int currentCount = 0;

			if (!materialNameCurrentCount.ContainsKey (materialName))
				materialNameCurrentCount[materialName] = 1;
			else
				materialNameCurrentCount[materialName] = materialNameCurrentCount[materialName] + 1;

			if(materialNameTotalCount[materialName] > 1)
				materialName = materialName + "_" + materialNameCurrentCount[materialName];

            Console.WriteLine ("Material: " + materialName);
			materialNames[lMaterial] = materialName;
		}
	}

	void GetMeshes(IntPtr pNode, List<IntPtr> meshes)
	{
		// Process the node's attributes.
		for(int i = 0; i < FbxNodeGetNodeAttributeCount(pNode); i++)
		{
			var pAttribute = FbxNodeGetNodeAttributeByIndex(pNode, i);

			if(pAttribute == IntPtr.Zero) return;
		
			if ((EType) FbxNodeAttributeGetAttributeType(pAttribute) == EType.eMesh)
			{
				var pMesh = /*(FbxMesh*)*/pAttribute;
				meshes.Add(pMesh);
			}
		}

		// Recursively process the children nodes.
		for(int j = 0; j < FbxNodeGetChildCount(pNode); j++)
		{
			GetMeshes(FbxNodeGetChild(pNode, j), meshes);
		}
	}
	
	void GenerateMeshesName(Dictionary<IntPtr, String> meshNames)
	{
		List<IntPtr> meshes = new List<IntPtr> ();
		GetMeshes(FbxSceneGetRootNode(scene), meshes);

		Dictionary<String, int> meshNameTotalCount = new Dictionary<String, int> ();
		Dictionary<String, int> meshNameCurrentCount = new Dictionary<String, int> ();
		Dictionary<IntPtr, String> tempNames = new Dictionary<IntPtr, String> ();

		foreach (var pMesh in meshes)
		{
			var meshName = FbxNodeGetName(FbxMeshGetNode (pMesh)).ToString ();
            Console.WriteLine ("Mesh name: " + meshName);

			// remove all bad characters
            meshName.Replace (" ", string.Empty);
			tempNames[pMesh] = meshName;

			if (!meshNameTotalCount.ContainsKey (meshName))
				meshNameTotalCount[meshName] = 1;
			else
				meshNameTotalCount[meshName] = meshNameTotalCount[meshName] + 1;
		}

		foreach (var pMesh in meshes)
		{
			var meshName = tempNames[pMesh];
			int currentCount = 0;

			if (!meshNameCurrentCount.ContainsKey (meshName))
				meshNameCurrentCount[meshName] = 1;
			else
				meshNameCurrentCount[meshName] = meshNameCurrentCount[meshName] + 1;

			if(meshNameTotalCount[meshName] > 1)
				meshName = meshName + "_" + meshNameCurrentCount[meshName];

			meshNames[pMesh] = meshName;
		}
	}

// 	MaterialInstantiation^ GetOrCreateMaterial(FbxSurfaceMaterial* lMaterial, List<String^>^ uvNames, List<MaterialInstantiation^>^ instances, Dictionary<String, size_t>& uvElements, Dictionary<FbxSurfaceMaterial*, String>& materialNames)
// 	{
// 		for (int i = 0; i < instances.Count; ++i)
// 		{
// 			if (lMaterial == instances[i].SourceMaterial)
// 				return instances[i];
// 		}
// 
// 		var newMaterialInstantiation = new MaterialInstantiation();
// 		newMaterialInstantiation.SourceMaterial = lMaterial;
// 		newMaterialInstantiation.MaterialName = new String(materialNames[lMaterial].c_str());
// 
// 		// TODO: We currently use UV mapping of first requesting mesh.
// 		//       However, we probably need to reverse everything: mesh describes what they have, materials what they need, and an appropriate input layout is created at runtime?
// 		//       Such a mechanism would also be able to handle missing streams gracefully.
// 		newMaterialInstantiation.Material = ProcessMeshMaterialAsset(lMaterial, uvElements);
// 		instances.Add(newMaterialInstantiation);
// 		return newMaterialInstantiation;
// 	}
// 
// 	void SearchMeshInAttribute(FbxNode* pNode, FbxNodeAttribute* pAttribute, Dictionary<FbxSurfaceMaterial*, String> materialNames, Dictionary<FbxMesh*, String> meshNames, List<MeshParameters^>^ models, List<MaterialInstantiation^>^ materialInstantiations)
// 	{
// 		if(!pAttribute) return;
//  
// 		if (pAttribute.GetAttributeType() == FbxNodeAttribute.eMesh)
// 		{
// 			var pMesh = (FbxMesh*)pAttribute;
// 			int polygonCount = pMesh.GetPolygonCount();
// 			FbxGeometryElement.EMappingMode materialMappingMode = FbxGeometryElement.eNone;
// 			FbxLayerElementArrayTemplate<int>* materialIndices = NULL;
// 			
// 			if (pMesh.GetElementMaterial())
// 			{
// 				materialMappingMode = pMesh.GetElementMaterial().GetMappingMode();
// 				materialIndices = &pMesh.GetElementMaterial().GetIndexArray();
// 			}
// 
// 			var buildMeshes = new List<BuildMesh^>();
// 
// 			// Count polygon per materials
// 			for (int i = 0; i < polygonCount; i++)
// 			{
// 				int materialIndex = 0;
// 				if (materialMappingMode == FbxGeometryElement.eByPolygon)
// 				{
// 					materialIndex = materialIndices.GetAt(i);
// 				}
// 				else if (materialMappingMode == FbxGeometryElement.eAllSame)
// 				{
// 					materialIndex = materialIndices.GetAt(0);
// 				}
// 
// 				// Equivalent to std.vector.resize()
// 				while (materialIndex >= buildMeshes.Count)
// 				{
// 					buildMeshes.Add(nullptr);
// 				}
// 
// 				if (buildMeshes[materialIndex] == nullptr)
// 					buildMeshes[materialIndex] = new BuildMesh();
// 
// 				int polygonSize = pMesh.GetPolygonSize(i) - 2;
// 				if (polygonSize > 0)
// 					buildMeshes[materialIndex].polygonCount += polygonSize;
// 			}
// 
// 			for (int i = 0; i < buildMeshes.Count; ++i)
// 			{
// 				var meshParams = new MeshParameters();
// 				var meshName = meshNames[pMesh];
// 				if (buildMeshes.Count > 1)
// 					meshName = meshName + "_" + std.to_string(i + 1);
// 				meshParams.MeshName = new String(meshName.c_str());
// 				meshParams.NodeName = sceneMapping.FindNode(pNode).Name;
// 
// 				// Collect bones
// 				int skinDeformerCount = pMesh.GetDeformerCount(FbxDeformer.eSkin);
// 				if (skinDeformerCount > 0)
// 				{
// 					meshParams.BoneNodes = new HashSet<String^>();
// 					for (int deformerIndex = 0; deformerIndex < skinDeformerCount; deformerIndex++)
// 					{
// 						FbxSkin* skin = FbxCast<FbxSkin>(pMesh.GetDeformer(deformerIndex, FbxDeformer.eSkin));
// 
// 						var totalClusterCount = skin.GetClusterCount();
// 						for (int clusterIndex = 0; clusterIndex < totalClusterCount; ++clusterIndex)
// 						{
// 							FbxCluster* cluster = skin.GetCluster(clusterIndex);
// 							int indexCount = cluster.GetControlPointIndicesCount();
// 							if (indexCount == 0)
// 							{
// 								continue;
// 							}
// 
// 							FbxNode* link = cluster.GetLink();
// 
// 							MeshBoneDefinition bone;
// 							meshParams.BoneNodes.Add(sceneMapping.FindNode(link).Name);
// 						}
// 					}
// 				}
// 
// 				FbxGeometryElementMaterial* lMaterialElement = pMesh.GetElementMaterial();
// 				FbxSurfaceMaterial* lMaterial = pNode.GetMaterial(i);
// 				if ((materialMappingMode == FbxGeometryElement.eByPolygon || materialMappingMode == FbxGeometryElement.eAllSame)
// 					&& lMaterialElement != NULL && lMaterial != NULL)
// 				{
// 					Dictionary<String, size_t> uvElements;
// 					var uvNames = new List<String^>();
// 					for (int j = 0; j < pMesh.GetElementUVCount(); ++j)
// 					{
// 						uvElements[pMesh.GetElementUV(j).GetName()] = j;
// 						uvNames.Add(new String(pMesh.GetElementUV(j).GetName()));
// 					}
// 
// 					var material = GetOrCreateMaterial(lMaterial, uvNames, materialInstantiations, uvElements, materialNames);
// 					meshParams.MaterialName = material.MaterialName;
// 				}
// 				else
// 				{
// 					logger.Warning(String.Format("Mesh {0} does not have a material. It might not be displayed.", meshParams.MeshName), (CallerInfo^)nullptr);
// 				}
// 
// 				models.Add(meshParams);
// 			}
// 		}
// 	}
// 
// 	void SearchMesh(FbxNode* pNode, Dictionary<FbxSurfaceMaterial*, String> materialNames, Dictionary<FbxMesh*, String> meshNames, List<MeshParameters^>^ models, List<MaterialInstantiation^>^ materialInstantiations)
// 	{
// 		// Process the node's attributes.
// 		for(int i = 0; i < pNode.GetNodeAttributeCount(); i++)
// 			SearchMeshInAttribute(pNode, pNode.GetNodeAttributeByIndex(i), materialNames, meshNames, models, materialInstantiations);
// 
// 		// Recursively process the children nodes.
// 		for(int j = 0; j < pNode.GetChildCount(); j++)
// 		{
// 			SearchMesh(pNode.GetChild(j), materialNames, meshNames, models, materialInstantiations);
// 		}
// 	}
// 
// 	Dictionary<String^, MaterialAsset^>^ ExtractMaterialsNoInit()
// 	{
// 		Dictionary<FbxSurfaceMaterial*, String> materialNames;
// 		GenerateMaterialNames(materialNames);
// 
// 		var materials = new Dictionary<String^, MaterialAsset^>();
// 		for (int i = 0;  i < scene.GetMaterialCount(); i++)
// 		{
// 			Dictionary<String, size_t> dict;
// 			var lMaterial = scene.GetMaterial(i);
// 			var materialName = materialNames[lMaterial];
// 			materials.Add(new String(materialName.c_str()), ProcessMeshMaterialAsset(lMaterial, dict));
// 		}
// 		return materials;
// 	}
// 
// 	MeshMaterials^ ExtractModelNoInit()
// 	{
// 		Dictionary<FbxSurfaceMaterial*, String> materialNames;
// 		GenerateMaterialNames(materialNames);
// 
// 		Dictionary<FbxMesh*, String> meshNames;
// 		GenerateMeshesName(meshNames);
// 			
// 		Dictionary<String, FbxSurfaceMaterial*> materialPerMesh;
// 		var models = new List<MeshParameters^>();
// 		var materialInstantiations = new List<MaterialInstantiation^>();
// 		SearchMesh(scene.GetRootNode(), materialNames, meshNames, models, materialInstantiations);
// 
// 		var ret = new MeshMaterials();
// 		ret.Models = models;
// 		ret.Materials = new Dictionary<String^, MaterialAsset^>();
// 		for (int i = 0; i < materialInstantiations.Count; ++i)
// 		{
// 			if (!ret.Materials.ContainsKey(materialInstantiations[i].MaterialName))
// 			{
// 				ret.Materials.Add(materialInstantiations[i].MaterialName, materialInstantiations[i].Material);
// 			}
// 		}
//         
// 		return ret;
// 	}
// 
// 	List<String^>^ ExtractTextureDependenciesNoInit()
// 	{
// 		var textureNames = new List<String^>();
// 			
// 		var textureCount = scene.GetTextureCount();
// 		for(int i=0; i<textureCount; ++i)
// 		{
// 			var texture  = FbxCast<FbxFileTexture>(scene.GetTexture(i));
// 
// 			if(texture == nullptr)
// 				continue;
// 			
// 			var texturePath = FindFilePath(texture);
// 			if (!String.IsNullOrEmpty(texturePath))
// 			{
// 				if (texturePath.Contains(".fbm\\"))
// 					logger.Info(String.Format("Importer detected an embedded texture. It has been extracted at address '{0}'.", texturePath), (CallerInfo^)nullptr);
// 				if (!File.Exists(texturePath))
// 					logger.Warning(String.Format("Importer detected a texture not available on disk at address '{0}'", texturePath), (CallerInfo^)nullptr);
// 
// 				textureNames.Add(texturePath);
// 			}
// 		}
// 
// 		return textureNames;
// 	}
// 
// 	List<String^>^ ExtractTextureDependencies(String^ inputFile)
// 	{
// 		try
// 		{
// 			Initialize(inputFile, nullptr, ImportConfiguration.ImportTexturesOnly());
// 			return ExtractTextureDependenciesNoInit();
// 		}
// 		finally
// 		{
// 			Destroy();
// 		}
// 		return nullptr;
// 	}
// 
// 	Dictionary<String^, MaterialAsset^>^ ExtractMaterials(String^ inputFilename)
// 	{
// 		try
// 		{
// 			Initialize(inputFilename, nullptr, ImportConfiguration.ImportMaterialsOnly());
// 			return ExtractMaterialsNoInit();
// 		}
// 		finally
// 		{
// 			Destroy();
// 		}
// 		return nullptr;
// 	}
// 
// 	void GetNodes(FbxNode* node, int depth, List<NodeInfo^>^ allNodes)
// 	{
// 		var newNodeInfo = new NodeInfo();
// 		newNodeInfo.Name = sceneMapping.FindNode(node).Name;
// 		newNodeInfo.Depth = depth;
// 		newNodeInfo.Preserve = true;
// 		
// 		allNodes.Add(newNodeInfo);
// 		for (int i = 0; i < node.GetChildCount(); ++i)
// 			GetNodes(node.GetChild(i), depth + 1, allNodes);
// 	}
// 
// 	List<NodeInfo^>^ ExtractNodeHierarchy()
// 	{
// 		var allNodes = new List<NodeInfo^>();
// 		GetNodes(scene.GetRootNode(), 0, allNodes);
// 		return allNodes;
// 	}
// 
// public:
// 	EntityInfo^ ExtractEntity(String^ inputFileName, bool extractTextureDependencies)
// 	{
// 		try
// 		{
// 			Initialize(inputFileName, nullptr, ImportConfiguration.ImportEntityConfig());
// 			
// 			var animationConverter = new AnimationConverter(logger, sceneMapping);
// 			
// 			var entityInfo = new EntityInfo();
// 			if (extractTextureDependencies)
// 				entityInfo.TextureDependencies = ExtractTextureDependenciesNoInit();
// 			entityInfo.AnimationNodes = animationConverter.ExtractAnimationNodesNoInit();
// 			var models = ExtractModelNoInit();
// 			entityInfo.Models = models.Models;
// 			entityInfo.Materials = models.Materials;
// 			entityInfo.Nodes = ExtractNodeHierarchy();
// 
// 			return entityInfo;
// 		}
// 		finally
// 		{
// 			Destroy();
// 		}
// 		return nullptr;
// 	}
// 
// 	double GetAnimationDuration(String^ inputFileName, int animationStack)
// 	{
// 		try
// 		{
// 			Initialize(inputFileName, nullptr, ImportConfiguration.ImportEntityConfig());
// 
// 			var animationConverter = new AnimationConverter(logger, sceneMapping);
// 			var animationData = animationConverter.ProcessAnimation(inputFilename, "", true, animationStack);
// 
// 			return animationData.Duration.TotalSeconds;
// 		}
// 		finally
// 		{
// 			Destroy();
// 		}
// 
// 		return 0;
// 	}
// 
	public Model Convert(String inputFilename, String vfsOutputFilename, Dictionary<String, int> materialIndices)
	{
		try
		{
			Initialize(inputFilename, vfsOutputFilename, ImportConfiguration.ImportAll());

			// Create default ModelViewData
			modelData = new Model();

			//var sceneName = scene.GetName();
			//if (sceneName != NULL && strlen(sceneName) > 0)
			//{
			//	entity.Name = new String(sceneName);
			//}
			//else
			//{
			//	// Build scene name from file name
			//	entity.Name = Path.GetFileName(this.inputFilename);
			//}

			Dictionary<IntPtr, String> meshNames = new Dictionary<IntPtr, String> ();
			GenerateMeshesName(meshNames);

			Dictionary<IntPtr, String> materialNames = new Dictionary<IntPtr, String> ();
			GenerateMaterialNames(materialNames);

			Dictionary</*FbxSurfaceMaterial*/IntPtr, int> materials = new Dictionary<IntPtr, int> ();
			foreach (KeyValuePair<IntPtr, String> entry in materialNames)
			{
				var materialName = entry.Value;
				int materialIndex;
				if (materialIndices.TryGetValue(materialName, out materialIndex))
				{
					materials[entry.Key] = materialIndex;
				}
				else
				{
					logger.Warning(String.Format("Model references material '{0}', but it was not defined in the ModelAsset.", materialName));
				}
			}

			// Process and add root entity
			ProcessNodeTransformation(FbxSceneGetRootNode (scene));
			ProcessNodeAttributes(FbxSceneGetRootNode (scene), meshNames, materials);

			return modelData;
		}
		finally
		{
			Destroy();
		}

		return null;
	}

	public AnimationInfo ConvertAnimation(String inputFilename, String vfsOutputFilename, bool importCustomAttributeAnimations, int animationStack)
	{
		try
		{
// 			Initialize(inputFilename, vfsOutputFilename, ImportConfiguration.ImportAnimationsOnly());

// 			var animationConverter = new AnimationConverter(logger, sceneMapping);
// 			return animationConverter.ProcessAnimation(inputFilename, vfsOutputFilename, importCustomAttributeAnimations, animationStack);
		}
		finally
		{
			Destroy();
		}

		return null;
	}

	public Skeleton ConvertSkeleton(String inputFilename, String vfsOutputFilename)
	{
		try
		{
// 			Initialize(inputFilename, vfsOutputFilename, ImportConfiguration.ImportSkeletonOnly());
// 			ProcessNodeTransformation(scene.GetRootNode());
// 
// 			var skeleton = new Skeleton();
// 			skeleton.Nodes = sceneMapping.Nodes;
// 			return skeleton;
		}
		finally
		{
			Destroy();
		}

		return null;
	}
	
    }

}
