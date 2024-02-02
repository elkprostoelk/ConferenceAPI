using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceAPI.DataAccess
{
    public class ConferenceApiDbContext : DbContext
    {
        public ConferenceApiDbContext(DbContextOptions<ConferenceApiDbContext> options) : base(options) { }
    }
}
