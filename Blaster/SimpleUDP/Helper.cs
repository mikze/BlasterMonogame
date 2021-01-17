using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleConnection
{
    public static class Helper
    {
        public static string StringToBaseString(string s) => Convert.ToBase64String(Encoding.ASCII.GetBytes(s));

        public static byte[] ParseObjToSend(object obj)
        {
            var Json = JsonConvert.SerializeObject(obj);
            var base64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(Json));
            return Encoding.ASCII.GetBytes(base64);
        }

        public static string ByteArrayToStringMsg(byte[] msg) => Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(msg)));
    }
}
