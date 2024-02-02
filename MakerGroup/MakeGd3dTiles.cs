using Esri.ArcGISRuntime.Geometry;
using GeoAPI.Geometries;
using gltfMaker.Models;
using Newtonsoft.Json;
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
        const string GD3dTilesFolderName = "Gd3dTiles";
        private static string GD3dTilesFolder = "";
        private static Dictionary<int, int> zoomCountDict = new Dictionary<int, int>();
        public static void MakeGd3dTiles(List<Pipe> pipes)
        {
            double[] resolutions = new double[21];
            for (int i = 0; i < resolutions.Length; i++)
                resolutions[i] = 0;
            //resolutions[12] = 38.21851414257816;
            resolutions[13] = 19.10925707128908;
            resolutions[14] = 9.55462853564454;
            resolutions[15] = 4.77731426782227;
            resolutions[16] = 2.388657133911135;
            resolutions[17] = 1.1943285669555674;
            resolutions[18] = 0.5971642834777837;
            resolutions[19] = 0.29858214173889186;
            resolutions[20] = 0.14929107086944593;
            ProcessPipes(pipes);
            /*for (int i = 12; i <= 20; i++)
            {
                var newPipes = GetPipes(pipes, resolutions[i]);
                Console.WriteLine("i:" + i + ",count:" + newPipes.Count);
                if (newPipes.Count == 0)
                    continue;
            }
            return;*/
            /*var newPipes = GetPipes(pipes, resolutions[16]);
            TilesetSchema tilesetSchema = new TilesetSchema();
            tilesetSchema.zoom = 16;
            tilesetSchema.region = GetRegion(newPipes);
            string folder = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + GD3dTilesFolderName; 
            string gltfName = tilesetSchema.zoom + ".gltf";
            string path = folder + "\\" + gltfName;
            gltfCreator.CreateGdPipesGltf(newPipes, path);
            tilesetSchema.gltfUri = GD3dTilesFolderName + "/" + gltfName;*/
            for (int i = 13; i <= 20; i++)
                zoomCountDict.Add(i, 0);
            //string dir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + GD3dTilesFolderName;
            GD3dTilesFolder = @"D:\Workplace\HTML\GdMap3dTile\" + GD3dTilesFolderName;
            if (Directory.Exists(GD3dTilesFolder))
                Directory.Delete(GD3dTilesFolder, true);
            while (Directory.Exists(GD3dTilesFolder)) ;
            Directory.CreateDirectory(GD3dTilesFolder);

            int zoom = 13;
            TilesetSchema schema = CreateSchema(pipes, zoom, resolutions, zoom.ToString());
            CreateChildren(pipes, zoom + 1, resolutions, schema, zoom.ToString()); 
            string path = GD3dTilesFolder + "\\" + zoom.ToString() + ".json";
            SaveSchema(schema, path);
            for (int i = 13; i <= 20; i++)
                Console.WriteLine(zoomCountDict[i]);
        }

        private static void CreateChildren(List<Pipe> pipes, int zoom, double[] resolutions, TilesetSchema parentSchema, string parentName)
        {
            List<List<Pipe>> fourPipes = SplitPipes(pipes);
            int index = 0;
            List<string> childrenUri = new List<string>();
            foreach (var newPipes in fourPipes)
            {
                if (newPipes.Count == 0)
                    continue;

                string name = parentName + "_" + index;
                TilesetSchema schema = CreateSchema(newPipes, zoom, resolutions, name);
                childrenUri.Add(GD3dTilesFolderName + "/" + name + ".json");
                index++;
                if (zoom < 20)
                {
                    CreateChildren(newPipes, zoom + 1, resolutions, schema, name);
                }
                else
                    schema.childrenUri = new string[0];
                string path = GD3dTilesFolder + "\\" + name + ".json";
                SaveSchema(schema, path);
            }
            parentSchema.childrenUri = childrenUri.ToArray();
        }

        private static void SaveSchema(TilesetSchema schema, string path)
        {
            string json = JsonConvert.SerializeObject(schema);
            File.WriteAllText(path, json);
        }

        private static TilesetSchema CreateSchema(List<Pipe> pipes, int zoom, double[] resolutions, string name)
        {
            zoomCountDict[zoom] += pipes.Count;
            var newPipes = GetPipes(pipes, resolutions[zoom]);
            TilesetSchema tilesetSchema = new TilesetSchema();
            tilesetSchema.zoom = zoom;
            tilesetSchema.region = GetRegion(pipes);
            if (newPipes.Count > 0)
            {
               // string folder = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + GD3dTilesFolderName;
                string gltfName = name + ".gltf";
                string path = GD3dTilesFolder + "\\" + gltfName;
                gltfCreator.CreateGdPipesGltf(newPipes, path);
                tilesetSchema.gltfUri = GD3dTilesFolderName + "/" + gltfName;
            }
            else
                tilesetSchema.gltfUri = "";
            return tilesetSchema;
        }

        private static List<List<Pipe>> SplitPipes(List<Pipe> pipes)
        {
            List<List<Pipe>> fourPipes = SplitFourPipes(pipes);
            //return fourPipes;
            List<List<Pipe>> result = new List<List<Pipe>>();
            foreach (var pipes0 in fourPipes)
            {
                result.AddRange(SplitFourPipes(pipes0));
            }
            return result;
        }

        private static List<List<Pipe>> SplitFourPipes(List<Pipe> pipes)
        {
            double[] region = GetRegion(pipes);
            double centerLon = (region[0] + region[2]) / 2.0d;
            double centerLat = (region[1] + region[3]) / 2.0d;
            List<Pipe> leftUpPipes = new List<Pipe>();
            List<Pipe> rightUpPipes = new List<Pipe>();
            List<Pipe> leftDownPipes = new List<Pipe>();
            List<Pipe> rightDownPipes = new List<Pipe>();
            foreach (var pipe in pipes)
            {
                double lon = (pipe.lonLat0.X + pipe.lonLat1.X) / 2.0d;
                double lat = (pipe.lonLat0.Y + pipe.lonLat1.Y) / 2.0d;
                if (lon < centerLon && lat < centerLat)
                    leftDownPipes.Add(pipe);
                else if (lat < centerLat)
                    rightDownPipes.Add(pipe);
                else if (lon < centerLon)
                    leftUpPipes.Add(pipe);
                else
                    rightUpPipes.Add(pipe);
            }
            return new List<List<Pipe>>() { leftUpPipes, rightUpPipes, leftDownPipes, rightDownPipes };
        }

        private static void ProcessPipes(List<Pipe> pipes)
        {
            var p0Gd3dCoord = CoordinateChangeUtils.GetGd3dCoord(new Coordinate(0, 0, 0));
            foreach (var pipe in pipes)
            {
                var point = new MapPoint(pipe.c0.X, pipe.c0.Y, new SpatialReference(3857));
                point = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(point, new SpatialReference(4326));
                pipe.lonLat0 = new Coordinate(Math.Round(point.X, 5), Math.Round(point.Y, 5));

                point = new MapPoint(pipe.c1.X, pipe.c1.Y, new SpatialReference(3857));
                point = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(point, new SpatialReference(4326));
                pipe.lonLat1 = new Coordinate(Math.Round(point.X, 5), Math.Round(point.Y, 5));

                var gd3dCoord0 = CoordinateChangeUtils.GetGd3dCoord(pipe.c0);
                pipe.p0 = new Point3Df(gd3dCoord0.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord0.y, 0);

                var gd3dCoord1 = CoordinateChangeUtils.GetGd3dCoord(pipe.c1);
                pipe.p1 = new Point3Df(gd3dCoord1.x - p0Gd3dCoord.x, p0Gd3dCoord.y - gd3dCoord1.y, 0);
            }
        }

        private static List<Pipe> GetPipes(List<Pipe> pipes, double resolution)
        {
            if (resolution == 0.14929107086944593)
                return pipes;
            else
            {
                int b = 10;
                while (true)
                {
                    List<Pipe> r = new List<Pipe>();
                    foreach (var pipe in pipes)
                    {
                        double d = pipe.width / 1000d * b / resolution;
                        if (d >= 1)
                            r.Add(pipe);
                    }
                    if (r.Count < 1000)
                        return r;
                    else
                        b--;
                }
            }
        }

        private static double[] GetRegion(List<Pipe> pipes)
        {
            double xmin = double.MaxValue, ymin = double.MaxValue, xmax = double.MinValue, ymax = double.MinValue;
            foreach (var pipe in pipes)
            {
                if (pipe.lonLat0.X < xmin)
                    xmin = pipe.lonLat0.X;
                if (pipe.lonLat0.Y < ymin)
                    ymin = pipe.lonLat0.Y;
                if (pipe.lonLat0.X > xmax)
                    xmax = pipe.lonLat0.X;
                if (pipe.lonLat0.Y > ymax)
                    ymax = pipe.lonLat0.Y;

                if (pipe.lonLat1.X < xmin)
                    xmin = pipe.lonLat1.X;
                if (pipe.lonLat1.Y < ymin)
                    ymin = pipe.lonLat1.Y;
                if (pipe.lonLat1.X > xmax)
                    xmax = pipe.lonLat1.X;
                if (pipe.lonLat1.Y > ymax)
                    ymax = pipe.lonLat1.Y;
            }
            return new double[4] { xmin, ymin, xmax, ymax };
        }
    }
}
