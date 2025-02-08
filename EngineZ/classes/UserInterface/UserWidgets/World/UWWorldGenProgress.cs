using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.Entities;
using EngineZ.UI;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.CodeDom.Compiler;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EngineZ.UI
{
    public class UWWorldGenProgress : PanelWidget
    {
        public UWWorldGenProgress(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
        }

        public UWWorldGenProgress(HUD ownerHUD, Rectangle renderTransform, EWidgetAlignment widgetAlignment) : base(ownerHUD, renderTransform, widgetAlignment)
        {
        }
        WTextBlock progressText;
        WTextBlock taskText;
        WVerticalBox vert;
        int verticalBoxWidth = 500;
        public override void Construct()
        {
            base.Construct();

            taskText = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 100, 200, 64), "task", ETextJustification.Center);
            progressText = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 200, 64), "progress", ETextJustification.Center);
            vert = HUD.CreateWidget<WVerticalBox>(ownerHUD, new Rectangle(0, 0, verticalBoxWidth, 200));
            vert.AddChild(taskText);
            vert.AddChild(progressText);
            AddChild(vert);

            StartWorldGeneration();
        }

        private async void StartWorldGeneration()
        {
            TaskCompletionSource<bool> _genTaskCompletionSource = new TaskCompletionSource<bool>();
            Task _genTask;

            _genTask = Task.Run(async () =>
            {
                var newParams = new WorldGenParams()
                {
                    maxTilesX = 100,
                    maxTilesY = 100,
                    seed = 0,
                };

                Main.GetGame().world.GenFillWorld();
                Main.GetGame().world.taskProgressChanged += WGenProgressChanged;

                Airraret.renderWorld = false;
                await Main.GetGame().world.GenerateWorld(newParams);
                Airraret.renderWorld = true;

                Main.GetGame().CreatePlayer(World.worldSpawn);
                DestroyWidget();
            });

            //CODE HERE DOES NOT WAIT FOR Task.Run() TO FINISH
        }

        private void WGenProgressChanged(object sender, WorldGenProgress e)
        {
            taskText.text = e.CurrentTask.ToString();
            string t = Math.Ceiling(e.PercentComplete * 100f) + "%";
            progressText.text = t;
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            origin = new Vector2((WidgetAlignments.GetFullScreenWidget().Width / 2) - (verticalBoxWidth / 2) * HUD.DPIScale, 500);
            base.Draw(ref spriteBatch);

        }
    }
}
