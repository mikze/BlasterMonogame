using Blaster.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Blaster.Network
{
    public static class NetworkHelper
    {
        public static NetElement[] DownloadElementsFromServer()
        {
            BlasterClient.Send(new Frame() { body = "GetEntities" });
            Thread.Sleep(200);
            var Read = BlasterClient.GetFrames().Where(x => ((FrameKind)x.FrameKind) == FrameKind.entity);
            var netElements = splitNetElementsFromFrameBody(Read.First().body);
            return netElements;
        }

        public enum FrameKind
        {
            entity,
            movement,
            newPLayer
        }

        public struct Frame
        {
            public int id;
            public int FrameKind;
            public string body;
        }

        static NetElement[] splitNetElementsFromFrameBody(string frameBody)
        {
            List<NetElement> elems = new List<NetElement>();
            var splittedFrameBody = frameBody.Split('#', StringSplitOptions.RemoveEmptyEntries);

            foreach(var e in splittedFrameBody)
                elems.Add(splitNetElementFromFrameBody(e));

            return elems.ToArray();
        }

        public static NetElement splitNetElementFromFrameBody(string elemBody)
        {
            var splittedBody = elemBody.Split('-');
            var elem = new NetElement()
            {
                Id = int.Parse(splittedBody[0]),
                ComponentType = splittedBody[1],
                Name = splittedBody[2],
                Position = new Microsoft.Xna.Framework.Vector2(float.Parse(splittedBody[3]), float.Parse(splittedBody[4]))
            };

            return elem;
        }
    }
}

