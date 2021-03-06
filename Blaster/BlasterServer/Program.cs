﻿using BlasterServer.Entities;
using SimpleConnection.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SimpleConnection;
using SimpleConnection.Connection;
using SimpleUDP.Server;
using BlasterServer.Entities.Interfaces;

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
            for (int i = 0; i < 7; i++)
            {
                entities.Add(EnityFactory.CreateWall(0, i*55));
            }

            for (int i = 0; i < 10; i++)
            {
                entities.Add(EnityFactory.CreateWall(i*53,0));
            }

            for (int i = 0; i < 8; i++)
            {
                entities.Add(EnityFactory.CreateWall(10 * 53, i * 55));
            }

            for (int i = 0; i < 10; i++)
            {
                entities.Add(EnityFactory.CreateWall(i * 53, 7 * 55));
            }

            host = new Host<Frame>(new UDPConnection(6666));

            host.OnClientAdded += c =>
            {
                Console.WriteLine($"Client connected: {c.EndPoint}");
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

                if ((FrameKind)t.FrameKind == FrameKind.createBomb)
                    HandleCreateBomb(o, t);
            };

            host.OnClientDisconnected += c =>
            {
                entities.Remove(entities.First(x => x.Id == c.Id));
                host.BroadCast(new Frame() { id = c.Id, FrameKind = (int)FrameKind.playerDisconnected, body = string.Empty });

                Console.WriteLine($"Client disconnected: {c.EndPoint}");
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

        private static void HandleCreateBomb(int o, Frame t)
        {
            host.BroadCast(new Frame() { id = o, FrameKind = (int)FrameKind.createBomb, body = $"{entities.Where(x => x.Id == o).First().Position.X}-{entities.Where(x => x.Id == o).First().Position.Y}" });
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
            var e = EnityFactory.CreatePlayer(o);
            e.Name = t.body;
            entities.Add(e);
            BradCastNewPlayerToOthers(e);
        }

        private static void HandleSetName(int o, Frame t) 
        {
            var e = entities.First(x => x.Id == o);
            e.Name = t.body;
        }

        private static void HandleChat(int o, Frame t) { 
            host.BroadCast(new Frame() { id = o, FrameKind = (int)FrameKind.chat, body = t.body });
            Console.WriteLine($"{t.id} wrote: {t.body}");
        }

        private static void BradCastNewPlayerToOthers(Entity e)
        {
            foreach (var c in host.clients)
            {
                if (c.Id != e.Id)
                    host.Send(new Frame() { id = e.Id, FrameKind = (int)FrameKind.newPLayer, body = ParseEntityToBody(e) }, c.Id, true);
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
            var oldPosition = e.Position;
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
            
            foreach(var _e in entities)
            {
                if (_e is IWallEntity && e.Id != _e.Id && _e.Hitbox.IsCollision(e.Hitbox))
                {
                    e.Position = oldPosition;
                }
            }

            host.BroadCast(new Frame() { id = id, FrameKind = (int)FrameKind.movement, body = $"{e.Position.X}-{e.Position.Y}" });
        }

        private static string ParseEntityToBody(Entity e)
        {
            return $"{e.Id}-{e.Name}-{SniffType(e)}-{e.Position.X}-{e.Position.Y}";
        }

        private static string SniffType(Entity e)
        {
            if (e is IPlayerEntity)
                return "player";
            if (e is IWallEntity)
                return "wall";

            return "";
        }

        public enum FrameKind
        {
            entity = 1,
            movement = 2,
            newPLayer = 3,
            playerDisconnected = 4,
            chat = 5,
            setName = 6 ,
            playerConnect = 7,
            setPlayerState = 8,
            createBomb = 9
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
