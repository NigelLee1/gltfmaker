using GeoAPI.Geometries;
using glTFLoader.Schema;
using NetTopologySuite.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
        public static void CreateGdPipesGltf(List<Pipe> pipes, string path)
        {
            const ushort n = 16;
            //float r = pipes[0].width / 1000f / 2.0f;
            //float h = (float)Math.Sqrt((pipes[0].c0.X - pipes[0].c1.X) * (pipes[0].c0.X - pipes[0].c1.X) +
            //    (pipes[0].c0.Y - pipes[0].c1.Y) * (pipes[0].c0.Y - pipes[0].c1.Y));
            //r = r * 20;
            //h = h;
            float r = 10;
            float h = 1;
            /*var p0Gd3dCoord = CoordinateChangeUtils.GetGd3dCoord(new Coordinate(0, 0, 0));
            foreach (var pipe in pipes)
            {
                var gd3dCoord0 = CoordinateChangeUtils.GetGd3dCoord(pipe.c0);
                pipe.p0 = new Point3Df(gd3dCoord0.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord0.y, 0);

                var gd3dCoord1 = CoordinateChangeUtils.GetGd3dCoord(pipe.c1);
                pipe.p1 = new Point3Df(gd3dCoord1.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord1.y, 0);
            }*/
            int pipeCount = pipes.Count;
            //Point3Df p0 = new Point3Df(gd3dCoord.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord.y, r); // 84543000, 17237000 / 84542952, 17236860
            Point3Df p0 = new Point3Df(0, 0, r);
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            int[] nodes = new int[pipeCount];
            for (int i = 0; i < pipeCount; i++)
                nodes[i] = i;
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = nodes } };
            gltf.Nodes = new Node[pipeCount];
            for (int i = 0; i < pipeCount; i++)
            {
                float angle = CalcAngle(pipes[i].p0, pipes[i].p1);
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 0, 1), angle);
                Quaternion rotationQuaternion = Quaternion.CreateFromRotationMatrix(rotationMatrix);
                float len = (float)Math.Sqrt((pipes[i].p0.x - pipes[i].p1.x) * (pipes[i].p0.x - pipes[i].p1.x) +
                    (pipes[i].p0.y - pipes[i].p1.y) * (pipes[i].p0.y - pipes[i].p1.y));
                float newR = (float)(pipes[i].width / 1000d / 2.0d / CoordinateChangeUtils.resolution);
                int meshId;
                if (pipes[i].systemTypeCode == "YS")
                    meshId = 0;
                else if (pipes[i].systemTypeCode == "WS")
                    meshId = 1;
                else if (pipes[i].systemTypeCode == "HS")
                    meshId = 2;
                else
                    throw new Exception("error systemTypeCode:" + pipes[i].systemTypeCode);
                gltf.Nodes[i] = new Node()
                {
                    Mesh = meshId,
                    Name = "idd:" + pipes[i].id,
                    Translation = new float[3] { pipes[i].p0.x, pipes[i].p0.y, 0 },
                    Rotation = new float[4] { rotationQuaternion.X, rotationQuaternion.Y, rotationQuaternion.Z, rotationQuaternion.W },
                    Scale = new float[3] { len, newR, newR }
                }; //  Translation = new float[3] { pipes[0].p0.x, pipes[0].p0.y, 0 }
            }
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            attributes.Add("TEXCOORD_0", 2);
            gltf.Meshes = new Mesh[3] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 3, Material = 0 } } },
                new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 3, Material = 1 } } },
                new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 3, Material = 2 } } }};
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetNewPipePoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x + h, p0.y, p0.z);
            for (int i = 0; i < n; i++)
                points.Add(GetNewPipePoint(newP0, r, radian * i));
            int count = points.Count;
            for (int i = 0; i < count; i++)
                points.Add(new Point3Df(points[i].x, points[i].y, points[i].z));

            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 0; i < n; i++)
            {
                Point3Df p = GetNewPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            for (int i = 0; i < n; i++)
            {
                var p = GetNewPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            foreach (var p in normals)
                list.AddRange(p.ToByteList());

            List<Point2Df> textureCoords = new List<Point2Df>();
            for (int i = 0; i < 2 * n; i++)
                textureCoords.Add(new Point2Df(-1, -1));
            float x0 = 0;
            float x1 = 1;
            for (int i = 0; i <= n / 2; i++)
                textureCoords.Add(new Point2Df(x0, i / (n / 2.0f)));
            for (int i = n / 2 + 1; i < n; i++)
                textureCoords.Add(new Point2Df(x0, float.MaxValue));
                //textureCoords.Add(new Point2Df(x0, 1));
            for (int i = 0; i <= n / 2; i++)
                textureCoords.Add(new Point2Df(x1, i / (n / 2.0f)));
            for (int i = n / 2 + 1; i < n; i++)
                textureCoords.Add(new Point2Df(x1, float.MaxValue));    
                //textureCoords.Add(new Point2Df(x1, 1));    
            foreach (var p in textureCoords)
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

            gltf.BufferViews = new BufferView[4] { new BufferView() { Buffer = 0, ByteOffset = 0, 
                ByteLength = points.Count * 3 * 4, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48  
                Target = BufferView.TargetEnum.ARRAY_BUFFER
            }, new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4, 
                ByteLength = points.Count * 3 * 4, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48  
                Target = BufferView.TargetEnum.ARRAY_BUFFER
            }, new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4 * 2,     // 顶点texture坐标 (0,0)图片的左上角，（1，0）图片的右上角，
                ByteLength = points.Count * 2 * 4,                                       // (0, 1)图片的左下角，(1, 1)图片的右下角
                Target = BufferView.TargetEnum.ARRAY_BUFFER
            },  new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4 * 2 + points.Count * 2 * 4, 
                ByteLength = 2 * indices.Count, // ushort 两字节，4个点总共8字节
                Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER
                }
            };
            gltf.Accessors = new Accessor[4] { new Accessor() { BufferView = 0, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(points), Min = MakerUtils.GetMin(points) },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(normals), Min = MakerUtils.GetMin(normals) },
                new Accessor() { BufferView = 2, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC2, Max = new float[1] { 1 }, Min = new float[1] { 0 } },
                new Accessor() { BufferView = 3, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 23 }, Min = new float[1] { 0 } }};
            gltf.Materials = new Material[3] { 
                new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                    //BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 0.8f }}, 
                    BaseColorTexture = new TextureInfo() { Index = 0 } },
                    //MetallicFactor = 0.5f, RoughnessFactor = 0.1f}, 
                    EmissiveFactor = new float[] { 0f,0f, 0f } },
                new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                   // BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 0.8f }}, 
                    BaseColorTexture = new TextureInfo() { Index = 1 } },
                    //MetallicFactor = 0.5f, RoughnessFactor = 0.1f}, 
                    EmissiveFactor = new float[] { 0f,0f, 0f } },
                new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                    //BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 0.8f }}, 
                    BaseColorTexture = new TextureInfo() { Index = 2 } },
                    //MetallicFactor = 0.5f, RoughnessFactor = 0.1f}, 
                    EmissiveFactor = new float[] { 0f,0f, 0f } }}; // AlphaMode = Material.AlphaModeEnum.BLEND
            gltf.Textures = new Texture[3] { new Texture() { Sampler = 0, Source = 0 }, new Texture() { Sampler = 0, Source = 1 }, new Texture() { Sampler = 0, Source = 2 } };
            gltf.Samplers = new Sampler[1]{ new Sampler() { MagFilter = Sampler.MagFilterEnum.LINEAR, MinFilter = Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR,
                WrapS = Sampler.WrapSEnum.CLAMP_TO_EDGE, WrapT = Sampler.WrapTEnum.CLAMP_TO_EDGE } };
            string ysPngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ys_r2.png";
            string ysPngBase64 = MakerUtils.GetFileBase64String(ysPngPath);
            string wsPngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ws_r2.png";
            string wsPngBase64 = MakerUtils.GetFileBase64String(wsPngPath);
            string hsPngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\hs_r2.png";
            string hsPngBase64 = MakerUtils.GetFileBase64String(hsPngPath);
            gltf.Images = new Image[3] { new Image() { Uri = "data:application/octet-stream;base64," + ysPngBase64 }, new Image() { Uri = "data:application/octet-stream;base64," + wsPngBase64 },
                new Image() { Uri = "data:application/octet-stream;base64," + hsPngBase64 }};
            //gltf.Images = new Image[3] { new Image() { Uri = "ys_r2.png" }, new Image() { Uri = "ws_r2.png" },
              //  new Image() { Uri = "hs_r2.png" }};
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            //string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\pipes2.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }
        
        /// <summary>
        /// 创建圆柱体
        /// </summary>
        /// <param name="p0">圆心坐标</param>
        /// <param name="r">圆的半径</param>
        /// <param name="h">圆柱体的高度</param>
        /// <param name="n">圆的顶点个数</param>
        public static void CreateOnePipeGltf(List<Pipe> pipes)
        {
            const ushort n = 16;
            float r = pipes[0].width / 1000f / 2.0f;
            float h = (float)Math.Sqrt((pipes[0].p0.x - pipes[0].p1.x) * (pipes[0].p0.x - pipes[0].p1.x) +
                (pipes[0].p0.y - pipes[0].p1.y) * (pipes[0].p0.y - pipes[0].p1.y));
            r = r * 100;
            h = h * 100;
            var p0Gd3dCoord = CoordinateChangeUtils.GetGd3dCoord(new Coordinate(0, 0, 0));
            var gd3dCoord = CoordinateChangeUtils.GetGd3dCoord(pipes[0].c0);
            Point3Df p0 = new Point3Df(gd3dCoord.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord.y, r); // 84543000, 17237000 / 84542952, 17236860
            //Point3Df p0 = new Point3Df(0, 0, r);
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = new int[1] { 0 } } };
            gltf.Nodes = new Node[1] { new Node() { Mesh = 0 } }; //  Translation = new float[3] { pipes[0].p0.x, pipes[0].p0.y, 0 }
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2, Material = 0 } } } };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetNewPipePoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x + h, p0.y, p0.z);
            for (int i = 0; i < n; i++)
                points.Add(GetNewPipePoint(newP0, r, radian * i));
            int count = points.Count;
            for (int i = 0; i < count; i++)
                points.Add(new Point3Df(points[i].x, points[i].y, points[i].z));

            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 0; i < n; i++)
            {
                Point3Df p = GetNewPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            for (int i = 0; i < n; i++)
            {
                var p = GetNewPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
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
                BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 0.8f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f}, AlphaMode = Material.AlphaModeEnum.BLEND } }; // 
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\pipes2.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        private static Point3Df GetNewPipePoint(Point3Df p0, float r, double radian)
        {
            float x = p0.x;
            float y = p0.y + (float)(Math.Sin(radian - Math.PI / 2.0d) * r);
            float z = p0.z + (float)(Math.Cos(radian - Math.PI / 2.0d) * r);
            return new Point3Df(x, y, z);
        }

        private static float CalcAngle(Point3Df p0, Point3Df p1)
        {
            return (float)Math.Atan2(p1.y - p0.y, p1.x - p0.x);
        }
    }
}
