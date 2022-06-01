using System.Drawing;
using System.Drawing.Text;
using Eastern_Arabic_numerals_Convertor;
using PdfSharp.Pdf;

#pragma warning disable CA1416
#pragma warning disable CS8618

namespace Writer;

public class WriterToFile
{
    private Bitmap TemplateImage { get; set; }
    private List<Bitmap> Imgs { get; set; }

    private string pic_link;
    private List<string> Dirs { get; set; }
    private Font _amiri;
    private string Dir { get; set; }

    private void IntializeFont()
    {
        var collection = new PrivateFontCollection();
        collection.AddFontFile(Path.Combine(Dir, "bold.ttf"));
        var fontFamily = new FontFamily("Amiri", collection);
        _amiri = new Font(fontFamily, 35);
    }

    public WriterToFile(string fileName, string workingdir)
    {
        Dir = workingdir;
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
        // var objLoc = Path.Combine(Dir, "obj.png");
        // var img_loc = Path.Combine(Dir, "img2.png");
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

    private void Write(List<int> list, int j)
    {
        var end = j + 2;
        int pics = 2;
        for (int i = j; i <= end && i < list.Count; i++)
        {
            WriteIt(list[i]);
        }

        var x = new Bitmap(TemplateImage.Width + 250, (TemplateImage.Height * (pics + 2)));
        using (Graphics g = Graphics.FromImage(x))
        {
            if (Imgs.Count > 0) g.DrawImage(Imgs[0], 0, 0);
            var h = TemplateImage.Height + 40;


            for (var i = 1; i < Imgs.Count; i++)
            {
                g.DrawImage(Imgs[i], 0, h);
                h += h + 40;
            }
        }


        var file = @"imgs\" + j + ".png";
        if(Imgs.Count!= 0)
        x.Save(file);
        Imgs = new List<Bitmap>();
        Dirs.Add(file);
    }


    private List<int> CreateList(int start, int middle, int end, int counter)
    {
        var l = new List<int>();
        while (counter != 0)
        {
            l.Add(start++);
            l.Add(middle++);
            l.Add(end++);
            counter--;
        }

        return l;
    }


    private List<string> _Write(int start, int middle, int end, int counter)
    {
        var imgdir = Path.Combine(Dir, "imgs");
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

        counter += counter % 3;
        var l = CreateList(start, middle, end, counter);
        int j = 0;


        for (int i = 0; i < l.Count; )
        {
            
            Write(l, i);
            i += 3;
        }
        
        
        
        // foreach ( var w in l)
        // {
        // }

        // end += start % 3;
        // for (int i = start; i <= end; i += 3)
        // {
        //     Write(i);
        //     Imgs = new List<Bitmap>();
        // }


        return Dirs;
    }

    public void Write(int start, int middle, int end, int counter)
    {
        var outter = Path.Combine(Dir, "out.pdf");
        if (File.Exists(outter))
            File.Delete(outter);

        var temp = Path.Combine(Dir, "file.pdf");
        var k = _Write(start, middle, end, counter);
        var x = new PdfDocument(temp);
        foreach (var i in k)
        {
            PdfHelper.Instance.SaveImageAsPdf(i, x, 1000);
        }

        TemplateImage.Dispose();
        x.Close();
        x.Dispose();

        x.Save(outter);
    }
}