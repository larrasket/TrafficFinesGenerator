using System.Drawing;
using System.Drawing.Text;
using Eastern_Arabic_numerals_Convertor;
using PdfSharp.Pdf;
using Writer;

#pragma warning disable CS8618

namespace WriterToTheFile;

public class WriterToFile
{
    private Bitmap TemplateImage { get; set; }
    private List<Bitmap> Imgs { get; set; }

    private string pic_link;
    private List<string> Dirs { get; set; }
    private Font _amiri;
    public string dir { get; set; }

    private void IntializeFont()
    {
        var collection = new PrivateFontCollection();
        collection.AddFontFile(Path.Combine(dir, "bold.ttf"));
        var fontFamily = new FontFamily("Amiri", collection);
        _amiri = new Font(fontFamily, 35);
    }

    public WriterToFile(string fileName, string workingdir)
    {
        dir = workingdir;
        Dirs = new List<string>();
        Imgs = new List<Bitmap>();
        pic_link = fileName;
        TemplateImage = new Bitmap(@fileName);
        IntializeFont();
    }

    public WriterToFile()
    {
        Dirs = new List<string>();
        TemplateImage = new Bitmap(@"obj.png");
        Imgs = new List<Bitmap>();
        IntializeFont();
    }

    private void WriteIt(long value)
    {
        var obj_loc = Path.Combine(dir, "obj.png");
        var img_loc = Path.Combine(dir, "img2.png");
        TemplateImage = new Bitmap(pic_link);
        var t = ArabicIndic.ConvertToIndic(value);
        var firstLocation = new PointF(500, 50f);
        var secondLocation = new PointF(138, 50f);
        using var graphics = Graphics.FromImage(TemplateImage);
        graphics.DrawString(t, _amiri, Brushes.Red, firstLocation);
        graphics.DrawString(t, _amiri, Brushes.Red, secondLocation);
        // TemplateImage.Save(@"out.png");
        Imgs.Add(TemplateImage);
    }

    private void Write(int start)
    {
        var end = start + 2;
        int pics = end - start;
        for (int i = start; i <= end; i++)
        {
            WriteIt(i);
        }

        var x = new Bitmap(TemplateImage.Width + 250, (TemplateImage.Height * (pics + 2)));
        using (Graphics g = Graphics.FromImage(x))
        {
            g.DrawImage(Imgs[0], 0, 0);
            var h = TemplateImage.Height + 40;


            for (var i = 1; i < Imgs.Count; i++)
            {
                g.DrawImage(Imgs[i], 0, h);
                h += h + 40;
            }
        }

        var file = @"imgs\" + start + ".png";
        x.Save(file);
        Dirs.Add(file);
    }

    private List<string> _Write(int start, int end)
    {
        var imgdir = Path.Combine(dir, "imgs");
        if (!Directory.Exists(imgdir))
            Directory.CreateDirectory(imgdir);
        else

        {
            string[] files = Directory.GetFiles(imgdir);
            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }


        end += start % 3;

        for (int i = start; i <= end; i += 3)
        {
            Write(i);
            Imgs = new List<Bitmap>();
        }

        return Dirs;
    }

    public void Write(int start, int end)
    {
        var outter = Path.Combine(dir, "out.pdf");
        if (File.Exists(outter))
            File.Delete(outter);

        var temp = Path.Combine(dir, "file.pdf");
        var k = _Write(start, end);
        var x = new PdfDocument(temp);
        foreach (var i in k)
        {
            PdfHelper.Instance.SaveImageAsPdf(i, x, 1000);
        }


        var save = Path.Combine(dir, "img2.png");

        TemplateImage.Dispose();
        x.Close();
        x.Dispose();

        x.Save(outter);
    }
}