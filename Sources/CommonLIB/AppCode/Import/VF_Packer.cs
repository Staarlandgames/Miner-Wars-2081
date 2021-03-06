﻿using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWarsMath.Graphics.PackedVector;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

namespace MinerWars.CommonLIB.AppCode.Import
{
    // Vertex Format Packer
    public class VF_Packer
    {
        public static short PackAmbientAndAlpha(float ambient, byte alpha)
        {
            Debug.Assert(alpha <= 2, "Alpha can be 0, 1 or 2");
            Debug.Assert(ambient >= -1 && ambient <= 1);
            short packed = (short)(ambient * 8191); // ambient in <-8191,8191>
            int sign = packed < 0 ? -1 : 1;
            packed += (short)(sign * (alpha * 8192));
            return packed;
        }

        public static float UnpackAmbient(float packed)
        {
            // HLSL
            //frac(PositionAndAmbient.w / 8192.0f);

            return (packed % 8192.0f) / 8191.0f;
        }

        public static float UnpackAmbient(short packed)
        {
            // HLSL
            //frac(PositionAndAmbient.w / 8192.0f);

            return (packed % 8192.0f) / 8191.0f;
        }

        public static byte UnpackAlpha(short packed)
        {
            // HLSL
            //int index = (int)abs(PositionAndAmbient.w / 8192);
            //return float3(step(0, -index), step(abs(index - 1), 0), step(2, index));

            return (byte)Math.Abs(packed / 8192);
        }

        public static byte UnpackAlpha(float packed)
        {
            // HLSL
            //int index = (int)abs(PositionAndAmbient.w / 8192);
            //return float3(step(0, -index), step(abs(index - 1), 0), step(2, index));

            return (byte)Math.Abs(packed / 8192);
        }

        static public uint PackNormal(ref Vector3 normal)
        {
            Vector3 new_normal = normal;

            // normal must be normalized!
            //System.Diagnostics.Debug.Assert(System.Math.Abs(normal.LengthSquared() - 1.0f) < 0.005f);

            // scale to 0.0 - 1.0 format
            new_normal.X = 0.5f * (new_normal.X + 1.0f);
            new_normal.Y = 0.5f * (new_normal.Y + 1.0f);

            // scale to 0 - 32767
            uint scaled_x = (ushort)((ushort)(new_normal.X * 32767));
            uint scaled_y = (ushort)(new_normal.Y * 32767);

            // set last bit of scaled_x to sign of normal.Z so we can recompute Z in HLSL
            ushort z_sign = (ushort)(new_normal.Z > 0 ? 1 : 0);
            scaled_x |= (ushort)(z_sign << 15);

            return scaled_x | scaled_y << 16;
        }

        static public Byte4 PackNormalB4(ref Vector3 normal)
        {
            uint packedValue = PackNormal(ref normal);
            Byte4 b4 = new Byte4();
            b4.PackedValue = packedValue;
            return b4;
        }

        static public Vector3 UnpackNormal(ref uint packedNormal)
        {
            Byte4 pn = new Byte4();
            pn.PackedValue = packedNormal;
            return UnpackNormal(ref pn);
        }

        static public Vector3 UnpackNormal(ref Byte4 packedNormal)
        {
            Vector4 unpacked = packedNormal.ToVector4();

            // get sign of Z from last bit of Y
            float z_sign = unpacked.Y > 127.5f ? 1.0f : -1.0f;

            // clear last bit of Y
            if (z_sign > 0)
                unpacked.Y -= 128.0f;

            // construct X and Y into format <0, 32767>
            float x = unpacked.X + 256.0f * unpacked.Y;
            float y = unpacked.Z + 256.0f * unpacked.W;

            // normalize X and Y to <0,1>
            x /= 32767.0f;
            y /= 32767.0f;

            // transform X and Y to <-1, 1>
            float nx = (2 * x) - 1.0f;
            float ny = (2 * y) - 1.0f;

            // compute Z
            float squaredZ = System.Math.Max(0, 1 - nx * nx - ny * ny);
            float nz = z_sign * (float)System.Math.Sqrt(squaredZ);

            return new Vector3(nx, ny, nz);
        }

        static public HalfVector4 PackPosition(ref Vector3 position)
        {
            return PositionPacker.PackPosition(ref position);
        }

        static public Vector3 UnpackPosition(ref HalfVector4 position)
        {
            return PositionPacker.UnpackPosition(ref position);
        }

        // This will pack and unpack position for model, so repacked position will be the same as in shader.
        static public Vector3 RepackModelPosition(ref Vector3 position)
        {
            HalfVector4 packed = PackPosition(ref position);
            return UnpackPosition(ref packed);
        }
    }
}
