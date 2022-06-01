using System.Diagnostics;
using SO = System.IO.File;
using Microsoft.AspNetCore.Mvc;
using WebDemo.Models;
using WebDemo.ViewModels;
using Writer;
#pragma warning disable CS0618
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebDemo.Controllers;

public class HomeController : Controller
{
    private readonly IHostingEnvironment
        _hostEnvironment; //diference is here : IHostingEnvironment  vs I*Web*HostEnvironment 

    public HomeController(IHostingEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Index(PostViewModel model)
    {
        string webRootPath = _hostEnvironment.WebRootPath;

        var save = Path.Combine(webRootPath, "img2.png");

        if (model.Image != null && model.Image.Length != 0)
        {
            using (var f = new FileStream(save, FileMode.Create))
            {
                await model.Image.CopyToAsync(f);
                f.Close();
                WriterToFile i = new WriterToFile(save, webRootPath);
                i.Write(model.Start, model.Middle, model.End, model.Counter);
                f.Close();
                await f.DisposeAsync();
            }
        }
        else
        {
            var savea = Path.Combine(webRootPath, "obj.png");
            var i = new WriterToFile(savea, webRootPath);
            i.Write(model.Start, model.Middle, model.End, model.Counter);
        }

//        SO.Delete(save);

        return Redirect("/out.pdf");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}