using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamWork_26._03._25_.Models
{
    public enum UserRole
    {
        Patient,
        Doctor
    }
   public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}

}
