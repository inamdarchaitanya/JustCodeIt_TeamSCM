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
    [Route("api/realtimepayloads")]
    public class RealTimePayloadsController : Controller
    {
        private readonly PayLoadContext _context;

        public RealTimePayloadsController(PayLoadContext context)
        {
            _context = context;
        }

        // GET: RealTimePayloads
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return this.Ok(await _context.RealTimePayLoad.ToListAsync());
        }

        // GET: RealTimePayloads/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var realTimePayload = await _context.RealTimePayLoad
                .SingleOrDefaultAsync(m => m.RealTimePayLoadId == id);
            if (realTimePayload == null)
            {
                return NotFound();
            }

            return this.Ok(realTimePayload);
        }

        // GET: RealTimePayloads/Create
        public IActionResult Create()
        {
            return this.Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RealTimePayload realTimePayload)
        {
            if (ModelState.IsValid)
            {
                _context.Add(realTimePayload);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetAll));
            }
            return this.Ok(realTimePayload);
        }
        //// GET: RealTimePayloads/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var realTimePayload = await _context.RealTimePayLoad.SingleOrDefaultAsync(m => m.RealTimePayLoadId == id);
        //    if (realTimePayload == null)
        //    {
        //        return NotFound();
        //    }
        //    return this.Ok(realTimePayload);
        //}

        //// POST: RealTimePayloads/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("RealTimePayLoadId,CarId,Speed,Rpm,FuelLevel,EngineOilTemperature,CoolantTemperature,Timestamp")] RealTimePayload realTimePayload)
        //{
        //    if (id != realTimePayload.RealTimePayLoadId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(realTimePayload);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!RealTimePayloadExists(realTimePayload.RealTimePayLoadId))
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
        //    return this.Ok(realTimePayload);
        //}

        //// GET: RealTimePayloads/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var realTimePayload = await _context.RealTimePayLoad
        //        .SingleOrDefaultAsync(m => m.RealTimePayLoadId == id);
        //    if (realTimePayload == null)
        //    {
        //        return NotFound();
        //    }

        //    return this.Ok(realTimePayload);
        //}

        //// POST: RealTimePayloads/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var realTimePayload = await _context.RealTimePayLoad.SingleOrDefaultAsync(m => m.RealTimePayLoadId == id);
        //    _context.RealTimePayLoad.Remove(realTimePayload);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(GetAll));
        //}

        private bool RealTimePayloadExists(int id)
        {
            return _context.RealTimePayLoad.Any(e => e.RealTimePayLoadId == id);
        }
    }
}
