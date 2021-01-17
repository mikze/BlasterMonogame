using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleConnection
{
    public static class Helper
    {
        public static string StringToBaseString(string s) => Convert.ToBase64String(Encoding.ASCII.GetBytes(s));

        public static byte[] ParseObjToSend(object obj, string ackHash = "")
        {
            var Json = JsonConvert.SerializeObject(obj);
            var Frame = JsonConvert.SerializeObject(new Ramka() { msg = Json, ackHash = ackHash });
            var base64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(Frame));
            return Encoding.ASCII.GetBytes(base64);
        }

        public static string Generatehash()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rnd = new Random();
            return new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[rnd.Next(chars.Length)]).ToArray());
        }

        public static Ramka EncapsulateFromFrame(string JsonStr)
        {
            return JsonConvert.DeserializeObject<Ramka>(JsonStr);
        }
        public static string ByteArrayToStringMsg(byte[] msg) => Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(msg)));
    }
}
