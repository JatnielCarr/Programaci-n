using Microsoft.AspNetCore.Mvc;
using Core.Domain;
using Core.Infrastructure.Data;

namespace APIPROYECT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var course = new Course(dto.Title, dto.Description);
            _context.Courses.Add(course);
            _context.SaveChanges();
            return Ok(course);
        }

        // GET: api/courses/{id}
        [HttpGet("{id}")]
        public IActionResult GetCourse(Guid id)
        {
            var course = _context.Courses
                .Where(c => c.Id == id)
                .Select(c => new {
                    c.Id,
                    c.Title,
                    c.Description,
                    c.IsPublished,
                    Modules = c.Modules,
                    Instructors = c.CourseInstructors.Select(ci => ci.Instructor)
                })
                .FirstOrDefault();
            if (course == null) return NotFound();
            return Ok(course);
        }

        // PUT: api/courses/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCourse(Guid id, [FromBody] CourseUpdateDto dto)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();
            course.SetTitle(dto.Title);
            course.SetDescription(dto.Description);
            _context.SaveChanges();
            return Ok(course);
        }

        // DELETE: api/courses/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(Guid id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return NoContent();
        }

        // POST: api/courses/{courseId}/publish
        [HttpPost("{courseId}/publish")]
        public IActionResult PublishCourse(Guid courseId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return NotFound();
            course.Publish();
            _context.SaveChanges();
            return Ok(course);
        }

        // POST: api/courses/{courseId}/modules
        [HttpPost("{courseId}/modules")]
        public IActionResult AddModule(Guid courseId, [FromBody] ModuleCreateDto dto)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return NotFound();
            var module = new Module(dto.Title);
            try { course.AddModule(module); } catch (Exception ex) { return BadRequest(ex.Message); }
            _context.Modules.Add(module);
            _context.SaveChanges();
            return Ok(module);
        }

        // GET: api/courses/{courseId}/modules
        [HttpGet("{courseId}/modules")]
        public IActionResult GetModules(Guid courseId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return NotFound();
            return Ok(course.Modules);
        }

        // PUT: api/modules/{moduleId}
        [HttpPut("/api/modules/{moduleId}")]
        public IActionResult UpdateModule(Guid moduleId, [FromBody] ModuleUpdateDto dto)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Id == moduleId));
            if (course == null) return NotFound();
            try { course.UpdateModule(moduleId, dto.Title); } catch (Exception ex) { return BadRequest(ex.Message); }
            _context.SaveChanges();
            return Ok();
        }

        // DELETE: api/modules/{moduleId}
        [HttpDelete("/api/modules/{moduleId}")]
        public IActionResult DeleteModule(Guid moduleId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Id == moduleId));
            if (course == null) return NotFound();
            try { course.RemoveModule(moduleId); } catch (Exception ex) { return BadRequest(ex.Message); }
            var module = _context.Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module != null) _context.Modules.Remove(module);
            _context.SaveChanges();
            return NoContent();
        }

        // POST: api/modules/{moduleId}/lessons
        [HttpPost("/api/modules/{moduleId}/lessons")]
        public IActionResult AddLesson(Guid moduleId, [FromBody] LessonCreateDto dto)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Id == moduleId));
            if (course == null) return NotFound();
            var lesson = new Lesson(dto.Title, dto.Content);
            try { course.AddLessonToModule(moduleId, lesson); } catch (Exception ex) { return BadRequest(ex.Message); }
            _context.Lessons.Add(lesson);
            _context.SaveChanges();
            return Ok(lesson);
        }

        // GET: api/modules/{moduleId}/lessons
        [HttpGet("/api/modules/{moduleId}/lessons")]
        public IActionResult GetLessons(Guid moduleId)
        {
            var module = _context.Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return NotFound();
            return Ok(module.Lessons);
        }

        // PUT: api/lessons/{lessonId}
        [HttpPut("/api/lessons/{lessonId}")]
        public IActionResult UpdateLesson(Guid lessonId, [FromBody] LessonUpdateDto dto)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Lessons.Any(l => l.Id == lessonId)));
            if (course == null) return NotFound();
            var module = course.Modules.FirstOrDefault(m => m.Lessons.Any(l => l.Id == lessonId));
            try { course.UpdateLesson(module.Id, lessonId, dto.Title, dto.Content); } catch (Exception ex) { return BadRequest(ex.Message); }
            _context.SaveChanges();
            return Ok();
        }

        // DELETE: api/lessons/{lessonId}
        [HttpDelete("/api/lessons/{lessonId}")]
        public IActionResult DeleteLesson(Guid lessonId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Modules.Any(m => m.Lessons.Any(l => l.Id == lessonId)));
            if (course == null) return NotFound();
            var module = course.Modules.FirstOrDefault(m => m.Lessons.Any(l => l.Id == lessonId));
            try { course.RemoveLesson(module.Id, lessonId); } catch (Exception ex) { return BadRequest(ex.Message); }
            var lesson = _context.Lessons.FirstOrDefault(l => l.Id == lessonId);
            if (lesson != null) _context.Lessons.Remove(lesson);
            _context.SaveChanges();
            return NoContent();
        }

        // POST: api/courses/{courseId}/instructors
        [HttpPost("{courseId}/instructors")]
        public IActionResult AddInstructor(Guid courseId, [FromBody] InstructorCreateDto dto)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return NotFound();
            var instructor = _context.Instructors.FirstOrDefault(i => i.Name == dto.Name);
            if (instructor == null)
            {
                instructor = new Instructor(dto.Name);
                _context.Instructors.Add(instructor);
                _context.SaveChanges();
            }
            try { course.AddInstructor(instructor); } catch (Exception ex) { return BadRequest(ex.Message); }
            _context.CourseInstructors.Add(new Core.Domain.CourseInstructor { CourseId = course.Id, InstructorId = instructor.Id, Instructor = instructor });
            _context.SaveChanges();
            return Ok(instructor);
        }

        // DELETE: api/courses/{courseId}/instructors/{instructorId}
        [HttpDelete("{courseId}/instructors/{instructorId}")]
        public IActionResult DeleteInstructor(Guid courseId, Guid instructorId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return NotFound();
            try { course.RemoveInstructor(instructorId); } catch (Exception ex) { return BadRequest(ex.Message); }
            var ci = _context.CourseInstructors.FirstOrDefault(x => x.CourseId == courseId && x.InstructorId == instructorId);
            if (ci != null) _context.CourseInstructors.Remove(ci);
            _context.SaveChanges();
            return NoContent();
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