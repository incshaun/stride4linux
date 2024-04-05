// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Stride.Core.Mathematics;

namespace Stride.Importer.FBX
{
    public class UtilityFunctions
    {

    // Conversion functions
    public static Quaternion AxisRotationToQuaternion(Vector3 axisRotation)
    {
        return Quaternion.RotationX(axisRotation.X) * Quaternion.RotationY(axisRotation.Y) * Quaternion.RotationZ(axisRotation.Z);
    }
//     public static Quaternion AxisRotationToQuaternion(FbxDouble3 axisRotation)
//     {
//         return AxisRotationToQuaternion(FbxDouble3ToVector3(axisRotation));
//     }

//     public static Vector2 FbxDouble2ToVector2(FbxDouble2 vector)
//     {
//         return Vector2((float)vector[0], (float)vector[1]);
//     }
// 
//     public static Color4 FbxDouble3ToColor4(FbxDouble3 vector, float alphaValue)
//     {
//         return Color4((float)vector[0], (float)vector[1], (float)vector[2], alphaValue);
//     }
// 
//     public static Vector3 FbxDouble3ToVector3(FbxDouble3 vector)
//     {
//         return Vector3((float)vector[0], (float)vector[1], (float)vector[2]);
//     }
// 
//     public static Vector4 FbxDouble3ToVector4(FbxDouble3 vector, float wValue)
//     {
//         return Vector4((float)vector[0], (float)vector[1], (float)vector[2], wValue);
//     }
// 
//     public static Vector4 FbxDouble4ToVector4(FbxDouble4 vector)
//     {
//         return Vector4((float)vector[0], (float)vector[1], (float)vector[2], (float)vector[3]);
//     }
// 
//     public static CompressedTimeSpan FBXTimeToTimeSpan(/*const FbxTime&*/long time)
//     {
//         double resultTime = (double)time.Get();
//         resultTime *= (double)CompressedTimeSpan.TicksPerSecond / (double)FBXSDK_TIME_ONE_SECOND.Get();
//         return CompressedTimeSpan((int)resultTime);
//     }
// 
    public static Matrix FBXMatrixToMatrix(double[] matrix)
    {
        Matrix result = new Matrix ();

        for (int i = 0; i < 4; ++i)
            for (int j = 0; j < 4; ++j)
                result[i * 4 + j] = (float) matrix[j * 4 + i];

        return result;
    }

//     public static FbxAMatrix MatrixToFBXMatrix(Matrix matrix)
//     {
//         FbxAMatrix result;
// 
//         for (int i = 0; i < 4; ++i)
//             for (int j = 0; j < 4; ++j)
//                 ((double*)&result)[i * 4 + j] = (double)((float*)&matrix)[j * 4 + i];
// 
//         return result;
//     }

    public static double FocalLengthToVerticalFov(double filmHeight, double focalLength)
    {
        return 2.0 * Math.Atan(filmHeight * 0.5 * 10.0 * 2.54 / focalLength);
    }

    // Operators
    /*public static FbxDouble3 operator*(double factor, FbxDouble3 vector)
    {
        return FbxDouble3(factor * vector[0], factor * vector[1], factor * vector[2]);
    }*/

    // string manipulation
    /*public static String ConvertToUTF8(String str)
    {
        auto byteCount = str.length();
        // Check `str' cannot be more than the size of a int.
        assert(byteCount <= INT32_MAX);
        if (byteCount <= 0)
        {
            return "";
        }
        array<Byte>^ bytes = gcnew array<Byte>((int) byteCount);
        pin_ptr<Byte> p = &bytes[0];
        memcpy(p, str.c_str(), byteCount);
        return System.Text.Encoding.UTF8->GetString(bytes);
    }*/
    }
}
