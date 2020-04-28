using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace dgSerializeBin
{
    public class CustomSerializeBin<T>
    {
        public byte[] SerializetoBytes(T data)
        {
            byte[] rs = null;
            if (data != null)
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms,data);
                rs= ms.GetBuffer();
            }
            return rs;
            
        }
        public T DeserializeFromBytes(byte[] dataBin)
        {
            T rs = default(T);
            if(dataBin != null && dataBin.Length > 0)
            {
                MemoryStream ms = new MemoryStream(dataBin);
                BinaryFormatter bf = new BinaryFormatter();
                rs = (T)bf.Deserialize(ms);
            }
            return rs;
        }
    }
}
