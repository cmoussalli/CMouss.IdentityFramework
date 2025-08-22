using CMouss.IdentityFramework;
using Microsoft.EntityFrameworkCore;

namespace BlazorUIDemo.Models
{
    public class DemoDBContext:IDFDBContext
    {

        public DbSet<Student> Students { get; set; }


    }
}
