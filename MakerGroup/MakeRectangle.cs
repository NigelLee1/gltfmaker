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
        public static void CreateRectangleGltf(Point3Df p0, Point3Df p1, Point3Df p2, Point3Df p3)
        {
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = new int[1] { 0 } } };
            gltf.Nodes = new Node[1] { new Node() { Mesh = 0 } };
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 1);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 0, Material = 0 } } } };
            List<Point3Df> points = new List<Point3Df>() { p0, p1, p2, p3 };
            List<byte> list = new List<byte>();
            List<ushort> indices = new List<ushort>() { 0, 1, 2, 2, 3, 0 };
            foreach (var index in indices)
            {
                list.AddRange(BitConverter.GetBytes(index));
            }
            //list.AddRange(new byte[2] { 0, 0 });
            list.AddRange(p0.ToByteList());
            list.AddRange(p1.ToByteList());
            list.AddRange(p2.ToByteList());
            list.AddRange(p3.ToByteList());
            string base64 = Convert.ToBase64String(list.ToArray());
            gltf.Buffers = new glTFLoader.Schema.Buffer[1] { new glTFLoader.Schema.Buffer() { Uri = "data:application/octet-stream;base64," + base64, 
                ByteLength = list.Count } };

            gltf.BufferViews = new BufferView[2] { new BufferView() { Buffer = 0, ByteOffset = 0, 
                ByteLength = 2 * indices.Count,  // ushort 两字节，4个点总共8字节
                Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER
            }, new BufferView() { Buffer = 0, ByteOffset = 2 * indices.Count, // 要对齐，float 4字节，这里要是4的倍数
                ByteLength = 48, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48
                Target = BufferView.TargetEnum.ARRAY_BUFFER
                }
            };
            gltf.Accessors = new Accessor[2] { new Accessor() { BufferView = 0, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 3 }, Min = new float[1] { 0 } },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = 4, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(points),
                Min = MakerUtils.GetMin(points) }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0.766f, 0.336f, 1.0f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f} } };

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\rectangle.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }
    }
}
