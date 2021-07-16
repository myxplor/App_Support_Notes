using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesAPI.Data;
using NotesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace NotesAPI.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : Controller {

        private readonly ApplicationDbContext _db;

        public NotesController(ApplicationDbContext db) {
            _db = db;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(List<Notes>))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNote([FromBody] Notes notes) {
            if (notes == null) {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                _db.Notes.Add(notes);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError("", $"Something went wrong while saving the record");
                _db.Dispose();
                return StatusCode(500, ModelState);
            }
            Save();

            var obj = CreatedAtRoute("GetNotes", new { noteId = notes.Id }, notes);

            //return obj;
            return Ok();
        }

        //GET METHODS
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Notes>))]
        public ICollection<Notes> GetNotes() {
            return _db.Notes.OrderBy(a => a.Created).ToList();
        }


        [NonAction]
        private bool Save() {
            return _db.SaveChanges() >= 0 ? true : false;
        }

    }
}
