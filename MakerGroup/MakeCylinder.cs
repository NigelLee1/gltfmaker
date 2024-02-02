using glTFLoader.Schema;
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
        /// 创建圆柱体
        /// </summary>
        /// <param name="p0">圆心坐标</param>
        /// <param name="r">圆的半径</param>
        /// <param name="h">圆柱体的高度</param>
        /// <param name="n">圆的顶点个数</param>
        public static void CreateCylinderGltf(Point3Df p0, float r, float h, ushort n = 8)
        {
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = new int[1] { 0 } } };
            gltf.Nodes = new Node[1] { new Node() { Mesh = 0 } };
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2, Material = 0 } } } };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetNewPoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x, p0.y, p0.z - h);
            for (int i = 0; i < n; i++)
                points.Add(GetNewPoint(newP0, r, radian * i));
            int count = points.Count;
            for (int i = 0; i < count; i++)
                points.Add(new Point3Df(points[i].x, points[i].y, points[i].z));

            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(0, 0, 1));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(0, 0, -1));
            for (int i = 0; i < n; i++)
                normals.Add(GetNewPoint(new Point3Df(0, 0, 0), 1, radian * i));
            for (int i = 0; i < n; i++)
                normals.Add(GetNewPoint(new Point3Df(0, 0, 0), 1, radian * i));
            foreach (var p in normals)
                list.AddRange(p.ToByteList());

            List<ushort> indices = new List<ushort>();
            for (ushort i = 1; i < n - 1; i++)
                indices.AddRange(new ushort[3] { 0, i, (ushort)(i + 1) });
            for (ushort i = (ushort)(n + 1); i < n + n - 1; i++)
                indices.AddRange(new ushort[3] { n, (ushort)(i + 1), i });
            for (ushort i = 0; i < n - 1; i++)
            {
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(3 * n + i), (ushort)(3 * n + 1 + i) });
                indices.AddRange(new ushort[3] { (ushort)(3 * n + 1 + i), (ushort)(2 * n + 1 + i), (ushort)(2 * n + i) });
            }
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(3 * n + n - 1), (ushort)(3 * n) });
            indices.AddRange(new ushort[3] { (ushort)(3 * n), (ushort)(2 * n), (ushort)(2 * n + n - 1) });
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
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 23 }, Min = new float[1] { 0 } }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0.766f, 0.336f, 0.5f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f}, AlphaMode = Material.AlphaModeEnum.BLEND } };
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\cylinder.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        private static Point3Df GetNewPoint(Point3Df p0, float r, double radian)
        {
            float y = p0.y + (float)(Math.Sin(radian) * r);
            float x = p0.x + (float)(Math.Cos(radian) * r);
            return new Point3Df(x, y, p0.z);
        }
    }
}
