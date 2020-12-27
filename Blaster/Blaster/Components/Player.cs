using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Components
{
    public enum State
    {
        Idle,
        Walking
    }

    public class Player
    {
        public State State { get; set; }
    }
}
