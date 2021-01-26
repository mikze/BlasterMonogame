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
            Thread.Sleep(300);
            BlasterClient.Send(new Frame() { FrameKind = (int)FrameKind.entity });
            Thread.Sleep(300);
            var Read = BlasterClient.GetFrames().Where(x => ((FrameKind)x.FrameKind) == FrameKind.entity);
            if (Read.First().body != string.Empty)
            {
                var netElements = splitNetElementsFromFrameBody(Read.First().body);
                return netElements;
            }
            else
                throw new Exception("Cannot connect to server.");
        }

        public static void PlayerConnect(string playerName)
        {
            BlasterClient.Send(new Frame() { FrameKind = (int)FrameKind.playerConnect, body = playerName });
        }
        public enum FrameKind
        {
            entity = 1,
            movement = 2,
            newPLayer = 3,
            playerDisconnected = 4,
            chat = 5,
            setName = 6,
            playerConnect = 7,
            setPlayerState = 8
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
                Name = splittedBody[1],
                ComponentType = splittedBody[2],
                Position = new Microsoft.Xna.Framework.Vector2(float.Parse(splittedBody[3]), float.Parse(splittedBody[4]))
            };

            return elem;
        }
    }
}

