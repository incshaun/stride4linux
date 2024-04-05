// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Stride.Core.Diagnostics;
using Stride.Animations;
using Stride.Rendering;
using Stride.Engine;
using Stride.Core.Mathematics;

namespace Stride.Importer.FBX
{
    /// <summary>
    /// Contains mapping between FBX nodes and Stride ModelNodeDefinition
    /// </summary>
    public class SceneMapping
    {
        private IntPtr scene;
        private Dictionary<IntPtr, int> nodeMapping;
        private ModelNodeDefinition [] nodes;

        private Matrix convertMatrix;
        private Matrix inverseConvertMatrix;
        private Matrix normalConvertMatrix;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMapping"/> class.
        /// </summary>
        /// <param name="sceneArg">The scene argument.</param>
        public SceneMapping(IntPtr scene)
        {
            if (scene == IntPtr.Zero)
            {
                throw new ArgumentNullException("scene");
            }
            this.scene = scene;
            nodeMapping = new Dictionary<IntPtr, int>();

            // Generate names for all nodes
            Dictionary</*FbxNode*/IntPtr, String> nodeNames = new Dictionary<IntPtr, String> ();
            GenerateNodesName(scene, nodeNames);

            // Generate all ModelNodeDefinition
            var nodeList = new List<ModelNodeDefinition>();
            RegisterNode(MeshConverter.FbxSceneGetRootNode (scene), -1, nodeNames, nodeMapping, nodeList);
            nodes = nodeList.ToArray ();

            // Setup the conversion
            IntPtr settings = MeshConverter.FbxSceneGetGlobalSettings (scene);
            InitializeMatrix(MeshConverter.FbxGlobalSettingsGetAxisSystem (settings), MeshConverter.FbxGlobalSettingsGetSystemUnit (settings));
        }

        /// <summary>
        /// Gets all the nodes.
        /// </summary>
        public ModelNodeDefinition [] Nodes
        {
            get
            {
                return nodes;
            }
        }

        /// <summary>
        /// Gets the associated FbxScene.
        /// </summary>
        public IntPtr Scene
        {
            get
            {
                return scene;
            }
        }

        public Matrix MatrixModifier
        {
            get
            {
                return convertMatrix;
            }
        }

        public float ScaleToMeters;

        public Matrix AxisSystemRotationMatrix;

        /// <summary>
        /// Finds the index of the FBX node in the <see cref="ModelNodeDefinition"/> from a FBX node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Stride.Rendering.ModelNodeDefinition.</returns>
        public int FindNodeIndex(/*FbxNode*/IntPtr node)
        {
            int nodeIndex;
            if (!nodeMapping.TryGetValue(node, out nodeIndex))
            {
                throw new ArgumentException("Invalid node not found", "node");
            }

            return nodeIndex;
        }


        /// <summary>
        /// Finds a <see cref="ModelNodeDefinition"/> from a FBX node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Stride.Rendering.ModelNodeDefinition.</returns>
//         public ModelNodeDefinition FindNode(FbxNode* node)
//         {
//             int nodeIndex;
//             if (!nodeMapping.TryGetValue((IntPtr)node, nodeIndex))
//             {
//                 throw new ArgumentException("Invalid node not found", "node");
//             }
// 
//             return nodes[nodeIndex];
//         }

        public Matrix ConvertMatrixFromFbx(/*FbxAMatrix&*/double [] _m) 
        {
            var result = UtilityFunctions.FBXMatrixToMatrix(_m);
            // Adjust translation
            result.M41 *= ScaleToMeters;
            result.M42 *= ScaleToMeters;
            result.M43 *= ScaleToMeters;
            return result;
        }

//         public Vector3 ConvertPointFromFbx(const FbxVector4& _p)
//         {
//             return (Vector3)FbxDouble4ToVector4(_p) * ScaleToMeters;
//         }
// 
//         public Vector3 ConvertNormalFromFbx(const FbxVector4& _p)
//         {
//             return (Vector3)FbxDouble4ToVector4(_p);
//         }
//     
        private static void GetNodes(IntPtr pNode, List<IntPtr> nodes)
        {
            nodes.Add(pNode);

            // Recursively process the children nodes.
            for (int j = 0; j < MeshConverter.FbxNodeGetChildCount (pNode); j++)
                GetNodes(MeshConverter.FbxNodeGetChild (pNode, j), nodes);
        }

