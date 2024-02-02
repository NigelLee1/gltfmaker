//using NetTopologySuite.Geometries;
//using NetTopologySuite.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using NetTopologySuite.Mathematics;

namespace gltfMaker.MakerGroup
{
    static class MakerUtils
    {
        public static float[] GetMax(List<Point3Df> points)
        {
            float[] result = new float[3];
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            foreach (var p in points)
            {
                if (p.x > maxX)
                    maxX = p.x;
                if (p.y > maxY)
                    maxY = p.y;
                if (p.z > maxZ)
                    maxZ = p.z;
            }
            result[0] = maxX;
            result[1] = maxY;
            result[2] = maxZ;
            return result;
        }

        public static float[] GetMin(List<Point3Df> points)
        {
            float[] result = new float[3];
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            foreach (var p in points)
            {
                if (p.x < minX)
                    minX = p.x;
                if (p.y < minY)
                    minY = p.y;
                if (p.z < minZ)
                    minZ = p.z;
            }
            result[0] = minX;
            result[1] = minY;
            result[2] = minZ;
            return result;
        }

        public static Point3Df CalcTriangleNormal(Point3Df p0, Point3Df p1, Point3Df p2)
        {
            //Coordinate c0 = new Coordinate() { X = p0.x, Y = p0.y, Z = p0.z };
            //Coordinate c1 = new Coordinate() { X = p1.x, Y = p1.y, Z = p1.z };
            //Coordinate c2 = new Coordinate() { X = p2.x, Y = p2.y, Z = p2.z };
            var n = NormalToTriangle(p0, p1, p2);
            //float fx = (float)n.X;
            //float fy = (float)n.Y;
            //float fz = (float)n.Z;
            //double dx = fx;
            //double dy = fy;
            //double dz = fz;
            //if (!n.X.Equals(dx) || !n.Y.Equals(dy) || !n.Z.Equals(dz))
            //  throw new Exception("CalcTriangleNormal error");
            //return new Point3Df(fx, fy, fz);
            return n;
        }

        /// <summary>
        /// Computes the normal vector to the triangle p0-p1-p2. In order to compute the normal each
        /// triangle coordinate must have a Z value. If this is not the case, the returned Coordinate
        /// will have NaN values. The returned vector has unit length.
        /// </summary>
        /// <param name="p0">A point</param>
        /// <param name="p1">A point</param>
        /// <param name="p2">A point</param>
        /// <returns>The normal vector to the triangle <paramref name="p0"/>-<paramref name="p1"/>-<paramref name="p2"/></returns>
        public static Point3Df NormalToTriangle(Point3Df p0, Point3Df p1, Point3Df p2)
        {
            var v1 = new Point3Df(p1.x - p0.x, p1.y - p0.y, p1.z - p0.z);
            var v2 = new Point3Df(p2.x - p0.x, p2.y - p0.y, p2.z - p0.z);
            var cp = CrossProduct(v1, v2);
            Normalize(cp);
            return cp;
        }

        /// <summary>
        /// Normalizes the vector <param name="v"></param>
        /// </summary>
        /// <param name="v">The normalized <paramref name="v"/></param>
        public static void Normalize(Point3Df v)
        {
            float absVal = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            v.x /= absVal;
            v.y /= absVal;
            v.z /= absVal;
        }

        /// <summary>
        /// Computes the cross product of <paramref name="v1"/> and <paramref name="v2"/>
        /// </summary>
        /// <param name="v1">A vector</param>
        /// <param name="v2">A vector</param>
        /// <returns>The cross product of <paramref name="v1"/> and <paramref name="v2"/></returns>
        public static Point3Df CrossProduct(Point3Df v1, Point3Df v2)
        {
            float x = Det(v1.y, v1.z, v2.y, v2.z);
            float y = -Det(v1.x, v1.z, v2.x, v2.z);
            float z = Det(v1.x, v1.y, v2.x, v2.y);
            return new Point3Df(x, y, z);
        }

        /// <summary>
        /// Computes the determinant of a 2x2 matrix
        /// </summary>
        /// <param name="a1">The m[0,0] value</param>
        /// <param name="a2">The m[0,1] value</param>
        /// <param name="b1">The m[1,0] value</param>
        /// <param name="b2">The m[1,1] value</param>
        /// <returns>The determinant</returns>
        public static float Det(float a1, float a2, float b1, float b2)
        {
            return (a1 * b2) - (a2 * b1);
        }

        public static string GetFileBase64String(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Close();
            return Convert.ToBase64String(buffer);
        }

        public static string GetStreamBase64String(Stream stream)
        {
            //FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            return Convert.ToBase64String(buffer);
        }

        public static float GetFloatColor(int c)
        {
            return (float)Math.Pow(c / 255, 2.2);
        }

        public static float[] GetFloatColor(int r, int g, int b)
        {
            float[] fs = new float[4];
            fs[0] = (float)Math.Pow(r / 255f, 2.2);
            fs[1] = (float)Math.Pow(g / 255f, 2.2);
            fs[2] = (float)Math.Pow(r / 255f, 2.2);
            fs[3] = 1;
            return fs;
        }

        /*public static float[] GetFloatColor(int r, int g, int b)
        {
            float[] fs = new float[4];
            fs[0] = r / 255f;
            fs[1] = g / 255f;
            fs[2] = r / 255f;
            fs[3] = 1;
            return fs;
        }*/
    }
}
