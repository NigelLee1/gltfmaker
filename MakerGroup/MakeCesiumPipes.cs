using Esri.ArcGISRuntime.Geometry;
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
        /// x轴向东，y轴向南,z轴向下
        /// </summary>
        /// <param name="pipes"></param>
        public static void Create3DTileIdentityPipeWithArrow()
        {
            const ushort n = 16;
            const float r = 1;
            const float h = 1;
            Point3Df p0 = new Point3Df(0, 0, -1);
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
            attributes.Add("TEXCOORD_0", 2);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 3, Material = 0 } } } };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumInstancedPipePoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x + h, p0.y, p0.z);
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
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(1, 0, 0));
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

            List<Point2Df> textureCoords = new List<Point2Df>();
            /*for (int i = 0; i <= n / 4; i++)
                textureCoords.Add(new Point2Df(0, 0.5f + i / (n / 2.0f)));
            for (int i = n / 2 - 1; i > 0; i--)
                textureCoords.Add(new Point2Df(0, i / (n / 2.0f)));
            for (int i = 0; i < n / 4; i++)
                textureCoords.Add(new Point2Df(0, i / (n / 2.0f)));
            for (int i = 0; i < n; i++)
                textureCoords.Add(new Point2Df(1, textureCoords[i].y));
            for (int i = 0; i < 2 * n; i++)
                textureCoords.Add(textureCoords[i]);
            for (int i = 0; i < 2 * n; i++)
                textureCoords[i] = new Point2Df(-1, -1);*/
           
            for (int i = 0; i < 2 * n; i++)
                textureCoords.Add(new Point2Df(-1, -1));
            /*if (Math.PI * r / h > imageW / imageH)
            {
                float myW = (float)(h * imageW / imageH);
                float y0 = 0.5f - (float)(0.5f * Math.PI * r / myW);
                float y1 = 0.5f + (float)(0.5f * Math.PI * r / myW);
                for (int i = 0; i <= n / 2; i++)
                    textureCoords.Add(new Point2Df(0, y0 + (y1 - y0) * i / (n / 2.0f)));
                for (int i = n / 2 + 1; i < n; i++)
                    textureCoords.Add(new Point2Df(0, float.MaxValue));
                for (int i = 0; i <= n / 2; i++)
                    textureCoords.Add(new Point2Df(1, y0 + (y1 - y0) * i / (n / 2.0f)));
                for (int i = n / 2 + 1; i < n; i++)
                    textureCoords.Add(new Point2Df(0, float.MaxValue));
            }
            else*/
            {
                // float myH = (float)(Math.PI * r * imageH / imageW);
                // float x0 = 0.5f - 0.5f * h / myH;
                // float x1 = 0.5f + 0.5f * h / myH;
                float x0 = 0;
                float x1 = 1;
                for (int i = 0; i <= n / 2; i++)
                    textureCoords.Add(new Point2Df(x0, i / (n / 2.0f)));
                for (int i = n / 2 + 1; i < n; i++)
                    textureCoords.Add(new Point2Df(x0, float.MaxValue));
                for (int i = 0; i <= n / 2; i++)
                    textureCoords.Add(new Point2Df(x1, i / (n / 2.0f)));
                for (int i = n / 2 + 1; i < n; i++)
                    textureCoords.Add(new Point2Df(x1, float.MaxValue));    
            }
            /*for (int i = 0; i < 2 * n; i++)
                textureCoords.Add(new Point2Df(-1, -1));
            for (int i = 0; i <= n / 2; i++)
                textureCoords.Add(new Point2Df(0, i / (n / 2.0f)));
            for (int i = n / 2 + 1; i < n; i++)
                textureCoords.Add(new Point2Df(0, float.MaxValue));
            for (int i = 0; i <= n / 2; i++)
                textureCoords.Add(new Point2Df(1, i / (n / 2.0f)));
            for (int i = n / 2 + 1; i < n; i++)
                textureCoords.Add(new Point2Df(0, float.MaxValue));*/
            foreach (var p in textureCoords)
                list.AddRange(p.ToByteList());

            List<ushort> indices = new List<ushort>();
            for (ushort i = 1; i < n - 1; i++)
                indices.AddRange(new ushort[3] { 0, (ushort)(i + 1), i  });
            for (ushort i = (ushort)(n + 1); i < n + n - 1; i++)
                indices.AddRange(new ushort[3] { n, i, (ushort)(i + 1) });
            for (ushort i = 0; i < n - 1; i++)
            {
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(3 * n + 1 + i), (ushort)(3 * n + i) });
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(2 * n + 1 + i), (ushort)(3 * n + 1 + i) });
            }
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(3 * n), (ushort)(3 * n + n - 1) });
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(2 * n), (ushort)(3 * n) });
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
            },
            new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4 * 2 + points.Count * 2 * 4,
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
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 4 * n - 1 }, Min = new float[1] { 0 } }
                };
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                //BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 0.8f }, 
                BaseColorTexture = new TextureInfo() { Index = 0 },
                MetallicFactor = 0.5f, RoughnessFactor = 0.1f} } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective
            gltf.Textures = new Texture[1] { new Texture() { Sampler = 0, Source = 0 } };
            gltf.Samplers = new Sampler[1]{ new Sampler() { MagFilter = Sampler.MagFilterEnum.LINEAR, MinFilter = Sampler.MinFilterEnum.LINEAR_MIPMAP_LINEAR,
                WrapS = Sampler.WrapSEnum.CLAMP_TO_EDGE, WrapT = Sampler.WrapTEnum.CLAMP_TO_EDGE } };

            string pngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ys_r.png";
            string pngBase64 = MakerUtils.GetFileBase64String(pngPath);
            gltf.Images = new Image[1] { new Image() { Uri = "data:application/octet-stream;base64," + pngBase64 } };
            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\IdentityPipeWithYSArrow.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);

            pngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ws_r.png";
            pngBase64 = MakerUtils.GetFileBase64String(pngPath);
            gltf.Images = new Image[1] { new Image() { Uri = "data:application/octet-stream;base64," + pngBase64 } };
            path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\IdentityPipeWithWSArrow.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);

            pngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\hs_r.png";
            pngBase64 = MakerUtils.GetFileBase64String(pngPath);
            gltf.Images = new Image[1] { new Image() { Uri = "data:application/octet-stream;base64," + pngBase64 } };
            path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\IdentityPipeWithHSArrow.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        private static Point3Df GetCesiumInstancedPipePoint(Point3Df p0, float r, double radian)
        {
            float x = p0.x;
            float y = p0.y + (float)(Math.Cos(radian + Math.PI) * r);
            float z = p0.z + (float)(Math.Sin(radian + Math.PI) * r);
            return new Point3Df(x, y, z);
        }

        /// <summary>
        /// x轴向东，y轴向天，z轴向南   featureTableJson.EAST_NORTH_UP = true;
        /// </summary>
        /// <param name="pipes"></param>
        public static void Create3DTileIdentityPipe()
        {
            const ushort n = 16;
            const float r = 10;
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
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2, Material = 0 } } } };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x + h, p0.y, p0.z);
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePoint(newP0, r, radian * i));
            int count = points.Count;
            for (int i = 0; i < count; i++)
                points.Add(new Point3Df(points[i].x, points[i].y, points[i].z));

            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 0; i < n; i++)
            {
                Point3Df p = GetCesiumPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            for (int i = 0; i < n; i++)
            {
                var p = GetCesiumPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
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
                indices.AddRange(new ushort[3] { 0, (ushort)(i + 1), i  });
            for (ushort i = (ushort)(n + 1); i < n + n - 1; i++)
                indices.AddRange(new ushort[3] { n, i, (ushort)(i + 1) });
            for (ushort i = 0; i < n - 1; i++)
            {
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(3 * n + 1 + i), (ushort)(3 * n + i) });
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(2 * n + 1 + i), (ushort)(3 * n + 1 + i) });
            }
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(3 * n), (ushort)(3 * n + n - 1) });
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(2 * n), (ushort)(3 * n) });
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
                BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 1f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f} } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\IdentityPipe.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }
        /// <summary>
        /// x轴向东，y轴向天，z轴向南
        /// </summary>
        /// <param name="pipes"></param>
        public static void CreateCesiumPipes3DTileGltf(List<Pipe> pipes) 
        {
            const ushort n = 16;
            //float r = pipes[0].width / 1000f / 2.0f;
            //float h = (float)Math.Sqrt((pipes[0].c0.X - pipes[0].c1.X) * (pipes[0].c0.X - pipes[0].c1.X) +
            //    (pipes[0].c0.Y - pipes[0].c1.Y) * (pipes[0].c0.Y - pipes[0].c1.Y));
            //r = r * 20;
            //h = h;
            const float r = 10;
            const float h = 1;
            int pipeCount = 1;
            int originPipeCount = pipeCount;
            //Coordinate center = CoordinateChangeUtils.GetCesiumCoord(pipes, pipeCount);
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue, maxH = float.MinValue;
            for (int i = 0; i < pipeCount; i++)
            {
                var pipe = pipes[i];
                // pipe.p0 = new Point3Df((float)pipe.c0.X, (float)pipe.c0.Y, 0);
                // pipe.p1 = new Point3Df((float)pipe.c1.X, (float)pipe.c1.Y, 0);
                //pipe.p0 = CoordinateChangeUtils.GetCesium3DTileCoord(pipe.c0);
                //pipe.p1 = CoordinateChangeUtils.GetCesium3DTileCoord(pipe.c1);
                //Console.WriteLine("i:" + i + ",p0:" + pipe.p0 + ",p1:" + pipe.p1 + ",c0:" + pipe.c0 + ",c1:" + pipe.c1);

                // if (i < 1)
                /*{
                    if (pipe.p0.x < minX)
                        minX = pipe.p0.x;
                    if (pipe.p0.y < minY)
                        minY = pipe.p0.y;
                    if (pipe.p1.x < minX)
                        minX = pipe.p1.x;
                    if (pipe.p1.y < minY)
                        minY = pipe.p1.y;

                    if (pipe.p0.x > maxX)
                        maxX = pipe.p0.x;
                    if (pipe.p0.y > maxY)
                        maxY = pipe.p0.y;
                    if (pipe.p1.x > maxX)
                        maxX = pipe.p1.x;
                    if (pipe.p1.y > maxY)
                        maxY = pipe.p1.y;
                }*/
                GeoAPI.Geometries.Envelope envelope = pipe.GetExtent();
                if (envelope.MinX < minX)
                    minX = (float)envelope.MinX;
                if (envelope.MinY < minY)
                    minY = (float)envelope.MinY;
                if (envelope.MaxX > maxX)
                    maxX = (float)envelope.MaxX;
                if (envelope.MaxY > maxY)
                    maxY = (float)envelope.MaxY;
                float hh = (float)(pipes[i].width / 1000d * r);
                if (hh > maxH)
                    maxH = hh;
            }
            Console.WriteLine("minX:" + minX.ToString("G9") + ",minY:" + minY.ToString("G9") + ",maxX:" + maxX.ToString("G9") + ",maxY:" + maxY.ToString());
            var minPoint = new MapPoint(minX, minY, new SpatialReference(3857));
            minPoint = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(minPoint, new SpatialReference(4326));
            var maxPoint = new MapPoint(maxX, maxY, new SpatialReference(3857));
            maxPoint = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(maxPoint, new SpatialReference(4326));
            Console.WriteLine("minX_:" + minPoint.X.ToString("G9") + ",minY_:" + minPoint.Y.ToString("G9") 
                + ",maxX_:" + maxPoint.X.ToString("G9") + ",maxY_:" + maxPoint.Y.ToString() + ",h:" + maxH.ToString("G9"));

            float originX = (minX + maxX) / 2.0f;
            float originY = (minY + maxY) / 2.0f;
           // float originX3857 = (float)((originX - center.X) * 1.088 + center.X);
           // float originY3857 = (float)((originY - center.Y) * 1.088 + center.Y);
            //var mapPoint = new MapPoint(originX3857, originY3857, new SpatialReference(3857));
            var mapPoint = new MapPoint(originX, originY, new SpatialReference(3857));
            mapPoint = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mapPoint, new SpatialReference(4326));

            //return new Point(mapPoint.X, mapPoint.Y);
            //float originX = pipes[0].p0.x;
            //float originY = pipes[0].p0.y;
            Console.WriteLine("originX:" + mapPoint.X.ToString("G9") + ",originY:" + mapPoint.Y.ToString("G9"));
            CoordinateChangeUtils.GetCesiumCoord(pipes, pipeCount, new Coordinate(originX, originY));
            for (int i = pipeCount - 1; i >= 0; i--)
            {
                if (pipes[i].p0.x == pipes[i].p1.x && pipes[i].p0.y == pipes[i].p1.y || pipes[i].width == 0)
                {
                    pipes.RemoveAt(i);
                    pipeCount--;
                }
            }
            float maxLen = float.MinValue;
            for (int i = 0; i < pipeCount; i++)
            {
                var pipe = pipes[i];
                if (Math.Abs(pipe.p1.x - pipe.p0.x) > maxLen)
                    maxLen = Math.Abs(pipe.p1.x - pipe.p0.x);
                if (Math.Abs(pipe.p1.y - pipe.p0.y) > maxLen)
                    maxLen = Math.Abs(pipe.p1.y - pipe.p0.y);
            }
            Console.WriteLine("maxLen:" + maxLen.ToString("G9"));
            //Point3Df p0 = new Point3Df(gd3dCoord.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord.y, r); // 84543000, 17237000 / 84542952, 17236860
            //Point3Df p0 = new Point3Df(0, r, 0);
            Point3Df p0 = new Point3Df(0, 0, 0);
            Gltf gltf = new Gltf();
            gltf.Asset = new Asset() { Version = "2.0" };
            int[] nodes = new int[pipeCount];
            for (int i = 0; i < pipeCount; i++)
                nodes[i] = i + 1;
            gltf.Scenes = new Scene[1] { new Scene() { Nodes = new int[1] { 0 }, Name = "suwnScene" } };
            gltf.Nodes = new Node[pipeCount + 1];
            //Matrix4x4 rotationMatrix2 = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)(Math.PI/2.0));
            //Quaternion rotationQuaternion2 = Quaternion.CreateFromRotationMatrix(rotationMatrix2);
            gltf.Nodes[0] = new Node() { Children = nodes, Name = "suwnNode0"/*, Rotation = new float[4] { rotationQuaternion2.X, rotationQuaternion2.Y, rotationQuaternion2.Z, rotationQuaternion2.W }*/ };
            for (int i = 0; i < pipeCount; i++)
            {
                float angle = CalcAngle(pipes[i].p0, pipes[i].p1);
                //float angle = (float)(Math.PI / 2);
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
                Quaternion rotationQuaternion = Quaternion.CreateFromRotationMatrix(rotationMatrix);
                float len = (float)Math.Sqrt((pipes[i].p0.x - pipes[i].p1.x) * (pipes[i].p0.x - pipes[i].p1.x) +
                    (pipes[i].p0.y - pipes[i].p1.y) * (pipes[i].p0.y - pipes[i].p1.y));
                float newR = (float)(pipes[i].width / 1000d / 2.0d);
                Dictionary<string, object> extensions = new Dictionary<string, object>();
                extensions.Add("idsuwn:", pipes[i].id);
                gltf.Nodes[i + 1] = new Node()
                {
                    Mesh = 0,
                    Name = "iddsuwn:" + pipes[i].id,
                    Extras = new MyExtras() { data = "extrassuwn:" + pipes[i].id },
                    Extensions = extensions,
                    //Translation = new float[3] { pipes[i].p0.x - originX, 0, originY - pipes[i].p0.y },
                    Translation = new float[3] { pipes[i].p0.x - originX, 0, originY - pipes[i].p0.y },
                    Rotation = new float[4] { rotationQuaternion.X, rotationQuaternion.Y, rotationQuaternion.Z, rotationQuaternion.W },
                    Scale = new float[3] { len, newR, newR }
                }; 
            }
            
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            attributes.Add("_BATCHID", 3);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2, Material = 0, Extras = new MyExtras() { data = "suwnMeshPrimitive" } } },
                Name = "suwnMesh"} };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x + h, p0.y, p0.z);
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePoint(newP0, r, radian * i));
            int count = points.Count;
            for (int i = 0; i < count; i++)
                points.Add(new Point3Df(points[i].x, points[i].y, points[i].z));

            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 0; i < n; i++)
            {
                Point3Df p = GetCesiumPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            for (int i = 0; i < n; i++)
            {
                var p = GetCesiumPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
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
                indices.AddRange(new ushort[3] { 0, (ushort)(i + 1), i  });
            for (ushort i = (ushort)(n + 1); i < n + n - 1; i++)
                indices.AddRange(new ushort[3] { n, i, (ushort)(i + 1) });
            for (ushort i = 0; i < n - 1; i++)
            {
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(3 * n + 1 + i), (ushort)(3 * n + i) });
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(2 * n + 1 + i), (ushort)(3 * n + 1 + i) });
            }
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(3 * n), (ushort)(3 * n + n - 1) });
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(2 * n), (ushort)(3 * n) });
            foreach (var index in indices)
                list.AddRange(BitConverter.GetBytes(index));
            for (int i = 0; i < points.Count; i++)
            {
                float batchId = 0;
                list.AddRange(BitConverter.GetBytes(batchId));
            }

            string base64 = Convert.ToBase64String(list.ToArray());
            gltf.Buffers = new glTFLoader.Schema.Buffer[1] { new glTFLoader.Schema.Buffer() { Uri = "data:application/octet-stream;base64," + base64, 
                ByteLength = list.Count } };

            gltf.BufferViews = new BufferView[4] { new BufferView() { Buffer = 0, ByteOffset = 0,
                ByteLength = points.Count * 3 * 4, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48  
                Target = BufferView.TargetEnum.ARRAY_BUFFER, Name = "suwnBufferView0"
            }, new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4,
                ByteLength = points.Count * 3 * 4, // 4个点，每个点xyz坐标分别占4字节，所以为4*3*4=48  
                Target = BufferView.TargetEnum.ARRAY_BUFFER, Name = "suwnBufferView1"
            }, new BufferView() { Buffer = 0, ByteOffset = points.Count * 3 * 4 * 2,
                ByteLength = 2 * indices.Count, // ushort 两字节，4个点总共8字节
                Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER, Name = "suwnBufferView2"
            },  new BufferView() { Buffer = 0, ByteOffset = (points.Count * 3 * 4 * 2 + 2 * indices.Count), ByteLength = 4 * points.Count, 
                Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER, Name = "suwnBufferView3" }
            };
            gltf.Accessors = new Accessor[4] { new Accessor() { BufferView = 0, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(points), Min = MakerUtils.GetMin(points), Name = "suwnAccesor0" },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = MakerUtils.GetMax(normals), Min = MakerUtils.GetMin(normals), Name = "suwnAccesor1" },
                new Accessor() { BufferView = 2, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 4 * n - 1 }, Min = new float[1] { 0 }, Name = "suwnAccesor2" },
                new Accessor() { BufferView = 3, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT, Count = points.Count,
                Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 0 }, Min = new float[1] { 0 }, Name = "suwnAccesor3" }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 1f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f}, Name = "suwnMaterial" } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Cesium" + DateTime.Now.ToString("yyyyMMdd")
                + "Pipes" + originPipeCount + ".gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        private static Point3Df GetCesiumPipePoint(Point3Df p0, float r, double radian)
        {
            float x = p0.x;
            float y = p0.y + (float)(Math.Cos(radian - Math.PI / 2) * r);
            float z = p0.z + (float)(Math.Sin(radian - Math.PI / 2) * r);
            return new Point3Df(x, y, z);
        }
        /// <summary>
        /// 创建圆柱体
        /// </summary>
        /// <param name="p0">圆心坐标</param>
        /// <param name="r">圆的半径</param>
        /// <param name="h">圆柱体的高度</param>
        /// <param name="n">圆的顶点个数</param>
        /*public static void CreateCesiumPipes3DTileGltf(List<Pipe> pipes)
        {
            const ushort n = 16;
            //float r = pipes[0].width / 1000f / 2.0f;
            //float h = (float)Math.Sqrt((pipes[0].c0.X - pipes[0].c1.X) * (pipes[0].c0.X - pipes[0].c1.X) +
            //    (pipes[0].c0.Y - pipes[0].c1.Y) * (pipes[0].c0.Y - pipes[0].c1.Y));
            //r = r * 20;
            //h = h;
            const float r = 10;
            const float h = 1;
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            int pipeCount = 10;
            for (int i = 0; i < pipeCount; i++)
            {
                var pipe = pipes[i];
                pipe.p0 = new Point3Df((float)pipe.c0.X, (float)pipe.c0.Y, 0);
                pipe.p1 = new Point3Df((float)pipe.c1.X, (float)pipe.c1.Y, 0);
                Console.WriteLine("i:" + i + ",p0:" + pipe.p0 + ",p1:" + pipe.p1);
               // if (i < 1)
                {
                    if (pipe.p0.x < minX)
                        minX = pipe.p0.x;
                    if (pipe.p0.y < minY)
                        minY = pipe.p0.y;
                    if (pipe.p1.x < minX)
                        minX = pipe.p1.x;
                    if (pipe.p1.y < minY)
                        minY = pipe.p1.y;

                    if (pipe.p0.x > maxX)
                        maxX = pipe.p0.x;
                    if (pipe.p0.y > maxY)
                        maxY = pipe.p0.y;
                    if (pipe.p1.x > maxX)
                        maxX = pipe.p1.x;
                    if (pipe.p1.y > maxY)
                        maxY = pipe.p1.y;
                }
            }
            float originX = (minX + maxX) / 2.0f;
            float originY = (minY + maxY) / 2.0f;
            //float originX = pipes[0].p0.x;
            //float originY = pipes[0].p0.y;
            Console.WriteLine("originX:" + originX.ToString("G9") + ",originY:" + originY.ToString("G9"));
            //Point3Df p0 = new Point3Df(gd3dCoord.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord.y, r); // 84543000, 17237000 / 84542952, 17236860
            Point3Df p0 = new Point3Df(0, r, 0);
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
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
                Quaternion rotationQuaternion = Quaternion.CreateFromRotationMatrix(rotationMatrix);
                float len = (float)Math.Sqrt((pipes[i].p0.x - pipes[i].p1.x) * (pipes[i].p0.x - pipes[i].p1.x) +
                    (pipes[i].p0.y - pipes[i].p1.y) * (pipes[i].p0.y - pipes[i].p1.y));
                float newR = (float)(pipes[i].width / 1000d / 2.0d);
                gltf.Nodes[i] = new Node()
                {
                    Mesh = 0,
                    Name = "idd:" + pipes[i].id,
                    Translation = new float[3] { pipes[i].p0.x - originX, 0, pipes[i].p0.y - originY },
                    Rotation = new float[4] { rotationQuaternion.X, rotationQuaternion.Y, rotationQuaternion.Z, rotationQuaternion.W },
                    Scale = new float[3] { len, newR, newR }
                }; 
            }
            
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2, Material = 0 } } } };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePoint(p0, r, radian * i));
            Point3Df newP0 = new Point3Df(p0.x - h, p0.y, p0.z);
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePoint(newP0, r, radian * i));
            int count = points.Count;
            for (int i = 0; i < count; i++)
                points.Add(new Point3Df(points[i].x, points[i].y, points[i].z));

            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(1, 0, 0));
            for (int i = 0; i < n; i++)
            {
                Point3Df p = GetCesiumPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
                Coordinate c = new Coordinate(p.x, p.y, p.z);
                VectorMath.Normalize(c);
                p.x = (float)c.X;
                p.y = (float)c.Y;
                p.z = (float)c.Z;
                normals.Add(p);
            }
            for (int i = 0; i < n; i++)
            {
                var p = GetCesiumPipePoint(new Point3Df(0, 0, 0), 1, radian * i);
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
                indices.AddRange(new ushort[3] { n, (ushort)(i + 1), i });
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
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = GetMax(points), Min = GetMin(points) },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = GetMax(normals), Min = GetMin(normals) },
                new Accessor() { BufferView = 2, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 4 * n - 1 }, Min = new float[1] { 0 } }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 1f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f} } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\CesiumPipes10zyReverse.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        private static Point3Df GetCesiumPipePoint(Point3Df p0, float r, double radian)
        {
            float x = p0.x;
            float z = p0.y + (float)(Math.Sin(radian) * r);
            float y = p0.z + (float)(Math.Cos(radian) * r);
            return new Point3Df(x, y, z);
        }*/

        /// <summary>
        /// 创建圆柱体
        /// </summary>
        /// <param name="p0">圆心坐标</param>
        /// <param name="r">圆的半径</param>
        /// <param name="h">圆柱体的高度</param>
        /// <param name="n">圆的顶点个数</param>
        /*public static void CreateCesiumPipes3DTileGltf(List<Pipe> pipes)
        {
            const ushort n = 16;
            //float r = pipes[0].width / 1000f / 2.0f;
            //float h = (float)Math.Sqrt((pipes[0].c0.X - pipes[0].c1.X) * (pipes[0].c0.X - pipes[0].c1.X) +
            //    (pipes[0].c0.Y - pipes[0].c1.Y) * (pipes[0].c0.Y - pipes[0].c1.Y));
            //r = r * 20;
            //h = h;
            const float r = 10;
            const float h = 1;
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            int pipeCount = 10;
            for (int i = 0; i < pipeCount; i++)
            {
                var pipe = pipes[i];
                pipe.p0 = new Point3Df((float)pipe.c0.X, (float)pipe.c0.Y, 0);
                pipe.p1 = new Point3Df((float)pipe.c1.X, (float)pipe.c1.Y, 0);
                Console.WriteLine("i:" + i + ",p0:" + pipe.p0 + ",p1:" + pipe.p1);
               // if (i < 1)
                {
                    if (pipe.p0.x < minX)
                        minX = pipe.p0.x;
                    if (pipe.p0.y < minY)
                        minY = pipe.p0.y;
                    if (pipe.p1.x < minX)
                        minX = pipe.p1.x;
                    if (pipe.p1.y < minY)
                        minY = pipe.p1.y;

                    if (pipe.p0.x > maxX)
                        maxX = pipe.p0.x;
                    if (pipe.p0.y > maxY)
                        maxY = pipe.p0.y;
                    if (pipe.p1.x > maxX)
                        maxX = pipe.p1.x;
                    if (pipe.p1.y > maxY)
                        maxY = pipe.p1.y;
                }
            }
            float originX = (minX + maxX) / 2.0f;
            float originY = (minY + maxY) / 2.0f;
            //float originX = pipes[0].p0.x;
            //float originY = pipes[0].p0.y;
            Console.WriteLine("originX:" + originX.ToString("G9") + ",originY:" + originY.ToString("G9"));
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
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
                Quaternion rotationQuaternion = Quaternion.CreateFromRotationMatrix(rotationMatrix);
                float len = (float)Math.Sqrt((pipes[i].p0.x - pipes[i].p1.x) * (pipes[i].p0.x - pipes[i].p1.x) +
                    (pipes[i].p0.y - pipes[i].p1.y) * (pipes[i].p0.y - pipes[i].p1.y));
                float newR = (float)(pipes[i].width / 1000d / 2.0d);
                gltf.Nodes[i] = new Node()
                {
                    Mesh = 0,
                    Name = "idd:" + pipes[i].id,
                    //Translation = new float[3] { pipes[i].p0.x - originX, pipes[i].p0.y - originY, 0 },
                    Translation = new float[3] { pipes[i].p0.x - originX, 0, pipes[i].p0.y - originY },
                    Rotation = new float[4] { rotationQuaternion.X, rotationQuaternion.Y, rotationQuaternion.Z, rotationQuaternion.W },
                    Scale = new float[3] { len, newR, newR }
                }; 
            }
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
                normals.Add(new Point3Df(-1, 0, 0));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(1, 0, 0));
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
                indices.AddRange(new ushort[3] { 0, i, (ushort)(i + 1)  });
            for (ushort i = (ushort)(n + 1); i < n + n - 1; i++)
                indices.AddRange(new ushort[3] { n, (ushort)(i + 1), i });
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
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = GetMax(points), Min = GetMin(points) },
                new Accessor() { BufferView = 1, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.FLOAT,
                Count = points.Count, Type = Accessor.TypeEnum.VEC3, Max = GetMax(normals), Min = GetMin(normals) },
                new Accessor() { BufferView = 2, ByteOffset = 0, ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT,
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 4 * n - 1 }, Min = new float[1] { 0 } }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 1f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f} } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\CesiumPipes103DTileSolidh1WithTReversezyRy.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }*/

        /// <summary>
        /// 创建圆柱体 // x轴向北,y轴向天,z轴向东
        /// </summary>
        /// <param name="p0">圆心坐标</param>
        /// <param name="r">圆的半径</param>
        /// <param name="h">圆柱体的高度</param>
        /// <param name="n">圆的顶点个数</param>
        public static void CreateCesiumPipesGltf(List<Pipe> pipes)
        {
            const ushort n = 16;
            //float r = pipes[0].width / 1000f / 2.0f;
            //float h = (float)Math.Sqrt((pipes[0].c0.X - pipes[0].c1.X) * (pipes[0].c0.X - pipes[0].c1.X) +
            //    (pipes[0].c0.Y - pipes[0].c1.Y) * (pipes[0].c0.Y - pipes[0].c1.Y));
            //r = r * 20;
            //h = h;
            const float r = 10;
            const float h = 1;
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            int pipeCount = pipes.Count;
            int originPipeCount = pipeCount;
            for (int i = 0; i < pipeCount; i++)
            {
                var pipe = pipes[i];
               // pipe.p0 = new Point3Df((float)pipe.c0.X, (float)pipe.c0.Y, 0);
               // pipe.p1 = new Point3Df((float)pipe.c1.X, (float)pipe.c1.Y, 0);
                /*Console.WriteLine("i:" + i + ",p0:" + pipe.p0 + ",p1:" + pipe.p1);
               // if (i < 1)
                {
                    if (pipe.p0.x < minX)
                        minX = pipe.p0.x;
                    if (pipe.p0.y < minY)
                        minY = pipe.p0.y;
                    if (pipe.p1.x < minX)
                        minX = pipe.p1.x;
                    if (pipe.p1.y < minY)
                        minY = pipe.p1.y;

                    if (pipe.p0.x > maxX)
                        maxX = pipe.p0.x;
                    if (pipe.p0.y > maxY)
                        maxY = pipe.p0.y;
                    if (pipe.p1.x > maxX)
                        maxX = pipe.p1.x;
                    if (pipe.p1.y > maxY)
                        maxY = pipe.p1.y;
                }*/
                GeoAPI.Geometries.Envelope envelope = pipe.GetExtent();
                if (envelope.MinX < minX)
                    minX = (float)envelope.MinX;
                if (envelope.MinY < minY)
                    minY = (float)envelope.MinY;
                if (envelope.MaxX > maxX)
                    maxX = (float)envelope.MaxX;
                if (envelope.MaxY > maxY)
                    maxY = (float)envelope.MaxY;
            }
            float originX = (minX + maxX) / 2.0f;
            float originY = (minY + maxY) / 2.0f;
            var mapPoint = new MapPoint(originX, originY, new SpatialReference(3857));
            mapPoint = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mapPoint, new SpatialReference(4326));
            Console.WriteLine("originX:" + mapPoint.X.ToString("G9") + ",originY:" + mapPoint.Y.ToString("G9"));
            CoordinateChangeUtils.GetCesiumCoord(pipes, pipeCount, new Coordinate(originX, originY));
            for (int i = pipeCount - 1; i >= 0; i--)
            {
                if (pipes[i].p0.x == pipes[i].p1.x && pipes[i].p0.y == pipes[i].p1.y || pipes[i].width == 0)
                {
                    pipes.RemoveAt(i);
                    pipeCount--;
                }
            }
            //Point3Df p0 = new Point3Df(gd3dCoord.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord.y, r); // 84543000, 17237000 / 84542952, 17236860
            Point3Df p0 = new Point3Df(0, 0, 0);
            Point3Df newP0 = new Point3Df(p0.x, p0.y, p0.z + h);
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
                float len = (float)Math.Sqrt((pipes[i].p0.x - pipes[i].p1.x) * (pipes[i].p0.x - pipes[i].p1.x) +
                    (pipes[i].p0.y - pipes[i].p1.y) * (pipes[i].p0.y - pipes[i].p1.y));
                float newR = (float)(pipes[i].width / 1000d / 2.0d);
                Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(pipes[i].p0.y - originY, 0, pipes[i].p0.x - originX);
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
                Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(newR, newR, len);
                Matrix4x4 totalMatrix = Matrix4x4.Multiply(Matrix4x4.Multiply(scaleMatrix, rotationMatrix), translationMatrix);

                if (totalMatrix.GetDeterminant() == 0)
                    throw new Exception("Determinant = 0");

                float[] matrix = new float[16] { totalMatrix.M11, totalMatrix.M12, totalMatrix.M13, totalMatrix.M14,
                    totalMatrix.M21, totalMatrix.M22, totalMatrix.M23, totalMatrix.M24,
                    totalMatrix.M31, totalMatrix.M32, totalMatrix.M33, totalMatrix.M34,
                    totalMatrix.M41, totalMatrix.M42, totalMatrix.M43, totalMatrix.M44};
                //Quaternion rotationQuaternion = Quaternion.CreateFromRotationMatrix(rotationMatrix);
                gltf.Nodes[i] = new Node()
                {
                    Mesh = 0,
                    Name = "idd:" + pipes[i].id,
                    Matrix = matrix
                  //  Translation = new float[3] { pipes[i].p0.y - originY, 0, pipes[i].p0.x - originX },
                    //Rotation = new float[4] { rotationQuaternion.X, rotationQuaternion.Y, rotationQuaternion.Z, rotationQuaternion.W },
                   // Scale = new float[3] { newR, newR, len }
                }; 
            }
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            gltf.Meshes = new Mesh[1] { new Mesh() { Primitives = new MeshPrimitive[1] { new MeshPrimitive() { Attributes = attributes, Indices = 2, Material = 0 } } } };
            double radian = 2 * Math.PI / n;
            List<Point3Df> points = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePointGltf(p0, r, radian * i));
            for (int i = 0; i < n; i++)
                points.Add(GetCesiumPipePointGltf(newP0, r, radian * i));
            int count = points.Count;
            for (int i = 0; i < count; i++)
                points.Add(new Point3Df(points[i].x, points[i].y, points[i].z));

            List<byte> list = new List<byte>();
            foreach (var p in points)
                list.AddRange(p.ToByteList());
            List<Point3Df> normals = new List<Point3Df>();
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(0, 0, -1));
            for (int i = 0; i < n; i++)
                normals.Add(new Point3Df(0, 0, 1));
            for (int i = 0; i < n; i++)
            {
                Point3Df p = GetCesiumPipePointGltf(new Point3Df(0, 0, 0), 1, radian * i);
                MakerUtils.Normalize(p);
                //Coordinate c = new Coordinate(p.x, p.y, p.z);
                //VectorMath.Normalize(c);
                //p.x = (float)c.X;
                //p.y = (float)c.Y;
                //p.z = (float)c.Z;
                normals.Add(p);
            }
            for (int i = 0; i < n; i++)
            {
                var p = GetCesiumPipePointGltf(new Point3Df(0, 0, 0), 1, radian * i);
                //Coordinate c = new Coordinate(p.x, p.y, p.z);
                MakerUtils.Normalize(p);
                //p.x = (float)c.X;
                //p.y = (float)c.Y;
                //p.z = (float)c.Z;
                normals.Add(p);
            }
            foreach (var p in normals)
                list.AddRange(p.ToByteList());

            List<ushort> indices = new List<ushort>();
            for (ushort i = 1; i < n - 1; i++)
                indices.AddRange(new ushort[3] { 0, (ushort)(i + 1), i });
            for (ushort i = (ushort)(n + 1); i < n + n - 1; i++)
                indices.AddRange(new ushort[3] { n, i, (ushort)(i + 1) });
            for (ushort i = 0; i < n - 1; i++)
            {
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(3 * n + 1 + i), (ushort)(3 * n + i) });
                indices.AddRange(new ushort[3] { (ushort)(2 * n + i), (ushort)(2 * n + 1 + i), (ushort)(3 * n + 1 + i) });
            }
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(3 * n), (ushort)(3 * n + n - 1) });
            indices.AddRange(new ushort[3] { (ushort)(2 * n + n - 1), (ushort)(2 * n), (ushort)(3 * n) });
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
                Count = indices.Count, Type = Accessor.TypeEnum.SCALAR, Max = new float[1] { 4 * n - 1 }, Min = new float[1] { 0 } }};
            gltf.Materials = new Material[1] { new Material() { PbrMetallicRoughness = new MaterialPbrMetallicRoughness() {
                BaseColorFactor = new float[4] { 1.000f, 0f, 0f, 0.8f }, MetallicFactor = 0.5f, RoughnessFactor = 0.1f}, AlphaMode = Material.AlphaModeEnum.BLEND } }; // AlphaMode = Material.AlphaModeEnum.BLEND
            //gltf.Cameras = new Camera[1] { new Camera() { Type = Camera.TypeEnum.perspective

            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\CesiumPipes" + originPipeCount + "zxy.gltf";
            glTFLoader.Interface.SaveModel(gltf, path);
        }

        private static Point3Df GetCesiumPipePointGltf(Point3Df p0, float r, double radian)
        {
            float x = p0.x + (float)(Math.Cos(radian) * r);
            float y = p0.y + (float)(Math.Sin(radian) * r);
            float z = p0.z;  
            return new Point3Df(x, y, z);
        }

        private static Point3Df GetCesium3DTilePipePoint(Point3Df p0, float r, double radian)
        {
            float x = p0.x;
            float y = p0.y + (float)(Math.Cos(radian) * r);
            float z = p0.z + (float)(Math.Sin(radian) * r);
            return new Point3Df(x, y, z);
        }
    }
}
