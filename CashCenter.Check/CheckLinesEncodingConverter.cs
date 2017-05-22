using System.Text;

namespace CashCenter.Check
{
    public class CheckLinesEncodingConverter
    {
        private readonly Encoding srcEncoding;
        private readonly Encoding dstEncoding = Encoding.GetEncoding(1251);

        public CheckLinesEncodingConverter(Encoding srcEncoding)
        {
            this.srcEncoding = srcEncoding ?? Encoding.UTF8;
        }

        public string EncodeString(string srcString)
        {
            byte[] srcBytes = srcEncoding.GetBytes(srcString);
            byte[] dstBytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(dstBytes);
        }
    }
}
