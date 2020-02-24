namespace CommunicateFramework.ace.auto
{
    public class MessageEncoding
    {
        public static byte[] Encode(object _obj)
        {
            SocketModel sm = _obj as SocketModel;
            ByteArray ba = new ByteArray();
            ba.write(sm.type);
            ba.write(sm.area);
            ba.write(sm.command);
            if (sm.message != null)
            {
                byte[] a = SerializeUtil.encode(sm.message);
                ba.write(a);
            }

            byte[] res = ba.getBuff();
            ba.Close();
            return res;
        }

        public static object Decode(byte[] _byteArray)
        {
            ByteArray ba = new ByteArray(_byteArray);
            SocketModel sm = new SocketModel();
            int type;
            int area;
            int command;
            ba.read(out type);
            ba.read(out area);
            ba.read(out command);
            sm.type = type;
            sm.area = area;
            sm.command = command;
            if (ba.Readable)
            {
                byte[] message;
                ba.read(out message, ba.Length - ba.Position);
                sm.message = SerializeUtil.decoder(message);
            }

            ba.Close();
            return sm;
        }
    }
}