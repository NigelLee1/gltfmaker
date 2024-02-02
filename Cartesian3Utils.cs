using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker
{
    static class Cartesian3Utils
    {
        public static Coordinate fromDegrees(double longitude, double latitude, double height = 0) {
          longitude = toRadians(longitude);
          latitude = toRadians(latitude);
          return fromRadians(longitude, latitude, height);
        }

        private static Coordinate fromRadians(double longitude, double latitude, double height = 0)
        {
            var cosLatitude = Math.Cos(latitude);
            Coordinate scratchN = new Coordinate();
            scratchN.X = cosLatitude * Math.Cos(longitude);
            scratchN.Y = cosLatitude * Math.Sin(longitude);
            scratchN.Z = Math.Sin(latitude);
            scratchN = Normalize(scratchN);
            var wgs84RadiiSquared = new Coordinate(
              6378137.0 * 6378137.0,
              6378137.0 * 6378137.0,
              6356752.3142451793 * 6356752.3142451793
            );
            Coordinate scratchK = multiplyComponents(wgs84RadiiSquared, scratchN);
            var gamma = Math.Sqrt(dot(scratchN, scratchK));
            scratchK = divideByScalar(scratchK, gamma);
            scratchN = multiplyByScalar(scratchN, height);

            return add(scratchK, scratchN);
        }

        private static Coordinate Normalize(Coordinate cartesian)
        {
            Coordinate result = new Coordinate();
            var mag = magnitude(cartesian);
            result.X = cartesian.X / mag;
            result.Y = cartesian.Y / mag;
            result.Z = cartesian.Z / mag;
            return result;
        }

        private static double magnitude(Coordinate cartesian) {
            return Math.Sqrt(magnitudeSquared(cartesian));
        }

        private static double magnitudeSquared(Coordinate cartesian) 
        {
            return cartesian.X * cartesian.X + cartesian.Y * cartesian.Y + cartesian.Z * cartesian.Z;
        }

        private static Coordinate multiplyComponents(Coordinate left, Coordinate right)
        {
            Coordinate coordinate = new Coordinate();
            coordinate.X = left.X * right.X;
            coordinate.Y = left.Y * right.Y;
            coordinate.Z = left.Z * right.Z;
            return coordinate;
        }

        private static double dot(Coordinate left, Coordinate right) {
          return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        private static Coordinate divideByScalar(Coordinate cartesian, double scalar) {
            Coordinate result = new Coordinate();
            result.X = cartesian.X / scalar;
            result.Y = cartesian.Y / scalar;
            result.Z = cartesian.Z / scalar;
            return result;
        }

        private static Coordinate multiplyByScalar(Coordinate cartesian, double scalar) {
            Coordinate result = new Coordinate();
            result.X = cartesian.X * scalar;
            result.Y = cartesian.Y * scalar;
            result.Z = cartesian.Z * scalar;
            return result;
        }

        private static Coordinate add(Coordinate left, Coordinate right) {
            Coordinate result = new Coordinate();
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            return result;
        }

        private static Coordinate subtract(Coordinate left, Coordinate right) {
            Coordinate result = new Coordinate();
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            return result;
        }

        private static double toRadians(double degrees) {
            double RADIANS_PER_DEGREE = Math.PI / 180.0;
            return degrees * RADIANS_PER_DEGREE;
        }

        public static double distance(Coordinate left, Coordinate right)
        {
            Coordinate distanceScratch = subtract(left, right);
            return magnitude(distanceScratch);
        }
    }
}
