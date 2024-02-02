using Esri.ArcGISRuntime.Geometry;
using GeoAPI.Geometries;
using gltfMaker.Models;
using NetTopologySuite;
using Newtonsoft.Json;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Data.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker
{
    static class ShapeFileManager
    {
        public static List<Pipe> LoadPipeFromShapeFile(string fileName)
        {
            var gss = new NtsGeometryServices();
            var css = new SharpMap.CoordinateSystems.CoordinateSystemServices(
                new CoordinateSystemFactory(),
                new CoordinateTransformationFactory(),
                SharpMap.Converters.WellKnownText.SpatialReference.GetAllReferenceSystems());

            //GeoAPI.GeometryServiceProvider.Instance = gss;
            SharpMap.Session.Instance
                .SetGeometryServices(gss)
                .SetCoordinateSystemServices(css)
                .SetCoordinateSystemRepository(css);

            List<Pipe> pipes = new List<Pipe>();
            ShapeFile shapeFile = new ShapeFile(fileName);
            shapeFile.Open();
            try
            {
                int count = shapeFile.GetFeatureCount();
                Console.WriteLine("featureCount:" + count);
                Stopwatch stopwatch = new Stopwatch();
                /*
                stopwatch.Start();
                var objectlist = shapeFile.GetObjectIDsInView(shapeFile.GetExtents());
                foreach (var oid in objectlist)
                {
                    FeatureDataRow feature = shapeFile.GetFeature(oid);
                }
                stopwatch.Stop();
                Console.WriteLine("objectlist.Count:" + objectlist.Count + ";time:" + stopwatch.ElapsedMilliseconds + "ms");*/
                stopwatch.Restart();
                shapeFile.DoTrueIntersectionQuery = false;
                var fds = new SharpMap.Data.FeatureDataSet();
                shapeFile.ExecuteIntersectionQuery(shapeFile.GetExtents(), fds);
                FeatureDataTable featureDataTable = fds.Tables[0];
                for (int i = 0; i < featureDataTable.Count; i++)
                {
                    FeatureDataRow feature = featureDataTable[i];
                    if (feature.Geometry.OgcGeometryType == GeoAPI.Geometries.OgcGeometryType.LineString)
                    {
                        NetTopologySuite.Geometries.LineString lineString = feature.Geometry as NetTopologySuite.Geometries.LineString;
                        string id = (string)feature[0]; // ["ID"];
                        float width = (float)feature[29]; // ["WIDTH"];
                        string systemType = (string)feature[3];
                        float us_invert_level = (float)feature[6];
                        float ds_invert_level = (float)feature[8];
                        string systemTypeCode;
                        if (systemType.Equals("雨水"))
                            systemTypeCode = "YS";
                        else if (systemType.Equals("污水"))
                            systemTypeCode = "WS";
                        else if (systemType.Equals("合流"))
                            systemTypeCode = "HS";
                        else
                            systemTypeCode = "HS";
                        Coordinate c0 = new Coordinate(lineString[0].X, lineString[0].Y, 0);
                        Coordinate c1 = new Coordinate(lineString[1].X, lineString[1].Y, 0);
                        if (c0.X != c1.X || c0.Y != c1.Y)
                            pipes.Add(new Pipe() { id = id, width = width, c0 = c0, c1 = c1, systemTypeCode = systemTypeCode, us_invert_level = us_invert_level,
                                ds_invert_level = ds_invert_level});
                    }
                }
                SavePipesJson(pipes);
                stopwatch.Stop();
                Console.WriteLine("rowCount:" + fds.Tables[0].Count + ";time:" + stopwatch.ElapsedMilliseconds + "ms");
            }
            finally
            {
                shapeFile.Close();
            }
            return pipes;
        }

        private static void SavePipesJson(List<Pipe> pipes)
        {
            PipeJson[] list = new PipeJson[pipes.Count];
            for (int i = 0; i < pipes.Count; i++)
            {
                var pipe = pipes[i];
                list[i] = new PipeJson()
                {
                    a = new PipeJsonPoint() { x = Math.Round(pipe.c0.X, 3), y = Math.Round(pipe.c0.Y, 3), h = pipe.us_invert_level },
                    b = new PipeJsonPoint() { x = Math.Round(pipe.c1.X, 3), y = Math.Round(pipe.c1.Y, 3), h = pipe.ds_invert_level },
                    id = pipe.id,
                    w = pipe.width,
                    t = pipe.systemTypeCode
                };
            }
            string json = JsonConvert.SerializeObject(list);
            string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\pipes.json";
            File.WriteAllText(path, json);
        }

        public static List<Comb> LoadCombFromShapeFile(string fileName)
        {
            var gss = new NtsGeometryServices();
            var css = new SharpMap.CoordinateSystems.CoordinateSystemServices(
                new CoordinateSystemFactory(),
                new CoordinateTransformationFactory(),
                SharpMap.Converters.WellKnownText.SpatialReference.GetAllReferenceSystems());

            //GeoAPI.GeometryServiceProvider.Instance = gss;
            SharpMap.Session.Instance
                .SetGeometryServices(gss)
                .SetCoordinateSystemServices(css)
                .SetCoordinateSystemRepository(css);

            List<Comb> combs = new List<Comb>();
            ShapeFile shapeFile = new ShapeFile(fileName);
            shapeFile.Open();
            try
            {
                int count = shapeFile.GetFeatureCount();
                Console.WriteLine("featureCount:" + count);
                Stopwatch stopwatch = new Stopwatch();
                /*
                stopwatch.Start();
                var objectlist = shapeFile.GetObjectIDsInView(shapeFile.GetExtents());
                foreach (var oid in objectlist)
                {
                    FeatureDataRow feature = shapeFile.GetFeature(oid);
                }
                stopwatch.Stop();
                Console.WriteLine("objectlist.Count:" + objectlist.Count + ";time:" + stopwatch.ElapsedMilliseconds + "ms");*/
                stopwatch.Restart();
                shapeFile.DoTrueIntersectionQuery = false;
                var fds = new SharpMap.Data.FeatureDataSet();
                shapeFile.ExecuteIntersectionQuery(shapeFile.GetExtents(), fds);
                FeatureDataTable featureDataTable = fds.Tables[0];
                for (int i = 0; i < featureDataTable.Count; i++)
                {
                    FeatureDataRow feature = featureDataTable[i];
                    if (feature.Geometry.OgcGeometryType == GeoAPI.Geometries.OgcGeometryType.Point)
                    {
                        NetTopologySuite.Geometries.Point point = feature.Geometry as NetTopologySuite.Geometries.Point;
                        try
                        {
                            string id = (string)feature[0]; // ["ID"];
                            //float co_x = (float)feature[1]; // ["CO_X"];
                            //float co_y = (float)feature[2]; // CO_Y
                            float ground_lev = (float)feature[3]; // ground_lev
                            float invert_lev = (float)feature[4]; // invert_lev
                            if (invert_lev > ground_lev)
                                throw new Exception("error");
                            string comb_size = (string)feature[25]; // comb_size
                            if (comb_size.Length == 0)
                                comb_size = "750X450";
                            string[] sizes = comb_size.Split(new char[] { 'X' });
                            float xl = float.Parse(sizes[0]);
                            float yl;
                            if (sizes.Length > 1)
                                yl = float.Parse(sizes[1]);
                            else
                                yl = xl;
                            if (xl <= 0 || yl <= 0)
                                throw new Exception("error");
                            if (ground_lev > invert_lev)
                                combs.Add(new Comb() { id = id, x = point.X, y = point.Y, gl = ground_lev, il = invert_lev, xl = xl, yl = yl });
                          //  Console.WriteLine(comb_size);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                       /* Coordinate c0 = new Coordinate(lineString[0].X, lineString[0].Y, 0);
                        Coordinate c1 = new Coordinate(lineString[1].X, lineString[1].Y, 0);
                        if (c0.X != c1.X || c0.Y != c1.Y)
                            pipes.Add(new Pipe() { id = id, width = width, c0 = c0, c1 = c1, systemTypeCode = systemTypeCode, us_invert_level = us_invert_level,
                                ds_invert_level = ds_invert_level});*/
                    }
                }
                string json = JsonConvert.SerializeObject(combs);
                string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\combs.json";
                File.WriteAllText(path, json);
                stopwatch.Stop();
                Console.WriteLine("rowCount:" + fds.Tables[0].Count + ";time:" + stopwatch.ElapsedMilliseconds + "ms");
            }
            finally
            {
                shapeFile.Close();
            }
            return combs;
        }
    }

    class PipeJsonPoint
    {
        public double x { get; set; }
        public double y { get; set; }
        public float h { get; set; }
    }

    class PipeJson
    {
        public PipeJsonPoint a { get; set; }
        public PipeJsonPoint b { get; set; }
        public string id { get; set; }
        public float w { get; set; }

        public string t { get; set; }
    }

    class Pipe
    {
        /// <summary>
        /// 原始3857坐标
        /// </summary>
        public Coordinate c0;
        public Coordinate c1;
        /// <summary>
        /// gltf用坐标
        /// </summary>
        public Point3Df p0;
        public Point3Df p1;
        /// <summary>
        /// 高德经纬度坐标
        /// </summary>
        public Coordinate lonLat0;
        public Coordinate lonLat1;
        public string id;
        public float width;
        /// <summary>
        /// YS,WS,HS
        /// </summary>
        public string systemTypeCode;
        public float us_invert_level;
        public float ds_invert_level;

        /*public Envelope GetExtent()
        {
            double r = width / 1000d / 2.0d * 10d;
            double len = Math.Sqrt((p0.x - p1.x) * (p0.x - p1.x) +
                    (p0.y - p1.y) * (p0.y - p1.y));
            double angle = Math.Atan2(p1.y - p0.y, p1.x - p0.x);
            double leftFaceAngle1 = angle + Math.PI / 2.0d;
            double leftFaceX1 = p0.x + r * Math.Cos(leftFaceAngle1);
            double leftFaceY1 = p0.y + r * Math.Sin(leftFaceAngle1);
            double leftFaceAngle2 = angle + Math.PI / 2.0d + Math.PI;
            double leftFaceX2 = p0.x + r * Math.Cos(leftFaceAngle2);
            double leftFaceY2 = p0.y + r * Math.Sin(leftFaceAngle2);

            double rightFaceAngle1 = angle + Math.PI / 2.0d;
            double rightFaceX1 = p1.x + r * Math.Cos(rightFaceAngle1);
            double rightFaceY1 = p1.y + r * Math.Sin(rightFaceAngle1);
            double rightFaceAngle2 = angle + Math.PI / 2.0d + Math.PI;
            double rightFaceX2 = p1.x + r * Math.Cos(rightFaceAngle2);
            double rightFaceY2 = p1.y + r * Math.Sin(rightFaceAngle2);
            double xmin = Math.Min(Math.Min(leftFaceX1, leftFaceX2), Math.Min(rightFaceX1, rightFaceX2));
            double ymin = Math.Min(Math.Min(leftFaceY1, leftFaceY2), Math.Min(rightFaceY1, rightFaceY2));
            double xmax = Math.Max(Math.Max(leftFaceX1, leftFaceX2), Math.Max(rightFaceX1, rightFaceX2));
            double ymax = Math.Max(Math.Max(leftFaceY1, leftFaceY2), Math.Max(rightFaceY1, rightFaceY2));
            return new Envelope(xmin, xmax, ymin, ymax);
        }*/

        /*public GeoAPI.Geometries.Envelope GetExtent()
        {
            double k = 1.088; // 10米以上的数不建议用这值
            double r = width / 1000d / 2.0d * 10d * k;
            double len = Math.Sqrt((c0.X - c1.X) * (c0.X - c1.X) +
                    (c0.Y - c1.Y) * (c0.Y - c1.Y));
            double angle = Math.Atan2(c1.Y - c0.Y, c1.X - c0.X);
            double leftFaceAngle1 = angle + Math.PI / 2.0d;
            double leftFaceX1 = c0.X + r * Math.Cos(leftFaceAngle1);
            double leftFaceY1 = c0.Y + r * Math.Sin(leftFaceAngle1);
            //Coordinate leftFace1 = GetNewPoint(c0, leftFaceAngle1, r);
            double leftFaceAngle2 = angle + Math.PI / 2.0d + Math.PI;
            double leftFaceX2 = c0.X + r * Math.Cos(leftFaceAngle2);
            double leftFaceY2 = c0.Y + r * Math.Sin(leftFaceAngle2);
            //Coordinate leftFace2 = GetNewPoint(c0, leftFaceAngle2, r);

            double rightFaceAngle1 = angle + Math.PI / 2.0d;
            double rightFaceX1 = c1.X + r * Math.Cos(rightFaceAngle1);
            double rightFaceY1 = c1.Y + r * Math.Sin(rightFaceAngle1);
            //Coordinate rightFace1 = GetNewPoint(c1, rightFaceAngle1, r);
            double rightFaceAngle2 = angle + Math.PI / 2.0d + Math.PI;
            double rightFaceX2 = c1.X + r * Math.Cos(rightFaceAngle2);
            double rightFaceY2 = c1.Y + r * Math.Sin(rightFaceAngle2);
            //Coordinate rightFace2 = GetNewPoint(c1, rightFaceAngle2, r);
            double xmin = Math.Min(Math.Min(leftFaceX1, leftFaceX2), Math.Min(rightFaceX1, rightFaceX2));
            //double xmin = Math.Min(Math.Min(leftFace1.X, leftFace2.X), Math.Min(rightFace1.X, rightFace2.X));
            double ymin = Math.Min(Math.Min(leftFaceY1, leftFaceY2), Math.Min(rightFaceY1, rightFaceY2));
            //double ymin = Math.Min(Math.Min(leftFace1.Y, leftFace2.Y), Math.Min(rightFace1.Y, rightFace2.Y));
            double xmax = Math.Max(Math.Max(leftFaceX1, leftFaceX2), Math.Max(rightFaceX1, rightFaceX2));
            //double xmax = Math.Max(Math.Max(leftFace1.X, leftFace2.X), Math.Max(rightFace1.X, rightFace2.X));
            double ymax = Math.Max(Math.Max(leftFaceY1, leftFaceY2), Math.Max(rightFaceY1, rightFaceY2));
            //double ymax = Math.Max(Math.Max(leftFace1.Y, leftFace2.Y), Math.Max(rightFace1.Y, rightFace2.Y));
            return new GeoAPI.Geometries.Envelope(xmin, xmax, ymin, ymax);
        }*/

        public GeoAPI.Geometries.Envelope GetExtent()
        {
            //double k = 1.088; // 10米以上的数不建议用这值
            double r = width / 1000d / 2.0d * 10d; //*k
            //double len = Math.Sqrt((c0.X - c1.X) * (c0.X - c1.X) +
              //      (c0.Y - c1.Y) * (c0.Y - c1.Y));
            double angle = Math.Atan2(c1.Y - c0.Y, c1.X - c0.X);
            angle = Math.PI / 2.0d - angle;
            double leftFaceAngle1 = angle + Math.PI / 2.0d;
            //double leftFaceX1 = c0.X + r * Math.Cos(leftFaceAngle1);
            //double leftFaceY1 = c0.Y + r * Math.Sin(leftFaceAngle1);
            Coordinate leftFace1 = GetNewPoint(c0, leftFaceAngle1, r);
            double leftFaceAngle2 = angle + Math.PI / 2.0d + Math.PI;
            //double leftFaceX2 = c0.X + r * Math.Cos(leftFaceAngle2);
            //double leftFaceY2 = c0.Y + r * Math.Sin(leftFaceAngle2);
            Coordinate leftFace2 = GetNewPoint(c0, leftFaceAngle2, r);

            double rightFaceAngle1 = angle + Math.PI / 2.0d;
            //double rightFaceX1 = c1.X + r * Math.Cos(rightFaceAngle1);
            //double rightFaceY1 = c1.Y + r * Math.Sin(rightFaceAngle1);
            Coordinate rightFace1 = GetNewPoint(c1, rightFaceAngle1, r);
            double rightFaceAngle2 = angle + Math.PI / 2.0d + Math.PI;
            //double rightFaceX2 = c1.X + r * Math.Cos(rightFaceAngle2);
            //double rightFaceY2 = c1.Y + r * Math.Sin(rightFaceAngle2);
            Coordinate rightFace2 = GetNewPoint(c1, rightFaceAngle2, r);
            //double xmin = Math.Min(Math.Min(leftFaceX1, leftFaceX2), Math.Min(rightFaceX1, rightFaceX2));
            double xmin = Math.Min(Math.Min(leftFace1.X, leftFace2.X), Math.Min(rightFace1.X, rightFace2.X));
            //double ymin = Math.Min(Math.Min(leftFaceY1, leftFaceY2), Math.Min(rightFaceY1, rightFaceY2));
            double ymin = Math.Min(Math.Min(leftFace1.Y, leftFace2.Y), Math.Min(rightFace1.Y, rightFace2.Y));
            //double xmax = Math.Max(Math.Max(leftFaceX1, leftFaceX2), Math.Max(rightFaceX1, rightFaceX2));
            double xmax = Math.Max(Math.Max(leftFace1.X, leftFace2.X), Math.Max(rightFace1.X, rightFace2.X));
            //double ymax = Math.Max(Math.Max(leftFaceY1, leftFaceY2), Math.Max(rightFaceY1, rightFaceY2));
            double ymax = Math.Max(Math.Max(leftFace1.Y, leftFace2.Y), Math.Max(rightFace1.Y, rightFace2.Y));
            return new GeoAPI.Geometries.Envelope(xmin, xmax, ymin, ymax);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="angle">方位角是从某点的指北方向线起依顺时针方向至目标方向线间的水平夹角，单位弧度</param>
        /// <param name="dis">单位米</param>
        /// <returns></returns>
        private Coordinate GetNewPoint(Coordinate coordinate, double angle, double dis)
        {
            var r = GeometryEngine.MoveGeodetic(new List<MapPoint>() { new MapPoint(coordinate.X, coordinate.Y, new SpatialReference(3857)) }
                , dis, LinearUnits.Meters, angle, AngularUnits.Radians, GeodeticCurveType.Geodesic);
            return new Coordinate(r[0].X, r[0].Y);
        }
    }
}
