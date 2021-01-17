using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Components
{
    public enum Facing
    {
        Left, Right
    }
    public enum State
    {
        Idle,
        Walking
    }

    public class Player
    {
        public Facing Facing { get; set; } = Facing.Right;
        public State State { get; set; }
    }
}
