using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnMobility.Service.Model;
using SCM.Model;

namespace ConnMobility.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/troublecodespayloads")]
    public class TroubleCodesPayloadsController : Controller
    {
        private readonly PayLoadContext _context;

        public TroubleCodesPayloadsController(PayLoadContext context)
        {
            _context = context;
        }

        // GET api/troublecodespayloads
        [HttpGet]        
        // GET: TroubleCodesPayloads
        public async Task<IActionResult> GetAll()
        {
           return this.Ok(await _context.TroubleCodesPayLoad.ToListAsync());
        }

        // GET: TroubleCodesPayloads/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var troubleCodesPayload = await _context.TroubleCodesPayLoad
                .SingleOrDefaultAsync(m => m.TroubleCodeId == id);
            if (troubleCodesPayload == null)
            {
                return NotFound();
            }

            return this.Ok(troubleCodesPayload);
        }

        // GET: TroubleCodesPayloads/Create
        public IActionResult Create()
        {
            return this.Ok();
        }

        // POST: TroubleCodesPayloads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TroubleCodesPayload troubleCodesPayload)
        {
            if (ModelState.IsValid)
            {
                _context.Add(troubleCodesPayload);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetAll));
            }
            return this.Ok(troubleCodesPayload);
        }

        //// GET: TroubleCodesPayloads/Edit/5
        //[Route("Edit/{id}")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var troubleCodesPayload = await _context.TroubleCodesPayLoad.SingleOrDefaultAsync(m => m.TroubleCodeId == id);
        //    if (troubleCodesPayload == null)
        //    {
        //        return NotFound();
        //    }
        //    return this.Ok(troubleCodesPayload);
        //}

        //// POST: TroubleCodesPayloads/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Route("Edit/{id}")]
        //public async Task<IActionResult> Edit(int id, [Bind("TroubleCodeId,CarId,ErrorCode,Description,Timestamp")] TroubleCodesPayload troubleCodesPayload)
        //{
        //    if (id != troubleCodesPayload.TroubleCodeId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(troubleCodesPayload);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TroubleCodesPayloadExists(troubleCodesPayload.TroubleCodeId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(GetAll));
        //    }
        //    return this.Ok(troubleCodesPayload);
        //}

        //// GET: TroubleCodesPayloads/Delete/5
        //[Route("Delete/{id}")]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var troubleCodesPayload = await _context.TroubleCodesPayLoad
        //        .SingleOrDefaultAsync(m => m.TroubleCodeId == id);
        //    if (troubleCodesPayload == null)
        //    {
        //        return NotFound();
        //    }

        //    return this.Ok(troubleCodesPayload);
        //}

        //// POST: TroubleCodesPayloads/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var troubleCodesPayload = await _context.TroubleCodesPayLoad.SingleOrDefaultAsync(m => m.TroubleCodeId == id);
        //    _context.TroubleCodesPayLoad.Remove(troubleCodesPayload);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(GetAll));
        //}

        private bool TroubleCodesPayloadExists(int id)
        {
            return _context.TroubleCodesPayLoad.Any(e => e.TroubleCodeId == id);
        }
    }
}
