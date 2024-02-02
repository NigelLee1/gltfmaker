using GeoAPI.Geometries;
using glTFLoader.Schema;
using NetTopologySuite.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker.MakerGroup
{
    static class ManholeMaker
    {
        /// <summary>
        /// x轴向东，y轴向南,z轴向下
        /// </summary>
        public static void CreateManhole()
        {
            const ushort n = 16;
            const float r = 1;
            const float h = 1;
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
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { 
                Attributes = attributes, Indices = 2, Material = 0 } } } };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumInstancedPipePoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x, p0.y, p0.z - h);
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumInstancedPipePoint(newP0, r, radian * i));
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
            {
                Point3Df p = GetCesiumInstancedPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            for (int i = 0; i < n; i++)
            {
                var p = GetCesiumInstancedPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            foreach (var p in normals)
                list.AddRange(p.ToByteList());

            List<ushort> indices = new List<ushort>();
            for (ushort i = 1; i < n - 1; i++)
                indices.AddRange(new ushort[3] { 0, i, (ushort)(i + 1)  });
            for (ushort i = (ushort)(n + 1); i < n + n - 1; i++)
                indices.AddRange(new ushort[3] { n, (ushort)(i + 1), i  });
            for (ushort i = 0; i < n - 1; i++)
            {
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(3 * n + i), (ushort)(3 * n + 1 + i) });
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(3 * n + 1 + i), (ushort)(2 * n + 1 + i) });
            }
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(3 * n + n - 1), (ushort)(3 * n) });
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(3 * n), (ushort)(2 * n) });
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
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 4 * n - 1 }, Min = new float[1] { 0 } }
                };
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = MakerUtils.GetFloatColor(174, 251, 136), 
                //BaseColorTexture = new TextureInfo() { Index = 0 },
                MetallicFactor = 0f, RoughnessFactor = 1f} } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective
            //gltf.Textures = new Texture[1] { new Texture() { Sampler = 0, Source = 0 } };
            //gltf.Samplers = new Sampler[1]{ new Sampler() { MagFilter = Sampler.MagFilterEnum.LINEAR, MinFilter = Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR,
              //  WrapS = Sampler.WrapSEnum.CLAMP_TO_EDGE, WrapT = Sampler.WrapTEnum.CLAMP_TO_EDGE } };
            //Assembly assm = Assembly.GetExecutingAssembly();
            //Stream stream = assm.GetManifestResourceStream("gltfMaker.assets.hs_r.png");
            
            //string pngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ys_r.png";
            //string pngBase64 = MakerUtils.GetFileBase64String(pngPath);

            //gltf.Images = new Image[1] { new Image() { Uri = "data:application/octet-stream;base64," + pngBase64 } };
            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\YSRoundManhole.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);

            //pngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ws_r.png";
            //pngBase64 = MakerUtils.GetFileBase64String(pngPath);
            //gltf.Images = new Image[1] { new Image() { Uri = "data:application/octet-stream;base64," + pngBase64 } };
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = MakerUtils.GetFloatColor(235, 161, 161), 
                //BaseColorTexture = new TextureInfo() { Index = 0 },
                MetallicFactor = 0f, RoughnessFactor = 1f} } }; 
            path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\WSRoundManhole.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);

            //pngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\hs_r.png";
            //pngBase64 = MakerUtils.GetFileBase64String(pngPath);
            //gltf.Images = new Image[1] { new Image() { Uri = "data:application/octet-stream;base64," + pngBase64 } };
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = MakerUtils.GetFloatColor(254, 202, 162), 
                //BaseColorTexture = new TextureInfo() { Index = 0 },
                MetallicFactor = 0f, RoughnessFactor = 1f} } };
            path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\HSRoundManhole.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        private static Point3Df GetCesiumInstancedPipePoint(Point3Df p0, float r, double radian)
        {
            float x = p0.x + (float)(Math.Cos(radian) * r);
            float y = p0.y + (float)(Math.Sin(radian) * r);
            float z = p0.z;
            return new Point3Df(x, y, z);
        }
    }
}