        private static void GenerateNodesName(IntPtr scene, Dictionary<IntPtr, String> nodeNames)
        {
            List<IntPtr> nodes = new List<IntPtr> ();
            GetNodes(MeshConverter.FbxSceneGetRootNode (scene), nodes);

            Dictionary<String, int> nodeNameTotalCount = new Dictionary<String, int> ();
            Dictionary<String, int> nodeNameCurrentCount = new Dictionary<String, int> ();
            Dictionary<IntPtr, String> tempNames = new Dictionary<IntPtr, String> ();

            foreach (var pNode in nodes)
            {
                var nodeName = MeshConverter.FbxNodeGetName (pNode).ToString ();
                var subBegin = nodeName.LastIndexOf(':');
                if (subBegin != -1)
                    nodeName = nodeName.Substring(subBegin + 1);
                tempNames[pNode] = nodeName;

                if (!nodeNameTotalCount.ContainsKey (nodeName))
                    nodeNameTotalCount[nodeName] = 1;
                else
                    nodeNameTotalCount[nodeName] = nodeNameTotalCount[nodeName] + 1;
            }

            foreach (var pNode in nodes)
            {
                var nodeName = tempNames[pNode];
                int currentCount = 0;

                if (!nodeNameCurrentCount.ContainsKey (nodeName))
                    nodeNameCurrentCount[nodeName] = 1;
                else
                    nodeNameCurrentCount[nodeName] = nodeNameCurrentCount[nodeName] + 1;

                if (nodeNameTotalCount[nodeName] > 1)
                    nodeName = nodeName + "_" + nodeNameCurrentCount[nodeName];

                nodeNames[pNode] = nodeName;
            }
        }

        private static void RegisterNode(/*FbxNode*/IntPtr pNode, int parentIndex, Dictionary</*FbxNode*/IntPtr, String> nodeNames, Dictionary<IntPtr, int> nodeMapping, List<ModelNodeDefinition> nodes)
        {
            int currentIndex = nodes.Count;

            nodeMapping[(IntPtr)pNode] = currentIndex;

            // Create node
            ModelNodeDefinition modelNodeDefinition = new ModelNodeDefinition ();
            modelNodeDefinition.ParentIndex = parentIndex;
            modelNodeDefinition.Transform.Scale = Vector3.One;
//             modelNodeDefinition.Name = ConvertToUTF8(nodeNames[pNode]);
            modelNodeDefinition.Name = nodeNames[pNode];
            modelNodeDefinition.Flags = ModelNodeFlags.Default;
            nodes.Add(modelNodeDefinition);

            // Recursively process the children nodes.
            for (int j = 0; j < MeshConverter.FbxNodeGetChildCount (pNode); j++)
            {
                RegisterNode(MeshConverter.FbxNodeGetChild (pNode, j), currentIndex, nodeNames, nodeMapping, nodes);
            }
        }

        private void InitializeMatrix(/*FbxAxisSystem*/IntPtr axisSystem, /*FbxSystemUnit*/IntPtr unitSystem)
        {
            var fromMatrix = BuildAxisSystemMatrix(axisSystem);
            fromMatrix.Invert();
            //var fromMatrix = Matrix.Identity;

            // Finds unit conversion ratio to ScaleImport (usually 0.01 so 1 meter). GetScaleFactor() is in cm.
            ScaleToMeters = (float)MeshConverter.FbxSystemUnitGetScaleFactor (unitSystem) * 0.01f;

            // Builds conversion matrices.
            AxisSystemRotationMatrix = fromMatrix;
        }

        private static Matrix BuildAxisSystemMatrix(IntPtr axisSystem) {

            int signUp = 0;
            int signFront = 0;
            Vector3 up = Vector3.UnitY;
            Vector3 at = Vector3.UnitZ;

            var upAxis = MeshConverter.FbxAxisSystemGetUpVector(axisSystem, ref signUp);
            var frontAxisParityEven = MeshConverter.FbxAxisSystemGetFrontVector(axisSystem, ref signFront) == (int) MeshConverter.EFrontVector.eParityEven;
            switch (upAxis)
            {
            case (int) MeshConverter.EUpVector.eXAxis:
            {
                up = Vector3.UnitX;
                at = frontAxisParityEven ? Vector3.UnitY : Vector3.UnitZ;
                break;
            }

            case (int) MeshConverter.EUpVector.eYAxis:
            {
                up = Vector3.UnitY;
                at = frontAxisParityEven ? Vector3.UnitX : Vector3.UnitZ;
                break;
            }

            case (int) MeshConverter.EUpVector.eZAxis:
            {
                up = Vector3.UnitZ;
                at = frontAxisParityEven ? Vector3.UnitX : Vector3.UnitY;
                break;
            }
            }
            up *= (float)signUp;
            at *= (float)signFront;

            var right = MeshConverter.FbxAxisSystemGetCoordSystem (axisSystem) == (int) MeshConverter.ECoordSystem.eRightHanded ? Vector3.Cross(up, at) : Vector3.Cross(at, up);

            var matrix = Matrix.Identity;
            matrix.Right = right;
            matrix.Up = up;
            matrix.Backward = at;

            return matrix;
        }
    }
}
