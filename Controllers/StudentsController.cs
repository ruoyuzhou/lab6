using Lab6.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab6.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab6.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get collection of Students
        /// </summary>
        /// <returns>A collection of Students</returns>
        /// <response code="200">Returns a collection of Students</response>
        /// <response code="500">Internal error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Student>>> Get()
        {
            return Ok(await _context.Students.ToListAsync());
        }

        /// <summary>
        /// Get a Student
        /// </summary>
        /// <returns>Return Student Information</returns>
        /// <response code="200">Success</response>
        /// <response code="404">If the Student is null</response>
        [HttpGet("{ID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Student>> GetById(Guid ID)
        {
            return Ok(await _context.Students.Where(c => c.ID == ID).FirstOrDefaultAsync());
        }

        /// <summary>
        /// Creates a Student
        /// </summary>
        /// <param name="student">new Student</param>
        /// <response code="201">Returns the newly created Student</response>
        /// <response code="400">If the Student is malformed</response>
        /// <response code="500">Internal error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Student>> Create([FromBody] StudentBase student)
        {
            var newly = new Student()
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Program = student.Program
            };
            _context.Students.Add(newly);
            await _context.SaveChangesAsync();
            Response.StatusCode = StatusCodes.Status201Created;
            return Ok(newly);
        }

        /// <summary>
        /// Deletes a Student
        /// </summary>
        /// <param name="ID"></param>
        /// <response code="202">Student is deleted</response>
        /// <response code="400">If the id is malformed</response>
        /// <response code="500">Internal error</response>
        [HttpDelete("{ID}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid ID)
        {
            var student = await _context.Students.SingleAsync(c => c.ID == ID);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            Response.StatusCode = StatusCodes.Status202Accepted;
            return Ok();
        }

        /// <summary>
        /// Upserts a Student
        /// </summary>
        /// <response code="200">Returns the updated Student</response>
        /// <response code="201">Returns the newly created Student</response>
        /// <response code="400">If the Student or id is malformed</response>
        /// <response code="500">Internal error</response>
        [HttpPut("{ID}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid ID, StudentBase info)
        {
            var student = await _context.Students.SingleOrDefaultAsync(c => c.ID == ID);
            Action copy = () =>
            {
                student.FirstName = info.FirstName;
                student.LastName = info.LastName;
                student.Program = info.Program;
            };
            if (student != null)
            {
                copy();
                _context.Students.Update(student);
                Response.StatusCode = StatusCodes.Status200OK;
            }
            else
            {
                student = new Student();
                copy();
                _context.Students.Add(student);
                Response.StatusCode = StatusCodes.Status201Created;
            }
            
            await _context.SaveChangesAsync();
            return Ok(student);
        }

    }
}
