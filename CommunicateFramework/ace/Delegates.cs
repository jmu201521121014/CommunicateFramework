using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicateFramework.ace
{
    public delegate byte[] LengthEncode(byte[] value);
    public delegate byte[] LengthDecode(ref List<byte> cache);
    public delegate byte[] SerEncode(object value);
    public delegate object SerDecode(byte[] value);
}
