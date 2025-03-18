using System.Windows.Forms;
using System.Threading;
namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        Renderer renderer = new Renderer();
        
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();
            initialize();
            programTimer.Enabled = true;
        }
        void initialize()
        {
            renderer.Initialize();   
        }
        
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Update();
            renderer.Draw();
        }

        private void programTimer_Tick(object sender, System.EventArgs e)
        {
            simpleOpenGlControl1.Refresh();
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            float speed = 5;

            if (e.KeyChar == 'd')
                renderer.translationX += speed; 
            if (e.KeyChar == 'a')
                renderer.translationX -= speed;
            
            if (e.KeyChar == 'w')
                renderer.translationY += speed;
            if (e.KeyChar == 's')
                renderer.translationY -= speed;

            if (e.KeyChar == 'z')
                renderer.translationZ += speed;
            if (e.KeyChar == 'c')
                renderer.translationZ -= speed;

            simpleOpenGlControl1.Refresh();
        }

    }
}
