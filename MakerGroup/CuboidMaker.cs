using GeoAPI.Geometries;
using glTFLoader.Schema;
using NetTopologySuite.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker.MakerGroup
{
    static class CuboidMaker
    {
        /// <summary>
        /// x轴向东，y轴向南,z轴向下
        /// </summary>
        public static void CreateCuboid()
        {
            const float xl = 2;
            const float yl = 2;
            const float h = 1;
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = new int[1] { 0 } } };
            gltf.Nodes = new Node[1] { new Node()
                {
                    Mesh = 0
                } }; 
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { 
                Attributes = attributes, Indices = 2, Material = 0 } } } };
            Point3Df p0 = new Point3Df(-xl / 2, yl / 2, 0);
            Point3Df p1 = new Point3Df(xl / 2, yl / 2, 0);
            Point3Df p2 = new Point3Df(xl / 2, -yl / 2, 0);
            Point3Df p3 = new Point3Df(-xl / 2, -yl / 2, 0);
            Point3Df p4 = new Point3Df(-xl / 2, yl / 2, -h);
            Point3Df p5 = new Point3Df(xl / 2, yl / 2, -h);
            Point3Df p6 = new Point3Df(xl / 2, -yl / 2, -h);
            Point3Df p7 = new Point3Df(-xl / 2, -yl / 2, -h);
            List<Point3Df> points = new List<Point3Df>() { p0, p0, p0, p1, p1, p1, p2, p2, p2, p3, p3, p3, p4, p4, p4, p5, p5, p5, p6, p6, p6, p7, p7, p7 };
            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            normals.Add(MakerUtils.CalcTriangleNormal(points[3], points[0], points[9]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[13], points[1], points[5]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[11], points[2], points[14]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[6], points[3], points[0]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[17], points[4], points[8]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[1], points[5], points[16]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[9], points[6], points[3]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[19], points[7], points[10]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[4], points[8], points[20]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[0], points[9], points[6]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[7], points[10], points[23]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[22], points[11], points[2]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[21], points[12], points[15]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[16], points[13], points[1]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[2], points[14], points[22]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[12], points[15], points[18]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[5], points[16], points[13]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[20], points[17], points[4]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[15], points[18], points[21]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[23], points[19], points[7]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[8], points[20], points[17]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[18], points[21], points[12]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[14], points[22], points[11]));
            normals.Add(MakerUtils.CalcTriangleNormal(points[10], points[23], points[19]));
            foreach (var p in normals)
                list.AddRange(p.ToByteList());

            List<ushort> indices = new List<ushort>() { 0, 6, 3, /**/ 0, 9, 6, /**/ 4, 20, 17, /**/ 4, 8, 20, /**/ 7, 23, 19, 
                /**/7, 10, 23, /**/14, 22, 11 /**/ ,14, 11, 2
                /**/  ,16, 13, 1, /**/ 1, 5, 16 /**/ ,18, 21, 12, /**/ 12, 15, 18 };
            foreach (var index in indices)
                list.AddRange(BitConverter.GetBytes(index));

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
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 23 }, Min = new float[1] { 0 } }
                };
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = MakerUtils.GetFloatColor(174, 251, 136), 
                MetallicFactor = 0f, RoughnessFactor = 1f} } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\YSCuboid.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);

            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = MakerUtils.GetFloatColor(235, 161, 161), 
                MetallicFactor = 0f, RoughnessFactor = 1f} } }; 
            path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\WSCuboid.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);

            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = MakerUtils.GetFloatColor(254, 202, 162), 
                MetallicFactor = 0f, RoughnessFactor = 1f} } };
            path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\HSCuboid.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }
    }
}
