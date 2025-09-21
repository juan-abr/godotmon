using Godot;
using System;

namespace Game.Core
{
    public partial class Globals : Node
    {
        public static Globals Instance { get; private set; }

        [ExportCategory("Gameplay")]
        [Export] public int GRID_SIZE = 16;

        public override void _Ready()
        {
            Instance = this;

            // Check to make sure we're getting log messages.
            //Logger.Debug("Loading Globals ...");
            //Logger.Info("Loading Globals ...");
            //Logger.Warning("Loading Globals ...");
            //Logger.Error("Loading Globals ...");
        }
    }
}
