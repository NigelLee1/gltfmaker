﻿using glTFLoader.Schema;
//using NetTopologySuite.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker.MakerGroup
{
    static partial class gltfCreator
    {
        /// <summary>
        /// 创建长方体
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public static void CreateCuboidGltf(Point3Df p0, Point3Df p1, Point3Df p2, Point3Df p3, Point3Df p4, Point3Df p5, Point3Df p6, Point3Df p7)
        {
            /*
             * List<Point3Df> point3Dfs = new List<Point3Df>() { p0, p1, p2, p3, p4, p5, p6, p7 };
            foreach (var p in point3Dfs)
            {
                var c0 = Cartesian3Utils.fromDegrees(p.x, p.z, p.y);
                p.x = (float)c0.X;
                p.y = (float)c0.Y;
                p.z = (float)c0.Z;
            }*/
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = new int[1] { 0 } } };
            gltf.Nodes = new Node[1] { new Node() { Mesh = 0 } };
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2, Material = 0 } } } };
            List<Point3Df> points = new List<Point3Df>() { p0, p0, p0, p1, p1, p1, p2, p2, p2, p3, p3, p3, p4, p4, p4, p5, p5, p5, p6, p6, p6, p7, p7, p7 };
            List<byte> list = new List<byte>();
            //list.AddRange(new byte[2] { 0, 0 });
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            normals.Add(MakerUtils.CalcTriangleNormal(points[9], points[0], points[3]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[5], points[1], points[13]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[14], points[2], points[11]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[0], points[3], points[6]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[8], points[4], points[17]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[16], points[5], points[1]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[3], points[6], points[9]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[10], points[7], points[19]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[20], points[8], points[4]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[6], points[9], points[0]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[23], points[10], points[7]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[2], points[11], points[22]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[15], points[12], points[21]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[1], points[13], points[16]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[22], points[14], points[2]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[18], points[15], points[12]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[13], points[16], points[5]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[4], points[17], points[20]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[21], points[18], points[15]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[7], points[19], points[23]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[17], points[20], points[8]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[12], points[21], points[18]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[11], points[22], points[14]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[19], points[23], points[10]));
            foreach (var p in normals)
                list.AddRange(p.ToByteList());

            List<ushort> indices = new List<ushort>() { 0, 3, 6, /**/ 6, 9, 0, /**/ 4, 17, 20, /**/ 20, 8, 4, /**/ 7, 19, 23, /**/ 23, 10, 7, /**/ 11, 22, 14 /**/ ,14, 2, 11
                /**/  ,1, 13, 16, /**/ 16, 5, 1 /**/ ,12, 21, 18, /**/ 18, 15, 12 };
            //List<ushort> indices = new List<ushort>() { 0, 1, 2, /**/ 2, 3, 0, /**/ 1, 5, 6, /**/ 6, 2, 1, /**/ 2, 6, 7, /**/ 7, 3, 2, /**/ 3, 7, 4, /**/ 4, 0, 3
              //  /**/  ,0, 4, 5, /**/ 5, 1, 0 /**/ ,4, 7, 6, /**/ 6, 5, 4 };
            foreach (var index in indices)
            {
                list.AddRange(BitConverter.GetBytes(index));
            }
            string base64 = Convert.ToBase64String(list.ToArray());
            gltf.Buffers = new glTFLoader.Schema.Buffer[1] { new glTFLoader.Schema.Buffer() { Uri = "data:application/octet-stream;base64," + base64, 
                ByteLength = list.Count } };

            gltf.BufferViews = new BufferView[3] { new BufferView() { Buffer = 0, ByteOffset = 0, 
                ByteLength = points.Count * 3 * 4, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48  
                Target = BufferView.TargetEnum.ARRAY_BUFFER
            }, new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4, 
                ByteLength = points.Count * 3 * 4, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48  
                Target = BufferView.TargetEnum.ARRAY_BUFFER
            }, new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4 * 2, 
                ByteLength = 2 * indices.Count, // ushort 两字节，4个点总共8字节
                Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER
                }
            };
            gltf.Accessors = new Accessor[3] { new Accessor() { BufferView = 0, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(points), Min = MakerUtils.GetMin(points) },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(normals), Min = MakerUtils.GetMin(normals) },
                new Accessor() { BufferView = 2, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 23 }, Min = new float[1] { 0 } }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0.766f, 0.336f, 1.0f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f} } };
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\cuboid.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        /// <summary>
        /// 创建长方体
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public static void CreateCuboidGltfWithOutNormal(Point3Df p0, Point3Df p1, Point3Df p2, Point3Df p3, Point3Df p4, Point3Df p5, Point3Df p6, Point3Df p7)
        {
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = new int[1] { 0 } } };
            gltf.Nodes = new Node[1] { new Node() { Mesh = 0 } };
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 1, Material = 0 } } } };
            List<Point3Df> points = new List<Point3Df>() { p0, p1, p2, p3, p4, p5, p6, p7 };
            List<byte> list = new List<byte>();
            //list.AddRange(new byte[2] { 0, 0 });
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<ushort> indices = new List<ushort>() { 0, 1, 2, /**/ 2, 3, 0, /**/ 1, 5, 6, /**/ 6, 2, 1, /**/ 2, 6, 7, /**/ 7, 3, 2, /**/ 3, 7, 4, /**/ 4, 0, 3
                /**/  ,0, 4, 5, /**/ 5, 1, 0 /**/ ,4, 7, 6, /**/ 6, 5, 4 };
            foreach (var index in indices)
            {
                list.AddRange(BitConverter.GetBytes(index));
            }
            string base64 = Convert.ToBase64String(list.ToArray());
            gltf.Buffers = new glTFLoader.Schema.Buffer[1] { new glTFLoader.Schema.Buffer() { Uri = "data:application/octet-stream;base64," + base64, 
                ByteLength = list.Count } };

            gltf.BufferViews = new BufferView[2] { new BufferView() { Buffer = 0, ByteOffset = 0, 
                ByteLength = points.Count * 3 * 4, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48  
                Target = BufferView.TargetEnum.ARRAY_BUFFER
            }, new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4, 
                ByteLength = 2 * indices.Count, // ushort 两字节，4个点总共8字节
                Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER
                }
            };
            gltf.Accessors = new Accessor[2] { new Accessor() { BufferView = 0, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(points), Min = MakerUtils.GetMin(points) },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 7 }, Min = new float[1] { 0 } }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0.766f, 0.336f, 1.0f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f} } };
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\cuboid.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }
    }
}
