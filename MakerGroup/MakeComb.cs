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
        /// x轴向东，y轴向南,z轴向下
        /// </summary>
        public static void CreateComb()
        {
            const float xl = 1;
            const float yl = 1;
            const float zl = 1;
            Point3Df p0 = new Point3Df(0, 0, 0);
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
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2,
                Material = 0 } } } };
            List<Point3Df> points = new List<Point3Df>();
            points.AddRange(new Point3Df[] { new Point3Df(0, 1, -1), new Point3Df(1, 1, -1), new Point3Df(1, 0, -1), new Point3Df(0, 0, -1) }); // 0,1,2,3
            points.AddRange(new Point3Df[] { new Point3Df(0.125f, 0.75f, -1), new Point3Df(0.375f, 0.75f, -1), new Point3Df(0.375f, 0.25f, -1), new Point3Df(0.125f, 0.25f, -1) }); // 4,5,6,7
            points.AddRange(new Point3Df[] { new Point3Df(0.625f, 0.75f, -1), new Point3Df(0.875f, 0.75f, -1), new Point3Df(0.875f, 0.25f, -1), new Point3Df(0.625f, 0.25f, -1) }); // 8,9,10,11
            points.AddRange(new Point3Df[] { new Point3Df(0.125f, 0.75f, -1), new Point3Df(0.375f, 0.75f, -1), new Point3Df(0.375f, 0.75f, 0), new Point3Df(0.125f, 0.75f, 0) }); // 12,13,14,15
            points.AddRange(new Point3Df[] { new Point3Df(0.375f, 0.75f, -1), new Point3Df(0.375f, 0.25f, -1), new Point3Df(0.375f, 0.25f, 0), new Point3Df(0.375f, 0.75f, 0) }); // 16,17,18,19
            points.AddRange(new Point3Df[] { new Point3Df(0.375f, 0.25f, -1), new Point3Df(0.125f, 0.25f, -1), new Point3Df(0.125f, 0.25f, 0), new Point3Df(0.375f, 0.25f, 0) }); // 20,21,22,23
            points.AddRange(new Point3Df[] { new Point3Df(0.125f, 0.25f, -1), new Point3Df(0.125f, 0.75f, -1), new Point3Df(0.125f, 0.75f, 0), new Point3Df(0.125f, 0.25f, 0) }); // 24,25,26,27
            points.AddRange(new Point3Df[] { new Point3Df(0.625f, 0.75f, -1), new Point3Df(0.875f, 0.75f, -1), new Point3Df(0.875f, 0.75f, 0), new Point3Df(0.625f, 0.75f, 0) }); // 28,29,30,31
            points.AddRange(new Point3Df[] { new Point3Df(0.875f, 0.75f, -1), new Point3Df(0.875f, 0.25f, -1), new Point3Df(0.875f, 0.25f, 0), new Point3Df(0.875f, 0.75f, 0) }); // 32,33,34,35
            points.AddRange(new Point3Df[] { new Point3Df(0.875f, 0.25f, -1), new Point3Df(0.625f, 0.25f, -1), new Point3Df(0.625f, 0.25f, 0), new Point3Df(0.875f, 0.25f, 0) }); // 36,37,38,39
            points.AddRange(new Point3Df[] { new Point3Df(0.625f, 0.25f, -1), new Point3Df(0.625f, 0.75f, -1), new Point3Df(0.625f, 0.75f, 0), new Point3Df(0.625f, 0.25f, 0) }); // 40,41,42,43
            points.AddRange(new Point3Df[] { new Point3Df(0, 1, 0), new Point3Df(1, 1, 0), new Point3Df(1, 0, 0), new Point3Df(0, 0, 0) }); // 44,45,46,47 // 0,1,2,3
            points.AddRange(new Point3Df[] { new Point3Df(0.125f, 0.75f, 0), new Point3Df(0.375f, 0.75f, 0), new Point3Df(0.375f, 0.25f, 0), new Point3Df(0.125f, 0.25f, 0) }); // 48,49,50,51 // 4,5,6,7
            points.AddRange(new Point3Df[] { new Point3Df(0.625f, 0.75f, 0), new Point3Df(0.875f, 0.75f, 0), new Point3Df(0.875f, 0.25f, 0), new Point3Df(0.625f, 0.25f, 0) }); // 52,53,54,55 // 8,9,10,11
            points.AddRange(new Point3Df[] { new Point3Df(0, 1, -1), new Point3Df(0, 0, -1), new Point3Df(0, 0, 0), new Point3Df(0, 1, 0) }); // 56,57,58,59
            points.AddRange(new Point3Df[] { new Point3Df(1, 1, -1), new Point3Df(0, 1, -1), new Point3Df(0, 1, 0), new Point3Df(1, 1, 0) }); // 60,61,62,63
            points.AddRange(new Point3Df[] { new Point3Df(1, 0, -1), new Point3Df(1, 1, -1), new Point3Df(1, 1, 0), new Point3Df(1, 0, 0) }); // 64,65,66,67
            points.AddRange(new Point3Df[] { new Point3Df(0, 0, -1), new Point3Df(1, 0, -1), new Point3Df(1, 0, 0), new Point3Df(0, 0, 0) }); // 68,69,70,71
            List<byte> list = new List<byte>();
            foreach (var p in points)
            {
                p.x -= 0.5f;
                p.y -= 0.5f;
                list.AddRange(p.ToByteList());
            }

            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i <= 11; i++)
                normals.Add(new Point3Df(0, 0, -1));
            for (int i = 12; i <= 15; i++)
                normals.Add(new Point3Df(0, -1, 0));
            for (int i = 16; i <= 19; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 20; i <= 23; i++)
                normals.Add(new Point3Df(0, 1, 0));
            for (int i = 24; i <= 27; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 28; i <= 31; i++)
                normals.Add(new Point3Df(0, -1, 0));
            for (int i = 32; i <= 35; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 36; i <= 39; i++)
                normals.Add(new Point3Df(0, 1, 0));
            for (int i = 40; i <= 43; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 44; i <= 55; i++)
                normals.Add(new Point3Df(0, 0, 1));
            for (int i = 56; i <= 59; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 60; i <= 63; i++)
                normals.Add(new Point3Df(0, 1, 0));
            for (int i = 64; i <= 67; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 68; i <= 71; i++)
                normals.Add(new Point3Df(0, -1, 0));
            foreach (var p in normals)
                list.AddRange(p.ToByteList());

            List<ushort> indices = new List<ushort>();
            indices.AddRange(new ushort[3] { 0, 1, 4 }); // a
            indices.AddRange(new ushort[3] { 4, 1, 5 }); // b
            indices.AddRange(new ushort[3] { 5, 1, 8 }); // c
            indices.AddRange(new ushort[3] { 8, 1, 9 }); // d
            indices.AddRange(new ushort[3] { 9, 1, 10 }); // e
            indices.AddRange(new ushort[3] { 10, 1, 2 }); // f
            indices.AddRange(new ushort[3] { 10, 2, 3 }); // g
            indices.AddRange(new ushort[3] { 10, 3, 11 }); // h
            indices.AddRange(new ushort[3] { 11, 3, 6 }); // i
            indices.AddRange(new ushort[3] { 6, 3, 7 }); // j
            indices.AddRange(new ushort[3] { 7, 3, 4 }); // k
            indices.AddRange(new ushort[3] { 4, 3, 0 }); // l
            indices.AddRange(new ushort[3] { 5, 8, 6 }); // m
            indices.AddRange(new ushort[3] { 6, 8, 11 }); // n
            const ushort dd = 44;
            for (int i = 0; i <= 13; i++)
                indices.AddRange(new ushort[3] { (ushort)(indices[i * 3] + dd), (ushort)(indices[i * 3 + 2] + dd), (ushort)(indices[i * 3 + 1] + dd) });
            indices.AddRange(new ushort[3] { 12, 13, 15 });
            indices.AddRange(new ushort[3] { 13, 14, 15 });
            indices.AddRange(new ushort[3] { 16, 17, 18 });
            indices.AddRange(new ushort[3] { 16, 18, 19 });
            indices.AddRange(new ushort[3] { 20, 21, 22 });
            indices.AddRange(new ushort[3] { 20, 22, 23 });
            indices.AddRange(new ushort[3] { 24, 25, 26 });
            indices.AddRange(new ushort[3] { 24, 26, 27 });
            indices.AddRange(new ushort[3] { 28, 29, 30 });
            indices.AddRange(new ushort[3] { 28, 30, 31 });
            indices.AddRange(new ushort[3] { 32, 33, 34 });
            indices.AddRange(new ushort[3] { 32, 34, 35 });
            indices.AddRange(new ushort[3] { 36, 37, 38 });
            indices.AddRange(new ushort[3] { 36, 38, 39 });
            indices.AddRange(new ushort[3] { 40, 41, 42 });
            indices.AddRange(new ushort[3] { 40, 42, 43 });

            indices.AddRange(new ushort[3] { 56, 57, 58 });
            indices.AddRange(new ushort[3] { 56, 58, 59 });
            indices.AddRange(new ushort[3] { 60, 61, 62 });
            indices.AddRange(new ushort[3] { 60, 62, 63 });
            indices.AddRange(new ushort[3] { 64, 65, 66 });
            indices.AddRange(new ushort[3] { 64, 66, 67 });
            indices.AddRange(new ushort[3] { 68, 69, 70 });
            indices.AddRange(new ushort[3] { 68, 70, 71 });
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
            },
            new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4 * 2,
                ByteLength = 2 * indices.Count, // ushort 两字节，4个点总共8字节
                Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER
            }
            };
            gltf.Accessors = new Accessor[3] { new Accessor() { BufferView = 0, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(points), Min = MakerUtils.GetMin(points) },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(normals), Min = MakerUtils.GetMin(normals) },
                new Accessor() { BufferView = 2, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 71 }, Min = new float[1] { 0 } }
                };
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 0f, 1f, 0f, 1f }, 
                MetallicFactor = 0.5f, RoughnessFactor = 0.5f} } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Comb.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }
    }
}
