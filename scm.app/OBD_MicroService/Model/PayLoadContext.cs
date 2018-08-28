using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Model;

namespace ConnMobility.Service.Model
{
    public class PayLoadContext : DbContext
    {
        public PayLoadContext(DbContextOptions<PayLoadContext> options)
            : base(options)
        { }

        public DbSet<RealTimePayload> RealTimePayLoad { get; set; }
        public DbSet<TroubleCodesPayload> TroubleCodesPayLoad { get; set; }
    }
}
