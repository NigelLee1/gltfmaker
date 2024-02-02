using Esri.ArcGISRuntime.Geometry;
using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker
{
    static class CoordinateChangeUtils
    {
        // 高德三维坐标系使用了 20 级的平面像素坐标作为基础，并且将坐标系原点调整为中国大地原点。X 轴方向是正东，Y 轴为正南，Z 轴指向地面
        // 中国大地原点 北纬34°32′27.00″东经108°55′25.00″  // 34+32/60.0+27/3600.0,108+55/60.0+25/3600.0=34.54083333333333,108.92361111111111
        // 即3857:{12125320.9242534,4101655.98664977}/{12125836.9551515,4101459.54921725}
        // 待处理：确定3857坐标转 20 级的平面像素坐标 的公式
        // gcj02                                   3857              高德20级平面像素坐标
        //[113.38093545036813, 22.513659665686344] 12621508, 2573309 {x: 218760681.0814615, y: 116980869.92796119}
        //[0, 0] 0,0 {x: 134217728, y: 134217728}
        //(218760681.0814615-134217728)*0.14929107086944593=12621507.999996709
        //(134217728-116980869.92796119)*0.14929107086944593=2573308.9999993276

        public const double resolution = 0.14929107086944593d; // 20级分辨率

        /// <summary>
        /// 求高德三维坐标
        /// </summary>
        /// <param name="p">3857坐标</param>
        /// <returns></returns>
        public static Point3Df GetGd3dCoord(Coordinate c)
        {
            double pixelX = c.X / resolution + 134217728; // 218760688
            double pixelY = 134217728 - c.Y / resolution; // 116980872

            //float pixel0X = 12125320.9242534f / resolution + 134217728f; // 215437056
            //float pixel0Y = 134217728f - 4101655.98664977f / resolution; // 106743504
            const double pixel0X = 215440491d; // 通过高德JS API将map.lngLatToGeodeticCoord 与 map.lnglatToPixel的值相减求得
            const double pixel0Y = 106744817d;
            double x = pixelX - pixel0X;
            double y = pixelY - pixel0Y;
            float fx = (float)x;
            float fy = (float)y;
            return new Point3Df(fx, fy, 0); // 3320176,10236048/3323632,10237368
        }

        public static Coordinate GetCesium3DTileCoord(Coordinate c)
        {
            var mapPoint = new MapPoint(c.X, c.Y, new SpatialReference(3857));
            string wktext = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_World Geodetic System 1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.017453292519943295]]";
            //SpatialReference sr4979 = new SpatialReference(wktext);
            SpatialReference spatialReference4326 = new SpatialReference(4326);
            mapPoint = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mapPoint, spatialReference4326);
            //Console.WriteLine("c:" + c + ",mapPoint:" + mapPoint);
            //return new Point3Df((float)mapPoint.X, (float)mapPoint.Y, 0);
            Coordinate coordinate = Cartesian3Utils.fromDegrees(mapPoint.X, mapPoint.Y);
            return new Coordinate((float)coordinate.X, (float)coordinate.Y, (float)coordinate.Z);
        }

        public static Point3Df GetCesium3DTileCoord(Coordinate c, Coordinate center)
        {
            //double k = 1000.0d / 1082.0254505109042d;
            //double k = 1;
            //double w = (c.X - center.X) * k;
            //double h = (c.Y - center.Y) * k;
            var disX = GeometryEngine.DistanceGeodetic(new MapPoint(c.X, center.Y, new SpatialReference(3857)), 
              new MapPoint(center.X, center.Y, new SpatialReference(3857)), LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic).Distance;
            //Coordinate Cartesian3center = GetCesium3DTileCoord(center);
            //Coordinate Cartesian3x = GetCesium3DTileCoord(new Coordinate(c.X, center.Y));
            //double disX = Cartesian3Utils.distance(Cartesian3x, Cartesian3center);
            if (c.X < center.X)
                disX = -disX;
            var disY = GeometryEngine.DistanceGeodetic(new MapPoint(center.X, c.Y, new SpatialReference(3857)), 
              new MapPoint(center.X, center.Y, new SpatialReference(3857)), LinearUnits.Meters, AngularUnits.Degrees, GeodeticCurveType.Geodesic).Distance;
            //Coordinate Cartesian3y = GetCesium3DTileCoord(new Coordinate(center.X, c.Y));
            //double disY = Cartesian3Utils.distance(Cartesian3y, Cartesian3center);
            if (c.Y < center.Y)
                disY = -disY;
            
           // Console.WriteLine("3857dis:" + (c.Y - center.Y).ToString("G9") + ",disY:" + disY.ToString("G9") + ",k:" + ((c.Y - center.Y)/disY*1000.0d).ToString("G9"));
            float x = (float)(center.X + disX);
            float y = (float)(center.Y + disY);
            return new Point3Df(x, y, 0);
        }

        public static void GetCesiumCoord(List<Pipe> pipes, int pipeCount, Coordinate center)
        {
            /*double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
            for (int i = 0; i < pipeCount; i++)
            {
                var pipe = pipes[i];
                if (pipe.c0.X < minX)
                    minX = pipe.c0.X;
                if (pipe.c0.Y < minY)
                    minY = pipe.c0.Y;
                if (pipe.c1.X < minX)
                    minX = pipe.c1.X;
                if (pipe.c1.Y < minY)
                    minY = pipe.c1.Y;

                if (pipe.c0.X > maxX)
                    maxX = pipe.c0.X;
                if (pipe.c0.Y > maxY)
                    maxY = pipe.c0.Y;
                if (pipe.c1.X > maxX)
                    maxX = pipe.c1.X;
                if (pipe.c1.Y > maxY)
                    maxY = pipe.c1.Y;
            }
            double originX = (minX + maxX) / 2.0d;
            double originY = (minY + maxY) / 2.0d;
            Console.WriteLine("GetCesiumCoord:minX:" + minX.ToString("G9") + ",minY:" + minY.ToString("G9") + ",maxX:" + maxX.ToString("G9") + ",maxY:" + maxY.ToString());
            Console.WriteLine("GetCesiumCoord:originX:" + originX.ToString("G9") + ",originY:" + originY.ToString("G9"));
            Coordinate center = new Coordinate(originX, originY);*/
            for (int i = 0; i < pipeCount; i++)
            {
                var pipe = pipes[i];
                pipe.p0 = GetCesium3DTileCoord(pipe.c0, center);
                pipe.p1 = GetCesium3DTileCoord(pipe.c1, center);
            }
        }
    }
}
