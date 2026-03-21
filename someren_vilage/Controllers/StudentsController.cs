using Microsoft.AspNetCore.Mvc;

namespace someren_vilage.Controllers;

public class StudentsController : Controller
{
    private readonly someren_vilage.Repositorie.IStudentRepository _repo;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(someren_vilage.Repositorie.IStudentRepository repo, ILogger<StudentsController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var students = _repo.GetAll();
        return View(students);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(someren_vilage.Models.Student student)
    {
        if (ModelState.IsValid)
        {
            _repo.Add(student);
            return RedirectToAction("Index");
        }

        return View(student);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        try
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student");
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var student = _repo.GetById(id);
        if (student == null) return NotFound();

        return View(student);
    }

    [HttpPost]
    public IActionResult Edit(someren_vilage.Models.Student student)
    {
        if (ModelState.IsValid)
        {
            _repo.Update(student);
            return RedirectToAction("Index");
        }

        return View(student);
    }

}