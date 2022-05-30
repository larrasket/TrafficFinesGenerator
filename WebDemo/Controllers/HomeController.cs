using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server.Features;
using SO = System.IO.File;
using Microsoft.AspNetCore.Mvc;
using WebDemo.Models;
using WebDemo.ViewModels;
using WriterToTheFile;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebDemo.Controllers;

public class HomeController : Controller
{
    private readonly IHostingEnvironment
        _HostEnvironment; //diference is here : IHostingEnvironment  vs I*Web*HostEnvironment 

    public HomeController(IHostingEnvironment HostEnvironment)
    {
        _HostEnvironment = HostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Index(PostViewModel model)
    {
        string webRootPath = _HostEnvironment.WebRootPath;

        var save = Path.Combine(webRootPath, "img2.png");

        if (model.Image != null && model.Image.Length != 0)
        {
            if (SO.Exists(save))
            {
                // SO.Delete(save);
            }

            using (var f = new FileStream(save, FileMode.Create))
            {
                await model.Image.CopyToAsync(f);
                f.Close();
                WriterToFile i = new WriterToFile(save, webRootPath);
                i.Write(model.Start, model.End);
                f.Close();
                await f.DisposeAsync();
            }
        }
        else
        {
            var savea = Path.Combine(webRootPath, "obj.png");
            var i = new WriterToFile(savea, webRootPath);
            i.Write(model.Start, model.End);
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