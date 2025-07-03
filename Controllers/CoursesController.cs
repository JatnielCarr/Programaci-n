using Microsoft.AspNetCore.Mvc;
using Core.Domain;
using Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace APIPROYECT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/courses
        [HttpPost]
        public IActionResult CreateCourse([FromBody] CourseCreateDto dto)
        {
            try
            {
                var course = new Course(dto.Title, dto.Description);
                _context.Courses.Add(course);
                _context.SaveChanges();
                return Ok(course);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateCourse: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/courses/{id}
        [HttpGet("{id}")]
        public IActionResult GetCourse(Guid id)
        {
            try
            {
                var course = _context.Courses
                    .Include(c => c.Modules)
                    .Include(c => c.CourseInstructors).ThenInclude(ci => ci.Instructor)
                    .FirstOrDefault(c => c.Id == id);
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                return Ok(new {
                    course.Id,
                    course.Title,
                    course.Description,
                    course.IsPublished,
                    Modules = course.Modules,
                    Instructors = course.CourseInstructors.Select(ci => ci.Instructor)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCourse: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/courses/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCourse(Guid id, [FromBody] CourseUpdateDto dto)
        {
            try
            {
                var course = _context.Courses.FirstOrDefault(c => c.Id == id);
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                course.SetTitle(dto.Title);
                course.SetDescription(dto.Description);
                _context.SaveChanges();
                return Ok(course);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateCourse: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/courses/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(Guid id)
        {
            try
            {
                var course = _context.Courses.FirstOrDefault(c => c.Id == id);
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                _context.Courses.Remove(course);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteCourse: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/courses/{courseId}/publish
        [HttpPost("{courseId}/publish")]
        public IActionResult PublishCourse(Guid courseId)
        {
            var course = _context.Courses.Include(c => c.CourseInstructors).FirstOrDefault(c => c.Id == courseId);
            if (course == null) return NotFound();
            if (course.CourseInstructors == null || !course.CourseInstructors.Any())
                return BadRequest(new { message = "No se puede publicar un curso sin instructores asignados." });
            course.Publish();
            _context.SaveChanges();
            return Ok(course);
        }

        // POST: api/courses/{courseId}/modules
        [HttpPost("{courseId}/modules")]
        public IActionResult AddModule(Guid courseId, [FromBody] ModuleCreateDto dto)
        {
            try
            {
                var course = _context.Courses.Include(c => c.Modules).FirstOrDefault(c => c.Id == courseId);
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                var module = new Module(dto.Title);
                try { course.AddModule(module); } catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
                _context.Modules.Add(module);
                _context.SaveChanges();
                return Ok(module);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddModule: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/courses/{courseId}/modules
        [HttpGet("{courseId}/modules")]
        public IActionResult GetModules(Guid courseId)
        {
            try
            {
                var course = _context.Courses.Include(c => c.Modules).FirstOrDefault(c => c.Id == courseId);
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                return Ok(course.Modules);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetModules: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/modules/{moduleId}
        [HttpPut("modules/{moduleId}")]
        public IActionResult UpdateModule(Guid moduleId, [FromBody] ModuleUpdateDto dto)
        {
            try
            {
                var course = _context.Courses.Include(c => c.Modules).FirstOrDefault(c => c.Modules.Any(m => m.Id == moduleId));
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                try { course.UpdateModule(moduleId, dto.Title); } catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateModule: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/modules/{moduleId}
        [HttpDelete("modules/{moduleId}")]
        public IActionResult DeleteModule(Guid moduleId)
        {
            try
            {
                var course = _context.Courses.Include(c => c.Modules).FirstOrDefault(c => c.Modules.Any(m => m.Id == moduleId));
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                try { course.RemoveModule(moduleId); } catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
                var module = _context.Modules.FirstOrDefault(m => m.Id == moduleId);
                if (module != null) _context.Modules.Remove(module);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteModule: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/modules/{moduleId}/lessons
        [HttpPost("modules/{moduleId}/lessons")]
        public IActionResult AddLesson(Guid moduleId, [FromBody] LessonCreateDto dto)
        {
            try
            {
                var module = _context.Modules.Include(m => m.Lessons).FirstOrDefault(m => m.Id == moduleId);
                if (module == null) return NotFound(new { message = "Módulo no encontrado" });
                var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Id == moduleId));
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                var lesson = new Lesson(dto.Title, dto.Content);
                try { module.AddLesson(lesson); } catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
                _context.Lessons.Add(lesson);
                _context.SaveChanges();
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddLesson: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/modules/{moduleId}/lessons
        [HttpGet("modules/{moduleId}/lessons")]
        public IActionResult GetLessons(Guid moduleId)
        {
            try
            {
                var module = _context.Modules.Include(m => m.Lessons).FirstOrDefault(m => m.Id == moduleId);
                if (module == null) return NotFound(new { message = "Módulo no encontrado" });
                return Ok(module.Lessons);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetLessons: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/lessons/{lessonId}
        [HttpPut("lessons/{lessonId}")]
        public IActionResult UpdateLesson(Guid lessonId, [FromBody] LessonUpdateDto dto)
        {
            try
            {
                var module = _context.Modules.Include(m => m.Lessons).FirstOrDefault(m => m.Lessons.Any(l => l.Id == lessonId));
                if (module == null) return NotFound(new { message = "Módulo no encontrado" });
                var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Id == module.Id));
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                var lesson = module.Lessons.FirstOrDefault(l => l.Id == lessonId);
                if (lesson == null) return NotFound(new { message = "Lección no encontrada" });
                lesson.SetTitle(dto.Title);
                lesson.SetContent(dto.Content);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateLesson: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/lessons/{lessonId}
        [HttpDelete("lessons/{lessonId}")]
        public IActionResult DeleteLesson(Guid lessonId)
        {
            try
            {
                var module = _context.Modules.Include(m => m.Lessons).FirstOrDefault(m => m.Lessons.Any(l => l.Id == lessonId));
                if (module == null) return NotFound(new { message = "Módulo no encontrado" });
                var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Id == module.Id));
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                var lesson = module.Lessons.FirstOrDefault(l => l.Id == lessonId);
                if (lesson == null) return NotFound(new { message = "Lección no encontrada" });
                module.RemoveLesson(lessonId);
                _context.Lessons.Remove(lesson);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteLesson: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/courses/{courseId}/instructors
        [HttpPost("{courseId}/instructors")]
        public IActionResult AddInstructor(Guid courseId, [FromBody] InstructorCreateDto dto)
        {
            try
            {
                var course = _context.Courses.Include(c => c.CourseInstructors).ThenInclude(ci => ci.Instructor).FirstOrDefault(c => c.Id == courseId);
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                var instructorExistente = _context.Instructors.FirstOrDefault(i => i.Name == dto.Name);
                if (instructorExistente != null)
                {
                    if (course.CourseInstructors.Any(ci => ci.Instructor.Name == dto.Name))
                        return BadRequest(new { message = "No se pueden agregar instructores con nombre repetido." });
                }
                else
                {
                    instructorExistente = new Instructor(dto.Name);
                    _context.Instructors.Add(instructorExistente);
                    _context.SaveChanges();
                }
                try { course.AddInstructor(instructorExistente); } catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
                _context.CourseInstructors.Add(new Core.Domain.CourseInstructor { CourseId = course.Id, InstructorId = instructorExistente.Id, Instructor = instructorExistente });
                _context.SaveChanges();
                return Ok(instructorExistente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddInstructor: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/courses/{courseId}/instructors/{instructorId}
        [HttpDelete("{courseId}/instructors/{instructorId}")]
        public IActionResult DeleteInstructor(Guid courseId, Guid instructorId)
        {
            try
            {
                var course = _context.Courses.Include(c => c.CourseInstructors).FirstOrDefault(c => c.Id == courseId);
                if (course == null) return NotFound(new { message = "Curso no encontrado" });
                if (course.IsPublished) return BadRequest(new { message = "No se pueden modificar cursos publicados." });
                try { course.RemoveInstructor(instructorId); } catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
                var ci = _context.CourseInstructors.FirstOrDefault(x => x.CourseId == courseId && x.InstructorId == instructorId);
                if (ci != null) _context.CourseInstructors.Remove(ci);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteInstructor: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("modules/{moduleId}/assign-instructor")]
        public IActionResult AssignInstructorToModule(Guid moduleId, [FromBody] InstructorCreateDto dto)
        {
            try
            {
                var module = _context.Modules.Include(m => m.Instructor).FirstOrDefault(m => m.Id == moduleId);
                if (module == null) return NotFound(new { message = "Módulo no encontrado" });
                var instructor = _context.Instructors.FirstOrDefault(i => i.Name == dto.Name);
                if (instructor == null) return NotFound(new { message = "Instructor no encontrado" });
                module.AssignInstructor(instructor);
                _context.SaveChanges();
                return Ok(module);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("modules/{moduleId}/remove-instructor")]
        public IActionResult RemoveInstructorFromModule(Guid moduleId)
        {
            try
            {
                var module = _context.Modules.Include(m => m.Instructor).FirstOrDefault(m => m.Id == moduleId);
                if (module == null) return NotFound(new { message = "Módulo no encontrado" });
                module.RemoveInstructor();
                _context.SaveChanges();
                return Ok(module);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("modules/{moduleId}/publish")]
        public IActionResult PublishModule(Guid moduleId)
        {
            try
            {
                var module = _context.Modules.Include(m => m.Instructor).FirstOrDefault(m => m.Id == moduleId);
                if (module == null) return NotFound(new { message = "Módulo no encontrado" });
                module.Publish();
                _context.SaveChanges();
                return Ok(module);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // DTOs para requests
    public class CourseCreateDto { public string Title { get; set; } public string Description { get; set; } }
    public class CourseUpdateDto { public string Title { get; set; } public string Description { get; set; } }
    public class ModuleCreateDto { public string Title { get; set; } }
    public class ModuleUpdateDto { public string Title { get; set; } }
    public class LessonCreateDto { public string Title { get; set; } public string Content { get; set; } }
    public class LessonUpdateDto { public string Title { get; set; } public string Content { get; set; } }
    public class InstructorCreateDto { public string Name { get; set; } }
} 