namespace Writer;

using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

public sealed class PdfHelper
{
    private PdfHelper()
    {
    }

    public static PdfHelper Instance { get; } = new PdfHelper();

    public void SaveImageAsPdf(string imageFileName, PdfDocument x, int width = 1117)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var document = x )
        {
            PdfPage page = document.AddPage();
            using (XImage img = XImage.FromFile(imageFileName))
            {
                // Calculate new height to keep image ratio
                var height = (int) (((double) width / (double) img.PixelWidth) * img.PixelHeight);

                // Change PDF Page size to match image
                page.Width = width;
                page.Height = height;

                XGraphics gfx = XGraphics.FromPdfPage(page);
                gfx.DrawImage(img, 0, 0, width, height);
            }

            // document.Save("file.pdf");
        }
    }
}