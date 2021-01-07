using BlasterServer.Entities;
using SimpleUDP.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SimpleUDP;

namespace BlasterServer
{
    class Program
    {
        private static bool exit;
        static Host<Frame> host;
        static List<Entity> entities = new List<Entity>();
        private static float speed = 5;

        static void Main(string[] args)
        {
            host = new Host<Frame>(6666);

            host.OnClientAdded += c =>
            {
                var client = (Client)c;
                var e = new Entity(client.Id, new System.Numerics.Vector2(100, 100), "Player");
                entities.Add(e);
                BradCastNewPlayerToOthers(e);
            };

            host.RecieveHandler = (t, o) =>
            {
                if ((FrameKind)t.FrameKind == FrameKind.entity)
                    HandleSendEntities(o);

                if ((FrameKind)t.FrameKind == FrameKind.movement)
                    HandleMovement(o, t);

                if ((FrameKind)t.FrameKind == FrameKind.chat)
                    HandleChat(o, t);
            };

            host.OnClientDisconnected += c =>
            {
                entities.Remove(entities.First(x => x.Id == c.Id));
            };

            new TaskFactory().StartNew(
                () =>
                    {
                        while (!exit)
                        {
                            handleCommand(Console.ReadLine());
                        }
                    }
                );

            host.Listen();

        }

        private static void HandleChat(int o, Frame t)
        {
            host.BroadCast(new Frame() { id = o, FrameKind = (int)FrameKind.chat, body = t.body });
        }

        private static void BradCastNewPlayerToOthers(Entity e)
        {
            foreach(var c in host.clients)
            {
                if(c.Id != e.Id)
                    host.Send(new Frame() { id = e.Id, FrameKind = 2, body = ParseEntityToBody(e) }, c.Id);
            }
        }

        private static void HandleSendEntities(int id)
        {
            var body = "";
            foreach (var e in entities)
            {
                body += ParseEntityToBody(e) + "#";
            }
            host.Send(new Frame() { id = id, FrameKind = 0, body = body }, id);
        }

        private static void HandleMovement(int id, Frame frame)
        {
            var e = entities.First(x => x.Id == id);
            if (frame.body == "right")
            {
                e.Position = new System.Numerics.Vector2(e.Position.X + speed, e.Position.Y);
            }
            if (frame.body == "left")
            {
                e.Position = new System.Numerics.Vector2(e.Position.X - speed, e.Position.Y);
            }
            if (frame.body == "up")
            {
                e.Position = new System.Numerics.Vector2(e.Position.X, e.Position.Y - speed);
            }
            if (frame.body == "down")
            {
                e.Position = new System.Numerics.Vector2(e.Position.X, e.Position.Y + speed);
            }

            host.BroadCast(new Frame() { id = id, FrameKind = 1, body = $"{e.Position.X}-{e.Position.Y}" });
        }
        private static string ParseEntityToBody(Entity e)
        {
            return $"{e.Id}-{e.Name}-{e.ComponentType}-{e.Position.X}-{e.Position.Y}";
        }

        public enum FrameKind
        {
            entity,
            movement,
            newPLayer,
            chat
        }

        public struct Frame
        {
            public int id;
            public int FrameKind;
            public string body;
        }

        static void handleCommand(string command)
        {
            if(command == "exit")
            {
                exit = true;
                host.Exit();
            }
            if(command == "ShowClients")
            {
                foreach(var c in host.clients)
                {
                    Console.WriteLine(c.Id);
                }
            }

            if (command == "ShowEntitiesNum")
            {
                Console.WriteLine(entities.Count);
            }
        }
    }
}
