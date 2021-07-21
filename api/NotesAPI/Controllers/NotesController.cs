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

            return Ok();
        }

        //GET METHODS
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Notes>))]
        public ICollection<Notes> GetNotes() {
            return _db.Notes.OrderBy(a => a.Created).ToList();
        }

        [HttpPatch("{NoteId:int}", Name = "UpdateNote")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNote(Notes note) {
            if (!NoteExists(note.Id)) {
                return NotFound();
            }
            if (note == null) {
                return BadRequest();
            }
            try {
                _db.Notes.Update(note);
            }
            catch (Exception e) {
                ModelState.AddModelError("", $"Something went wrong while removing the record");
                _db.Dispose();
                return StatusCode(500, ModelState);
            }
            Save();
            return NoContent();
        }

        [HttpDelete("{NoteId:int}", Name = "DeleteNote")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteExpense(int NoteId) {
            if (!NoteExists(NoteId)) {
                return NotFound();
            }
            var expenseToRemove = _db.Notes.SingleOrDefault(x => x.Id == NoteId);
            try {
                _db.Notes.Remove(expenseToRemove);
            }
            catch (Exception e) {
                ModelState.AddModelError("", $"Something went wrong while removing the record");
                _db.Dispose();
                return StatusCode(500, ModelState);
            }
            Save();
            return NoContent();
        }

        [NonAction]
        private bool Save() {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        [NonAction]
        private bool NoteExists(int noteId) {
            return _db.Notes.Any(a => a.Id == noteId);
        }

    }
}
