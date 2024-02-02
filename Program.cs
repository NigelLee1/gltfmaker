using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gltfMaker.MakerGroup;
using System.Diagnostics;
using NetTopologySuite;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Geometry;
using GeoAPI.Geometries;
using gltfMaker.Models;
using System.Reflection;

namespace gltfMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //gltfCreator.CreateTriangleGltf(new Point3Df(0, 0, 0), new Point3Df(1, 0, 0), new Point3Df(0, 1, 0));
            //gltfCreator.CreateRectangleGltf(new Point3Df(0, 0, 0), new Point3Df(1, 0, 0), new Point3Df(1, 1, 0), new Point3Df(0, 1, 0));
            //gltfCreator.CreateCuboidGltf(new Point3Df(0, 0, 0), new Point3Df(1000, 0, 0), new Point3Df(1000, 1000, 0), new Point3Df(0, 1000, 0),
            //  new Point3Df(0, 0, -1000), new Point3Df(1000, 0, -1000), new Point3Df(1000, 1000, -1000), new Point3Df(0, 1000, -1000));
            //return;
            //          var disX = GeometryEngine.DistanceGeodetic(new MapPoint(12620776.204, 2573318.802, new SpatialReference(3857)), 
            //              new MapPoint(12620776.041, 2573316.65, new SpatialReference(3857)), LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic).Distance;
            //Console.WriteLine(disX);
            // double disX = GeometryEngine.DistanceGeodetic(new MapPoint(12620776.041, 2573316.65, new SpatialReference(3857)), 
            // new MapPoint(12620774.681, 2573312.841, new SpatialReference(3857)), LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic).Distance;
            // Console.WriteLine(disX);
            //  float r = 1;
            //  gltfCreator.CreateCuboidGltf(new Point3Df(-r, -r, 0), new Point3Df(r, -r, 0), new Point3Df(r, r, 0), new Point3Df(-r, r, 0),
            //   new Point3Df(-r, -r, -2*r), new Point3Df(r, -r, -2*r), new Point3Df(r, r, -2*r), new Point3Df(-r, r, -2*r)); // getOrientations
            // gltfCreator.CreateCuboidGltf(new Point3Df(-r, 0, r), new Point3Df(r, 0, r), new Point3Df(r, 2*r, r), new Point3Df(-r, 2*r, r),
            //  new Point3Df(-r, 0, -r), new Point3Df(r, 0, -r), new Point3Df(r, 2*r, -r), new Point3Df(-r, 2*r, -r)); // featureTableJson.EAST_NORTH_UP = true;
            //gltfCreator.CreateCuboidGltf(new Point3Df(-r, -r, r), new Point3Df(r, -r, r), new Point3Df(r, r, r), new Point3Df(-r, r, r),
            // new Point3Df(-r, -r, -r), new Point3Df(r, -r, -r), new Point3Df(r, r, -r), new Point3Df(-r, r, -r));
            // string pngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ws_r.png";
            // string outPngPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\ws_r2.png";
            // PngUtils.TransparentPng(pngPath, 0.8f, outPngPath);
            /*gltfCreator.Create3DTileIdentityPipeWithArrow();
           // gltfCreator.CreateComb();
            Console.WriteLine("success");
            Console.ReadLine();
            return;*/
            //Assembly assm = Assembly.GetExecutingAssembly();
            //String[] strs = assm.GetManifestResourceNames();
            
            /*try
            {
                ArcGISRuntimeEnvironment.InstallPath = Directory.GetCurrentDirectory();
                // Deployed applications must be licensed at the Lite level or greater. 
                // See https://developers.arcgis.com/licensing for further details.
                string licenseKey = "runtimelite,1000,rud4335202841,none,HC5X0H4AH7CLTK118123";
                Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.SetLicense(licenseKey);
                
                // Initialize the ArcGIS Runtime before any components are created.
                ArcGISRuntimeEnvironment.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //MessageBox.Show(ex.ToString(), "ArcGIS Runtime initialization failed.");

                // Exit application
                Console.ReadLine();
                return;
            }*/
            //TestMoveGeodetic();
            //return;
           // Test();
            //Console.ReadLine();
           // return;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //ManholeMaker.CreateManhole();
            CuboidMaker.CreateCuboid();
           // gltfCreator.CreateCylinderGltf(new Point3Df(0, 1, 0), 1, 2, 16);
            

           // string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\PS_COMB.shp";
            //List<Comb> combs = ShapeFileManager.LoadCombFromShapeFile(path);
             /*string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\PS_PIPE.shp";
            List<Pipe> pipes = ShapeFileManager.LoadPipeFromShapeFile(path);
            gltfCreator.MakeGd3dTiles(pipes);*/
            //gltfCreator.CreateCesiumPipesGltf(pipes);
         //   gltfCreator.CreateCesiumPipes3DTileGltf(pipes);
           // gltfCreator.Create3DTileIdentityPipe(pipes);
           // gltfCreator.CreateGdPipesGltf(pipes);
            sw.Stop();
            Console.WriteLine("success;用时:" + sw.ElapsedMilliseconds + "ms");
            Console.ReadLine();
        }

        static void Test()
        {
            Coordinate center4326 = new Coordinate(113.38078273676983, 22.513659665686344);
            Coordinate center3857 = Trans4326To3857(center4326);
            Coordinate left3857 = new Coordinate(center3857.X, center3857.Y - 500);
            Coordinate right3857 = new Coordinate(center3857.X, center3857.Y + 500);
            var dis = GeometryEngine.DistanceGeodetic(new MapPoint(left3857.X, left3857.Y, new SpatialReference(3857)), 
              new MapPoint(right3857.X, right3857.Y, new SpatialReference(3857)),
                            LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic); // 924.241954230439
            //Coordinate left4326 = Trans3857To4326(left3857);
            //Coordinate right4326 = Trans3857To4326(right3857);
            //var dis = GeometryEngine.DistanceGeodetic(new MapPoint(left4326.X, left4326.Y, new SpatialReference(4326)), 
              //  new MapPoint(right4326.X, right4326.Y, new SpatialReference(4326)),
         //       LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic); // 924.241954230439
            Console.WriteLine(dis.Distance);
        }

        static void TestMoveGeodetic()
        {
            Coordinate center4326 = new Coordinate(113.38078273676983, 22.513659665686344);
            Coordinate c0 = Trans4326To3857(center4326);
            Console.WriteLine("c0.x:" + c0.X + ",c0.y:" + c0.Y);
            Coordinate c1 = new Coordinate(c0.X + 100, c0.Y);
            List<MapPoint> mapPoints = new List<MapPoint>();
            mapPoints.Add(new MapPoint(c0.X, c0.Y, new SpatialReference(3857)));
            //mapPoints.Add(new MapPoint(c1.X, c1.Y, new SpatialReference(3857)));
           // Polyline polyline = new Polyline(mapPoints);
            var r = GeometryEngine.MoveGeodetic(mapPoints, 10, LinearUnits.Meters, 0, AngularUnits.Radians, GeodeticCurveType.Geodesic);
            Console.WriteLine("x:" + r[0].X + ",y:" + r[0].Y);
            var r2 = GeometryEngine.MoveGeodetic(mapPoints, 10, LinearUnits.Meters, Math.PI/2.0, AngularUnits.Radians, GeodeticCurveType.Geodesic);
            Console.WriteLine("x2:" + r2[0].X + ",y2:" + r2[0].Y);
            var dis1 = GeometryEngine.DistanceGeodetic(mapPoints[0], r[0], LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic).Distance;
            var dis2 = GeometryEngine.DistanceGeodetic(mapPoints[0], r2[0], LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic).Distance;
            Console.WriteLine("dis1:" + dis1.ToString("G9") + ",dis2:" + dis2.ToString("G9"));
            Console.ReadLine();
        }

        public static Coordinate Trans4326To3857(Coordinate p)
        {
            var mapPoint = new MapPoint(p.X, p.Y, new SpatialReference(4326));
            mapPoint = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mapPoint, new SpatialReference(3857));
            return new Coordinate(mapPoint.X, mapPoint.Y);
        }

        public static Coordinate Trans3857To4326(Coordinate p)
        {
            var mapPoint = new MapPoint(p.X, p.Y, new SpatialReference(3857));
            mapPoint = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mapPoint, new SpatialReference(4326));
            return new Coordinate(mapPoint.X, mapPoint.Y);
        }
    }
}
