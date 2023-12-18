using GraphicsEngine.Engine;
using System.Diagnostics;

namespace GraphicsEngine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Width = EngineManager.instance.mainCamera.screenWidthScaled; 
            Height = EngineManager.instance.mainCamera.screenHeightScaled;
            EngineManager.instance.Init();
            InitEvents();
        }
        private void InitEvents()
        {
            pictureBox1.Paint += EngineManager.instance.Draw;
            this.KeyDown += EngineManager.instance.GetPlayerKeyHandlerDown();
            this.KeyUp += EngineManager.instance.GetPlayerKeyHandlerUp();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            EngineManager.instance.Update();
            pictureBox1.Refresh();
        }
    }
}