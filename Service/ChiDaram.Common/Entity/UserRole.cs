using System.Collections.Generic;

namespace ChiDaram.Common.Entity
{
    public class UserRole
    {
        public UserRole()
        {
            MenuCodes = new List<UserRoleMenuCode>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int UserCount { get; set; }

        public List<UserRoleMenuCode> MenuCodes { get; set; }
    }
}
