using System.Diagnostics;

namespace CommunicateFramework.ace.auto
{
    public class SocketModel
    {
        public int type { get; set; }
        public int area { get; set; }
        public int command { get; set; }
        public object message { get; set; }
        
        public SocketModel(){}

        public SocketModel(int _type, int _area, int _command, object _obj)
        {
            this.type = _type;
            this.area = _area;
            this.command = _command;
            this.message = _obj;
        }

        public T getMessage<T>()
        {
            return (T) message;
        }

        public override string ToString()
        {
            return type + "------" + area + "------" + command + "------" + message;
        }
    }
}