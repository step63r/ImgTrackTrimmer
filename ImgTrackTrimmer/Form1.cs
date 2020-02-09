using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ImgTrackTrimmer
{
    public struct CaptureSize
    {
        public int startx, starty, endx, endy;
    }

    public partial class Main : Form
    {
        string strfilename, access;
        private Bitmap bmp;
        private int startx, starty, endx, endy;
        private bool dirtyFlag = false;
        private bool readOnlyFlag = false;
        private PictureBoxSizeMode sizeMode;

        OpenFileDialog ofd;
        SaveFileDialog sfd;

        public CaptureSize newSize = new CaptureSize();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            strfilename = "";
            access = "";
            sizeMode = pictureBox1.SizeMode;
            if (sizeMode == PictureBoxSizeMode.Normal)
            {
                stToolStripMenuItem.Checked = true;
            }
            else if (sizeMode == PictureBoxSizeMode.StretchImage)
            {
                stretchImageToolStripMenuItem.Checked = true;
            }
            else if (sizeMode == PictureBoxSizeMode.CenterImage)
            {
                centerImageToolStripMenuItem.Checked = true;
            }
            else
            {
                zoomToolStripMenuItem.Checked = true;
            }

            numericUpDown1.Maximum = Screen.PrimaryScreen.Bounds.Width;
            numericUpDown2.Maximum = Screen.PrimaryScreen.Bounds.Height;
            numericUpDown3.Maximum = Screen.PrimaryScreen.Bounds.Width;
            numericUpDown4.Maximum = Screen.PrimaryScreen.Bounds.Height;
            Status1.Text = "Ready";
        }

        private void saveAsAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string MSGBOX_TITLE = "Save File";
            sfd = new SaveFileDialog();
            sfd.Filter = "JPG Image(*.jpg)|*.jpg|PNG Image(*.png)|*.png|Bitmap Image(*.bmp)|*.bmp|GIF Image(*.gif)|*.gif|Icon(*.ico)|*,ico";
            sfd.FilterIndex = 1;
            sfd.FileName = "*.jpg";
            DialogResult result = new DialogResult();
            result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    string extension = Path.GetExtension(sfd.FileName);
                    strfilename = Path.GetFileName(sfd.FileName);
                    access = Path.GetFullPath(sfd.FileName);

                    switch (extension.ToUpper())
                    {
                        case ".JPG":
                            pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case ".PNG":
                            pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case ".BMP":
                            pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case ".GIF":
                            pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case ".ICO":
                            pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Icon);
                            break;
                    }
                    Status1.Text = "Image Saved: " + access + " at " + DateTime.Now.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, MSGBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string MSGBOX_TITLE = "Save File";
            string extension = Path.GetExtension(strfilename);
            try
            {
                switch (extension.ToUpper())
                {
                    case ".JPG":
                        pictureBox1.Image.Save(access, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".PNG":
                        pictureBox1.Image.Save(access, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".BMP":
                        pictureBox1.Image.Save(access, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".GIF":
                        pictureBox1.Image.Save(access, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case ".ICO":
                        pictureBox1.Image.Save(access, System.Drawing.Imaging.ImageFormat.Icon);
                        break;
                }
                Status1.Text = "Image Saved: " + access + " at " + DateTime.Now.ToString();
                setDirty(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MSGBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuitAppXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadSizeLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "XML File(*.xml)|*.xml|All File(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CaptureSize));
                    StreamReader sr = new StreamReader(ofd.FileName, new System.Text.UTF8Encoding(false));
                    newSize = (CaptureSize)serializer.Deserialize(sr);
                    sr.Close();

                    numericUpDown1.Value = newSize.startx;
                    numericUpDown2.Value = newSize.starty;
                    numericUpDown3.Value = newSize.endx;
                    numericUpDown4.Value = newSize.endy;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveSizeZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfd = new SaveFileDialog();
            sfd.Filter = "XML File(*.xml)|*.xml|All File(*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.FileName = "*.xml";
            DialogResult result = new DialogResult();
            result = sfd.ShowDialog();

            newSize.startx = (int)numericUpDown1.Value;
            newSize.starty = (int)numericUpDown2.Value;
            newSize.endx = (int)numericUpDown3.Value;
            newSize.endy = (int)numericUpDown4.Value;

            XmlSerializer serializer = new XmlSerializer(typeof(CaptureSize));
            StreamWriter sw = new StreamWriter(sfd.FileName, false, new System.Text.UTF8Encoding(false));

            if (result == DialogResult.OK)
            {
                try
                {
                    serializer.Serialize(sw, newSize);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void showReadmetxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string stCurrentDir = System.IO.Directory.GetCurrentDirectory();
            Process.Start(stCurrentDir + "/Readme.txt");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                startx = (int)numericUpDown1.Value;
                starty = (int)numericUpDown2.Value;
                endx = (int)numericUpDown3.Value;
                endy = (int)numericUpDown4.Value;
                bmp = new Bitmap(endx - startx, endy - starty);
                Point startp = new Point(startx, starty);
                Point endp = new Point(endx, endy);
                Graphics g = Graphics.FromImage(bmp);

                g.CopyFromScreen(startp, new Point(0, 0), bmp.Size);
                g.Dispose();

                pictureBox1.Image = bmp;
                if (strfilename != "")
                {
                    setDirty(true);
                }
                
            }catch(Exception ex)
            {
                Status1.Text = "Error!: " + ex.Message;
            }
            
        }

        private void stToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SizeModeChanged(stToolStripMenuItem, PictureBoxSizeMode.Normal);
        }

        private void stretchImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SizeModeChanged(stretchImageToolStripMenuItem, PictureBoxSizeMode.StretchImage);
        }

        private void centerImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SizeModeChanged(centerImageToolStripMenuItem, PictureBoxSizeMode.CenterImage);
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SizeModeChanged(zoomToolStripMenuItem, PictureBoxSizeMode.Zoom);
        }

        private void SizeModeChanged(ToolStripMenuItem sItem, PictureBoxSizeMode sMode)
        {
            if (sItem.Checked == true)
            {
            }
            else
            {
                sizeMode = sMode;
                pictureBox1.SizeMode = sizeMode;
                sItem.Checked = true;
                if (sItem == stToolStripMenuItem)
                {
                    stretchImageToolStripMenuItem.Checked = false;
                    centerImageToolStripMenuItem.Checked = false;
                    zoomToolStripMenuItem.Checked = false;

                }
                else if (sItem == stretchImageToolStripMenuItem)
                {
                    stToolStripMenuItem.Checked = false;
                    centerImageToolStripMenuItem.Checked = false;
                    zoomToolStripMenuItem.Checked = false;
                }
                else if (sItem == centerImageToolStripMenuItem)
                {
                    stToolStripMenuItem.Checked = false;
                    stretchImageToolStripMenuItem.Checked = false;
                    zoomToolStripMenuItem.Checked = false;
                }
                else
                {
                    stToolStripMenuItem.Checked = false;
                    stretchImageToolStripMenuItem.Checked = false;
                    centerImageToolStripMenuItem.Checked = false;
                }
            }
        }

        private void setDirty(bool flag)
        {
            dirtyFlag = flag;
            saveSToolStripMenuItem.Enabled = (readOnlyFlag) ? false : flag;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
