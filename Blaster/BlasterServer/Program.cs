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
                
            };

            host.RecieveHandler = (t, o) =>
            {
                if ((FrameKind)t.FrameKind == FrameKind.playerConnect)
                    HandlePlayerConnect(o, t);

                if ((FrameKind)t.FrameKind == FrameKind.entity)
                    HandleSendEntities(o);

                if ((FrameKind)t.FrameKind == FrameKind.movement)
                    HandleMovement(o, t);

                if ((FrameKind)t.FrameKind == FrameKind.chat)
                    HandleChat(o, t);

                if ((FrameKind)t.FrameKind == FrameKind.setName)
                    HandleSetName(o, t);

                if ((FrameKind)t.FrameKind == FrameKind.setPlayerState)
                    HandleSetPLayerState(o, t);
            };

            host.OnClientDisconnected += c =>
            {
                entities.Remove(entities.First(x => x.Id == c.Id));
                host.BroadCast(new Frame() { id = c.Id, FrameKind = (int)FrameKind.playerDisconnected, body = string.Empty });
                Console.WriteLine($"Disconnected {c.Id}");
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
        public enum State
        {
            Idle,
            Walking
        }

        private static void HandleSetPLayerState(int o, Frame t)
        {    
            host.BroadCast(new Frame() {id = o, FrameKind = (int)FrameKind.setPlayerState, body = t.body });
        }

        private static void HandlePlayerConnect(int o, Frame t)
        {
            Console.WriteLine($"Connected {o}");
            var e = new Entity(o, new System.Numerics.Vector2(100, 100), t.body);
            entities.Add(e);
            BradCastNewPlayerToOthers(e);
        }

        private static void HandleSetName(int o, Frame t) 
        {
            var e = entities.First(x => x.Id == o);
            e.Name = t.body;
        }

        private static void HandleChat(int o, Frame t) => host.BroadCast(new Frame() { id = o, FrameKind = (int)FrameKind.chat, body = t.body });

        private static void BradCastNewPlayerToOthers(Entity e)
        {
            foreach (var c in host.clients)
            {
                if (c.Id != e.Id)
                    host.Send(new Frame() { id = e.Id, FrameKind = (int)FrameKind.newPLayer, body = ParseEntityToBody(e) }, c.Id);
            }
        }

        private static void HandleSendEntities(int id)
        {
            var body = "";
            foreach (var e in entities)
            {
                body += ParseEntityToBody(e) + "#";
            }
            host.Send(new Frame() { id = id, FrameKind = (int)FrameKind.entity, body = body }, id);
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

            host.BroadCast(new Frame() { id = id, FrameKind = (int)FrameKind.movement, body = $"{e.Position.X}-{e.Position.Y}" });
        }
        private static string ParseEntityToBody(Entity e)
        {
            return $"{e.Id}-{e.Name}-{e.ComponentType}-{e.Position.X}-{e.Position.Y}";
        }

        public enum FrameKind
        {
            entity = 1,
            movement = 2,
            newPLayer = 3,
            playerDisconnected = 4,
            chat= 5,
            setName =6 ,
            playerConnect = 7,
            setPlayerState = 8
        }

        public struct Frame
        {
            public int id;
            public int FrameKind;
            public string body;
        }

        static void handleCommand(string command)
        {
            if (command == "exit")
            {
                exit = true;
                host.Exit();
            }
            if (command == "ShowClients")
            {
                foreach (var c in host.clients)
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
