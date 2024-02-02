using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker
{
    class Point2Df
    {
        public float x = 0;
        public float y = 0;

        public Point2Df(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public byte[] ToBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(x));
            list.AddRange(BitConverter.GetBytes(y));
            return list.ToArray();
        }

        public List<byte> ToByteList()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(x));
            list.AddRange(BitConverter.GetBytes(y));
            return list;
        }

        public override string ToString()
        {
            return "x:" + x.ToString("G9") + ",y:" + y.ToString("G9");
        }
    }
}
