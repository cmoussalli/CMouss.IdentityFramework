using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMouss.IdentityFramework;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework.Tests
{
    public  class CustomDBModel:IDFDBContext
    {

        public DbSet<CustomEntity1> CustomEntity1s { get; set; }

    }
}
