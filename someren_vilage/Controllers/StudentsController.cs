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
}