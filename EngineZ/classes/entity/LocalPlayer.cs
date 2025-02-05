using EngineZ.DataStructures;
using EngineZ.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.classes.entity
{
    public class LocalPlayer : Character
    {
        public LocalPlayer(Game game, EntityTypes initType) : base(game, initType)
        {
        }
    }
}
