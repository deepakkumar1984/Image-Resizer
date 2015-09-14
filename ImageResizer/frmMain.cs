using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ImageResizer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void txtFilePath_DoubleClick(object sender, EventArgs e)
        {
            DialogResult diagResult = fileDialog.ShowDialog();
            if (diagResult.ToString().ToUpper() == "OK")
            {
                txtFilePath.Text = fileDialog.FileName;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StringBuilder messageBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                MessageBox.Show("Please select a file!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOutput.Text))
            {
                MessageBox.Show("Please select output folder!");
                return;
            }

            if (string.IsNullOrWhiteSpace(ddlFileType.Text))
            {
                MessageBox.Show("Please select file type!");
                return;
            }


            Image sourceImage = Image.FromFile(txtFilePath.Text);

            List<ImageSize> imageSizeList = DeserializeObject(@"D:\PersonalWork\Tools\ImageResizer\ImageResizer\ImageSize.xml");
            var filteredList = imageSizeList.Where(x => (x.Type == ddlFileType.Text)).ToList();
            foreach (var item in filteredList)
            {

                System.Drawing.Size size = new Size(item.Width, item.Height);
                Image resultImg = ResizeImage(sourceImage, size);
                string savePath = "";

                if (string.IsNullOrWhiteSpace(item.FolderName))
                {
                    savePath = string.Format("{0}\\{1}_{2}X{3}.png", txtOutput.Text, txtFileNameStarts.Text, item.Width, item.Height);
                }
                else
                {
                    savePath = string.Format("{0}\\{4}\\{1}_{2}X{3}.png", txtOutput.Text, txtFileNameStarts.Text, item.Width, item.Height, item.FolderName);
                    if (!Directory.Exists(string.Format("{0}\\{1}", txtOutput.Text, item.FolderName)))
                    {
                        Directory.CreateDirectory(string.Format("{0}\\{1}", txtOutput.Text, item.FolderName));
                    }
                }

                try
                {
                    if (ddlFileType.Text == "Screen")
                    {
                        resultImg.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else
                    {
                        resultImg.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    messageBuilder.AppendLine(string.Format("{0} - Created", savePath));
                }
                catch (Exception ex)
                {
                    messageBuilder.AppendLine(string.Format("{0} - Failed ({1})", savePath, ex.Message));
                }
            }

            txtMessage.Text = messageBuilder.ToString();
        }

        private List<ImageSize> DeserializeObject(string filename)
        {
            XmlSerializer serializer = new
            XmlSerializer(typeof(List<ImageSize>));

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            // Declare an object variable of the type to be deserialized.
            List<ImageSize> i;

            // Use the Deserialize method to restore the object's state.
            i = (List<ImageSize>)serializer.Deserialize(reader);
            fs.Close();

            return i;
        }

        private static Image ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            Bitmap b = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
            g.Dispose();

            return (Image)b;
        }

        private void txtOutput_DoubleClick(object sender, EventArgs e)
        {
            DialogResult diagResult = folderBrowser.ShowDialog();
            if (diagResult.ToString().ToUpper() == "OK")
            {
                txtOutput.Text = fileDialog.FileName;
            }
        }

        private void ddlFileType_SelectedValueChanged(object sender, EventArgs e)
        {
            txtFileNameStarts.Text = ddlFileType.Text;
        }
    }
}
